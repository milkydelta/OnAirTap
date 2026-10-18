namespace OnAirTap.Melon;

public class MelonLogger : AbLogger{

    private MelonLoader.MelonLogger.Instance log;

    public MelonLogger(MelonLoader.MelonLogger.Instance logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        if (enabled != true){return;}
        log.Msg(message);
    }

    public override void Warn(string message)
    {
        if (enabled != true){return;}
        log.Warning(message);
    }

    public override void Error(string message)
    {
        if (enabled != true){return;}
        log.Error(message);
    }  
}
