using BepInEx;
using BepInEx.Logging;

using UnityEngine;
using LIV.SDK.Unity;
using Klak.Spout;

using HarmonyLib;
using System.Reflection;

using System.IO;
using System.IO.MemoryMappedFiles;


namespace OnAirTap;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    //Klak.Spout.Sender spout;
    //Klak.Spout.SpoutSender spoutsend;

    GameObject LivObject;

    FieldInfo BrInputFrame;
    FieldInfo BrResolution;
    FieldInfo BrIsActive;
    FieldInfo BrDisableSubmit;
    FieldInfo BrDisableSubmitAppOut;
    FieldInfo BrDisableAddTex;
    FieldInfo BrDisableCreateFrame;

    SDKInputFrame inFrame;
        
    void DummyFunction(){}

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(typeof(Patches));

        inFrame = SDKInputFrame.empty;
        inFrame.features = inFrame.features | FEATURES.BACKGROUND_RENDER;
        inFrame.features = inFrame.features | FEATURES.FOREGROUND_RENDER;

        // I cannot, for the life of me, tell what complex clip does.
        //inFrame.features = inFrame.features | FEATURES.COMPLEX_CLIP_PLANE;

        //inFrame.clipPlane.width = 2056;
        //inFrame.clipPlane.height = 2056;
        //inFrame.clipPlane.tesselation = 100.0f;

        inFrame.features = inFrame.features | FEATURES.GROUND_CLIP_PLANE;

        inFrame.groundClipPlane.transform = SDKMatrix4x4.Translate(SDKVector3.up * 0.01f)
        * SDKMatrix4x4.Rotate(SDKQuaternion.Euler(1.5708f,0,0));

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
            data = new SDKResolution{width=1920, height=1080}
        });

        //BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
        //    active = true,
        //    action = null,
        //    data = true
        //});

        BrDisableSubmit.SetValue(null, true);
        BrDisableSubmitAppOut.SetValue(null, true);
        BrDisableAddTex.SetValue(null, true);
        BrDisableCreateFrame.SetValue(null, true);


        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        HoldingArea.mmf = MemoryMappedFile.CreateOrOpen("uk.lum.livnyan.cameradata.v1.0", (sizeof(float) * 8)+sizeof(int));
        HoldingArea.mmfView = HoldingArea.mmf.CreateViewAccessor(0, (sizeof(float) * 8)+sizeof(int), MemoryMappedFileAccess.Read);

        if (NativeMethods.GetPlatform() == "Wine"){HoldingArea.shm = new LComms();}
        else {HoldingArea.shm = new WComms();}



        HoldingArea.shm.Open("uk.lum.livnyan.cameradata.v1.0");

    }

    void Update() {
        if (!HoldingArea.spoutObject){
            HoldingArea.spoutObject = new GameObject();
            HoldingArea.spoutObject.name = "Acrylobutadiene Styrene";
            GameObject.DontDestroyOnLoad(HoldingArea.spoutObject);


            if (HoldingArea.spoutObject)
            {
                HoldingArea.bg = HoldingArea.spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                HoldingArea.bg.enabled=false;
                HoldingArea.bg.spoutName="OnAirTap Background";

                HoldingArea.fg = HoldingArea.spoutObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
                HoldingArea.fg.enabled=false;
                HoldingArea.fg.spoutName = "OnAirTap Foreground";

                BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
                    active = true,
                    action = null,
                    data = true
                });
            }
        }


    }

//    void Update() {
//        if (!LivObject){
//            Logger.LogInfo("Finding LivObject");
//            LivObject = GameObject.Find("LIV");
//
//            if (LivObject){
//                HoldingArea.bg = LivObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
//                HoldingArea.bg.enabled=false;
//                HoldingArea.bg.spoutName="OnAirTap Background";
//                HoldingArea.fg = LivObject.AddComponent(typeof(SpoutSender)) as SpoutSender;
//                HoldingArea.fg.enabled=false;
//                HoldingArea.fg.spoutName = "OnAirTap Foreground";
//                BrIsActive.SetValue(null, new SDKBridge.SDKInjection<bool>{
//                    active = true,
//                    action = null,
//                    data = true
//                });
//            }
//        }
//
//
//    }
}

static class HoldingArea{
    public static GameObject spoutObject;
    public static SpoutSender fg;
    public static SpoutSender bg;

    public static MemoryMappedFile mmf;
    public static MemoryMappedViewAccessor mmfView;
    public static float[] cameraData = new float[9];

    public static AbComms shm;
}


class Patches {

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateBackgroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutBG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____backgroundRenderTexture) {
        //var bg = __instance.liv.gameObject.GetComponent<SpoutSender>();
        HoldingArea.bg.sourceTexture = ____backgroundRenderTexture;
        HoldingArea.bg.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "CreateForegroundTexture")]
    [HarmonyPostfix]
    static void HookSpoutFG(ref LIV.SDK.Unity.SDKRender __instance, RenderTexture ____foregroundRenderTexture) {
        HoldingArea.fg.sourceTexture = ____foregroundRenderTexture;
        HoldingArea.fg.captureMethod = CaptureMethod.Texture;
    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKBridge), "UpdateInputFrame")]
    [HarmonyPrefix]
    static void SetInputFrame(ref SDKBridge.SDKInjection<SDKInputFrame> ____injection_SDKInputFrame) {
        //HoldingArea.mmfView.ReadArray<float>(0,HoldingArea.cameraData,0,8);
        //____injection_SDKInputFrame.data.pose.localPosition.x = HoldingArea.cameraData[0];
        //____injection_SDKInputFrame.data.pose.localPosition.y = HoldingArea.cameraData[1];
        //____injection_SDKInputFrame.data.pose.localPosition.z = HoldingArea.cameraData[2];

        //____injection_SDKInputFrame.data.pose.localRotation.w = HoldingArea.cameraData[3];
        //____injection_SDKInputFrame.data.pose.localRotation.x = HoldingArea.cameraData[4];
        //____injection_SDKInputFrame.data.pose.localRotation.y = HoldingArea.cameraData[5];
        //____injection_SDKInputFrame.data.pose.localRotation.z = HoldingArea.cameraData[6];


        //float vfov = HoldingArea.cameraData[7];
        //____injection_SDKInputFrame.data.pose.projectionMatrix = SDKMatrix4x4.Perspective(vfov, 1920f/1080, 0.01f, 1000f);

        LIVnyan_dat camDat = HoldingArea.shm.Read();
        ____injection_SDKInputFrame.data.pose.localPosition.x = camDat.x;
        ____injection_SDKInputFrame.data.pose.localPosition.y = camDat.y;
        ____injection_SDKInputFrame.data.pose.localPosition.z = camDat.z;

        ____injection_SDKInputFrame.data.pose.localRotation.w = camDat.qw;
        ____injection_SDKInputFrame.data.pose.localRotation.x = camDat.qx;
        ____injection_SDKInputFrame.data.pose.localRotation.y = camDat.qy;
        ____injection_SDKInputFrame.data.pose.localRotation.z = camDat.qz;

        ____injection_SDKInputFrame.data.pose.projectionMatrix = SDKMatrix4x4.Perspective(camDat.fov, 1920f/1080, 0.01f, 1000f);


    }

    [HarmonyPatch(typeof(LIV.SDK.Unity.SDKRender), "Render")]
    [HarmonyPostfix]
    static void UpdateSpoutSenders( ref SDKRender __instance) {
        HoldingArea.bg.CaptureFrame();
        HoldingArea.fg.CaptureFrame();

    }
}
