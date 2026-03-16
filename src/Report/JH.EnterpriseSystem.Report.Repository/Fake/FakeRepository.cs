using JH.EnterpriseSystem.Report.Core.Interfaces;
using System.Data;

namespace JH.EnterpriseSystem.Report.Repository.Fake
{
    public class FakeRepository : IRepository
    {
        private readonly Random _rng = new();

        public async Task<DataTable> GetWeeklyRawDataAsync(
            string factory, string reportCode, string sDay, string eDay)
        {
            await Task.Delay(80);
            var dt = new DataTable();
            dt.Columns.Add("StockFlowDate", typeof(DateTime));
            dt.Columns.Add("DispatchCount", typeof(int));
            dt.Columns.Add("TotalCount", typeof(int));
            dt.Columns.Add("Mavg", typeof(double));

            if (!DateTime.TryParse(sDay, out var start)) start = DateTime.Today.AddDays(-6);
            if (!DateTime.TryParse(eDay, out var end)) end = DateTime.Today;

            double totalActual = 0, totalTarget = 0;
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                int dispatch = _rng.Next(80_000, 110_000);
                int total = (int)(dispatch * (_rng.NextDouble() * 0.20 + 0.85));
                totalActual += total; totalTarget += dispatch;
                dt.Rows.Add(date, dispatch, total,
                    Math.Round(totalActual / totalTarget * 100, 1));
            }
            
            return dt;
        }

        public async Task<DataTable> GetDailyRawDataAsync(
            string factory, string reportCode, string queryDay)
        {
            await Task.Delay(80);
            var dt = new DataTable();
            dt.Columns.Add("WorkTeamName", typeof(string));
            dt.Columns.Add("DataType", typeof(string));
            dt.Columns.Add("TotalCount", typeof(int));

            string[] teams =
            [
                "AS-B03","AS-B05","AS-B07","AS-B09","AS-B11","AS-B13",
                "AS-D01","AS-D03","AS-D05","AS-D07",
                "AS-MP01","AS-MP03","AS-MP05","AS-MP07","AS-MP09",
                "BU1P3","PACKING 1",
                "SNK AS-B01","SNK AS-E01","SNK AS-E03","SNK AS-E05"
            ];

            var (tType, aType) = reportCode switch
            {
                "RPT25002" => ("1271", "1273"),
                "RPT25006" => ("1292", "1209A"),
                "RPT25008" => ("1261", "1209A"),
                "RPT25010" => ("1281", "1209A"),
                "RPT25012" => ("1251", "1209A"),
                _ => ("1271", "1273")
            };

            foreach (var team in teams)
            {
                int dispatch = _rng.Next(1_000, 3_500);
                int actual = (int)(dispatch * (_rng.NextDouble() * 0.25 + 0.80));
                dt.Rows.Add(team, tType, dispatch);
                dt.Rows.Add(team, aType, actual);
            }
            return dt;
        }

      
        public async Task<List<DateTime>> GetHolidaysAsync(string sDay, string eDay)
        {
            await Task.Delay(10);
            return [];
        }
    }
}
