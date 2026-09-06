using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;
using HarmonyLib;
using System;


namespace OnAirTap;



class BridgePatchMethods {

    internal static SDKInputFrame Frame = SDKInputFrame.empty;

    internal static SDKResolution Res = SDKResolution.zero;

    internal static void SetInputFrame(ref SDKInputFrame ____inputFrame) {
        ICameraData camDat = Plugin.camDat;
        ____inputFrame.pose.localPosition = camDat.camPos;

        ____inputFrame.pose.localRotation = camDat.camRot;

        ____inputFrame.pose.farClipPlane = Plugin.cfg.FarClip;
        Vector2Int res = Plugin.resolution;

        ____inputFrame.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.vFov, ((float)res.x)/res.y, 0.01f, Plugin.cfg.FarClip);

        // In SDK 2, the projection matrix above is unused. As such, we do now need to set the frame fov separately.
        ____inputFrame.pose.verticalFieldOfView = camDat.vFov;


        Vector3 clipTarget;
        Vector3 camPos = ____inputFrame.pose.localPosition;

        if (Plugin.cfg.ReadClipFromShm && camDat.HasSetting(CamDatCfg.OAT_READCLIP)){
            clipTarget = camDat.clipPos;
        }else {
            clipTarget = Plugin.hmdPos;
        }

        Vector3 clipPos;
        Quaternion clipQuat;

        switch (Plugin.cfg.ClipBehaviour)
        {
            case 1:
                Vector3 camForward = (Quaternion)____inputFrame.pose.localRotation * Vector3.forward;
                clipQuat = ____inputFrame.pose.localRotation;

                if (Plugin.cfg.VerticalClipPlane) {
                    camForward.y = 0;
                    camForward = camForward.normalized;

                    clipQuat = Quaternion.LookRotation(camForward);
                }

                float distance = new Plane(camForward, camPos).GetDistanceToPoint(clipTarget);

                distance = Mathf.Clamp(distance, 0.02f, Plugin.cfg.FarClip -0.01f);

                clipPos = camPos + (camForward * distance);

                break;
            default: 
                clipPos = clipTarget;
                
                Vector3 clipNormal = camPos - clipPos;
                
                if (Plugin.cfg.VerticalClipPlane) {clipNormal.y = 0;}
                
                clipQuat = Quaternion.LookRotation(clipNormal);

                break;
        }

        ____inputFrame.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(clipQuat);

    }

    internal static void UpdateResolution(ref SDKResolution ____resolution) {
        if (Plugin.cfg.ReadResFromShm != true) {return;}

        ICameraData camDat = Plugin.camDat;
        Vector2Int r = camDat.resolution;

        if ( r.x == 0 || r.y == 0){
            Plugin.resolution.x = Plugin.cfg.ResX;
            Plugin.resolution.y = Plugin.cfg.ResY;
        } else {
            Plugin.resolution = r;
        }

        ____resolution.width = Plugin.resolution.x;
        ____resolution.height = Plugin.resolution.y;
    }

}
