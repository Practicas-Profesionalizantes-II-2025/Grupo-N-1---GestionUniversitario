using Datos.Repositories.Contracts;
using Datos.Repositories.Implementations;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;

namespace Logica.Implementations
{
    public class DiaLogic : IDiaLogic
    {
        private IDiaRepository _diaRepository;

        public DiaLogic(IDiaRepository diaRepository) 
        {
            _diaRepository = diaRepository;
        }

        public async Task<List<DiaDTO>> ObtenerDias()
        {
            try
            {
                List<Dia> listaDias = (await _diaRepository.FindAllAsync()).ToList();

                if (listaDias == null)
                {
                    return null;
                }

                List<DiaDTO> listaDiasDTO = listaDias.Select(t => new DiaDTO
                {
                    ID = t.ID,
                    Descripcion = t.Descripcion
                }).ToList();

                return listaDiasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<DiaDTO> ObtenerDiaId(int id)
        {
            Dia? dia = (await _diaRepository.FindByConditionAsync(t => t.ID == id)).FirstOrDefault();

            if (dia == null)
            {
                return null;
            }

            DiaDTO diaDTO = new DiaDTO()
            {
                ID = dia.ID,
                Descripcion = dia.Descripcion
            };

            return diaDTO;
        }
        public async Task<String> ObtenerDescripcionDiaPorIDUsoInterno(int idDia)
        {
            Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.ID == idDia)).FirstOrDefault();

            if (dia == null)
            {
                return null;
            }

            return $"{dia.Descripcion}";
        }
    }
}