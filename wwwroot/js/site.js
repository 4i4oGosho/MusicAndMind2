let currentAudio = null, currentBtn = null;

function makeAudio(src, vol) {
    const a = new Audio(src);
    a.loop = true;
    a.volume = vol ?? 0.3;
    return a;
}

document.addEventListener('click', e => {
    const btn = e.target.closest('.btn.play');
    if (!btn) return;

    const card = btn.closest('.card');
    const src = btn.dataset.src;
    const vol = card.querySelector('.vol');

    if (currentAudio && currentAudio.src.includes(src) && !currentAudio.paused) {
        currentAudio.pause();
        btn.textContent = '▶';
        btn.classList.remove('pause');
        currentAudio = null;
        currentBtn = null;
        return;
    }

    if (currentAudio) {
        currentAudio.pause();
        if (currentBtn) {
            currentBtn.textContent = '▶';
            currentBtn.classList.remove('pause');
        }
    }

    const a = makeAudio(src, parseFloat(vol.value));
    a.play();
    currentAudio = a;
    currentBtn = btn;
    btn.textContent = '⏸';
    btn.classList.add('pause');

    vol.addEventListener('input', () => {
        if (a) a.volume = parseFloat(vol.value);
    }, { once: true });
});

// Theme toggle: inject minimal theme CSS and add toggle button (inserted in header)
(function () {
    const themeCSS = `
/* Light theme variables override (keep readable UI colors) */
body.theme-light {
    --bg: transparent;
    --fg: #0b0b0b;
    --muted: #6b6b6b;
    --line: #e6e6e6;
    --card: rgba(255,255,255,0.9);
}

/* Day background (cherry trees) using provided image only */
body.theme-light {
    background-image: url("/images/sakura-bglight.png");
    background-repeat: no-repeat;
    background-position: center top;
    background-attachment: fixed;
    background-size: cover;
    background-color: transparent;
}

/* Remove any overlay for light theme so image is shown alone */
body.theme-light::after {
    background: transparent;
}

/* Toggle button styling for header placement */
.mm-theme-toggle {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 8px 12px;
    border-radius: 999px;
    border: 1px solid var(--line);
    background: rgba(11,11,11,0.06);
    color: var(--fg);
    cursor: pointer;
    backdrop-filter: blur(6px);
    font-weight: 700;
    box-shadow: 0 6px 18px rgba(0,0,0,0.10);
    font-size: 14px;
    height: 40px;
}

/* Light theme button colors */
body.theme-light .mm-theme-toggle {
    background: linear-gradient(180deg,#fff,#f7f7f7);
    color: #111;
    border-color: #e6e6e6;
}

/* Small screens: compact button */
@media (max-width: 800px) {
    .mm-theme-toggle { padding: 6px 10px; font-size: 13px; height: 36px; gap:6px; }
}
`;

    // Inject style once
    if (!document.getElementById('mm-theme-style')) {
        const style = document.createElement('style');
        style.id = 'mm-theme-style';
        style.textContent = themeCSS;
        document.head.appendChild(style);
    }

    // Helper to apply theme and persist
    function applyTheme(theme) {
        const isLight = theme === 'light';
        document.body.classList.toggle('theme-light', isLight);
        try { localStorage.setItem('mm-theme', theme); } catch { /* ignore storage errors */ }
        updateToggleLabel(isLight);
    }

    function readSavedTheme() {
        try { return localStorage.getItem('mm-theme') || 'dark'; }
        catch { return 'dark'; }
    }

    function updateToggleLabel(isLight) {
        if (!toggleBtn) return;
        // show icon + label in Bulgarian
        toggleBtn.innerHTML = isLight ? '☀️ &nbsp; Светъл' : '🌙 &nbsp; Тъмен';
        toggleBtn.setAttribute('aria-pressed', String(isLight));
    }

    // Create toggle button
    const toggleBtn = document.createElement('button');
    toggleBtn.type = 'button';
    toggleBtn.className = 'mm-theme-toggle';
    toggleBtn.setAttribute('aria-label', 'Превключи тема');
    toggleBtn.setAttribute('title', 'Превключи тема (Светъл / Тъмен)');
    toggleBtn.style.userSelect = 'none';

    toggleBtn.addEventListener('click', () => {
        const nowLight = document.body.classList.contains('theme-light');
        applyTheme(nowLight ? 'dark' : 'light');
    });

    // On DOM ready: apply saved theme and insert button into header (right side)
    document.addEventListener('DOMContentLoaded', () => {
        const saved = readSavedTheme();
        applyTheme(saved);

        // Insert the toggle into the navbar (so the menu will be centered via CSS)
        const navbar = document.querySelector('.navbar');
        if (navbar) {
            // append as last child so brand stays left, menu is centered by CSS, toggle is right
            navbar.appendChild(toggleBtn);
        } else {
            // fallback
            document.body.appendChild(toggleBtn);
        }
        updateToggleLabel(document.body.classList.contains('theme-light'));
    });
})();