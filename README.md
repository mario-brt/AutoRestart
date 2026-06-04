# 🔄 CS2 AutoRestart Plugin

<div align="center">

A Counter-Strike 2 server plugin that automatically restarts the server at a scheduled time with timezone support

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-v1.0.346%2B-blue)](https://github.com/roflmuffin/CounterStrikeSharp)

</div>

## 🎯 Features

- ⏰ **Scheduled Restart** - Automatically restart server at a specific time daily
- 🌏 **Timezone Support** - Configure any timezone (default: Asia/Seoul)
- 🧠 **Smart Queue** - Delays restart until the server is empty (configurable)
- 🚨 **Warning System** - Notify players before restart with customizable intervals
- 🎮 **Manual Restart** - Admin command to restart immediately
- 📊 **Status Check** - View next scheduled restart time
- 🌍 **Multi-Language** - English, German, and Korean support
- ⚙️ **Custom Prefix** - Configurable message prefix

## 📦 Requirements

- **[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)** v1.0.346 or newer

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
  "WaitForEmptyServer": true,
  "Flag": "@css/root",
  "AutoRestartTime": "06:00:00",
  "TimeZone": "Asia/Seoul",
  "WarningTimes": [300, 180, 60, 30, 10],
  "EnableWarnings": true,
  "Prefix": "[AutoRestart]",
  "Language": "ko",
  "ConfigVersion": 1,
  "MaximumPlayers": 0
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| **General** |
| `AutoRestartEnabled` | bool | `true` | Enable automatic scheduled restart |
| `EnableManualRestart` | bool | `true` | Allow manual restart command |
| `Flag` | string | `"@css/root"` | Required permission flag for manual restart |
| `WaitForEmptyServer` | bool | `true` | Delay scheduled restart if players are online |
| `MaximumPlayers` | int | `0` | Restart triggers only if player count is at or below this limit |
| **Schedule** |
| `AutoRestartTime` | string | `"06:00:00"` | Daily restart time (HH:mm:ss format) |
| `TimeZone` | string | `"Asia/Seoul"` | Timezone for restart schedule |
| **Warnings** |
| `WarningTimes` | int[] | `[300, 180, 60, 30, 10]` | Warning intervals in seconds before restart |
| `EnableWarnings` | bool | `true` | Enable restart warnings |
| **Localization** |
| `Prefix` | string | `"[AutoRestart]"` | Message prefix for all plugin messages |
| `Language` | string | `"ko"` | Language setting (`"en"`, `"de"`, or `"ko"`) |

**Apply changes:** `css_plugins reload AutoRestart`

## 🎮 Commands

| Command | Description | Permission |
|---------|-------------|------------|
| `css_restart` | Manually restart the server immediately | Required (`Flag` setting) |
| `css_restartstatus` | Check next scheduled restart time | None |


## 🛠️ Building from Source

```bash
# Build
dotnet build -c Release

# Output: bin/Release/net8.0/AutoRestart.dll
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

**Smart Queue System (New):**
- If `WaitForEmptyServer` is true, the server won't shut down while players are active
- Checks player count against the `MaximumPlayers` threshold
- Warns active players that a restart is pending
- Automatically shuts down the server once the player count drops to the limit

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