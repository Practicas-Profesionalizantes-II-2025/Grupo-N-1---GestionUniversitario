using System.Text.Json.Serialization;

namespace Front.Models.Crear
{
    public class CrearExamenFront
    {
        public string Tipo { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionDiaHorario {  get; set; }
        public DateTime Fecha { get; set; }
    }
}