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

    internal static AbLogger logger;

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

    internal static bool configSpoutSendBG;
    internal static bool configSpoutSendFG;
    internal static bool configSpoutSendOP;
    internal static bool configBlankSpoutSenders;

    internal static float configFarClip;

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

        Harmony.CreateAndPatchAll(typeof(Patches));

        resolution.x = configResX;
        resolution.y = configResY;

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

        string plat = NativeMethods.GetPlatform();
        logger.Info("Detected platform: "+plat+". Making Comms.");
        if (plat == "Wine"){nyanShm = new LComms();}
        else {nyanShm = new WComms();}

        logger.Info("Opening Comms.");
        nyanShm.Open("uk.lum.livnyan.cameradata", 1);

        logger.Info("Core Plugin has completed Awake().");

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