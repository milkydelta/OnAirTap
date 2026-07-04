using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;



class RenderingPatches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        if (Plugin.cfg.SpoutSendBG == false) {return;}
        Plugin.spoutBG.sourceTexture = ____backgroundRenderTexture;
        Plugin.spoutBG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        if (Plugin.cfg.SpoutSendFG == false) {return;}
        Plugin.spoutFG.sourceTexture = ____foregroundRenderTexture;
        Plugin.spoutFG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    static void HookSpoutOp(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____optimizedRenderTexture) {
        if (Plugin.cfg.SpoutSendOP == false) {return;}
        Plugin.spoutOptimised.sourceTexture = ____optimizedRenderTexture;
        Plugin.spoutOptimised.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "InvokePreRender")]
    [HarmonyPostfix]
    static void GetCameraForClipPlane( ref SDKRender __instance) {
        Plugin.hmdPos = __instance.stage.worldToLocalMatrix.MultiplyPoint3x4(__instance.hmdCamera.transform.position);
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Dispose")]
    [HarmonyPrefix]
    static void DisposeSpoutSenders() {
        if (Plugin.cfg.BlankSpoutSenders == false) {return;}

        /*For some reason, the Spout senders just stop working permanently if I disable them.
        The senders still appear in OBS, but there's no frames.
        I've used RUE to check the internal buffer rendertexture. That's still being copied to, but it's not sending.*/

        //For now, the only solution I can think of is keeping the senders around and just blanking them out.
        Vector2Int res = Plugin.resolution;
        Plugin.spoutBG.sourceTexture = new RenderTexture(res.x,res.y,24);
        Plugin.spoutFG.sourceTexture = new RenderTexture(res.x,res.y,24);
        Plugin.spoutOptimised.sourceTexture = new RenderTexture(res.x,res.y,24);

        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKUtils), "SetCamera")]
    [HarmonyPrefix]
    static void DoLayerMasks( ref int layerMask) {
        if (Plugin.cfg.LayerMask != 0){
            layerMask = Plugin.cfg.LayerMask;
        }
    }
}
