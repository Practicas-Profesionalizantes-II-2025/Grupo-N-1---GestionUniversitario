using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Front.Models.Respuestas
{
    public class ExamenFront
    {
        public int ID { get; set; }
        [Display(Name = "Tipo Examen")]
        public string Tipo { get; set; }
        [Display(Name = "Nombre Materia")]
        public string NombreMateria { get; set; }
        [Display(Name = "Fecha")]
        public string DescripcionDiaHorario { get; set; }
    }
}