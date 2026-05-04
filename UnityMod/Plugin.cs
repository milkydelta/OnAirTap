using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

using UnityEngine;
using LIV.SDK.Unity;
using Klak.Spout;

using HarmonyLib;
using System.Reflection;


namespace OnAirTap;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    FieldInfo BrInputFrame;
    FieldInfo BrResolution;
    FieldInfo BrIsActive;
    FieldInfo BrDisableSubmit;
    FieldInfo BrDisableSubmitAppOut;
    FieldInfo BrDisableAddTex;
    FieldInfo BrDisableCreateFrame;

    internal static ConfigEntry<int> configResX;
    internal static ConfigEntry<int> configResY;
    internal static ConfigEntry<bool> configRenderBG;
    internal static ConfigEntry<bool> configRenderFG;
    internal static ConfigEntry<bool> configRenderOP;
    internal static ConfigEntry<bool> configGroundPlaneOn;
    internal static ConfigEntry<float> configGroundPlaneHeight;
    internal static ConfigEntry<bool> configReadResFromShm;
    internal static ConfigEntry<bool> configReadClipFromShm;
    internal static ConfigEntry<bool> configVerticalClipPlane;


    bool isActive=false;
        
    void DummyFunction(){}

    private void BindConfigs(){
        configResX = Config.Bind("Resolution","X",1920);
        configResY = Config.Bind("Resolution","Y",1080);

        if (configResX.Value <= 0){configResX.Value = 1920;}
        if (configResY.Value <= 0){configResY.Value = 1080;}

        configRenderBG = Config.Bind("RenderPasses","Background",true);
        configRenderFG = Config.Bind("RenderPasses","Foreground",true);
        configRenderOP = Config.Bind("RenderPasses","Optimised",true);

        configGroundPlaneOn = Config.Bind("ClipPlanes","GroundEnabled",true);
        configGroundPlaneHeight = Config.Bind("ClipPlanes","GroundElevation",0.01f, "This is in metres, I think.");
        configVerticalClipPlane = Config.Bind("ClipPlanes", "ClipShouldBeVertical", true);

        configReadResFromShm = Config.Bind("OAT_MMF_Data","ReadWindowResolution", false);
        configReadClipFromShm = Config.Bind("OAT_MMF_Data","ReadClipPlaneLocation", false);
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        BindConfigs();

        Harmony.CreateAndPatchAll(typeof(Patches));

        // Even if we're only using optimised, we still need foreground. Rendering optimised without foreground changes the results of the optimised render.
        SDKInputFrame inFrame = SDKInputFrame.empty;
        if (configRenderBG.Value) {inFrame.features = inFrame.features | FEATURES.BACKGROUND_RENDER;}
        if (configRenderFG.Value) {inFrame.features = inFrame.features | FEATURES.FOREGROUND_RENDER;}
        if (configRenderOP.Value) {inFrame.features = inFrame.features | FEATURES.OPTIMIZED_RENDER;}

        // I cannot, for the life of me, tell what complex clip does.
        //inFrame.features = inFrame.features | FEATURES.COMPLEX_CLIP_PLANE;

        //inFrame.clipPlane.width = 2056;
        //inFrame.clipPlane.height = 2056;
        //inFrame.clipPlane.tesselation = 100.0f;

        if (configGroundPlaneOn.Value){
            inFrame.features = inFrame.features | FEATURES.GROUND_CLIP_PLANE;

            inFrame.groundClipPlane.transform = SDKMatrix4x4.Translate(SDKVector3.up * configGroundPlaneHeight.Value)
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
            data = new SDKResolution{width=configResX.Value, height=configResY.Value}
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

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        if (NativeMethods.GetPlatform() == "Wine"){HoldingArea.shm = new LComms();}
        else {HoldingArea.shm = new WComms();}

        HoldingArea.shm.Open("uk.lum.livnyan.cameradata.v1.0");

    }

    void Update() {
        if (!HoldingArea.spoutObject){

            HoldingArea.spoutObject = new GameObject();
            HoldingArea.spoutObject.name = "Acrylonitrile Butadiene Styrene";
            GameObject.DontDestroyOnLoad(HoldingArea.spoutObject);

            if (HoldingArea.spoutObject)
            {
                HoldingArea.bg = HoldingArea.spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                HoldingArea.bg.enabled=false;
                HoldingArea.bg.spoutName="OnAirTap Background";

                HoldingArea.fg = HoldingArea.spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                HoldingArea.fg.enabled=false;
                HoldingArea.fg.spoutName = "OnAirTap Foreground";

                HoldingArea.optimised = HoldingArea.spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                HoldingArea.optimised.enabled=false;
                HoldingArea.optimised.spoutName = "OnAirTap Foreground [Optimised]";
            }
        }
    }

    void LateUpdate() {
        HoldingArea.camDat = HoldingArea.shm.Read();

        bool CAM_ON = (HoldingArea.camDat.cfg & LIVnyan_cfg.CAM_ON) > 0;

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

static class HoldingArea{
    public static GameObject spoutObject;
    public static SpoutSender fg;
    public static SpoutSender bg;
    public static SpoutSender optimised;

    public static AbComms shm;

    public static Vector3 hmdPos;

    public static LIVnyan_dat camDat;
}


class Patches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        HoldingArea.bg.sourceTexture = ____backgroundRenderTexture;
        HoldingArea.bg.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        HoldingArea.fg.sourceTexture = ____foregroundRenderTexture;
        HoldingArea.fg.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateOptimizedTexture")]
    [HarmonyPostfix]
    static void HookSpoutOp(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____optimizedRenderTexture) {
        HoldingArea.optimised.sourceTexture = ____optimizedRenderTexture;
        HoldingArea.optimised.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "UpdateInputFrame")]
    [HarmonyPrefix]
    static void SetInputFrame(ref SDKBridge.SDKInjection<SDKInputFrame> ____injection_SDKInputFrame, ref SDKBridge.SDKInjection<SDKResolution> ____injection_SDKResolution) {
        LIVnyan_dat camDat = HoldingArea.camDat;
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

        if (Plugin.configReadClipFromShm.Value){
            //TODO: this is to be implemented later, if livnyan is modified to transmit a tracker down the MMF.
            clipPos = Vector3.zero;
        }else {
            clipPos = HoldingArea.hmdPos;
        }

        if (Plugin.configVerticalClipPlane.Value){
            camPos.y = clipPos.y;
        }

        Quaternion quat = Quaternion.LookRotation(camPos - clipPos);

        ____injection_SDKInputFrame.data.clipPlane.transform = SDKMatrix4x4.Translate(clipPos) * SDKMatrix4x4.Rotate(quat);

    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "GetResolution")]
    [HarmonyPrefix]
    static void UpdateResolution(ref SDKBridge.SDKInjection<SDKResolution> ____injection_SDKResolution) {
        if (Plugin.configReadResFromShm.Value != true) {return;}

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
        HoldingArea.bg.CaptureFrame();
        //HoldingArea.fg.CaptureFrame();
        HoldingArea.optimised.CaptureFrame();
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "RenderForeground")]
    [HarmonyPrefix]
    static void GetCameraForClipPlane( ref SDKRender __instance) {
        HoldingArea.hmdPos = __instance.liv.stageWorldToLocalMatrix.MultiplyPoint3x4(__instance.liv.HMDCamera.transform.position);
    }
}
