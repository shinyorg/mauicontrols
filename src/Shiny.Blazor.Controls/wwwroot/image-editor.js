const states = new WeakMap();

export function init(root, canvas, dotnetRef, options) {
    const ctx = canvas.getContext('2d');
    const state = {
        root, canvas, ctx, dotnet: dotnetRef,
        image: null,
        actions: [],
        redoStack: [],
        mode: 'none',
        viewTransform: { scale: 1, tx: 0, ty: 0 },
        // Crop
        cropRect: null, // { x, y, w, h } normalized 0-1
        activeCropHandle: null,
        cropDragStart: null,
        cropStartRect: null,
        // Draw
        currentStroke: null,
        drawColor: options?.drawColor || '#ff0000',
        drawWidth: options?.drawWidth || 3,
        // Text
        textColor: options?.textColor || '#ffffff',
        textSize: options?.textSize || 16,
        textFont: options?.textFont || 'Arial',
        // Zoom
        pointers: new Map(),
        pinchStartDist: 0,
        pinchStartScale: 1,
        panStart: null,
        viewStart: null,
        allowZoom: options?.allowZoom !== false,
        // Image rect cache
        imageRect: { x: 0, y: 0, w: 0, h: 0 }
    };

    states.set(root, state);

    canvas.addEventListener('pointerdown', e => onPointerDown(state, e));
    canvas.addEventListener('pointermove', e => onPointerMove(state, e));
    canvas.addEventListener('pointerup', e => onPointerUp(state, e));
    canvas.addEventListener('pointercancel', e => onPointerUp(state, e));

    resizeCanvas(state);
    const observer = new ResizeObserver(() => { resizeCanvas(state); redraw(state); });
    observer.observe(root);
    state._observer = observer;
}

export function loadImage(root, src) {
    const state = states.get(root);
    if (!state) return;

    const img = new Image();
    img.crossOrigin = 'anonymous';
    img.onload = () => { state.image = img; redraw(state); };
    img.src = src;
}

export function loadImageData(root, bytes) {
    const state = states.get(root);
    if (!state) return;

    const blob = new Blob([bytes]);
    const url = URL.createObjectURL(blob);
    const img = new Image();
    img.onload = () => {
        URL.revokeObjectURL(url);
        state.image = img;
        redraw(state);
    };
    img.src = url;
}

export function setMode(root, mode) {
    const state = states.get(root);
    if (!state) return;

    finalizeOperation(state);
    state.mode = mode;

    if (mode === 'crop') {
        state.cropRect = { x: 0, y: 0, w: 1, h: 1 };
    } else {
        state.cropRect = null;
    }
    redraw(state);
}

export function undo(root) {
    const state = states.get(root);
    if (!state || state.actions.length === 0) return;

    const action = state.actions.pop();
    state.redoStack.push(action);
    redraw(state);
    notifyUndoState(state);
}

export function redo(root) {
    const state = states.get(root);
    if (!state || state.redoStack.length === 0) return;

    const action = state.redoStack.pop();
    state.actions.push(action);
    redraw(state);
    notifyUndoState(state);
}

export function rotate(root, degrees) {
    const state = states.get(root);
    if (!state) return;

    state.actions.push({ type: 'rotate', angle: degrees });
    state.redoStack = [];
    redraw(state);
    notifyUndoState(state);
}

export function reset(root) {
    const state = states.get(root);
    if (!state) return;

    state.actions = [];
    state.redoStack = [];
    state.cropRect = null;
    state.currentStroke = null;
    state.mode = 'none';
    state.viewTransform = { scale: 1, tx: 0, ty: 0 };
    redraw(state);
    notifyUndoState(state);
}

export function applyCrop(root) {
    const state = states.get(root);
    if (!state || !state.cropRect) return;

    const c = state.cropRect;
    if (c.x < 0.01 && c.y < 0.01 && c.w > 0.98 && c.h > 0.98) {
        state.cropRect = null;
        state.mode = 'none';
        redraw(state);
        return;
    }

    state.actions.push({ type: 'crop', rect: { ...c } });
    state.redoStack = [];
    state.cropRect = null;
    state.mode = 'none';
    redraw(state);
    notifyUndoState(state);
}

