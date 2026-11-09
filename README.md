# OnAirTap
An alternative to the LIV application for Unity games using the LIV Unity SDK.<br>
Intended for use with the VRnyan plugin for VNyan.


LIV -> live -> On Air<br>
Spout -> Tap (sort of)<br>

I'm bad at naming things.

## A short word on compatibility

I initially developed this using BepInEx 5 on Beat Saber 1.43. The BSIPA port has been tested on that game version, as well as 1.40.8 and 1.44.1. There's no need to rebuild. The same assembly works on all tested versions.

My other main test game is Open Brush. I use that to test both MelonLoader and BepInEx, though the BepInEx versions are instead built against Firewatch.

Support for SDK 2 was developed against version `1.43.100_18399` of Beat Saber's URP beta. It might work with other games, but I have nothing to test that against.

Support for MelonLoader was developed using version `0.7.3`.

IL2CPP games can work, but only with MelonLoader. Support for native IL2CPP games was developed against Synth Riders `3.6.7`~ish. That's an SDK2 game, so I don't know if SDK1.5 will work.


## Setup

**For instructions, consult the [wiki](https://github.com/milkydelta/OnAirTap/wiki).**

## Building

### Native (Unmanaged) Components

In `Native`, there's a folder for each component. Run the `make.sh` file to build that component. For lincomm, you will need winegcc, which is usually included with the wine package. For wincomm, you will need mingw-w64.  
<sub>Some environments provide two versions of gcc, suffixed with `-posix` or `-win32`, to show the threading model. I'm on Arch, which doesn't do that, so you may need to change the script if you're on Debian or similar.</sub>

The Release configurations of both the BepInEx and MelonM projects will copy those
built files to their output folders.

 `vnyan_bridge` contains a helper program that was used on older versions, and can be safely ignored.  



### Unity Mod

Inside `UnityMod` is a couple folders. `Common` has the main bulk of the mod logic, and the other folders are small classes to perform the tasks that differ between mod loaders.

You will need to tell the project what game you're using, and where it is located.

---
For BSIPA, create an `OnAirTap_BSIPA.csproj.user` file. The format of that is the same as you might expect from other Beat Saber mods.

---

For OWML, everything should be handled automatically. Game and loader assembies are pulled from NuGet, and the assembies for NomaiVR are referenced from OWML's Mods folder, which is at a known location.  
The only thing you'll need to do is drop both OAT KlakSpout DLLs next to the sln file.

---

For BepInEx and MelonLoader, open up the `Build.props` file.
- GameBaseDirectory is the folder containing your game's main executable.
- GameName is the name of the game, as it appears on the Data folder.
- BepInExBaseDirectory is the game's BepInEx folder. This is usually inside the game's base folder, but can be elsewhere.

Both paths are written without the trailing slash.

For BepInEx, you may also need to edit the `csproj` file. If you're building against any game on a Unity version above 2017, change the Target Framework from `net35` to `net472`. Some games might also compile their LIV SDK outside of `Assembly-CSharp.dll`. In such cases, you should change the Hint Path accordingly.

<sub>On `net472`, you can optionally change from unmanaged to managed comms. That removes the need for wincomm and lincomm. Unmanaged is the default for compatibility reasons, since `net35` does not support the required features.</sub>

---

Then open a terminal and run `dotnet build`. If you get complaints about netstandard, try changing the Target Framework to `netstandard2.1`.

### KlakSpout

Code for KlakSpout is not in this repository. I have separated that out into [OAT.KlakSpout.Runtime](https://github.com/milkydelta/OAT.KlakSpout.Runtime), which you should have downloaded as part of the setup process.

It is important that you place the unmanaged plugin DLL in `GameName_Data/Plugins/`, or it's `x86_64` subfolder. If you place the plugin in a different folder, vital initialisation logic is not performed, and the game will crash whenever a Spout sender is created. I had to figure that out the hard way.

The only exception is on IL2CPP, where that logic is not performed regardless, and I have to use another mod to load the plugin manually.

## Thanks Very Much To These People For These Reasons

 - [LumKitty](https://github.com/LumKitty) - Inspiration, Beta Testing, Gifting me Firewatch
 - [Empoleon](https://github.com/mercurialworld) - Beta Testing

Your assistance has been invaluable.
