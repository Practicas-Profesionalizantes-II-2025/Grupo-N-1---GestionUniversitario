using Front.Models.Respuestas;
using System.ComponentModel.DataAnnotations;

namespace Front.Models.Modificar
{
    public class ModificarMateriaFront
    {
        public string MateriaSeleccionada { get; set; } = string.Empty;
        public List<MateriaFront> TodasMaterias { get; set; } = new();

        [Required(ErrorMessage = "El campo profesor es obligatorio")]
        public List<ProfesorFront> Profesores { get; set; } = new();
        [Required(ErrorMessage = "El campo Días y Horarios es obligatorio")]
        public List<DiaHorarioFront> DiasHorarios { get; set; } = new();

        // Datos modificables que se enviarán al backend
        public List<int> ProfesoresIDs { get; set; } = new();
        public List<int> DiasHorariosIDs { get; set; } = new();
    }
}