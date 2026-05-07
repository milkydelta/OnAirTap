namespace OnAirTap;

public class BSIPALogger : AbLogger{

    private IPA.Logging.Logger log;

    public BSIPALogger(IPA.Logging.Logger logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        log.Info(message);
    }

    public override void Warn(string message)
    {
        log.Warn(message);
    }

    public override void Error(string message)
    {
        log.Error(message);
    }  
}