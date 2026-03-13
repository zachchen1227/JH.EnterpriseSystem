using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Registry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartProviders
{
    public class ComboChartProvider : IChartProvider
    {
        private readonly IRepository _repo;
        public string ChartType => "combo";

        public ComboChartProvider(IRepository repo) => _repo = repo;

        public async Task<object> GenerateDataAsync(string factory, string reportCode, string queryDay, object extraParams)
        {
            // 1. 處理動態參數與日報判斷
            dynamic extra = extraParams;
            int days = 7; // 預設 7 天
            if (extra != null && extra.Days != null)
            {
                days = (int)extra.Days;
            }

            // 判斷是否為日報模式 (X軸為組別)
            bool isDaily = false;
            if (days == 1)
            {
                isDaily = true;
            }

            // 2. 取得廠別主題
            var fCode = Enum.Parse<FactoryCode>(factory);
            var theme = ChartThemeRegistry.Get(fCode);

            // 3. 抓取資料
            DataTable dt;
            if (isDaily)
            {
                // 日報：抓取單日組別明細
                dt = await _repo.GetDailyReportDataAsync(reportCode, queryDay);
            }
            else
            {
                // 週報：計算區間並抓取日期趨勢
                var eDay = DateTime.Today.AddDays(-1);
                if (!string.IsNullOrEmpty(queryDay))
                {
                    DateTime.TryParse(queryDay, out eDay);
                }
                var sDay = eDay.AddDays(-(days - 1));
                dt = await _repo.GetCommonReportDataAsync(reportCode, sDay.ToString("yyyy/MM/dd"), eDay.ToString("yyyy/MM/dd"));
            }

            // 4. 準備 X 軸標籤
            List<string> categories = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (isDaily)
                {
                    categories.Add(row["WorkTeamName"].ToString());
                }
                else
                {
                    categories.Add(Convert.ToDateTime(row["StockFlowDate"]).ToString("MM/dd"));
                }
            }

            // 5. 組裝 Series 資料
            var seriesList = new List<object>();

            // A. 實際產量 (長條圖 - Y軸 0)
            seriesList.Add(new
            {
                name = "實際產量",
                type = "column",
                color = theme.Colors[0],
                yAxis = 0,
                data = dt.AsEnumerable().Select(r => r["ActualQty"]).ToList()
            });

            // B. 目標產量 (橘色點狀圖 - Y軸 0)
            seriesList.Add(new
            {
                name = "目標產量",
                type = "scatter",
                marker = new { symbol = "diamond", radius = 5 },
                color = "#FFA500", // 橘色
                yAxis = 0,
                data = dt.AsEnumerable().Select(r => r["TargetQty"]).ToList()
            });

            // C. 達成率 (線圖或點圖 - Y軸 1)
            var rateData = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                double rate = Convert.ToDouble(row["Rate"]);
                string pointColor = theme.LineColor; // 預設使用主題線條顏色

                // 邏輯：達成率低於 90% 顯示紅色 (如日報截圖所示)
                if (rate < 90)
                {
                    pointColor = "#FF0000";
                }

                rateData.Add(new { y = rate, color = pointColor });
            }

            seriesList.Add(new
            {
                name = "達成率",
                type = isDaily ? "scatter" : "line",
                color = theme.LineColor,
                yAxis = 1,
                data = rateData,
                dataLabels = new { enabled = true, format = "{y}%" }
            });

            // D. 累計達成率 (僅週報顯示 - Y軸 1)
            if (!isDaily && dt.Columns.Contains("AccumulatedRate"))
            {
                seriesList.Add(new
                {
                    name = "累計達成率",
                    type = "line",
                    dashStyle = "ShortDash", // 虛線顯示
                    color = "#00CED1",
                    yAxis = 1,
                    data = dt.AsEnumerable().Select(r => r["AccumulatedRate"]).ToList()
                });
            }

            // 6. 整合回傳
            string reportTitle = reportCode;
            if (extra != null && extra.Title != null)
            {
                reportTitle = extra.Title;
            }

            return new
            {
                config = new
                {
                    type = this.ChartType,
                    title = $"{reportTitle} - {theme.FactoryName}",
                    colors = theme.Colors,
                    // 定義雙 Y 軸配置
                    yAxes = new[] {
                        new { title = new { text = "實際 / 目標產量" }, opposite = false },
                        new { title = new { text = "達成率" }, opposite = true }
                    }
                },
                data = new
                {
                    categories = categories,
                    series = seriesList
                }
            };
        }
    }
}