export function updateDrawSettings(root, color, width) {
    const state = states.get(root);
    if (!state) return;
    state.drawColor = color;
    state.drawWidth = width;
}

export function updateTextSettings(root, color, size, font) {
    const state = states.get(root);
    if (!state) return;
    state.textColor = color;
    state.textSize = size;
    state.textFont = font;
}

export function updateAllowZoom(root, allow) {
    const state = states.get(root);
    if (state) state.allowZoom = allow;
}

export async function exportImage(root, format, quality, targetWidth, targetHeight) {
    const state = states.get(root);
    if (!state || !state.image) return new Uint8Array(0);

    const w = targetWidth || state.image.naturalWidth;
    const h = targetHeight || state.image.naturalHeight;

    const offscreen = document.createElement('canvas');
    offscreen.width = w;
    offscreen.height = h;
    const ctx = offscreen.getContext('2d');

    // Replay all actions at export resolution (no view transform)
    replayActions(ctx, state.image, state.actions, w, h);

    const mimeType = format === 'jpeg' ? 'image/jpeg'
        : format === 'webp' ? 'image/webp'
        : 'image/png';

    const blob = await new Promise(resolve => offscreen.toBlob(resolve, mimeType, quality));
    const buf = await blob.arrayBuffer();
    return new Uint8Array(buf);
}

export function dispose(root) {
    const state = states.get(root);
    if (!state) return;

    state.canvas.removeEventListener('pointerdown', onPointerDown);
    state.canvas.removeEventListener('pointermove', onPointerMove);
    state.canvas.removeEventListener('pointerup', onPointerUp);
    state.canvas.removeEventListener('pointercancel', onPointerUp);
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
    ctx.clearRect(0, 0, w, h);
    ctx.fillStyle = '#000';
    ctx.fillRect(0, 0, w, h);

    if (!state.image) {
        ctx.restore();
        return;
    }

    // Calculate fit rect
    const ir = calculateFitRect(state.image.naturalWidth, state.image.naturalHeight, w, h);
    state.imageRect = ir;

    // Apply view transform
    const cx = w / 2, cy = h / 2;
    const vt = state.viewTransform;
    ctx.translate(cx + vt.tx, cy + vt.ty);
    ctx.scale(vt.scale, vt.scale);
    ctx.translate(-cx, -cy);

    // Draw image with all committed actions
    replayActions(ctx, state.image, state.actions, w, h, ir);

    // Draw in-progress crop overlay
    if (state.mode === 'crop' && state.cropRect) {
        drawCropOverlay(ctx, state.cropRect, ir);
    }

    // Draw in-progress stroke
    if (state.mode === 'draw' && state.currentStroke && state.currentStroke.points.length >= 2) {
        drawStroke(ctx, state.currentStroke.points, state.drawColor, state.drawWidth);
    }

    ctx.restore();
}

