(() => {
    const loaderId = "site-page-loader";
    const visibleClass = "site-page-loader-visible";
    const loadingClass = "site-page-is-loading";
    let navigationPending = false;
    let hideToken = 0;
    let blazorEventsRegistered = false;

    function getLoader() {
        return document.getElementById(loaderId);
    }

    function showLoader() {
        const loader = getLoader();
        if (!loader) return;

        hideToken++;
        navigationPending = true;
        document.documentElement.classList.add(loadingClass);
        loader.classList.add(visibleClass);
        loader.setAttribute("aria-hidden", "false");
    }

    async function waitForDestinationAssets() {
        // Give Blazor two paint frames to finish committing the destination DOM.
        await new Promise(resolve =>
            requestAnimationFrame(() => requestAnimationFrame(resolve)));

        const loader = getLoader();
        const imagePromises = Array.from(document.images)
            // The loading-screen artwork itself must never keep the screen open.
            .filter(image => !loader?.contains(image))
            .filter(image => !image.complete)
            .map(image => new Promise(resolve => {
                image.addEventListener("load", resolve, { once: true });
                image.addEventListener("error", resolve, { once: true });
            }));

        const fontPromise = document.fonts?.ready?.catch?.(() => undefined)
            ?? Promise.resolve();

        // A bad/slow remote asset must never trap the visitor on the overlay.
        const timeout = new Promise(resolve => setTimeout(resolve, 5000));
        await Promise.race([
            Promise.all([fontPromise, ...imagePromises]),
            timeout
        ]);
    }

    async function hideLoaderWhenReady() {
        const loader = getLoader();
        if (!loader) return;

        const token = ++hideToken;
        await waitForDestinationAssets();

        if (token !== hideToken) return;

        navigationPending = false;
        loader.classList.remove(visibleClass);
        loader.setAttribute("aria-hidden", "true");
        document.documentElement.classList.remove(loadingClass);
    }

    function isPageNavigation(anchor, event) {
        if (!anchor || event.defaultPrevented || event.button !== 0) return false;
        if (event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) return false;
        if (anchor.hasAttribute("download")) return false;
        if (anchor.target && anchor.target.toLowerCase() !== "_self") return false;

        const href = anchor.getAttribute("href");
        if (!href || href.startsWith("#") || href.startsWith("javascript:")) return false;

        let destination;
        try {
            destination = new URL(anchor.href, window.location.href);
        } catch {
            return false;
        }

        if (destination.origin !== window.location.origin) return false;

        const current = new URL(window.location.href);
        const sameDocument =
            destination.pathname === current.pathname &&
            destination.search === current.search;

        if (sameDocument && destination.hash) return false;

        return destination.href !== current.href;
    }

    document.addEventListener("click", event => {
        const anchor = event.target instanceof Element
            ? event.target.closest("a[href]")
            : null;

        if (isPageNavigation(anchor, event)) {
            showLoader();
        }
    }, true);

    // Full-document browser history navigations can begin before Blazor handles them.
    window.addEventListener("popstate", showLoader);

    function registerBlazorNavigationEvents() {
        if (blazorEventsRegistered || !window.Blazor?.addEventListener) {
            return false;
        }

        blazorEventsRegistered = true;

        // These are the supported .NET 10 enhanced-navigation events.
        Blazor.addEventListener("enhancednavigationstart", () => {
            showLoader();
        });

        Blazor.addEventListener("enhancednavigationend", () => {
            if (navigationPending) {
                void hideLoaderWhenReady();
            }
        });

        // Streaming rendering can produce additional enhancedload updates after
        // navigationend. This safely re-checks assets without trapping the loader.
        Blazor.addEventListener("enhancedload", () => {
            if (navigationPending) {
                void hideLoaderWhenReady();
            }
        });

        return true;
    }

    // page-loading.js is intentionally loaded before blazor.web.js, so wait until
    // Blazor publishes its event API rather than assuming it already exists.
    if (!registerBlazorNavigationEvents()) {
        let attempts = 0;
        const registrationTimer = window.setInterval(() => {
            attempts++;
            if (registerBlazorNavigationEvents() || attempts >= 200) {
                window.clearInterval(registrationTimer);
            }
        }, 25);
    }

    // Initial/full page loads and BFCache restores.
    window.addEventListener("load", () => void hideLoaderWhenReady());
    window.addEventListener("pageshow", () => void hideLoaderWhenReady());

    // Programmatic Navigation.NavigateTo callers (Spells/Bestiary) use this hook.
    window.royalTreasuryLoading = {
        show: showLoader,
        hideWhenReady: hideLoaderWhenReady
    };
})();
