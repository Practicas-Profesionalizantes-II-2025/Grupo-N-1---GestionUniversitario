using Entidades.DTOs.Respuestas;

namespace Logica.Contracts
{
    public interface IMateriaLogic
    {
        Task AltaMateria(string nombre, List<int> listaProfesores, List<int> listaHorarios, string modalidad, string anio);
        Task BajaMateria(string nombre);
        Task<MateriaDTO> ActualizacionMateria(string nombre, List<int> listaProfesoresID, List<int> listaDiasHorariosID);
        Task<List<MateriaDTO>> ObtenerMaterias();
        Task<MateriaDTO> ObtenerMateriaNombre(string nombre);
        Task<List<MateriaDTO>> ObtenerMateriasDNIProfesor(string dni);
    }
}