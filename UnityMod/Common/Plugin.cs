//using BepInEx;
//using BepInEx.Logging;
//using BepInEx.Configuration;

using UnityEngine;
using LIV.SDK.Unity;
using OnAirTap.Spout;

using HarmonyLib;
using System.Reflection;


namespace OnAirTap;

//[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin //: BaseUnityPlugin
{
    //internal static new ManualLogSource Logger;

    internal static AbLogger logger;

    FieldInfo BrInputFrame;
    FieldInfo BrResolution;
    FieldInfo BrIsActive;
    FieldInfo BrDisableSubmit;
    FieldInfo BrDisableSubmitAppOut;
    FieldInfo BrDisableAddTex;
    FieldInfo BrDisableCreateFrame;

    internal static Config cfg = new Config();
    
    internal static GameObject spoutObject;
    internal static SpoutSender spoutFG;
    internal static SpoutSender spoutBG;
    internal static SpoutSender spoutOptimised;
    internal static AbComms nyanShm;
    internal static LIVnyan_dat camDat;
    internal static Vector3 hmdPos;

    internal static Vector2Int resolution;


    bool isActive=false;
        
    void DummyFunction(){}

    internal void Awake()
    {
        var t = typeof(SDKBridge);

        BrInputFrame= t.GetField("_injection_SDKInputFrame", BindingFlags.NonPublic|BindingFlags.Static);
        BrResolution= t.GetField("_injection_SDKResolution", BindingFlags.NonPublic|BindingFlags.Static);
        BrIsActive= t.GetField("_injection_IsActive", BindingFlags.NonPublic|BindingFlags.Static);
        BrDisableSubmit= t.GetField("_injection_DisableSubmit", BindingFlags.NonPublic|BindingFlags.Static);
        BrDisableSubmitAppOut= t.GetField("_injection_DisableSubmitApplicationOutput", BindingFlags.NonPublic|BindingFlags.Static);
        BrDisableAddTex= t.GetField("_injection_DisableAddTexture", BindingFlags.NonPublic|BindingFlags.Static);
        BrDisableCreateFrame= t.GetField("_injection_DisableCreateFrame", BindingFlags.NonPublic|BindingFlags.Static);

        BrInputFrame.SetValue(null, new SDKBridge.SDKInjection<SDKInputFrame>{
            active = true,
            action = DummyFunction,
            data = SDKInputFrame.empty
        });

        BrResolution.SetValue(null, new SDKBridge.SDKInjection<SDKResolution>{
            active = true,
            action = DummyFunction,
            data = SDKResolution.zero
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

        string plat = NativeMethods.GetPlatform();
        logger.Info("Detected platform: "+plat+". Making Comms.");
        if (plat == "Wine"){nyanShm = new LComms();}
        else {nyanShm = new WComms();}

        logger.Info("Opening Comms.");
        nyanShm.Open("uk.lum.livnyan.cameradata", cfg.ProtoMinorVer);

        ReloadConfig(false);

        Harmony.CreateAndPatchAll(typeof(Patches));

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

        BrInputFrame.SetValue(null, new SDKBridge.SDKInjection<SDKInputFrame>{
            active = true,
            action = DummyFunction,
            data = inFrame
        });

        BrResolution.SetValue(null, new SDKBridge.SDKInjection<SDKResolution>{
            active = true,
            action = DummyFunction,
            data = new SDKResolution{width=cfg.ResX, height=cfg.ResY}
        });

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
        if (CAM_ON != isActive){
            BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
                    active = true,
                    action = null,
                    data = CAM_ON
            });
            isActive = CAM_ON;
        }

        logger.enabled = (camDat.cfg & LIVnyan_cfg.LOG_ON) != 0;
        if ((camDat.cfg & LIVnyan_cfg.LOGSPM) != 0) {logger.Info(camDat.ToString());}
        //logger.Info("LOGSPAM: "+camDat.ToString());;

    }
}