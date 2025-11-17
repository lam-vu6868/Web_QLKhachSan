// =============================================
//  QUẢN LÝ ĐẶT PHÒNG - JavaScript
//  File: datPhong.js
//  🔥 VIẾT LẠI HOÀN TOÀN - ĐƠN GIẢN & ỔN ĐỊNH
// =============================================

$(document).ready(function () {
    // ===== KHỞI TẠO =====
    initCreateForm();
    calculatePrices();
});

// ===== KHỞI TẠO FORM CREATE =====
function initCreateForm() {
    // 1. Tính số đêm tự động khi chọn ngày
    $('#txtNgayNhan, #txtNgayTra').on('change', function () {
   calculateSoDem();
        calculatePrices();
    });

 // 2. Tính tiền khi thay đổi số lượng hoặc giảm giá
    $('.input-so-luong, .input-giam-gia').on('input', function () {
      var index = $(this).data('index');
   calculateThanhTien(index);
  calculatePrices();
   
        // Load tầng khi chọn số lượng > 0
        if ($(this).hasClass('input-so-luong') && $(this).val() > 0) {
      var loaiPhongId = $('.select-tang[data-index="' + index + '"]').data('loaiphongid');
 loadAvailableFloors(loaiPhongId, index);
        }
    });

    // 3. Tìm khách hàng
    $('#btnTimKhach').on('click', function () {
   timKhachHang();
    });

    // 4. Validation form trước khi submit
    $('form').on('submit', function (e) {
        if (!validateForm()) {
    e.preventDefault();
   return false;
}
    });

 // 5. Set min date cho ngày nhận = hôm nay
    var today = new Date().toISOString().split('T')[0];
    $('#txtNgayNhan').attr('min', today);
}

// ===== TÍNH SỐ ĐÊM =====
function calculateSoDem() {
    var ngayNhan = new Date($('#txtNgayNhan').val());
    var ngayTra = new Date($('#txtNgayTra').val());

    if (ngayNhan && ngayTra && ngayTra > ngayNhan) {
    var soDem = Math.ceil((ngayTra - ngayNhan) / (1000 * 60 * 60 * 24));
        $('#txtSoDem').val(soDem);
        return soDem;
    } else {
        $('#txtSoDem').val(0);
        return 0;
    }
}

// ===== TÍNH THÀNH TIỀN CHO 1 DÒNG =====
function calculateThanhTien(index) {
    var row = $('tr').has('[data-index="' + index + '"]');
    var donGia = parseFloat(row.find('input[name*="DonGia"]').val()) || 0;
    var soLuong = parseInt(row.find('.input-so-luong').val()) || 0;
    var giamGia = parseFloat(row.find('.input-giam-gia').val()) || 0;
    var soDem = parseInt($('#txtSoDem').val()) || 0;

    if (soLuong > 0 && soDem > 0) {
        var tongTien = donGia * soLuong * soDem;
        var tienGiam = tongTien * (giamGia / 100);
   var thanhTien = tongTien - tienGiam;

        row.find('.thanh-tien').text(formatCurrency(thanhTien));
        return thanhTien;
    } else {
        row.find('.thanh-tien').text('0đ');
return 0;
    }
}

// ===== TÍNH TỔNG TIỀN =====
function calculatePrices() {
    var tongTienPhong = 0;
    var tongGiamGia = 0;

    $('.thanh-tien').each(function (index) {
     var row = $(this).closest('tr');
    var donGia = parseFloat(row.find('input[name*="DonGia"]').val()) || 0;
var soLuong = parseInt(row.find('.input-so-luong').val()) || 0;
        var giamGia = parseFloat(row.find('.input-giam-gia').val()) || 0;
     var soDem = parseInt($('#txtSoDem').val()) || 0;

        if (soLuong > 0 && soDem > 0) {
   var tien = donGia * soLuong * soDem;
            tongTienPhong += tien;
 tongGiamGia += tien * (giamGia / 100);
        }
    });

    var tongCong = tongTienPhong - tongGiamGia;

 $('#tongTienPhong').text(formatCurrency(tongTienPhong));
    $('#tongGiamGia').text('-' + formatCurrency(tongGiamGia));
    $('#tongCong').text(formatCurrency(tongCong));
}

