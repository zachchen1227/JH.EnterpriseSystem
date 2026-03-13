using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.ExtraParams
{
    public class WeeklyExtraParams
    {
        public int Days { get; init; } = 6;

        /// <summary>日期欄位名稱，預設 "StockFlowDate"</summary>
        public string DateColumn { get; init; } = "StockFlowDate";

        /// <summary>
        /// DB 欄位 → Domain Series Key 的對應。
        /// Key   = Series 名稱（SeriesDefinition.DataField）
        /// Value = DB 欄位名稱
        ///
        /// 範例（成型週報）：
        ///   ["ActualQty"]       = "TotalCount"
        ///   ["TargetQty"]       = "DispatchCount"
        ///   ["AccumulatedRate"] = "Mavg"
        ///
        /// 注意：DailyRate 不在此，因為是計算欄位（由 ComputeDailyRate 控制）。
        /// </summary>
        public Dictionary<string, string> ColumnMappings { get; init; } = new()
        {
            ["ActualQty"] = "TotalCount",
            ["TargetQty"] = "DispatchCount",
            ["AccumulatedRate"] = "Mavg",
        };

        /// <summary>
        /// true  = Mapper 計算 actual/target*100 填入 Series["DailyRate"]。
        /// false = 此報表不需要達成率折線。
        /// </summary>
        public bool ComputeDailyRate { get; init; } = true;

        /// <summary>null = 使用 Builder 預設 Series 組合</summary>
        public List<SeriesDefinition>? SeriesConfig { get; init; } = null;
    }
}
