const states = new WeakMap();

export function init(messagesEl, dotnetRef) {
    const state = {
        messagesEl,
        dotnet: dotnetRef,
        autoScroll: true
    };
    states.set(messagesEl, state);

    messagesEl.addEventListener('scroll', () => {
        const { scrollTop, scrollHeight, clientHeight } = messagesEl;
        state.autoScroll = scrollHeight - scrollTop - clientHeight < 50;
    });
}

export function scrollToEnd(messagesEl, animate) {
    const state = states.get(messagesEl);
    if (!state) return;

    messagesEl.scrollTo({
        top: messagesEl.scrollHeight,
        behavior: animate ? 'smooth' : 'instant'
    });
    state.autoScroll = true;
}

export function scrollToMessage(messagesEl, messageIndex) {
    const state = states.get(messagesEl);
    if (!state) return;

    const wraps = messagesEl.querySelectorAll('.shiny-chat-bubble-wrap');
    if (wraps[messageIndex]) {
        wraps[messageIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
        state.autoScroll = false;
    }
}

export function maintainScrollPosition(messagesEl, previousScrollHeight) {
    const state = states.get(messagesEl);
    if (!state) return;

    const newScrollHeight = messagesEl.scrollHeight;
    messagesEl.scrollTop = newScrollHeight - previousScrollHeight;
}

export function getScrollHeight(messagesEl) {
    return messagesEl ? messagesEl.scrollHeight : 0;
}

export function isNearBottom(messagesEl) {
    if (!messagesEl) return true;
    const { scrollTop, scrollHeight, clientHeight } = messagesEl;
    return scrollHeight - scrollTop - clientHeight < 50;
}

export function dispose(messagesEl) {
    states.delete(messagesEl);
}
