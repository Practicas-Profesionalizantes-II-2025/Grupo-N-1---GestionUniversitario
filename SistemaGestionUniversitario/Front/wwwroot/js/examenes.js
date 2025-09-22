

//document.addEventListener("DOMContentLoaded", function () {

//    ///////////////////////
//    //     ELIMINAR
//    ///////////////////////
//    const confirmModal = document.getElementById('confirmDeleteModal');
//    const confirmCheckbox = document.getElementById('confirmCheckbox');
//    const btnAceptar = document.getElementById('btnAceptar');
//    const deleteForm = document.getElementById('deleteForm');
//    const deleteMessage = document.getElementById('deleteMessage');

//    if (confirmModal) {
//        confirmModal.addEventListener('show.bs.modal', function (event) {
//            const button = event.relatedTarget;
//            const id = button.getAttribute('data-idExamen');
//            const tipo = button.getAttribute('data-tipoExamen');
//            const materia = button.getAttribute('data-nombreMateria');
//            const descripcion = button.getAttribute('data-descripcionDiaHorario');

//            deleteMessage.textContent =
//                `¿Desea eliminar el examen ${tipo} de la fecha ${descripcion} de la materia ${materia}?`;

//            const url = `/Examen/DeleteExamen/${id}`;
//            deleteForm.setAttribute('action', url);

//            confirmCheckbox.checked = false;
//            btnAceptar.disabled = true;
//        });

//        confirmCheckbox.addEventListener('change', () => {
//            btnAceptar.disabled = !confirmCheckbox.checked;
//        });
//    }
//});
//    ///////////////////////
//    //       FILTRO
//    ///////////////////////
//    const searchBox = document.getElementById("searchBox");
//    const chkParcial = document.getElementById("chkParcial");
//    const chkFinal = document.getElementById("chkFinal");
//    const tbody = document.querySelector("#tablaExamenes tbody");
//    const filasOriginales = Array.from(tbody.querySelectorAll("tr"));

//    function aplicarFiltros() {
//        let texto = searchBox.value.toLowerCase().trim();
//        let tiposSeleccionados = [];
//        if (chkParcial.checked) tiposSeleccionados.push("Parcial");
//        if (chkFinal.checked) tiposSeleccionados.push("Final");

//        const filtradas = filasOriginales.filter(fila => {
//            let c = fila.querySelectorAll("td");
//            let tipo = (c[0]?.textContent || "").trim();
//            let materia = (c[1]?.textContent || "").toLowerCase();
//            let horario = (c[2]?.textContent || "").toLowerCase();
//            return (materia.includes(texto) || horario.includes(texto)) &&
//                (tiposSeleccionados.length === 0 || tiposSeleccionados.includes(tipo));
//        });

//        tbody.innerHTML = "";
//        filtradas.forEach(f => tbody.appendChild(f));
//    }

//    searchBox.addEventListener("input", aplicarFiltros);
//    chkParcial.addEventListener("change", aplicarFiltros);
//    chkFinal.addEventListener("change", aplicarFiltros);

//    // Llamamos checkForm al cargar para asegurarnos que el botón inicie deshabilitado
//    checkForm();

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

    // Bloquear inputs inicialmente
    document.querySelectorAll('#infoExamenForm input').forEach(i => i.disabled = true);

    // Acción del form (si querés enviar al controlador con id)
    document.getElementById('infoExamenForm').setAttribute('action', `/Examen/UpdateExamen/${id}`);

    // Deshabilitar botón Confirmar hasta que se presione Modificar
    document.getElementById('btnConfirmarExamen').disabled = true;
});

const btnModificar = document.getElementById('btnModificarExamen');
if (btnModificar) {
    btnModificar.addEventListener('click', () => {
        document.querySelectorAll('#infoExamenForm input').forEach(i => i.disabled = false);
        document.getElementById('btnConfirmarExamen').disabled = false;
    });
}
});
