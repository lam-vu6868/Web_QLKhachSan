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
        initializeDashboard();
    });

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
        const doanhThuCanvas = document.getElementById('doanhThuChart');
        if (doanhThuCanvas) {
          const ctx = doanhThuCanvas.getContext('2d');
  const chartData = getDoanhThuChartData();

        doanhThuChart = new Chart(ctx, {
            type: 'line',
              data: {
      labels: chartData.labels,
  datasets: [{
  label: 'Doanh thu (VNĐ)',
       data: chartData.data,
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

 console.log('[Dashboard] Doanh thu chart initialized');
        }

        // 2. Biểu đồ trạng thái phòng (Doughnut Chart)
        const trangThaiCanvas = document.getElementById('trangThaiPhongChart');
        if (trangThaiCanvas) {
    const ctx = trangThaiCanvas.getContext('2d');
            const chartData = getTrangThaiPhongChartData();

          trangThaiPhongChart = new Chart(ctx, {
type: 'doughnut',
       data: {
              labels: ['Trống', 'Đã đặt', 'Đang ở', 'Đang dọn', 'Bảo trì'],
             datasets: [{
          data: chartData,
   backgroundColor: [
          '#28a745',
     '#ffc107',
   '#dc3545',
        '#17a2b8',
 '#6c757d'
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

  console.log('[Dashboard] Trang thai phong chart initialized');
   }
    }

    /**
     * Lấy dữ liệu cho biểu đồ doanh thu từ HTML
     */
    function getDoanhThuChartData() {
        // Data được embedded từ server vào hidden elements hoặc data attributes
        const chartElement = document.getElementById('doanhThuChart');
        if (!chartElement) return { labels: [], data: [] };

        const labels = chartElement.getAttribute('data-labels');
        const data = chartElement.getAttribute('data-values');

   return {
  labels: labels ? JSON.parse(labels) : [],
       data: data ? JSON.parse(data) : []
        };
    }

    /**
     * Lấy dữ liệu cho biểu đồ trạng thái phòng từ HTML
     */
    function getTrangThaiPhongChartData() {
        const chartElement = document.getElementById('trangThaiPhongChart');
     if (!chartElement) return [0, 0, 0, 0, 0];

        const data = chartElement.getAttribute('data-values');
  return data ? JSON.parse(data) : [0, 0, 0, 0, 0];
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

    /**
     * Reload dashboard data (AJAX)
     */
    function reloadDashboardData() {
        console.log('[Dashboard] Reloading data...');

        if (typeof $ === 'undefined') {
       console.warn('[Dashboard] jQuery not available, using full page reload');
       location.reload();
return;
 }

    $.ajax({
            url: window.location.href,
     type: 'GET',
            data: { ajax: true },
            success: function (response) {
         console.log('[Dashboard] Data reloaded successfully');
         // Update charts
            updateCharts(response);
       },
  error: function (xhr, status, error) {
     console.error('[Dashboard] Failed to reload data:', error);
   }
        });
    }

    /**
     * Update charts với data mới
     */
    function updateCharts(data) {
   // Update doanh thu chart
 if (doanhThuChart && data.doanhThu) {
            doanhThuChart.data.labels = data.doanhThu.labels;
        doanhThuChart.data.datasets[0].data = data.doanhThu.data;
 doanhThuChart.update();
        }

        // Update trang thai phong chart
        if (trangThaiPhongChart && data.trangThaiPhong) {
         trangThaiPhongChart.data.datasets[0].data = data.trangThaiPhong;
            trangThaiPhongChart.update();
   }

        console.log('[Dashboard] Charts updated');
 }

    /**
     * Show notification toast
     */
    function showNotification(message, type) {
        type = type || 'info';

        if (typeof window.showToast === 'function') {
          window.showToast(message, type);
        } else {
   alert(message);
     }
    }

    // Expose public methods
    window.DashboardUtils = {
   formatCurrency: formatCurrency,
        formatDate: formatDate,
   formatDateTime: formatDateTime,
        reloadData: reloadDashboardData,
        showNotification: showNotification
    };

    console.log('[Dashboard] Module loaded');

})();
