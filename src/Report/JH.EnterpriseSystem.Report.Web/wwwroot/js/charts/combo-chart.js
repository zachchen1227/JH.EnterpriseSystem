/**
 * combo-chart.js  ── 前端完整控管外觀版 v3.1 (修正 Y 軸標題顯示)
 */

(function () {
    'use strict';

    const { factory, reportCode, queryDay } = window.__REPORT__;

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
    //  外觀設定
    // ════════════════════════════════════════════════════════
    const THEME = {
        actualColor: '#3A7BD5',
        targetColor: '#FF7F0E',
        rateColor: '#2CA02C',
        accRateColor: '#17BECF',
        warningColor: '#D62728',

        rateWarningThreshold: 95.0,
        yLeftMag: 1.5,
        yRightCeiling: 120,
        yRightFloor: 0,
        yRightGap: 15,

        yLeftTitle: '實際/目標產量',
        yRightTitle: '達成率',

        yLeftLabelColor: 'rgb(56,80,212)',
        yRightLabelColor: 'rgb(74,102,255)',

        xWeeklyFontSize: 15,
        xDailyFontSize: 14,
        xDailyRotation: -68,

        legendFontSize: 16,
        legendSymbolWidth: 20,
        legendBgColor: 'rgba(255,255,255,0.25)',

        qtyUnit: 'Pair',

        // 新增：預留給 Y 軸標題的空間 (Gutter space)
        yAxisTitleOffset: 60,
        chartSideMargin: 80
    };

    // ── Y 軸標題 HTML 組裝 ────────────────
    function buildYAxisTitleHtml(text, color) {
        return `<div style="
            font-weight: bold;
            font-size: 15px;
            color: ${color};
            writing-mode: vertical-rl;
            text-orientation: mixed;
            letter-spacing: 0.2em;
            white-space: nowrap;
            display: block;
        ">${text}</div>`;
    }

    Highcharts.setOptions({
        lang: {
            viewFullscreen: '全螢幕檢視圖表',
            contextButtonTitle: '工具選單',
            exitFullscreen: '離開全螢幕',
        }
    });

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

    function renderChart(payload) {
        const meta = payload.meta;
        const d = payload.data;

        elTitle.textContent = meta.title ?? reportCode;
        if (currentChart) { currentChart.destroy(); currentChart = null; }

        const isDaily = meta.isDaily;
        const hasAccRate = meta.hasAccumulatedRate;
        const warnThr = meta.rateWarningThreshold ?? THEME.rateWarningThreshold;

        const allQty = [...(d.actualQty ?? []), ...(d.targetQty ?? [])];
        const maxQty = allQty.length > 0 ? Math.max(...allQty) : 0;
        const yLeftMax = maxQty > 0 ? Math.ceil(maxQty * THEME.yLeftMag) : undefined;

        const tickAmount = Math.round(
            (THEME.yRightCeiling - THEME.yRightFloor) / THEME.yRightGap
        ) + 1;

        const series = [];

        // 1. 實際產量
        series.push({
            name: '實際產量',
            type: 'column',
            yAxis: 0,
            color: THEME.actualColor,
            data: d.actualQty ?? [],
            dataLabels: {
                enabled: true,
                inside: true,
                verticalAlign: 'bottom',
                style: {
                    textOutline: 'none',
                    fontSize: '11px',
                    fontWeight: 'bold',
                    color: '#ffffff',
                },
                formatter() {
                    return this.y > 0 ? Highcharts.numberFormat(this.y, 0, '.', ',') : null;
                }
            }
        });

        // 2. 目標產量
        series.push({
            name: '目標產量',
            type: 'scatter',
            yAxis: 0,
            color: THEME.targetColor,
            marker: { symbol: 'diamond', radius: 7, lineWidth: 0 },
            data: d.targetQty ?? [],
            tooltip: { valueSuffix: ` ${THEME.qtyUnit}` }
        });

        // 3. 達成率
        const dailyRate = d.dailyRate ?? [];
        if (dailyRate.length > 0) {
            const ratePoints = dailyRate.map(r => ({
                y: r,
                color: r < warnThr ? THEME.warningColor : THEME.rateColor,
            }));

            series.push({
                name: '達成率',
                type: isDaily ? 'scatter' : 'line',
                yAxis: 1,
                color: THEME.rateColor,
                marker: isDaily
                    ? { symbol: 'circle', radius: 7, lineWidth: 0 }
                    : { enabled: true, symbol: 'circle', radius: 4 },
                data: ratePoints,
                tooltip: { valueSuffix: ' %' },
                dataLabels: {
                    enabled: true,
                    useHTML: true,
                    style: { textOutline: 'none', fontSize: '11px' },
                    formatter() {
                        const clr = this.y < warnThr ? THEME.warningColor : THEME.rateColor;
                        return `<span style="color:${clr};font-weight:bold">${this.y}%</span>`;
                    }
                }
            });
        }

        // 4. 累計達成率
        if (hasAccRate && (d.accumulatedRate ?? []).length > 0) {
            series.push({
                name: '累計達成率',
                type: 'line',
                yAxis: 1,
                color: THEME.accRateColor,
                dashStyle: 'ShortDash',
                marker: { enabled: false },
                data: d.accumulatedRate,
                tooltip: { valueSuffix: ' %' }
            });
        }

        currentChart = Highcharts.chart(elContainer, {
            chart: {
                animation: false,
                // 確保左右預留足夠空間給標題與標籤
                marginLeft: THEME.chartSideMargin,
                marginRight: THEME.chartSideMargin,
                events: {
                    render() {
                        this.series.forEach(s => {
                            if (!s.points) return;
                            s.points.forEach((p, i) => {
                                if (!p.dataLabel) return;
                                if (i === 0) { p.dataLabel.show(); return; }
                                const prev = s.points[i - 1];
                                if (!prev?.dataLabel) return;
                                const gap = p.dataLabel.attr('x') -
                                    (prev.dataLabel.attr('x') + (prev.dataLabel.width || 0));
                                if (gap < 2) p.dataLabel.hide();
                                else p.dataLabel.show();
                            });
                        });
                    }
                }
            },

            title: { text: null },

            xAxis: {
                categories: d.categories ?? [],
                labels: {
                    style: {
                        fontSize: `${isDaily ? THEME.xDailyFontSize : THEME.xWeeklyFontSize}px`,
                        fontWeight: 'bold',
                    },
                    rotation: isDaily ? THEME.xDailyRotation : 0,
                },
                crosshair: true,
            },

            yAxis: [
                // ── 左軸 (產量) ──
                {
                    title: {
                        useHTML: true,
                        text: buildYAxisTitleHtml(THEME.yLeftTitle, THEME.yLeftLabelColor),
                        rotation: 0,
                        offset: THEME.yAxisTitleOffset // 關鍵：推開標題位置
                    },
                    labels: {
                        style: { color: THEME.yLeftLabelColor, fontWeight: 'bold' },
                        formatter() {
                            return Highcharts.numberFormat(this.value, 0, '.', ',');
                        }
                    },
                    max: yLeftMax,
                    min: 0,
                    gridLineWidth: 1,
                    opposite: false,
                },
                // ── 右軸 (達成率) ──
                {
                    title: {
                        useHTML: true,
                        text: buildYAxisTitleHtml(THEME.yRightTitle, THEME.yRightLabelColor),
                        rotation: 0,
                        offset: THEME.yAxisTitleOffset // 關鍵：推開標題位置
                    },
                    labels: {
                        format: '{value} %',
                        style: { color: THEME.yRightLabelColor, fontWeight: 'bold' },
                    },
                    max: THEME.yRightCeiling,
                    min: THEME.yRightFloor,
                    tickAmount: tickAmount,
                    gridLineWidth: 0,
                    opposite: true,
                }
            ],

            legend: {
                verticalAlign: 'top',
                itemStyle: {
                    fontSize: `${THEME.legendFontSize}px`,
                    fontWeight: 'bold',
                    color: '#333333',
                },
                symbolWidth: THEME.legendSymbolWidth,
                backgroundColor: THEME.legendBgColor,
            },

            tooltip: {
                shared: true,
                useHTML: true,
                formatter() {
                    let html = `<div style="background:rgba(255,255,255,0.92);padding:6px 10px;border-radius:4px;border:1px solid #ccc;font-size:13px">`;
                    html += `<b>${this.x}</b><br/>`;
                    this.points.forEach(pt => {
                        const rawColor = pt.point.color || pt.color || pt.series.color;
                        const val = pt.series.name.includes('率')
                            ? `${pt.y} %`
                            : `${Highcharts.numberFormat(pt.y, 0, '.', ',')} ${THEME.qtyUnit}`;
                        html += `<span style="color:${rawColor}">●</span> ${pt.series.name}: <b>${val}</b><br/>`;
                    });
                    html += '</div>';
                    return html;
                }
            },

            plotOptions: {
                series: { animation: false },
                column: { groupPadding: 0.1, pointPadding: 0.05, borderWidth: 0 }
            },

            credits: { enabled: false },
            series,
        });

        showChart();
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
        elErrorMsg.textContent = msg;
        elContainer.style.visibility = 'hidden';
    }

    btnQuery.addEventListener('click', () => {
        loadChart(elInput.value.replace(/-/g, '/'));
    });
    btnRetry.addEventListener('click', () => {
        loadChart(elInput.value.replace(/-/g, '/'));
    });

    loadChart(window.__REPORT__.queryDay);

})();