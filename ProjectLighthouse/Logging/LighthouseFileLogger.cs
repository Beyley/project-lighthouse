using System;
using System.IO;
using Kettu;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Helpers.Extensions;

namespace LBPUnion.ProjectLighthouse.Logging {
    public class LighthouseFileLogger : LoggerBase {
        private static readonly string logsDirectory = Path.Combine(Environment.CurrentDirectory, "logs");
        
        public override void Send(LoggerLine line)
        {
            FileHelper.EnsureDirectoryCreated(logsDirectory);
            
            File.AppendAllText(Path.Combine(logsDirectory, line.LoggerLevel.Name.ToFileName() + ".log"), line.LineData + "\n");
            File.AppendAllText(Path.Combine(logsDirectory, "all.log"), line.LineData + "\n");
        }
    }
}