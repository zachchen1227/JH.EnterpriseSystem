/**
 * report.core.js
 * 數值運算、日期格式化等工具
 */

const ReportUtils = {
    // 防止 NaN 轉為 0
    checkNaNToZero: function (number) {
        if (Number.isNaN(number) || !Number.isFinite(number)) {
            return 0;
        }
        return number;
    },

    // 格式化數字 (千分位)
    formatNumber: function (number, minDigits = 0, maxDigits = 0) {
        number = this.checkNaNToZero(parseFloat(number));
        return number.toLocaleString('en-us', {
            maximumFractionDigits: maxDigits,
            minimumFractionDigits: minDigits
        });
    },

    // 計算達成率 (回傳百分比數值)
    calcRate: function (target, actual) {
        const t = +target || 0;
        const a = +actual || 0;
        return t > 0 ? Math.round((a / t) * 1000) / 10 : 0;
    }
};

// 2. API 服務 (使用 fetch 取代 $.ajax)
const ReportService = {
    /**
     * 呼叫後端 API 取得圖表資料
     * 對應後端: /api/chart/{factory}/{reportCode}?queryDay=...
     */
    getChartData: async function (factory, reportCode, queryDay) {
        const url = `/api/chart/${factory}/${reportCode}?queryDay=${encodeURIComponent(queryDay)}`;

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                // 處理 404, 500 等錯誤
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error("API Error:", error);
            throw error; // 將錯誤拋出讓前端處理 (例如顯示 alert)
        }
    }
};