# MP Server Provider

A Windows app for setting up and running your own game servers, without messing around with command lines, config files or port forwarding guides.

You pick a game, click install, and the app downloads the server for you. After that you can start it, stop it, watch the console, edit files and add mods, all from one window.

## Games you can host right now

- Minecraft: Java Edition (Vanilla, Paper, Purpur, Fabric, Forge, Velocity, BungeeCord)
- Minecraft: Bedrock Edition
- FiveM (GTA V)
- RedM (Red Dead Redemption 2)
- BeamMP (BeamNG.drive)
- Hytale
- Rust
- Valheim
- Garry's Mod
- ARK: Survival Evolved
- ARK: Survival Ascended
- 7 Days to Die
- PixARK
- Unturned
- Palworld
- Enshrouded
- Conan Exiles
- DayZ (Server and Experimental)
- Project Zomboid
- SCUM
- Don't Starve Together
- The Forest
- Sons of the Forest
- Core Keeper
- Craftopia
- Creativerse
- CryoFall
- Cubic Odyssey
- ASKA
- ASTRONEER
- Abiotic Factor
- DeadPoly
- Eco
- Empyrion - Galactic Survival
- Left 4 Dead
- Left 4 Dead 2
- Medieval Engineers
- Memories of Mars
- Smalland
- Starbound (stable or unstable branch)
- Survive the Nights
- Totally Accurate Battlegrounds
- VEIN
- American Truck Simulator
- Euro Truck Simulator 2
- OpenRCT2 (with a built-in scenario downloader)
- Half-Life
- Counter-Strike 1.6
- Terraria
- Rising World
- Stationeers
- Stormworks
- Satisfactory
- Vintage Story

More games are on the way. You can also add your own game with a custom JSON file.

## What it can do

- **Install servers in one click.** Steam servers are downloaded through SteamCMD automatically.
- **Import a server you already have.** Point it at the folder and it picks up the game, version and port for you.
- **Console.** See the live server output and type commands straight into it.
- **File manager.** Browse, edit, rename and delete server files. You can drag files in from Explorer too.
- **Mods and plugins.** Keep a mod library and add mods to any server. Drag and drop works here as well. For Minecraft it warns you when a plugin doesn't fit your server version.
- **Let friends join.** Open ports with UPnP or use a playit.gg tunnel if your router won't cooperate. There's also a button to test if your server can be reached from the internet.
- **Firewall rules** are added for you when a server needs them.
- **Resource usage.** See how much CPU and RAM each server is using.

## Installing

Grab the latest `MPServerProvider Installer x.x.x.exe` from the Releases page and run it. You don't need to install .NET or anything else first.

By default it installs to `%LOCALAPPDATA%\Programs\MP Server Provider`. Your servers, mods and settings live in the `data` folder next to the app. Uninstalling won't delete that folder, so your servers are safe if you reinstall.

## Running it from source

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) on Windows.

Double click `run dotnet.bat`, or run this from the repo folder:

```
dotnet run --project dotnet\src\MPServerProvider.App
```

When you run it from source, everything (servers, mods, settings) gets saved in `dotnet\data`.

To run the tests:

```
dotnet test dotnet\MPServerProvider.sln
```

## Building the installer

Install [Inno Setup 6](https://jrsoftware.org/isdl.php), then double click `build-installer.bat`.

It uses the version number from `dotnet\src\MPServerProvider.App\MPServerProvider.App.csproj`. If you want a different version, pass it along:

```
build-installer.bat 1.2.0
```

The installer ends up in `installer\Output`.

## Folder layout

```
assets/       app icon
data/         default data (custom game JSON files go in data/providers/json)
dotnet/       the actual app
  src/MPServerProvider.App/          the WPF app (windows, pages, dialogs)
  src/MPServerProvider.Core/         the logic (installing, running servers, mods, networking)
  src/MPServerProvider.Core.Tests/   tests
installer/    Inno Setup script for the installer
```

## Good to know

- Windows only.
- Don't install the app into the same folder as this source code. The installer blocks this on purpose, because an uninstall would wipe the code with it.
- If players get kicked with weird Java errors on a Minecraft server, check the console. The app adds a message telling you which plugin is causing it.
