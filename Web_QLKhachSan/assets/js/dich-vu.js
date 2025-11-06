// Toggle spa menu content
document.addEventListener('DOMContentLoaded', function() {
	var btn = document.getElementById('toggle-spa-menu');
	var content = document.getElementById('spa-menu-content');
	if (btn && content) {
		btn.addEventListener('click', function(e) {
			e.preventDefault();
			if (content.style.display === 'none' || content.style.display === '') {
				content.style.display = 'block';
				btn.textContent = 'Ẩn Menu Spa';
			} else {
				content.style.display = 'none';
				btn.textContent = 'Xem Menu Spa';
			}
		});
	}
});
