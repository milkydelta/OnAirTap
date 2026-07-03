using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;

class BridgePatches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "UpdateBridgeInputFrame")]
    [HarmonyPrefix]
    static bool FixNonsense(ref SDKInputFrame ____inputFrame)
    {
        //If I try to patch SDKBridge::UpdateInputFrame, HarmonyX fails to patch with an AmbiguousMatchException.
        //I have no idea why. There's no method with the same name in SDKBridge, and no other patches target that method.

        ____inputFrame = InjectionPatches.blankFrame;

        return false;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "UpdateBridgeInputFrame")]
    [HarmonyPostfix]
    static void SetInputFrame(ref SDKInputFrame ____inputFrame) {
        LIVnyan_dat camDat = Plugin.camDat;
        ____inputFrame.pose.localPosition.x = camDat.x;
        ____inputFrame.pose.localPosition.y = camDat.y;
        ____inputFrame.pose.localPosition.z = camDat.z;

        ____inputFrame.pose.localRotation.w = camDat.qw;
        ____inputFrame.pose.localRotation.x = camDat.qx;
        ____inputFrame.pose.localRotation.y = camDat.qy;
        ____inputFrame.pose.localRotation.z = camDat.qz;

        ____inputFrame.pose.farClipPlane = Plugin.cfg.FarClip;
        Vector2Int res = Plugin.resolution;

        ____inputFrame.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, ((float)res.x)/res.y, 0.01f, Plugin.cfg.FarClip);


        Vector3 clipTarget;
        Vector3 camPos = ____inputFrame.pose.localPosition;

        if (Plugin.cfg.ReadClipFromShm){
            clipTarget = new Vector3(camDat.clipX, camDat.clipY, camDat.clipZ);
        }else {
            clipTarget = Plugin.hmdPos;
        }

        Vector3 clipPos;
        Quaternion clipQuat;

        switch (Plugin.cfg.ClipBehaviour)
        {
            case 2:
                Vector3 camForward = (Quaternion)____inputFrame.pose.localRotation * Vector3.forward;
                float distance = new Plane(camForward, camPos).GetDistanceToPoint(clipTarget);

                distance = Mathf.Clamp(distance, 0.02f, Plugin.cfg.FarClip -0.01f);

                clipPos = camPos + (camForward * distance);
                clipQuat = ____inputFrame.pose.localRotation;

                break;
            case 1: // equivalent to true verticalclipplane
                clipPos = clipTarget;
                Vector3 clipNormal = camPos - clipPos;
                clipNormal.y = 0;
                clipQuat = Quaternion.LookRotation(clipNormal);
                
                break;
            default: //equivalent to false verticalclipplane
                clipPos = clipTarget;
                clipQuat = Quaternion.LookRotation(camPos - clipPos);

                break;
        }

        ____inputFrame.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(clipQuat);

    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "UpdateBridgeResolution")]
    [HarmonyPostfix]
    static void UpdateResolution(ref SDKResolution ____resolution) {
        if (Plugin.cfg.ReadResFromShm != true) {return;}

        LIVnyan_dat camDat = Plugin.camDat;

        if ( camDat.resX == 0 || camDat.resY == 0){
            Plugin.resolution.x = Plugin.cfg.ResX;
            Plugin.resolution.y = Plugin.cfg.ResY;
        } else {
            Plugin.resolution.x = camDat.resX;
            Plugin.resolution.y = camDat.resY;
        }

        ____resolution.width = Plugin.resolution.x;
        ____resolution.height = Plugin.resolution.y;
    }

}
