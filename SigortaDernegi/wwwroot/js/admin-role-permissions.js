document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector(
        "[data-permission-form]"
    );

    const countElement = document.querySelector(
        "[data-permission-count]"
    );

    if (!form || !countElement) {
        return;
    }

    const checkboxes = Array.from(
        form.querySelectorAll(
            'input[type="checkbox"]'
        )
    );

    function updatePermissionCount() {
        const enabledCount = checkboxes.filter(
            checkbox => checkbox.checked
        ).length;

        countElement.textContent =
            `${enabledCount} yetki açık`;
    }

    form.addEventListener("change", function (event) {
        const changedCheckbox = event.target;

        if (!(changedCheckbox instanceof HTMLInputElement)) {
            return;
        }

        if (changedCheckbox.type !== "checkbox") {
            return;
        }

        const row = changedCheckbox.closest(
            "[data-permission-row]"
        );

        if (!row) {
            updatePermissionCount();
            return;
        }

        const viewPermission = row.querySelector(
            "[data-view-permission]"
        );

        const actionPermissions = Array.from(
            row.querySelectorAll(
                "[data-action-permission]"
            )
        );

        if (
            changedCheckbox.hasAttribute(
                "data-action-permission"
            ) &&
            changedCheckbox.checked &&
            viewPermission
        ) {
            viewPermission.checked = true;
        }

        if (
            changedCheckbox.hasAttribute(
                "data-view-permission"
            ) &&
            !changedCheckbox.checked
        ) {
            actionPermissions.forEach(
                permission => {
                    permission.checked = false;
                }
            );
        }

        updatePermissionCount();
    });

    updatePermissionCount();
});