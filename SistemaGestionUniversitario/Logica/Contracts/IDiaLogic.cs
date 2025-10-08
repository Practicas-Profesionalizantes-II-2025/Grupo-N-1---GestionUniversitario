using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface IDiaLogic
    {
        Task<List<DiaDTO>> ObtenerDias();
        Task<DiaDTO> ObtenerDiaId(int id);
        Task<String> ObtenerDescripcionDiaPorIDUsoInterno(int idDia);
    }
}