using JH.EnterpriseSystem.Report.Core.Models;
using JH.EnterpriseSystem.Report.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    /// <summary>
    /// 圖表建構器介面（視覺維度）。
    /// 職責：把 Domain Model 轉成 Highcharts 可用的 { config, data } JSON。
    /// 不關心資料是週報還是日報，只管「怎麼畫圖」。
    /// </summary>
    public interface IChartBuilder
    {
        /// <summary>
        /// 將週報 Domain Model 轉成圖表 Payload。
        /// </summary>
        object BuildWeekly(
            WeeklyProductionData data,
            FactoryChartTheme theme,
            string title,
            string reportCode);

        /// <summary>
        /// 將日報 Domain Model 轉成圖表 Payload。
        /// </summary>
        object BuildDaily(
            DailyTeamProductionData data,
            FactoryChartTheme theme,
            string title,
            string reportCode);
    }
}
