// Filtro de búsqueda
document.getElementById("searchInput").addEventListener("keyup", function () {
    let filter = this.value.toLowerCase();
    let rows = document.querySelectorAll("#materiaTable tr");

    rows.forEach(row => {
        let text = row.innerText.toLowerCase();
        row.style.display = text.includes(filter) ? "" : "none";
    });
});

// Eliminar materia con confirmación
async function deleteMateria(nombreMateria) {
    if (confirm("¿Estás seguro que quieres eliminar la materia " + nombreMateria + "?")) {
        const response = await fetch(`/Materia/${nombreMateria}`, {method: "DELETE"});

        if (response.ok) {
            const msg = await response.text();
            alert(msg);
            location.reload();
        } else {
            const error = await response.text();
            alert("Error al eliminar la materia: " + error);
        }
    }
}

// Mas Información

async function infoMateria(nombreMateria) {
    try {
        const response = await fetch(`/Materia/GetMateriaJson/${nombreMateria}`);
        if (response.ok) {
            const materia = await response.json();
            document.getElementById("modalHorarios").textContent = materia.descripcionDiasHorarios?.join(", ") || "No asignado";
            document.getElementById("modalProfesores").textContent = materia.nombresProfesores?.join(", ") || "No asignado";
            document.getElementById("modalAnio").textContent = materia.anio;
            document.getElementById("modalModalidad").textContent = materia.modalidad;
            const modal = new bootstrap.Modal(document.getElementById('materiaModal'));
            modal.show();
        } else {
            const error = await response.text();
            alert("Error: " + error);
        }
    } catch (err) {
        console.error("Error:", err);
        alert("Error de conexión.");
    }
}

// Boton Filtros
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchInput");
    const filtroModalidad = document.getElementById("filtroModalidad");
    const ordenNombre = document.getElementById("ordenNombre");
    const tbody = document.querySelector("#materiaTable");

    // Guardar copia original de filas
    let filasOriginales = Array.from(tbody.querySelectorAll("tr"));

    function aplicarFiltros() {
        let texto = (searchInput?.value || "").toLowerCase().trim();
        let modalidad = (filtroModalidad?.value || "").toLowerCase().trim();
        let orden = (ordenNombre?.value || "").toLowerCase();

        console.log("Aplicando filtros:", { texto, modalidad, orden });

        let filasFiltradas = filasOriginales.filter(fila => {
            let columnas = fila.querySelectorAll("td");

            let nombre = (columnas[0]?.textContent || "").toLowerCase().trim();
            let anio = (columnas[1]?.textContent || "").toLowerCase().trim();
            let modalidadFila = (columnas[2]?.textContent || "").toLowerCase().trim();

            let cumpleBusqueda =
                nombre.includes(texto) || anio.includes(texto) || modalidadFila.includes(texto);

            let cumpleModalidad = modalidad === "" || modalidadFila === modalidad;

            return cumpleBusqueda && cumpleModalidad;
        });

        // Ordenar por nombre (columna 0)
        if (orden === "asc") {
            filasFiltradas.sort((a, b) =>
                a.cells[0].textContent.localeCompare(b.cells[0].textContent)
            );
        } else if (orden === "desc") {
            filasFiltradas.sort((a, b) =>
                b.cells[0].textContent.localeCompare(a.cells[0].textContent)
            );
        }

        // Repintar tabla
        tbody.innerHTML = "";
        filasFiltradas.forEach(f => tbody.appendChild(f));
    }

    // Eventos
    if (searchInput) searchInput.addEventListener("input", aplicarFiltros);
    if (filtroModalidad) filtroModalidad.addEventListener("change", aplicarFiltros);
    if (ordenNombre) ordenNombre.addEventListener("change", aplicarFiltros);
});




