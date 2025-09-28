namespace Entidades.DTOs.Respuestas
{
    public class ExamenDTO
    {
        public int ID { get; set; }
        public string Tipo { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionDiaHorario { get; set; }
        public DateTime Fecha { get; set; }    // Solo día, mes y año

    }
}