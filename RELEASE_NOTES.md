# Release Notes

## Version 1.1.0 - Initial Release

### 🎉 What's New

A Counter-Strike 2 server plugin that automatically restarts the server at a scheduled time with multi-language support and timezone configuration.

### ✨ Features

- **Scheduled Restart** - Automatically restart server at a specific time daily (default: 06:00 AM)
- **Timezone Support** - Configure any timezone (default: Asia/Seoul)
- **Warning System** - Notify players before restart with customizable intervals
- **Manual Restart** - Admin command to restart server immediately
- **Status Check** - View next scheduled restart time via command
- **Multi-Language** - Full English and Korean language support
- **Custom Prefix** - Configurable message prefix for all plugin messages
- **Smart Config** - Auto-generates configuration file on first load

### 🌍 Language Support

- **English (en)** - Full English translations
- **Korean (ko)** - Full Korean translations
- **Language Files** - Easy to add more languages via JSON files

### ⚙️ Default Configuration

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
  "Language": "ko"
}
```

### 🎮 Commands

- `css_restart` - Manually restart the server (admin only)
- `css_restartstatus` - Check next scheduled restart time

### 🚀 Quick Start

1. Install CounterStrikeSharp v1.0.346 or newer
2. Place plugin files in `addons/counterstrikesharp/plugins/AutoRestart/`
3. Include `lang/` folder with language files (en.json, ko.json)
4. Restart server or use `css_plugins load AutoRestart`
5. Config auto-generates on first load
6. Set your preferred language and restart time in config

### 💡 Usage Tips

- Set `AutoRestartTime` in HH:mm:ss format (24-hour)
- Configure `TimeZone` to match your server location
- Customize `WarningTimes` array for restart notifications
- Change `Language` to "en" or "ko" for your preference
- Modify `Prefix` to customize message branding

### 🔧 Technical Details

- Built with CounterStrikeSharp API v1.0.346
- Target Framework: .NET 8.0
- Timezone-aware scheduling system
- Safe timer cleanup on plugin unload
- Fallback to English if language file not found
