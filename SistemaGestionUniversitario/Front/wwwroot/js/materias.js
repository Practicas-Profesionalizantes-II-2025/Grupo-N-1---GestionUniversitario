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
        try {
            const response = await fetch(`/Materia/${encodeURIComponent(nombreMateria)}`, {
                method: "DELETE",
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const result = await response.json();

            if (response.ok) {
                alert(result.message || "Materia eliminada correctamente.");
                location.reload(); // recargar la página
            } else {
                alert(result.message || "Error al eliminar la materia.");
            }
        } catch (error) {
            console.error('Error:', error);
            alert("Error de conexión al intentar eliminar la materia.");
        }
    }
}