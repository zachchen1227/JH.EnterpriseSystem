/**
 * combo-chart.js
 * 負責：
 *   1. 從後端 API 取得 { config, data }
 *   2. 依 config 設定 Highcharts 選項
 *   3. 渲染圖表
 *
 * 設計原則：
 *   - 所有顏色、業務規則（閾值、Y 軸範圍）全部從 API config 讀取，不寫死
 *   - series[].type 由後端 config.data.series[].type 決定，前端不判斷
 */

(function () {
    'use strict';

    // ── 讀取後端注入的參數 ────────────────────────────────
    const { factory, reportCode, queryDay } = window.__REPORT__;

    // ── DOM 元素 ──────────────────────────────────────────
    const elTitle = document.getElementById('chartTitle');
    const elLoading = document.getElementById('chartLoading');
    const elError = document.getElementById('chartError');
    const elErrorMsg = document.getElementById('chartErrorMsg');
    const elContainer = document.getElementById('chartContainer');
    const elInput = document.getElementById('queryDayInput');
    const btnQuery = document.getElementById('btnQuery');
    const btnRetry = document.getElementById('btnRetry');

    let currentChart = null;

    // ════════════════════════════════════════════════════════
    //  主流程
    // ════════════════════════════════════════════════════════
    async function loadChart(day) {
        showLoading();

        try {
            const url = `/api/chart/${factory}/${reportCode}?queryDay=${day}`;
            const res = await fetch(url);

            if (!res.ok) {
                const body = await res.json().catch(() => ({}));
                throw new Error(body.error ?? `HTTP ${res.status}`);
            }

            const payload = await res.json();
            renderChart(payload);

        } catch (err) {
            showError(err.message);
        }
    }

    // ════════════════════════════════════════════════════════
    //  Highcharts 渲染
    // ════════════════════════════════════════════════════════
    function renderChart(payload) {
        const cfg = payload.config;
        const data = payload.data;

        // 更新標題
        elTitle.textContent = cfg.title ?? reportCode;

        // 銷毀舊圖表
        if (currentChart) {
            currentChart.destroy();
            currentChart = null;
        }

        // ── Y 軸設定 ──────────────────────────────────────
        // 後端傳入兩個 yAxes：[0]=左軸(產量)，[1]=右軸(達成率)
        const yAxes = buildYAxes(cfg.yAxes, data);

        // ── Series 設定 ───────────────────────────────────
        // type / color / yAxis / data / dashStyle 全由後端決定
        const series = buildSeries(cfg, data);

        // ── Highcharts 全域顏色陣列 ───────────────────────
        // 讓未明確指定 color 的系列自動套用廠區顏色順序
        Highcharts.setOptions({ colors: cfg.colors ?? [] });

        // ── 建立圖表 ──────────────────────────────────────
        currentChart = Highcharts.chart(elContainer, {
            chart: {
                animation: true,
                style: { fontFamily: 'system-ui, sans-serif' }
            },

            title: { text: null },   // 標題顯示在 DOM，不用 Highcharts 自帶
            subtitle: { text: null },

            xAxis: {
                categories: data.categories,
                crosshair: true,
                tickmarkPlacement: 'on',
                labels: { style: { fontSize: '12px' } }
            },

            yAxis: yAxes,

            tooltip: {
                shared: true,
                formatter: tooltipFormatter(cfg)
            },

            legend: {
                enabled: true,
                align: 'center',
                layout: 'horizontal'
            },

            plotOptions: {
                column: { grouping: true, borderWidth: 0 },
                scatter: {
                    marker: {
                        symbol: 'diamond',
                        radius: 6,
                        lineWidth: 1,
                        lineColor: '#333'
                    }
                },
                line: { lineWidth: 2 }
            },

            series,

            credits: { enabled: false },
            responsive: {
                rules: [{
                    condition: { maxWidth: 600 },
                    chartOptions: { legend: { layout: 'vertical' } }
                }]
            }
        });

        showChart();
    }

    // ════════════════════════════════════════════════════════
    //  Y 軸建構
    // ════════════════════════════════════════════════════════
    function buildYAxes(yAxesConfig, data) {
        if (!yAxesConfig || yAxesConfig.length === 0) {
            return [{ title: { text: '數量' } }];
        }

        return yAxesConfig.map((ax, i) => {
            const base = {
                title: ax.title ?? { text: '' },
                opposite: ax.opposite ?? false,
                labels: { style: { fontSize: '12px' } },
                gridLineWidth: i === 0 ? 1 : 0,
            };

            if (i === 0) {
                // 左軸（產量）：自動計算 max 或使用強制上限
                const allQty = [
                    ...flatNumbers(data.series, 'ActualQty'),
                    ...flatNumbers(data.series, 'TargetQty'),
                ];
                const dataMax = allQty.length > 0 ? Math.max(...allQty) : 0;

                base.min = 0;
                base.max = ax.maxOverride != null
                    ? ax.maxOverride                          // 強制上限（RPT25013 等）
                    : Math.ceil(dataMax * (ax.magnification ?? 1.5));
            } else {
                // 右軸（達成率）：固定範圍，tickAmount 依 gap 計算
                const ceiling = ax.ceiling ?? 120;
                const floor = ax.floor ?? 0;
                const gap = ax.gap ?? 15;

                base.min = floor;
                base.max = ceiling;
                // 對應舊系統 get tickAmount() { return Math.round(...) + 1; }
                base.tickAmount = Math.round((ceiling - floor) / gap) + 1;
                base.plotLines = [{
                    value: 100,
                    color: '#999',
                    dashStyle: 'ShortDash',
                    width: 1,
                    label: { text: '100%', align: 'right', style: { color: '#999' } }
                }];
            }

            return base;
        });
    }

    // ════════════════════════════════════════════════════════
    //  Series 建構
    //  後端已決定每條線的 type / color / yAxis / dashStyle
    //  前端只負責搬運，不做任何判斷
    // ════════════════════════════════════════════════════════
    function buildSeries(cfg, data) {
        return (data.series ?? []).map(s => {
            const base = {
                name: s.name,
                type: s.type,           // 後端決定：column / line / scatter
                color: s.color,          // 後端決定：廠區主題色
                yAxis: s.yAxis ?? 0,
                data: s.data,
                zIndex: s.yAxis === 1 ? 5 : 1,  // 達成率線畫在長條上方
            };

            // 虛線（累計達成率）
            if (s.dashStyle) base.dashStyle = s.dashStyle;

            // 達成率點：帶 isWarning 旗標，顏色已由後端每點設定
            if (s.yAxis === 1 && s.type !== 'column') {
                base.dataLabels = {
                    enabled: true,
                    formatter: function () {
                        const pt = this.point;
                        return `<span style="color:${pt.color ?? s.color}">${this.y}%</span>`;
                    },
                    useHTML: true,
                    style: { fontSize: '11px', fontWeight: 'normal' }
                };
                base.marker = {
                    enabled: true,
                    symbol: s.type === 'scatter' ? 'circle' : 'circle',
                    radius: 5,
                    fillColor: null   // 由每個點的 color 決定
                };
            }

            return base;
        });
    }

    // ════════════════════════════════════════════════════════
    //  Tooltip 格式化
    // ════════════════════════════════════════════════════════
    function tooltipFormatter(cfg) {
        const threshold = cfg.rateWarningThreshold ?? 95;
        const warning = cfg.seriesColors?.warning ?? '#D62728';

        return function () {
            let html = `<b>${this.x}</b><br/>`;
            this.points.forEach(pt => {
                const isRate = pt.series.yAxis.options.opposite;
                const color = pt.point.color ?? pt.series.color;
                const val = isRate
                    ? `<span style="color:${pt.y < threshold ? warning : color}"><b>${pt.y}%</b></span>`
                    : `<b>${Number(pt.y).toLocaleString()}</b>`;
                html += `<span style="color:${color}">●</span> ${pt.series.name}：${val}<br/>`;
            });
            return html;
        };
    }

    // ════════════════════════════════════════════════════════
    //  工具函式
    // ════════════════════════════════════════════════════════

    /** 從 series 陣列中撈出特定名稱系列的所有數字 */
    function flatNumbers(seriesArr, name) {
        const s = (seriesArr ?? []).find(x => x.name === name || x.dataField === name);
        if (!s) return [];
        return (s.data ?? []).map(d => typeof d === 'object' ? (d.y ?? 0) : d);
    }

    function showLoading() {
        elLoading.classList.remove('d-none');
        elError.classList.add('d-none');
        elContainer.style.visibility = 'hidden';
    }

    function showChart() {
        elLoading.classList.add('d-none');
        elError.classList.add('d-none');
        elContainer.style.visibility = 'visible';
    }

    function showError(msg) {
        elLoading.classList.add('d-none');
        elError.classList.remove('d-none');
        elErrorMsg.textContent = `載入失敗：${msg}`;
        elContainer.style.visibility = 'hidden';
    }

    // ════════════════════════════════════════════════════════
    //  事件綁定
    // ════════════════════════════════════════════════════════
    btnQuery.addEventListener('click', () => {
        const day = elInput.value.replaceAll('-', '/');
        if (day) loadChart(day);
    });

    btnRetry.addEventListener('click', () => {
        loadChart(elInput.value.replaceAll('-', '/') || queryDay);
    });

    // 初始載入
    loadChart(queryDay);

})();