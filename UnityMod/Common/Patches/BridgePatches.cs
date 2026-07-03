using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;

class BridgePatches {

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

        ____injection_SDKInputFrame.data.pose.farClipPlane = Plugin.cfg.FarClip;
        SDKResolution res = ____injection_SDKResolution.data;

        ____injection_SDKInputFrame.data.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, ((float)res.width)/res.height, 0.01f, Plugin.cfg.FarClip);


        Vector3 clipTarget;
        Vector3 camPos = ____injection_SDKInputFrame.data.pose.localPosition;

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
                Vector3 camForward = (Quaternion)____injection_SDKInputFrame.data.pose.localRotation * Vector3.forward;
                float distance = new Plane(camForward, camPos).GetDistanceToPoint(clipTarget);

                distance = Mathf.Clamp(distance, 0.02f, Plugin.cfg.FarClip -0.01f);

                clipPos = camPos + (camForward * distance);
                clipQuat = ____injection_SDKInputFrame.data.pose.localRotation;

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

        ____injection_SDKInputFrame.data.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(clipQuat);

    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "GetResolution")]
    [HarmonyPrefix]
    static void UpdateResolution(ref SDKBridge.SDKInjection<SDKResolution> ____injection_SDKResolution) {
        if (Plugin.cfg.ReadResFromShm != true) {return;}

        LIVnyan_dat camDat = Plugin.camDat;

        if ( camDat.resX == 0 || camDat.resY == 0){
            Plugin.resolution.x = Plugin.cfg.ResX;
            Plugin.resolution.y = Plugin.cfg.ResY;
        } else {
            Plugin.resolution.x = camDat.resX;
            Plugin.resolution.y = camDat.resY;
        }

        ____injection_SDKResolution.data.width = Plugin.resolution.x;
        ____injection_SDKResolution.data.height = Plugin.resolution.y;
    }

}
