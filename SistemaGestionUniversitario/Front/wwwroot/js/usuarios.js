const confirmModal = document.getElementById('confirmDeleteModal');
const confirmCheckbox = document.getElementById('confirmCheckbox');
const btnAceptar = document.getElementById('btnAceptar');
const deleteForm = document.getElementById('deleteForm');
const deleteMessage = document.getElementById('deleteMessage');

// Configurar modal al abrirse
confirmModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget; // Botón que abrió el modal
    const dni = button.getAttribute('data-dni');
    const nombre = button.getAttribute('data-nombre');
    const apellido = button.getAttribute('data-apellido');

    // Mensaje personalizado
    deleteMessage.textContent = `¿Desea eliminar el usuario ${nombre} ${apellido} (DNI: ${dni})?`;

    // Acción del formulario
    deleteForm.setAttribute('action', `/Usuario/DeleteUsuario?dni=${dni}`);

    // Resetear checkbox y botón
    confirmCheckbox.checked = false;
    btnAceptar.disabled = true;
});

// Habilitar aceptar solo si checkbox está marcado
confirmCheckbox.addEventListener('change', function () {
    btnAceptar.disabled = !this.checked;
});

document.addEventListener('DOMContentLoaded', function () {
    const searchBox = document.getElementById('searchBox');
    const filtroRol = document.getElementById('filtroRol');
    const ordenNombre = document.getElementById('ordenNombre');
    const tbody = document.querySelector('#tablaUsuarios tbody');

    // Copia original de todas las filas
    const filasOriginales = Array.from(tbody.querySelectorAll('tr'));

    function aplicarFiltro() {
        const textoBusqueda = searchBox.value.toLowerCase();
        const rolSeleccionado = filtroRol.value.toLowerCase();

        let filtradas = filasOriginales.filter(fila => {
            const textoFila = fila.textContent.toLowerCase();
            const rolFila = fila.cells[0].textContent.toLowerCase(); // suponiendo que rol está en la primera columna
            const coincideBusqueda = textoFila.includes(textoBusqueda);
            const coincideRol = rolSeleccionado === "" || rolFila === rolSeleccionado;
            return coincideBusqueda && coincideRol;
        });

        // Ordenar por nombre (columna 1)
        const orden = ordenNombre.value;
        if (orden) {
            filtradas.sort((a, b) => {
                const nombreA = a.cells[1].textContent.toLowerCase();
                const nombreB = b.cells[1].textContent.toLowerCase();
                if (nombreA < nombreB) return orden === "asc" ? -1 : 1;
                if (nombreA > nombreB) return orden === "asc" ? 1 : -1;
                return 0;
            });
        }

        // Pintar de nuevo en la tabla
        tbody.innerHTML = '';
        filtradas.forEach(fila => tbody.appendChild(fila));
    }

    // Eventos
    searchBox.addEventListener('input', aplicarFiltro);
    filtroRol.addEventListener('change', aplicarFiltro);
    ordenNombre.addEventListener('change', aplicarFiltro);
});
    