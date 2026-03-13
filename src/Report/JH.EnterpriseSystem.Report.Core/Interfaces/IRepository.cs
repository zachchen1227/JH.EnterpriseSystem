using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    /// <summary>
    /// 資料存取介面。
    /// 只負責「撈原始資料」，廠區過濾（FID）在此做。
    /// 不做任何計算或格式化。
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 取得週報時間區間原始資料。
        /// 欄位：StockFlowDate / DispatchCount / TotalCount / Mavg
        /// </summary>
        Task<DataTable> GetWeeklyRawDataAsync(
            string factory, string reportCode, string sDay, string eDay);

        /// <summary>
        /// 取得日報單日組別明細原始資料。
        /// 欄位：WorkTeamName / DataType / TotalCount
        /// </summary>
        Task<DataTable> GetDailyRawDataAsync(
            string factory, string reportCode, string queryDay);

        /// <summary>
        /// 取得假日清單（週報計算日期範圍用）。
        /// </summary>
        Task<List<DateTime>> GetHolidaysAsync(string sDay, string eDay);
    }
}
