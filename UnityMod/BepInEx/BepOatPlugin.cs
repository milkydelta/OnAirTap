using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

//using UnityEngine;
//using LIV.SDK.Unity;
//using Klak.Spout;

//using HarmonyLib;
//using System.Reflection;


namespace OnAirTap;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BepOatPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

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

    internal static ConfigEntry<bool> configSpoutSendBG;
    internal static ConfigEntry<bool> configSpoutSendFG;
    internal static ConfigEntry<bool> configSpoutSendOP;
    internal static ConfigEntry<bool> configBlankSpoutSenders;

    internal static ConfigEntry<float> configFarClip;

    internal static ConfigEntry<ushort> configProtoMinorVer;

    Plugin plug;

    private void BindConfigs(){
        configResX = Config.Bind("Resolution","X",1920);
        configResY = Config.Bind("Resolution","Y",1080);

        if (configResX.Value <= 0){configResX.Value = 1920;}
        if (configResY.Value <= 0){configResY.Value = 1080;}

        configRenderBG = Config.Bind("RenderPasses","RenderBackground",true);
        configRenderFG = Config.Bind("RenderPasses","RenderForeground",true);
        configRenderOP = Config.Bind("RenderPasses","RenderOptimised",true);

        configGroundPlaneOn = Config.Bind("ClipPlanes","GroundEnabled",true);
        configGroundPlaneHeight = Config.Bind("ClipPlanes","GroundElevation",0.01f, "This is in metres, I think.");
        configVerticalClipPlane = Config.Bind("ClipPlanes", "ClipShouldBeVertical", true);

        configReadResFromShm = Config.Bind("OAT_MMF_Data","ReadWindowResolution", false);
        configReadClipFromShm = Config.Bind("OAT_MMF_Data","ReadClipPlaneLocation", false);

        configSpoutSendBG = Config.Bind("RenderPasses","SendBackground",true);
        configSpoutSendFG = Config.Bind("RenderPasses","SendForeground",false);
        configSpoutSendOP = Config.Bind("RenderPasses","SendOptimised",true);
        configBlankSpoutSenders = Config.Bind("RenderPasses","BlankSendersOnRenderDispose",true);

        configFarClip = Config.Bind("ClipPlanes", "CameraFarClip", 1000f);

        configProtoMinorVer = Config.Bind("OAT_MMF_Data","ProtocolMinorVersion", (ushort)1);
    }

    private void SendConfigs(){
        Plugin.configResX = configResX.Value;
        Plugin.configResY = configResY.Value;

        Plugin.configRenderBG = configRenderBG.Value;
        Plugin.configRenderFG = configRenderFG.Value;
        Plugin.configRenderOP = configRenderOP.Value;

        Plugin.configGroundPlaneOn = configGroundPlaneOn.Value;
        Plugin.configGroundPlaneHeight = configGroundPlaneHeight.Value;
        Plugin.configVerticalClipPlane = configVerticalClipPlane.Value;

        Plugin.configReadResFromShm = configReadResFromShm.Value;
        Plugin.configReadClipFromShm = configReadClipFromShm.Value;

        Plugin.configSpoutSendBG = configSpoutSendBG.Value;
        Plugin.configSpoutSendFG = configSpoutSendFG.Value;
        Plugin.configSpoutSendOP = configSpoutSendOP.Value;
        Plugin.configBlankSpoutSenders = configBlankSpoutSenders.Value;

        Plugin.configFarClip = configFarClip.Value;
        
        Plugin.configProtoMinorVer = configProtoMinorVer.Value;
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Plugin.logger = new BepLogger(Logger);

        plug = new Plugin();

        BindConfigs();

        SendConfigs();

        plug.Awake();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    void Update() {
        plug.Update();
    }

    void LateUpdate(){
        plug.LateUpdate();
    }
}