// ===== TÌM KHÁCH HÀNG THEO SĐT =====
function timKhachHang() {
  var sdt = $('#txtSoDienThoai').val().trim();

    if (!sdt) {
        alert('Vui lòng nhập số điện thoại!');
return;
    }

  // Show loading
    $('#btnTimKhach').html('<i class="fas fa-spinner fa-spin"></i>').prop('disabled', true);

    $.ajax({
        url: '/NhanVienLeTan/DatPhong/TimKhachHang',
        type: 'POST',
      data: {
   __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
        sdt: sdt
        },
   success: function (response) {
 if (response.success && response.data) {
      // Điền thông tin khách cũ
 $('#txtHoVaTen').val(response.data.HoVaTen || '');
         $('#txtEmail').val(response.data.Email || '');
                $('#txtDiaChi').val(response.data.DiaChi || '');

  if (response.data.GioiTinh !== null && response.data.GioiTinh !== undefined) {
              $('input[name="GioiTinh"][value="' + response.data.GioiTinh + '"]').prop('checked', true);
        }

       showNotification('success', 'Đã tìm thấy khách hàng: ' + response.data.HoVaTen);
     } else {
    showNotification('info', 'Không tìm thấy khách hàng. Vui lòng nhập thông tin mới.');
                // Clear các field
           $('#txtHoVaTen, #txtEmail, #txtDiaChi').val('');
 $('input[name="GioiTinh"]').prop('checked', false);
       }
        },
        error: function () {
            showNotification('error', 'Lỗi khi tìm khách hàng!');
        },
        complete: function () {
   $('#btnTimKhach').html('<i class="fas fa-search"></i>').prop('disabled', false);
 }
    });
}

// ===== LOAD TẦNG ĐỘNG THEO LOẠI PHÒNG =====
function loadAvailableFloors(loaiPhongId, index) {
    var selectTang = $('.select-tang[data-index="' + index + '"]');
    var ngayNhan = $('#txtNgayNhan').val();
    var ngayTra = $('#txtNgayTra').val();

 if (!ngayNhan || !ngayTra) {
        return;
    }

    $.ajax({
        url: '/NhanVienLeTan/DatPhong/GetAvailableFloors',
        type: 'POST',
        data: {
    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
 loaiPhongId: loaiPhongId,
            ngayNhan: ngayNhan,
         ngayTra: ngayTra
 },
      success: function (response) {
    selectTang.html('<option value="">-- Chọn tầng --</option>');

         if (response.success && response.data && response.data.length > 0) {
       $.each(response.data, function (i, tang) {
      selectTang.append('<option value="' + tang.Tang + '">Tầng ' + tang.Tang + ' (' + tang.SoPhongTrong + ' phòng trống)</option>');
           });
        } else {
            selectTang.append('<option value="">Không có tầng nào</option>');
      }
        },
        error: function () {
        console.error('Lỗi khi load danh sách tầng');
        }
    });
}

// ===== VALIDATION FORM =====
function validateForm() {
    var isValid = true;
 var errors = [];

  // 1. Kiểm tra số điện thoại
    var sdt = $('#txtSoDienThoai').val().trim();
    if (!sdt) {
        errors.push('Vui lòng nhập số điện thoại!');
        isValid = false;
    }

    // 2. Kiểm tra họ tên
    var hoTen = $('#txtHoVaTen').val().trim();
    if (!hoTen) {
        errors.push('Vui lòng nhập họ tên!');
   isValid = false;
    }

    // 3. Kiểm tra ngày
    var ngayNhan = new Date($('#txtNgayNhan').val());
    var ngayTra = new Date($('#txtNgayTra').val());
 var today = new Date();
    today.setHours(0, 0, 0, 0);

    if (ngayNhan < today) {
        errors.push('Ngày nhận phòng không được là quá khứ!');
        isValid = false;
    }

    if (ngayTra <= ngayNhan) {
        errors.push('Ngày trả phòng phải sau ngày nhận!');
    isValid = false;
    }

    // 4. Kiểm tra số lượng khách
    var soKhach = parseInt($('#SoLuongKhach').val());
    if (!soKhach || soKhach < 1) {
        errors.push('Số lượng khách phải >= 1!');
        isValid = false;
    }

    // 5. Kiểm tra đã chọn phòng chưa
    var hasRoom = false;
    $('.input-so-luong').each(function () {
        if (parseInt($(this).val()) > 0) {
        hasRoom = true;
            return false;
        }
  });

    if (!hasRoom) {
   errors.push('Vui lòng chọn ít nhất 1 phòng!');
   isValid = false;
    }

    // Hiển thị lỗi
    if (!isValid) {
   alert('Lỗi:\n- ' + errors.join('\n- '));
    }

    return isValid;
}

// ===== SHOW NOTIFICATION =====
function showNotification(type, message) {
    var alertClass = type === 'success' ? 'alert-success' :
    type === 'info' ? 'alert-info' :
  type === 'warning' ? 'alert-warning' : 'alert-danger';

    var icon = type === 'success' ? 'fa-check-circle' :
    type === 'info' ? 'fa-info-circle' :
        type === 'warning' ? 'fa-exclamation-triangle' : 'fa-times-circle';

    var alertHtml = `
     <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
      <i class="fas ${icon}"></i> ${message}
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
      <span aria-hidden="true">&times;</span>
 </button>
   </div>
    `;

  $('.container-fluid').prepend(alertHtml);

    // Auto hide after 5s
    setTimeout(function () {
        $('.alert').fadeOut('slow', function () {
       $(this).remove();
        });
  }, 5000);
}

// ===== FORMAT CURRENCY =====
function formatCurrency(value) {
    if (!value || value === 0) return '0đ';
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
}
