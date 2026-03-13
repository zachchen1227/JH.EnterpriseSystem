using JH.EnterpriseSystem.Report.Core.Interfaces;


namespace JH.EnterpriseSystem.Report.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository _repo;

        public ReportService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> GetChartDataAsync(
            string factory, string reportCode, string chartPath, string queryDay)
        {
            var eDay = DateTime.TryParse(queryDay, out var d) ? d : DateTime.Today.AddDays(-1);
            var sDay = eDay.AddDays(-6);

            var rows = await _repo.GetCommonReportDataAsync("PRT25001",eDay,sDay);

            return new
            {
                title = $"{reportCode} － {factory}",
                categories = rows.Select(r => r["Date"].ToString()).ToList(),
                series = new object[]
                {
                    new
                    {
                        name = "派工數",
                        type = "column",
                        data = rows.Select(r => Convert.ToInt32(r["DispatchCount"])).ToList()
                    },
                    new
                    {
                        name = "完成數",
                        type = "column",
                        data = rows.Select(r => Convert.ToInt32(r["TotalCount"])).ToList()
                    },
                    new
                    {
                        name  = "達成率",
                        type  = "line",
                        yAxis = 1,
                        data  = rows.Select(r =>
                        {
                            var dispatch = Convert.ToDouble(r["DispatchCount"]);
                            var total    = Convert.ToDouble(r["TotalCount"]);
                            return dispatch == 0 ? 0 :
                                Math.Round(total / dispatch * 100, 1);
                        }).ToList()
                    }
                },
                yAxis = new object[]
                {
                    new { title = new { text = "數量" } },
                    new { title = new { text = "達成率(%)" }, opposite = true }
                }
            };
        }
    }
}
