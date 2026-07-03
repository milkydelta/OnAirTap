using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;

[HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge))]
class InjectionPatches {

    [HarmonyPatch("IsActive", MethodType.Getter)]
    [HarmonyPrefix]
    static bool Patch_IsActive(ref bool __result) {
        __result = (Plugin.camDat.cfg & LIVnyan_cfg.CAM_ON) != 0;
        return false;
    }

    [HarmonyPatch("IssuePluginEvent")]
    [HarmonyPrefix]
    static bool Patch_DisableSubmit() {
        return false;
    }

    [HarmonyPatch("SubmitApplicationOutput")]
    [HarmonyPrefix]
    static bool Patch_DisableSubmitApplicationOutput() {
        return false;
    }

    [HarmonyPatch("UpdateInputFrame")]
    [HarmonyPrefix]
    static bool Patch_SDKInputFrame(ref bool __result) {
        __result = true;
        return false;
    }

    [HarmonyPatch("AddTexture")]
    [HarmonyPrefix]
    static bool Patch_DisableAddTexture() {
        return false;
    }

    [HarmonyPatch("CreateFrame")]
    [HarmonyPrefix]
    static bool Patch_DisableCreateFrame() {
        return false;
    }

    [HarmonyPatch("GetResolution")]
    [HarmonyPrefix]
    static bool Patch_SDKResolution(ref bool __result) {
        __result = true;
        return false;
    }

}
