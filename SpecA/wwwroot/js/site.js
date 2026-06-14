// SpecA client-side behavior.

// Sidebar collapse/expand toggle (per SpecA/UIUX.MD).
document.addEventListener("DOMContentLoaded", function () {
    var toggleBtn = document.getElementById("toggle-sidebar-btn");
    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            document.body.classList.toggle("toggle-sidebar");
        });
    }
});
