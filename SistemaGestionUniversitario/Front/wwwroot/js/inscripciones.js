// ==========================
//    FILTRO Y BUSCADOR
//==============================
document.addEventListener("DOMContentLoaded", function () {
    const searchBox = document.getElementById("searchBox");
    const filtroModalidad = document.getElementById("filtroModalidad");
    const ordenNombre = document.getElementById("ordenNombre");
    const accordion = document.getElementById("accordionMaterias");

    // Guardar copia original
    const itemsOriginales = Array.from(accordion.querySelectorAll(".accordion-item"));

    function aplicarFiltros() {
        const texto = searchBox.value.toLowerCase().trim();
        const modalidadFiltro = filtroModalidad.value.toLowerCase().trim();
        const orden = ordenNombre.value;

        // Filtro Modalidad
        let itemsFiltrados = itemsOriginales.filter(item => {
            const nombre = item.querySelector(".accordion-button")?.textContent.toLowerCase().trim() || "";

            const modalidadP = Array.from(item.querySelectorAll(".accordion-body p"))
                .find(p => p.textContent.includes("Modalidad:"));
            const modalidad = modalidadP?.textContent.replace("Modalidad:", "").toLowerCase().trim() || "";

            const cumpleBusqueda = texto === "" || nombre.includes(texto);
            const cumpleModalidad = modalidadFiltro === "" || modalidad === modalidadFiltro;

            return cumpleBusqueda && cumpleModalidad;
        });

        // Ordenar por nombre
        if (orden === "asc") {
            itemsFiltrados.sort((a, b) =>
                a.querySelector(".accordion-button").textContent.localeCompare(
                    b.querySelector(".accordion-button").textContent
                )
            );
        } else if (orden === "desc") {
            itemsFiltrados.sort((a, b) =>
                b.querySelector(".accordion-button").textContent.localeCompare(
                    a.querySelector(".accordion-button").textContent
                )
            );
        }

        // Limpiar y mostrar los filtrados
        accordion.innerHTML = "";
        itemsFiltrados.forEach(item => accordion.appendChild(item));
    }

    searchBox.addEventListener("input", aplicarFiltros);
    filtroModalidad.addEventListener("change", aplicarFiltros);
    ordenNombre.addEventListener("change", aplicarFiltros);
});
