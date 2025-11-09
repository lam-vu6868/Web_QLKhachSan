/* =============================================
   Sơ Đồ Phòng JavaScript
   File: sodoPhong.js
============================================= */

(function () {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function () {
   initializeSoDoPhong();
    });

    /**
     * Initialize Sơ Đồ Phòng functionality
     */
  function initializeSoDoPhong() {
     console.log('Initializing Sơ Đồ Phòng...');
    }

    /**
     * View room details in modal
     * @param {number} phongId - Room ID
     */
    window.xemChiTietPhong = function (phongId) {
      $('#modalChiTietPhong').modal('show');
        $('#modalBody').html('<div class="text-center"><div class="loading-spinner"></div><p class="mt-2">Đang tải...</p></div>');

        $.ajax({
    url: '/NhanVienLeTan/SoDoPhong/ChiTietPhong',
        type: 'GET',
      data: { phongId: phongId },
     success: function (response) {
      if (response.success) {
       const phong = response.data;
          renderRoomDetails(phong);
       } else {
     showError(response.message);
        }
   },
            error: function (xhr, status, error) {
    console.error('Error loading room details:', error);
   showError('Có lỗi xảy ra khi tải thông tin phòng!');
    }
        });
    };

/**
     * Render room details in modal
     * @param {object} phong - Room data object
     */
  function renderRoomDetails(phong) {
        const html = `
        <div class="row">
    <div class="col-md-6">
        <h5 class="mb-3">
          <i class="fas fa-door-closed"></i> ${phong.MaPhong} - ${phong.TenPhong}
      </h5>
                 <table class="table table-sm table-borderless">
           <tr>
        <td class="font-weight-bold" style="width: 140px;">Loại phòng:</td>
      <td>${phong.TenLoaiPhong || 'N/A'}</td>
        </tr>
           <tr>
  <td class="font-weight-bold">Tầng:</td>
        <td>${phong.Tang}</td>
       </tr>
    <tr>
                 <td class="font-weight-bold">Giá:</td>
         <td>${formatCurrency(phong.GiaPhong)}</td>
            </tr>
          <tr>
             <td class="font-weight-bold">Số người tối đa:</td>
                  <td>${phong.SoNguoiToiDa || 'N/A'}</td>
        </tr>
           </table>
             </div>
        <div class="col-md-6">
   <p class="mb-3">
            <strong>Trạng thái:</strong> 
           <span class="badge badge-pill" style="background-color: ${phong.TrangThaiPhongColor}; color: white;">
     ${phong.TrangThaiPhongText}
        </span>
          </p>
${renderGuestInfo(phong)}
        ${renderNotes(phong)}
     </div>
            </div>
     <hr/>
    ${renderStatusButtons(phong)}
`;
    
        $('#modalBody').html(html);
    }

    /**
     * Render guest information if available
     * @param {object} phong - Room data
     * @returns {string} HTML string
     */
    function renderGuestInfo(phong) {
  if (!phong.TenKhach) return '';
 
return `
      <hr/>
 <h6 class="mb-2"><i class="fas fa-user"></i> Thông tin khách</h6>
         <table class="table table-sm table-borderless">
        <tr>
            <td class="font-weight-bold" style="width: 140px;">Tên khách:</td>
    <td>${phong.TenKhach}</td>
           </tr>
 <tr>
        <td class="font-weight-bold">SĐT:</td>
        <td>${phong.SoDienThoaiKhach || 'N/A'}</td>
     </tr>
 <tr>
    <td class="font-weight-bold">Check-in:</td>
         <td>${formatDate(phong.NgayCheckIn)}</td>
        </tr>
  <tr>
             <td class="font-weight-bold">Check-out:</td>
          <td>${formatDate(phong.NgayCheckOut)}</td>
  </tr>
            <tr>
   <td class="font-weight-bold">Số người:</td>
<td>${phong.SoNguoi || 'N/A'}</td>
                </tr>
  </table>
        `;
    }

    /**
     * Render notes if available
     * @param {object} phong - Room data
     * @returns {string} HTML string
     */
    function renderNotes(phong) {
        if (!phong.GhiChu) return '';
  
        return `
  <hr/>
      <p class="mb-0">
    <strong><i class="fas fa-sticky-note"></i> Ghi chú:</strong><br/>
              <span class="text-muted">${phong.GhiChu}</span>
    </p>
        `;
    }

    /**
     * Render status change buttons
     * @param {object} phong - Room data
     * @returns {string} HTML string
     */
    function renderStatusButtons(phong) {
      const buttons = [
  { status: 0, color: 'success', icon: 'door-open', text: 'Trống' },
        { status: 1, color: 'warning', icon: 'calendar-check', text: 'Đã đặt' },
         { status: 2, color: 'danger', icon: 'user', text: 'Đang ở' },
      { status: 3, color: 'info', icon: 'broom', text: 'Đang dọn' },
    { status: 4, color: 'secondary', icon: 'tools', text: 'Bảo trì' }
        ];

        const buttonHtml = buttons.map(btn => `
    <button type="button" 
          class="btn btn-sm btn-${btn.color} m-1" 
  onclick="chuyenTrangThai(${phong.PhongId}, ${btn.status})"
     ${phong.TrangThaiPhong === btn.status ? 'disabled' : ''}>
   <i class="fas fa-${btn.icon}"></i> ${btn.text}
  </button>
        `).join('');

        return `
            <div class="text-center">
      <p class="text-muted small mb-2">Thay đổi trạng thái phòng:</p>
          ${buttonHtml}
        </div>
 `;
 }

    /**
     * Change room status
     * @param {number} phongId - Room ID
 * @param {number} trangThaiMoi - New status
     */
    window.chuyenTrangThai = function (phongId, trangThaiMoi) {
        if (!confirm('Bạn có chắc muốn thay đổi trạng thái phòng này?')) {
 return;
        }

        // Show loading
        showToast('Đang cập nhật...', 'info');

    $.ajax({
     url: '/NhanVienLeTan/SoDoPhong/CapNhatTrangThai',
     type: 'POST',
       data: { 
          phongId: phongId, 
       trangThaiMoi: trangThaiMoi 
            },
   success: function (response) {
         if (response.success) {
    showToast(response.message, 'success');
     
    // Close modal and reload page after 1 second
        setTimeout(function () {
       $('#modalChiTietPhong').modal('hide');
             location.reload();
   }, 1000);
      } else {
             showToast(response.message, 'danger');
           }
         },
         error: function (xhr, status, error) {
                console.error('Error updating room status:', error);
         showToast('Có lỗi xảy ra khi cập nhật trạng thái!', 'danger');
      }
        });
    };

    /**
     * Show error message in modal
     * @param {string} message - Error message
     */
    function showError(message) {
   $('#modalBody').html(`
    <div class="alert alert-danger">
    <i class="fas fa-exclamation-triangle"></i> ${message}
    </div>
        `);
    }

    /**
     * Refresh room board
   */
    window.refreshRoomBoard = function () {
        location.reload();
    };

    /**
     * Filter rooms by floor
     * @param {number} tang - Floor number
     */
    window.filterByFloor = function (tang) {
        if (tang) {
 window.location.href = `/NhanVienLeTan/SoDoPhong?locTheoTang=${tang}`;
    } else {
        window.location.href = '/NhanVienLeTan/SoDoPhong';
        }
    };

    /**
     * Filter rooms by status
     * @param {number} status - Room status
     */
    window.filterByStatus = function (status) {
    if (status !== null && status !== undefined) {
  window.location.href = `/NhanVienLeTan/SoDoPhong?locTheoTrangThai=${status}`;
        } else {
      window.location.href = '/NhanVienLeTan/SoDoPhong';
   }
  };

    /**
     * Export room report (for future implementation)
     */
    window.exportRoomReport = function () {
  // TODO: Implement export functionality
        alert('Chức năng xuất báo cáo đang được phát triển!');
    };

})();
