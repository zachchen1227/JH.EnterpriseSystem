using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Services;
using JH.EnterpriseSystem.Report.Core.Services.ChartBuilders;
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

            // ChartBuilder
            builder.Services.AddScoped<ComboChartBuilder>();

            // ChartProviders
            builder.Services.AddScoped<IChartProvider>(sp =>
                new WeeklyComboProvider(
                    sp.GetRequiredService<IRepository>(),
                    sp.GetRequiredService<IReportDataMapper>(),
                    sp.GetRequiredService<ComboChartBuilder>()));

            builder.Services.AddScoped<IChartProvider>(sp =>
                new DailyComboProvider(
                    sp.GetRequiredService<IRepository>(),
                    sp.GetRequiredService<IReportDataMapper>(),
                    sp.GetRequiredService<ComboChartBuilder>()));

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

            app.MapControllerRoute("api", "api/{controller}/{action=Index}/{id?}");
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
