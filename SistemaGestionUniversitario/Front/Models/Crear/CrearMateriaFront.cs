using Front.Models.Respuestas;
using System.ComponentModel.DataAnnotations;

namespace Front.Models.Crear
{
    public class CrearMateriaFront
    {
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        public string Anio { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria")]
        public string Modalidad { get; set; }

        // IDs seleccionados en el form
        public List<int> ProfesoresIDs { get; set; } = new();
        public List<int> DiasHorariosIDs { get; set; } = new();

        // Listas para poblar dropdowns
        public List<ProfesorFront> Profesores { get; set; } = new();
        public List<DiaHorarioFront> DiasHorarios { get; set; } = new();
    }
}