function replayActions(ctx, image, actions, canvasW, canvasH, ir) {
    if (!ir) {
        ir = calculateFitRect(image.naturalWidth, image.naturalHeight, canvasW, canvasH);
    }

    let currentRect = { ...ir };
    let cumulativeRotation = 0;

    // First pass: compute effective crop & rotation
    for (const action of actions) {
        if (action.type === 'rotate') {
            cumulativeRotation = (cumulativeRotation + action.angle) % 360;
            if (cumulativeRotation < 0) cumulativeRotation += 360;
        } else if (action.type === 'crop') {
            const c = action.rect;
            currentRect = {
                x: currentRect.x + c.x * currentRect.w,
                y: currentRect.y + c.y * currentRect.h,
                w: c.w * currentRect.w,
                h: c.h * currentRect.h
            };
        }
    }

    // Draw image with rotation
    const needsSwap = Math.abs(cumulativeRotation % 180 - 90) < 0.1;
    let drawRect = currentRect;
    if (needsSwap) {
        const nw = currentRect.h, nh = currentRect.w;
        drawRect = {
            x: currentRect.x + currentRect.w / 2 - nw / 2,
            y: currentRect.y + currentRect.h / 2 - nh / 2,
            w: nw, h: nh
        };
    }

    const cx = drawRect.x + drawRect.w / 2;
    const cy = drawRect.y + drawRect.h / 2;

    if (Math.abs(cumulativeRotation) > 0.1) {
        ctx.save();
        ctx.translate(cx, cy);
        ctx.rotate(cumulativeRotation * Math.PI / 180);
        ctx.translate(-cx, -cy);

        if (needsSwap) {
            const ux = cx - currentRect.w / 2;
            const uy = cy - currentRect.h / 2;
            ctx.drawImage(image, ux, uy, currentRect.w, currentRect.h);
        } else {
            ctx.drawImage(image, drawRect.x, drawRect.y, drawRect.w, drawRect.h);
        }
        ctx.restore();
    } else {
        ctx.drawImage(image, drawRect.x, drawRect.y, drawRect.w, drawRect.h);
    }

    // Second pass: draw overlays (strokes, text)
    for (const action of actions) {
        if (action.type === 'draw') {
            const pts = action.points.map(p => ({
                x: drawRect.x + p.x * drawRect.w,
                y: drawRect.y + p.y * drawRect.h
            }));
            drawStroke(ctx, pts, action.color, action.width);
        } else if (action.type === 'text') {
            const tx = drawRect.x + action.position.x * drawRect.w;
            const ty = drawRect.y + action.position.y * drawRect.h;
            ctx.font = `${action.size}px ${action.font}`;
            ctx.fillStyle = action.color;
            ctx.textBaseline = 'top';
            ctx.fillText(action.text, tx, ty);
        }
    }
}

function drawCropOverlay(ctx, crop, ir) {
    const cx = ir.x + crop.x * ir.w;
    const cy = ir.y + crop.y * ir.h;
    const cw = crop.w * ir.w;
    const ch = crop.h * ir.h;

    // Dim overlay (4 rects around crop)
    ctx.fillStyle = 'rgba(0,0,0,0.5)';
    ctx.fillRect(ir.x, ir.y, ir.w, cy - ir.y); // top
    ctx.fillRect(ir.x, cy + ch, ir.w, ir.y + ir.h - cy - ch); // bottom
    ctx.fillRect(ir.x, cy, cx - ir.x, ch); // left
    ctx.fillRect(cx + cw, cy, ir.x + ir.w - cx - cw, ch); // right

    // Crop border
    ctx.strokeStyle = '#fff';
    ctx.lineWidth = 2;
    ctx.strokeRect(cx, cy, cw, ch);

    // Rule of thirds
    ctx.strokeStyle = 'rgba(255,255,255,0.3)';
    ctx.lineWidth = 1;
    const tw = cw / 3, th = ch / 3;
    for (let i = 1; i <= 2; i++) {
        ctx.beginPath();
        ctx.moveTo(cx + tw * i, cy); ctx.lineTo(cx + tw * i, cy + ch);
        ctx.stroke();
        ctx.beginPath();
        ctx.moveTo(cx, cy + th * i); ctx.lineTo(cx + cw, cy + th * i);
        ctx.stroke();
    }

    // Handles
    ctx.fillStyle = '#fff';
    const hs = 10;
    const handles = [
        [cx, cy], [cx + cw / 2, cy], [cx + cw, cy],
        [cx, cy + ch / 2], [cx + cw, cy + ch / 2],
        [cx, cy + ch], [cx + cw / 2, cy + ch], [cx + cw, cy + ch]
    ];
    for (const [hx, hy] of handles) {
        ctx.fillRect(hx - hs / 2, hy - hs / 2, hs, hs);
    }
}

function drawStroke(ctx, points, color, width) {
    if (points.length < 2) return;
    ctx.strokeStyle = color;
    ctx.lineWidth = width;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    for (let i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].x, points[i].y);
    }
    ctx.stroke();
}

function calculateFitRect(imgW, imgH, canvasW, canvasH) {
    if (imgW <= 0 || imgH <= 0) return { x: 0, y: 0, w: 0, h: 0 };
    const scale = Math.min(canvasW / imgW, canvasH / imgH);
    const w = imgW * scale, h = imgH * scale;
    return { x: (canvasW - w) / 2, y: (canvasH - h) / 2, w, h };
}

