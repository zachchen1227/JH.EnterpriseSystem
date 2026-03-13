using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    public interface IRepository
    {
        // 取得時間區間報表 (對應 DNADB.GetCommonRPTT1)
        Task<DataTable> GetCommonReportDataAsync(string reportCode, string sDay, string eDay);

        // 取得單日報表 (對應 DNADB.GetCommonRPTT2)
        Task<DataTable> GetDailyReportDataAsync(string reportCode, string queryDay);
    }
}
