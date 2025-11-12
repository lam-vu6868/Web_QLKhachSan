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
});
