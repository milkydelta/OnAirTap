using UnityEngine;
using OnAirTap.Spout;

using HarmonyLib;
using System.Linq;
using System;
using System.Reflection;


namespace OnAirTap;

public class Plugin
{

    internal static AbLogger logger;

    internal static Config cfg = new Config();
    
    internal static SpoutSender spoutFG;
    internal static SpoutSender spoutBG;
    internal static SpoutSender spoutOptimised;
    internal static AbComms nyanShm;
    internal static ICameraData camDat = new ShmStruct1_0{cfg = CamDatCfg.LOG_ON};
    internal static Vector3 hmdPos;

    internal static Vector2Int resolution;

    internal static HarmonyLib.Harmony harmony;

    internal static int defaultLayerMask = 0;

    #if OAT_IL2CPP
    private string[] testedSDKVersions = ["2.1.0"];
    #else
    private string[] testedSDKVersions = ["2.1.2", "1.5.4"];
    #endif

    private Type GetTypeFromName(string name)
    {
        foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
        {
            try {
                foreach (Type t in assem.GetTypes())
                {
                    if (t.FullName == name)
                    {
                        return t;
                    }
                }
            } catch {
                logger.Info("Assembly.GetTypes threw an exception for the following assembly.");
                logger.Info(assem.FullName);
            }

        }
        return null;
    }

    internal string GetSDKVersion()
    {
        string ver = "";
        //I need to do reflection because it's a const.

        Type t = typeof(SDKConstants);
        ver = (string)t.GetField("SDK_VERSION")?.GetValue(null);
        if (ver == "" || ver == null)
        {
            ver = (string)t.GetProperty("SDK_VERSION")?.GetValue(null, null);
        }

        return ver;
    }

    internal void Awake()
    {
        if (harmony == null){
            harmony = new HarmonyLib.Harmony("OnAirTap");
        }


        NativeMethods.Platform plat = NativeMethods.GetPlatform();
        logger.Info("Detected platform: "+plat.ToString()+". Making Comms.");
        if (plat == NativeMethods.Platform.Wine){nyanShm = new LComms();}
        else {nyanShm = new WComms();}

        logger.Info("Opening Comms.");
        nyanShm.Open(cfg.MemoryPath, cfg.ProtoMinorVer);

        ReloadConfig(false);

        harmony.PatchAll(typeof(RenderingPatches));

        string ver = GetSDKVersion();

        logger.Info("Reported LIV SDK version: "+ ver);
        if (!testedSDKVersions.Contains(ver))
        {
            logger.Warn("OnAirTap has not been tested with this SDK version. You may encounter problems.");
        }

        {
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableCreateFrame));
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableSubmit));
            harmony.PatchAll(typeof(InjectionPatchesCommon.Patch_DisableSubmitApplicationOutput));

            if (ver.StartsWith("2.")) {
                harmony.PatchAll(typeof(InjectionPatches2.Patch_IsConnected));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_DisableAddTexture));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_SDKInputFrame));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_SDKResolution));

                harmony.PatchAll(typeof(InjectionPatches2.Patch_CreateCaptureProtocol));
                harmony.PatchAll(typeof(InjectionPatches2.Patch_UpdateBridgeInputFrame));
#if OAT_IL2CPP
                {
                    logger.Info("Harmony patches completed. Performing extra setup.");

                    //SDKSettings.instance._captureProtocolType = SDKBridge.CaptureProtocolType.MOCK;

                    Type t = GetTypeFromName("Il2CppLIV.SDK.Unity.SDKSettings");
                    object o = t?.GetProperty("instance")?.GetValue(null);
                    if (o != null)
                    {
                        PropertyInfo p = t.GetProperty("_captureProtocolType");
                        Array pValues = p?.PropertyType?.GetEnumValues();
                        if (pValues == null){pValues = new object[]{};}

                        foreach (var item in pValues)
                        {
                            if (item.ToString().ToLower() == "mock")
                            {
                                p?.SetValue(o,item);
                            }
                        }

                        //p?.SetValue(o, Convert.ChangeType((uint)1, p.PropertyType)); // Apparently this is no good.
                    }
                }
#endif
            }
            else {
                harmony.PatchAll(typeof(InjectionPatches1.Patch_IsActive));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_DisableAddTexture));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_SDKInputFrame));
                harmony.PatchAll(typeof(InjectionPatches1.Patch_SDKResolution));
            }
        }

        Spout.Misc.RawLog = logger.Info;

        MakeSpoutSenders();

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
            if (!cfg.RenderBG){spoutBG.enabled = false;}
            if (!cfg.RenderFG){spoutFG.enabled = false;}
            if (!cfg.RenderOP){spoutOptimised.enabled = false;}
        }
    }

    internal void MakeSpoutSenders() {
        logger.Info("Making Spout Senders...");
        
        spoutBG?.Dispose();
        spoutFG?.Dispose();
        spoutOptimised?.Dispose();

        spoutBG = new SpoutSender();
        spoutBG.spoutName = "OnAirTap Background";
        spoutBG.verticalFlip = true;

        spoutFG = new SpoutSender();
        spoutFG.spoutName = "OnAirTap Foreground";
        spoutFG.verticalFlip = true;

        spoutOptimised = new SpoutSender();
        spoutOptimised.spoutName = "OnAirTap Foreground [Optimised]";
        spoutOptimised.verticalFlip = true;

        logger.Info("Made Spout Senders.");
    }

    internal void Update() {
        //Not technically necessary, but I'll keep the funciton around in case the future needs it.
    }

    internal void LateUpdate() {
        camDat = nyanShm.Read();

        logger.enabled = camDat.HasSetting(CamDatCfg.LOG_ON);
        if (camDat.HasSetting(CamDatCfg.LOGSPM)) {logger.Info(camDat.ToString());}
    }
}