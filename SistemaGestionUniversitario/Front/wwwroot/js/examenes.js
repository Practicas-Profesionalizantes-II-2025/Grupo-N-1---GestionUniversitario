
///////////////////////
// INFORMACION
////////////////////
document.addEventListener('DOMContentLoaded', () => {
const infoModal = document.getElementById('informacionExamen');

if (!infoModal) return;

infoModal.addEventListener('show.bs.modal', event => {
    const button = event.relatedTarget;

    // Datos desde los atributos del botón
    const id = button.getAttribute('data-id') || '';
    const tipo = button.getAttribute('data-tipo') || '';
    const materia = button.getAttribute('data-materia') || '';
    const horario = button.getAttribute('data-horario') || '';
    const fecha = button.getAttribute('data-fecha') || '';

    // Cargar en inputs
    document.getElementById('infoTipo').value = tipo;
    document.getElementById('infoMateria').value = materia;
    document.getElementById('infoHorario').value = horario;
    document.getElementById('infoFecha').value = fecha;
    document.getElementById('infoID').value = id; // <-- asignación importante

    // Bloquear inputs inicialmente
    document.querySelectorAll('#infoExamenForm input').forEach(i => i.disabled = true);

    // Acción del form (si querés enviar al controlador con id)
    document.getElementById('infoExamenForm').setAttribute('action', `/Examen/UpdateExamen/`);

    // Deshabilitar botón Confirmar hasta que se presione Modificar
    document.getElementById('btnConfirmarExamen').disabled = true;
});

const btnModificar = document.getElementById('btnModificarExamen');
if (btnModificar) {
    btnModificar.addEventListener('click', () => {
        document.querySelectorAll('#infoExamenForm input, #infoExamenForm select')
            .forEach(el => el.disabled = false);
        document.getElementById('btnConfirmarExamen').disabled = false;
    });
}
});
// =======================

// MODAL ELIMINAR EXAMEN

// =======================

const confirmModal = document.getElementById('confirmDeleteModal');
const confirmCheckbox = document.getElementById('confirmCheckbox');
const btnAceptar = document.getElementById('btnAceptar');
const deleteForm = document.getElementById('deleteForm');
const deleteMessage = document.getElementById('deleteMessage');

// Configurar modal al abrirse
confirmModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget; // Botón que abrió el modal
    const id = button.getAttribute('data-id');
    const nombreMateria = button.getAttribute('data-nombreMateria');
    const tipo = button.getAttribute('data-tipoExamen');
    const descripcion = button.getAttribute('data-descripcionDiaHorario');
    const fecha = button.getAttribute('data-fecha');

    deleteForm.setAttribute('action', `/Examen/DeleteExamen?nombreMateria=${nombreMateria}&descripcionDiaHorario=${descripcion}&fecha=${fecha}`);

    // Mensaje personalizado
    deleteMessage.textContent = `¿Desea eliminar el examen ${tipo} de ${nombreMateria}?`;
    // Resetear checkbox y botón
    confirmCheckbox.checked = false;
    btnAceptar.disabled = true;
});

// Habilitar aceptar solo si checkbox está marcado
confirmCheckbox.addEventListener('change', function () {
    btnAceptar.disabled = !this.checked;
});

// ==========================
//    FILTRO Y BUSCADOR DE EXÁMENES
//==============================
//document.addEventListener("DOMContentLoaded", function () {
//    const searchBox = document.getElementById("searchBox");
//    const filtroTipo = document.getElementById("filtroTipo");
//    const tbody = document.querySelector("#tablaExamenes tbody");

//    const filasOriginales = Array.from(tbody.querySelectorAll("tr"));

//    function aplicarFiltros() {
//        const texto = searchBox.value.toLowerCase().trim();
//        const tipo = filtroTipo.value.toLowerCase().trim();

//        const filasFiltradas = filasOriginales.filter(fila => {
//            const columnas = fila.querySelectorAll("td");
//            const tipoExamen = (columnas[0]?.textContent || "").toLowerCase().trim();
//            const nombreMateria = (columnas[1]?.textContent || "").toLowerCase().trim();
//            const fecha = (columnas[2]?.textContent || "").toLowerCase().trim();
//            const horario = (columnas[3]?.textContent || "").toLowerCase().trim();

//            const cumpleBusqueda =
//                tipoExamen.includes(texto) ||
//                nombreMateria.includes(texto) ||
//                fecha.includes(texto) ||
//                horario.includes(texto);

//            const cumpleTipo = tipo === "" || tipoExamen === tipo;

//            return cumpleBusqueda && cumpleTipo;
//        });

//        tbody.innerHTML = "";
//        filasFiltradas.forEach(f => tbody.appendChild(f));
//    }

//    searchBox.addEventListener("input", aplicarFiltros);
//    filtroTipo.addEventListener("change", aplicarFiltros);
//});
