using JH.EnterpriseSystem.Report.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    /// <summary>
    /// 資料組裝層介面。
    /// 職責：將 IRepository 回傳的原始 DataTable → 語意 Domain Model。
    /// 對應舊系統 ToShow1~14 裡的計算邏輯（達成率計算、欄位對應）。
    /// </summary>
    public interface IReportDataMapper
    {
        /// <summary>
        /// 原始週報資料 → WeeklyProductionData。
        /// 對應舊系統 ToShow1/5/7/9/11/14。
        /// </summary>
        WeeklyProductionData MapWeeklyProduction(
            DataTable rawData, bool hasAccumulatedRate = true);

        /// <summary>
        /// 原始日報資料 → DailyTeamProductionData。
        /// 對應舊系統 ToShow2/6/8/10/12/13。
        /// </summary>
        DailyTeamProductionData MapDailyTeamProduction(
            DataTable rawData,
            string targetDataType,
            string actualDataType);
    }
}
