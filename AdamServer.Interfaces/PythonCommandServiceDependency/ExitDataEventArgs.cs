using System;

namespace AdamServer.Interfaces.PythonCommandServiceDependency
{
    public class ExitDataEventArgs : EventArgs
    {

        public int ExitCode { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime ExitTime { get; set; }
        
        public TimeSpan TotalProcessorTime { get; set; }
        
        public TimeSpan UserProcessorTime { get; set; }
        
        public string MachineName { get; set; }
    }
}
