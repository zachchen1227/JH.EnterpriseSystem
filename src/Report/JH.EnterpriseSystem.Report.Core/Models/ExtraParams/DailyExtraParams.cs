using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.ExtraParams
{
    public class DailyExtraParams
    {

        /// <summary>目標產量的 DataType 代碼，例如 "1271"</summary>
        public string TargetDataType { get; init; } = "1271";

        /// <summary>實際產量的 DataType 代碼，例如 "1273"</summary>
        public string ActualDataType { get; init; } = "1273";

    }
}
