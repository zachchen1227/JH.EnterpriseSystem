using JH.EnterpriseSystem.Report.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    public class ReportMeta
    {
        public ReportCode Code { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<FactoryCode> SupportedFactories { get; set; } = [];
        public List<ChartDefinition> Charts { get; set; } = [];
    }
}
