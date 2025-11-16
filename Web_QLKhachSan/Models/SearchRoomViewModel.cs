using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Models
{
    public class SearchRoomViewModel
    {
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Số khách")]
        public int GuestCount { get; set; }

        [Display(Name = "Loại phòng")]
        public int? RoomTypeId { get; set; }

        // Properties for dropdown data
        public List<LoaiPhong> RoomTypes { get; set; }
        public List<int> GuestOptions { get; set; }

        public SearchRoomViewModel()
        {
            // Set default dates
            CheckInDate = DateTime.Now.Date;
            CheckOutDate = DateTime.Now.Date.AddDays(1);
            GuestCount = 1; // Default value (not used in search form anymore)
            RoomTypes = new List<LoaiPhong>();
            GuestOptions = new List<int> { 1, 2, 3, 4 }; // Hotel max capacity is 4 people
        }
    }

    public class HomeIndexViewModel
    {
        public SearchRoomViewModel SearchModel { get; set; }
        public List<LoaiDichVu> LoaiDichVus { get; set; }

        public HomeIndexViewModel()
        {
            SearchModel = new SearchRoomViewModel();
            LoaiDichVus = new List<LoaiDichVu>();
        }
    }
}