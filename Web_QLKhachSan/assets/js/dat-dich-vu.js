// Đặt Dịch Vụ - JavaScript đơn giản cho backend
document.addEventListener('DOMContentLoaded', function() {
    
    // Thiết lập ngày tối thiểu là hôm nay
    const dateInput = document.querySelector('input[name="PreferredDate"]');
    if (dateInput) {
        const today = new Date().toISOString().split('T')[0];
        dateInput.min = today;
    }    
});
