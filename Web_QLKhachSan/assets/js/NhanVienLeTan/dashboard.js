/**
 * =============================================
 * DASHBOARD LE TAN - JAVASCRIPT
 * =============================================
 */

(function($) {
    'use strict';

    $(document).ready(function() {
        // Load du lieu
        loadThongKe();
        loadCheckInList();
        loadCheckOutList();
    loadActivities();

  // Cap nhat thoi gian
        updateDateTime();
        setInterval(updateDateTime, 1000);

        // Auto refresh moi 30 giay
    setInterval(function() {
            loadThongKe();
 loadCheckInList();
      loadCheckOutList();
       loadActivities();
        }, 30000);
    });

    /**
     * Update Date and Time Display
     */
    function updateDateTime() {
    const now = new Date();
        $('#current-time').text(now.toLocaleTimeString('vi-VN'));
        $('#current-date').text(now.toLocaleDateString('vi-VN', { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
day: 'numeric' 
        }));
    }

    /**
     * Load Thong ke tong quan
     */
    function loadThongKe() {
      $.ajax({
 url: '/NhanVienLeTan/DashboardNVLeTan/GetThongKeTongQuan',
            type: 'GET',
    success: function(response) {
       if (response.success) {
               const data = response.data;
       $('#stat-dat-phong').text(data.datPhongHomNay);
          $('#stat-checkin').text(data.checkInHomNay);
   $('#stat-checkout').text(data.checkOutHomNay);
    $('#stat-phong-trong').html(data.phongTrong + '/<span id="stat-tong-phong">' + data.tongPhong + '</span>');
    $('#stat-ty-le').text(data.tyLePhong.toFixed(1) + '% l\u1EA5p \u0111\u1EA7y'); // % lấp đầy
        $('#stat-doanh-thu').text(formatCurrency(data.doanhThuHomNay));
    }
         },
      error: function(xhr, status, error) {
             console.error('Error loading thong ke:', error);
     }
 });
    }

  /**
     * Load Check-in List
     */
    function loadCheckInList() {
        $.ajax({
     url: '/NhanVienLeTan/DashboardNVLeTan/GetCheckInSapToi',
         type: 'GET',
      success: function(response) {
         if (response.success) {
    displayCheckInList(response.data);
           $('#badge-checkin').text(response.data.length);
        }
        },
      error: function(xhr, status, error) {
   console.error('Error loading check-in list:', error);
      $('#list-checkin').html('<div class="empty-state"><i class="fas fa-exclamation-triangle"></i><p>L\u1ED7i t\u1EA3i d\u1EEF li\u1EC7u</p></div>');
            }
        });
    }

    /**
     * Display Check-in List
     */
    function displayCheckInList(data) {
      let html = '';
        if (data.length === 0) {
    html = '<div class="empty-state"><i class="fas fa-inbox"></i><p>Kh\u00F4ng c\u00F3 check-in n\u00E0o</p></div>';
    } else {
  data.forEach(function(item) {
    html += `
              <div class="list-item">
 <div class="item-icon bg-success">
 <i class="fas fa-user"></i>
        </div>
            <div class="item-content">
       <h4>${item.tenKhachHang}</h4>
          <p><i class="fas fa-phone"></i> ${item.soDienThoai || 'N/A'}</p>
               <p><i class="fas fa-bed"></i> ${item.danhSachPhong}</p>
         </div>
      <div class="item-action">
          <span class="time-badge">${item.gioNhan}</span>
         <a href="/NhanVienLeTan/CheckInOut" class="btn btn-sm btn-success">
      <i class="fas fa-sign-in-alt"></i>
          </a>
               </div>
     </div>
    `;
         });
        }
        $('#list-checkin').html(html);
    }

    /**
     * Load Check-out List
  */
    function loadCheckOutList() {
      $.ajax({
            url: '/NhanVienLeTan/DashboardNVLeTan/GetCheckOutSapToi',
       type: 'GET',
         success: function(response) {
      if (response.success) {
  displayCheckOutList(response.data);
     $('#badge-checkout').text(response.data.length);
  }
   },
      error: function(xhr, status, error) {
     console.error('Error loading check-out list:', error);
      $('#list-checkout').html('<div class="empty-state"><i class="fas fa-exclamation-triangle"></i><p>L\u1ED7i t\u1EA3i d\u1EEF li\u1EC7u</p></div>');
            }
        });
    }

    /**
     * Display Check-out List
     */
    function displayCheckOutList(data) {
        let html = '';
        if (data.length === 0) {
            html = '<div class="empty-state"><i class="fas fa-inbox"></i><p>Kh\u00F4ng c\u00F3 check-out n\u00E0o</p></div>';
        } else {
    data.forEach(function(item) {
   const paymentBadge = item.daThanhToan 
          ? '<span class="badge bg-success"><i class="fas fa-check"></i> \u0110\u00E3 TT</span>'
   : '<span class="badge bg-danger"><i class="fas fa-times"></i> Ch\u01B0a TT</span>';
           
   html += `
      <div class="list-item">
   <div class="item-icon bg-warning">
       <i class="fas fa-user"></i>
           </div>
  <div class="item-content">
              <h4>${item.tenKhachHang}</h4>
            <p><i class="fas fa-phone"></i> ${item.soDienThoai || 'N/A'}</p>
  <p><i class="fas fa-bed"></i> ${item.danhSachPhong}</p>
      ${paymentBadge}
     </div>
         <div class="item-action">
   <span class="time-badge">${item.gioTra}</span>
         <a href="/NhanVienLeTan/CheckInOut" class="btn btn-sm btn-warning">
  <i class="fas fa-sign-out-alt"></i>
    </a>
      </div>
                  </div>
    `;
          });
        }
        $('#list-checkout').html(html);
    }

    /**
     * Load Activities
     */
  function loadActivities() {
   $.ajax({
   url: '/NhanVienLeTan/DashboardNVLeTan/GetHoatDongGanDay',
    type: 'GET',
       success: function(response) {
            if (response.success) {
  displayActivities(response.data);
       }
     },
 error: function(xhr, status, error) {
     console.error('Error loading activities:', error);
             $('#list-activities').html('<div class="empty-state"><i class="fas fa-exclamation-triangle"></i><p>L\u1ED7i t\u1EA3i d\u1EEF li\u1EC7u</p></div>');
}
   });
    }

    /**
     * Display Activities
     */
  function displayActivities(data) {
        let html = '';
        if (data.length === 0) {
            html = '<div class="empty-state"><i class="fas fa-inbox"></i><p>Ch\u01B0a c\u00F3 ho\u1EA1t \u0111\u1ED9ng n\u00E0o</p></div>';
      } else {
            html = '<div class="activity-timeline">';
            data.forEach(function(item) {
       html += `
      <div class="activity-item">
    <div class="activity-icon bg-${item.mau}">
          <i class="fas ${item.icon}"></i>
    </div>
             <div class="activity-content">
        <h4>${item.tieuDe}</h4>
     <p>${item.noiDung}</p>
   <span class="activity-time">${item.thoiGian}</span>
               </div>
          </div>
       `;
      });
 html += '</div>';
        }
        $('#list-activities').html(html);
    }

    /**
     * Format Currency
     */
    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { 
   style: 'currency', 
            currency: 'VND' 
        }).format(amount);
    }

    // Export functions to global scope if needed
    window.DashboardLeTan = {
        loadThongKe: loadThongKe,
        loadCheckInList: loadCheckInList,
    loadCheckOutList: loadCheckOutList,
        loadActivities: loadActivities,
formatCurrency: formatCurrency
    };

})(jQuery);
