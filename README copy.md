# OnAirTap
An alternative to the LIV application for Unity games using the LIV Unity SDK.<br>
Intended for use with the LIVnyan plugin for VNyan.


LIV -> live -> On Air<br>
Spout -> Tap (sort of)<br>

I'm bad at naming things.

## Setup

Go to Releases and download a build that's appropriate for your chosen mod loader, which you should already have set up.

Releases are named in a specific way: `OnAirTap-[Version]-[Loader]-[BuiltAgainst]-[BuiltAgainstVer].zip`

Extract that onto your game folder. As long as the loader is the same, builds made for another game *should* work. You will just need to rename the `GameName_Data` folder to put the KlakSpout plugin in the correct location.

You will then need to run the game once, to create a config file.

The mod will read information from LIVnyan's MemoryMappedFile and should activate when you press the LIVnyan button inside VNyan.

### Setup notes for Linux
If the game has a native Linux version, don't use it. Switch to the Windows version of the game. In Steam, you can do that from Properties > Compatibility, by ticking the box and choosing a Proton version.

To use the Spout feeds, you will need Spout2PW and obs-pwvideo by Hoshino Lina. Those are no longer actively developed, but they still work fine.
You may want to set the `SPOUT2PW_INSTANCE` variable to something, so the video feed name doesn't change for each game.

MemoryMappedFiles are a Windows feature. Wine has support, but that is contained to inside a wine instance. Communication across instances, to relay information between VNyan and the game, requires some additional code. OnAirTap has logic to use native Linux shared memory, but pulling the data out of LIVnyan's MMF requires a helper program. That's `mmf_vnyan`, which you can run by setting the `PROTON_REMOTE_DEBUG_CMD` environment variable when you run VNyan.

The helper exe currently has no accessible stop mechanism, so you will have to end it yourself.

## Config

I've tried to give the config options self-explanatory names.
There are some that could use a bit of background information.

### Section - Render Passes

 - Render[XYZ] - Whether to render a given pass. 
 - Send[XYZ] - Whether to update a pass' Spout sender each frame. Useful if you need to render a pass, but aren't using it in compositing.

 - BlankSpoutOnRenderDispose - The object that controls rendering is often destroyed on a scene change. When that happens, the spout feeds are frozen until the next scene creates a new one. This option blakns the feeds after destruction, so there's no objects seemingly stuck in place 
 - LayerMaskOverride - Format may be changed. This option allows you to overwrite the layer mask of the spectator camera. A value of 0 is ignored.

### Section - Clip Planes

 - GroundClipPlaneEnabled - true
 - GroundClipPlaneElevation - How far off the playspace ground is the ground plane? The default value is 1cm, to prevent Z-fighting at 0cm.
 - ClipPlaneShouldBeVertical - An actual mesh plane is used to divide the foreground from the background. I couldn't decide what the rotation behaviour should be, so this chooses between the two easiest options. NEEDS IMAGE AND FURTHER EXPLANATION.

 - CameraFarClip - This sets the far clip distance of the camera. Default value in BepInEx builds is 1000. That is raised to 5000 for BSIPA builds.

### Section - Extra Data

 - ShouldReadResolutionFromMMF - false,
 - ShouldReadTrackerFromMMF - false,
 - MMFProtocolMinorVersion - 0





## Compositing

There's three different ways to do compositing, based on if you're using just the Optimised texture, the Foreground and Background textures, or all three.

### Foreground/Background
---
This is the setup you'll probably be using. An extra camera is used to render the scene two additional times. The foreground texture can then be used as a premultiplied image, so that emissive (glowing) objects and fog will correctly appear in front of the model.

There are some extra steps needed for limit the influence of the foreground, so that areas of the background not covered by your model don't experience doubled glow. That can be done either with an [Advanced Mask](https://github.com/FiniteSingularity/obs-advanced-masks) filter or some adjustments to blend modes.

**Uses Plugin, Uses Groups**<br>
To composite this config:

![C_FG1](/docs/imgs/CompFGBG1.png)

**Uses Plugin, No Groups**<br>
To composite this config:

![C_FG2](/docs/imgs/CompFGBG2.png)

**No Plugin, No Groups**<br>
To composite this config:

![C_FG3](/docs/imgs/CompFGBG3.png)


### Just Optimised
---
If there are no emissive objects between you and the camera, this is the way to go. The Optimised texture has the RGB channels of a background texture, but with the Alpha channel of a foreground texture. It only adds one render, and is therefore quite efficient.

The OBS layout for compositing Optimised configs is very simple.

![C_OP1](/docs/imgs/CompOP1.png)

That's it. You don't even need any filters.

### All three
---
This config is not very efficient. You're rendering the active scene three extra times. Avoid, if you can.

In Beat Saber, the alpha channel is overwritten during post-processing. For reasons unknown, Beat Saber is missing the code needed to save and re-apply the original alpha channel at the end of the frame, so our Foreground render does not not produce a usable premultiplied texture. Luckily, the Optimised texture still works as intended, and can be used to fix the problem.

To composite this config:

![C_ALL3](/docs/imgs/CompALL1.png)

You only need a single filter here. The Foreground source must be masked to the Alpha channel of the VNyan source, so that foreground objects don't have double-strength glow.

### Notes on premultiplication
---
If you don't know what premultiplication is, watch [this video](https://www.youtube.com/watch?v=XobSAXZaKJ8).
It's about compositing in general, but does explain premultiplication, as part of that.

OBS seems to prefer straight alpha. OBS' plugin API gives direct enough control for a plugin source to set itself as premultiplied, but adding almost any effect filter will set it back to straight alpha. That means that things like glow and fog will disappear.

I've seen this with an empty shaderfilter, the render delay filter, and colour correction.

If you _**need**_ to apply a filter to a premultiplied source, either find a way to apply it to a group further up, or separate the source into two with different blend modes.

### Compositing notes for Linux
---
Pipewire Video sources can only be premultiplied. It is assumed that the feed always is, and there's no built-in way to change that, so some workarounds are needed.

#### For Default/Straight:

As stated above, most filters will reset the source back to straight alpha. I personally use a shaderfilter with the default shader, but you can use an unmodified Colour Correction.

#### For Opaque/Ignore:

To ignore the alpha channel, we can't use a filter, because those will first re-do the multiplication and blank out areas of the image. Instead, I put the source in a group with a black colour source of the same size. Filters, like Advanced Mask, can then be applied to the group.

Groups cannot be nested, though. If you need to put your source inside a group, you might need to use a separate scene. In that scene, put the pipewire source and a colour. You can then bring the scene into your original scene as a source, at which point it can be inside a group.



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
