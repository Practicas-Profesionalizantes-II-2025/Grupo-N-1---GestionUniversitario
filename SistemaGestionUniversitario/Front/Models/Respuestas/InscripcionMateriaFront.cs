namespace Front.Models.Respuestas
{
    public class InscripcionMateriaFront
    {
        public int IdMateria { get; set; }
        public string Nombre { get; set; }
        public int Anio { get; set; }
        public string Modalidad { get; set; }
        public List<string> NombresProfesores { get; set; }
        public List<string> DescripcionDiasHorarios { get; set; }

        public bool EstaInscripto { get; set; }
    }
}