function finalizeOperation(state) {
    // Commit in-progress stroke
    if (state.currentStroke && state.currentStroke.points.length >= 2) {
        const ir = state.imageRect;
        if (ir.w > 0 && ir.h > 0) {
            const normalized = state.currentStroke.points.map(p => ({
                x: (p.x - ir.x) / ir.w,
                y: (p.y - ir.y) / ir.h
            }));
            state.actions.push({
                type: 'draw',
                points: normalized,
                color: state.drawColor,
                width: state.drawWidth
            });
            state.redoStack = [];
            notifyUndoState(state);
        }
        state.currentStroke = null;
    }
}

function notifyUndoState(state) {
    state.dotnet.invokeMethodAsync('OnCanUndoChanged', state.actions.length > 0);
    state.dotnet.invokeMethodAsync('OnCanRedoChanged', state.redoStack.length > 0);
}

// --- Pointer events ---

function onPointerDown(state, e) {
    e.preventDefault();
    state.canvas.setPointerCapture(e.pointerId);
    state.pointers.set(e.pointerId, { x: e.offsetX, y: e.offsetY });

    const pt = { x: e.offsetX, y: e.offsetY };

    if (state.pointers.size === 2 && state.mode === 'none' && state.allowZoom) {
        // Start pinch
        const pts = [...state.pointers.values()];
        state.pinchStartDist = dist(pts[0], pts[1]);
        state.pinchStartScale = state.viewTransform.scale;
        return;
    }

    switch (state.mode) {
        case 'none':
            if (state.viewTransform.scale > 1.05) {
                state.panStart = pt;
                state.viewStart = { tx: state.viewTransform.tx, ty: state.viewTransform.ty };
            }
            break;
        case 'crop':
            startCropDrag(state, pt);
            break;
        case 'draw':
            state.currentStroke = { points: [pt] };
            redraw(state);
            break;
        case 'text':
            handleTextPlacement(state, pt);
            break;
    }
}

function onPointerMove(state, e) {
    e.preventDefault();
    const pt = { x: e.offsetX, y: e.offsetY };
    state.pointers.set(e.pointerId, pt);

    // Pinch zoom
    if (state.pointers.size === 2 && state.mode === 'none' && state.allowZoom) {
        const pts = [...state.pointers.values()];
        const d = dist(pts[0], pts[1]);
        if (state.pinchStartDist > 0) {
            state.viewTransform.scale = clamp(state.pinchStartScale * (d / state.pinchStartDist), 1, 5);
            redraw(state);
        }
        return;
    }

    switch (state.mode) {
        case 'none':
            if (state.panStart && state.viewStart) {
                state.viewTransform.tx = state.viewStart.tx + (pt.x - state.panStart.x);
                state.viewTransform.ty = state.viewStart.ty + (pt.y - state.panStart.y);
                redraw(state);
            }
            break;
        case 'crop':
            moveCropDrag(state, pt);
            break;
        case 'draw':
            if (state.currentStroke) {
                state.currentStroke.points.push(pt);
                redraw(state);
            }
            break;
    }
}

function onPointerUp(state, e) {
    e.preventDefault();
    state.canvas.releasePointerCapture(e.pointerId);
    state.pointers.delete(e.pointerId);

    switch (state.mode) {
        case 'none':
            state.panStart = null;
            state.viewStart = null;
            if (state.viewTransform.scale <= 1.05) {
                state.viewTransform = { scale: 1, tx: 0, ty: 0 };
                redraw(state);
            }
            break;
        case 'crop':
            state.activeCropHandle = null;
            break;
        case 'draw':
            if (state.currentStroke && state.currentStroke.points.length >= 2) {
                finalizeOperation(state);
                redraw(state);
            } else {
                state.currentStroke = null;
            }
            break;
    }
}

// --- Crop drag ---

function startCropDrag(state, pt) {
    if (!state.cropRect) return;
    const ir = state.imageRect;
    const cr = {
        x: ir.x + state.cropRect.x * ir.w,
        y: ir.y + state.cropRect.y * ir.h,
        w: state.cropRect.w * ir.w,
        h: state.cropRect.h * ir.h
    };

    state.activeCropHandle = hitTestCropHandle(pt, cr);
    state.cropDragStart = pt;
    state.cropStartRect = { ...state.cropRect };
}

