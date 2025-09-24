
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
        document.querySelectorAll('#infoExamenForm input, #infoExamenForm select')
            .forEach(el => el.disabled = false);
        document.getElementById('btnConfirmarExamen').disabled = false;
    });
}
});
