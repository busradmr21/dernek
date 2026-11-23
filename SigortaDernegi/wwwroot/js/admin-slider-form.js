(() => {
    const input = document.querySelector("[data-slider-image-input]");
    const image = document.querySelector("[data-slider-preview-image]");
    const placeholder = document.querySelector("[data-slider-preview-placeholder]");

    if (!input || !image || !placeholder) {
        return;
    }

    let previewUrl;

    input.addEventListener("change", () => {
        const file = input.files?.[0];

        if (!file) {
            return;
        }

        if (previewUrl) {
            URL.revokeObjectURL(previewUrl);
        }

        previewUrl = URL.createObjectURL(file);
        image.src = previewUrl;
        image.classList.remove("d-none");
        placeholder.classList.add("d-none");
    });

    window.addEventListener("beforeunload", () => {
        if (previewUrl) {
            URL.revokeObjectURL(previewUrl);
        }
    });
})();
