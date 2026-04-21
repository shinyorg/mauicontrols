const state = new WeakMap();

export function init(spectrumEl, hueEl, opacityEl, dotnetRef, initialColor, showOpacity) {
    const s = {
        dotnetRef,
        hueEl,
        opacityEl,
        showOpacity,
        hue: 0,
        sat: 1,
        bri: 1,
        alpha: 1,
        dragging: null
    };
    state.set(spectrumEl, s);

    parseColor(s, initialColor);
    drawSpectrum(spectrumEl, s);
    drawHue(hueEl, s);
    if (showOpacity && opacityEl) drawOpacity(opacityEl, s);

    // Spectrum events
    spectrumEl.addEventListener('pointerdown', e => onSpectrumDown(spectrumEl, s, e));
    spectrumEl.addEventListener('pointermove', e => onSpectrumMove(spectrumEl, s, e));
    spectrumEl.addEventListener('pointerup', () => s.dragging = null);
    spectrumEl.addEventListener('pointerleave', () => s.dragging = null);

    // Hue events
    hueEl.addEventListener('pointerdown', e => onHueDown(spectrumEl, s, e));
    hueEl.addEventListener('pointermove', e => onHueMove(spectrumEl, s, e));
    hueEl.addEventListener('pointerup', () => s.dragging = null);
    hueEl.addEventListener('pointerleave', () => s.dragging = null);

    // Opacity events
    if (showOpacity && opacityEl) {
        opacityEl.addEventListener('pointerdown', e => onOpacityDown(spectrumEl, s, e));
        opacityEl.addEventListener('pointermove', e => onOpacityMove(spectrumEl, s, e));
        opacityEl.addEventListener('pointerup', () => s.dragging = null);
        opacityEl.addEventListener('pointerleave', () => s.dragging = null);
    }
}

export function setColor(spectrumEl, hex) {
    const s = state.get(spectrumEl);
    if (!s) return;
    parseColor(s, hex);
    drawSpectrum(spectrumEl, s);
    drawHue(s.hueEl, s);
    if (s.showOpacity && s.opacityEl) drawOpacity(s.opacityEl, s);
}

export function dispose(spectrumEl) {
    state.delete(spectrumEl);
}

// --- Spectrum (saturation/brightness) ---

function onSpectrumDown(el, s, e) {
    s.dragging = 'spectrum';
    el.setPointerCapture(e.pointerId);
    pickSpectrum(el, s, e);
}

function onSpectrumMove(el, s, e) {
    if (s.dragging === 'spectrum') pickSpectrum(el, s, e);
}

function pickSpectrum(el, s, e) {
    const rect = el.getBoundingClientRect();
    s.sat = clamp((e.clientX - rect.left) / rect.width, 0, 1);
    s.bri = 1 - clamp((e.clientY - rect.top) / rect.height, 0, 1);
    drawSpectrum(el, s);
    if (s.showOpacity && s.opacityEl) drawOpacity(s.opacityEl, s);
    notify(s);
}

function drawSpectrum(el, s) {
    const ctx = el.getContext('2d');
    const w = el.width, h = el.height;

    // Hue base fill
    const hueColor = hsvToRgb(s.hue, 1, 1);
    ctx.fillStyle = `rgb(${hueColor[0]},${hueColor[1]},${hueColor[2]})`;
    ctx.fillRect(0, 0, w, h);

    // White → transparent (horizontal, saturation)
    const satGrad = ctx.createLinearGradient(0, 0, w, 0);
    satGrad.addColorStop(0, 'rgba(255,255,255,1)');
    satGrad.addColorStop(1, 'rgba(255,255,255,0)');
    ctx.fillStyle = satGrad;
    ctx.fillRect(0, 0, w, h);

    // Transparent → black (vertical, brightness)
    const briGrad = ctx.createLinearGradient(0, 0, 0, h);
    briGrad.addColorStop(0, 'rgba(0,0,0,0)');
    briGrad.addColorStop(1, 'rgba(0,0,0,1)');
    ctx.fillStyle = briGrad;
    ctx.fillRect(0, 0, w, h);

    // Selector
    const cx = s.sat * w, cy = (1 - s.bri) * h;
    ctx.beginPath();
    ctx.arc(cx, cy, 8, 0, Math.PI * 2);
    ctx.strokeStyle = '#fff';
    ctx.lineWidth = 3;
    ctx.stroke();
    ctx.beginPath();
    ctx.arc(cx, cy, 9, 0, Math.PI * 2);
    ctx.strokeStyle = '#000';
    ctx.lineWidth = 1;
    ctx.stroke();
}

// --- Hue bar ---

function onHueDown(spectrumEl, s, e) {
    s.dragging = 'hue';
    s.hueEl.setPointerCapture(e.pointerId);
    pickHue(spectrumEl, s, e);
}

function onHueMove(spectrumEl, s, e) {
    if (s.dragging === 'hue') pickHue(spectrumEl, s, e);
}

function pickHue(spectrumEl, s, e) {
    const rect = s.hueEl.getBoundingClientRect();
    s.hue = clamp((e.clientX - rect.left) / rect.width, 0, 1) * 360;
    drawSpectrum(spectrumEl, s);
    drawHue(s.hueEl, s);
    if (s.showOpacity && s.opacityEl) drawOpacity(s.opacityEl, s);
    notify(s);
}

