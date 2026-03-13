using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.Domain
{
    /// <summary>
    /// 日報 Domain Model。
    /// X 軸 = 組別，呈現單日各組比較。
    /// 對應舊系統 ToShow2/6/8/10/12/13 的資料結構。
    /// </summary>
    public class DailyTeamProductionData
    {
        /// <summary>X 軸組別標籤，例如 ["AS-B03","AS-B05",...]</summary>
        public List<string> Teams { get; set; } = [];

        /// <summary>各組派工數（目標）</summary>
        public List<int> TargetQty { get; set; } = [];

        /// <summary>各組實際產量</summary>
        public List<int> ActualQty { get; set; } = [];

        /// <summary>各組達成率（%）</summary>
        public List<double> Rates { get; set; } = [];
    }
}
