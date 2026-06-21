using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Xml.Schema;
using System;

//using UnityEngine;
//using LIV.SDK.Unity;


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
    internal static ConfigEntry<int> configLayerMask;

    internal static ConfigEntry<string> configLayerMaskString;

    Plugin plug;

    private void BindConfigs(){
        configResX = Config.Bind("Resolution","X",1920);
        configResY = Config.Bind("Resolution","Y",1080);

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
        configLayerMask = Config.Bind("RenderPasses", "LayerMaskOverride", 0);
        configLayerMaskString = Config.Bind("RenderPasses", "LayerMaskString", "");
    }

    private void ValidateConfigs()
    {
        if (!ConfigValidation.Resolution(configResX.Value, configResY.Value))
        {
            configResX.Value = (int) configResX.DefaultValue;
            configResY.Value = (int) configResY.DefaultValue;
        }

        if (!ConfigValidation.LayerMaskString(configLayerMaskString.Value))
        {
            configLayerMaskString.Value = (string) configLayerMaskString.DefaultValue;
        }
    }

    private void SendConfigs(){
        OnAirTap.Config cfg = Plugin.cfg;
        
        cfg.ResX = configResX.Value;
        cfg.ResY = configResY.Value;

        cfg.RenderBG = configRenderBG.Value;
        cfg.RenderFG = configRenderFG.Value;
        cfg.RenderOP = configRenderOP.Value;

        cfg.GroundPlaneOn = configGroundPlaneOn.Value;
        cfg.GroundPlaneHeight = configGroundPlaneHeight.Value;
        cfg.VerticalClipPlane = configVerticalClipPlane.Value;

        cfg.ReadResFromShm = configReadResFromShm.Value;
        cfg.ReadClipFromShm = configReadClipFromShm.Value;

        cfg.SpoutSendBG = configSpoutSendBG.Value;
        cfg.SpoutSendFG = configSpoutSendFG.Value;
        cfg.SpoutSendOP = configSpoutSendOP.Value;
        cfg.BlankSpoutSenders = configBlankSpoutSenders.Value;

        cfg.FarClip = configFarClip.Value;
        
        cfg.ProtoMinorVer = configProtoMinorVer.Value;
        
        if (configLayerMaskString.Value == "")
        {
            cfg.LayerMask = 0;
        }
        else
        {
            cfg.LayerMask = Convert.ToInt32(configLayerMaskString.Value, 2);
        }
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        Plugin.logger = new BepLogger(Logger);

        plug = new Plugin();

        BindConfigs();

        ValidateConfigs();

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