using NLog;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace UKSync
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string ServiceName;
        private static EventLog EventLog;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Properties.Settings.Default.Name = ConfigurationManager.AppSettings["Name"];
            Properties.Settings.Default.Description = ConfigurationManager.AppSettings["Description"];
            Properties.Settings.Default.Url = ConfigurationManager.AppSettings["Url"];
            Properties.Settings.Default.Token = ConfigurationManager.AppSettings["Token"];

            ServiceName = "CanvasSync";
            EventLog = new System.Diagnostics.EventLog();
            EventLog.Source = ServiceName;
            EventLog.Log = "Application";

            ((ISupportInitialize)EventLog).BeginInit();
            if (!EventLog.SourceExists(EventLog.Source))
            {
                EventLog.CreateEventSource(EventLog.Source, EventLog.Log);
            }
            ((ISupportInitialize)EventLog).EndInit();

            EventLog.WriteEntry("Program Started.", EventLogEntryType.Information);

            try
            {

                logger.Info("Program Started");

                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

                SyncService s = new SyncService();
#if (!DEBUG)
                ServiceBase.Run(s);
#else
                s.runTasks = true;
                s.RunMainThread();
#endif
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Handler para excepciones no controladas
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">UnhandledExceptionEventArgs</param>
        public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry((e.ExceptionObject as Exception).Message, EventLogEntryType.Error);
        }
    }
}
