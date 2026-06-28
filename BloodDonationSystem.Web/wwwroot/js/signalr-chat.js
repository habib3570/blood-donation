window.chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .withAutomaticReconnect()
    .build();

window.chatConnection.on("ReceiveMessage", function (message) {
    appendMessage(message, false);
    scrollToBottom();
    window.chatConnection.invoke("MarkAsRead", message.senderId);
});

window.chatConnection.on("MessageSent", function (message) {
    scrollToBottom();
});

window.chatConnection.on("UserTyping", function (userId) {
    showTypingIndicator(userId);
});

window.chatConnection.on("UserStoppedTyping", function (userId) {
    hideTypingIndicator();
});

window.chatConnection.start().catch(console.error);

function appendMessage(message, isMe) {
    const area = document.getElementById('messagesArea');
    if (!area) return;

    const wrapper = document.createElement('div');
    wrapper.className = `d-flex ${isMe ? 'justify-content-end' : 'justify-content-start'}`;
    wrapper.innerHTML = `
        <div style="max-width:70%;">
            <div style="background:${isMe ? 'var(--deep-red)' : 'white'};
                        color:${isMe ? 'white' : 'var(--navy-blue)'};
                        padding:0.65rem 1rem;
                        border-radius:${isMe ? '16px 16px 4px 16px' : '16px 16px 16px 4px'};
                        font-size:0.875rem;
                        box-shadow:0 2px 8px rgba(0,0,0,0.08);">
                ${escapeHtml(message.message)}
            </div>
            <div style="font-size:0.68rem;color:var(--text-muted);margin-top:3px;
                        text-align:${isMe ? 'right' : 'left'};">
                ${formatTime(message.sentAt)}
            </div>
        </div>
    `;
    area.appendChild(wrapper);
}

function scrollToBottom() {
    const area = document.getElementById('messagesArea');
    if (area) area.scrollTop = area.scrollHeight;
}

function showTypingIndicator(userId) {
    let indicator = document.getElementById('typingIndicator');
    if (!indicator) {
        indicator = document.createElement('div');
        indicator.id = 'typingIndicator';
        indicator.style.cssText = 'padding:0.5rem 1rem;font-size:0.78rem;color:var(--text-muted);font-style:italic;';
        indicator.textContent = 'Typing...';
        document.getElementById('messagesArea')?.appendChild(indicator);
    }
    scrollToBottom();
}

function hideTypingIndicator() {
    document.getElementById('typingIndicator')?.remove();
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.appendChild(document.createTextNode(text));
    return div.innerHTML;
}

function formatTime(dateStr) {
    const d = new Date(dateStr);
    return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

// Typing detection
let typingTimer;
document.getElementById('messageInput')?.addEventListener('input', function () {
    const activeUserId = window.activeUserId;
    if (!activeUserId) return;
    window.chatConnection.invoke('StartTyping', activeUserId);
    clearTimeout(typingTimer);
    typingTimer = setTimeout(() => {
        window.chatConnection.invoke('StopTyping', activeUserId);
    }, 2000);
});