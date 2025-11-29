using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.PhanCong
{
    /// <summary>
    /// Validation attribute để kiểm tra thời gian phải trong tương lai (hoặc ít nhất là không quá xa trong quá khứ)
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly int _minutesOffset;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="minutesOffset">Số phút cho phép trong quá khứ (mặc định 0 = phải là tương lai)</param>
        public FutureDateAttribute(int minutesOffset = 0)
        {
            _minutesOffset = minutesOffset;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                // Cho phép null (trường không bắt buộc)
                return ValidationResult.Success;
            }

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                DateTime now = DateTime.Now;
                DateTime minDateTime = now.AddMinutes(-_minutesOffset);

                if (dateTime < minDateTime)
                {
                    if (_minutesOffset == 0)
                    {
                        return new ValidationResult("Thời gian dự kiến phải sau thời gian hiện tại!");
                    }
                    else
                    {
                        return new ValidationResult($"Thời gian dự kiến không được quá {_minutesOffset} phút trong quá khứ!");
                    }
                }

                // Kiểm tra thời gian không quá xa trong tương lai (ví dụ: không quá 1 năm)
                DateTime maxDateTime = now.AddYears(1);
                if (dateTime > maxDateTime)
                {
                    return new ValidationResult("Thời gian dự kiến không được quá 1 năm trong tương lai!");
                }
            }

            return ValidationResult.Success;
        }
    }
}

