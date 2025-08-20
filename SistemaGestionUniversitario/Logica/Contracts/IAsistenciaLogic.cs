using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface IAsistenciaLogic
    {
        Task AltaAsistencia(int idinscripcion, int iddiahorariomateria, bool estado, DateTime fecha);
        Task<AsistenciaDTO> ActualizarAsistencia(string dniAlumno, string nombreMateria, DateTime fecha, bool estado);
        Task EliminarAsistencia(string dniAlumno, string nombreMateria, DateTime fecha);
        Task<List<AsistenciaDTO>> ObtenerAsistenciasPorMateria(string nombreMateria);
        Task<List<AsistenciaDTO>> ObtenerInasistenciasPorAlumno(string dni);
    }
}