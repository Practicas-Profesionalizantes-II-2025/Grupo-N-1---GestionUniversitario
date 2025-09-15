namespace Entidades.DTOs.Crear
{
    public class CrearAsistenciaDTO
    {
        public int idInscripcion { get; set; }
        public string nombreMateria { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}