using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// methods used for logging
    /// </summary>
    public static class Logger {
        public static Action<object, string, string> InfoLogger { get; set; }
        public static Action<object, string, string> WarningLogger { get; set; }
        public static Action<object, string, Exception> ErrorLogger { get; set; }

        /// <summary>
        /// logs an info
        /// </summary>
        /// <param name="sender">sender of the message</param>
        /// <param name="message">message to log</param>
        /// <param name="detail">detail for further information</param>
        public static void Info(object sender, string message, string detail=null) {
            InfoLogger?.Invoke(sender, message, detail);
        }

        /// <summary>
        /// logs a warning
        /// </summary>
        /// <param name="sender">sender of the message</param>
        /// <param name="message">message to log</param>
        /// <param name="detail">detail for further information</param>
        public static void Warning(object sender, string message, string detail = null) {
            WarningLogger?.Invoke(sender, message, detail);
        }

        /// <summary>
        /// logs an error
        /// </summary>
        /// <param name="sender">sender of the message</param>
        /// <param name="message">message to log</param>
        /// <param name="error">detail for further information</param>
        public static void Error(object sender, string message, Exception error = null) {
            ErrorLogger?.Invoke(sender, message, error);
        }
    }
}