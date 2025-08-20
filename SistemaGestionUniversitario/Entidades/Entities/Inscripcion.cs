namespace Entidades.Entities
{
    public class Inscripcion : EntityBase
    {
        public int IdAlumno { get; set; }
        public int IdMateria { get; set; }
        public bool Estado { get; set; }

        public ICollection<DiaHorarioMateria> DiaHorarioMaterias { get; set; }
    }
}