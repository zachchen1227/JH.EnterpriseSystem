using JH.EnterpriseSystem.Report.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JH.EnterpriseSystem.Report.Web.Controllers.Api
{
    [ApiController]
    [Route("api/chart")]
    public class ChartController : ControllerBase
    {
        private readonly IReportService _svc;
        private readonly ILogger<ChartController> _logger;

        public ChartController(IReportService svc, ILogger<ChartController> logger)
        { _svc = svc; _logger = logger; }

        [HttpGet("{factory}/{reportCode}")]
        public async Task<IActionResult> Get(
            string factory, string reportCode,
            [FromQuery] string? queryDay = null)
        {
            queryDay ??= DateTime.Today.ToString("yyyy/MM/dd");
            try
            {
                return Ok(await _svc.GetChartDataAsync(factory, reportCode, queryDay));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "參數錯誤");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "設定錯誤");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未預期錯誤");
                return StatusCode(500, new { error = "伺服器錯誤，請稍後再試" });
            }
        }
    }
}
