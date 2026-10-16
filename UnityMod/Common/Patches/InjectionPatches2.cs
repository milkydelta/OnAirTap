//using LIV.SDK.Unity;
using HarmonyLib;
using HarmonyLib.Tools;
using System;

namespace OnAirTap.InjectionPatches2;

[HarmonyPatch(typeof(CaptureProtocolMock))]
[HarmonyPatch("IsConnected")]
class Patch_IsConnected
{
    static void Postfix(ref bool __result)
    {
        __result = Plugin.camDat.HasSetting(CamDatCfg.CAM_ON);
    }
}

[HarmonyPatch(typeof(SDKBridge))]
[HarmonyPatch("AddTexture")]
class Patch_DisableAddTexture
{
    static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(CaptureProtocolMock))]
[HarmonyPatch("GetResolution")]
class Patch_SDKResolution
{
    static bool Prefix(ref SDKResolution sdkResolution)
    {
        BridgePatchMethods.UpdateResolution(ref BridgePatchMethods.Res);

        sdkResolution = BridgePatchMethods.Res;

        return false;
    }
}

[HarmonyPatch(typeof(CaptureProtocolMock))]
[HarmonyPatch("UpdateInputFrame")]
class Patch_SDKInputFrame
{
    static bool Prefix(ref SDKInputFrame __0)
    {
        BridgePatchMethods.SetInputFrame(ref BridgePatchMethods.Frame);

        __0 = BridgePatchMethods.Frame;
        return false;
    }
}

//SDK2 New Method

[HarmonyPatch(typeof(SDKBridge))]
[HarmonyPatch("CreateCaptureProtocol")]
class Patch_CreateCaptureProtocol
{
    //This uint is actually an enum representing the type of interface that has been requested.
    //The value comes from SDKSettings, which is loaded from a resource.
    //No numerical values are assigned in the enum, so it's automatic. Bridge is 0 and Mock is 1. 
    //We want Mock, because it doesn't complain about the main application not being installed.

    //This actually isn't called on IL2CPP. I presume it's some inlining problem.
    static void Prefix(ref uint __0)
    {
        __0 = 1;
    }
}

[HarmonyPatch(typeof(SDKRender))]
[HarmonyPatch("UpdateBridgeInputFrame")]
class Patch_UpdateBridgeInputFrame
{
    /*
    For some reason, SDK 2 (or at least the URp beta's version of it) changes how UpdateBridgeInputFrame works.
    In the older versions, the call to sdkbridge was the last thing in the function.
    Now, it's in the middle. That means it is before the code that changes the inputframe's near and far  clip values to the values of the active camera.
    Because of that, near and far clip are stuck at the default values of 0.01 and 1000, which is not sufficient for beat saber.
    This should revert it to the 1.5 behaviour
    */
    static bool Prefix(SDKRender __instance)
    {
        //var inpF = __instance._inputFrame;
        //SDKBridge.UpdateInputFrame(ref inpF);
        //__instance._inputFrame = inpF;
        
        BridgePatchMethods.SetInputFrame(ref BridgePatchMethods.Frame);
        __instance._inputFrame = BridgePatchMethods.Frame;
        

        #if OAT_IL2CPP
        //On IL2CPP, something prevents the rendering patches below from being called.
        //So I have to call them manually here, from a patch that does work, for some reason.
        RenderingPatches.HookSpoutBG(ref __instance);
        RenderingPatches.HookSpoutFG(ref __instance);
        RenderingPatches.HookSpoutOp(ref __instance);
        RenderingPatches.GetCameraForClipPlane(ref __instance);
        #endif

        return false;
    }
}
