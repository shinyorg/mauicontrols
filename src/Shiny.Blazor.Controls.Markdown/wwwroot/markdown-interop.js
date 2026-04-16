// Hooks every <a> inside the host element so .NET handles the click instead of the browser.
export function attachLinks(host, dotnetRef) {
    if (!host) return;

    const handler = (event) => {
        const anchor = event.target.closest('a');
        if (!anchor || !host.contains(anchor)) return;
        const href = anchor.getAttribute('href');
        if (!href) return;
        event.preventDefault();
        dotnetRef.invokeMethodAsync('OnLinkClicked', href);
    };

    if (host.__shinyMdHandler) {
        host.removeEventListener('click', host.__shinyMdHandler);
    }
    host.__shinyMdHandler = handler;
    host.addEventListener('click', handler);
}
