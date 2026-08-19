/* =========================================
   ROYAL TREASURY
   SMART GLOSSARY TOOLTIP POSITIONING

   Desktop rules:
   1. Stay beside the highlighted word.
   2. Never leave the visible viewport.

   Mobile positioning remains entirely CSS-
   controlled as a bottom panel.
   ========================================= */

(() => {
    "use strict";

    const MOBILE_BREAKPOINT = 600;
    const VIEWPORT_MARGIN = 16;
    const TERM_GAP = 10;

    function clamp(value, minimum, maximum) {
        if (maximum < minimum) {
            return minimum;
        }

        return Math.min(
            Math.max(value, minimum),
            maximum
        );
    }

    function clearDesktopPosition(tooltip) {
        if (!tooltip) {
            return;
        }

        const properties = [
            "position",
            "top",
            "left",
            "right",
            "bottom",
            "transform",
            "max-height",
            "visibility",
            "opacity",
            "pointer-events",
            "transition"
        ];

        for (const property of properties) {
            tooltip.style.removeProperty(property);
        }

        tooltip.removeAttribute("data-positioned");
        tooltip.removeAttribute("data-placement");
    }

    function setImportant(tooltip, property, value) {
        tooltip.style.setProperty(
            property,
            value,
            "important"
        );
    }

    function positionTooltip(term) {
        if (!(term instanceof HTMLElement)) {
            return;
        }

        const tooltip =
            term.querySelector(":scope > .glossary-tooltip");

        if (!(tooltip instanceof HTMLElement)) {
            return;
        }

        if (window.innerWidth <= MOBILE_BREAKPOINT) {
            clearDesktopPosition(tooltip);
            return;
        }

        const viewportWidth =
            document.documentElement.clientWidth;

        const viewportHeight =
            document.documentElement.clientHeight;

        const termRect =
            term.getBoundingClientRect();

        setImportant(tooltip, "position", "fixed");
        setImportant(tooltip, "left", "0px");
        setImportant(tooltip, "top", "0px");
        setImportant(tooltip, "right", "auto");
        setImportant(tooltip, "bottom", "auto");
        setImportant(tooltip, "transform", "none");
        setImportant(
            tooltip,
            "max-height",
            `${Math.max(
                120,
                viewportHeight - (VIEWPORT_MARGIN * 2)
            )}px`
        );
        setImportant(tooltip, "visibility", "hidden");
        setImportant(tooltip, "opacity", "0");
        setImportant(tooltip, "pointer-events", "none");
        setImportant(tooltip, "transition", "none");

        let tooltipRect =
            tooltip.getBoundingClientRect();

        let tooltipWidth =
            tooltipRect.width;

        let tooltipHeight =
            tooltipRect.height;

        const spaceBelow =
            viewportHeight
            - VIEWPORT_MARGIN
            - termRect.bottom
            - TERM_GAP;

        const spaceAbove =
            termRect.top
            - VIEWPORT_MARGIN
            - TERM_GAP;

        const spaceRight =
            viewportWidth
            - VIEWPORT_MARGIN
            - termRect.right
            - TERM_GAP;

        const spaceLeft =
            termRect.left
            - VIEWPORT_MARGIN
            - TERM_GAP;

        let placement;

        if (tooltipHeight <= spaceBelow) {
            placement = "below";
        }
        else if (tooltipHeight <= spaceAbove) {
            placement = "above";
        }
        else if (tooltipWidth <= spaceRight) {
            placement = "right";
        }
        else if (tooltipWidth <= spaceLeft) {
            placement = "left";
        }
        else {
            placement =
                spaceBelow >= spaceAbove
                    ? "below"
                    : "above";
        }

        let left;
        let top;
        let availableHeight =
            viewportHeight
            - (VIEWPORT_MARGIN * 2);

        if (placement === "below") {
            left =
                termRect.left
                + (termRect.width / 2)
                - (tooltipWidth / 2);

            top =
                termRect.bottom
                + TERM_GAP;

            availableHeight =
                Math.max(
                    80,
                    viewportHeight
                    - VIEWPORT_MARGIN
                    - top
                );
        }
        else if (placement === "above") {
            left =
                termRect.left
                + (termRect.width / 2)
                - (tooltipWidth / 2);

            availableHeight =
                Math.max(
                    80,
                    termRect.top
                    - VIEWPORT_MARGIN
                    - TERM_GAP
                );

            const displayedHeight =
                Math.min(
                    tooltipHeight,
                    availableHeight
                );

            top =
                termRect.top
                - TERM_GAP
                - displayedHeight;
        }
        else if (placement === "right") {
            left =
                termRect.right
                + TERM_GAP;

            top =
                termRect.top
                + (termRect.height / 2)
                - (tooltipHeight / 2);
        }
        else {
            left =
                termRect.left
                - TERM_GAP
                - tooltipWidth;

            top =
                termRect.top
                + (termRect.height / 2)
                - (tooltipHeight / 2);
        }

        setImportant(
            tooltip,
            "max-height",
            `${Math.min(
                availableHeight,
                viewportHeight - (VIEWPORT_MARGIN * 2)
            )}px`
        );

        tooltipRect =
            tooltip.getBoundingClientRect();

        tooltipWidth =
            tooltipRect.width;

        tooltipHeight =
            tooltipRect.height;

        left =
            clamp(
                left,
                VIEWPORT_MARGIN,
                viewportWidth
                - VIEWPORT_MARGIN
                - tooltipWidth
            );

        top =
            clamp(
                top,
                VIEWPORT_MARGIN,
                viewportHeight
                - VIEWPORT_MARGIN
                - tooltipHeight
            );

        setImportant(
            tooltip,
            "left",
            `${Math.round(left)}px`
        );

        setImportant(
            tooltip,
            "top",
            `${Math.round(top)}px`
        );

        setImportant(tooltip, "right", "auto");
        setImportant(tooltip, "bottom", "auto");
        setImportant(tooltip, "transform", "none");

        tooltip.setAttribute(
            "data-placement",
            placement
        );

        tooltip.setAttribute(
            "data-positioned",
            "true"
        );

        tooltip.style.removeProperty("visibility");
        tooltip.style.removeProperty("opacity");
        tooltip.style.removeProperty("pointer-events");
        tooltip.style.removeProperty("transition");
    }

    document.addEventListener(
        "pointerover",
        event => {
            const target = event.target;

            if (!(target instanceof Element)) {
                return;
            }

            const term =
                target.closest(".glossary-term");

            if (term) {
                positionTooltip(term);
            }
        },
        true
    );

    document.addEventListener(
        "focusin",
        event => {
            const target = event.target;

            if (!(target instanceof Element)) {
                return;
            }

            const term =
                target.closest(".glossary-term");

            if (term) {
                positionTooltip(term);
            }
        },
        true
    );

    window.addEventListener(
        "resize",
        () => {
            document
                .querySelectorAll(".glossary-term")
                .forEach(term => {
                    const tooltip =
                        term.querySelector(
                            ":scope > .glossary-tooltip"
                        );

                    if (window.innerWidth <= MOBILE_BREAKPOINT) {
                        clearDesktopPosition(tooltip);
                        return;
                    }

                    if (
                        term.matches(":hover")
                        ||
                        term.matches(":focus")
                        ||
                        term.matches(":focus-within")
                    ) {
                        positionTooltip(term);
                    }
                });
        },
        { passive: true }
    );

})();
