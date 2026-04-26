let currentUtterance = null;

export function speak(text, pitch, rate, volume, lang) {
    cancel();
    return new Promise((resolve) => {
        const utterance = new SpeechSynthesisUtterance(text);
        utterance.pitch = pitch;
        utterance.rate = rate;
        utterance.volume = volume;
        if (lang) utterance.lang = lang;
        utterance.onend = () => { currentUtterance = null; resolve(false); };
        utterance.onerror = (e) => { currentUtterance = null; resolve(e.error === 'canceled'); };
        currentUtterance = utterance;
        window.speechSynthesis.speak(utterance);
    });
}

export function cancel() {
    currentUtterance = null;
    window.speechSynthesis.cancel();
}

export function isSpeaking() {
    return window.speechSynthesis.speaking;
}
