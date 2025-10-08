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

// Cuando se abre el modal
infoModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget; // botón que abrió el modal
    const telefonoCompleto = button.getAttribute('data-telefono') || '';
    let caracteristica = '';
    let numero = '';

    // Separar característica y número
    const partes = telefonoCompleto.trim().split(' ');
    if (partes.length >= 2) {
        caracteristica = partes[0];
        numero = partes.slice(1).join(' ');
    } else {
        numero = telefonoCompleto;
    }

    // Cargar datos en los inputs
    document.getElementById('infoCaracteristicaTelefono').value = caracteristica;
    document.getElementById('infoNumeroTelefono').value = numero;
    document.getElementById('infoDni').value = button.getAttribute('data-dni');
    document.getElementById('infoNombre').value = button.getAttribute('data-nombre');
    document.getElementById('infoApellido').value = button.getAttribute('data-apellido');
    document.getElementById('infoLocalidad').value = button.getAttribute('data-localidad');
    document.getElementById('infoDireccion').value = button.getAttribute('data-direccion');
    document.getElementById('infoRol').value = button.getAttribute('data-rol');

    // Al abrir el modal
    document.querySelectorAll('#infoUsuarioForm input').forEach(i => {
        // Solo deshabilito los que sí quiero bloquear
        if (i.id !== 'infoDni' && i.id !== 'infoRol') {
            i.disabled = true;
        } else {
            // Para DNI y Rol → los dejo readonly (así viajan en el form)
            i.readOnly = true;
            i.disabled = false;
        }
    });    document.getElementById('btnConfirmar').disabled = true;

    // Asegurar que rol y fecha siempre estén deshabilitados
    document.getElementById('infoRol').disabled = true;

    document.getElementById('infoUsuarioForm').setAttribute('method', 'post');

});

// Botón Modificar → habilita todo excepto DNI, Rol y Fecha
const btnModificar = document.getElementById('btnModificar');
if (btnModificar) {
    btnModificar.addEventListener('click', function () {
        document.querySelectorAll('#infoUsuarioForm input').forEach(i => {
            if (i.id !== 'infoDni' && i.id !== 'infoRol') {
                i.disabled = false;
            }
        });
        document.getElementById('btnConfirmar').disabled = false;
    });
}

// ==========================
//    FILTRO Y BUSCADOR
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


//////////////////////////
// VALIDICION
///////////////////////////
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("infoUsuarioForm");
    const errorDiv = document.getElementById("errorMensaje");
    const successDiv = document.getElementById("successMensaje");

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        // Validación de campos requeridos
        const camposInvalidos = [];
        form.querySelectorAll("input[required]").forEach(input => {
            if (!input.value.trim()) {
                const label = form.querySelector(`label[for="${input.id}"]`);
                camposInvalidos.push(label ? label.innerText.replace(":", "") : input.name);
                input.classList.add("is-invalid");
            } else {
                input.classList.remove("is-invalid");
            }
        });

        // Preparar envío
        const formData = new FormData(form);
        const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : "";

        // Deshabilitar botón para evitar envíos múltiples
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) submitBtn.disabled = true;

        try {
            const response = await fetch(form.action, {
                method: "POST",
                headers: { "RequestVerificationToken": token },
                body: formData
            });

            // Cerrar modal automáticamente
            const modalEl = document.getElementById("informacionUsuario");
            const modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) {
                setTimeout(() => modal.hide(), 1500);
            }

            // Opcional: actualizar tabla de usuarios sin recargar
            if (typeof actualizarTablaUsuarios === "function") {
                actualizarTablaUsuarios();
            }

        } catch (err) {
            console.error(err);
        } finally {
            if (submitBtn) submitBtn.disabled = false;
        }
    });
});
