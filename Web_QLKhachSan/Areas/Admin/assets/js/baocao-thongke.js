// JavaScript cho trang Báo cáo Nâng cao
document.addEventListener('DOMContentLoaded', function() {
    
    // ======== UTILITY FUNCTIONS ========
    // Hàm format số tiền thành dạng viết tắt (M, B)
    function formatCurrency(value) {
        if (value >= 1000000000) {
            return (value / 1000000000).toFixed(1).replace(/\.0$/, '') + 'B ₫';
        } else if (value >= 1000000) {
            return (value / 1000000).toFixed(0) + 'M ₫';
        } else if (value >= 1000) {
            return (value / 1000).toFixed(0) + 'K ₫';
        }
        return value.toLocaleString('vi-VN') + ' ₫';
    }

    // Cập nhật giá trị doanh thu với format
    const totalRevenueElement = document.getElementById('totalRevenue');
    if (totalRevenueElement) {
        const revenueValue = 850000000; // 850 triệu
        totalRevenueElement.textContent = formatCurrency(revenueValue);
    }
    
    // ======== DATA MANAGEMENT ======== 
    let chartData = {
        revenue: {
            labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
            data: [450, 520, 480, 610, 580, 650, 720, 690, 750, 680, 700, 820]
        },
        roomTypes: {
            labels: ['Phòng Đơn', 'Phòng Đôi', 'Phòng Gia Đình', 'Suite VIP'],
            data: [35, 40, 15, 10]
        }
    };

    // Biểu đồ Doanh thu theo tháng
    const revenueCtx = document.getElementById('revenueChart').getContext('2d');
    const revenueChart = new Chart(revenueCtx, {
        type: 'line',
        data: {
            labels: chartData.revenue.labels,
            datasets: [{
                label: 'Doanh thu (triệu ₫)',
                data: chartData.revenue.data,
                borderColor: '#667eea',
                backgroundColor: 'rgba(102, 126, 234, 0.1)',
                tension: 0.4,
                fill: true,
                pointRadius: 5,
                pointHoverRadius: 7
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return value + 'tr';
                        }
                    }
                }
            }
        }
    });

    // Biểu đồ Tỷ lệ loại phòng
    const roomTypeCtx = document.getElementById('roomTypeChart').getContext('2d');
    const roomTypeChart = new Chart(roomTypeCtx, {
        type: 'doughnut',
        data: {
            labels: chartData.roomTypes.labels,
            datasets: [{
                data: chartData.roomTypes.data,
                backgroundColor: [
                    '#667eea',
                    '#f5576c',
                    '#4facfe',
                    '#43e97b'
                ],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                }
            }
        }
    });

    // ======== FILTER FUNCTIONALITY ======== 
    const reportType = document.getElementById('reportType');
    const timePeriod = document.getElementById('timePeriod');

    reportType.addEventListener('change', function() {
        updateChartByType(this.value, timePeriod.value);
        showNotification(`Đã cập nhật báo cáo ${getReportTypeName(this.value)}`, 'success');
    });

    timePeriod.addEventListener('change', function() {
        updateChartByType(reportType.value, this.value);
        showNotification(`Đã cập nhật khoảng thời gian ${getTimePeriodName(this.value)}`, 'success');
    });

    function getReportTypeName(type) {
        const types = {
            revenue: 'Doanh thu',
            booking: 'Đặt phòng',
            customer: 'Khách hàng',
            service: 'Dịch vụ'
        };
        return types[type] || type;
    }

    function getTimePeriodName(period) {
        const periods = {
            week: '7 ngày qua',
            month: '30 ngày qua',
            quarter: '3 tháng qua',
            year: '12 tháng qua'
        };
        return periods[period] || period;
    }

    function updateChartByType(type, period) {
        let newData, newLabels;

        // Tạo nhãn dựa trên khoảng thời gian
        if (period === 'week') {
            newLabels = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
            newData = Array.from({length: 7}, () => Math.floor(Math.random() * 100) + 50);
        } else if (period === 'month') {
            newLabels = Array.from({length: 30}, (_, i) => `${i+1}`);
            newData = Array.from({length: 30}, () => Math.floor(Math.random() * 100) + 50);
        } else if (period === 'quarter') {
            newLabels = ['T1', 'T2', 'T3'];
            newData = Array.from({length: 3}, () => Math.floor(Math.random() * 500) + 400);
        } else {
            newLabels = ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'];
            newData = Array.from({length: 12}, () => Math.floor(Math.random() * 400) + 400);
        }

        // Điều chỉnh dữ liệu theo loại báo cáo
        if (type === 'booking') {
            newData = newData.map(val => Math.floor(val / 10));
        } else if (type === 'customer') {
            newData = newData.map(val => Math.floor(val / 5));
        } else if (type === 'service') {
            newData = newData.map(val => Math.floor(val / 3));
        }

        // Cập nhật biểu đồ
        revenueChart.data.labels = newLabels;
        revenueChart.data.datasets[0].data = newData;
        revenueChart.data.datasets[0].label = getReportTypeName(type) + 
            (type === 'revenue' ? ' (triệu ₫)' : 
             type === 'booking' ? ' (lượt)' : 
             type === 'customer' ? ' (khách)' : ' (dịch vụ)');
        revenueChart.update();
    }

    // ======== EXPORT FUNCTIONALITY ======== 
    const exportBtn = document.getElementById('exportBtn');
    exportBtn.addEventListener('click', function() {
        const type = reportType.value;
        const period = timePeriod.value;

        // Tạo CSV content
        let csvContent = '\uFEFF'; // UTF-8 BOM
        csvContent += `Báo cáo ${getReportTypeName(type)} - ${getTimePeriodName(period)}\n`;
        csvContent += `Ngày xuất: ${new Date().toLocaleString('vi-VN')}\n\n`;
        csvContent += 'Kỳ,Giá trị\n';
        
        const labels = revenueChart.data.labels;
        const data = revenueChart.data.datasets[0].data;
        
        labels.forEach((label, index) => {
            csvContent += `${label},${data[index]}\n`;
        });

        // Thêm thống kê tổng hợp
        const total = data.reduce((a, b) => a + b, 0);
        const avg = (total / data.length).toFixed(2);
        const max = Math.max(...data);
        const min = Math.min(...data);
        
        csvContent += `\nThống kê tổng hợp:\n`;
        csvContent += `Tổng cộng,${total}\n`;
        csvContent += `Trung bình,${avg}\n`;
        csvContent += `Cao nhất,${max}\n`;
        csvContent += `Thấp nhất,${min}\n`;

        // Tạo và tải file
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        const fileName = `BaoCao_${type}_${period}_${new Date().toISOString().slice(0,10)}.csv`;
        
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        showNotification('Đã xuất báo cáo Excel thành công!', 'success');
    });

    // Hàm hiển thị thông báo
    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.style.cssText = `
            position: fixed;
            top: 100px;
            right: 20px;
            padding: 1rem 1.5rem;
            background: ${type === 'success' ? '#27ae60' : type === 'error' ? '#e74c3c' : '#3498db'};
            color: white;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.2);
            z-index: 9999;
            animation: slideIn 0.3s ease;
            font-weight: 500;
        `;
        notification.textContent = message;
        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.animation = 'slideOut 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Thêm CSS animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideIn {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }
        @keyframes slideOut {
            from { transform: translateX(0); opacity: 1; }
            to { transform: translateX(100%); opacity: 0; }
        }
    `;
    document.head.appendChild(style);
});

