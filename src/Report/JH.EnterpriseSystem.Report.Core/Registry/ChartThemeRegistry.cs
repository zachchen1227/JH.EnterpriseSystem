using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Registry
{
    public static class ChartThemeRegistry
    {
        private static FactoryChartTheme Default(string name) => new()
        {
            FactoryName = name,
            Colors = ["#4572A7", "#AA4643", "#89A54E", "#71588F"],
            LineColor = "#FF0000",
            TargetColor = "#FFA500"
        };

        public static readonly Dictionary<FactoryCode, FactoryChartTheme> All
            = new()
            {
            { FactoryCode.BU1, Default("BU1") },
            { FactoryCode.BU2, Default("BU2") },
            { FactoryCode.JT1, Default("JT1") },
            { FactoryCode.JT2, Default("JT2") }
        };

        public static FactoryChartTheme Get(FactoryCode factory) => All[factory];
    }
}
