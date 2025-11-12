// JavaScript for Room Listing Page - Sticky Sidebar
document.addEventListener("DOMContentLoaded", function () {
  const filterSidebar = document.querySelector(".filter-sidebar");

  if (filterSidebar) {
    // Debug: Log initial position
    console.log("Filter sidebar found:", filterSidebar);
    console.log(
      "Initial position:",
      window.getComputedStyle(filterSidebar).position
    );

    // Force sticky position if not already set
    const ensureSticky = () => {
      const currentPosition = window.getComputedStyle(filterSidebar).position;
      if (currentPosition !== "sticky" && window.innerWidth > 991) {
        filterSidebar.style.position = "sticky";
        filterSidebar.style.top = "80px";
        filterSidebar.style.zIndex = "999";
        console.log("Forced sticky position");
      }
    };

    // Ensure sticky on load and resize
    ensureSticky();
    window.addEventListener("resize", ensureSticky);

    // Debug scroll events
    window.addEventListener("scroll", function () {
      const rect = filterSidebar.getBoundingClientRect();
      console.log("Scroll position:", window.scrollY, "Sidebar top:", rect.top);
    });

    // Add visual indicator when sticky is active
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.intersectionRatio < 1) {
          filterSidebar.classList.add("is-sticky");
        } else {
          filterSidebar.classList.remove("is-sticky");
        }
      },
      {
        threshold: 1,
        rootMargin: "-80px 0px 0px 0px",
      }
    );

    observer.observe(filterSidebar);
  }

  // Price range slider functionality with fixed price levels
  const priceSlider = document.querySelector(
    '.price-range-slider input[type="range"]'
  );
  const priceDisplays = document.querySelectorAll(".price-range-slider .price-display span");
  const priceValueDisplay = document.querySelector(".price-value");

  if (priceSlider) {
    // Định nghĩa 4 mức giá cố định (300, 400, 600, 900)
    const priceLevels = [300000, 400000, 600000, 900000];
    
    // Format số tiền theo VN (300.000đ)
    function formatVND(amount) {
      return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + "đ";
    }
    
    // Lấy giá trị từ server (đã được set trong HTML value attribute)
    const currentLevel = parseInt(priceSlider.value);
    
    // Cập nhật hiển thị giá min/max
    if (priceDisplays.length >= 2) {
      priceDisplays[0].textContent = formatVND(priceLevels[0]); // 300.000đ
      priceDisplays[1].textContent = formatVND(priceLevels[3]); // 900.000đ
    }
    
    // Hàm cập nhật giá trị và gradient
    function updatePrice() {
      const level = parseInt(priceSlider.value);
      const selectedPrice = priceLevels[level];
      
      // Cập nhật giá hiển thị trong tiêu đề h5
      if (priceValueDisplay) {
        priceValueDisplay.textContent = `(≤ ${formatVND(selectedPrice)})`;
      }
      
      // Cập nhật gradient cho slider
      const percentage = (level / 3) * 100;
      priceSlider.style.background = `linear-gradient(to right, #d4af37 0%, #d4af37 ${percentage}%, #ddd ${percentage}%, #ddd 100%)`;
    }
    
    // Khởi tạo giá trị và gradient từ server
    updatePrice();
    
    // Lắng nghe sự kiện thay đổi
    priceSlider.addEventListener("input", updatePrice);
  }

  // Filter functionality
  const applyFilterBtn = document.querySelector(".filter-apply-btn");
  if (applyFilterBtn) {
    applyFilterBtn.addEventListener("click", function () {
      // Filter logic is handled by form submission
      console.log("Applying filters...");
    });
  }

  // --- Logic cho Off-Canvas Filter ---
  const openFilterBtn = document.querySelector(".open-filter-btn");
  const closeFilterBtn = document.querySelector(".close-filter-btn");

  // Tạo và chèn backdrop vào body
  const filterBackdrop = document.createElement("div");
  filterBackdrop.className = "filter-backdrop";
  document.body.appendChild(filterBackdrop);

  // Hàm để đóng/mở filter
  const toggleFilter = (show) => {
    if (filterSidebar && filterBackdrop) {
      filterSidebar.classList.toggle("active", show);
      filterBackdrop.classList.toggle("active", show);
      // Chặn cuộn trang nền khi filter mở
      document.body.style.overflow = show ? "hidden" : "";
    }
  };

  // Gán sự kiện cho các nút
  if (openFilterBtn) {
    openFilterBtn.addEventListener("click", () => toggleFilter(true));
  }

  if (closeFilterBtn) {
    closeFilterBtn.addEventListener("click", () => toggleFilter(false));
  }

  // Gán sự kiện cho backdrop để đóng filter
  filterBackdrop.addEventListener("click", () => toggleFilter(false));

  // --- Enhanced Filter Functionality ---

  // Collapsible Filter
  const filterHeader = document.querySelector(".filter-header");
  const filterContent = document.querySelector(".filter-content");
  const filterToggle = document.querySelector(".filter-toggle");

  if (filterHeader && filterContent && filterToggle) {
    filterHeader.addEventListener("click", () => {
      const isActive = filterContent.classList.contains("active");
      filterContent.classList.toggle("active", !isActive);
      filterToggle.classList.toggle("active", !isActive);
    });
  }

  // Quick Filter Tags
  const quickFilterTags = document.querySelectorAll(".quick-filter-tag");
  quickFilterTags.forEach((tag) => {
    tag.addEventListener("click", () => {
      // Remove active from all tags
      quickFilterTags.forEach((t) => t.classList.remove("active"));
      // Add active to clicked tag
      tag.classList.add("active");

      // Here you can add logic to filter rooms based on the selected quick filter
      console.log("Quick filter selected:", tag.textContent);
    });
  });

  // Enhanced Apply Button with Loading State
  const enhancedApplyBtn = document.querySelector(".filter-apply-btn");
  if (enhancedApplyBtn) {
    enhancedApplyBtn.addEventListener("click", function () {
      // Add loading state
      this.classList.add("loading");

      // Simulate API call
      setTimeout(() => {
        this.classList.remove("loading");
        console.log("Filters applied successfully!");

        // Close filter on mobile after applying
        if (window.innerWidth <= 767 && filterContent) {
          filterContent.classList.remove("active");
          filterToggle.classList.remove("active");
        }
      }, 1500);
    });
  }

  // --- Enhanced Sticky Filter ---
  let lastScrollTop = 0;
  const stickyFilter = document.querySelector(".filter-sidebar");

  // Force sticky positioning detection
  const testSticky = () => {
    const testEl = document.createElement("div");
    testEl.style.position = "sticky";
    document.body.appendChild(testEl);
    const isSticky = testEl.style.position === "sticky";
    document.body.removeChild(testEl);

    if (!isSticky) {
      document.documentElement.classList.add("no-sticky");
    }
  };

  testSticky();

  // Only apply sticky for tablet range (768px - 991px)
  if (stickyFilter && window.innerWidth <= 991 && window.innerWidth >= 768) {
    // Force sticky positioning only for tablet
    const forceSticky = () => {
      if (window.innerWidth <= 991 && window.innerWidth >= 768) {
        stickyFilter.style.position = "sticky";
        stickyFilter.style.position = "-webkit-sticky";
        stickyFilter.style.top = "80px";
        stickyFilter.style.zIndex = "1000";
      } else if (window.innerWidth < 768) {
        // Reset to normal position for mobile
        stickyFilter.style.position = "static";
        stickyFilter.style.top = "auto";
        stickyFilter.style.zIndex = "auto";
      }
    };

    forceSticky();
    window.addEventListener("resize", forceSticky);

    // Only add scroll effects for tablet sticky
    window.addEventListener("scroll", () => {
      const scrollTop =
        window.pageYOffset || document.documentElement.scrollTop;

      // Only apply scroll effects if sticky (tablet range)
      if (window.innerWidth <= 991 && window.innerWidth >= 768) {
        if (scrollTop > 50) {
          stickyFilter.classList.add("scrolled");
          stickyFilter.style.boxShadow = "0 6px 20px rgba(0, 0, 0, 0.2)";
        } else {
          stickyFilter.classList.remove("scrolled");
          stickyFilter.style.boxShadow = "0 4px 15px rgba(0, 0, 0, 0.15)";
        }
      }

      lastScrollTop = scrollTop;
    });

    // Auto-collapse filter when scrolling down on mobile
    if (window.innerWidth <= 767) {
      let scrollTimer = null;

      window.addEventListener("scroll", () => {
        // Clear previous timer
        if (scrollTimer) {
          clearTimeout(scrollTimer);
        }

        // Set timer to collapse filter after scroll stops
        scrollTimer = setTimeout(() => {
          const scrollTop =
            window.pageYOffset || document.documentElement.scrollTop;

          if (
            scrollTop > 200 &&
            filterContent &&
            filterContent.classList.contains("active")
          ) {
            filterContent.classList.remove("active");
            if (filterToggle) {
              filterToggle.classList.remove("active");
            }
          }
        }, 1000); // Collapse after 1 second of no scrolling
      });
    }
  }
});

