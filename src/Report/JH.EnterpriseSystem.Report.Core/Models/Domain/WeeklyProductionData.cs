using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.Domain
{
    /// <summary>
    /// 週報通用 Domain Model。
    ///
    /// Series 改為字典，有幾條就放幾條，Builder 找不到自動跳過。
    ///
    /// 範例（成型週報四條）：
    ///   Series["ActualQty"]       = List&lt;int&gt;    [91000, 89000, ...]
    ///   Series["TargetQty"]       = List&lt;int&gt;    [95000, 92000, ...]
    ///   Series["DailyRate"]       = List&lt;double&gt; [95.8,  96.7,  ...]
    ///   Series["AccumulatedRate"] = List&lt;double&gt; [95.1,  95.3,  ...]
    ///
    /// 範例（只有兩條折線的週報）：
    ///   Series["LineA"] = List&lt;double&gt; [...]
    ///   Series["LineB"] = List&lt;double&gt; [...]
    /// </summary>
    public class WeeklyProductionData
    {
        /// <summary>X 軸日期標籤，例如 ["03/06","03/07",...]</summary>
        public List<string> Dates { get; set; } = [];

        /// <summary>
        /// 各 Series 資料。
        /// Key   = SeriesDefinition.DataField
        /// Value = List&lt;int&gt; 或 List&lt;double&gt;
        /// </summary>
        public Dictionary<string, object> Series { get; set; } = [];

        public void AddSeries(string key, List<int> data) => Series[key] = data;
        public void AddSeries(string key, List<double> data) => Series[key] = data;
    }
}
