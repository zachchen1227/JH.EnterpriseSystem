using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Services;
using JH.EnterpriseSystem.Report.Core.Services.ChartProviders;
using JH.EnterpriseSystem.Report.Repository.Fake;

namespace JH.EnterpriseSystem.Report.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);









            builder.Services.AddControllersWithViews()
                .AddJsonOptions(o =>
                    o.JsonSerializerOptions.PropertyNamingPolicy =
                        System.Text.Json.JsonNamingPolicy.CamelCase);

            // Repository
            builder.Services.AddScoped<IRepository, FakeRepository>();

            // Mapper
            builder.Services.AddScoped<IReportDataMapper, ReportDataMapper>();

            // ChartProviders
            builder.Services.AddScoped<IChartProvider>(sp =>
                 new WeeklyComboProvider(
                     sp.GetRequiredService<IRepository>(),
                     sp.GetRequiredService<IReportDataMapper>()));

            builder.Services.AddScoped<IChartProvider>(sp =>
                new DailyComboProvider(
                    sp.GetRequiredService<IRepository>(),
                    sp.GetRequiredService<IReportDataMapper>()));

            // ReportService
            builder.Services.AddScoped<IReportService, ReportService>();












            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();


            // 報表頁面路由：/Report/{factory}/{reportCode}
            app.MapControllerRoute(
                name: "report",
                pattern: "Report/{factory}/{reportCode}",
                defaults: new { controller = "Report", action = "Index" });

            // API 路由
            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action=Index}/{id?}");

            // 預設路由
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
