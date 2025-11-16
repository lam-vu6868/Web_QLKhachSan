// =============================================
// DatPhong Create - JavaScript
// File: datphong-create.js
// =============================================

$(document).ready(function () {
    let currentStep = 1;

    // ===== STEP NAVIGATION =====
    function showStep(step) {
        $('.form-step').removeClass('active');
 $(`.form-step[data-step="${step}"]`).addClass('active');

        $('.step-item').removeClass('active completed');
    for (let i = 1; i <= step; i++) {
 if (i < step) {
          $(`.step-item[data-step="${i}"]`).addClass('completed');
        } else if (i === step) {
    $(`.step-item[data-step="${i}"]`).addClass('active');
          }
        }

    currentStep = step;

        // Update summary ở step 3
        if (step === 3) {
   updateCustomerSummary();
            updateRoomSummary();
   }
    }

    // Next/Prev buttons
    $('#btnNextStep1').click(function () {
 if (validateStep1()) {
            calculateTotalPrice();
            showStep(2);
      }
    });

    $('#btnNextStep2').click(function () {
        if (validateStep2()) {
            showStep(3);
        }
    });

  $('#btnPrevStep2').click(function () {
        showStep(1);
    });

    $('#btnPrevStep3').click(function () {
        showStep(2);
    });

    // ===== VALIDATION =====
    function validateStep1() {
        let isValid = true;
 let errors = [];

   // Clear previous errors
$('.form-control').removeClass('is-invalid');
     $('.invalid-feedback').remove();

  // Validate SĐT
        const sdt = $('#soDienThoai').val().trim();
      if (!sdt) {
         showFieldError('#soDienThoai', 'Vui lòng nhập số điện thoại!');
errors.push('Số điện thoại');
     isValid = false;
        } else if (!isValidPhoneNumber(sdt)) {
        showFieldError('#soDienThoai', 'Số điện thoại không hợp lệ! (VD: 0912345678)');
      errors.push('Số điện thoại');
            isValid = false;
        }

        // Validate Họ tên
        const hoTen = $('#HoVaTen').val().trim();
        if (!hoTen) {
 showFieldError('#HoVaTen', 'Vui lòng nhập họ tên!');
   errors.push('Họ tên');
isValid = false;
} else if (hoTen.length < 2) {
            showFieldError('#HoVaTen', 'Họ tên phải có ít nhất 2 ký tự!');
            errors.push('Họ tên');
        isValid = false;
    } else if (!isValidVietnameseName(hoTen)) {
       showFieldError('#HoVaTen', 'Họ tên chỉ được chứa chữ cái!');
            errors.push('Họ tên');
            isValid = false;
        }

        // Validate Email (nếu có nhập)
  const email = $('#Email').val().trim();
        if (email && !isValidEmail(email)) {
     showFieldError('#Email', 'Email không hợp lệ!');
        errors.push('Email');
   isValid = false;
        }

        // Validate Ngày nhận
      const ngayNhan = new Date($('#ngayNhan').val());
      const today = new Date();
        today.setHours(0, 0, 0, 0);
        ngayNhan.setHours(0, 0, 0, 0);

        if (!$('#ngayNhan').val()) {
 showFieldError('#ngayNhan', 'Vui lòng chọn ngày nhận phòng!');
            errors.push('Ngày nhận');
  isValid = false;
     } else if (ngayNhan < today) {
 showFieldError('#ngayNhan', 'Ngày nhận phòng phải từ hôm nay trở đi!');
       errors.push('Ngày nhận');
            isValid = false;
     }

        // Validate Ngày trả
        const ngayTra = new Date($('#ngayTra').val());
        ngayTra.setHours(0, 0, 0, 0);

        if (!$('#ngayTra').val()) {
            showFieldError('#ngayTra', 'Vui lòng chọn ngày trả phòng!');
            errors.push('Ngày trả');
            isValid = false;
        } else if (ngayTra <= ngayNhan) {
            showFieldError('#ngayTra', 'Ngày trả phòng phải sau ngày nhận!');
       errors.push('Ngày trả');
   isValid = false;
        }

        // Validate Số lượng khách
        const soKhach = parseInt($('#SoLuongKhach').val());
        if (!soKhach || soKhach < 1) {
   showFieldError('#SoLuongKhach', 'Vui lòng nhập số lượng khách (tối thiểu 1)!');
        errors.push('Số lượng khách');
            isValid = false;
  } else if (soKhach > 50) {
            showFieldError('#SoLuongKhach', 'Số lượng khách tối đa 50 người!');
   errors.push('Số lượng khách');
isValid = false;
        }

        // Show summary error
   if (!isValid) {
      toastr.error('Vui lòng kiểm tra lại các thông tin: ' + errors.join(', '));
        }

        return isValid;
}

  function validateStep2() {
        let hasSelectedRoom = false;
        let totalRooms = 0;

        $('.room-quantity').each(function () {
            const qty = parseInt($(this).val()) || 0;
  if (qty > 0) {
      hasSelectedRoom = true;
  totalRooms += qty;
    }
        });

        if (!hasSelectedRoom) {
    toastr.error('Vui lòng chọn ít nhất một phòng!');
            return false;
   }

        // Kiểm tra số lượng khách có phù hợp với số phòng
        const soKhach = parseInt($('#SoLuongKhach').val()) || 0;
   if (totalRooms > soKhach) {
   toastr.warning(`Bạn đã chọn ${totalRooms} phòng cho ${soKhach} khách. Có chắc chắn không?`);
    }

        return true;
    }

    // Helper validation functions
    function isValidPhoneNumber(phone) {
// Vietnam phone number: 0xxxxxxxxx (10-11 digits)
     return /^(0[3|5|7|8|9])+([0-9]{8,9})$/.test(phone);
    }

    function isValidEmail(email) {
        return /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(email);
    }

    function isValidVietnameseName(name) {
        // Allow Vietnamese characters + spaces
        return /^[\p{L}\s]+$/u.test(name);
    }

function showFieldError(fieldSelector, message) {
        const $field = $(fieldSelector);
        $field.addClass('is-invalid');
        
        // Add error message
   $field.after(`<div class="invalid-feedback d-block">${message}</div>`);
        
        // Scroll to first error
        if ($('.is-invalid').length === 1) {
   $('html, body').animate({
          scrollTop: $field.offset().top - 100
        }, 500);
        }
    }
  // ===== TÌM KHÁCH HÀNG =====
    $('#btnTimKhach').click(function () {
 const sdt = $('#soDienThoai').val().trim();
    if (!sdt) {
        alert('Vui lòng nhập số điện thoại!');
  return;
  }

    // AJAX tìm khách hàng
        $.ajax({
         url: '/NhanVienLeTan/DatPhong/TimKhachHang',
   type: 'POST',
  data: { soDienThoai: sdt },
    success: function (response) {
    if (response.success) {
    // Fill thông tin khách hàng
    $('#HoVaTen').val(response.data.HoVaTen);
   $('#Email').val(response.data.Email);
     $('#DiaChi').val(response.data.DiaChi);
    if (response.data.GioiTinh !== null) {
   $('#GioiTinh').val(response.data.GioiTinh);
        }

        // Show message
 toastr.success('Đã tìm thấy khách hàng!');
      } else {
       toastr.info('Không tìm thấy khách hàng. Vui lòng nhập thông tin mới.');
    // Clear form
    $('#HoVaTen, #Email, #DiaChi').val('');
        $('#GioiTinh').val('');
   }
   },
            error: function () {
              toastr.error('Có lỗi xảy ra khi tìm khách hàng!');
  }
     });
    });

  // ===== TÍNH SỐ ĐÊM =====
    function calculateNights() {
        const ngayNhan = new Date($('#ngayNhan').val());
      const ngayTra = new Date($('#ngayTra').val());

      if (ngayNhan && ngayTra && ngayTra > ngayNhan) {
   const diffTime = Math.abs(ngayTra - ngayNhan);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
   $('#soDem').val(diffDays);
            calculateTotalPrice();
 } else {
          $('#soDem').val(0);
        }
    }

    $('#ngayNhan, #ngayTra').change(calculateNights);

  // ===== TĂNG/GIẢM SỐ LƯỢNG PHÒNG =====
    $('.btn-increase').click(function () {
        const index = $(this).data('index');
        const max = parseInt($(this).data('max'));
    const input = $(`.room-quantity[data-index="${index}"]`);
     let currentVal = parseInt(input.val()) || 0;

        if (currentVal < max) {
            input.val(currentVal + 1);
   updateRoomCard($(this).closest('.room-card'));
            calculateTotalPrice();
        } else {
        toastr.warning('Không đủ phòng trống!');
      }
    });

    $('.btn-decrease').click(function () {
        const index = $(this).data('index');
        const input = $(`.room-quantity[data-index="${index}"]`);
        let currentVal = parseInt(input.val()) || 0;

        if (currentVal > 0) {
       input.val(currentVal - 1);
     updateRoomCard($(this).closest('.room-card'));
            calculateTotalPrice();
        }
  });

    $('.room-quantity').change(function () {
        const max = parseInt($(this).attr('max'));
        let val = parseInt($(this).val()) || 0;

        if (val < 0) val = 0;
if (val > max) {
            val = max;
            toastr.warning('Không đủ phòng trống!');
        }

        $(this).val(val);
        updateRoomCard($(this).closest('.room-card'));
        calculateTotalPrice();
    });

    function updateRoomCard($card) {
        const quantity = parseInt($card.find('.room-quantity').val()) || 0;
        if (quantity > 0) {
        $card.addClass('selected');
        } else {
  $card.removeClass('selected');
        }
    }

    // ===== TÍNH TỔNG TIỀN =====
    function calculateTotalPrice() {
        const soDem = parseInt($('#soDem').val()) || 1;
        let tongTienPhong = 0;

        // Tính tổng tiền phòng
        $('.room-quantity').each(function () {
      const quantity = parseInt($(this).val()) || 0;
          const donGia = parseFloat($(this).closest('.room-card').find('input[name*="DonGia"]').val()) || 0;
         tongTienPhong += quantity * donGia * soDem;
        });

        // Giảm giá (sẽ update khi áp dụng khuyến mãi)
        const tienGiamGia = parseFloat($('#TienGiamGia').val()) || 0;

        // Thuế VAT 10%
        const thueVAT = (tongTienPhong - tienGiamGia) * 0.1;

        // Tổng cộng
        const tongCong = tongTienPhong - tienGiamGia + thueVAT;

        // Update UI
    $('#tongTienPhong').text(formatCurrency(tongTienPhong));
        $('#tienGiamGia').text(formatCurrency(tienGiamGia));
        $('#thueVAT').text(formatCurrency(thueVAT));
    $('#tongCong').text(formatCurrency(tongCong));
    }

  // ===== ÁP DỤNG KHUYẾN MÃI =====
    $('#btnApDungKM').click(function () {
        const maKM = $('#MaKhuyenMai').val().trim();
        if (!maKM) {
     toastr.warning('Vui lòng nhập mã khuyến mãi!');
      return;
}

        const tongTien = parseFloat($('#tongTienPhong').text().replace(/[^0-9]/g, ''));

   $.ajax({
            url: '/NhanVienLeTan/DatPhong/ValidateKhuyenMai',
  type: 'POST',
            data: { maCode: maKM, tongTien: tongTien },
  success: function (response) {
              if (response.success) {
      $('#MaKhuyenMaiId').val(response.maKhuyenMai);
             $('#TienGiamGia').val(response.tienGiam);
       $('#kmMessage').html(`<i class="fas fa-check-circle text-success"></i> ${response.tenKhuyenMai} - Giảm ${formatCurrency(response.tienGiam)}`).addClass('text-success');
            calculateTotalPrice();
          toastr.success('Áp dụng mã khuyến mãi thành công!');
        } else {
        $('#kmMessage').html(`<i class="fas fa-times-circle text-danger"></i> ${response.message}`).addClass('text-danger');
        toastr.error(response.message);
          }
},
      error: function () {
          toastr.error('Có lỗi xảy ra!');
            }
        });
    });

    // ===== UPDATE SUMMARY STEP 3 =====
    function updateCustomerSummary() {
        const html = `
   <table class="table table-sm table-borderless mb-0">
                <tr>
                    <td class="text-muted" width="40%">Họ tên:</td>
         <td><strong>${$('#HoVaTen').val()}</strong></td>
 </tr>
          <tr>
        <td class="text-muted">Số điện thoại:</td>
                    <td><strong>${$('#soDienThoai').val()}</strong></td>
            </tr>
      <tr>
     <td class="text-muted">Email:</td>
        <td>${$('#Email').val() || 'N/A'}</td>
          </tr>
        <tr>
          <td class="text-muted">Ngày nhận:</td>
            <td><strong>${formatDate($('#ngayNhan').val())}</strong></td>
    </tr>
    <tr>
  <td class="text-muted">Ngày trả:</td>
         <td><strong>${formatDate($('#ngayTra').val())}</strong></td>
       </tr>
          <tr>
     <td class="text-muted">Số đêm:</td>
   <td><strong class="text-primary">${$('#soDem').val()} đêm</strong></td>
   </tr>
  <tr>
     <td class="text-muted">Số khách:</td>
        <td><strong>${$('#SoLuongKhach').val()} người</strong></td>
          </tr>
            </table>
  `;
        $('#customerSummary').html(html);
    }

    function updateRoomSummary() {
      let html = '<table class="table table-sm">';
        html += '<thead><tr><th>Loại phòng</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>';

        const soDem = parseInt($('#soDem').val()) || 1;
        let hasRoom = false;

        $('.room-quantity').each(function () {
            const quantity = parseInt($(this).val()) || 0;
    if (quantity > 0) {
    hasRoom = true;
      const card = $(this).closest('.room-card');
       const tenPhong = card.find('input[name*="TenLoaiPhong"]').val();
     const donGia = parseFloat(card.find('input[name*="DonGia"]').val()) || 0;
                const thanhTien = quantity * donGia * soDem;

  html += `<tr>
           <td>${tenPhong}</td>
           <td>${quantity}</td>
          <td>${formatCurrency(donGia)}/đêm</td>
            <td class="text-primary"><strong>${formatCurrency(thanhTien)}</strong></td>
        </tr>`;
   }
     });

        if (!hasRoom) {
       html += '<tr><td colspan="4" class="text-center text-muted">Chưa chọn phòng</td></tr>';
   }

        html += '</tbody></table>';
      $('#roomSummary').html(html);
    }

  // ===== HELPER FUNCTIONS =====
    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }

    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
    }

    // ===== FORM SUBMIT =====
    $('#createBookingForm').submit(function (e) {
      // Disable submit button
   $('#btnSubmit').prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');
    });

    // ===== INIT =====
    calculateNights();
    calculateTotalPrice();

    // ===== REAL-TIME VALIDATION =====
    
    // Validate SĐT khi blur
    $('#soDienThoai').on('blur', function () {
   const sdt = $(this).val().trim();
$(this).removeClass('is-invalid is-valid');
      $(this).next('.invalid-feedback').remove();
        
     if (sdt) {
    if (isValidPhoneNumber(sdt)) {
    $(this).addClass('is-valid');
   } else {
                $(this).addClass('is-invalid');
         $(this).after('<div class="invalid-feedback d-block">Số điện thoại không hợp lệ!</div>');
     }
        }
    });

    // Validate Họ tên khi blur
    $('#HoVaTen').on('blur', function () {
        const hoTen = $(this).val().trim();
     $(this).removeClass('is-invalid is-valid');
        $(this).next('.invalid-feedback').remove();
   
  if (hoTen) {
     if (hoTen.length >= 2 && isValidVietnameseName(hoTen)) {
       $(this).addClass('is-valid');
  } else {
   $(this).addClass('is-invalid');
           $(this).after('<div class="invalid-feedback d-block">Họ tên không hợp lệ!</div>');
     }
        }
    });

    // Validate Email khi blur
    $('#Email').on('blur', function () {
        const email = $(this).val().trim();
    $(this).removeClass('is-invalid is-valid');
$(this).next('.invalid-feedback').remove();
        
 if (email) {
     if (isValidEmail(email)) {
           $(this).addClass('is-valid');
        } else {
     $(this).addClass('is-invalid');
          $(this).after('<div class="invalid-feedback d-block">Email không hợp lệ!</div>');
            }
        }
    });

    // Validate Số lượng khách khi change
    $('#SoLuongKhach').on('input', function () {
const soKhach = parseInt($(this).val());
        $(this).removeClass('is-invalid is-valid');
     $(this).next('.invalid-feedback').remove();
      
        if (soKhach) {
if (soKhach >= 1 && soKhach <= 50) {
   $(this).addClass('is-valid');
       } else {
            $(this).addClass('is-invalid');
       $(this).after('<div class="invalid-feedback d-block">Số khách từ 1-50 người!</div>');
   }
        }
    });

    // Auto-fix phone number format (remove spaces, dashes)
    $('#soDienThoai').on('input', function () {
     let val = $(this).val().replace(/[^0-9]/g, ''); // Chỉ giữ số
        $(this).val(val);
    });

    // Auto-capitalize first letter of each word in name
  $('#HoVaTen').on('input', function () {
        let val = $(this).val();
     val = val.replace(/\s+/g, ' '); // Remove extra spaces
     $(this).val(val);
});

    // Prevent negative numbers
    $('input[type="number"]').on('keydown', function (e) {
if (e.key === '-' || e.key === 'e' || e.key === '+') {
            e.preventDefault();
        }
    });
});
