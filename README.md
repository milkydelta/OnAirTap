# OnAirTap
An alternative to the LIV application for Unity games using the LIV Unity SDK.<br>
Intended for use with the LIVnyan plugin for VNyan.


LIV -> live -> On Air<br>
Spout -> Tap (sort of)<br>

I'm bad at naming things.

## A short word on compatibility

I initially developed this using BepInEx 5 on Beat Saber 1.43. The BSIPA port has been tested on that game version, as well as 1.40.8 and 1.44.1. It shouldn't need a rebuild, I think.

The only other game I've tested is Open Brush. If I remember rightly, that was initially just me dropping the BepInEx Beat Saber build into OB. Now, I build the BepInEx versions directly against OpenBrush. This should be reasonably game-agnostic, though.

The support for SDK 2 was developed against version `1.43.100_18399` of Beat Saber's URP beta. It might work with other games, but I have nothing to test that against.

Support for MelonLoader was developed using version `0.7.3`.

IL2CPP games can work, but only with MelonLoader, and only when the LIV support is added through a mod. There is currently no support for IL2CPP games with native LIV support, although I am looking into it.

## Setup

**For instructions, consult the [wiki](https://github.com/milkydelta/OnAirTap/wiki).**

## Building

### Native Linux Components

In `Native`, there's a folder for each of the two components. Run the `make.sh` files to build them. For lincomm, you will need winegcc, which is usually included with the wine package. For mmf_vnyan, you will need mingw-w64.

Copy the built dll to the same folder as the game executable. If you're using the helper program, that should also have a copy of the dll.

### Unity Mod

Inside `UnityMod` is a couple folders. `Common` has the main bulk of the mod logic, and the other folders are small classes to perform the tasks that differ between mod loaders.

Go into a loader folder and open up the csproj. One of the properties sets the location of the game, for assembly references. You'll want to change that.
If you're building the BepInEx version, you should also check the LIV reference. Some games compile the SDK outside of `Assembly-CSharp`, so you may need to update the HintPath.

Then open a terminal and run `dotnet build`. If you get complaints about netstandard, try changing the TargetFramework.

### KlakSpout

This repo includes the C# code for KlakSpout, but some of the backend Spout logic is contained in a separate C dll, which I have not included. Go to the KlakSpout GitHub repo, and grab KlakSpout.dll from `Packages/jp.keijiro.klak.spout/Plugin`. Place that in `GameName_Data/Plugins/x86_64/` as `OAT_KlakSpout.dll`. If you put it in the other plugins folder, vital initialisation logic is not performed, and the game will crash whenever a Spout sender is created. I had to figure that out the hard way.


## Thanks Very Much To These People For These Reasons

 - [LumKitty](https://github.com/LumKitty) - Inspiration, Beta Testing
 - [Empoleon](https://github.com/mercurialworld) - Beta Testing

Your assistance has been invaluable.
