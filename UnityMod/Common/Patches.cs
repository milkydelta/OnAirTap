using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;

class Patches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        if (Plugin.configSpoutSendBG == false) {return;}
        Plugin.spoutBG.sourceTexture = ____backgroundRenderTexture;
        Plugin.spoutBG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        if (Plugin.configSpoutSendFG == false) {return;}
        Plugin.spoutFG.sourceTexture = ____foregroundRenderTexture;
        Plugin.spoutFG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    static void HookSpoutOp(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____optimizedRenderTexture) {
        if (Plugin.configSpoutSendOP == false) {return;}
        Plugin.spoutOptimised.sourceTexture = ____optimizedRenderTexture;
        Plugin.spoutOptimised.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "UpdateInputFrame")]
    [HarmonyPrefix]
    static void SetInputFrame(ref SDKBridge.SDKInjection<SDKInputFrame> ____injection_SDKInputFrame, ref SDKBridge.SDKInjection<SDKResolution> ____injection_SDKResolution) {
        LIVnyan_dat camDat = Plugin.camDat;
        ____injection_SDKInputFrame.data.pose.localPosition.x = camDat.x;
        ____injection_SDKInputFrame.data.pose.localPosition.y = camDat.y;
        ____injection_SDKInputFrame.data.pose.localPosition.z = camDat.z;

        ____injection_SDKInputFrame.data.pose.localRotation.w = camDat.qw;
        ____injection_SDKInputFrame.data.pose.localRotation.x = camDat.qx;
        ____injection_SDKInputFrame.data.pose.localRotation.y = camDat.qy;
        ____injection_SDKInputFrame.data.pose.localRotation.z = camDat.qz;

        ____injection_SDKInputFrame.data.pose.farClipPlane = Plugin.configFarClip;
        SDKResolution res = ____injection_SDKResolution.data;

        ____injection_SDKInputFrame.data.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, ((float)res.width)/res.height, 0.01f, Plugin.configFarClip);


        Vector3 clipPos;
        Vector3 camPos = ____injection_SDKInputFrame.data.pose.localPosition;

        if (Plugin.configReadClipFromShm){
            clipPos = new Vector3(camDat.clipX, camDat.clipY, camDat.clipZ);
        }else {
            clipPos = Plugin.hmdPos;
        }

        if (Plugin.configVerticalClipPlane){
            camPos.y = clipPos.y;
        }

        Quaternion quat = Quaternion.LookRotation(camPos - clipPos);

        ____injection_SDKInputFrame.data.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(quat);

    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "GetResolution")]
    [HarmonyPrefix]
    static void UpdateResolution(ref SDKBridge.SDKInjection<SDKResolution> ____injection_SDKResolution) {
        if (Plugin.configReadResFromShm != true) {return;}

        LIVnyan_dat camDat = Plugin.camDat;

        if ( camDat.resX == 0 || camDat.resY == 0){
            Plugin.resolution.x = Plugin.configResX;
            Plugin.resolution.y = Plugin.configResY;
        } else {
            Plugin.resolution.x = camDat.resX;
            Plugin.resolution.y = camDat.resY;
        }

        ____injection_SDKResolution.data.width = Plugin.resolution.x;
        ____injection_SDKResolution.data.height = Plugin.resolution.y;
    }


    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderForeground")]
    [HarmonyPrefix]
    static void GetCameraForClipPlane( ref SDKRender __instance) {
        Plugin.hmdPos = __instance.liv.stageWorldToLocalMatrix.MultiplyPoint3x4(__instance.liv.HMDCamera.transform.position);
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Dispose")]
    [HarmonyPrefix]
    static void DisposeSpoutSenders() {
        if (Plugin.configBlankSpoutSenders == false) {return;}

        //Plugin.spoutBG.OnDisable();
        //Plugin.spoutFG.OnDisable();
        //Plugin.spoutOptimised.OnDisable();

        /*For some reason, the Spout senders just stop working permanently if I disable them.
        The senders still appear in OBS, but there's no frames.
        I've used RUE to check the internal buffer rendertexture. That's still being copied to, but it's not sending.*/

        //For now, the only solution I can think of is keeping the senders around and just blanking them out.
        Plugin.spoutBG.sourceTexture = new RenderTexture(1920,1080,24);
        Plugin.spoutFG.sourceTexture = new RenderTexture(1920,1080,24);
        Plugin.spoutOptimised.sourceTexture = new RenderTexture(1920,1080,24);

        Plugin.spoutBG.CaptureFrame();
        Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPrefix]
    static void DoLayerMasks( ref SDKRender __instance) {
        if (Plugin.configLayerMask == 0){return;}

        __instance.liv.spectatorLayerMask = Plugin.configLayerMask;
        //1979644927

        //Plugin.logger.Info(
        //Convert.ToString((int)__instance.liv.spectatorLayerMask)
        //);
    }
}
