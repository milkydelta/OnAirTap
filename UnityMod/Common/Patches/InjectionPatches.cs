using LIV.SDK.Unity;
using HarmonyLib;
using System;

namespace OnAirTap.InjectionPatches;

//patches to replace the use of SDKBridge's private __injection fields in sdk 1.5

//These are all in different classes because HarmonyX was complaining about ambiguous matches.

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("IsActive", MethodType.Getter)]
class Patch_IsActive
{
    static bool Prefix(ref bool __result)
    {
        __result = (Plugin.camDat.cfg & LIVnyan_cfg.CAM_ON) != 0;
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("IssuePluginEvent")]
class Patch_DisableSubmit
{
    static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("SubmitApplicationOutput")]
class Patch_DisableSubmitApplicationOutput
{
    static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("AddTexture", [typeof(SDKTexture)])]
class Patch_DisableAddTexture
{
    static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("CreateFrame")]
class Patch_DisableCreateFrame
{
    static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("GetResolution")]
class Patch_SDKResolution
{
    static bool Prefix(ref bool __result, ref SDKResolution sdkResolution)
    {
        BridgePatchMethods.UpdateResolution(ref BridgePatchMethods.Res);

        sdkResolution = BridgePatchMethods.Res;

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
[HarmonyPatch("UpdateInputFrame")]
class Patch_SDKInputFrame
{
    static bool Prefix(ref bool __result, ref SDKInputFrame setframe)
    {
        BridgePatchMethods.SetInputFrame(ref BridgePatchMethods.Frame);

        setframe = BridgePatchMethods.Frame;
        __result = true;
        return false;
    }
}