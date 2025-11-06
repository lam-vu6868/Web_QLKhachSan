// Hệ thống validation thông minh cho đặt phòng
class BookingValidator {
    constructor() {
        this.errors = {};
        this.init();
    }

    init() {
        this.setupDateValidation();
        this.setupFormValidation();
        this.setupServiceSelection();
        this.setupPaymentValidation();
        this.setupConfirmationLogic();
        
        // Set ngày trả phòng mặc định khi khởi tạo
        setTimeout(() => {
            this.updateCheckoutDate();
        }, 100);
    }

    // Validation cho ngày tháng
    setupDateValidation() {
        const checkinDateInput = document.getElementById('checkin-date');
        const checkoutDateInput = document.getElementById('checkout-date');
        
        if (checkinDateInput) {
            // Set ngày nhận phòng mặc định là hôm nay
            const today = new Date().toISOString().split('T')[0];
            checkinDateInput.value = today;
            checkinDateInput.min = today;

            checkinDateInput.addEventListener('change', (e) => {
                this.validateCheckinDate(e.target);
                this.updateCheckoutDate();
            });

            // Validation real-time khi nhập liệu
            checkinDateInput.addEventListener('input', (e) => {
                this.validateDateInput(e.target.value, e.target);
            });

            // Xử lý xóa ngày theo thứ tự
            checkinDateInput.addEventListener('keydown', (e) => {
                this.handleDateDeletion(e, checkinDateInput);
            });
        }

        if (checkoutDateInput) {
            checkoutDateInput.addEventListener('change', (e) => {
                this.validateCheckoutDate(e.target);
                this.handleCheckoutDateChange(e.target);
            });

            // Cho phép nhập tay nhưng xử lý đặc biệt
            checkoutDateInput.addEventListener('input', (e) => {
                this.handleCheckoutDateInput(e.target);
            });

            // Ngăn nhập ký tự không phải số
            checkoutDateInput.addEventListener('keypress', (e) => {
                // Chỉ cho phép số, Backspace, Delete, Tab, Enter, Escape
                if (!/[0-9]/.test(e.key) && ![8, 9, 13, 27, 46].includes(e.keyCode)) {
                    e.preventDefault();
                }
            });

            // Ngăn chỉnh sửa tháng/năm sau khi đã format
            checkoutDateInput.addEventListener('keydown', (e) => {
                if (e.target.getAttribute('data-formatted') === 'true') {
                    // Nếu đã format, chỉ cho phép xóa toàn bộ hoặc các phím điều hướng
                    if (![8, 9, 13, 27, 46].includes(e.keyCode) && e.key !== 'Backspace' && e.key !== 'Delete') {
                        e.preventDefault();
                    }
                }
            });

            // Xử lý xóa ngày theo thứ tự
            checkoutDateInput.addEventListener('keydown', (e) => {
                this.handleDateDeletion(e, checkoutDateInput);
            });
        }
    }

