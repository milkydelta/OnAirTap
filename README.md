# OnAirTap

LIV -> live -> On Air

Spout -> Tap (sort of)

I'm bad at naming things.

## Setup (and also building)

### BepInEx

Download the latest windows version of [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) and extract it to the game directory. Run the game once, to make some initial files.

### Unity Mod

Open up the .csproj and set GameDataDirectory to be the GameName_Data folder of the game you're targeting. You might also have to change the LIV Reference, since many games compile the LIV SDK as part of Assembly-CSharp.dll.

Then, run `dotnet build` in the UnityMod folder.

If the build complains, try fiddling with the TargetFramework.

Once you have a successful build, copy OnAirTap.dll to `BepInEx/plugins`.

### KlakSpout

This repo includes the C# code for KlakSpout, but some of the backend Spout logic is contained in a separate C dll, which I have not included. Go to the KlakSpout GitHub repo, and grab KlakSpout.dll from `Packages/jp.keijiro.klak.spout/Plugin`. Place that in `GameName_Data/Plugins/x86_64/`. If you put it in the other plugins folder, vital initialisation logic is not performed, and the game will crash whenever a Spout sender is created. I had to figure that out the hard way.

### Native Linux Components

If you're on Windows, you can stop here. The plugin will read camera data from LIVnyan's MemoryMappedFile, and everything will work. If you're on Linux, proceed.

I have added a quick bit of logic to detect wine. If wine is found, the plugin will load a DLL that I wrote, so it can punch out of the wine environment and use native Linux shared memory mechanisms. That means that you don't have to run VNyan in the same wine environment as your game.

In `Native/`, there's two folders, for the two components. Each has a simple .sh file you can run to build them. You will need winegcc and mingw-w64. Place the .dll file in a location where the game can load it. That'll be either the BepInEx plugins folder or the base game folder. The .exe file is intended to be run inside your VNyan prefix, using the PROTON_REMOTE_DEBUG_CMD environment variable. Wherever you put that, also give it a copy of the dll. The bridge exe currently has no stop mechanism, so you will have to kill it yourself.

## The Transparency Issue

I'm working on an OBS shader to fix it.
