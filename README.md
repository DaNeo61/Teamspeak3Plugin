# Teamspeak3Plugin
Teamspeak 3 Client Integration Plugin for Macro Deck 2

## Current Features
### Actions
- Un-/Mute Input
- Un-/Mute Output
- Switch to Configured Channel
### Variables
- Input State (un-/muted)
- Output State (un-/muted)

## Planned Features
### Actions
- Change Nickname to Configured
- Away Options

### Variables
- ChannelName
- Talkstate

## Requirements
- Macro Deck 2 (v2.9.0 or later recommended)
- .NET 8.0 Runtime (included with Macro Deck)
- Teamspeak 3 + Client Query Plugin

# TeamSpeak 3 Integration: How to get the ClientQuery API Key  
1. Open your TeamSpeak3 Client
2. Click on `Tools` > `Options` (Default shortcut: ALT+P)![Tools-Options](https://i.imgur.com/ZPOVVP3.png)
3. Click on `Settings`![Addons-ClientQuery-Settings](https://i.imgur.com/cq9s798.png)
4. The API Key should now be shown, copy it
![Addons-ClientQuery-Settings-API-Key](https://i.imgur.com/1XBTvcb.png)
5. Paste the API Key in the settings of the Plugin


# Version History

## 1.0.0	
- Initial Release

## 1.0.1
- Small refactorings
- Added some error handling for disconnect handling
- Removed deprecated Screenshots for Query API Installation in Readme