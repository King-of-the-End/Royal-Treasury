let scrollHandler = null;
let resizeHandler = null;
let currentDotNetRef = null;
let trackedSections = [];
let lastLabel = null;

function updateActiveSection() {
    if (!currentDotNetRef || trackedSections.length === 0) {
        return;
    }

    // Slightly below the fixed mobile bar.
    const threshold = 76;

    let active = trackedSections[0];

    for (const item of trackedSections) {
        const top = item.element.getBoundingClientRect().top;

        if (top <= threshold) {
            active = item;
        } else {
            break;
        }
    }

    // At the very bottom of the page, make sure the final section can win.
    const nearBottom =
        window.innerHeight + window.scrollY >=
        document.documentElement.scrollHeight - 4;

    if (nearBottom) {
        active = trackedSections[trackedSections.length - 1];
    }

    if (active.label !== lastLabel) {
        lastLabel = active.label;

        currentDotNetRef
            .invokeMethodAsync("SetActiveIndexLabel", active.label)
            .catch(() => {
                // Circuit may have disconnected during navigation.
            });
    }
}

export function initializeSectionTracking(dotNetRef, items) {
    disposeSectionTracking();

    currentDotNetRef = dotNetRef;

    trackedSections = (items ?? [])
        .map(item => {
            const element = document.getElementById(item.id);

            return element
                ? {
                    id: item.id,
                    label: item.label,
                    element
                }
                : null;
        })
        .filter(Boolean);

    if (trackedSections.length === 0) {
        return;
    }

    scrollHandler = () => {
        window.requestAnimationFrame(updateActiveSection);
    };

    resizeHandler = () => {
        window.requestAnimationFrame(updateActiveSection);
    };

    window.addEventListener("scroll", scrollHandler, { passive: true });
    window.addEventListener("resize", resizeHandler, { passive: true });

    window.requestAnimationFrame(updateActiveSection);
}

export function disposeSectionTracking() {
    if (scrollHandler) {
        window.removeEventListener("scroll", scrollHandler);
    }

    if (resizeHandler) {
        window.removeEventListener("resize", resizeHandler);
    }

    scrollHandler = null;
    resizeHandler = null;
    currentDotNetRef = null;
    trackedSections = [];
    lastLabel = null;
}