function moveCropDrag(state, pt) {
    if (!state.activeCropHandle || !state.cropDragStart || !state.cropStartRect) return;
    const ir = state.imageRect;
    if (ir.w <= 0 || ir.h <= 0) return;

    const dx = (pt.x - state.cropDragStart.x) / ir.w;
    const dy = (pt.y - state.cropDragStart.y) / ir.h;
    const c = state.cropStartRect;
    const minSize = 0.05;

    let nc = { ...c };

    switch (state.activeCropHandle) {
        case 'move':
            nc.x = clamp(c.x + dx, 0, 1 - c.w);
            nc.y = clamp(c.y + dy, 0, 1 - c.h);
            break;
        case 'tl': nc = resizeCrop(c, dx, dy, 0, 0, minSize); break;
        case 'tc': nc = resizeCrop(c, 0, dy, 0, 0, minSize); break;
        case 'tr': nc = resizeCrop(c, 0, dy, dx, 0, minSize); break;
        case 'ml': nc = resizeCrop(c, dx, 0, 0, 0, minSize); break;
        case 'mr': nc = resizeCrop(c, 0, 0, dx, 0, minSize); break;
        case 'bl': nc = resizeCrop(c, dx, 0, 0, dy, minSize); break;
        case 'bc': nc = resizeCrop(c, 0, 0, 0, dy, minSize); break;
        case 'br': nc = resizeCrop(c, 0, 0, dx, dy, minSize); break;
    }

    state.cropRect = nc;
    redraw(state);
}

function resizeCrop(c, dLeft, dTop, dRight, dBottom, minSize) {
    let x = c.x + dLeft, y = c.y + dTop;
    let w = c.w - dLeft + dRight, h = c.h - dTop + dBottom;
    if (w < minSize) { w = minSize; x = c.x + c.w - minSize; }
    if (h < minSize) { h = minSize; y = c.y + c.h - minSize; }
    x = clamp(x, 0, 1 - minSize);
    y = clamp(y, 0, 1 - minSize);
    w = Math.min(w, 1 - x);
    h = Math.min(h, 1 - y);
    return { x, y, w, h };
}

function hitTestCropHandle(pt, cr) {
    const r = 20;
    const cx = cr.x + cr.w / 2, cy = cr.y + cr.h / 2;

    if (dist(pt, { x: cr.x, y: cr.y }) < r) return 'tl';
    if (dist(pt, { x: cr.x + cr.w, y: cr.y }) < r) return 'tr';
    if (dist(pt, { x: cr.x, y: cr.y + cr.h }) < r) return 'bl';
    if (dist(pt, { x: cr.x + cr.w, y: cr.y + cr.h }) < r) return 'br';
    if (dist(pt, { x: cx, y: cr.y }) < r) return 'tc';
    if (dist(pt, { x: cx, y: cr.y + cr.h }) < r) return 'bc';
    if (dist(pt, { x: cr.x, y: cy }) < r) return 'ml';
    if (dist(pt, { x: cr.x + cr.w, y: cy }) < r) return 'mr';

    if (pt.x >= cr.x && pt.x <= cr.x + cr.w && pt.y >= cr.y && pt.y <= cr.y + cr.h)
        return 'move';

    return null;
}

async function handleTextPlacement(state, pt) {
    const ir = state.imageRect;
    if (ir.w <= 0 || ir.h <= 0) return;
    if (pt.x < ir.x || pt.x > ir.x + ir.w || pt.y < ir.y || pt.y > ir.y + ir.h) return;

    const text = await state.dotnet.invokeMethodAsync('OnPromptText');
    if (!text) return;

    const normalized = {
        x: (pt.x - ir.x) / ir.w,
        y: (pt.y - ir.y) / ir.h
    };

    state.actions.push({
        type: 'text',
        text,
        position: normalized,
        size: state.textSize,
        color: state.textColor,
        font: state.textFont
    });
    state.redoStack = [];
    redraw(state);
    notifyUndoState(state);
}

function dist(a, b) {
    return Math.sqrt((a.x - b.x) ** 2 + (a.y - b.y) ** 2);
}

function clamp(v, mn, mx) {
    return Math.max(mn, Math.min(mx, v));
}
