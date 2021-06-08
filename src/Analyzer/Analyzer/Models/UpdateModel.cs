using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Models
{
    public class UpdateModel
    {
        public string DevicePlatform { get; set; }
        public string Url { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }
    }
}
