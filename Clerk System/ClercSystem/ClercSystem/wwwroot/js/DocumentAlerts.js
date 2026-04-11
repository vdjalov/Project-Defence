
    document.addEventListener("DOMContentLoaded", function () {
        setTimeout(function () {
            document.querySelectorAll(".temp-alert").forEach(function (el) {
                el.classList.add("fade");

                // Bootstrap fade-out effect (optional)
                el.classList.remove("show");

                setTimeout(() => {
                    el.remove();
                }, 150);
            });
        }, 2000); // 2 seconds
    });
