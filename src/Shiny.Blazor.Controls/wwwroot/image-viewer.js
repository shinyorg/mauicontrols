// Image viewer: pinch to zoom, pan when zoomed, double-tap zoom toggle.
const states = new WeakMap();

function clamp(v, mn, mx) { return Math.max(mn, Math.min(mx, v)); }

function apply(state, animate) {
    state.img.style.transition = animate ? 'transform 250ms cubic-bezier(.2,.8,.2,1)' : 'none';
    state.img.style.transform = `translate(${state.tx}px, ${state.ty}px) scale(${state.scale})`;
}

function clampPan(state) {
    if (state.scale <= 1) { state.tx = 0; state.ty = 0; return; }
    const w = state.img.clientWidth;
    const h = state.img.clientHeight;
    const maxX = w * (state.scale - 1) / 2;
    const maxY = h * (state.scale - 1) / 2;
    state.tx = clamp(state.tx, -maxX, maxX);
    state.ty = clamp(state.ty, -maxY, maxY);
}

export function init(root, backdrop, img, dotnetRef, maxZoom) {
    const state = {
        root, backdrop, img,
        dotnet: dotnetRef,
        maxZoom,
        scale: 1, tx: 0, ty: 0,
        pointers: new Map(),
        pinchStartDistance: 0,
        pinchStartScale: 1,
        panStartX: 0, panStartY: 0,
        panOriginTx: 0, panOriginTy: 0,
        lastTapTime: 0,
        lastTapX: 0, lastTapY: 0,
    };
    states.set(root, state);

    const onDown = (ev) => {
        state.pointers.set(ev.pointerId, { x: ev.clientX, y: ev.clientY });
        img.setPointerCapture(ev.pointerId);

        if (state.pointers.size === 2) {
            const pts = [...state.pointers.values()];
            state.pinchStartDistance = Math.hypot(pts[0].x - pts[1].x, pts[0].y - pts[1].y);
            state.pinchStartScale = state.scale;
        } else if (state.pointers.size === 1) {
            state.panStartX = ev.clientX;
            state.panStartY = ev.clientY;
            state.panOriginTx = state.tx;
            state.panOriginTy = state.ty;

            // Double tap detection
            const now = performance.now();
            const dx = ev.clientX - state.lastTapX;
            const dy = ev.clientY - state.lastTapY;
            if (now - state.lastTapTime < 300 && Math.hypot(dx, dy) < 30) {
                onDoubleTap(state, ev);
                state.lastTapTime = 0;
            } else {
                state.lastTapTime = now;
                state.lastTapX = ev.clientX;
                state.lastTapY = ev.clientY;
            }
        }
    };

    const onMove = (ev) => {
        if (!state.pointers.has(ev.pointerId)) return;
        state.pointers.set(ev.pointerId, { x: ev.clientX, y: ev.clientY });

        if (state.pointers.size === 2) {
            const pts = [...state.pointers.values()];
            const d = Math.hypot(pts[0].x - pts[1].x, pts[0].y - pts[1].y);
            if (state.pinchStartDistance > 0) {
                state.scale = clamp(state.pinchStartScale * (d / state.pinchStartDistance), 1, state.maxZoom);
                clampPan(state);
                apply(state, false);
            }
        } else if (state.pointers.size === 1 && state.scale > 1) {
            state.tx = state.panOriginTx + (ev.clientX - state.panStartX);
            state.ty = state.panOriginTy + (ev.clientY - state.panStartY);
            clampPan(state);
            apply(state, false);
        }
    };

    const onUp = (ev) => {
        state.pointers.delete(ev.pointerId);
        try { img.releasePointerCapture(ev.pointerId); } catch {}
        if (state.scale <= 1.001) {
            state.scale = 1; state.tx = 0; state.ty = 0;
            apply(state, true);
        }
    };

    img.addEventListener('pointerdown', onDown);
    img.addEventListener('pointermove', onMove);
    img.addEventListener('pointerup', onUp);
    img.addEventListener('pointercancel', onUp);

    backdrop.addEventListener('click', () => state.dotnet.invokeMethodAsync('OnRequestClose'));

    state._handlers = { onDown, onMove, onUp };
}

function onDoubleTap(state, ev) {
    if (state.scale > 1) {
        state.scale = 1; state.tx = 0; state.ty = 0;
    } else {
        const target = Math.min(2.5, state.maxZoom);
        const rect = state.img.getBoundingClientRect();
        const cx = rect.left + rect.width / 2;
        const cy = rect.top + rect.height / 2;
        state.tx = -(ev.clientX - cx) * (target - 1);
        state.ty = -(ev.clientY - cy) * (target - 1);
        state.scale = target;
        clampPan(state);
    }
    apply(state, true);
}

export function open(root) {
    const state = states.get(root);
    if (!state) return;
    state.scale = 1; state.tx = 0; state.ty = 0;
    apply(state, false);
}

export function close(root) {
    const state = states.get(root);
    if (!state) return;
    state.scale = 1; state.tx = 0; state.ty = 0;
    apply(state, false);
}

export function dispose(root) {
    const state = states.get(root);
    if (!state) return;
    const { onDown, onMove, onUp } = state._handlers;
    state.img.removeEventListener('pointerdown', onDown);
    state.img.removeEventListener('pointermove', onMove);
    state.img.removeEventListener('pointerup', onUp);
    state.img.removeEventListener('pointercancel', onUp);
    states.delete(root);
}
