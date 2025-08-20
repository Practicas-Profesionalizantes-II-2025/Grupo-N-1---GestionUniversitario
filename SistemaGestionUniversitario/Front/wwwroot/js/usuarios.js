// =======================

// MODAL ELIMINAR USUARIO

// =======================

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

// =======================

// MODAL INFORMACION USUARIO

// =======================

const infoModal = document.getElementById('informacionUsuario');

infoModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget;

    const dni = button.getAttribute('data-dni');
    const nombre = button.getAttribute('data-nombre');
    const apellido = button.getAttribute('data-apellido');
    const localidad = button.getAttribute('data-localidad');
    const direccion = button.getAttribute('data-direccion');
    const telefono = button.getAttribute('data-telefono');
    const rol = button.getAttribute('data-rol');

    document.getElementById('infoUsuarioLabel').textContent = `${nombre} ${apellido}`;
    document.getElementById('infoDni').textContent = dni;
    document.getElementById('infoLocalidad').textContent = localidad;
    document.getElementById('infoDireccion').textContent = direccion;
    document.getElementById('infoTelefono').textContent = telefono;
    document.getElementById('infoRol').textContent = rol;
});

// ==========================
//    FILTRO Y BUSCASDOR
//==============================
document.addEventListener("DOMContentLoaded", function () {
    const searchBox = document.getElementById("searchBox");
    const filtroRol = document.getElementById("filtroRol");
    const ordenNombre = document.getElementById("ordenNombre");
    const tbody = document.querySelector("#tablaUsuarios tbody");

    // Guardar copia original de filas
    const filasOriginales = Array.from(tbody.querySelectorAll("tr"));

    function aplicarFiltros() {
        let texto = searchBox.value.toLowerCase().trim();
        let rol = filtroRol.value.toLowerCase().trim();
        let orden = ordenNombre.value;

        let filasFiltradas = filasOriginales.filter(fila => {
            let columnas = fila.querySelectorAll("td");

            // En tu tabla: col[0] = Rol, col[1] = Nombre+Apellido, col[2] = DNI
            let rolUsuario = (columnas[0]?.textContent || "").toLowerCase().trim();
            let nombre = (columnas[1]?.textContent || "").toLowerCase().trim();
            let dni = (columnas[2]?.textContent || "").toLowerCase().trim();

            // Buscar en todas las columnas
            let cumpleBusqueda =
                nombre.includes(texto) ||
                rolUsuario.includes(texto) ||
                dni.includes(texto);

            // Filtrar por rol (solo si se eligió uno)
            let cumpleRol = rol === "" || rolUsuario === rol;

            return cumpleBusqueda && cumpleRol;
        });

        // Ordenar por nombre (columna 1)
        if (orden === "asc") {
            filasFiltradas.sort((a, b) =>
                a.cells[1].textContent.localeCompare(b.cells[1].textContent)
            );
        } else if (orden === "desc") {
            filasFiltradas.sort((a, b) =>
                b.cells[1].textContent.localeCompare(a.cells[1].textContent)
            );
        }

        // Pintar tabla
        tbody.innerHTML = "";
        filasFiltradas.forEach(f => tbody.appendChild(f));
    }

    // Eventos
    searchBox.addEventListener("input", aplicarFiltros);
    filtroRol.addEventListener("change", aplicarFiltros);
    ordenNombre.addEventListener("change", aplicarFiltros);
});





