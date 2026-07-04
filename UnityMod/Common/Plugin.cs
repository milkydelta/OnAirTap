//using BepInEx;
//using BepInEx.Logging;
//using BepInEx.Configuration;

using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;

using HarmonyLib;
//using System.Reflection;
using System.Linq;


namespace OnAirTap;

//[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin //: BaseUnityPlugin
{
    //internal static new ManualLogSource Logger;

    internal static AbLogger logger;

    //FieldInfo BrInputFrame;
    //FieldInfo BrResolution;
    //FieldInfo BrIsActive;
    //FieldInfo BrDisableSubmit;
    //FieldInfo BrDisableSubmitAppOut;
    //FieldInfo BrDisableAddTex;
    //FieldInfo BrDisableCreateFrame;

    internal static Config cfg = new Config();
    
    internal static GameObject spoutObject;
    internal static SpoutSender spoutFG;
    internal static SpoutSender spoutBG;
    internal static SpoutSender spoutOptimised;
    internal static AbComms nyanShm;
    internal static LIVnyan_dat camDat;
    internal static Vector3 hmdPos;

    internal static Vector2Int resolution;

    internal static Harmony harmony;

    private string[] testedSDKVersions = ["2.1.2", "1.5.4"];


    internal void Awake()
    {
        harmony = new Harmony("OnAirTap");

        var t = typeof(SDKBridge);

        NativeMethods.Platform plat = NativeMethods.GetPlatform();
        logger.Info("Detected platform: "+plat.ToString()+". Making Comms.");
        if (plat == NativeMethods.Platform.Wine){nyanShm = new LComms();}
        else {nyanShm = new WComms();}

        logger.Info("Opening Comms.");
        nyanShm.Open("uk.lum.livnyan.cameradata", cfg.ProtoMinorVer);

        ReloadConfig(false);

        harmony.PatchAll(typeof(RenderingPatches));

        logger.Info("Reported LIV SDK version: "+ SDKConstants.SDK_VERSION);
        if (!testedSDKVersions.Contains(SDKConstants.SDK_VERSION))
        {
            logger.Warn("OnAirTap has not been tested with this SDK version. You may encounter problems.");
        }

        {
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableCreateFrame));
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableSubmit));
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableSubmitApplicationOutput));

            if (SDKConstants.SDK_VERSION.StartsWith("2.")) {
                harmony.PatchAll(typeof(InjectionPatches2.Patch_IsConnected));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_DisableAddTexture));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_SDKInputFrame));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_SDKResolution));

                harmony.PatchAll(typeof(InjectionPatches2.Patch_CreateCaptureProtocol));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_UpdateBridgeInputFrame));
            }
            else {
                harmony.PatchAll(typeof(InjectionPatches1.Patch_IsActive));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_DisableAddTexture));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_SDKInputFrame));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_SDKResolution));
            }
        }

        logger.Info("Core Plugin has completed Awake().");

    }

    internal void ReloadConfig( bool notInitial = false)
    {
        resolution.x = cfg.ResX;
        resolution.y = cfg.ResY;

        SDKInputFrame inFrame = SDKInputFrame.empty;
        if (cfg.RenderBG) {inFrame.features = inFrame.features | FEATURES.BACKGROUND_RENDER;}
        if (cfg.RenderFG) {inFrame.features = inFrame.features | FEATURES.FOREGROUND_RENDER;}
        if (cfg.RenderOP) {inFrame.features = inFrame.features | FEATURES.OPTIMIZED_RENDER;}

        inFrame.features = inFrame.features | FEATURES.FIX_FOREGROUND_ALPHA;

        if (cfg.GroundPlaneOn){
            inFrame.features = inFrame.features | FEATURES.GROUND_CLIP_PLANE;

            inFrame.groundClipPlane.transform = SDKMatrix4x4.Translate(SDKVector3.up * cfg.GroundPlaneHeight)
            * SDKMatrix4x4.Rotate(SDKQuaternion.Euler(1.5708f,0,0));
        }

        BridgePatchMethods.Frame = inFrame;

        BridgePatchMethods.Res = new SDKResolution{width=cfg.ResX, height=cfg.ResY};

        if (notInitial){
            if (!cfg.SpoutSendBG){spoutBG.captureMethod = CaptureMethod.GameView;}
            if (!cfg.SpoutSendFG){spoutFG.captureMethod = CaptureMethod.GameView;}
            if (!cfg.SpoutSendOP){spoutOptimised.captureMethod = CaptureMethod.GameView;}
        }
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

                logger.Info("Successfully made Spout Holding Object.");
            }
        }
    }

    internal void LateUpdate() {
        camDat = nyanShm.Read();

        bool CAM_ON = (camDat.cfg & LIVnyan_cfg.CAM_ON) != 0;

        //I've heard reflection is expensive, so I'll put it behind an if.
        // if (CAM_ON != isActive){
        //     BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
        //             active = true,
        //             action = null,
        //             data = CAM_ON
        //     });
        //     isActive = CAM_ON;
        // }

        logger.enabled = (camDat.cfg & LIVnyan_cfg.LOG_ON) != 0;
        if ((camDat.cfg & LIVnyan_cfg.LOGSPM) != 0) {logger.Info(camDat.ToString());}
        //logger.Info("LOGSPAM: "+camDat.ToString());;

    }
}