using Front.Models.Respuestas;
using System.ComponentModel.DataAnnotations;

namespace Front.Models.Crear
{
    public class CrearMateriaFront
    {
        [Required (ErrorMessage = "El campo nombre es obligatorio")]
        public string Nombre { get; set; }
        public string Anio { get; set; }
        public string Modalidad { get; set; }
        public List<int> ProfesoresIDs { get; set; } = new();
        public List<int> DiasHorariosIDs { get; set; }= new();
        public List<ProfesorFront> Profesores { get; set; } = new();
        public List<DiaHorarioFront> DiasHorarios { get; set; } = new();
    }
}