namespace Front.Models.Respuestas
{
    public class ProfesorFront
    {
        public int ID { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaInicioContrato { get; set; }
        public string NombreCompleto => $"{Apellido}, {Nombre}";
    }
}