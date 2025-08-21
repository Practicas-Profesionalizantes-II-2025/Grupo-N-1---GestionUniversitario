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

