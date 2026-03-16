using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models.Domain;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Services
{
    public class ReportDataMapper : IReportDataMapper
    {
        // ════════════════════════════════════════════════════════
        //  週報
        // ════════════════════════════════════════════════════════
        public WeeklyProductionData MapWeeklyProduction(
            DataTable raw, WeeklyExtraParams extra)
        {
            var result = new WeeklyProductionData();

            // 依 ColumnMappings 的型別預建 List
            var intLists = new Dictionary<string, List<int>>();
            var doubleLists = new Dictionary<string, List<double>>();

            foreach (var (seriesKey, dbCol) in extra.ColumnMappings)
            {
                if (!raw.Columns.Contains(dbCol)) continue;

                var type = raw.Columns[dbCol]!.DataType;
                if (type == typeof(int) || type == typeof(long))
                    intLists[seriesKey] = [];
                else
                    doubleLists[seriesKey] = [];
            }

            // DailyRate 計算用暫存
            var dailyRates = extra.ComputeDailyRate ? new List<double>() : null;

            foreach (DataRow row in raw.Rows)
            {
                // X 軸日期
                var dateStr = row[extra.DateColumn]?.ToString() ?? "";
                result.Dates.Add(
                    DateTime.TryParse(dateStr, out var dt)
                        ? dt.ToString("yyyy/MM/dd") : dateStr);

                // 依 ColumnMappings 填入各 Series
                foreach (var (seriesKey, dbCol) in extra.ColumnMappings)
                {
                    if (!raw.Columns.Contains(dbCol)) continue;

                    if (intLists.TryGetValue(seriesKey, out var il))
                        il.Add(ParseInt(row, dbCol));
                    else if (doubleLists.TryGetValue(seriesKey, out var dl))
                        dl.Add(ParseDouble(row, dbCol));
                }

                // 計算 DailyRate
                if (dailyRates is not null)
                {
                    int target = intLists.TryGetValue("TargetQty", out var tl)
                                 && tl.Count > 0 ? tl[^1] : 0;
                    int actual = intLists.TryGetValue("ActualQty", out var al)
                                 && al.Count > 0 ? al[^1] : 0;
                    dailyRates.Add(
                        target > 0 && actual > 0
                            ? Math.Round(actual * 100.0 / target, 1,
                                         MidpointRounding.AwayFromZero)
                            : 0);
                }
            }

            // 填入 result.Series
            foreach (var (key, list) in intLists) result.AddSeries(key, list);
            foreach (var (key, list) in doubleLists) result.AddSeries(key, list);
            if (dailyRates?.Count > 0) result.AddSeries("DailyRate", dailyRates);

            return result;
        }

        // ════════════════════════════════════════════════════════
        //  日報
        // ═════════���══════════════════════════════════════════════
        public DailyTeamProductionData MapDailyTeamProduction(
            DataTable raw, DailyExtraParams extra)
        {
            var result = new DailyTeamProductionData();
            var targets = new List<int>();
            var actuals = new List<int>();
            var rates = new List<double>();

            var teams = raw.AsEnumerable()
                           .Select(r => r["WorkTeamName"]?.ToString() ?? "")
                           .Distinct().ToList();

            foreach (var team in teams)
            {
                result.Teams.Add(team);

                var tRows = raw.Select(
                    $"WorkTeamName='{Escape(team)}' AND DataType='{extra.TargetDataType}'");
                int target = tRows.Length > 0 ? ParseInt(tRows[0], "TotalCount") : 0;
                targets.Add(target);

                var aRows = raw.Select(
                    $"WorkTeamName='{Escape(team)}' AND DataType='{extra.ActualDataType}'");
                int actual = aRows.Length > 0 ? ParseInt(aRows[0], "TotalCount") : 0;
                actuals.Add(actual);

                rates.Add(target > 0
                    ? Math.Round(actual * 100.0 / target, 1,
                                 MidpointRounding.AwayFromZero)
                    : 0);
            }

            result.AddSeries("TargetQty", targets);
            result.AddSeries("ActualQty", actuals);
            result.AddSeries("Rates", rates);
            return result;
        }       

        private static int ParseInt(DataRow row, string col) =>
            row.Table.Columns.Contains(col) && !row.IsNull(col)
                ? (int)Convert.ToDouble(row[col]) : 0;
        private static double ParseDouble(DataRow row, string col) =>
            row.Table.Columns.Contains(col) && !row.IsNull(col)
                ? Convert.ToDouble(row[col]) : 0;
        private static string Escape(string v) => v.Replace("'", "''");
    }
}