// Handle sort change
function handleSortChange(select) {
  const sortValue = select.value;
  const currentUrl = window.location.href.split('?')[0];
  const urlParams = new URLSearchParams(window.location.search);
  
  // Lấy các filter hiện tại
  const currentPage = urlParams.get('page') || '1';
  const priceLevel = urlParams.get('priceLevel');
  const loaiPhong = urlParams.getAll('loaiPhong');
  const tienIch = urlParams.getAll('tienIch');
  
  // Tạo URL mới giữ lại tất cả filter
  const newParams = new URLSearchParams();
  newParams.set('sort', sortValue);
  newParams.set('page', currentPage);
  
  if (priceLevel) {
    newParams.set('priceLevel', priceLevel);
  }
  
  loaiPhong.forEach(lp => {
    newParams.append('loaiPhong', lp);
  });
  
  tienIch.forEach(ti => {
    newParams.append('tienIch', ti);
  });
  
  window.location.href = currentUrl + '?' + newParams.toString();
}

// ============================================
// Logic tách biệt 2 bộ lọc: Giá và Loại phòng
// ============================================
document.addEventListener('DOMContentLoaded', function() {
  const filterForm = document.getElementById('filterForm');
  const priceSlider = document.getElementById('priceRange');
  const loaiPhongCheckboxes = document.querySelectorAll('input[name="loaiPhong"]');
  
  if (!filterForm || !priceSlider) return;
  
  // Hàm cập nhật gradient cho slider
  function updatePriceSlider() {
    if (!priceSlider) return;
    
    const priceLevels = [300000, 400000, 600000, 900000];
    const level = parseInt(priceSlider.value);
    const selectedPrice = priceLevels[level];
    
    // Format VND
    function formatVND(amount) {
      return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + "đ";
    }
    
    // Cập nhật giá hiển thị
    const priceValueDisplay = document.querySelector(".price-value");
    if (priceValueDisplay) {
      priceValueDisplay.textContent = `(≤ ${formatVND(selectedPrice)})`;
    }
    
    // Cập nhật gradient cho slider
    const percentage = (level / 3) * 100;
    priceSlider.style.background = `linear-gradient(to right, #d4af37 0%, #d4af37 ${percentage}%, #ddd ${percentage}%, #ddd 100%)`;
  }
  
  // Khi thay đổi slider giá -> Uncheck tất cả checkbox loại phòng
  priceSlider.addEventListener('change', function() {
    loaiPhongCheckboxes.forEach(cb => {
      cb.checked = false;
    });
  });
  
  // Khi check/uncheck loại phòng -> Reset slider về 0 (không lọc giá)
  loaiPhongCheckboxes.forEach(function(checkbox) {
    checkbox.addEventListener('change', function() {
      if (this.checked) {
        // Khi check loại phòng, reset slider về giá trị mặc định
        priceSlider.value = 0;
        // Cập nhật lại gradient và text
        updatePriceSlider();
      }
    });
  });
  
  // Khi submit form, loại bỏ parameter không cần thiết
  filterForm.addEventListener('submit', function(e) {
    const hasLoaiPhong = Array.from(loaiPhongCheckboxes).some(cb => cb.checked);
    
    if (hasLoaiPhong) {
      // Nếu có chọn loại phòng -> Xóa priceLevel khỏi form
      priceSlider.removeAttribute('name');
    } else {
      // Nếu không chọn loại phòng -> Giữ priceLevel
      if (!priceSlider.hasAttribute('name')) {
        priceSlider.setAttribute('name', 'priceLevel');
      }
    }
  });
});
