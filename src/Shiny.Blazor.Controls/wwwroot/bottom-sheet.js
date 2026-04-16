// Bottom sheet drag/snap controller. One instance per sheet element.
const states = new WeakMap();

function clamp(v, min, max) { return Math.max(min, Math.min(max, v)); }

function applyTransform(sheet, y) {
    sheet.style.transform = `translateY(${y}px)`;
}

function backdropOpacityFor(y, height) {
    const progress = 1 - (y / height);
    return clamp(progress * 0.5, 0, 0.5);
}

function snapInternal(state, ratio, animate) {
    const targetY = state.height * (1 - ratio);
    state.currentY = targetY;
    state.sheet.style.transition = animate ? `transform ${state.duration}ms cubic-bezier(.2,.8,.2,1)` : 'none';
    applyTransform(state.sheet, targetY);
    state.dotnet.invokeMethodAsync('OnBackdropOpacity', backdropOpacityFor(targetY, state.height));
    state.dotnet.invokeMethodAsync('OnDetentChanged', ratio);
}

export function init(root, sheet, handle, dotnetRef, ratios, duration) {
    const state = {
        root, sheet, handle,
        dotnet: dotnetRef,
        ratios: [...ratios].sort((a, b) => a - b),
        duration,
        height: root.clientHeight || window.innerHeight,
        currentY: 0,
        dragStartY: 0,
        dragStartTransform: 0,
        dragging: false,
        pointerId: null,
    };
    states.set(sheet, state);

    state.height = root.clientHeight || window.innerHeight;
    // Start hidden
    applyTransform(sheet, state.height);
    state.currentY = state.height;

    const onResize = () => {
        state.height = root.clientHeight || window.innerHeight;
    };
    window.addEventListener('resize', onResize);
    state.onResize = onResize;

    const onDown = (ev) => {
        if (state.pointerId !== null) return;
        state.pointerId = ev.pointerId;
        state.dragging = true;
        state.dragStartY = ev.clientY;
        state.dragStartTransform = state.currentY;
        sheet.style.transition = 'none';
        handle.setPointerCapture(ev.pointerId);
    };
    const onMove = (ev) => {
        if (!state.dragging || ev.pointerId !== state.pointerId) return;
        const delta = ev.clientY - state.dragStartY;
        const highest = state.height * (1 - state.ratios[state.ratios.length - 1]);
        const newY = clamp(state.dragStartTransform + delta, highest - 20, state.height);
        state.currentY = newY;
        applyTransform(sheet, newY);
        state.dotnet.invokeMethodAsync('OnBackdropOpacity', backdropOpacityFor(newY, state.height));
    };
    const onUp = (ev) => {
        if (!state.dragging || ev.pointerId !== state.pointerId) return;
        state.dragging = false;
        state.pointerId = null;
        try { handle.releasePointerCapture(ev.pointerId); } catch {}

        const totalDelta = ev.clientY - state.dragStartY;
        const lowest = state.height * (1 - state.ratios[0]);
        if (totalDelta > 50 && state.currentY > lowest + state.height * 0.1) {
            state.dotnet.invokeMethodAsync('OnOpenChanged', false);
            return;
        }

        // Find closest detent
        let bestRatio = state.ratios[0];
        let bestDist = Infinity;
        for (const r of state.ratios) {
            const targetY = state.height * (1 - r);
            const dist = Math.abs(state.currentY - targetY);
            if (dist < bestDist) { bestDist = dist; bestRatio = r; }
        }
        snapInternal(state, bestRatio, true);
    };

    handle.addEventListener('pointerdown', onDown);
    handle.addEventListener('pointermove', onMove);
    handle.addEventListener('pointerup', onUp);
    handle.addEventListener('pointercancel', onUp);
    state.onDown = onDown; state.onMove = onMove; state.onUp = onUp;
}

export function open(sheet, ratios) {
    const state = states.get(sheet);
    if (!state) return;
    state.ratios = [...ratios].sort((a, b) => a - b);
    state.height = state.root.clientHeight || window.innerHeight;
    // Start off-screen if not already, then animate to first detent
    sheet.style.transition = 'none';
    applyTransform(sheet, state.height);
    state.currentY = state.height;
    void sheet.offsetHeight; // force reflow
    requestAnimationFrame(() => snapInternal(state, state.ratios[0], true));
}

export function close(sheet) {
    const state = states.get(sheet);
    if (!state) return;
    state.sheet.style.transition = `transform ${state.duration}ms cubic-bezier(.4,.0,.2,1)`;
    applyTransform(sheet, state.height);
    state.currentY = state.height;
    state.dotnet.invokeMethodAsync('OnBackdropOpacity', 0);
}

export function snapTo(sheet, ratio) {
    const state = states.get(sheet);
    if (!state) return;
    snapInternal(state, ratio, true);
}

export function dispose(sheet) {
    const state = states.get(sheet);
    if (!state) return;
    state.handle.removeEventListener('pointerdown', state.onDown);
    state.handle.removeEventListener('pointermove', state.onMove);
    state.handle.removeEventListener('pointerup', state.onUp);
    state.handle.removeEventListener('pointercancel', state.onUp);
    window.removeEventListener('resize', state.onResize);
    states.delete(sheet);
}
