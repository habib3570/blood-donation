// গ্লোবাল ভ্যারিয়েবল - শুরুতেই ডিক্লেয়ার করা, যাতে সব ফাংশন থেকে access করা যায়
let notifConnection = null;

if (document.body.dataset.authenticated === "true") {

    notifConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/notification")
        .withAutomaticReconnect()
        .build();

    notifConnection.on("ReceiveNotification", function (notification) {
        updateNotifBadge();
        showNotifToast(notification);

        if (notification.type === 0 || notification.type === 1) {
            playNotifSound();
        }
    });

    notifConnection.on("EmergencyAlert", function (alert) {
        showEmergencyAlert(alert);
        playEmergencySound();
    });

    notifConnection.start().then(function () {
        notifConnection.invoke("JoinEmergencyGroup");
    }).catch(function (err) {
        console.warn("Notification hub connection failed:", err);
    });
}

function updateNotifBadge() {
    if (document.body.dataset.authenticated !== "true") return;

    fetch('/Notification/GetUnreadCount')
        .then(r => {
            if (!r.ok) throw new Error('Not authenticated');
            return r.json();
        })
        .then(data => {
            const dot = document.getElementById('notifDot');
            if (dot) {
                if (data.count > 0) dot.classList.remove('d-none');
                else dot.classList.add('d-none');
            }
        }).catch(() => { });
}

function showNotifToast(notification) {
    const toast = document.createElement('div');
    toast.style.cssText = `
        position:fixed;bottom:1.5rem;right:1.5rem;
        background:white;border:1px solid var(--card-border);
        border-left:4px solid var(--deep-red);
        border-radius:12px;padding:1rem 1.25rem;
        box-shadow:0 8px 24px rgba(0,0,0,0.12);
        z-index:9999;max-width:320px;
    `;
    toast.innerHTML = `
        <div style="font-weight:700;color:var(--navy-blue);font-size:0.875rem;margin-bottom:0.25rem;">
            ${notification.title}
        </div>
        <div style="font-size:0.8rem;color:var(--text-muted);">
            ${notification.message}
        </div>
    `;
    document.body.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.5s';
        setTimeout(() => toast.remove(), 500);
    }, 5000);
}

function showEmergencyAlert(alert) {
    const modal = document.createElement('div');
    modal.style.cssText = `
        position:fixed;inset:0;
        background:rgba(0,0,0,0.7);
        z-index:9999;display:flex;
        align-items:center;justify-content:center;
    `;
    modal.innerHTML = `
        <div style="background:white;border-radius:20px;
                    padding:2rem;max-width:400px;width:90%;
                    border-top:6px solid var(--deep-red);text-align:center;">
            <div style="font-size:2.5rem;margin-bottom:0.5rem;">🚨</div>
            <div style="font-size:1.3rem;font-weight:800;color:var(--deep-red);margin-bottom:0.5rem;">
                EMERGENCY ALERT
            </div>
            <div style="font-size:0.9rem;color:var(--navy-blue);margin-bottom:1.5rem;">
                ${alert.message || 'Emergency blood needed nearby!'}
            </div>
            <div class="d-flex gap-2 justify-content-center">
                <a href="/Emergency/Index" style="background:var(--deep-red);color:white;
                    padding:0.75rem 1.5rem;border-radius:10px;text-decoration:none;font-weight:700;">
                    View Emergency
                </a>
                <button onclick="this.closest('div[style]').remove()"
                        style="background:var(--light-gray);color:var(--navy-blue);
                               padding:0.75rem 1.5rem;border-radius:10px;
                               border:none;cursor:pointer;font-weight:600;">
                    Dismiss
                </button>
            </div>
        </div>
    `;
    document.body.appendChild(modal);
}

function playNotifSound() {
    try {
        const ctx = new (window.AudioContext || window.webkitAudioContext)();
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.frequency.value = 880;
        gain.gain.setValueAtTime(0.3, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.3);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.3);
    } catch (e) { }
}

function playEmergencySound() {
    try {
        const ctx = new (window.AudioContext || window.webkitAudioContext)();
        [0, 0.3, 0.6].forEach(t => {
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            osc.frequency.value = 660;
            gain.gain.setValueAtTime(0.5, ctx.currentTime + t);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + t + 0.25);
            osc.start(ctx.currentTime + t);
            osc.stop(ctx.currentTime + t + 0.25);
        });
    } catch (e) { }
}

window.addEventListener('load', updateNotifBadge);