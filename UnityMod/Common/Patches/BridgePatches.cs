using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;

class BridgePatches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "UpdateBridgeInputFrame")]
    [HarmonyPostfix]
    static void SetInputFrame(ref SDKInputFrame ___inputFrame) {
        LIVnyan_dat camDat = Plugin.camDat;
        ___inputFrame.pose.localPosition.x = camDat.x;
        ___inputFrame.pose.localPosition.y = camDat.y;
        ___inputFrame.pose.localPosition.z = camDat.z;

        ___inputFrame.pose.localRotation.w = camDat.qw;
        ___inputFrame.pose.localRotation.x = camDat.qx;
        ___inputFrame.pose.localRotation.y = camDat.qy;
        ___inputFrame.pose.localRotation.z = camDat.qz;

        ___inputFrame.pose.farClipPlane = Plugin.cfg.FarClip;
        Vector2Int res = Plugin.resolution;

        ___inputFrame.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, ((float)res.x)/res.y, 0.01f, Plugin.cfg.FarClip);


        Vector3 clipTarget;
        Vector3 camPos = ___inputFrame.pose.localPosition;

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
                Vector3 camForward = (Quaternion)___inputFrame.pose.localRotation * Vector3.forward;
                float distance = new Plane(camForward, camPos).GetDistanceToPoint(clipTarget);

                distance = Mathf.Clamp(distance, 0.02f, Plugin.cfg.FarClip -0.01f);

                clipPos = camPos + (camForward * distance);
                clipQuat = ___inputFrame.pose.localRotation;

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

        ___inputFrame.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(clipQuat);

    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "UpdateBridgeResolution")]
    [HarmonyPostfix]
    static void UpdateResolution(ref SDKResolution ___resolution) {
        if (Plugin.cfg.ReadResFromShm != true) {return;}

        LIVnyan_dat camDat = Plugin.camDat;

        if ( camDat.resX == 0 || camDat.resY == 0){
            Plugin.resolution.x = Plugin.cfg.ResX;
            Plugin.resolution.y = Plugin.cfg.ResY;
        } else {
            Plugin.resolution.x = camDat.resX;
            Plugin.resolution.y = camDat.resY;
        }

        ___resolution.width = Plugin.resolution.x;
        ___resolution.height = Plugin.resolution.y;
    }

}
