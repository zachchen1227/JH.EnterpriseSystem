using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.Domain
{
    /// <summary>
    /// 日報通用 Domain Model。
    ///
    /// 範例（成型日報三條）：
    ///   Series["ActualQty"] = List&lt;int&gt;    [2650, 1800, ...]
    ///   Series["TargetQty"] = List&lt;int&gt;    [2800, 2000, ...]
    ///   Series["Rates"]     = List&lt;double&gt; [94.6, 90.0, ...]
    /// </summary>
    public class DailyTeamProductionData
    {
        /// <summary>X 軸組別標籤，例如 ["AS-B03","AS-B05",...]</summary>
        public List<string> Teams { get; set; } = [];

        public Dictionary<string, object> Series { get; set; } = [];

        public void AddSeries(string key, List<int> data) => Series[key] = data;
        public void AddSeries(string key, List<double> data) => Series[key] = data;
    }
}
