using JH.EnterpriseSystem.Report.Core.Interfaces;
using System.Data;

namespace JH.EnterpriseSystem.Report.Repository.Fake
{
    public class FakeRepository : IRepository
    {
        private readonly Random _rng = new Random();

        /// <summary>
        /// 模擬 週報表 資料
        /// </summary>
        public async Task<DataTable> GetCommonReportDataAsync(string reportCode, string sDay, string eDay)
        {
            await Task.Delay(100); // 模擬連線延遲

            DataTable dt = new DataTable();
            dt.Columns.Add("StockFlowDate", typeof(DateTime)); // X軸: 日期
            dt.Columns.Add("ActualQty", typeof(int));          // 實際產量 (長條圖)
            dt.Columns.Add("TargetQty", typeof(int));          // 目標產量 (點狀圖)
            dt.Columns.Add("Rate", typeof(double));            // 當日達成率 (線圖/點圖)
            dt.Columns.Add("AccumulatedRate", typeof(double)); // 累計達成率 (虛線)

            // 解析傳入的日期區間
            if (!DateTime.TryParse(sDay, out DateTime start)) start = DateTime.Today.AddDays(-6);
            if (!DateTime.TryParse(eDay, out DateTime end)) end = DateTime.Today;

            double totalActual = 0;
            double totalTarget = 0;

            // 逐日產生假資料
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                // 模擬每日目標約 8萬 ~ 11萬
                int target = _rng.Next(80000, 110000);

                // 模擬達成率在 85% ~ 105% 之間震盪
                int actual = (int)(target * (_rng.NextDouble() * 0.2 + 0.85));

                // 累加總量 (用於計算累計達成率)
                totalActual += actual;
                totalTarget += target;

                dt.Rows.Add(
                    date,
                    actual,
                    target,
                    Math.Round((double)actual / target * 100, 1),       // 當日達成率
                    Math.Round(totalActual / totalTarget * 100, 1)      // 累計達成率
                );
            }

            return dt;
        }

        /// <summary>
        /// 模擬 日報表 資料
        /// </summary>
        public async Task<DataTable> GetDailyReportDataAsync(string reportCode, string queryDay)
        {
            await Task.Delay(100);

            DataTable dt = new DataTable();
            dt.Columns.Add("WorkTeamName", typeof(string)); // X軸: 組別
            dt.Columns.Add("ActualQty", typeof(int));       // 實際產量
            dt.Columns.Add("TargetQty", typeof(int));       // 目標產量
            dt.Columns.Add("Rate", typeof(double));         // 達成率

            // 模擬截圖中的 25 個組別
            string[] teams = {
                "AS-B03", "AS-B05", "AS-B07", "AS-B09", "AS-B11", "AS-B13", "AS-D01", "AS-D03",
                "AS-D05", "AS-D07", "AS-MP01", "AS-MP03", "AS-MP05", "AS-MP07", "AS-MP09",
                "AS-MP11", "AS-MP13", "BU1P3", "PACKING 1", "SNK AS-B01", "SNK AS-E01",
                "SNK AS-E03", "SNK AS-E05", "SNK AS-E07", "SNK AS-E11"
            };

            foreach (var team in teams)
            {
                // 模擬各組目標產量較小，約 1000 ~ 3500
                int target = _rng.Next(1000, 3500);

                // 模擬：有 20% 的機率達成率會低於 90% (觸發前端變紅色邏輯)
                double rateMultiplier = _rng.NextDouble() > 0.8 ? 0.82 : (_rng.NextDouble() * 0.15 + 0.95);
                int actual = (int)(target * rateMultiplier);

                dt.Rows.Add(
                    team,
                    actual,
                    target,
                    Math.Round((double)actual / target * 100, 1)
                );
            }

            return dt;
        }
    }
}
