using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace YATDL
{
    public class YATDLLogger : Logger
    {
        private static readonly Dictionary<MessageType, object> _logLocks = new Dictionary<MessageType, object>();
        private static readonly Dictionary<MessageType, string> _messageLogDirNames = new Dictionary<MessageType, string>();

        private static readonly object _operationLogLock = new object();
        private static readonly string _operationLogDirectoryName;
        private static readonly string _logDirectoryName;

        static YATDLLogger()
        {
            if (HttpContext.Current != null)
                _logDirectoryName = HttpContext.Current.Server.MapPath("~/App_Data/Logs");
            else
                _logDirectoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "App_Data", "Logs");

            // init log locks
            foreach (var messageTypes in Enum.GetValues(typeof(MessageType)))
            {
                var severity = (MessageType)messageTypes;
                _logLocks.Add(severity, new object());
                _messageLogDirNames.Add(severity, Path.Combine(_logDirectoryName, severity.ToString()));

                lock (_logLocks[severity])
                {
                    string severityLogDir = Path.Combine(_logDirectoryName, severity.ToString());
                    if (!Directory.Exists(severityLogDir))
                        Directory.CreateDirectory(severityLogDir);
                }
            }

            lock (_operationLogLock)
            {
                _operationLogDirectoryName = Path.Combine(_logDirectoryName, "ExternalOperations");
                if (!Directory.Exists(_operationLogDirectoryName))
                    Directory.CreateDirectory(_operationLogDirectoryName);
            }
        }

        public override void Log(string message, MessageType severity, Exception exception)
        {
            string fileName = Path.Combine(_messageLogDirNames[severity], DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            try
            {
                lock (_logLocks[severity])
                {
                    using (var sWriter = new StreamWriter(fileName, true))
                    {
                        switch (severity)
                        {
                            case MessageType.Error:
                                var sBuilder = new StringBuilder();

                                sBuilder.AppendFormat("\n\n============== Ошибка. Дата: {0} ===================\n{1}\n", DateTime.Now, message);

                                // Записываем детали запроса
                                sBuilder.AppendLine("\nДетали запроса:\nParams:");
                                var request = HttpContext.Current.Request;
                                for (int i = 0; i < request.Params.Count; i++)
                                    sBuilder.AppendFormat("{0})  {1}: {2}\n", i + 1, request.Params.GetKey(i), request.Params[i]);

                                // Записываем детали исключения
                                if (exception != null)
                                {
                                    sBuilder.Append("\nДетали исключения:");
                                    var innerException = exception;
                                    do
                                    {
                                        sBuilder.AppendFormat("\nMessage: {0}\nStackTrace:\n{1}{2}",
                                            innerException.Message,
                                            innerException.StackTrace,
                                            (innerException.InnerException != null) ? "\n|\nV" : "");       // Рисуем стрелку вниз

                                        innerException = innerException.InnerException;
                                    }
                                    while (innerException != null);
                                }

                                sBuilder.Append("\n==============================================================================");
                                sWriter.WriteLine(sBuilder.ToString());
                                break;
                            default:
                                sWriter.WriteLine("\n{0}  {1}: {2}", DateTime.Now, severity, message);
                                break;
                        }
                    }
                }

                base.Log(message, severity, exception);
            }
            catch { }
        }


        public void Operation(string operationName, string inputParams, string responseParams)
        {
            string fileName = Path.Combine(_operationLogDirectoryName, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            try
            {
                lock (_operationLogLock)
                {
                    using (var sWriter = new StreamWriter(fileName, true))
                    {
                        sWriter.WriteLine("\n\nDate: {0}", DateTime.Now);
                        sWriter.WriteLine("Operation: {0}", operationName);
                        sWriter.WriteLine("Input params: {0}", inputParams);
                        sWriter.WriteLine("Response params: {0}", responseParams);
                    }
                }
            }
            catch { }
        }
    }
}