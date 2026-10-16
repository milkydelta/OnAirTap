using UnityEngine;
//using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;
using System.Linq;


namespace OnAirTap;



class RenderingPatches {

    [HarmonyPatch(typeof(SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    internal static void HookSpoutBG(ref SDKRender __instance) {
        if (Plugin.cfg.RenderBG == false) {return;}
        Plugin.spoutBG.sourceTexture = __instance._backgroundRenderTexture;
        Plugin.spoutBG.enabled = true;
    }

    [HarmonyPatch(typeof(SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    internal static void HookSpoutFG(ref SDKRender __instance) {
        if (Plugin.cfg.RenderFG == false) {return;}
        Plugin.spoutFG.sourceTexture = __instance._foregroundRenderTexture;
        Plugin.spoutFG.enabled = true;
    }

    [HarmonyPatch(typeof(SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    internal static void HookSpoutOp(ref SDKRender __instance) {
        if (Plugin.cfg.RenderOP == false) {return;}
        Plugin.spoutOptimised.sourceTexture = __instance._optimizedRenderTexture;
        Plugin.spoutOptimised.enabled = true;
    }

    [HarmonyPatch(typeof(SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(SDKRender), "InvokePreRender")]
    [HarmonyPostfix]
    internal static void GetCameraForClipPlane( ref SDKRender __instance) {
        if (__instance.stage == null || __instance.hmdCamera == null) {return;}
        Plugin.hmdPos = __instance.stage.worldToLocalMatrix.MultiplyPoint3x4(__instance.hmdCamera.transform.position);
    }



    [HarmonyPatch(typeof(SDKRender), "CreateAssets")]
    [HarmonyPostfix]
    static void PrintMaskDefault(ref SDKRender __instance)
    {
        Type t = __instance.GetType();

        var propGetter = t.GetProperties().Where((a) => { return a.Name == "spectatorLayerMask" || a.Name == "renderingLayerMask"; }).First().GetGetMethod();
        object prop = propGetter.Invoke(__instance, []);

        if (prop.GetType().Name.EndsWith("LayerMask"))
        {
            Plugin.defaultLayerMask = (int)prop.GetType().GetProperty("value").GetGetMethod().Invoke(prop, []);
        } else
        {
            Plugin.defaultLayerMask = (int)prop;
        }

        Plugin.logger.Info(
            String.Concat("Default LayerMask: ",
                            Convert.ToString(Plugin.defaultLayerMask, 2).PadLeft(32, '0'),
                            " (", Convert.ToString(Plugin.defaultLayerMask), ")"
            )
        );
    }

    [HarmonyPatch(typeof(SDKRender), "Dispose")]
    [HarmonyPrefix]
    static void DisposeSpoutSenders() {
        if (Plugin.cfg.BlankSpoutSenders == false) {return;}

        /*For some reason, the Spout senders just stop working permanently if I disable them.
        The senders still appear in OBS, but there's no frames.
        I've used RUE to check the internal buffer rendertexture. That's still being copied to, but it's not sending.*/

        //For now, the only solution I can think of is keeping the senders around and just blanking them out.
        Vector2Int res = Plugin.resolution;

        RenderTexture rt = RenderTexture.GetTemporary(res.x,res.y,24, RenderTextureFormat.ARGB32);
        
        Plugin.spoutBG.sourceTexture = rt;
        Plugin.spoutFG.sourceTexture = rt;
        Plugin.spoutOptimised.sourceTexture = rt;

        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();

        RenderTexture.ReleaseTemporary(rt);
    }

    [HarmonyPatch(typeof(SDKUtils), "SetCamera")]
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

    [HarmonyPatch(typeof(SDKRender), "RenderBackground")]
    [HarmonyPrefix]
    static void IsPassBG() {
        currentPass = RenPass.BG;
    }

    [HarmonyPatch(typeof(SDKRender), "RenderForeground")]
    [HarmonyPrefix]
    static void IsPassFG() {
        currentPass = RenPass.FG;
    }

    [HarmonyPatch(typeof(SDKRender), "RenderOptimized")]
    [HarmonyPrefix]
    static void IsPassOP() {
        currentPass = RenPass.OP;
    }
}
