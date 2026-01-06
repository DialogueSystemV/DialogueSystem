using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Rage;
using Task = System.Threading.Tasks.Task;

namespace DialogueSystem.Logging
{
    public class Logger
    {
        private readonly BlockingCollection<string> messageQueue;
        private readonly string logFilePath;
        internal static Logger logger = null;

        /// <summary>
        /// Creates a new logger instance for a specific file
        /// </summary>
        public Logger(string filePath)
        {
            this.logFilePath = Path.ChangeExtension(filePath, ".log");
            Game.LogTrivial("Creating log at " + this.logFilePath);
    
            Logger.logger = this;
            
            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Game.LogTrivial("Creating Logs Directory at " + directory);
                Directory.CreateDirectory(directory);
            }

            // Create the file if it doesn't exist
            if (File.Exists(filePath))
            {
                Game.LogTrivial("Deleting old log file at " + filePath);
                File.Delete(filePath);
            }
            
            Game.LogTrivial("Creating log file at " + filePath);
            File.Create(filePath).Close();

            
            messageQueue = new BlockingCollection<string>();
            Game.LogTrivial("Starting task to write log messages");
            // Start background writer task on threadpool
            Task.Run(() => WriteLoop());
            Log("Logger initialized.");
        }

        /// <summary>
        /// Logs a message to the file
        /// </summary>
        public void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("MM/dd/yyyy h:mm:ss.fff tt");
            string logEntry = $"[{timestamp}] {message}";
            messageQueue.Add(logEntry);
        }

        private void WriteLoop()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    writer.AutoFlush = false;

                    while (true)
                    {
                        if (messageQueue.TryTake(out string message, 100))
                        {
                            writer.WriteLine(message);
                            
                            // Drain any additional messages in the queue
                            while (messageQueue.TryTake(out string msg, 0))
                            {
                                writer.WriteLine(msg);
                            }
                            
                            writer.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"Logger WriteLoop Error: {ex.Message}");
            }
        }
    }
}
