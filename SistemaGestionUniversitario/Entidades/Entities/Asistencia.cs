namespace Entidades.Entities
{
    public class Asistencia : EntityBase
    {
        public int IdInscripcion {  get; set; }
        public int? IdDiaHorarioMateria { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}