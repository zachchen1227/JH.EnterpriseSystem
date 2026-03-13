using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    public interface IReportService
    {
        Task<object> GetChartDataAsync(
            string factory, string reportCode, string queryDay);
    }
}
