using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    public interface IChartProvider
    {
        string ChartType { get; }
        Task<object> GenerateDataAsync(
            string factory,
            string reportCode,
            string title,
            string queryDay,
            object? extraParams = null);
    }
}
