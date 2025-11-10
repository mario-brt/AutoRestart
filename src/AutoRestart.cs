using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Config;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoRestart;

public class AutoRestartConfig : BasePluginConfig
{
    [JsonPropertyName("AutoRestartEnabled")]
    public bool AutoRestartEnabled { get; set; } = true;

    [JsonPropertyName("EnableManualRestart")]
    public bool EnableManualRestart { get; set; } = true;

    [JsonPropertyName("Flag")]
    public string Flag { get; set; } = "@css/root";

    [JsonPropertyName("AutoRestartTime")]
    public string AutoRestartTime { get; set; } = "06:00:00";

    [JsonPropertyName("TimeZone")]
    public string TimeZone { get; set; } = "Asia/Seoul";

    [JsonPropertyName("WarningTimes")]
    public int[] WarningTimes { get; set; } = [300, 180, 60, 30, 10]; // Warning times (seconds)

    [JsonPropertyName("EnableWarnings")]
    public bool EnableWarnings { get; set; } = true;

    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "[AutoRestart]";

    [JsonPropertyName("Language")]
    public string Language { get; set; } = "ko";
}

public class AutoRestart : BasePlugin, IPluginConfig<AutoRestartConfig>
{
    public override string ModuleName => "AutoRestart";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "sn0wman";

    public required AutoRestartConfig Config { get; set; }
    private static IStringLocalizer? _localizer;
    private CounterStrikeSharp.API.Modules.Timers.Timer? _restartTimer;
    private readonly List<CounterStrikeSharp.API.Modules.Timers.Timer> _warningTimers = new();
    private TimeZoneInfo? _timeZone;
    private Dictionary<string, string> _translations = new();

    public void OnConfigParsed(AutoRestartConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        _localizer = Localizer;
        LoadTranslations();
        
        Console.WriteLine(GetLocalizedMessage("plugin.loaded"));

        // Timezone configuration
        try
        {
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(Config.TimeZone);
            Console.WriteLine(GetLocalizedMessage("plugin.using_timezone", Config.TimeZone));
        }
        catch (TimeZoneNotFoundException)
        {
            Console.WriteLine(GetLocalizedMessage("plugin.timezone_not_found", Config.TimeZone));
            _timeZone = TimeZoneInfo.Local;
        }

        // Register commands
        AddCommand("css_restart", "Restart the server immediately", RestartCommand);
        AddCommand("css_restartstatus", "Check restart status", RestartStatusCommand);

        if (Config.AutoRestartEnabled)
        {
            ScheduleAutoRestart();
        }
    }

    public override void Unload(bool hotReload)
    {
        // Clean up timers
        _restartTimer?.Kill();
        foreach (var timer in _warningTimers)
        {
            timer?.Kill();
        }
        _warningTimers.Clear();
        
        Console.WriteLine(GetLocalizedMessage("plugin.unloaded"));
    }

    private void LoadTranslations()
    {
        try
        {
            string langFile = Path.Combine(ModuleDirectory, "lang", $"{Config.Language}.json");
            if (!File.Exists(langFile))
            {
                // Fallback to English if default language file doesn't exist
                langFile = Path.Combine(ModuleDirectory, "lang", "en.json");
            }

            if (File.Exists(langFile))
            {
                string jsonContent = File.ReadAllText(langFile);
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                if (translations != null)
                {
                    _translations = translations;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoRestart] Failed to load translations: {ex.Message}");
        }
    }

    private string GetLocalizedMessage(string key, params object[] args)
    {
        if (_translations.TryGetValue(key, out string? message))
        {
            try
            {
                return $"{Config.Prefix} {string.Format(message, args)}";
            }
            catch
            {
                return $"{Config.Prefix} {message}";
            }
        }
        return $"{Config.Prefix} {key}";
    }

    private string GetLocalizedMessageWithoutPrefix(string key, params object[] args)
    {
        if (_translations.TryGetValue(key, out string? message))
        {
            try
            {
                return string.Format(message, args);
            }
            catch
            {
                return message;
            }
        }
        return key;
    }

    private bool HasAdminPermission(CCSPlayerController? player)
    {
        return player != null && AdminManager.PlayerHasPermissions(player, Config.Flag);
    }

    private void ScheduleAutoRestart()
    {
        if (!TimeSpan.TryParse(Config.AutoRestartTime, out TimeSpan restartTime))
        {
            Console.WriteLine(GetLocalizedMessage("plugin.invalid_restart_time", Config.AutoRestartTime));
            return;
        }

        // Convert current time to configured timezone
        DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone ?? TimeZoneInfo.Local);
        DateTime todayRestart = currentTime.Date.Add(restartTime);
        
        // If today's restart time has already passed, set it to tomorrow
        if (todayRestart <= currentTime)
        {
            todayRestart = todayRestart.AddDays(1);
        }

        TimeSpan delay = todayRestart - currentTime;
        
        Console.WriteLine(GetLocalizedMessage("restart.server_will_restart_at", 
            todayRestart.ToString("yyyy-MM-dd HH:mm:ss"), Config.TimeZone, delay.TotalHours.ToString("F1")));

        // Remove existing timers
        _restartTimer?.Kill();
        foreach (var timer in _warningTimers)
        {
            timer?.Kill();
        }
        _warningTimers.Clear();

        // Schedule warning timers
        if (Config.EnableWarnings)
        {
            ScheduleWarnings(delay);
        }

        // Schedule restart timer
        _restartTimer = AddTimer((float)delay.TotalSeconds, RestartServer);
    }

