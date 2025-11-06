document.addEventListener('DOMContentLoaded', () => {
    // Helper function to format currency
    const formatCurrency = (value) => new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);

    // Chart Colors
    const chartColors = {
        primary: 'rgba(212, 175, 55, 0.8)',
        primaryLight: 'rgba(212, 175, 55, 0.2)',
        secondary: 'rgba(10, 25, 49, 0.8)',
        grey: 'rgba(54, 54, 54, 0.8)',
        lightGrey: 'rgba(200, 200, 200, 0.8)',
    };

    // ====== REVENUE CHART (BAR) ======
    const revenueCtx = document.getElementById('revenueChart');
    if (revenueCtx) {
        const revenueChart = new Chart(revenueCtx, {
            type: 'bar',
            data: {
                labels: ['Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9'],
                datasets: [{
                    label: 'Doanh Thu',
                    data: [28000, 35000, 42000, 38000, 55000, 61000],
                    backgroundColor: chartColors.primary,
                    borderColor: chartColors.primary,
                    borderWidth: 1,
                    borderRadius: 8,
                    hoverBackgroundColor: chartColors.secondary,
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
                            label: function(context) {
                                return `Doanh thu: ${formatCurrency(context.raw)}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value, index, values) {
                                return formatCurrency(value);
                            }
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
    }

    // ====== SERVICES CHART (DOUGHNUT) ======
    const servicesCtx = document.getElementById('servicesChart');
    if (servicesCtx) {
        const servicesChart = new Chart(servicesCtx, {
            type: 'doughnut',
            data: {
                labels: ['Ăn uống', 'Giặt ủi', 'Spa & Massage', 'Thuê xe', 'Khác'],
                datasets: [{
                    label: 'Tỷ trọng dịch vụ',
                    data: [40, 25, 18, 12, 5],
                    backgroundColor: [
                        'rgba(212, 175, 55, 0.9)',
                        'rgba(102, 126, 234, 0.9)',
                        'rgba(245, 87, 108, 0.9)',
                        'rgba(79, 172, 254, 0.9)',
                        'rgba(149, 165, 166, 0.9)'
                    ],
                    borderColor: '#ffffff',
                    borderWidth: 5,
                    hoverOffset: 15
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            boxWidth: 18,
                            padding: 20,
                            font: {
                                size: 14,
                                weight: '600'
                            },
                            color: '#0a1931'
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(10, 25, 49, 0.95)',
                        titleColor: '#d4af37',
                        bodyColor: '#ffffff',
                        padding: 15,
                        cornerRadius: 10,
                        displayColors: true,
                        callbacks: {
                            label: function(context) {
                                let label = context.label || '';
                                let value = context.parsed || 0;
                                let total = context.dataset.data.reduce((a, b) => a + b, 0);
                                let percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value}% (${percentage}% tổng dịch vụ)`;
                            }
                        }
                    }
                },
                cutout: '60%'
            }
        });
    }

    // ====== STAFF PERFORMANCE CHART (HORIZONTAL BAR) ======
    const staffPerformanceCtx = document.getElementById('staffPerformanceChart');
    if (staffPerformanceCtx) {
        const staffPerformanceChart = new Chart(staffPerformanceCtx, {
            type: 'bar',
            data: {
                labels: ['Trần Minh Quân', 'Lê Thị Hoa', 'Phạm Văn Hùng', 'Nguyễn Anh Dũng', 'Võ Thị Mai'],
                datasets: [{
                    label: 'Số lượng công việc hoàn thành',
                    data: [95, 88, 65, 92, 78],
                    backgroundColor: [
                        'rgba(212, 175, 55, 0.9)',
                        'rgba(102, 126, 234, 0.9)',
                        'rgba(245, 87, 108, 0.9)',
                        'rgba(79, 172, 254, 0.9)',
                        'rgba(168, 224, 99, 0.9)'
                    ],
                    borderColor: [
                        'rgba(212, 175, 55, 1)',
                        'rgba(102, 126, 234, 1)',
                        'rgba(245, 87, 108, 1)',
                        'rgba(79, 172, 254, 1)',
                        'rgba(168, 224, 99, 1)'
                    ],
                    borderWidth: 2,
                    borderRadius: 10
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(10, 25, 49, 0.95)',
                        titleColor: '#d4af37',
                        bodyColor: '#ffffff',
                        padding: 15,
                        cornerRadius: 10,
                        callbacks: {
                            label: function(context) {
                                return `Hoàn thành: ${context.parsed.x} công việc`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        max: 100,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        },
                        ticks: {
                            color: '#666',
                            font: {
                                size: 12
                            }
                        }
                    },
                    y: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            color: '#0a1931',
                            font: {
                                size: 13,
                                weight: '600'
                            }
                        }
                    }
                }
            }
        });
    }
});
