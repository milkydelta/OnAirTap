using LIV.SDK.Unity;
using HarmonyLib;
using System;

namespace OnAirTap.InjectionPatches2;

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("IsConnected")]
class Patch_IsConnected
{
    static bool Prefix(ref bool __result)
    {
        __result = (Plugin.camDat.cfg & LIVnyan_cfg.CAM_ON) != 0;
        return false;
    }
}

// [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
// [HarmonyPatch("IssuePluginEvent")]
// class Patch_DisableSubmit
// {
//     static bool Prefix()
//     {
//         return false;
//     }
// }

// [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
// [HarmonyPatch("SubmitApplicationOutput")]
// class Patch_DisableSubmitApplicationOutput
// {
//     static bool Prefix()
//     {
//         return false;
//     }
// }

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("AddTexture")]
class Patch_DisableAddTexture
{
    static bool Prefix()
    {
        return false;
    }
}

// [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
// [HarmonyPatch("CreateFrame")]
// class Patch_DisableCreateFrame
// {
//     static bool Prefix()
//     {
//         return false;
//     }
// }

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
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

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
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

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("CreateCaptureProtocol")]
class Patch_CreateCaptureProtocol
{
    //This uint is actually an enum representing the type of interface that has been requested.
    //The value comes from SDKSettings, which is loaded from a resource.
    //No numerical values are assigned in the enum, so it's automatic. Bridge is 0 and Mock is 1. 
    //We want Mock, because it doesn't complain about the main application not being installed.
    static void Prefix(ref uint __0)
    {
        __0 = 1;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender))]
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
    static bool Prefix(SDKRender __instance, ref SDKInputFrame ____inputFrame)
    {
        SDKBridge.UpdateInputFrame(ref ____inputFrame);
        return false;
    }
}
