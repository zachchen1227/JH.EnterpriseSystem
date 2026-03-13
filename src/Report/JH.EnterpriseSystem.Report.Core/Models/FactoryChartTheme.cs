using JH.EnterpriseSystem.Report.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    public class FactoryChartTheme
    {
        public string FactoryName { get; set; }= string.Empty;
        public List<string> Colors { get; set; } = [];
        public string LineColor { get; set; } = string.Empty;
        public string TargetColor { get; set; } = string.Empty;
    }
}
