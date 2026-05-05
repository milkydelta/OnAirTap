//using BepInEx;
//using BepInEx.Logging;
//using BepInEx.Configuration;

using UnityEngine;
using LIV.SDK.Unity;
using Klak.Spout;

using HarmonyLib;
using System.Reflection;


namespace OnAirTap;

//[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin //: BaseUnityPlugin
{
    //internal static new ManualLogSource Logger;

    FieldInfo BrInputFrame;
    FieldInfo BrResolution;
    FieldInfo BrIsActive;
    FieldInfo BrDisableSubmit;
    FieldInfo BrDisableSubmitAppOut;
    FieldInfo BrDisableAddTex;
    FieldInfo BrDisableCreateFrame;

    internal static int configResX;
    internal static int configResY;
    internal static bool configRenderBG;
    internal static bool configRenderFG;
    internal static bool configRenderOP;
    internal static bool configGroundPlaneOn;
    internal static float configGroundPlaneHeight;
    internal static bool configReadResFromShm;
    internal static bool configReadClipFromShm;
    internal static bool configVerticalClipPlane;

    internal static GameObject spoutObject;
    internal static SpoutSender spoutFG;
    internal static SpoutSender spoutBG;
    internal static SpoutSender spoutOptimised;
    internal static AbComms nyanShm;
    internal static LIVnyan_dat camDat;
    internal static Vector3 hmdPos;


    bool isActive=false;
        
    void DummyFunction(){}

    internal void Awake()
    {

        Harmony.CreateAndPatchAll(typeof(Patches));

        // Even if we're only using optimised, we still need foreground. Rendering optimised without foreground changes the results of the optimised render.
        SDKInputFrame inFrame = SDKInputFrame.empty;
        if (configRenderBG) {inFrame.features = inFrame.features | FEATURES.BACKGROUND_RENDER;}
        if (configRenderFG) {inFrame.features = inFrame.features | FEATURES.FOREGROUND_RENDER;}
        if (configRenderOP) {inFrame.features = inFrame.features | FEATURES.OPTIMIZED_RENDER;}

        // I cannot, for the life of me, tell what complex clip does.
        //inFrame.features = inFrame.features | FEATURES.COMPLEX_CLIP_PLANE;

        //inFrame.clipPlane.width = 2056;
        //inFrame.clipPlane.height = 2056;
        //inFrame.clipPlane.tesselation = 100.0f;

        if (configGroundPlaneOn){
            inFrame.features = inFrame.features | FEATURES.GROUND_CLIP_PLANE;

            inFrame.groundClipPlane.transform = SDKMatrix4x4.Translate(SDKVector3.up * configGroundPlaneHeight)
            * SDKMatrix4x4.Rotate(SDKQuaternion.Euler(1.5708f,0,0));
        }

        var t = typeof(SDKBridge);

        BrInputFrame= t.GetField("_injection_SDKInputFrame", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrResolution= t.GetField("_injection_SDKResolution", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrIsActive= t.GetField("_injection_IsActive", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrDisableSubmit= t.GetField("_injection_DisableSubmit", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrDisableSubmitAppOut= t.GetField("_injection_DisableSubmitApplicationOutput", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrDisableAddTex= t.GetField("_injection_DisableAddTexture", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
        BrDisableCreateFrame= t.GetField("_injection_DisableCreateFrame", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);

        BrInputFrame.SetValue(null, new SDKBridge.SDKInjection<SDKInputFrame>{
            active = true,
            action = DummyFunction,
            data = inFrame
        });

        BrResolution.SetValue(null, new SDKBridge.SDKInjection<SDKResolution>{
            active = true,
            action = DummyFunction,
            data = new SDKResolution{width=configResX, height=configResY}
        });

        BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
            active = true,
            action = null,
            data = isActive
        });

        BrDisableSubmit.SetValue(null, true);
        BrDisableSubmitAppOut.SetValue(null, true);
        BrDisableAddTex.SetValue(null, true);
        BrDisableCreateFrame.SetValue(null, true);


        if (NativeMethods.GetPlatform() == "Wine"){nyanShm = new LComms();}
        else {nyanShm = new WComms();}

        nyanShm.Open("uk.lum.livnyan.cameradata.v1.0");

    }

    internal void Update() {
        if (!spoutObject){

            spoutObject = new GameObject();
            spoutObject.name = "Acrylonitrile Butadiene Styrene";
            GameObject.DontDestroyOnLoad(spoutObject);

            if (spoutObject)
            {
                spoutBG = spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                spoutBG.enabled=false;
                spoutBG.spoutName="OnAirTap Background";

                spoutFG = spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                spoutFG.enabled=false;
                spoutFG.spoutName = "OnAirTap Foreground";

                spoutOptimised = spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                spoutOptimised.enabled=false;
                spoutOptimised.spoutName = "OnAirTap Foreground [Optimised]";
            }
        }
    }

    internal void LateUpdate() {
        camDat = nyanShm.Read();

        bool CAM_ON = (camDat.cfg & LIVnyan_cfg.CAM_ON) > 0;

        //I've heard reflection is expensive, so I'll put it behind an if.
        if (CAM_ON != isActive){
            BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
                    active = true,
                    action = null,
                    data = CAM_ON
            });
            isActive = CAM_ON;
        }

    }
}


class Patches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        Plugin.spoutBG.sourceTexture = ____backgroundRenderTexture;
        Plugin.spoutBG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        Plugin.spoutFG.sourceTexture = ____foregroundRenderTexture;
        Plugin.spoutFG.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    static void HookSpoutOp(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____optimizedRenderTexture) {
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

        SDKResolution res = ____injection_SDKResolution.data;

        ____injection_SDKInputFrame.data.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, ((float)res.width)/res.height, 0.01f, 1000f);


        Vector3 clipPos;
        Vector3 camPos = ____injection_SDKInputFrame.data.pose.localPosition;

        if (Plugin.configReadClipFromShm){
            //TODO: this is to be implemented later, if livnyan is modified to transmit a tracker down the MMF.
            clipPos = Vector3.zero;
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

        //TODO: this is to be implemented later, if livnyan is modified to transmit window resolution down the MMF.
        Vector2Int Resolution = new Vector2Int();
        Resolution.x=1280;
        Resolution.y=720;

        ____injection_SDKResolution.data.width = Resolution.x;
        ____injection_SDKResolution.data.height = Resolution.y;
    }


    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        Plugin.spoutBG.CaptureFrame();
        //Plugin.spoutFG.CaptureFrame();
        Plugin.spoutOptimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderForeground")]
    [HarmonyPrefix]
    static void GetCameraForClipPlane( ref SDKRender __instance) {
        Plugin.hmdPos = __instance.liv.stageWorldToLocalMatrix.MultiplyPoint3x4(__instance.liv.HMDCamera.transform.position);
    }
}
