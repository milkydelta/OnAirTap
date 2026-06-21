using IPA;
using IPA.Loader;
using IpaLogger = IPA.Logging.Logger;
using IPA.Config;
using IPA.Config.Stores;

using UnityEngine;

namespace OnAirTap;

[Plugin(RuntimeOptions.SingleStartInit)]
internal class BSIPAPlugin
{
    internal static IpaLogger Log { get; private set; } = null!;

    internal static IPA.Config.Config cfg { get; private set; }

    [Init]
    public BSIPAPlugin(IpaLogger ipaLogger,IPA.Config.Config config, PluginMetadata pluginMetadata)
    {
        Log = ipaLogger;
        cfg = config;
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
    }
        
    [OnStart]
    public void OnApplicationStart()
    {
        Log.Debug("OnApplicationStart");
        new GameObject("OatPluginHoldingObject").AddComponent<BSIPA_OatComponent>();
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        Log.Debug("OnApplicationQuit");
    }
}