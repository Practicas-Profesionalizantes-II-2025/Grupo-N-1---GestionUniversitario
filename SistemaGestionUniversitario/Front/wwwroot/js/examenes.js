//////////////////////
//     ELIMINAR
/////////////////////
document.addEventListener("DOMContentLoaded", function () {
    const confirmModal = document.getElementById('confirmDeleteModal');
    const confirmCheckbox = document.getElementById('confirmCheckbox');
    const btnAceptar = document.getElementById('btnAceptar');
    const deleteForm = document.getElementById('deleteForm');
    const deleteMessage = document.getElementById('deleteMessage');

    if (confirmModal) { // por seguridad
        confirmModal.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            const tipo = button.getAttribute('data-tipoExamen');
            const materia = button.getAttribute('data-nombreMateria');
            const descripcion = button.getAttribute('data-descripcionDiaHorario');

            deleteMessage.textContent =
                `¿Desea eliminar el examen ${tipo} de la fecha ${descripcion} de la materia ${materia}?`;

            // Codificar parámetros
            const url = `/Examen/DeleteExamen?nombreMateria=${encodeURIComponent(materia)}&descripcionDiaHorario=${encodeURIComponent(descripcion)}`;
            deleteForm.setAttribute('action', url);

            confirmCheckbox.checked = false;
            btnAceptar.disabled = true;
        });

        confirmCheckbox.addEventListener('change', () => {
            btnAceptar.disabled = !confirmCheckbox.checked;
        });
    }

    ///////////////////////
    //       FILTRO     //
    /////////////////////
    const searchBox = document.getElementById("searchBox");
    const chkParcial = document.getElementById("chkParcial");
    const chkFinal = document.getElementById("chkFinal");
    const tbody = document.querySelector("#tablaExamenes tbody");
    const filasOriginales = Array.from(tbody.querySelectorAll("tr"));

    function aplicarFiltros() {
        let texto = searchBox.value.toLowerCase().trim();
        let tiposSeleccionados = [];
        if (chkParcial.checked) tiposSeleccionados.push("Parcial");
        if (chkFinal.checked) tiposSeleccionados.push("Final");

        const filtradas = filasOriginales.filter(fila => {
            let c = fila.querySelectorAll("td");
            let tipo = (c[0]?.textContent || "").trim();
            let materia = (c[1]?.textContent || "").toLowerCase();
            let horario = (c[2]?.textContent || "").toLowerCase();
            return (materia.includes(texto) || horario.includes(texto)) &&
                (tiposSeleccionados.length === 0 || tiposSeleccionados.includes(tipo));
        });

        tbody.innerHTML = "";
        filtradas.forEach(f => tbody.appendChild(f));
    }

    searchBox.addEventListener("input", aplicarFiltros);
    chkParcial.addEventListener("change", aplicarFiltros);
    chkFinal.addEventListener("change", aplicarFiltros);
});
