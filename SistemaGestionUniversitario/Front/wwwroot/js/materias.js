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
async function deleteMateria(nombre) {
    if (confirm("¿Estás seguro que quieres eliminar la materia " + nombre + "?")) {
        const response = await fetch(`/Materia/${nombre}`, {
            method: "DELETE"
        });

        if (response.ok) {
            location.reload(); // recargar la tabla
        } else {
            alert("Error al eliminar la materia.");
        }
    }
}