function drawHue(el, s) {
    const ctx = el.getContext('2d');
    const w = el.width, h = el.height;

    const grad = ctx.createLinearGradient(0, 0, w, 0);
    for (let i = 0; i <= 6; i++) {
        const rgb = hsvToRgb((i / 6) * 360, 1, 1);
        grad.addColorStop(i / 6, `rgb(${rgb[0]},${rgb[1]},${rgb[2]})`);
    }
    ctx.fillStyle = grad;
    ctx.beginPath();
    ctx.roundRect(0, 0, w, h, 6);
    ctx.fill();

    // Selector
    const cx = (s.hue / 360) * w;
    ctx.beginPath();
    ctx.arc(cx, h / 2, 10, 0, Math.PI * 2);
    ctx.fillStyle = '#fff';
    ctx.fill();
    ctx.strokeStyle = '#000';
    ctx.lineWidth = 1.5;
    ctx.stroke();
}

// --- Opacity bar ---

function onOpacityDown(spectrumEl, s, e) {
    s.dragging = 'opacity';
    s.opacityEl.setPointerCapture(e.pointerId);
    pickOpacity(spectrumEl, s, e);
}

function onOpacityMove(spectrumEl, s, e) {
    if (s.dragging === 'opacity') pickOpacity(spectrumEl, s, e);
}

function pickOpacity(spectrumEl, s, e) {
    const rect = s.opacityEl.getBoundingClientRect();
    s.alpha = clamp((e.clientX - rect.left) / rect.width, 0, 1);
    drawOpacity(s.opacityEl, s);
    notify(s);
}

function drawOpacity(el, s) {
    const ctx = el.getContext('2d');
    const w = el.width, h = el.height;

    ctx.clearRect(0, 0, w, h);

    const rgb = hsvToRgb(s.hue, s.sat, s.bri);
    const grad = ctx.createLinearGradient(0, 0, w, 0);
    grad.addColorStop(0, `rgba(${rgb[0]},${rgb[1]},${rgb[2]},0)`);
    grad.addColorStop(1, `rgba(${rgb[0]},${rgb[1]},${rgb[2]},1)`);
    ctx.fillStyle = grad;
    ctx.fillRect(0, 0, w, h);

    // Selector
    const cx = s.alpha * w;
    ctx.beginPath();
    ctx.arc(cx, h / 2, 8, 0, Math.PI * 2);
    ctx.fillStyle = '#fff';
    ctx.fill();
    ctx.strokeStyle = '#000';
    ctx.lineWidth = 1.5;
    ctx.stroke();
}

// --- Helpers ---

function notify(s) {
    const rgb = hsvToRgb(s.hue, s.sat, s.bri);
    const r = Math.round(rgb[0]), g = Math.round(rgb[1]), b = Math.round(rgb[2]);
    const a = Math.round(s.alpha * 255);

    const hex = a < 255
        ? '#' + toHex(a) + toHex(r) + toHex(g) + toHex(b)
        : '#' + toHex(r) + toHex(g) + toHex(b);

    const css = s.alpha < 1
        ? `rgba(${r},${g},${b},${s.alpha.toFixed(2)})`
        : `rgb(${r},${g},${b})`;

    s.dotnetRef.invokeMethodAsync('OnColorChanged', hex, css);
}

function parseColor(s, hex) {
    if (!hex || hex[0] !== '#') return;

    let r, g, b, a = 255;
    if (hex.length === 7) {
        r = parseInt(hex.substr(1, 2), 16);
        g = parseInt(hex.substr(3, 2), 16);
        b = parseInt(hex.substr(5, 2), 16);
    } else if (hex.length === 9) {
        a = parseInt(hex.substr(1, 2), 16);
        r = parseInt(hex.substr(3, 2), 16);
        g = parseInt(hex.substr(5, 2), 16);
        b = parseInt(hex.substr(7, 2), 16);
    } else {
        return;
    }

    const hsv = rgbToHsv(r, g, b);
    s.hue = hsv[0];
    s.sat = hsv[1];
    s.bri = hsv[2];
    s.alpha = a / 255;
}

function hsvToRgb(h, s, v) {
    const c = v * s;
    const x = c * (1 - Math.abs(((h / 60) % 2) - 1));
    const m = v - c;
    let r, g, b;
    const i = Math.floor(h / 60) % 6;
    switch (i) {
        case 0: r = c; g = x; b = 0; break;
        case 1: r = x; g = c; b = 0; break;
        case 2: r = 0; g = c; b = x; break;
        case 3: r = 0; g = x; b = c; break;
        case 4: r = x; g = 0; b = c; break;
        default: r = c; g = 0; b = x; break;
    }
    return [(r + m) * 255, (g + m) * 255, (b + m) * 255];
}

function rgbToHsv(r, g, b) {
    r /= 255; g /= 255; b /= 255;
    const max = Math.max(r, g, b), min = Math.min(r, g, b);
    const d = max - min;
    let h = 0, s = max === 0 ? 0 : d / max, v = max;

    if (d !== 0) {
        if (max === r) h = 60 * (((g - b) / d) % 6);
        else if (max === g) h = 60 * (((b - r) / d) + 2);
        else h = 60 * (((r - g) / d) + 4);
        if (h < 0) h += 360;
    }
    return [h, s, v];
}

function clamp(v, min, max) {
    return Math.max(min, Math.min(max, v));
}

function toHex(n) {
    return n.toString(16).padStart(2, '0').toUpperCase();
}
