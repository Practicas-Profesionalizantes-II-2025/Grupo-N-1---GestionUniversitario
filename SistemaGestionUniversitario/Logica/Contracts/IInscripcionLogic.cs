using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface IInscripcionLogic
    {
        Task AltaInscripcion(string dniAlumno, string nombreMateria);
        Task BajaInscripcion(string dniAlumno, string nombreMateria);
        Task<List<InscripcionDTO>> ObtenerInscripciones();
        Task<List<InscripcionDTO>> ObtenerInscripcionesMateria(string nombreMateria);
        Task<List<InscripcionDTO>> ObtenerInscripcionesDNI(string dni);
    }
}