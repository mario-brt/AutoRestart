# 🔄 CS2 AutoRestart Plugin

<div align="center">

A Counter-Strike 2 server plugin that automatically restarts the server at a scheduled time with timezone support

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-v1.0.346%2B-blue)](https://github.com/roflmuffin/CounterStrikeSharp)

</div>

## 🎯 Features

- ⏰ **Scheduled Restart** - Automatically restart server at a specific time daily
- 🌏 **Timezone Support** - Configure any timezone (default: Asia/Seoul)
- 🚨 **Warning System** - Notify players before restart with customizable intervals
- 🎮 **Manual Restart** - Admin command to restart immediately
- 📊 **Status Check** - View next scheduled restart time
- 🌍 **Multi-Language** - English and Korean support
- ⚙️ **Custom Prefix** - Configurable message prefix

## 📦 Requirements

- **[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)** v1.0.346 or newer
- **.NET 8.0 Runtime**

## 🔧 Installation

### Quick Install

1. **Install CounterStrikeSharp**
   - Download from [CounterStrikeSharp Releases](https://github.com/roflmuffin/CounterStrikeSharp/releases)
   - Extract to your CS2 server directory

2. **Install Plugin**
   - Download or build `AutoRestart.dll`
   - Place in: `addons/counterstrikesharp/plugins/AutoRestart/`
   - Include `lang/` folder with language files

3. **Load Plugin**
   - Restart server or use: `css_plugins load AutoRestart`
   - Config auto-generates at: `addons/counterstrikesharp/configs/plugins/AutoRestart/AutoRestart.json`

## ⚙️ Configuration

Config file auto-generates on first load.

**Location:**
```
addons/counterstrikesharp/configs/plugins/AutoRestart/AutoRestart.json
```

### Full Configuration

```json
{
  "AutoRestartEnabled": true,
  "EnableManualRestart": true,
  "Flag": "@css/root",
  "AutoRestartTime": "06:00:00",
  "TimeZone": "Asia/Seoul",
  "WarningTimes": [300, 180, 60, 30, 10, 5],
  "EnableWarnings": true,
  "Prefix": "[AutoRestart]",
  "Language": "ko",
  "ConfigVersion": 1
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| **General** |
| `AutoRestartEnabled` | bool | `true` | Enable automatic scheduled restart |
| `EnableManualRestart` | bool | `true` | Allow manual restart command |
| `Flag` | string | `"@css/root"` | Required permission flag for manual restart |
| **Schedule** |
| `AutoRestartTime` | string | `"06:00:00"` | Daily restart time (HH:mm:ss format) |
| `TimeZone` | string | `"Asia/Seoul"` | Timezone for restart schedule |
| **Warnings** |
| `WarningTimes` | int[] | `[300, 180, 60, 30, 10, 5]` | Warning intervals in seconds before restart |
| `EnableWarnings` | bool | `true` | Enable restart warnings |
| **Localization** |
| `Prefix` | string | `"[AutoRestart]"` | Message prefix for all plugin messages |
| `Language` | string | `"ko"` | Language setting (`"en"` or `"ko"`) |

**Apply changes:** `css_plugins reload AutoRestart`

## 🎮 Commands

| Command | Description | Permission |
|---------|-------------|------------|
| `css_restart` | Manually restart the server immediately | Required (`Flag` setting) |
| `css_restartstatus` | Check next scheduled restart time | None |

## 🌍 Language Support

### Available Languages
- **English (en)** - Default language
- **Korean (ko)** - Full Korean translation

### Language Files
```
addons/counterstrikesharp/plugins/AutoRestart/lang/
├── en.json    # English translations
└── ko.json    # Korean translations
```

### Change Language
Edit config file:
```json
{
  "Language": "ko",           // Set to "en" for English, "ko" for Korean
  "Prefix": "[자동재시작]"    // Customize prefix for any language
}
```

## 🛠️ Building from Source

```bash
# Build
dotnet build -c Release

# Output: bin/Release/net8.0/AutoRestart.dll

# Or use build script
.\build.ps1
```

## 📋 Usage Examples

### Example 1: Daily 6 AM Restart (Korean)
```json
{
  "AutoRestartTime": "06:00:00",
  "TimeZone": "Asia/Seoul",
  "Language": "ko",
  "Prefix": "[자동재시작]"
}
```

### Example 2: Daily 3 AM Restart (English)
```json
{
  "AutoRestartTime": "03:00:00",
  "TimeZone": "America/New_York",
  "Language": "en",
  "Prefix": "[AutoRestart]"
}
```

### Example 3: Custom Warning Times
```json
{
  "WarningTimes": [600, 300, 120, 60, 30, 10],
  "EnableWarnings": true
}
```
*Warnings at: 10min, 5min, 2min, 1min, 30sec, 10sec before restart*

## 🌏 Timezone Reference

Common timezone IDs:
- `Asia/Seoul` - Korea Standard Time (KST)
- `Asia/Tokyo` - Japan Standard Time (JST)
- `UTC` - Coordinated Universal Time
- `America/New_York` - Eastern Time (ET)
- `America/Los_Angeles` - Pacific Time (PT)
- `Europe/London` - Greenwich Mean Time (GMT)

**Check available timezones:**
```powershell
[System.TimeZoneInfo]::GetSystemTimeZones() | Format-Table Id, DisplayName
```

## 🔍 How It Works

**Restart Scheduling:**
- Calculates time until next restart based on configured timezone
- If restart time has passed today, schedules for tomorrow
- Creates timer for exact restart time
- Schedules warning timers at configured intervals

**Warning System:**
- Sends chat messages to all players at specified intervals
- Displays remaining time in minutes or seconds
- Console logs for server administrators

**Manual Restart:**
- Admin command with permission check
- 2-second delay before server shutdown
- Notifies all players before restart

## 🐛 Troubleshooting

**Problem:** Timezone not recognized
```powershell
# List available timezone IDs
[System.TimeZoneInfo]::GetSystemTimeZones() | Where-Object {$_.Id -like "*Seoul*"}
```

**Problem:** Plugin not loading
- Verify CounterStrikeSharp is installed correctly
- Check .NET 8.0 Runtime is installed
- Review console for error messages

**Problem:** Language file not working
- Ensure `lang/` folder is in plugin directory
- Check language file exists (`en.json` or `ko.json`)
- Verify JSON syntax is valid

**Need help?** Open an [Issue](../../issues) with:
- Server OS & CounterStrikeSharp version
- Console error messages
- Configuration file contents

## 🤝 Contributing

Contributions welcome!

**Report bugs or suggest features:** Open an [Issue](../../issues)

**Code contributions:**
1. Fork the repository
2. Create feature branch: `git checkout -b feature/my-feature`
3. Commit changes: `git commit -m "Add feature"`
4. Push to branch: `git push origin feature/my-feature`
5. Open a Pull Request

<div align="center">

**⭐ Star this repo if you find it useful! ⭐**

</div>