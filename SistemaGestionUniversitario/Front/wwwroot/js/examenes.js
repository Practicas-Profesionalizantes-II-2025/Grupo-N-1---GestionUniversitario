
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
//================================
document.addEventListener("DOMContentLoaded", () => {
    const searchBox = document.getElementById("searchBox");
    const chkFinal = document.getElementById("chkFinal");
    const chkParcial = document.getElementById("chkParcial");
    const tabla = document.getElementById("tablaExamenes");

    if (!tabla) return;

    const filasOriginales = Array.from(tabla.querySelectorAll("tbody tr"));

    function aplicarFiltros() {
        const textoBusqueda = searchBox.value.toLowerCase();
        const tiposSeleccionados = [];
        if (chkFinal.checked) tiposSeleccionados.push("Final");
        if (chkParcial.checked) tiposSeleccionados.push("Parcial");

        filasOriginales.forEach(fila => {
            const celdas = fila.querySelectorAll("td");
            const textoFila = Array.from(celdas).map(td => td.textContent.toLowerCase()).join(" ");
            // Índice 0 = columna Tipo
            const tipoExamen = celdas[0]?.textContent || "";

            const coincideBusqueda = textoFila.includes(textoBusqueda);
            const coincideTipo = tiposSeleccionados.length === 0 || tiposSeleccionados.includes(tipoExamen);

            fila.style.display = coincideBusqueda && coincideTipo ? "" : "none";
        });
    }

    // Eventos
    searchBox.addEventListener("input", aplicarFiltros);
    chkFinal.addEventListener("change", aplicarFiltros);
    chkParcial.addEventListener("change", aplicarFiltros);
});
    //modal notas
        // Setear ID examen en hidden input
    document.addEventListener('DOMContentLoaded', function () 
    {

        // Cuando se abre el modal de notas
        const cargarNotasModal = document.getElementById('cargarNotasModal');

        cargarNotasModal.addEventListener('show.bs.modal', async function (event) {
            const button = event.relatedTarget;
            const nombreMateria = button.getAttribute('data-materia'); // ← viene del botón del examen
            console.log("Nombre de la materia:", nombreMateria);
            const examenId = button.getAttribute('data-id');

            // Asignamos el ID del examen al campo oculto del formulario
            document.getElementById('notaExamenId').value = examenId;

            // Limpiamos la tabla antes de cargar los nuevos datos
            const tablaBody = document.getElementById('tablaAlumnosNotas');
            tablaBody.innerHTML = '';

            try {
                // Llamamos al método del controlador
                const response = await fetch(`/Examen/GetAlumnosPorMateria?nombreMateria=${encodeURIComponent(nombreMateria)}`);

                if (!response.ok) {
                    throw new Error('Error al obtener alumnos');
                }

                const alumnos = await response.json();
                console.log("Alumnos:", alumnos);

                // Si no hay alumnos, mostramos un mensaje
                if (alumnos.length === 0) {
                    tablaBody.innerHTML = '<tr><td colspan="3" class="text-center">No hay alumnos inscriptos.</td></tr>';
                    return;
                }

                // Rellenamos la tabla con los alumnos inscriptos
                alumnos.forEach((alumno, index) => {
                    const fila = document.createElement('tr');
                    fila.innerHTML = `
                    <td>${alumno.nombreAlumno} ${alumno.apellidoAlumno}</td>
                    <td>${alumno.dniAlumno}</td>
                    <td>
                        <input type="number" class="form-control nota-input" name="Notas[${index}].Nota" min="0" max="100" required />
                        <input type="hidden" name="Notas[${index}].DNIAlumno" value="${alumno.dni}" />
                    </td>
                `;
                    tablaBody.appendChild(fila);
                });

            } catch (error) {
                console.error('Error al cargar alumnos:', error);
                tablaBody.innerHTML = '<tr><td colspan="3" class="text-center text-danger">Error al cargar los alumnos.</td></tr>';
            }
        });
    });