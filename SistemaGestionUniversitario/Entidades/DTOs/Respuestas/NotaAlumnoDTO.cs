﻿namespace Entidades.DTOs.Respuestas
{
    public class NotaAlumnoDTO
    {
        public int ID { get; set; }
        public string AlumnoNombre { get; set; }
        public string ExamenMateriaNombre { get; set; }
        public string ExamenTipo { get; set; }
        public int Nota { get; set; }
    }
}