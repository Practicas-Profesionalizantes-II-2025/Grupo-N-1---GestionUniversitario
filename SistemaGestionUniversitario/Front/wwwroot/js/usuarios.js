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

//buscador
document.getElementById('searchBox').addEventListener('input', function () {
    let filtro = this.value.toLowerCase();
    let filas = document.querySelectorAll('#tablaUsuarios tbody tr');
    console.log('Filas encontradas:', filas.length);

    filas.forEach(fila => {
        let texto = fila.textContent.toLowerCase();
        console.log('Fila:', texto);
        fila.style.display = texto.includes(filtro) ? '' : 'none';
    });
    const coincideBusqueda = textoBusqueda === "" || textoFila.includes(textoBusqueda);
});

//filtro
document.addEventListener('DOMContentLoaded', function () {
    const searchBox = document.getElementById('searchBox');
    const filtroRol = document.getElementById('filtroRol');
    const ordenNombre = document.getElementById('ordenNombre');
    const btnAplicarFiltro = document.getElementById('btnAplicarFiltro');
    const tbody = document.querySelector('#tablaUsuarios tbody');

    // Copia original de todas las filas
    const filasOriginales = Array.from(tbody.querySelectorAll('tr'));

    function aplicarFiltro() {
        const textoBusqueda = searchBox.value.toLowerCase();
        const rolSeleccionado = filtroRol.value.toLowerCase();

        let filtradas = filasOriginales.filter(fila => {
            const textoFila = fila.textContent.toLowerCase();
            const rolFila = fila.cells[0].textContent.toLowerCase();
            const coincideBusqueda = textoFila.includes(textoBusqueda);
            const coincideRol = rolSeleccionado === "" || rolFila === rolSeleccionado;
            return coincideBusqueda && coincideRol;
        });

        // Orden por nombre
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

        tbody.innerHTML = '';
        filtradas.forEach(fila => tbody.appendChild(fila));
    }

    // Eventos
    filtroRol.addEventListener('change', aplicarFiltro);
    ordenNombre.addEventListener('change', aplicarFiltro);

    //modificar ussuario
    document.addEventListener("DOMContentLoaded", function () {
        const selectUsuario = document.getElementById("usuarioExistente");

        selectUsuario.addEventListener("change", async function () {
            const userId = this.value;
            if (!userId) return; // si no selecciona nada, no hago nada

            try {
                const response = await fetch(`/Usuario/GetUsuario/${userId}`);
                if (!response.ok) throw new Error("Error al obtener usuario");

                const usuario = await response.json();

                // autocompleto los campos
                document.querySelector("[name='Nombre']").value = usuario.nombre || "";
                document.querySelector("[name='Apellido']").value = usuario.apellido || "";
                document.querySelector("[name='Password']").value = usuario.password || "";
                document.querySelector("[name='Direccion']").value = usuario.direccion || "";
                document.querySelector("[name='CaracteristicaTelefono']").value = usuario.caracteristicaTelefono || "";
                document.querySelector("[name='NumeroTelefono']").value = usuario.numeroTelefono || "";
                document.querySelector("[name='Localidad']").value = usuario.localidad || "";
                document.querySelector("[name='RolUsuarioDescripcion']").value = usuario.rolUsuarioDescripcion || "";
                document.querySelector("[name='FechaContratoIngreso']").value = usuario.fechaContratoIngreso?.split('T')[0] || "";

            } catch (error) {
                console.error(error);
            }
        });
    });


