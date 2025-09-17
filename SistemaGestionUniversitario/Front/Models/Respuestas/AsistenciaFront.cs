namespace Front.Models.Respuestas
{
    public class AsistenciaFront
    {
        public int ID { get; set; }
        public int IdInscripcion { get; set; }
        public string DniAlumno { get; set; }
        public string NombreMateria { get; set; }
        public string NombreAlumno { get; set; }
        public string ApellidoAlumno { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}