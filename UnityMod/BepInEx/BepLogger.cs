namespace OnAirTap;

public class BepLogger : AbLogger{

    private BepInEx.Logging.ManualLogSource log;

    public BepLogger(BepInEx.Logging.ManualLogSource logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        log.LogInfo(message);
    }

    public override void Warn(string message)
    {
        log.LogWarning(message);
    }

    public override void Error(string message)
    {
        log.LogError(message);
    }  
}