using Microsoft.AspNetCore.Mvc;

namespace JH.EnterpriseSystem.Report.Web.Controllers
{
    public class ReportController : Controller
    {
        /// <summary>
        /// GET /Report/{factory}/{reportCode}?queryDay=2026/03/13
        /// 只負責回傳頁面殼，資料由前端 JS 透過 API 取得
        /// </summary>
        public IActionResult Index(string factory, string reportCode, string? queryDay = null)
        {
            // 預設前一天（工廠通常看前一天資料）
            queryDay ??= DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd");

            ViewBag.Factory = factory;
            ViewBag.ReportCode = reportCode;
            ViewBag.QueryDay = queryDay;

            return View();
        }
    }
}
