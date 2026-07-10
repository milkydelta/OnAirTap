namespace OnAirTap.BSIPA;

public class BSIPALogger : AbLogger{

    private IPA.Logging.Logger log;

    public BSIPALogger(IPA.Logging.Logger logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        if (enabled != true){return;}
        log.Info(message);
    }

    public override void Warn(string message)
    {
        if (enabled != true){return;}
        log.Warn(message);
    }

    public override void Error(string message)
    {
        if (enabled != true){return;}
        log.Error(message);
    }  
}