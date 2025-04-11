using System;

namespace TestFramework.Core.Configuration
{
    public class ApplicationConfiguration
    {
        public int Port { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Timeout { get; set; }
    }
} 