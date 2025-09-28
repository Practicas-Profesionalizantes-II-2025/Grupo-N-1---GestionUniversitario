namespace Entidades.DTOs.Modificar
{
    public class ModificarExamenDTO
    {
        public int ID { get; set; }
        public string DescripcionDiaHorario { get; set; }
        public string NombreMateria { get; set; }

        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
    }
}