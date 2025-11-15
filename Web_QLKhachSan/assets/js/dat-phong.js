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
        // Add event listeners for service category toggles - click vào toàn bộ title
        document.querySelectorAll('.category-title').forEach(title => {
            title.addEventListener('click', (e) => {
                // Tìm button toggle trong title này
                const button = title.querySelector('.toggle-service');
                if (!button) return;
                
                const targetId = button.dataset.target;
                const optionsDiv = document.getElementById(targetId);
                const icon = button.querySelector('i');
                
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

    updateServiceTotal() {
        const selectedServices = document.querySelectorAll('input[name="services"]:checked');
        const selectedServicesSection = document.getElementById('selected-services-section');
        const selectedServicesList = document.getElementById('selected-services-list');
        const servicesTotal = document.getElementById('services-total');
        const summaryTotal = document.getElementById('summary-total');
        
        // Clear existing services được tạo từ JavaScript (không xóa server services)
        if (selectedServicesList) {
            const jsCreatedServices = selectedServicesList.querySelectorAll('.js-created-service');
            jsCreatedServices.forEach(service => service.remove());
        }
        
        let totalServicePrice = 0;
        
        // Tính tổng từ server services trước (từ session/database)
        const serverServices = document.querySelectorAll('.service-item:not(.js-created-service)');
        serverServices.forEach(serviceItem => {
            const priceText = serviceItem.querySelector('span:last-child').textContent;
            const price = parseFloat(priceText.replace(/[đ,\.]/g, '')) || 0;
            totalServicePrice += price;
        });
        
        // Add each selected service từ checkbox
        selectedServices.forEach(service => {
            const price = parseFloat(service.dataset.price) || 0;
            const name = service.dataset.name || 'Dịch vụ';
            
            // Chỉ thêm vào total nếu không có trong server services
            const existsInServer = Array.from(serverServices).some(serverItem => 
                serverItem.getAttribute('data-service-id') === service.value
            );
            
            if (!existsInServer) {
                totalServicePrice += price;
                
                // Add service to list
                if (selectedServicesList) {
                    const serviceElement = document.createElement('div');
                    serviceElement.className = 'summary-line service-item js-created-service';
                    serviceElement.setAttribute('data-service-id', service.value);
                    serviceElement.style.fontSize = '14px';
                    serviceElement.style.color = '#666';
                    serviceElement.style.paddingLeft = '10px';
                    serviceElement.innerHTML = `
                        <span>${name}</span>
                        <span>${price.toLocaleString('vi-VN')}đ</span>
                    `;
                    selectedServicesList.appendChild(serviceElement);
                }
            }
        });
        
        // Show/hide services section
        if (selectedServicesSection) {
            if (selectedServices.length > 0 || serverServices.length > 0) {
                selectedServicesSection.style.display = 'block';
            } else {
                selectedServicesSection.style.display = 'none';
            }
        }
        
        // Update services total
        if (servicesTotal) {
            servicesTotal.textContent = totalServicePrice.toLocaleString('vi-VN') + 'đ';
        }
        
        // Update grand total
        this.updateGrandTotal(totalServicePrice);
    }
    
    updateGrandTotal(servicePrice = 0) {
        const summaryRoomPrice = document.getElementById('summary-room-price');
        const summaryTotal = document.getElementById('summary-total');
        
        if (summaryRoomPrice && summaryTotal) {
            // Lấy giá phòng từ text content (loại bỏ 'đ', dấu phẩy và dấu chấm)
            const roomPriceText = summaryRoomPrice.textContent.replace(/[đ,\.]/g, '');
            const roomPrice = parseFloat(roomPriceText) || 0;
            
            const grandTotal = roomPrice + servicePrice;
            summaryTotal.textContent = grandTotal.toLocaleString('vi-VN') + 'đ';
        }
    }

}

// Khởi tạo khi DOM loaded
document.addEventListener('DOMContentLoaded', () => {
    const bookingValidator = new BookingValidator();
    setupDateValidation();
    setupSummaryAutoUpdate();
    
    // Đánh dấu các dịch vụ đã chọn từ server (nếu có) - thực hiện trước
    markPreSelectedServices();
    
    // Cập nhật total sau khi đã sync checkbox với server
    setTimeout(() => {
        if (typeof bookingValidator.updateServiceTotal === 'function') {
            bookingValidator.updateServiceTotal();
        }
    }, 50);
});

// Function để đánh dấu các dịch vụ đã chọn từ server
function markPreSelectedServices() {
    // Lấy danh sách dịch vụ đã có trong summary (từ server)
    const serverServices = document.querySelectorAll('.service-item:not(.js-created-service)');
    
    if (serverServices.length === 0) {
        return; // Không có dịch vụ nào được chọn từ trước
    }
    
    // Tạo danh sách ID dịch vụ từ server
    const serverServiceIds = [];
    serverServices.forEach(serviceItem => {
        const serviceId = serviceItem.getAttribute('data-service-id');
        if (serviceId) {
            serverServiceIds.push(serviceId);
        }
    });
    
    // Đồng bộ checkbox với server data
    const allCheckboxes = document.querySelectorAll('input[name="services"]');
    allCheckboxes.forEach(checkbox => {
        const shouldBeChecked = serverServiceIds.includes(checkbox.value);
        
        // Chỉ update nếu state khác nhau để tránh trigger event không cần thiết
        if (checkbox.checked !== shouldBeChecked) {
            checkbox.checked = shouldBeChecked;
            // Trigger change event để update UI nhưng không bị loop
            setTimeout(() => {
                checkbox.dispatchEvent(new Event('change', { bubbles: true }));
            }, 10);
        }
    });
}

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
// Payment method toggle
function setupPaymentToggle() {
    const instantRadio = document.getElementById('instant-transfer');
    const delayedRadio = document.getElementById('delayed-transfer');
    const qrPayment = document.getElementById('qr-payment');
    const delayedPayment = document.getElementById('delayed-payment');
    const paymentOptions = document.querySelectorAll('.payment-option');
    
    function togglePaymentMethod() {
        // Remove selected class from all options
        paymentOptions.forEach(option => option.classList.remove('selected'));
        
        if (instantRadio.checked) {
            qrPayment.style.display = 'block';
            delayedPayment.style.display = 'none';
            instantRadio.closest('.payment-option').classList.add('selected');
        } else {
            qrPayment.style.display = 'none';
            delayedPayment.style.display = 'block';
            delayedRadio.closest('.payment-option').classList.add('selected');
        }
    }
    
    instantRadio.addEventListener('change', togglePaymentMethod);
    delayedRadio.addEventListener('change', togglePaymentMethod);
    
    // Set initial state
    togglePaymentMethod();
}

// Delay time selection - UI only
function setupDelaySelection() {
    const customRadio = document.getElementById('delay-custom');
    const customDatetime = document.getElementById('custom-datetime');
    const delayOptions = document.querySelectorAll('input[name="delay-time"]');
    
    if (!customRadio || !customDatetime) return;
    
    // Show/hide custom datetime section
    delayOptions.forEach(option => {
        option.addEventListener('change', function() {
            if (this.value === 'custom' && this.checked) {
                customDatetime.style.display = 'block';
            } else {
                customDatetime.style.display = 'none';
            }
        });
    });
    
    // Set default values only
    const customDate = document.getElementById('custom-date');
    const customTime = document.getElementById('custom-time');
    
    if (customDate && customTime) {
        // Set default to tomorrow 12:00
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        customDate.value = tomorrow.toISOString().split('T')[0];
    }
}

// Confirm payment - removed (form handles submission)
function confirmPayment() {
    // Logic đã chuyển sang controller, form tự submit
}

// Initialize
document.addEventListener('DOMContentLoaded', function() {
    setupPaymentToggle();
    setupDelaySelection();
});
