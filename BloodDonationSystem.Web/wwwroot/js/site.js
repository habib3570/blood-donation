// ── DARK MODE ──
function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    const isDark = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDark);
    const icon = document.getElementById('darkModeIcon');
    if (icon) icon.className = isDark ? 'fas fa-sun' : 'fas fa-moon';
}

// ── MOBILE MENU ──
function toggleMobileMenu() {
    const menu = document.getElementById('mobileMenu');
    if (menu) menu.classList.toggle('d-none');
}

// ── USER DROPDOWN ──
function toggleUserMenu() {
    const dropdown = document.getElementById('userDropdown');
    const notifDropdown = document.getElementById('notifDropdown');
    if (notifDropdown) notifDropdown.classList.add('d-none');
    if (dropdown) dropdown.classList.toggle('d-none');
}

// ── NOTIFICATION DROPDOWN ──
const notifBtnEl = document.getElementById('notifBtn');
if (notifBtnEl) {
    notifBtnEl.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const userDropdown = document.getElementById('userDropdown');
        if (userDropdown) userDropdown.classList.add('d-none');
        const dropdown = document.getElementById('notifDropdown');
        if (dropdown) {
            dropdown.classList.toggle('d-none');
            if (!dropdown.classList.contains('d-none')) loadNotifications();
        }
    });
}

async function loadNotifications() {
    try {
        const res = await fetch('/Notification/GetUnreadCount');
        const data = await res.json();
        if (data.count > 0) {
            document.getElementById('notifDot')?.classList.remove('d-none');
        }
    } catch (e) { }
}

async function markAllRead() {
    try {
        await fetch('/Notification/MarkAllAsRead', {
            method: 'POST',
            headers: { 'RequestVerificationToken': getAntiForgeryToken() }
        });
        document.getElementById('notifDot')?.classList.add('d-none');
        document.getElementById('notifDropdown')?.classList.add('d-none');
    } catch (e) { }
}

// ── CLOSE DROPDOWNS ON OUTSIDE CLICK (event delegation দিয়ে নির্ভরযোগ্যভাবে) ──
document.addEventListener('click', function (e) {
    const userMenuWrap = document.getElementById('userMenuWrap');
    const userDropdown = document.getElementById('userDropdown');
    const notifBtn = document.getElementById('notifBtn');
    const notifDropdown = document.getElementById('notifDropdown');

    // যদি ক্লিক userMenuWrap এর বাইরে হয়, dropdown বন্ধ করো
    if (userMenuWrap && !userMenuWrap.contains(e.target)) {
        userDropdown?.classList.add('d-none');
    }

    // যদি ক্লিক notification বাটন/dropdown এর বাইরে হয়, বন্ধ করো
    if (notifBtn && notifDropdown &&
        !notifBtn.contains(e.target) && !notifDropdown.contains(e.target)) {
        notifDropdown.classList.add('d-none');
    }
});

// ── ANTI FORGERY TOKEN ──
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

// ── RADIUS FILTER ──
document.querySelectorAll('.radius-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.radius-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
        const radius = this.dataset.radius;
        const input = document.getElementById('radiusInput');
        if (input && radius) input.value = radius;
    });
});

// ── TOAST AUTO HIDE ──
setTimeout(() => {
    document.querySelectorAll('.toast-custom').forEach(t => {
        t.style.opacity = '0';
        setTimeout(() => t.remove(), 500);
    });
}, 4000);

// ── LOAD DARK MODE ──
window.addEventListener('DOMContentLoaded', function () {
    const isDark = localStorage.getItem('darkMode') === 'true';
    if (isDark) {
        document.body.classList.add('dark-mode');
        const icon = document.getElementById('darkModeIcon');
        if (icon) icon.className = 'fas fa-sun';
    }
});

// ── FAVORITE TOGGLE ──
async function toggleFavorite(donorProfileId, btn) {
    const isFav = btn.dataset.fav === 'true';
    const url = isFav ? '/Donor/RemoveFavorite' : '/Donor/AddFavorite';
    try {
        const res = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: `donorProfileId=${donorProfileId}`
        });

        if (!res.ok) {
            showToast('Please login to add favorites.', 'error');
            return;
        }

        const data = await res.json();
        if (data.success) {
            btn.dataset.fav = isFav ? 'false' : 'true';
            btn.innerHTML = isFav
                ? '<i class="far fa-heart"></i>'
                : '<i class="fas fa-heart" style="color:var(--deep-red)"></i>';
            showToast(isFav ? 'Removed from favorites.' : 'Added to favorites!', 'success');
        } else {
            showToast(data.message || 'Action failed.', 'error');
        }
    } catch (e) {
        showToast('Something went wrong.', 'error');
    }
}

// ── CONFIRM DELETE ──
function confirmAction(message, formId) {
    if (confirm(message)) {
        document.getElementById(formId)?.submit();
    }
}

// ── COPY TO CLIPBOARD ──
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showToast('Copied to clipboard!', 'success');
    });
}

// ── SHOW TOAST ──
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-custom toast-${type}`;
    toast.innerHTML = `<i class="fas fa-${type === 'success' ? 'check' : 'times'}-circle"></i> ${message}`;
    document.body.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 500);
    }, 3500);
}

// ── GPS LOCATION ──
function getCurrentLocation(latInput, lngInput, callback) {
    if (!navigator.geolocation) {
        showToast('Geolocation not supported.', 'error');
        return;
    }
    navigator.geolocation.getCurrentPosition(
        pos => {
            const { latitude, longitude } = pos.coords;
            if (latInput) document.getElementById(latInput).value = latitude;
            if (lngInput) document.getElementById(lngInput).value = longitude;
            if (callback) callback(latitude, longitude);
        },
        () => showToast('Location access denied.', 'error')
    );
}