using JH.EnterpriseSystem.Report.Core.Models.Domain;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{

    public interface IReportDataMapper
    {
        WeeklyProductionData MapWeeklyProduction(DataTable raw, WeeklyExtraParams extra);
        DailyTeamProductionData MapDailyTeamProduction(DataTable raw, DailyExtraParams extra);
    }
}
