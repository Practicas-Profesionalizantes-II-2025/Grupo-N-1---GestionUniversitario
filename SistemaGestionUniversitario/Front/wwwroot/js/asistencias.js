// Boton cancelar
document.getElementById("btnCancelar").addEventListener("click", function () {
    // Selecciona todos los checkboxes
    document.querySelectorAll(".asistencia").forEach(cb => {
        cb.checked = true; // los marca
    });
});

//Boton aceptar
document.getElementById("confirmCheckbox").addEventListener("change", function () {
    document.getElementById("btnAceptarFinal").disabled = !this.checked;
});

let selectMateria = document.getElementById("materia");
let nombreMateria = selectMateria.options[selectMateria.selectedIndex].text;

// Antes de enviar el form, agregamos los checkboxes marcados
document.getElementById("confirmForm").addEventListener("submit", function (e) {
    // limpiar viejos hidden inputs
    document.querySelectorAll("#confirmForm input[name='asistencias.Index']").forEach(el => el.remove());
    document.querySelectorAll("#confirmForm input[name^='asistencias']").forEach(el => el.remove());

    let asistencias = [];
    document.querySelectorAll(".asistencia").forEach(cb => {
        asistencias.push({
            idInscripcion: cb.value,
            nombreMateria: nombreMateria,
            Estado: cb.checked,
            Fecha: document.getElementById("fechaAsistencia").value
        });
    });

    // crear inputs hidden para que viajen en el POST
    asistencias.forEach((a, i) => {
        this.insertAdjacentHTML("beforeend", `
            <input type="hidden" name="asistencias[${i}].idInscripcion" value="${a.idInscripcion}" />
            <input type="hidden" name="asistencias[${i}].nombreMateria" value="${a.nombreMateria}" />
            <input type="hidden" name="asistencias[${i}].Estado" value="${a.Estado}" />
            <input type="hidden" name="asistencias[${i}].Fecha" value="${a.Fecha}" />
        `);
    });
});