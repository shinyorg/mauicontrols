const states = new WeakMap();

export function init(root, canvas, dotnetRef, options) {
    const ctx = canvas.getContext('2d');
    const state = {
        root, canvas, ctx,
        dotnet: dotnetRef,
        strokes: [],
        activeStroke: null,
        strokeColor: options?.strokeColor || '#000000',
        strokeWidth: options?.strokeWidth || 3,
        backgroundColor: options?.backgroundColor || '#FFFFFF',
        pointerId: null
    };
    states.set(root, state);

    canvas.addEventListener('pointerdown', e => onPointerDown(state, e));
    canvas.addEventListener('pointermove', e => onPointerMove(state, e));
    canvas.addEventListener('pointerup', e => onPointerUp(state, e));
    canvas.addEventListener('pointercancel', e => onPointerUp(state, e));

    resizeCanvas(state);
    redraw(state);

    const observer = new ResizeObserver(() => { resizeCanvas(state); redraw(state); });
    observer.observe(root);
    state._observer = observer;
}

export function updateSettings(root, strokeColor, strokeWidth, backgroundColor) {
    const state = states.get(root);
    if (!state) return;
    state.strokeColor = strokeColor;
    state.strokeWidth = strokeWidth;
    state.backgroundColor = backgroundColor;
    redraw(state);
}

export function clear(root) {
    const state = states.get(root);
    if (!state) return;
    state.strokes = [];
    state.activeStroke = null;
    redraw(state);
}

export async function exportPng(root, width, height) {
    const state = states.get(root);
    if (!state || state.strokes.length === 0) return new Uint8Array(0);

    const offscreen = document.createElement('canvas');
    offscreen.width = width;
    offscreen.height = height;
    const ctx = offscreen.getContext('2d');

    // Fill background
    ctx.fillStyle = state.backgroundColor;
    ctx.fillRect(0, 0, width, height);

    // Draw all strokes scaled to export resolution
    for (const stroke of state.strokes) {
        drawStroke(ctx, stroke.points, stroke.color, stroke.width, width, height);
    }

    const blob = await new Promise(resolve => offscreen.toBlob(resolve, 'image/png'));
    const buf = await blob.arrayBuffer();
    return new Uint8Array(buf);
}

export function dispose(root) {
    const state = states.get(root);
    if (!state) return;
    state._observer?.disconnect();
    states.delete(root);
}

// --- Internal ---

function resizeCanvas(state) {
    const rect = state.root.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;
    state.canvas.width = rect.width * dpr;
    state.canvas.height = rect.height * dpr;
    state.canvas.style.width = rect.width + 'px';
    state.canvas.style.height = rect.height + 'px';
    state.ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
}

function redraw(state) {
    const { ctx, canvas } = state;
    const dpr = window.devicePixelRatio || 1;
    const w = canvas.width / dpr;
    const h = canvas.height / dpr;

    ctx.save();
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);

    // Fill background
    ctx.fillStyle = state.backgroundColor;
    ctx.fillRect(0, 0, w, h);

    // Draw committed strokes (normalized 0-1 coordinates)
    for (const stroke of state.strokes) {
        drawStroke(ctx, stroke.points, stroke.color, stroke.width, w, h);
    }

    // Draw active stroke (pixel coordinates)
    if (state.activeStroke && state.activeStroke.length >= 2) {
        ctx.strokeStyle = state.strokeColor;
        ctx.lineWidth = state.strokeWidth;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.beginPath();
        ctx.moveTo(state.activeStroke[0].x, state.activeStroke[0].y);
        for (let i = 1; i < state.activeStroke.length; i++) {
            ctx.lineTo(state.activeStroke[i].x, state.activeStroke[i].y);
        }
        ctx.stroke();
    }

    ctx.restore();
}

function drawStroke(ctx, points, color, width, canvasW, canvasH) {
    if (points.length < 2) return;
    ctx.strokeStyle = color;
    ctx.lineWidth = width;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    ctx.beginPath();
    ctx.moveTo(points[0].x * canvasW, points[0].y * canvasH);
    for (let i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].x * canvasW, points[i].y * canvasH);
    }
    ctx.stroke();
}

function onPointerDown(state, e) {
    if (state.pointerId !== null) return;
    e.preventDefault();
    state.pointerId = e.pointerId;
    state.canvas.setPointerCapture(e.pointerId);
    state.activeStroke = [{ x: e.offsetX, y: e.offsetY }];
}

function onPointerMove(state, e) {
    if (e.pointerId !== state.pointerId || !state.activeStroke) return;
    e.preventDefault();
    state.activeStroke.push({ x: e.offsetX, y: e.offsetY });
    redraw(state);
}

function onPointerUp(state, e) {
    if (e.pointerId !== state.pointerId) return;
    e.preventDefault();
    try { state.canvas.releasePointerCapture(e.pointerId); } catch {}
    state.pointerId = null;

    if (state.activeStroke && state.activeStroke.length >= 2) {
        const dpr = window.devicePixelRatio || 1;
        const w = state.canvas.width / dpr;
        const h = state.canvas.height / dpr;

        // Normalize to 0-1
        const normalized = state.activeStroke.map(p => ({
            x: p.x / w,
            y: p.y / h
        }));

        state.strokes.push({
            points: normalized,
            color: state.strokeColor,
            width: state.strokeWidth
        });

        state.dotnet.invokeMethodAsync('OnHasSignatureChanged', true);
    }

    state.activeStroke = null;
    redraw(state);
}
