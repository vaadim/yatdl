namespace YATDL
{
    public abstract class Logger
    {
        public delegate void LogEventHandler(Logger sender, LogEventArgs args);
        public event LogEventHandler OnRecordAdded;

        public virtual void Debug(string message)
        {
            Log(message, MessageType.Debug, null);
        }

        public virtual void Trace(string message)
        {
            Log(message, MessageType.Trace, null);
        }

        public virtual void Info(string message, params object[] args)
        {
            Log(string.Format(message, args), MessageType.Info, null);
        }

        public virtual void Warning(string message, params object[] args)
        {
            Log(string.Format(message, args), MessageType.Warning, null);
        }

        public virtual void Error(string message, System.Exception ex = null)
        {
            Log(message, MessageType.Error, ex);
        }

        public virtual void Fatal(string message, System.Exception ex = null)
        {
            Log(message, MessageType.Fatal, ex);
        }


        public virtual void Log(string message, MessageType severity, System.Exception exception)
        {
            if (OnRecordAdded != null)
                OnRecordAdded(this, new LogEventArgs(message, exception));
        }
    }

    public enum MessageType
    {
        Debug = 1,
        Trace = 2,
        Info = 3,
        Warning = 4,
        Error = 5,
        Fatal = 6
    }

    public class LogEventArgs : System.EventArgs
    {
        public LogEventArgs(string message, System.Exception exception)
        {
            LogMessage = message;
            Exception = exception;
        }

        public string LogMessage { get; set; }
        public System.Exception Exception { get; set; }
    }
}
