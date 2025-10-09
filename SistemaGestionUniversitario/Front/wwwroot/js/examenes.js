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
document.addEventListener('DOMContentLoaded', function () {

    const cargarNotasModal = document.getElementById('cargarNotasModal');
    const cargarNotasForm = document.getElementById('cargarNotasForm');
    const tablaBody = document.getElementById('tablaAlumnosNotas');
    const examenIdInput = document.getElementById('notaExamenId');

    // Cuando se abre el modal de notas
    cargarNotasModal.addEventListener('show.bs.modal', async function (event) {
        const button = event.relatedTarget;
        const nombreMateria = button.getAttribute('data-materia');
        const examenId = button.getAttribute('data-id');
        console.log("Materia:", nombreMateria);
        console.log("Examen ID:", examenId);

        examenIdInput.value = examenId;
        tablaBody.innerHTML = ''; // limpiar tabla

        try {
            const response = await fetch(`/Examen/GetAlumnosPorMateria?nombreMateria=${encodeURIComponent(nombreMateria)}`);
            if (!response.ok) throw new Error('Error al obtener alumnos');

            const alumnos = await response.json();
            console.log("Alumnos:", alumnos);

            if (alumnos.length === 0) {
                tablaBody.innerHTML = '<tr><td colspan="3" class="text-center">No hay alumnos inscriptos.</td></tr>';
                return;
            }
            const notasResponse = await fetch(`/Examen/GetNotasPorExamen?idExamen=${examenId}`);
            let notasExistentes = [];
            if (notasResponse.ok) {
                notasExistentes = await notasResponse.json();
                console.log("Notas existentes:", notasExistentes);
            } else {
                console.warn("No se pudieron obtener las notas existentes o aún no hay ninguna cargada.");
            }
            console.log("Notas existentes:", notasExistentes);

            alumnos.forEach((alumno, index) => {

                const notaExistente = notasExistentes.find(n => n.alumnoNombre === `${alumno.nombreAlumno} ${alumno.apellidoAlumno}`);
                console.log(`Nota existente para`, notasExistentes.find(n => n.alumnoNombre));
                const valorNota = notaExistente ? notaExistente.nota : '';
                const fila = document.createElement('tr');
                fila.innerHTML = `
                    <td>${alumno.nombreAlumno} ${alumno.apellidoAlumno}</td>
                    <td>${alumno.dniAlumno}</td>
        <td>
            <input type="number"
                   class="form-control nota-input" 
                   data-dni="${alumno.dniAlumno}"
                   min="0" max="10"
                   value="${valorNota}"
                   placeholder="Nota" 
                   required />
        </td>
                `;
                tablaBody.appendChild(fila);
            });

        } catch (error) {
            console.error('Error al cargar alumnos:', error);
            tablaBody.innerHTML = '<tr><td colspan="3" class="text-center text-danger">Error al cargar los alumnos.</td></tr>';
        }
    });

    // Cuando se envía el formulario de notas
    cargarNotasForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const examenId = examenIdInput.value;
        const notaInputs = tablaBody.querySelectorAll('.nota-input');
        const notas = [];

        notaInputs.forEach(input => {
            const nota = parseInt(input.value);
            const dni = input.getAttribute('data-dni');
            console.log ("dni:", dni);

            if (!isNaN(nota)) {
                notas.push({
                    nota: nota,
                    dniAlumno: dni,
                    idExamen: parseInt(examenId)
                });
            }
        });

        if (notas.length === 0) {
            alert("No se ingresaron notas.");
            return;
        }

        console.log("Notas a enviar:", notas);

        try {
            const response = await fetch('/Nota/CargarNotas', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify(notas)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            const result = await response.json();
            console.log("Respuesta:", result);

            // Mostrar mensaje de éxito
            var modal = new bootstrap.Modal(document.getElementById('mensajeModal'));
            modal.show();

            // Cerrar modal y limpiar tabla
            const modalInstance = bootstrap.Modal.getInstance(cargarNotasModal);
            modalInstance.hide();
            tablaBody.innerHTML = '';

        } catch (error) {
            console.error("Error al guardar notas:", error);
            alert("❌ Error al guardar las notas. Revisa la consola para más detalles.");
        }
    });
});