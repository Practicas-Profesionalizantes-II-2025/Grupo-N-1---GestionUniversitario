using Front.Models.Respuestas;

namespace Front.Models.Modificar
{
    public class ModificarMateriaFront
    {
        public string MateriaSeleccionada { get; set; } = string.Empty;
        public List<MateriaFront> TodasMaterias { get; set; } = new();

        public List<ProfesorFront> Profesores { get; set; } = new();
        public List<DiaHorarioFront> DiasHorarios { get; set; } = new();

        // Datos modificables que se enviarán al backend
        public List<int> ProfesoresIDs { get; set; } = new();
        public List<int> DiasHorariosIDs { get; set; } = new();
    }
}