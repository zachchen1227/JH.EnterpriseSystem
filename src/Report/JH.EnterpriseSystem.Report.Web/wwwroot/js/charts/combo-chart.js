/**
 * combo-chart.js  ── 完整對齊舊系統外觀版
 *
 * 設計原則：
 *   - 所有顏色、樣式、業務規則 全部從 API config 讀取，不寫死
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
    //  Highcharts 全域設定（全螢幕中文）
    // ════════════════════════════════════════════════════════
    Highcharts.setOptions({
        lang: {
            viewFullscreen: '全螢幕檢視圖表',
            contextButtonTitle: '工具選單',
            exitFullscreen: '離開全螢幕',
        }
    });

    // ════════════════════════════════════════════════════════
    //  Highcharts 渲染
    // ════════════════════════════════════════════════════════
    function renderChart(payload) {
        const cfg = payload.config;
        const data = payload.data;

        // 更新 DOM 標題
        elTitle.textContent = cfg.title ?? reportCode;

        // 銷毀舊圖表
        if (currentChart) { currentChart.destroy(); currentChart = null; }

        // ── 全域顏色陣列 ──────────────────────────────────
        Highcharts.setOptions({ colors: cfg.colors ?? [] });

        // ── Y 軸 ──────────────────────────────────────────
        const yAxes = buildYAxes(cfg.yAxes, data); 

        // ── Series ────────────────────────────────────────
        const series = buildSeries(cfg, data);

        // ── 縮放設定 ─────────────────────────────────────
        const zoomType = cfg.plotOptions?.enableZooming ? 'xy' : undefined;

        // ── 建立圖表 ──────────────────────────────────────
        currentChart = Highcharts.chart(elContainer, {

            // ── 圖表容器 ──────────────────────────────────
            chart: {
                animation: true,
                style: { fontFamily: 'system-ui, sans-serif' },
                zooming: zoomType ? { type: zoomType } : undefined,
                events: cfg.plotOptions?.enableOverlapFix
                    ? { render: buildOverlapFixHandler() }
                    : {}
            },

            // ── 標題（顯示在 DOM，Highcharts 標題設為 null）──
            title: { text: null },
            subtitle: { text: null },

            // ── 全螢幕 / 匯出 ─────────────────────────────
            exporting: cfg.enableExporting
                ? {
                    enabled: true,
                    buttons: {
                        contextButton: {
                            menuItems: ['viewFullscreen', 'separator', 'downloadPNG', 'downloadPDF']
                        }
                    }
                }
                : { enabled: false },

            // ── X 軸 ──────────────────────────────────────
            xAxis: buildXAxis(cfg.xAxis, data.categories),

            // ── Y 軸 ──────────────────────────────────────
            yAxis: yAxes,

            // ── Tooltip ───────────────────────────────────
            tooltip: buildTooltip(cfg),

            // ── Legend ────────────────────────────────────
            legend: buildLegend(cfg.legend),

            // ── PlotOptions ───────────────────────────────
            plotOptions: buildPlotOptions(cfg),

            // ── Series ────────────────────────────────────
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
    //  X 軸建構
    // ════════════════════════════════════════════════════════
    function buildXAxis(xCfg, categories) {
        const fontSize = xCfg?.fontSize ?? 15;
        const fontBold = xCfg?.fontBold ?? true;
        const rotation = xCfg?.labelRotation ?? 0;

        return {
            categories,
            crosshair: true,
            tickmarkPlacement: 'on',
            labels: {
                rotation,
                style: {
                    fontSize: `${fontSize}px`,
                    fontWeight: fontBold ? 'bold' : 'normal',
                }
            }
        };
    }

    // ════════════════════════════════════════════════════════
    //  Y 軸建構
    // ════════════════════════════════════════════════════════
    function buildYAxes(yAxesConfig, data) {
        if (!yAxesConfig || yAxesConfig.length === 0)
            return [{ title: { text: '數量' } }];

        return yAxesConfig.map((ax, i) => {
            const base = {
                // ── 標題：使用 HTML 直書版本 ──
                title: {
                    useHTML: true,
                    text: ax.titleHtml ?? ax.title?.text ?? '',
                    rotation: 0,
                    x: ax.titleOffsetX ?? 0,
                },
                opposite: ax.opposite ?? false,
                gridLineWidth: i === 0 ? 1 : 0,
                // ── 刻度標籤顏色 ──
                labels: {
                    style: {
                        color: i === 0
                            ? (ax.labelColor ?? 'rgb(56,80,212)')
                            : (ax.labelColor ?? 'rgb(74,102,255)'),
                        fontSize: '12px',
                    },
                    ...(i === 1 && ax.labelFormat ? { format: ax.labelFormat } : {}),
                    ...(i === 1 && ax.labelOffsetX ? { x: ax.labelOffsetX } : {}),
                }
            };

            if (i === 0) {
                // 左軸（產量）
                const allQty = [
                    ...flatNumbers(data.series, '實際產量'),
                    ...flatNumbers(data.series, '目標產量'),
                ];
                const dataMax = allQty.length > 0 ? Math.max(...allQty) : 0;
                base.min = 0;
                base.max = ax.maxOverride != null
                    ? ax.maxOverride
                    : Math.ceil(dataMax * (ax.magnification ?? 1.2));
            } else {
                // 右軸（達成率）
                const ceiling = ax.ceiling ?? 120;
                const floor = ax.floor ?? 0;
                const gap = ax.gap ?? 15;
                base.min = floor;
                base.max = ceiling;
                base.tickAmount = Math.round((ceiling - floor) / gap) + 1;
                base.plotLines = [{
                    value: 100, color: '#999', dashStyle: 'ShortDash', width: 1,
                    label: { text: '100%', align: 'right', style: { color: '#999' } }
                }];
            }

            return base;
        });
    }

    // ════════════════════════════════════════════════════════
    //  Legend 建構
    // ════════════════════════════════════════════════════════
    function buildLegend(lgCfg) {
        if (!lgCfg) return { enabled: true };
        return {
            enabled: true,
            align: 'center',
            verticalAlign: lgCfg.verticalAlign ?? 'top',       // ★ 舊系統：top
            layout: 'horizontal',
            itemStyle: {
                fontSize: `${lgCfg.fontSize ?? 20}px`,         // ★ 舊系統：20px
                fontWeight: 'bold',
            },
            symbolRadius: lgCfg.symbolSquare ? 0 : undefined,  // ★ 舊系統：0（方形）
            symbolWidth: lgCfg.symbolWidth ?? 20,             // ★ 舊系統：20
            backgroundColor: lgCfg.backgroundColor ?? 'rgba(255,255,255,0.25)',
        };
    }

    // ════════════════════════════════════════════════════════
    //  Tooltip 建構
    // ════════════════════════════════════════════════════════
    function buildTooltip(cfg) {
        const ttCfg = cfg.tooltip ?? {};
        const threshold = cfg.rateWarningThreshold ?? 95;
        const warning = cfg.seriesColors?.warning ?? '#D62728';
        const qtyUnit = ttCfg.qtyUnit ?? 'Pair';

        return {
            shared: true,
            useHTML: true,
            backgroundColor: ttCfg.transparent ? 'transparent' : undefined,
            padding: ttCfg.transparent ? 0 : undefined,
            followPointer: ttCfg.followPointer ?? true,
            formatter: function () {
                let html = `<div class="tev">`;
                html += `<span style="font-size:10px">${this.x}</span><br/>`;
                this.points.forEach(pt => {
                    const isRate = pt.series.yAxis.options.opposite;
                    const color = pt.point.color ?? pt.series.color;

                    // ★ 修正：用 options.type 取代 series.type（Highcharts v10+ 相容）
                    const seriesType = pt.series.options?.type ?? pt.series.type ?? '';
                    const symbol = seriesType === 'column' ? '■' :
                        seriesType === 'scatter' ? '◆' : '●';

                    const val = isRate
                        ? `<b>${pt.y}%</b>`
                        : `<b>${Number(pt.y).toLocaleString()} ${qtyUnit}</b>`;

                    html += `<span style="color:${color}">${symbol}</span> ` +
                        `${pt.series.name}：` +
                        `<span style="color:${isRate && pt.y < threshold ? warning : color}">${val}</span><br/>`;
                });
                html += `</div>`;
                return html;
            }
        };
    }
    // ════════════════════════════════════════════════════════
    //  PlotOptions 建構
    // ════════════════════════════════════════════════════════
    function buildPlotOptions(cfg) {
        const dlCfg = cfg.dataLabel ?? {};
        const poCfg = cfg.plotOptions ?? {};

        return {
            column: {
                grouping: true,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    inside: dlCfg.columnInside ?? true,
                    verticalAlign: dlCfg.columnVerticalAlign ?? 'bottom',
                    allowOverlap: true,
                    crop: false,
                    overflow: 'none',
                    useHTML: true,
                    formatter: function () {
                        if (!this.y) return '';
                        return `<span>${Number(this.y).toLocaleString()}</span>`;
                    },
                    style: {
                        textOutline: dlCfg.noTextOutline ? 'none' : undefined,
                    }
                }
            },
            scatter: {
                marker: {
                    symbol: 'diamond',
                    radius: 6,
                    lineWidth: 1,
                    lineColor: '#333'
                },
                // ★ 修正：scatter 的 dataLabels 預設關閉
                //   yAxis=1 的達成率 scatter 會在 buildSeries 裡自己開啟
                //   yAxis=0 的目標產量 scatter 不需要 dataLabels
                dataLabels: {
                    enabled: false
                }
            },
            line: {
                lineWidth: 2
            },
            series: {
                states: poCfg.disableInactiveState
                    ? { inactive: { enabled: false } }
                    : {}
            }
        };
    }

    // ════════════════════════════════════════════════════════
    //  Series 建構
    // ════════════════════════════════════════════════════════
    function buildSeries(cfg, data) {
        const dlCfg = cfg.dataLabel ?? {};

        return (data.series ?? []).map(s => {
            const base = {
                name: s.name,
                type: s.type,
                color: s.color,
                yAxis: s.yAxis ?? 0,
                data: s.data,
                zIndex: s.yAxis === 1 ? 5 : 1,
            };

            // 虛線（累計達成率）
            if (s.dashStyle) base.dashStyle = s.dashStyle;

            // 達成率系列（右軸非長條）→ 顯示百分比 dataLabel
            if (s.yAxis === 1 && s.type !== 'column') {
                base.dataLabels = {
                    enabled: true,
                    allowOverlap: true,
                    useHTML: true,
                    style: {
                        fontSize: '11px',
                        fontWeight: 'normal',
                        textOutline: dlCfg.noTextOutline ? 'none' : undefined,
                    },
                    formatter: function () {
                        const pt = this.point;
                        return `<span style="color:${pt.color ?? s.color}">${this.y}%</span>`;
                    }
                };
                base.marker = {
                    enabled: true,
                    symbol: 'circle',
                    radius: 5,
                    fillColor: null
                };
            }

            return base;
        });
    }

    // ════════════════════════════════════════════════════════
    //  ★ 舊系統 render 防重疊 Function
    //    series[1]=目標產量 / series[2]=達成率 的 dataLabel 重疊修正
    // ════════════════════════════════════════════════════════
    function buildOverlapFixHandler() {
        return function () {
            const series = this.series;
            if (!series[1] || !series[2]) return;

            series[1].points.forEach(function (point, index) {
                const dl1 = series[1].points[index]?.dataLabel;
                const dl2 = series[2].points[index]?.dataLabel;
                if (!dl1 || !dl2) return;

                const dl1Top = dl1.y;
                const dl1Bottom = dl1.y + dl1.height;
                const dl2Top = dl2.y;
                const dl2Bottom = dl2.y + dl2.height;

                const isOverlapping = !(dl1Bottom < dl2Top || dl2Bottom < dl1Top);

                if (isOverlapping) {
                    if (dl1Top > dl2Top) {
                        point.dataLabel.attr({ translateY: point.dataLabel.y - 25 });
                    } else {
                        point.dataLabel.attr({ translateY: point.dataLabel.y + 25 });
                    }
                }
            });
        };
    }

    // ════════════════════════════════════════════════════════
    //  工具函式
    // ════════════════════════════════════════════════════════
    function flatNumbers(seriesArr, name) {
        const s = (seriesArr ?? []).find(x => x.name === name);
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