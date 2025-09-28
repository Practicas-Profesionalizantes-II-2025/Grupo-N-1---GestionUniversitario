using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface IExamenLogic
    {
        Task AltaExamen(string nombreMateria, string descripcionDiaHorario, string tipoExamen,DateTime fechaExistente);
        Task<ExamenDTO> ActualizacionExamen(int id,string nombreMateria, string descripcionDiaHorario, string tipo, DateTime fecha);
        Task BajaExamen(string nombreMateria, string descripcionDiaHorario, DateTime fecha);
        Task<List<ExamenDTO>> ObtenerExamenes();
        Task<List<ExamenDTO>> ObtenerExamenesPorMateria(string nombreMateria);
    }
}