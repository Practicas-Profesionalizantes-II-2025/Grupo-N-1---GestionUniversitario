namespace Entidades.DTOs.Crear
{
    public class CrearExamenDTO
    {
        public string Tipo { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionDiaHorario { get; set; }
        public DateTime Fecha { get; set; }    // Solo día, mes y año

    }
}