    private void ScheduleWarnings(TimeSpan totalDelay)
    {
        foreach (int warningSeconds in Config.WarningTimes)
        {
            double warningDelay = totalDelay.TotalSeconds - warningSeconds;
            
            if (warningDelay > 0)
            {
                CounterStrikeSharp.API.Modules.Timers.Timer warningTimer = AddTimer((float)warningDelay, () => SendWarning(warningSeconds));
                _warningTimers.Add(warningTimer);
            }
        }
    }

    private void SendWarning(int seconds)
    {
        string message;
        if (seconds >= 60)
        {
            int minutes = seconds / 60;
            string pluralSuffix = minutes > 1 ? "s" : "";
            message = GetLocalizedMessage("restart.server_will_restart_in_minutes", minutes, pluralSuffix);
        }
        else
        {
            string pluralSuffix = seconds > 1 ? "s" : "";
            message = GetLocalizedMessage("restart.server_will_restart_in_seconds", seconds, pluralSuffix);
        }

        Console.WriteLine(message);
        Server.PrintToChatAll($" {ChatColors.Red}{message}");
    }

    private void RestartServer()
    {
        Console.WriteLine(GetLocalizedMessage("restart.server_restarting_now"));
        Server.PrintToChatAll($" {ChatColors.Red}{GetLocalizedMessage("restart.server_restarting_now")}");
        
        // Shut down server after short delay
        AddTimer(2.0f, () =>
        {
            Server.ExecuteCommand("quit");
        });
    }

    private void RestartCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!HasAdminPermission(player))
        {
            Console.WriteLine(GetLocalizedMessage("log.no_admin_permission"));
            if (player?.IsValid == true)
            {
                player.PrintToChat($" {ChatColors.Red}{GetLocalizedMessageWithoutPrefix("command.no_permission")}");
            }
            return;
        }

        if (!Config.EnableManualRestart)
        {
            if (player?.IsValid == true)
            {
                player.PrintToChat($" {ChatColors.Red}{GetLocalizedMessageWithoutPrefix("command.manual_restart_disabled")}");
            }
            return;
        }

        Console.WriteLine(GetLocalizedMessage("command.restart_executed", player?.PlayerName ?? "Console"));
        
        if (player?.IsValid == true)
        {
            player.PrintToChat($" {ChatColors.Green}{GetLocalizedMessageWithoutPrefix("command.restarting_now")}");
        }
        
        Server.PrintToChatAll($" {ChatColors.Red}{GetLocalizedMessage("restart.server_manually_restarted")}");
        RestartServer();
    }

    private void RestartStatusCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!Config.AutoRestartEnabled)
        {
            if (player?.IsValid == true)
            {
                player.PrintToChat($" {ChatColors.Yellow}{GetLocalizedMessage("command.auto_restart_disabled")}");
            }
            return;
        }

        if (!TimeSpan.TryParse(Config.AutoRestartTime, out TimeSpan restartTime))
        {
            if (player?.IsValid == true)
            {
                player.PrintToChat($" {ChatColors.Red}{GetLocalizedMessage("command.invalid_restart_config")}");
            }
            return;
        }

        DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone ?? TimeZoneInfo.Local);
        DateTime todayRestart = currentTime.Date.Add(restartTime);
        
        if (todayRestart <= currentTime)
        {
            todayRestart = todayRestart.AddDays(1);
        }

        TimeSpan timeUntilRestart = todayRestart - currentTime;
        
        string message = GetLocalizedMessage("command.next_restart", 
            todayRestart.ToString("HH:mm:ss"), Config.TimeZone, 
            timeUntilRestart.Hours, timeUntilRestart.Minutes);
        
        if (player?.IsValid == true)
        {
            player.PrintToChat($" {ChatColors.Green}{message}");
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}