/* =============================================
   Dashboard JavaScript cho Nhân Viên Lễ Tân
   File: dashboard.js
============================================= */

(function () {
    'use strict';

    // ===== BIẾN TOÀN CỤC =====
    let doanhThuChart = null;
    let trangThaiPhongChart = null;

    // ===== KHỞI TẠO KHI DOM SẴN SÀNG =====
    document.addEventListener('DOMContentLoaded', function () {
        console.log('[Dashboard] DOM loaded, waiting for Chart.js...');
        
        // Đợi Chart.js load xong
        waitForChartJS(function() {
            console.log('[Dashboard] Chart.js loaded, initializing dashboard...');
            initializeDashboard();
        });
    });

    /**
     * Đợi Chart.js load xong
     */
    function waitForChartJS(callback, maxAttempts) {
        maxAttempts = maxAttempts || 50; // Tối đa 5 giây (50 * 100ms)
        let attempts = 0;

        function checkChartJS() {
            attempts++;
            if (typeof Chart !== 'undefined') {
                callback();
            } else if (attempts < maxAttempts) {
                setTimeout(checkChartJS, 100);
            } else {
                console.error('[Dashboard] Chart.js failed to load after ' + maxAttempts + ' attempts');
            }
        }

        checkChartJS();
    }

    /**
     * Khởi tạo Dashboard
     */
    function initializeDashboard() {
        console.log('[Dashboard] Initializing...');

        // 1. Khởi tạo biểu đồ
        initCharts();

        // 2. Khởi tạo tooltips
        initTooltips();

        // 3. Setup auto-refresh
        setupAutoRefresh();

        // 4. Setup click handlers
        setupClickHandlers();

        console.log('[Dashboard] Initialized successfully');
    }

    /**
     * Khởi tạo biểu đồ (Line Chart + Doughnut Chart)
     */
    function initCharts() {
        // 1. Biểu đồ doanh thu (Line Chart)
        initDoanhThuChart();

        // 2. Biểu đồ trạng thái phòng (Doughnut Chart)
        initTrangThaiPhongChart();
    }

    /**
     * Khởi tạo biểu đồ doanh thu
     */
    function initDoanhThuChart() {
        const canvas = document.getElementById('doanhThuChart');
        if (!canvas) {
            console.error('[Dashboard] doanhThuChart canvas not found');
            return;
        }

        const ctx = canvas.getContext('2d');
        const chartData = getDoanhThuChartData();
        
        console.log('[Dashboard] Doanh thu chart data:', chartData);

        if (!chartData.labels || chartData.labels.length === 0) {
            console.warn('[Dashboard] No data for doanh thu chart');
        }

        try {
            doanhThuChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: chartData.labels || [],
                    datasets: [{
                        label: 'Doanh thu (VNĐ)',
                        data: chartData.data || [],
                        backgroundColor: 'rgba(102, 126, 234, 0.1)',
                        borderColor: 'rgba(102, 126, 234, 1)',
                        borderWidth: 2,
                        fill: true,
                        tension: 0.4,
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        pointBackgroundColor: 'rgba(102, 126, 234, 1)',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: false
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    return 'Doanh thu: ' + formatCurrency(context.parsed.y);
                                }
                            },
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            padding: 12,
                            titleColor: '#fff',
                            bodyColor: '#fff',
                            borderColor: 'rgba(102, 126, 234, 1)',
                            borderWidth: 1
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: function (value) {
                                    return (value / 1000000).toFixed(0) + 'tr';
                                }
                            },
                            grid: {
                                color: 'rgba(0, 0, 0, 0.05)'
                            }
                        },
                        x: {
                            grid: {
                                display: false
                            }
                        }
                    }
                }
            });

            console.log('[Dashboard] Doanh thu chart initialized successfully');
        } catch (error) {
            console.error('[Dashboard] Error initializing doanh thu chart:', error);
        }
    }

    /**
     * Khởi tạo biểu đồ trạng thái phòng
     */
    function initTrangThaiPhongChart() {
        const canvas = document.getElementById('trangThaiPhongChart');
        if (!canvas) {
            console.error('[Dashboard] trangThaiPhongChart canvas not found');
            return;
        }

        const ctx = canvas.getContext('2d');
        const chartData = getTrangThaiPhongChartData();
        
        console.log('[Dashboard] Trang thai phong chart data:', chartData);

        if (!chartData || chartData.length === 0) {
            console.warn('[Dashboard] No data for trang thai phong chart');
        }

        try {
            trangThaiPhongChart = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Trống', 'Đã đặt', 'Đang ở', 'Đang dọn', 'Bảo trì'],
                    datasets: [{
                        data: chartData || [0, 0, 0, 0, 0],
                        backgroundColor: [
                            '#28a745', // Xanh lá - Trống
                            '#ffc107', // Vàng - Đã đặt
                            '#dc3545', // Đỏ - Đang ở
                            '#17a2b8', // Xanh dương - Đang dọn
                            '#6c757d'  // Xám - Bảo trì
                        ],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: false
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const label = context.label || '';
                                    const value = context.parsed || 0;
                                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                    const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                                    return label + ': ' + value + ' phòng (' + percentage + '%)';
                                }
                            },
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            padding: 12
                        }
                    }
                }
            });

            console.log('[Dashboard] Trang thai phong chart initialized successfully');
        } catch (error) {
            console.error('[Dashboard] Error initializing trang thai phong chart:', error);
        }
    }

    /**
     * Lấy dữ liệu cho biểu đồ doanh thu
     */
    function getDoanhThuChartData() {
        // Ưu tiên lấy từ window object (được set từ server)
        if (window.doanhThuChartData) {
            console.log('[Dashboard] Found doanhThuChartData in window:', window.doanhThuChartData);
            return {
                labels: window.doanhThuChartData.labels || [],
                data: window.doanhThuChartData.values || []
            };
        }

        console.warn('[Dashboard] doanhThuChartData not found in window object');
        return { labels: [], data: [] };
    }

    /**
     * Lấy dữ liệu cho biểu đồ trạng thái phòng
     */
    function getTrangThaiPhongChartData() {
        // Ưu tiên lấy từ window object (được set từ server)
        if (window.trangThaiPhongChartData && Array.isArray(window.trangThaiPhongChartData)) {
            console.log('[Dashboard] Found trangThaiPhongChartData in window:', window.trangThaiPhongChartData);
            return window.trangThaiPhongChartData;
        }

        console.warn('[Dashboard] trangThaiPhongChartData not found in window object');
        return [0, 0, 0, 0, 0];
    }

    /**
     * Khởi tạo tooltips (Bootstrap)
     */
    function initTooltips() {
        if (typeof $ !== 'undefined' && $.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
            console.log('[Dashboard] Tooltips initialized');
        }
    }

    /**
     * Setup auto-refresh sau 5 phút
     */
    function setupAutoRefresh() {
        const REFRESH_INTERVAL = 5 * 60 * 1000; // 5 phút

        setTimeout(function () {
            console.log('[Dashboard] Auto-refreshing...');
            location.reload();
        }, REFRESH_INTERVAL);

        console.log('[Dashboard] Auto-refresh scheduled in 5 minutes');
    }

    /**
     * Setup click handlers cho các interactive elements
     */
    function setupClickHandlers() {
        // Click vào stat card để xem chi tiết
        document.querySelectorAll('.stat-card').forEach(function (card) {
            card.addEventListener('click', function () {
                const link = this.getAttribute('data-link');
                if (link) {
                    window.location.href = link;
                }
            });
        });

        // Click vào notification item
        document.querySelectorAll('.notification-item').forEach(function (item) {
            item.addEventListener('click', function () {
                const link = this.getAttribute('data-link');
                if (link) {
                    window.location.href = link;
                }
            });
        });

        console.log('[Dashboard] Click handlers initialized');
    }

    /**
     * Format currency VND
     */
    function formatCurrency(value) {
        if (!value && value !== 0) return 'N/A';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(value);
    }

    /**
     * Format date
     */
    function formatDate(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN');
    }

    /**
     * Format datetime
     */
    function formatDateTime(dateString) {
        if (!dateString) return 'N/A';
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN');
    }

    // Expose public methods
    window.DashboardUtils = {
        formatCurrency: formatCurrency,
        formatDate: formatDate,
        formatDateTime: formatDateTime,
        showNotification: function(message, type) {
            type = type || 'info';
            if (typeof window.showToast === 'function') {
                window.showToast(message, type);
            } else {
                alert(message);
            }
        }
    };

    console.log('[Dashboard] Module loaded');

})();
