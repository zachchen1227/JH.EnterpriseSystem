using JH.EnterpriseSystem.Report.Core.Interfaces;
using System.Data;

namespace JH.EnterpriseSystem.Report.Repository.Dna
{
    /// <summary>
    /// 真實 DB 的 Repository 實作。
    /// 透過 DnaDb（舊系統 DNADB.cs）撈取資料。
    ///
    /// IRepository 方法 → DnaDb 方法 對照：
    ///   GetWeeklyRawDataAsync  → DnaDb.GetCommonRPTT1
    ///   GetDailyRawDataAsync   → DnaDb.GetCommonRPTT2
    ///   GetHolidaysAsync       → DnaDb.GetHoliday
    /// </summary>
    public class DnaRepository : IRepository
    {
        private readonly DNADB _db;

        public DnaRepository(DNADB db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════
        //  週報：傳入廠區、報表代碼、日期區間
        //  → 對應舊系統 GetCommonRPTT1(reportName, sDay, eDay)
        //
        //  reportCode（例如 "RPT25001"）就是 DB View 名稱
        //  factory 目前舊系統用 FID 欄位過濾，這裡先回傳全部資料
        //  讓 Mapper 自行依 FID 過濾（與舊系統 ToShow 行為一致）
        // ════════════════════════════════════════════════════════
        public Task<DataTable> GetWeeklyRawDataAsync(
            string factory, string reportCode, string sDay, string eDay)
        {
            var dt = _db.GetCommonRPTT1(reportCode, sDay, eDay);
            return Task.FromResult(dt);
        }

        // ════════════════════════════════════════════════════════
        //  日報：傳入廠區、報表代碼、查詢日期
        //  → 對應舊系統 GetCommonRPTT2(reportName, queryDay)
        // ════════════════════════════════════════════════════════
        public Task<DataTable> GetDailyRawDataAsync(
            string factory, string reportCode, string queryDay)
        {
            var dt = _db.GetCommonRPTT2(reportCode, queryDay);
            return Task.FromResult(dt);
        }

        // ════════════════════════════════════════════════════════
        //  假日清單：用於 Provider 計算「往前扣掉假日的週期」
        //  → 對應舊系統 GetHoliday(sDay, eDay)
        // ════════════════════════════════════════════════════════
        public Task<List<DateTime>> GetHolidaysAsync(string sDay, string eDay)
        {
            var dt = _db.GetHoliday(sDay, eDay);

            var holidays = dt.Rows
                .Cast<DataRow>()
                .Select(r => DateTime.Parse(r["Holiday"].ToString()!))
                .ToList();

            return Task.FromResult(holidays);
        }
    }
}
