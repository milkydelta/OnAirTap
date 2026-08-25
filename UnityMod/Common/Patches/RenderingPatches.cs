using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;



class RenderingPatches {

    static internal RenderTexture ExtraRenderTexture;

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        if (Plugin.cfg.RenderBG == false) {return;}
        Plugin.spoutBG.sourceTexture = ____backgroundRenderTexture;
        Plugin.spoutBG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        if (Plugin.cfg.RenderFG == false) {return;}
        Plugin.spoutFG.sourceTexture = ____foregroundRenderTexture;
        Plugin.spoutFG.captureMethod = CaptureMethod.Texture;
    }

    // Add an additional render during the Optimised stage, that uses the EX layer mask
    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "InvokePreRenderBackground")]
    [HarmonyPostfix]
    static void BodgeSpoutEX(ref LIV.SDK.Unity.SDKRender __instance) {
        if (currentPass >= RenPass.OP && Plugin.cfg.RenderEX == true) { 
            __instance.cameraInstance.Render();
            SDKUtils.SetCamera(__instance.cameraInstance, __instance.cameraInstance.transform, __instance.inputFrame, __instance.localToWorldMatrix, Plugin.cfg.LayerMaskEX);
            __instance.cameraInstance.targetTexture = ExtraRenderTexture;
        }
    }

    // Add creation of the additional texture for the EX render
    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    static void HookSpoutOp(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____optimizedRenderTexture, SDKResolution ____resolution) {
        if (Plugin.cfg.RenderOP == false) {return;}
        Plugin.spoutOptimised.sourceTexture = ____optimizedRenderTexture;
        Plugin.spoutOptimised.captureMethod = CaptureMethod.Texture;
        if (Plugin.cfg.RenderEX == true) {
            SDKUtils.CreateTexture(ref ExtraRenderTexture, ____resolution.width, ____resolution.height, 24, RenderTextureFormat.ARGB32);
            Plugin.spoutExtra.sourceTexture = ExtraRenderTexture;
            Plugin.spoutExtra.captureMethod = CaptureMethod.Texture;
        }
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutExtra.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "InvokePreRender")]
    [HarmonyPostfix]
    static void GetCameraForClipPlane( ref SDKRender __instance) {
        if (__instance.stage == null || __instance.hmdCamera == null) {return;}
        Plugin.hmdPos = __instance.stage.worldToLocalMatrix.MultiplyPoint3x4(__instance.hmdCamera.transform.position);
    }



    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateAssets")]
    [HarmonyPostfix]
    static void PrintMaskDefault( ref SDKRender __instance) {
            Plugin.logger.Info(
                String.Concat(  "Default LayerMask: ",
                                Convert.ToString(__instance.spectatorLayerMask, 2).PadLeft(32,'0'),
                                " (", Convert.ToString(__instance.spectatorLayerMask), ")"
                )
            );
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
        Plugin.spoutExtra.sourceTexture = new RenderTexture(res.x, res.y, 24);

        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
        Plugin.spoutExtra.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKUtils), "SetCamera")]
    [HarmonyPrefix]
    static void DoLayerMasks( ref int layerMask) {
        int mask = Plugin.cfg.LayerMask;
        if (currentPass == RenPass.FG){mask = Plugin.cfg.LayerMaskFG;}
        else if (currentPass == RenPass.OP){mask = Plugin.cfg.LayerMaskOP;}
        
        if (mask != 0){
            layerMask = mask;
        }
    }

    enum RenPass {
        BG,FG,OP
    }

    static RenPass currentPass;

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderBackground")]
    [HarmonyPrefix]
    static void IsPassBG() {
        currentPass = RenPass.BG;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderForeground")]
    [HarmonyPrefix]
    static void IsPassFG() {
        currentPass = RenPass.FG;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderOptimized")]
    [HarmonyPrefix]
    static void IsPassOP() {
        currentPass = RenPass.OP;
    }
}
