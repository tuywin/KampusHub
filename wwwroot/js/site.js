// Mobilde yan menüyü aç / kapat
document.addEventListener("DOMContentLoaded", function () {
    var menu = document.getElementById("yanMenu");
    var buton = document.getElementById("menuAc");
    var perde = document.getElementById("perde");
    if (!menu || !buton || !perde) return;

    function ayarla(acik) {
        menu.classList.toggle("acik", acik);
        perde.classList.toggle("acik", acik);
        buton.setAttribute("aria-expanded", acik);
    }

    buton.addEventListener("click", function () { ayarla(!menu.classList.contains("acik")); });
    perde.addEventListener("click", function () { ayarla(false); });
    document.addEventListener("keydown", function (e) { if (e.key === "Escape") ayarla(false); });
});
