using JH.EnterpriseSystem.Report.Core.Models;
using JH.EnterpriseSystem.Report.Core.Models.Domain;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    public interface IChartBuilder
    {
        object BuildWeekly(WeeklyProductionData data, FactoryChartTheme theme,
             string title, string reportCode, List<SeriesDefinition>? seriesConfig = null);
        object BuildDaily(DailyTeamProductionData data, FactoryChartTheme theme,
            string title, string reportCode, List<SeriesDefinition>? seriesConfig = null);
    }
}
