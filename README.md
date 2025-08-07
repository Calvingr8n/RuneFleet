# RuneFleet
A Windows application for managing and launching multiple game clients simultaneously. Features include live thumbnail previews, grouped account management, and streamlined multilogging control.

## Setup Guide
Can be used for normal accounts and jagex accounts. Video setup guides below.

### Jagex Accounts
1. Download the latest release from [here](https://github.com/Calvingr8n/RuneFleet/releases).
2. Start RuneFleet
3. Click Import
4. Run your preferred client from the Jagex Launcher until it's fully loaded
5. Stop Import

https://github.com/user-attachments/assets/c6ddaeeb-e4d2-4e85-bcf2-ae72b6b4a520

There is an [example accounts.csv file in the repository](https://github.com/Calvingr8n/RuneFleet/blob/master/RuneFleet/example_accounts.csv). It shows the different configurations on groups, profile and RuneLite scale.

### Normal Accounts
1. Download the latest release from [here](https://github.com/Calvingr8n/RuneFleet/releases).
2. Start RuneFleet
3. Close RuneFleet
4. Open generated _account.csv_ file
5. Type your character name for **JX_DISPLAY_NAME**
6. Type _%userprofile%\appdata\local\runelite\runelite.exe_ for **Client**
7. Change **Arguments** format to Text
8. Type _--profile=anythingWithoutSpaces_ for **Arguments**
9. Save
10. Relaunch RuneFleet


https://github.com/user-attachments/assets/dfb5941e-11c0-4287-9366-71b52b47e7b7


## Controls
Account List:
- *Click* to focus
- *Double Click* to launch
  
Live Client View:
- *Click* to focus
- *Right Click* to close
  
Scale, Topmost:
- *Click* to activate
- Scale only works with RuneLite

## Disclaimer
RuneFleet is an unofficial third-party project and is not in any way affiliated with any of the games or companies it interacts with. Said games and companies are not responsible for any problems with RuneFleet nor any damage caused by using RuneFleet.

RuneFleet is NOT a game client. It starts unmodified game clients. This program does not and will not automate game input on your behalf.




