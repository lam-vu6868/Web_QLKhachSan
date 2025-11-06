export function loadingHeader() {
    document.getElementById("_header").innerHTML = `
    <nav class="main-navbar">
      <div class="navbar-container">
        <a class="navbar-logo" style="display: flex; align-items: center; gap: 10px;">
          <img src="./assets/imgs/logo.png" alt="Logo" style="height: 38px; width: auto; display: inline-block; vertical-align: middle;" />
          <span>Serene Horizon</span>
        </a>

        <ul class="navbar-links">
          <li><a href="index.html">Trang Chủ</a></li>
          <li><a href="phong-nghi.html">Phòng Nghỉ</a></li>
          <li><a href="dich-vu.html">Dịch Vụ</a></li>
          <li><a href="gioi-thieu.html">Giới Thiệu</a></li>
        </ul>

        <div class="navbar-right">
          <div class="navbar-actions">
            <a href="/dang-nhap.html" class="cta-button">Đăng Nhập</a>
          </div>
          <button class="navbar-toggler" aria-label="Toggle navigation">
            <span></span><span></span><span></span>
          </button>
        </div>
      </div>
    </nav>
    <div class="mobile-menu-overlay">
      <div class="mobile-menu-content">
        <ul>
          <li><a href="index.html">Trang Chủ</a></li>
          <li><a href="phong-nghi.html">Phòng</a></li>
          <li><a href="dich-vu.html">Dịch Vụ</a></li>
          <li><a href="gioi-thieu.html">Giới Thiệu</a></li>
        </ul>
        <a href="dang-nhap.html" class="mobile-cta-button">Đăng Nhập</a>
      </div>
    </div>
    `;
}

export function loadingFooter() {
    document.getElementById("_footer").innerHTML = `
    <div class="container footer-layout">
        <div class="footer-col">
          <h4>The Serene Horizon</h4>
          <p>
            Nơi sự sang trọng và bình yên hòa quyện, mang đến những trải nghiệm
            nghỉ dưỡng đích thực.
          </p>
        </div>
        <div class="footer-col">
          <h4>Liên kết nhanh</h4>
          <ul>
            <li><a href="#">Về chúng tôi</a></li>
            <li><a href="#">Các loại phòng</a></li>
            <li><a href="#">Thư viện ảnh</a></li>
            <li><a href="#">Liên hệ</a></li>
          </ul>
        </div>
        <div class="footer-col">
          <h4>Liên hệ</h4>
          <p>
            <i class="fas fa-map-marker-alt"></i> 123 Đường Bờ Biển, Thành Phố
            Paradise
          </p>
          <p><i class="fas fa-phone"></i> (+84) 123 456 789</p>
          <p><i class="fas fa-envelope"></i> contact@serenehorizon.com</p>
        </div>
        <div class="footer-col">
          <h4>Theo dõi chúng tôi</h4>
          <div class="social-icons">
            <a href="#"><i class="fab fa-facebook-f"></i></a>
            <a href="#"><i class="fab fa-instagram"></i></a>
            <a href="#"><i class="fab fa-youtube"></i></a>
          </div>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; 2025 The Serene Horizon. All Rights Reserved.</p>
      </div>
     `;
}

export function loadingOrderSummary() {
  document.getElementById("order-summary").innerHTML = `
    <h4>Tóm Tắt Đơn Hàng</h4>
    <div class="summary-room-details">
      <img src="https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&w=300&q=80" alt="Deluxe Ocean View" />
      <div class="room-info-summary">
        <h5>Deluxe Ocean View</h5>
        <p>2 khách</p>
      </div>
    </div>
    <div class="summary-line"><span>Nhận phòng:</span><span>10/09/2025</span></div>
    <div class="summary-line"><span>Trả phòng:</span><span>12/09/2025</span></div>
    <hr />
    <div class="summary-line"><span>Giá 2 đêm (2 x $150):</span><span>$300.00</span></div>
    <div class="summary-line"><span>Thuế & Phí:</span><span>$30.00</span></div>
    <hr />P
    <div class="summary-line total"><span>Tổng Cộng:</span><span>$330.00</span></div>
    <div class="trust-badges">
      <i class="fas fa-shield-alt"></i>
      <span>Giao dịch an toàn và bảo mật</span>
    </div>
  `;
}
