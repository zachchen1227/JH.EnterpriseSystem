/**
 * report.config.js
 * 集中管理所有報表的顏色、圖示與部門設定
 */

const ReportConfig = (function () {
    // 1. 基礎色票 (來自舊系統 jhOptions.js)
    const Colors = {
        Blue: '#3A7BD5',
        Orange: '#FF7F0E',
        Green: '#2CA02C',
        Cyan: '#17BECF',
        Red: '#D62728',
        Black: 'black',
        White: 'white'
    };

    // 2. 圖示定義
    const Symbols = {
        circle: { code: "\u25CF", fontSize: 18 },
        square: { code: "&#9632", fontSize: 12 },
        diamond: { code: "\u25C6", fontSize: 13 }
    };

    // 3. 部門專屬設定 (工廠模式)
    // 原本分散在 highchartsBU1.js, BU2.js 的顏色差異，在這裡統一定義
    const DepartmentThemes = {
        // BU1: 藍色系為主
        'BU1': {
            primary: Colors.Blue,
            target: Colors.Orange,
            actual: Colors.Green,
            cumulative: Colors.Cyan
        },
        // BU2: 橘色系為主 (假設邏輯，可依舊系統調整)
        'BU2': {
            primary: Colors.Orange,
            target: Colors.Cyan,
            actual: Colors.Red,
            cumulative: Colors.Green
        },
        // JT1: 綠色系為主
        'JT1': {
            primary: Colors.Green,
            target: Colors.Blue,
            actual: Colors.Red,
            cumulative: Colors.Orange
        },
        // 預設
        'DEFAULT': {
            primary: Colors.Blue,
            target: Colors.Orange,
            actual: Colors.Green,
            cumulative: Colors.Cyan
        }
    };

    return {
        // 取得特定部門的設定
        getTheme: function (deptName) {
            return DepartmentThemes[deptName] || DepartmentThemes['DEFAULT'];
        },

        // 取得圖示定義
        getSymbol: function (name) {
            return Symbols[name];
        },

        // Highcharts 全域預設設定
        commonChartOptions: {
            credits: { enabled: false }, // 隱藏 Highcharts 商標
            title: {
                style: { fontSize: '24px', fontWeight: 'bold' }
            },
            lang: {
                viewFullscreen: "全螢幕檢視圖表",
                contextButtonTitle: '工具選單'
            },
            chart: {
                style: { fontFamily: 'Arial, Microsoft JhengHei, sans-serif' }
            }
        }
    };
})();