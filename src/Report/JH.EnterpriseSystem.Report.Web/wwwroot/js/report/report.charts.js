/**
 * report.charts.js
 * 統一的圖表繪製控制器
 */

class ReportChartManager {
    constructor(containerId, department) {
        this.containerId = containerId;
        this.department = department;
        this.theme = ReportConfig.getTheme(department); // 載入該部門的主題色
    }

    /**
     * 繪製週產量報表 (對應舊系統的 DrawBu_01, DrawJt_01)
     * @param {Object} data - 資料物件 { dates: [], targets: [], actuals: [] }
     * @param {string} title - 圖表標題
     */
    renderWeeklyProduction(data, title) {
        const theme = this.theme;

        // 合併共用設定與特定圖表設定
        const options = Highcharts.merge(ReportConfig.commonChartOptions, {
            chart: {
                renderTo: this.containerId,
                zoomType: 'xy',
                backgroundColor: 'transparent' // 保持背景乾淨
            },
            title: {
                text: title || `${this.department} 成型產量週報表`
            },
            xAxis: [{
                categories: data.categories,
                crosshair: true
            }],
            yAxis: [
                { // Primary yAxis (目標/實際數值)
                    labels: {
                        style: { color: theme.primary }
                    },
                    title: {
                        text: '目標/實際數值',
                        style: { color: theme.primary }
                    }
                },
                { // Secondary yAxis (達成率 - 若有的話)
                    title: {
                        text: '達成率',
                        style: { color: theme.actual }
                    },
                    labels: {
                        format: '{value}%',
                        style: { color: theme.actual }
                    },
                    opposite: true
                }
            ],
            tooltip: {
                shared: true
            },
            legend: {
                layout: 'vertical',
                align: 'left',
                x: 120,
                verticalAlign: 'top',
                y: 100,
                floating: true,
                backgroundColor: 'rgba(255,255,255,0.25)'
            },
            series: [
                {
                    name: '目標產量',
                    type: 'column',
                    yAxis: 0,
                    data: data.targets,
                    color: theme.target, // 使用配置檔中的顏色
                    tooltip: { valueSuffix: ' 双' }
                },
                {
                    name: '實際產量',
                    type: 'spline', // 曲線圖
                    yAxis: 0,
                    data: data.actuals,
                    color: theme.actual, // 使用配置檔中的顏色
                    lineWidth: 3, // 來自 jhOptions 的設定
                    marker: {
                        lineWidth: 2,
                        lineColor: theme.actual,
                        fillColor: 'white'
                    },
                    tooltip: { valueSuffix: ' 双' }
                }
            ]
        });

        // 建立圖表
        this.chartInstance = new Highcharts.Chart(options);
    }

    /**
     * 範例：繪製 RFT 報表 (對應舊系統的 DrawBu_03, DrawJt_03)
     * 透過參數化，同一段程式碼可繪製不同部門的圖
     */
    renderRFT(data, title) {
        const theme = this.theme;

        const options = Highcharts.merge(ReportConfig.commonChartOptions, {
            chart: { renderTo: this.containerId, type: 'line' },
            title: { text: title },
            xAxis: { categories: data.categories },
            yAxis: { title: { text: 'RFT (%)' }, max: 100 },
            series: [{
                name: 'RFT',
                data: data.values,
                color: theme.primary // 自動套用該部門主色
            }]
        });

        this.chartInstance = new Highcharts.Chart(options);
    }
}