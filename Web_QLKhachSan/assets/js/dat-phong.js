// Script đơn giản cho đặt phòng
class BookingValidator {
    constructor() {
        this.init();
    }

    init() {
        this.setupServiceSelection();
    }

    // Chỉ giữ dropdown cho service selection
    setupServiceSelection() {
        // Add event listeners for service category toggles
        document.querySelectorAll('.toggle-service').forEach(button => {
            button.addEventListener('click', (e) => {
                const targetId = e.currentTarget.dataset.target;
                const optionsDiv = document.getElementById(targetId);
                const icon = e.currentTarget.querySelector('i');
                
                // Toggle the active class and show/hide options
                button.classList.toggle('active');
                optionsDiv.classList.toggle('show');
                
                // Rotate the chevron icon
                if (button.classList.contains('active')) {
                    icon.style.transform = 'rotate(180deg)';
                } else {
                    icon.style.transform = 'rotate(0deg)';
                }
            });
        });

        // Add event listeners for service checkboxes to update total
        document.querySelectorAll('input[name="services"]').forEach(checkbox => {
            checkbox.addEventListener('change', this.updateServiceTotal.bind(this));
        });
    }


}

// Khởi tạo khi DOM loaded
document.addEventListener('DOMContentLoaded', () => {
    new BookingValidator();
    setupDateValidation();
    setupSummaryAutoUpdate();
});

// ======== DATE VALIDATION ========
function setupDateValidation() {
    const checkinDate = document.getElementById('checkin-date');
    const checkoutDate = document.getElementById('checkout-date');
    
    if (!checkinDate || !checkoutDate) return;
    
    // Khi chọn ngày nhận, cập nhật min cho ngày trả và disable ngày trả cho đến khi chọn ngày nhận
    checkinDate.addEventListener('change', function() {
        // Enable ngày trả
        checkoutDate.disabled = false;
        
        const selectedCheckin = new Date(this.value);
        const nextDay = new Date(selectedCheckin);
        nextDay.setDate(nextDay.getDate() + 1);
        
        const minCheckout = nextDay.toISOString().split('T')[0];
        checkoutDate.setAttribute('min', minCheckout);
        
        // Nếu ngày trả đã chọn nhỏ hơn ngày nhận + 1, reset ngày trả
        if (checkoutDate.value && checkoutDate.value <= this.value) {
            checkoutDate.value = '';
        }
        
        // Cập nhật summary
        updateSummaryDisplay();
    });
    
    // Cập nhật summary khi chọn ngày trả
    checkoutDate.addEventListener('change', function() {
        updateSummaryDisplay();
    });
    
    // Disable ngày trả ban đầu nếu chưa chọn ngày nhận
    if (!checkinDate.value) {
        checkoutDate.disabled = true;
        checkoutDate.title = "Vui lòng chọn ngày nhận phòng trước";
    }
}

// ======== AUTO UPDATE SUMMARY ========
function setupSummaryAutoUpdate() {
    // Cập nhật summary ban đầu
    updateSummaryDisplay();
}

function updateSummaryDisplay() {
    const checkinInput = document.getElementById('checkin-date');
    const checkoutInput = document.getElementById('checkout-date');
    const summaryCheckin = document.getElementById('summary-checkin');
    const summaryCheckout = document.getElementById('summary-checkout');
    const summaryNights = document.getElementById('summary-nights');
    const summaryNightsDetail = document.getElementById('summary-nights-detail');
    const summaryRoomPrice = document.getElementById('summary-room-price');
    const summaryTotal = document.getElementById('summary-total');
    
    if (!checkinInput || !checkoutInput) return;
    
    const checkinDate = checkinInput.value;
    const checkoutDate = checkoutInput.value;
    
    if (checkinDate && checkoutDate) {
        // Format ngày theo DD/MM/YYYY
        const checkin = new Date(checkinDate);
        const checkout = new Date(checkoutDate);
        
        if (summaryCheckin) {
            summaryCheckin.textContent = checkin.toLocaleDateString('vi-VN');
        }
        if (summaryCheckout) {
            summaryCheckout.textContent = checkout.toLocaleDateString('vi-VN');
        }
        
        // Tính số đêm
        const nights = Math.ceil((checkout - checkin) / (1000 * 60 * 60 * 24));
        
        // Lấy giá phòng từ hidden input hoặc data attribute
        const giaPhongInput = document.querySelector('input[name="GiaPhong"]');
        const giaPhong = giaPhongInput ? parseFloat(giaPhongInput.value) : 0;
        
        // Cập nhật số đêm ở 2 chỗ
        if (summaryNights) {
            summaryNights.textContent = nights;
        }
        
        // Cập nhật chi tiết (8 x 300.000đ)
        if (summaryNightsDetail) {
            summaryNightsDetail.textContent = nights + ' x ' + giaPhong.toLocaleString('vi-VN') + 'đ';
        }
        
        // Tính tổng tiền (không có thuế phí)
        const total = giaPhong * nights;
        
        if (summaryRoomPrice) {
            summaryRoomPrice.textContent = total.toLocaleString('vi-VN') + 'đ';
        }
        if (summaryTotal) {
            summaryTotal.textContent = total.toLocaleString('vi-VN') + 'đ';
        }
    }
}