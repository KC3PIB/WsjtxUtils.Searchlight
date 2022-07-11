using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WsjtxUtils.Searchlight.Common.Settings
{
    /// <summary>
    /// Logged QSO manager settings
    /// </summary>
    public class LoggedQsoManagerSettings
    {
        /// <summary>
        /// Constructs a logged QSO manager settings object
        /// </summary>
        public LoggedQsoManagerSettings() : this(string.Empty, QsoManagerBehavior.OncePerBand)
        {
        }

        /// <summary>
        /// Constructs a logged QSO manager settings object
        /// </summary>
        /// <param name="logFilePath"></param>
        /// <param name="qsoManagerBehavior"></param>
        public LoggedQsoManagerSettings(string logFilePath, QsoManagerBehavior qsoManagerBehavior)
        {
            LogFilePath = logFilePath;
            QsoManagerBehavior = qsoManagerBehavior;
        }

        /// <summary>
        /// Path to the logged QSO file
        /// </summary>
        public string LogFilePath { get; set; }

        /// <summary>
        /// The behavior of the QSO manager
        /// </summary>
        public QsoManagerBehavior QsoManagerBehavior { get; set; }
    }
}
