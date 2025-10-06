namespace Front.Models.Crear
{
    public class CrearNotaAlumnoFront
    {
        public int Nota { get; set; }
        required
        public string DNIAlumno { get; set; }
        public int IDExamen { get; set; }
    }
}