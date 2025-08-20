namespace Entidades.DTOs.Respuestas
{
    public class InscripcionDTO
    {
        public int ID { get; set; }

        public int IdAlumno { get; set; }
        public string NombreAlumno { get; set; }
        public string ApellidoAlumno { get; set; }
        public string DNIAlumno { get; set; }
        public string Estado { get; set; }

        public int IdMateria { get; set; }
        public string NombreMateria { get; set; }
    }
}