using OWML.Common;

namespace OnAirTap.OWML;

public class OWLogger : AbLogger{

    private IModConsole log;

    public OWLogger(IModConsole logger)
    {
        log = logger;
    }

    public override void Info(string message)
    {
        if (enabled != true){return;}
        log.WriteLine(message, MessageType.Message);
    }

    public override void Warn(string message)
    {
        if (enabled != true){return;}
        log.WriteLine(message, MessageType.Warning);
    }

    public override void Error(string message)
    {
        if (enabled != true){return;}
        log.WriteLine(message, MessageType.Error);
    }  
}