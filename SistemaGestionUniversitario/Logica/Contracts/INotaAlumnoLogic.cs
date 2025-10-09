using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface INotaAlumnoLogic
    {
        Task AltaNotaAlumno(int nota, string dniAlumno, int idExamen);
        Task BajaNotaAlumno(string dniAlumno, int idExamen);
        Task<NotaAlumnoDTO> ActualizacionNotaAlumno(int nota, string dniAlumno, int idExamen);
        Task<List<NotaAlumnoDTO>> ObtenerNotas();
        Task<List<NotaAlumnoDTO>> ObtenerNotasPorMateria(string nombreMateria);
        Task<List<NotaAlumnoDTO>> ObtenerNotasPorAlumno(string dniAlumno);
        Task<List<NotaAlumnoDTO>> ObtenerNotasPorExamen(int idexamen);
    }
}