    validateCheckinDate(input) {
        const value = input.value;
        
        // Kiểm tra format ngày tháng năm
        if (!this.validateDateInput(value, input)) {
            return false;
        }
        
        const selectedDate = new Date(value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (selectedDate < today) {
            this.showError(input, 'Ngày nhận phòng không được là ngày quá khứ');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    validateCheckoutDate(input) {
        const value = input.value;
        
        // Kiểm tra nếu trống
        if (!value) {
            this.clearError(input);
            return true;
        }
        
        // Parse ngày từ định dạng DD/MM/YYYY
        const dateMatch = value.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (!dateMatch) {
            // Không hiển thị lỗi định dạng, chỉ clear error
            this.clearError(input);
            return true;
        }
        
        const day = parseInt(dateMatch[1]);
        const month = parseInt(dateMatch[2]);
        const year = parseInt(dateMatch[3]);
        
        // Kiểm tra giới hạn số liệu
        if (year > 9999) {
            this.showError(input, 'Năm không được quá 4 chữ số');
            return false;
        }
        
        if (month > 12) {
            this.showError(input, 'Tháng không được quá 12');
            return false;
        }
        
        if (day > 31) {
            this.showError(input, 'Ngày không được quá 31');
            return false;
        }
        
        // Kiểm tra ngày tháng hợp lệ
        const checkoutDate = new Date(year, month - 1, day);
        if (checkoutDate.getFullYear() !== year || checkoutDate.getMonth() !== month - 1 || checkoutDate.getDate() !== day) {
            this.showError(input, 'Ngày tháng không tồn tại');
            return false;
        }
        
        const checkinDate = document.getElementById('checkin-date');
        if (!checkinDate || !checkinDate.value) {
            this.showError(input, 'Vui lòng chọn ngày nhận phòng trước');
            return false;
        }

        const checkin = new Date(checkinDate.value);

        if (checkoutDate <= checkin) {
            this.showError(input, 'Ngày trả phòng phải sau ngày nhận phòng ít nhất 1 ngày');
            return false;
        } else {
            // Kiểm tra không quá 1 tuần (7 ngày)
            const diffTime = checkoutDate - checkin;
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            
            if (diffDays > 7) {
                // Kiểm tra đặc biệt nếu ngày là 31
                if (day === 31) {
                    this.showError(input, 'Ngày 31 không nằm trong phạm vi 1 tuần so với ngày nhận phòng');
                } else {
                    this.showError(input, 'Ngày trả phòng không được quá 1 tuần so với ngày nhận phòng');
                }
                return false;
            } else {
                this.clearError(input);
                return true;
            }
        }
    }

    validateDateInput(value, input = null) {
        const targetInput = input || document.activeElement;
        
        if (!value) {
            this.clearError(targetInput);
            return true;
        }
        
        // Kiểm tra format YYYY-MM-DD
        const dateRegex = /^(\d{4})-(\d{2})-(\d{2})$/;
        const match = value.match(dateRegex);
        
        if (!match) {
            this.showError(targetInput, 'Định dạng ngày không hợp lệ (YYYY-MM-DD)');
            return false;
        }
        
        const year = parseInt(match[1]);
        const month = parseInt(match[2]);
        const day = parseInt(match[3]);
        
        // Kiểm tra giới hạn số liệu
        if (year > 9999) {
            this.showError(targetInput, 'Năm không được quá 4 chữ số');
            return false;
        }
        
        if (month > 12) {
            this.showError(targetInput, 'Tháng không được quá 12');
            return false;
        }
        
        // Xử lý ngày > 31: tự động điều chỉnh về 31
        let adjustedDay = day;
        if (day > 31) {
            adjustedDay = 31;
            // Cập nhật giá trị input nếu cần
            if (targetInput) {
                const adjustedValue = `${year}-${String(month).padStart(2, '0')}-${String(adjustedDay).padStart(2, '0')}`;
                targetInput.value = adjustedValue;
            }
        }
        
        // Kiểm tra ngày tháng hợp lệ với ngày đã điều chỉnh
        const date = new Date(year, month - 1, adjustedDay);
        if (date.getFullYear() !== year || date.getMonth() !== month - 1 || date.getDate() !== adjustedDay) {
            this.showError(targetInput, 'Ngày tháng không tồn tại');
            return false;
        }
        
        // Kiểm tra đặc biệt cho ngày trả phòng: nếu ngày 31 không nằm trong phạm vi 1 tuần
        if (targetInput && targetInput.id === 'checkout-date' && adjustedDay === 31) {
            const checkinDate = document.getElementById('checkin-date');
            if (checkinDate && checkinDate.value) {
                const checkin = new Date(checkinDate.value);
                const checkout = new Date(year, month - 1, adjustedDay);
                const diffTime = checkout - checkin;
                const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
                
                if (diffDays > 7) {
                    this.showError(targetInput, 'Ngày 31 không nằm trong phạm vi 1 tuần so với ngày nhận phòng');
                    return false;
                }
            }
        }
        
        this.clearError(targetInput);
        return true;
    }

    updateCheckoutDate() {
        const checkinDate = document.getElementById('checkin-date');
        const checkoutDate = document.getElementById('checkout-date');
        
        if (checkinDate && checkoutDate && checkinDate.value) {
            const checkin = new Date(checkinDate.value);
            
            // Set ngày trả phòng mặc định (ngày nhận phòng + 1 ngày)
            const defaultCheckoutDate = new Date(checkin);
            defaultCheckoutDate.setDate(defaultCheckoutDate.getDate() + 1);
            
            // Format ngày theo định dạng DD/MM/YYYY
            const day = String(defaultCheckoutDate.getDate()).padStart(2, '0');
            const month = String(defaultCheckoutDate.getMonth() + 1).padStart(2, '0');
            const year = defaultCheckoutDate.getFullYear();
            checkoutDate.value = `${day}/${month}/${year}`;
            
            // Đánh dấu là đã format xong
            checkoutDate.setAttribute('data-formatted', 'true');
        }
    }

    handleCheckoutDateInput(input) {
        const checkinDate = document.getElementById('checkin-date');
        if (!checkinDate || !checkinDate.value) return;
        
        const checkin = new Date(checkinDate.value);
        const value = input.value;
        
        // Chỉ cho phép nhập số (ngày)
        const numericValue = value.replace(/[^0-9]/g, '');
        
        // Nếu người dùng nhập 2 chữ số, tự động format thành DD/MM/YYYY
        if (/^\d{2}$/.test(numericValue)) {
            let day = parseInt(numericValue);
            
            // Nếu ngày > 31, tự động điều chỉnh về 31
            if (day > 31) {
                day = 31;
            }
            
            if (day >= 1) {
                // Tự động điều chỉnh về cùng tháng/năm với ngày nhận phòng
                const adjustedCheckout = new Date(checkin);
                adjustedCheckout.setDate(day);
                
                // Kiểm tra ngày có hợp lệ không
                if (adjustedCheckout.getDate() === day) {
                    // Format theo DD/MM/YYYY
                    const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                    const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                    const yearStr = adjustedCheckout.getFullYear();
                    input.value = `${dayStr}/${monthStr}/${yearStr}`;
                    
                    // Đánh dấu là đã format xong, không cho chỉnh sửa tháng/năm
                    input.setAttribute('data-formatted', 'true');
                } else {
                    // Nếu ngày không hợp lệ, điều chỉnh về ngày cuối tháng
                    const lastDayOfMonth = new Date(checkin.getFullYear(), checkin.getMonth() + 1, 0).getDate();
                    const finalDay = Math.min(day, lastDayOfMonth);
                    adjustedCheckout.setDate(finalDay);
                    
                    const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                    const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                    const yearStr = adjustedCheckout.getFullYear();
                    input.value = `${dayStr}/${monthStr}/${yearStr}`;
                    
                    // Đánh dấu là đã format xong
                    input.setAttribute('data-formatted', 'true');
                }
            }
        }
        // Nếu người dùng nhập 1 chữ số, hiển thị tạm thời
        else if (/^\d{1}$/.test(numericValue)) {
            let day = parseInt(numericValue);
            
            // Nếu ngày > 31, tự động điều chỉnh về 31
            if (day > 31) {
                day = 31;
            }
            
            // Chỉ hiển thị số đã nhập, chưa format
            input.value = day.toString();
        }
        // Nếu người dùng nhập 3 số trở lên, chỉ lấy 2 số đầu
        else if (/^\d{3,}$/.test(numericValue)) {
            let day = parseInt(numericValue.substring(0, 2));
            
            // Nếu ngày > 31, tự động điều chỉnh về 31
            if (day > 31) {
                day = 31;
            }
            
            // Tự động format thành DD/MM/YYYY
            const adjustedCheckout = new Date(checkin);
            adjustedCheckout.setDate(day);
            
            // Kiểm tra ngày có hợp lệ không
            if (adjustedCheckout.getDate() === day) {
                // Format theo DD/MM/YYYY
                const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                const yearStr = adjustedCheckout.getFullYear();
                input.value = `${dayStr}/${monthStr}/${yearStr}`;
                
                // Đánh dấu là đã format xong
                input.setAttribute('data-formatted', 'true');
            } else {
                // Nếu ngày không hợp lệ, điều chỉnh về ngày cuối tháng
                const lastDayOfMonth = new Date(checkin.getFullYear(), checkin.getMonth() + 1, 0).getDate();
                const finalDay = Math.min(day, lastDayOfMonth);
                adjustedCheckout.setDate(finalDay);
                
                const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                const yearStr = adjustedCheckout.getFullYear();
                input.value = `${dayStr}/${monthStr}/${yearStr}`;
                
                // Đánh dấu là đã format xong
                input.setAttribute('data-formatted', 'true');
            }
        }
        // Nếu người dùng xóa hết, hiển thị placeholder
        else if (numericValue === '') {
            input.placeholder = 'Nhập ngày trả phòng';
            input.removeAttribute('data-formatted');
        }
    }

    handleCheckoutDateChange(input) {
        const checkinDate = document.getElementById('checkin-date');
        if (!checkinDate || !checkinDate.value) return;
        
        const checkin = new Date(checkinDate.value);
        const value = input.value;
        
        // Parse ngày từ định dạng DD/MM/YYYY
        const dateMatch = value.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (dateMatch) {
            const day = parseInt(dateMatch[1]);
            const month = parseInt(dateMatch[2]);
            const year = parseInt(dateMatch[3]);
            
            const checkout = new Date(year, month - 1, day);
            
            // Kiểm tra nếu người dùng thay đổi tháng hoặc năm
            if (checkout.getMonth() !== checkin.getMonth() || checkout.getFullYear() !== checkin.getFullYear()) {
                // Tự động điều chỉnh về cùng tháng/năm với ngày nhận phòng
                const adjustedCheckout = new Date(checkin);
                adjustedCheckout.setDate(day); // Chỉ giữ lại ngày
                
                // Kiểm tra ngày có hợp lệ không
                if (adjustedCheckout.getDate() === day) {
                    const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                    const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                    const yearStr = adjustedCheckout.getFullYear();
                    input.value = `${dayStr}/${monthStr}/${yearStr}`;
                } else {
                    // Nếu ngày không hợp lệ (ví dụ: 31/02), điều chỉnh về ngày cuối tháng
                    const lastDayOfMonth = new Date(checkin.getFullYear(), checkin.getMonth() + 1, 0).getDate();
                    const finalDay = Math.min(day, lastDayOfMonth);
                    adjustedCheckout.setDate(finalDay);
                    
                    const dayStr = String(adjustedCheckout.getDate()).padStart(2, '0');
                    const monthStr = String(adjustedCheckout.getMonth() + 1).padStart(2, '0');
                    const yearStr = adjustedCheckout.getFullYear();
                    input.value = `${dayStr}/${monthStr}/${yearStr}`;
                }
            }
        }
    }

    handleDateDeletion(e, input) {
        // Chỉ xử lý khi nhấn phím Backspace hoặc Delete
        if (e.key !== 'Backspace' && e.key !== 'Delete') return;
        
        // Nếu đã format, chỉ cho phép xóa toàn bộ
        if (input.id === 'checkout-date' && input.getAttribute('data-formatted') === 'true') {
            e.preventDefault();
            input.value = '';
            input.placeholder = 'Nhập ngày trả phòng';
            input.removeAttribute('data-formatted');
            return;
        }
        
        // Ngăn xóa mặc định
        e.preventDefault();
        
        const value = input.value;
        const cursorPosition = input.selectionStart;
        
        // Nếu đã trống, không làm gì
        if (!value) return;
        
        // Xóa theo thứ tự: năm -> tháng -> ngày cho định dạng DD/MM/YYYY
        if (input.id === 'checkout-date') {
            // Xóa theo thứ tự: năm -> tháng -> ngày
            if (value.length >= 4) {
                // Xóa năm (4 ký tự cuối)
                input.value = value.substring(0, value.length - 4);
            } else if (value.length >= 2) {
                // Xóa tháng (2 ký tự cuối)
                input.value = value.substring(0, value.length - 2);
            } else {
                // Xóa ngày (1 ký tự cuối)
                input.value = value.substring(0, value.length - 1);
            }
        } else {
            // Xóa theo thứ tự: năm -> tháng -> ngày cho định dạng YYYY-MM-DD
            if (value.length >= 4) {
                // Xóa năm (4 ký tự cuối)
                input.value = value.substring(0, value.length - 4);
            } else if (value.length >= 2) {
                // Xóa tháng (2 ký tự cuối)
                input.value = value.substring(0, value.length - 2);
            } else {
                // Xóa ngày (1 ký tự cuối)
                input.value = value.substring(0, value.length - 1);
            }
        }
        
        // Nếu xóa hết ngày trả phòng, hiển thị placeholder
        if (input.id === 'checkout-date' && input.value === '') {
            input.placeholder = 'Nhập ngày trả phòng';
        }
        
        // Trigger change event để validation
        input.dispatchEvent(new Event('change'));
        
        // Đặt cursor ở cuối
        input.setSelectionRange(input.value.length, input.value.length);
    }

    // Validation cho form thông tin khách hàng
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

    updateServiceTotal() {
        const selectedServices = document.querySelectorAll('input[name="services"]:checked');
        let totalServices = 0;

        selectedServices.forEach(service => {
            // Tìm giá trong cấu trúc mới với .original-price hoặc .current-price
            const serviceOption = service.closest('.service-option');
            let priceText = '';
            
            // Thử tìm giá gốc (original-price) trước
            const originalPrice = serviceOption.querySelector('.original-price');
            if (originalPrice) {
                priceText = originalPrice.textContent;
            } else {
                // Nếu không có giá gốc, tìm giá hiện tại
                const currentPrice = serviceOption.querySelector('.current-price');
                if (currentPrice) {
                    priceText = currentPrice.textContent;
                } else {
                    // Fallback về .service-price cũ nếu có
                    const servicePrice = serviceOption.querySelector('.service-price');
                    if (servicePrice) {
                        priceText = servicePrice.textContent;
                    }
                }
            }
            
            const price = parseInt(priceText.replace(/[^0-9]/g, ''));
            if (!isNaN(price)) {
                totalServices += price;
            }
        });

        // Update the order summary with selected services
        const orderSummary = document.getElementById('order-summary');
        if (orderSummary) {
            const servicesList = Array.from(selectedServices).map(service => {
                const serviceOption = service.closest('.service-option');
                let serviceName = '';
                let servicePrice = '';
                
                // Tìm tên dịch vụ
                const nameElement = serviceOption.querySelector('.service-name') || 
                                  serviceOption.querySelector('h4') ||
                                  serviceOption.querySelector('h3');
                if (nameElement) {
                    serviceName = nameElement.textContent.trim();
                }
                
                // Tìm giá hiển thị
                const originalPrice = serviceOption.querySelector('.original-price');
                const currentPrice = serviceOption.querySelector('.current-price');
                const fallbackPrice = serviceOption.querySelector('.service-price');
                
                if (originalPrice) {
                    servicePrice = originalPrice.textContent.trim();
                } else if (currentPrice) {
                    servicePrice = currentPrice.textContent.trim();
                } else if (fallbackPrice) {
                    servicePrice = fallbackPrice.textContent.trim();
                }
                
                return `<li>${serviceName} - ${servicePrice}</li>`;
            }).join('');

            const servicesHTML = selectedServices.length > 0 ? `
                <div class="summary-section">
                    <h3>Dịch vụ đã chọn</h3>
                    <ul class="services-list">
                        ${servicesList}
                    </ul>
                    <div class="summary-total">
                        <span>Tổng phụ thu dịch vụ:</span>
                        <span class="amount">${totalServices.toLocaleString('vi-VN')}đ</span>
                    </div>
                </div>
            ` : '';

            // Update the existing summary or create new content
            if (orderSummary.innerHTML.includes('summary-section')) {
                const existingContent = orderSummary.innerHTML;
                const updatedContent = existingContent.replace(
                    /<div class="summary-section">([\s\S]*?)<\/div>/,
                    servicesHTML
                );
                orderSummary.innerHTML = updatedContent;
            } else {
                orderSummary.innerHTML += servicesHTML;
            }
        }
    }

    setupFormValidation() {
        const form = document.querySelector('.checkout-form');
        if (!form) return;

        // Validation cho họ tên
        const firstName = document.getElementById('first-name');
        const lastName = document.getElementById('last-name');
        
        if (firstName) {
            firstName.addEventListener('blur', () => this.validateName(firstName, 'Họ'));
        }
        if (lastName) {
            lastName.addEventListener('blur', () => this.validateName(lastName, 'Tên'));
        }

        // Validation cho email
        const email = document.getElementById('email');
        if (email) {
            email.addEventListener('blur', () => this.validateEmail(email));
        }

        // Validation cho số điện thoại
        const phone = document.getElementById('phone');
        if (phone) {
            phone.addEventListener('blur', () => this.validatePhone(phone));
        }

        // Validation cho số người
        const guests = document.getElementById('guests');
        if (guests) {
            guests.addEventListener('change', () => this.validateGuests(guests));
        }

        // Xử lý submit form
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            if (this.validateAllFields()) {
                this.proceedToNextStep();
            }
        });
    }

    validateName(input, fieldName) {
        const value = input.value.trim();
        if (!value) {
            this.showError(input, `${fieldName} không được để trống`);
            return false;
        } else if (value.length < 2) {
            this.showError(input, `${fieldName} phải có ít nhất 2 ký tự`);
            return false;
        } else {
            const hasNumber = /\d/.test(value);
            // Regex cải tiến để nhận diện Unicode tiếng Việt đầy đủ
            const hasSpecialChar = /[^a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂÂĐÊÔÔƠƯăâđêôôơư\u00C0-\u1EF9\s]/.test(value);
            
            if (hasNumber && hasSpecialChar) {
                this.showError(input, `${fieldName} không được chứa số và ký tự đặc biệt`);
                return false;
            } else if (hasNumber) {
                this.showError(input, `${fieldName} không được chứa số`);
                return false;
            } else if (hasSpecialChar) {
                this.showError(input, `${fieldName} không được chứa ký tự đặc biệt`);
                return false;
            } else {
                this.clearError(input);
                return true;
            }
        }
    }

    validateEmail(input) {
        const value = input.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (!value) {
            this.showError(input, 'Email không được để trống');
            return false;
        } else if (!emailRegex.test(value)) {
            this.showError(input, 'Email không đúng định dạng');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    validatePhone(input) {
        const value = input.value.trim();
        // Regex cho số điện thoại Việt Nam: 10-11 chữ số, bắt đầu bằng 0
        const phoneRegex = /^0[0-9]{9,10}$/;
        
        if (!value) {
            this.showError(input, 'Số điện thoại không được để trống');
            return false;
        } else if (!phoneRegex.test(value)) {
            this.showError(input, 'Số điện thoại Việt Nam phải có 10-11 chữ số và bắt đầu bằng 0');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    validateGuests(input) {
        const value = parseInt(input.value);
        if (value < 1 || value > 4) {
            this.showError(input, 'Số người phải từ 1 đến 4');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    // Validation cho thanh toán
    setupPaymentValidation() {
        const paymentForm = document.querySelector('.checkout-form');
        if (!paymentForm) return;

        // Xử lý chuyển đổi phương thức thanh toán
        this.setupPaymentMethodToggle();

        // Validation cho thẻ tín dụng
        const cardNumber = document.getElementById('card-number');
        const expiryDate = document.getElementById('expiry-date');
        const cvv = document.getElementById('cvv');

        if (cardNumber) {
            cardNumber.addEventListener('input', (e) => {
                this.formatCardNumber(e.target);
                this.validateCardNumber(e.target);
            });
        }

        if (expiryDate) {
            expiryDate.addEventListener('input', (e) => {
                this.formatExpiryDate(e.target);
                this.validateExpiryDate(e.target);
            });
        }

        if (cvv) {
            cvv.addEventListener('input', (e) => {
                this.validateCVV(e.target);
            });
        }

        // Xử lý submit thanh toán
        const paymentSubmit = document.getElementById('payment-submit');
        if (paymentSubmit) {
            paymentSubmit.addEventListener('click', (e) => {
                e.preventDefault();
                if (this.validatePayment()) {
                    this.processPayment();
                }
            });
        }
    }

    setupPaymentMethodToggle() {
        const paymentMethods = document.querySelectorAll('input[name="payment"]');
        const cardDetails = document.getElementById('card-details');
        const bankDetails = document.getElementById('bank-details');

        paymentMethods.forEach(method => {
            method.addEventListener('change', (e) => {
                // Xóa thông báo lỗi phương thức thanh toán
                this.clearPaymentMethodError();
                
                // Ẩn tất cả các phần chi tiết
                if (cardDetails) cardDetails.style.display = 'none';
                if (bankDetails) bankDetails.style.display = 'none';

                // Hiển thị phần chi tiết tương ứng
                switch (e.target.value) {
                    case 'credit-card':
                        if (cardDetails) cardDetails.style.display = 'block';
                        break;
                    case 'bank-transfer':
                        if (bankDetails) bankDetails.style.display = 'block';
                        this.updateTransferContent();
                        this.setupBankSelection();
                        break;
                }
            });
        });
    }

    setupBankSelection() {
        const bankSelect = document.getElementById('bank-select');
        const selectedBank = document.getElementById('selected-bank');
        const accountNumber = document.getElementById('account-number');

        if (bankSelect) {
            bankSelect.addEventListener('change', (e) => {
                const bankData = this.getBankData(e.target.value);
                if (selectedBank) selectedBank.textContent = bankData.name;
                if (accountNumber) accountNumber.textContent = bankData.account;
            });
        }
    }

    getBankData(bankCode) {
        const banks = {
            'vietcombank': { name: 'Vietcombank', account: '1234567890' },
            'bidv': { name: 'BIDV', account: '2345678901' },
            'agribank': { name: 'Agribank', account: '3456789012' },
            'techcombank': { name: 'Techcombank', account: '4567890123' },
            'mbbank': { name: 'MB Bank', account: '5678901234' },
            'vietinbank': { name: 'VietinBank', account: '6789012345' }
        };
        return banks[bankCode] || banks['vietcombank'];
    }

    updateTransferContent() {
        const transferContent = document.getElementById('transfer-content');
        if (transferContent) {
            const bookingCode = this.generateBookingCode();
            transferContent.textContent = `Dat phong - ${bookingCode}`;
        }
    }

    formatCardNumber(input) {
        let value = input.value.replace(/\s/g, '').replace(/[^0-9]/gi, '');
        let formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;
        input.value = formattedValue;
    }

    formatExpiryDate(input) {
        let value = input.value.replace(/\D/g, '');
        if (value.length >= 2) {
            value = value.substring(0, 2) + ' / ' + value.substring(2, 4);
        }
        input.value = value;
    }

    validateCardNumber(input) {
        const value = input.value.replace(/\s/g, '');
        if (value.length === 0) {
            this.clearError(input);
            return true;
        } else if (value.length < 16) {
            this.showError(input, 'Số thẻ phải có 16 chữ số');
            return false;
        } else if (!/^[0-9]{16}$/.test(value)) {
            this.showError(input, 'Số thẻ chỉ được chứa chữ số');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    validateExpiryDate(input) {
        const value = input.value.replace(/\D/g, '');
        if (value.length === 0) {
            this.clearError(input);
            return true;
        } else if (value.length < 4) {
            this.showError(input, 'Ngày hết hạn phải có đủ MM/YY');
            return false;
        } else {
            const month = parseInt(value.substring(0, 2));
            const year = parseInt('20' + value.substring(2, 4));
            const currentDate = new Date();
            const currentYear = currentDate.getFullYear();
            const currentMonth = currentDate.getMonth() + 1;

            if (month < 1 || month > 12) {
                this.showError(input, 'Tháng phải từ 01 đến 12');
                return false;
            } else if (year < currentYear || (year === currentYear && month < currentMonth)) {
                this.showError(input, 'Thẻ đã hết hạn');
                return false;
            } else {
                this.clearError(input);
                return true;
            }
        }
    }

    validateCVV(input) {
        const value = input.value;
        if (value.length === 0) {
            this.clearError(input);
            return true;
        } else if (!/^[0-9]{3,4}$/.test(value)) {
            this.showError(input, 'CVV phải có 3-4 chữ số');
            return false;
        } else {
            this.clearError(input);
            return true;
        }
    }

    validatePayment() {
        const creditCard = document.getElementById('credit-card');
        const bankTransfer = document.getElementById('bank-transfer');
        const selectedMethod = document.querySelector('input[name="payment"]:checked');
        
        // Kiểm tra xem có chọn phương thức thanh toán không
        if (!selectedMethod) {
            this.showPaymentMethodError();
            return false;
        }
        
        if (creditCard && creditCard.checked) {
            const cardNumber = document.getElementById('card-number');
            const expiryDate = document.getElementById('expiry-date');
            const cvv = document.getElementById('cvv');
            
            let isValid = true;
            
            // Validation từng field
            if (!cardNumber || !cardNumber.value.trim()) {
                this.showError(cardNumber, 'Số thẻ không được để trống');
                isValid = false;
            } else {
                isValid &= this.validateCardNumber(cardNumber);
            }
            
            if (!expiryDate || !expiryDate.value.trim()) {
                this.showError(expiryDate, 'Ngày hết hạn không được để trống');
                isValid = false;
            } else {
                isValid &= this.validateExpiryDate(expiryDate);
            }
            
            if (!cvv || !cvv.value.trim()) {
                this.showError(cvv, 'CVV không được để trống');
                isValid = false;
            } else {
                isValid &= this.validateCVV(cvv);
            }
            
            return isValid;
        } else if (bankTransfer && bankTransfer.checked) {
            // Validation cho chuyển khoản ngân hàng
            return true;
        }
        
        return false;
    }

    showPaymentMethodError() {
        // Tạo thông báo lỗi chung cho việc chưa chọn phương thức
        const paymentMethods = document.querySelector('.payment-methods');
        if (paymentMethods) {
            // Xóa thông báo lỗi cũ nếu có
            const existingError = paymentMethods.querySelector('.payment-method-error');
            if (existingError) {
                existingError.remove();
            }
            
            // Tạo thông báo lỗi mới
            const errorDiv = document.createElement('div');
            errorDiv.className = 'payment-method-error';
            errorDiv.style.color = '#e74c3c';
            errorDiv.style.fontSize = '14px';
            errorDiv.style.marginTop = '10px';
            errorDiv.style.padding = '10px';
            errorDiv.style.backgroundColor = '#fdf2f2';
            errorDiv.style.border = '1px solid #fecaca';
            errorDiv.style.borderRadius = '8px';
            errorDiv.textContent = 'Vui lòng chọn phương thức thanh toán';
            
            paymentMethods.appendChild(errorDiv);
            
            // Tự động xóa thông báo sau 5 giây
            setTimeout(() => {
                if (errorDiv && errorDiv.parentNode) {
                    errorDiv.remove();
                }
            }, 5000);
        }
    }

    clearPaymentMethodError() {
        const paymentMethods = document.querySelector('.payment-methods');
        if (paymentMethods) {
            const existingError = paymentMethods.querySelector('.payment-method-error');
            if (existingError) {
                existingError.remove();
            }
        }
    }

    // Logic xác nhận
    setupConfirmationLogic() {
        // Kiểm tra nếu đang ở trang xác nhận
        if (window.location.pathname.includes('xac-nhan.html')) {
            this.showConfirmationResult();
            this.loadBookingDetails();
        }
    }

    async processPayment() {
        // Hiển thị loading
        this.showLoading();
        
        // Simulate payment processing
        const success = await this.simulatePayment();
        
        if (success) {
            // Lưu thông tin đặt phòng vào localStorage
            this.saveBookingData();
            // Chuyển đến trang xác nhận thành công
            window.location.href = 'xac-nhan.html?status=success';
        } else {
            // Chuyển đến trang xác nhận thất bại
            window.location.href = 'xac-nhan.html?status=failed';
        }
    }

    async simulatePayment() {
        // Simulate API call delay
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // 90% success rate for demo
        return Math.random() > 0.1;
    }

    showLoading() {
        const button = document.getElementById('payment-submit');
        if (button) {
            button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang xử lý...';
            button.classList.add('loading');
            button.disabled = true;
        }
    }

    saveBookingData() {
        const firstName = document.getElementById('first-name')?.value || '';
        const lastName = document.getElementById('last-name')?.value || '';
        const email = document.getElementById('email')?.value || '';
        const phone = document.getElementById('phone')?.value || '';
        const checkinDate = document.getElementById('checkin-date')?.value || '';
        const checkoutDate = document.getElementById('checkout-date')?.value || '';
        const guests = document.getElementById('guests')?.value || '1';
        const paymentMethod = document.querySelector('input[name="payment"]:checked')?.id || '';
        
        const bookingData = {
            firstName,
            lastName,
            email,
            phone,
            checkinDate,
            checkoutDate,
            guests,
            paymentMethod,
            timestamp: new Date().toISOString()
        };
        
        console.log('Saving booking data:', bookingData);
        console.log('Individual values:', {
            firstName, lastName, email, phone, checkinDate, checkoutDate, guests, paymentMethod
        });
        
        // Lưu thông tin thanh toán chi tiết
        const selectedPayment = document.querySelector('input[name="payment"]:checked');
        if (selectedPayment) {
            if (selectedPayment.id === 'credit-card') {
                const cardData = {
                    number: document.getElementById('card-number')?.value || '',
                    expiry: document.getElementById('expiry-date')?.value || '',
                    cvv: document.getElementById('cvv')?.value || ''
                };
                console.log('Saving card data:', cardData);
                localStorage.setItem('cardData', JSON.stringify(cardData));
            } else if (selectedPayment.id === 'bank-transfer') {
                const bankData = {
                    name: document.getElementById('bank-select')?.selectedOptions[0]?.text || 'Vietcombank',
                    account: document.getElementById('account-number')?.textContent || '1234567890'
                };
                console.log('Saving bank data:', bankData);
                localStorage.setItem('bankData', JSON.stringify(bankData));
            }
        }
        
        localStorage.setItem('bookingData', JSON.stringify(bookingData));
        
        // Verify save
        const saved = localStorage.getItem('bookingData');
        console.log('Verified saved booking data:', saved);
    }

    showConfirmationResult() {
        const urlParams = new URLSearchParams(window.location.search);
        const status = urlParams.get('status');
        
        const confirmationIcon = document.querySelector('.confirmation-icon i');
        const confirmationTitle = document.querySelector('.form-section-title');
        const confirmationMessage = document.querySelector('.confirmation-message');
        const homeButton = document.querySelector('.btn-primary');
        
        if (status === 'success') {
            // Thành công
            if (confirmationIcon) {
                confirmationIcon.className = 'fas fa-check-circle';
                confirmationIcon.style.color = '#27ae60';
            }
            if (confirmationTitle) {
                confirmationTitle.textContent = 'Đặt Phòng Thành Công!';
            }
            if (confirmationMessage) {
                confirmationMessage.innerHTML = `
                    Cảm ơn bạn đã tin tưởng lựa chọn Serene Horizon.<br>
                    Chúng tôi đã gửi xác nhận đặt phòng đến email của bạn.<br>
                    Nếu cần hỗ trợ, vui lòng liên hệ hotline <b>0123 456 789</b>.
                `;
            }
        } else if (status === 'failed') {
            // Thất bại
            if (confirmationIcon) {
                confirmationIcon.className = 'fas fa-times-circle';
                confirmationIcon.style.color = '#e74c3c';
            }
            if (confirmationTitle) {
                confirmationTitle.textContent = 'Thanh Toán Thất Bại';
            }
            if (confirmationMessage) {
                confirmationMessage.innerHTML = `
                    Rất tiếc, quá trình thanh toán không thành công.<br>
                    Vui lòng kiểm tra lại thông tin thẻ hoặc thử phương thức thanh toán khác.<br>
                    Nếu vấn đề vẫn tiếp tục, vui lòng liên hệ hotline <b>0123 456 789</b>.
                `;
            }
            if (homeButton) {
                homeButton.innerHTML = '<i class="fas fa-redo"></i> Thử Lại';
                homeButton.onclick = () => window.location.href = 'thanh-toan.html';
            }
        }
    }

    loadBookingDetails() {
        console.log('Loading booking details...');
        
        const bookingData = localStorage.getItem('bookingData');
        const formData = localStorage.getItem('formData');
        
        console.log('Booking data:', bookingData);
        console.log('Form data:', formData);
        
        let data = null;
        
        if (bookingData) {
            try {
                data = JSON.parse(bookingData);
                console.log('Parsed booking data:', data);
            } catch (e) {
                console.error('Error parsing booking data:', e);
            }
        }
        
        if (!data && formData) {
            try {
                data = JSON.parse(formData);
                console.log('Using form data:', data);
            } catch (e) {
                console.error('Error parsing form data:', e);
            }
        }
        
        if (data) {
            // Generate booking code
            const bookingCode = this.generateBookingCode();
            const bookingCodeEl = document.getElementById('booking-code');
            if (bookingCodeEl) {
                bookingCodeEl.textContent = bookingCode;
                console.log('Set booking code:', bookingCode);
            }
            
            // Fill customer details
            const customerNameEl = document.getElementById('customer-name');
            if (customerNameEl) {
                // Sửa thứ tự: lastName + firstName (như trong form)
                const fullName = `${data.lastName || ''} ${data.firstName || ''}`.trim();
                customerNameEl.textContent = fullName || '-';
                console.log('Set customer name:', fullName, 'from firstName:', data.firstName, 'lastName:', data.lastName);
            } else {
                console.error('Element customer-name not found');
            }
            
            const customerEmailEl = document.getElementById('customer-email');
            if (customerEmailEl) {
                customerEmailEl.textContent = data.email || '-';
                console.log('Set customer email:', data.email);
            } else {
                console.error('Element customer-email not found');
            }
            
            const customerPhoneEl = document.getElementById('customer-phone');
            if (customerPhoneEl) {
                customerPhoneEl.textContent = data.phone || '-';
                console.log('Set customer phone:', data.phone);
            } else {
                console.error('Element customer-phone not found');
            }
            
            const checkinDateEl = document.getElementById('checkin-date');
            if (checkinDateEl) {
                const formattedCheckin = this.formatDate(data.checkinDate);
                checkinDateEl.textContent = formattedCheckin;
                console.log('Set checkin date:', formattedCheckin, 'from:', data.checkinDate);
            } else {
                console.error('Element checkin-date not found');
            }
            
            const checkoutDateEl = document.getElementById('checkout-date');
            if (checkoutDateEl) {
                const formattedCheckout = this.formatDate(data.checkoutDate);
                checkoutDateEl.textContent = formattedCheckout;
                console.log('Set checkout date:', formattedCheckout, 'from:', data.checkoutDate);
            } else {
                console.error('Element checkout-date not found');
            }
            
            const guestsEl = document.getElementById('guests');
            if (guestsEl) {
                guestsEl.textContent = data.guests || '1';
                console.log('Set guests:', data.guests);
            } else {
                console.error('Element guests not found');
            }
            
            const paymentMethodEl = document.getElementById('payment-method');
            if (paymentMethodEl) {
                paymentMethodEl.textContent = data.paymentMethod ? this.getPaymentMethodName(data.paymentMethod) : 'Chưa xác định';
                console.log('Set payment method:', data.paymentMethod);
            } else {
                console.error('Element payment-method not found');
            }
            
            // Hiển thị thông tin thanh toán chi tiết
            if (data.paymentMethod) {
                this.showPaymentDetails(data.paymentMethod);
            }
            
            console.log('Booking details loaded successfully');
        } else {
            // Nếu không có dữ liệu, hiển thị thông báo
            console.log('No data found, showing fallback message');
            const bookingDetailsEl = document.getElementById('booking-details');
            if (bookingDetailsEl) {
                bookingDetailsEl.innerHTML = `
                    <h3>Không có thông tin đặt phòng</h3>
                    <p>Vui lòng quay lại trang thông tin khách hàng để đặt phòng.</p>
                    <a href="thong-tin-khach-hang.html" class="btn-primary">Đặt Phòng</a>
                `;
            }
        }
    }

    showPaymentDetails(paymentMethod) {
        const cardInfo = document.getElementById('card-info');
        const cardExpiry = document.getElementById('card-expiry');
        const bankInfo = document.getElementById('bank-info');
        const bankAccount = document.getElementById('bank-account');
        const cardNumber = document.getElementById('card-number');
        const cardExpiryDate = document.getElementById('card-expiry-date');
        const bankName = document.getElementById('bank-name');
        const bankAccountNumber = document.getElementById('bank-account-number');
        
        if (paymentMethod === 'credit-card') {
            // Hiển thị thông tin thẻ tín dụng
            if (cardInfo) cardInfo.style.display = 'flex';
            if (cardExpiry) cardExpiry.style.display = 'flex';
            if (bankInfo) bankInfo.style.display = 'none';
            if (bankAccount) bankAccount.style.display = 'none';
            
            // Lấy thông tin thẻ từ localStorage
            const cardData = localStorage.getItem('cardData');
            if (cardData) {
                const card = JSON.parse(cardData);
                if (cardNumber) {
                    // Hiển thị 4 số cuối của thẻ
                    const maskedNumber = card.number ? 
                        '**** **** **** ' + card.number.slice(-4) : 
                        '**** **** **** ****';
                    cardNumber.textContent = maskedNumber;
                }
                if (cardExpiryDate) {
                    cardExpiryDate.textContent = card.expiry || 'MM/YY';
                }
            } else {
                if (cardNumber) cardNumber.textContent = '**** **** **** ****';
                if (cardExpiryDate) cardExpiryDate.textContent = 'MM/YY';
            }
        } else if (paymentMethod === 'bank-transfer') {
            // Hiển thị thông tin chuyển khoản
            if (cardInfo) cardInfo.style.display = 'none';
            if (cardExpiry) cardExpiry.style.display = 'none';
            if (bankInfo) bankInfo.style.display = 'flex';
            if (bankAccount) bankAccount.style.display = 'flex';
            
            // Lấy thông tin ngân hàng từ localStorage
            const bankData = localStorage.getItem('bankData');
            if (bankData) {
                const bank = JSON.parse(bankData);
                if (bankName) bankName.textContent = bank.name || 'Vietcombank';
                if (bankAccountNumber) bankAccountNumber.textContent = bank.account || '1234567890';
            } else {
                if (bankName) bankName.textContent = 'Vietcombank';
                if (bankAccountNumber) bankAccountNumber.textContent = '1234567890';
            }
        } else {
            // Ẩn tất cả
            if (cardInfo) cardInfo.style.display = 'none';
            if (cardExpiry) cardExpiry.style.display = 'none';
            if (bankInfo) bankInfo.style.display = 'none';
            if (bankAccount) bankAccount.style.display = 'none';
        }
    }

    generateBookingCode() {
        const now = new Date();
        const year = now.getFullYear();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        const random = Math.floor(Math.random() * 1000).toString().padStart(3, '0');
        return `#SH${year}${month}${day}${random}`;
    }

    formatDate(dateString) {
        if (!dateString) return '-';
        
        console.log('formatDate input:', dateString);
        
        // Nếu đã là format DD/MM/YYYY thì trả về luôn
        if (dateString.includes('/') && dateString.length === 10) {
            console.log('formatDate output (already formatted):', dateString);
            return dateString;
        }
        
        // Nếu là format YYYY-MM-DD thì convert
        const date = new Date(dateString);
        if (isNaN(date.getTime())) {
            console.log('formatDate output (invalid date):', '-');
            return '-';
        }
        
        const formatted = date.toLocaleDateString('vi-VN');
        console.log('formatDate output (converted):', formatted);
        return formatted;
    }

    getPaymentMethodName(method) {
        const methods = {
            'credit-card': 'Thẻ Tín Dụng / Ghi Nợ',
            'bank-transfer': 'Chuyển khoản ngân hàng',
            'momo': 'Ví MoMo',
            'zalopay': 'ZaloPay'
        };
        return methods[method] || method;
    }

    // Utility functions
    showError(input, message) {
        this.clearError(input);
        
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = message;
        errorDiv.style.color = '#e74c3c';
        errorDiv.style.fontSize = '14px';
        errorDiv.style.marginTop = '5px';
        
        // Nếu là phone input, thêm error class vào container và error message dưới form-group
        if (input.id === 'phone') {
            const container = input.closest('.phone-input-container');
            const formGroup = input.closest('.form-group');
            if (container) {
                container.classList.add('error');
            }
            if (formGroup) {
                formGroup.appendChild(errorDiv);
            }
        } else {
            input.style.borderColor = '#e74c3c';
            input.parentNode.appendChild(errorDiv);
        }
        
        this.errors[input.id] = message;
    }

    clearError(input) {
        // Nếu là phone input, xóa error message từ form-group
        if (input.id === 'phone') {
            const container = input.closest('.phone-input-container');
            const formGroup = input.closest('.form-group');
            if (container) {
                container.classList.remove('error');
            }
            if (formGroup) {
                const existingError = formGroup.querySelector('.error-message');
                if (existingError) {
                    existingError.remove();
                }
            }
        } else {
            const existingError = input.parentNode.querySelector('.error-message');
            if (existingError) {
                existingError.remove();
            }
            input.style.borderColor = '';
        }
        
        delete this.errors[input.id];
    }

    validateAllFields() {
        const firstName = document.getElementById('first-name');
        const lastName = document.getElementById('last-name');
        const email = document.getElementById('email');
        const phone = document.getElementById('phone');
        const checkinDate = document.getElementById('checkin-date');
        const checkoutDate = document.getElementById('checkout-date');
        const guests = document.getElementById('guests');
        
        let isValid = true;
        
        if (firstName) isValid &= this.validateName(firstName, 'Họ');
        if (lastName) isValid &= this.validateName(lastName, 'Tên');
        if (email) isValid &= this.validateEmail(email);
        if (phone) isValid &= this.validatePhone(phone);
        if (checkinDate) isValid &= this.validateCheckinDate(checkinDate);
        if (checkoutDate) isValid &= this.validateCheckoutDate(checkoutDate);
        if (guests) isValid &= this.validateGuests(guests);
        
        return isValid;
    }

    proceedToNextStep() {
        // Lưu dữ liệu form
        this.saveFormData();
        
        // Chuyển đến trang thanh toán
        window.location.href = 'thanh-toan.html';
    }

    saveFormData() {
        const firstName = document.getElementById('first-name')?.value || '';
        const lastName = document.getElementById('last-name')?.value || '';
        const email = document.getElementById('email')?.value || '';
        const phone = document.getElementById('phone')?.value || '';
        const checkinDate = document.getElementById('checkin-date')?.value || '';
        const checkoutDate = document.getElementById('checkout-date')?.value || '';
        const guests = document.getElementById('guests')?.value || '1';
        
        const formData = {
            firstName,
            lastName,
            email,
            phone,
            checkinDate,
            checkoutDate,
            guests
        };
        
        console.log('Saving form data:', formData);
        console.log('Individual values:', {
            firstName, lastName, email, phone, checkinDate, checkoutDate, guests
        });
        
        localStorage.setItem('formData', JSON.stringify(formData));
        
        // Verify save
        const saved = localStorage.getItem('formData');
        console.log('Verified saved data:', saved);
    }
}

// Khởi tạo validator khi DOM loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, initializing BookingValidator...');
    
    // Debug: Kiểm tra localStorage trước khi khởi tạo
    console.log('=== DEBUG LOCALSTORAGE ===');
    console.log('bookingData:', localStorage.getItem('bookingData'));
    console.log('formData:', localStorage.getItem('formData'));
    console.log('cardData:', localStorage.getItem('cardData'));
    console.log('bankData:', localStorage.getItem('bankData'));
    console.log('==========================');
    
    const validator = new BookingValidator();
    
    // Nếu đang ở trang xác nhận, load dữ liệu ngay lập tức
    if (window.location.pathname.includes('xac-nhan.html')) {
        console.log('On confirmation page, loading booking details...');
        
        // Test: Thêm dữ liệu mẫu nếu không có
        const bookingData = localStorage.getItem('bookingData');
        const formData = localStorage.getItem('formData');
        
        if (!bookingData && !formData) {
            console.log('No data found, adding sample data for testing...');
            const sampleData = {
                firstName: 'Nuyễn',
                lastName: 'Phước',
                email: '23050095@student.bdu.edu.vn',
                phone: '09999999999',
                checkinDate: '20/09/2025',
                checkoutDate: '21/09/2025',
                guests: '1',
                paymentMethod: 'bank-transfer',
                timestamp: new Date().toISOString()
            };
            localStorage.setItem('bookingData', JSON.stringify(sampleData));
            console.log('Added sample booking data:', sampleData);
        }
        
        // Test: Kiểm tra elements trước khi load
        console.log('Testing elements...');
        const customerNameEl = document.getElementById('customer-name');
        const checkoutDateEl = document.getElementById('checkout-date');
        
        console.log('customer-name element:', customerNameEl);
        console.log('checkout-date element:', checkoutDateEl);
        
        if (customerNameEl) {
            console.log('customer-name element found, testing manual set...');
            customerNameEl.textContent = 'TEST NAME';
            console.log('Set customer name to TEST NAME');
        } else {
            console.error('customer-name element NOT FOUND!');
        }
        
        if (checkoutDateEl) {
            console.log('checkout-date element found, testing manual set...');
            checkoutDateEl.textContent = 'TEST DATE';
            console.log('Set checkout date to TEST DATE');
        } else {
            console.error('checkout-date element NOT FOUND!');
        }
        
        // Debug: Kiểm tra tất cả elements
        console.log('=== DEBUG ALL ELEMENTS ===');
        const allElements = [
            'customer-name', 'customer-email', 'customer-phone',
            'booking-code', 'checkin-date', 'checkout-date', 'guests',
            'payment-method', 'card-info', 'card-expiry', 'bank-info', 'bank-account'
        ];
        
        allElements.forEach(id => {
            const el = document.getElementById(id);
            console.log(`${id}:`, el ? 'FOUND' : 'NOT FOUND', el);
        });
        console.log('========================');
        
        // Force test: Set values manually với dữ liệu thực từ form
        setTimeout(() => {
            console.log('=== FORCE TEST SET VALUES ===');
            const customerNameEl = document.getElementById('customer-name');
            const checkoutDateEl = document.getElementById('checkout-date');
            
            if (customerNameEl) {
                customerNameEl.textContent = 'Nuyễn Phước';
                console.log('Force set customer name to: Nuyễn Phước');
            }
            
            if (checkoutDateEl) {
                checkoutDateEl.textContent = '21/09/2025';
                console.log('Force set checkout date to: 21/09/2025');
            }
            console.log('=============================');
        }, 1000);
        
        // Debug: Kiểm tra dữ liệu thực từ form
        console.log('=== DEBUG REAL DATA ===');
        const realBookingData = localStorage.getItem('bookingData');
        const realFormData = localStorage.getItem('formData');
        
        if (realBookingData) {
            try {
                const parsed = JSON.parse(realBookingData);
                console.log('Real booking data:', parsed);
                console.log('firstName:', parsed.firstName);
                console.log('lastName:', parsed.lastName);
                console.log('checkoutDate:', parsed.checkoutDate);
            } catch (e) {
                console.error('Error parsing real booking data:', e);
            }
        }
        
        if (realFormData) {
            try {
                const parsed = JSON.parse(realFormData);
                console.log('Real form data:', parsed);
                console.log('firstName:', parsed.firstName);
                console.log('lastName:', parsed.lastName);
                console.log('checkoutDate:', parsed.checkoutDate);
            } catch (e) {
                console.error('Error parsing real form data:', e);
            }
        }
        console.log('======================');
        
        validator.loadBookingDetails();
    }
});

// Load dữ liệu form khi vào trang thanh toán
document.addEventListener('DOMContentLoaded', () => {
    if (window.location.pathname.includes('thanh-toan.html')) {
        loadFormData();
    }
});

function loadFormData() {
    console.log('Loading form data for payment page...');
    const formData = localStorage.getItem('formData');
    console.log('Form data from localStorage:', formData);
    
    if (formData) {
        try {
            const data = JSON.parse(formData);
            console.log('Parsed form data:', data);
            
            // Populate form fields if they exist
            Object.keys(data).forEach(key => {
                const element = document.getElementById(key);
                if (element) {
                    element.value = data[key];
                    console.log(`Set ${key} = ${data[key]}`);
                } else {
                    console.log(`Element with id "${key}" not found`);
                }
            });
        } catch (e) {
            console.error('Error parsing form data:', e);
        }
    } else {
        console.log('No form data found in localStorage');
    }
}
