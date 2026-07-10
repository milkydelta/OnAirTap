namespace OnAirTap.BepInEx5;

public class BepLogger : AbLogger{

    private BepInEx.Logging.ManualLogSource log;

    public BepLogger(BepInEx.Logging.ManualLogSource logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        if (enabled != true){return;}
        log.LogInfo(message);
    }

    public override void Warn(string message)
    {
        if (enabled != true){return;}
        log.LogWarning(message);
    }

    public override void Error(string message)
    {
        if (enabled != true){return;}
        log.LogError(message);
    }  
}