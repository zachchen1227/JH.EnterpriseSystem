using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    public class ChartDefinition
    {
        // 預設圖表類型為 "combo"
        public string ChartType { get; set; } = "combo";
        
        // 預設標題
        public string Title { get; set; } = "New Chart";

        //動態參數
        public object? ExtraParams { get; set; } = null;
    }

}
