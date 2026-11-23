// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    const backToTop = document.querySelector("[data-back-to-top]");

    if (!backToTop) {
        return;
    }

    backToTop.addEventListener("click", event => {
        event.preventDefault();

        const reduceMotion = window.matchMedia(
            "(prefers-reduced-motion: reduce)"
        ).matches;

        window.scrollTo({
            top: 0,
            left: 0,
            behavior: reduceMotion ? "auto" : "smooth"
        });
    });
});

document.addEventListener("DOMContentLoaded", () => {
    const presidentMessage = document.querySelector("[data-president-message]");

    if (!presidentMessage) {
        return;
    }

    const reduceMotion = window.matchMedia(
        "(prefers-reduced-motion: reduce)"
    ).matches;

    if (reduceMotion || !("IntersectionObserver" in window)) {
        presidentMessage.classList.add("is-visible");
        return;
    }

    presidentMessage.dataset.revealReady = "true";

    const observer = new IntersectionObserver(
        entries => {
            const [entry] = entries;

            if (!entry.isIntersecting) {
                return;
            }

            presidentMessage.classList.add("is-visible");
            observer.disconnect();
        },
        { threshold: 0.24 }
    );

    observer.observe(presidentMessage);
});

document.addEventListener("DOMContentLoaded", () => {
    const homeUpdates = document.querySelector("[data-home-updates]");

    if (!homeUpdates) {
        return;
    }

    const reduceMotion = window.matchMedia(
        "(prefers-reduced-motion: reduce)"
    ).matches;

    if (reduceMotion || !("IntersectionObserver" in window)) {
        homeUpdates.classList.add("is-visible");
        return;
    }

    homeUpdates.dataset.revealReady = "true";

    const observer = new IntersectionObserver(
        entries => {
            const [entry] = entries;

            if (!entry.isIntersecting) {
                return;
            }

            homeUpdates.classList.add("is-visible");
            observer.disconnect();
        },
        { threshold: 0.16 }
    );

    observer.observe(homeUpdates);
});

document.addEventListener("DOMContentLoaded", () => {
    const reportForm = document.querySelector("[data-confidential-report-form]");

    if (!reportForm) {
        return;
    }

    const contactToggle = reportForm.querySelector("[data-report-contact-toggle]");
    const contactFields = reportForm.querySelector("[data-report-contact-fields]");
    const modeLabel = reportForm.querySelector("[data-report-mode-label]");

    if (!contactToggle || !contactFields || !modeLabel) {
        return;
    }

    const updateContactMode = () => {
        const isContactRequested = contactToggle.checked;
        contactFields.classList.toggle("is-visible", isContactRequested);
        modeLabel.textContent = isContactRequested
            ? "İletişim bilgilerimi paylaşacağım"
            : "Kimlik bilgisi vermeden bildirim";

        contactFields.querySelectorAll("input").forEach(input => {
            input.disabled = !isContactRequested;
        });
    };

    contactToggle.addEventListener("change", updateContactMode);
    updateContactMode();
});
