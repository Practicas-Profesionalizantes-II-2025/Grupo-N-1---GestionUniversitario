using Datos.Repositories.Contracts;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;

namespace Logica.Implementations
{
    public class DiaHorarioLogic : IDiaHorarioLogic
    {
        private IDiaHorarioRepository _diaHorarioRepository;
        private IDiaRepository _diaRepository;
        private IHorarioRepository _horarioRepository;


        public DiaHorarioLogic(IDiaHorarioRepository diaHorarioRepository, IDiaRepository diaRepository, IHorarioRepository horarioRepository)
        {
            _diaHorarioRepository = diaHorarioRepository;;
            _horarioRepository = horarioRepository;
            _diaRepository = diaRepository;
        }

        public async Task<List<DiaHorarioDTO>> ObtenerDiasHorarios()
        {
            try
            {
                List<DiaHorario> listaDiasHorarios = (await _diaHorarioRepository.FindAllAsync()).ToList();

                if (listaDiasHorarios == null)
                {
                    return null;
                }

                List<DiaHorarioDTO> listaDiasHorariosDTO = new List<DiaHorarioDTO>();
                foreach(DiaHorario diaHorario in listaDiasHorarios)
                {
                    Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.ID == diaHorario.IdDia)).FirstOrDefault();
                    Horario? horario = (await _horarioRepository.FindByConditionAsync(d => d.ID == diaHorario.IdHorario)).FirstOrDefault();

                    listaDiasHorariosDTO.Add(new DiaHorarioDTO()
                    {
                        ID = diaHorario.ID,
                        DescripcionDia = dia.Descripcion,
                        DescripcionHorario = horario.Descripcion
                    });
                }

                return listaDiasHorariosDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<DiaHorarioDTO> ObtenerDiaHorarioID(int id)
        {
            DiaHorario? diaHorario = (await _diaHorarioRepository.FindByConditionAsync(t => t.ID == id)).FirstOrDefault();

            if (diaHorario == null)
            {
                return null;
            }

            Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.ID == diaHorario.IdDia)).FirstOrDefault();
            Horario? horario = (await _horarioRepository.FindByConditionAsync(d => d.ID == diaHorario.IdHorario)).FirstOrDefault();

            DiaHorarioDTO diaHorarioDTO = new DiaHorarioDTO()
            {
                ID = diaHorario.ID,
                DescripcionDia = dia.Descripcion,
                DescripcionHorario = horario.Descripcion
            };

            return diaHorarioDTO;
        }
        public async Task<DiaHorario?> ObtenerDiaHorarioPorDescripcionUsoInterno(string descripcionDiaHorario)
        {
            string[] partes = descripcionDiaHorario.Split(' ', 2);
            string descripcionDia = partes[0];
            string descripcionHorario = partes.Length > 1 ? partes[1] : "";

            Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.Descripcion == descripcionDia)).FirstOrDefault();
            Horario? horario = (await _horarioRepository.FindByConditionAsync(h => h.Descripcion == descripcionHorario)).FirstOrDefault();

            if (dia == null || horario == null)
            {
                return null;
            }

            int idDia = dia.ID;
            int idHorario = horario.ID;

            return (await _diaHorarioRepository
                        .FindByConditionAsync(dh => dh.IdDia == idDia && dh.IdHorario == idHorario))
                        .FirstOrDefault();
        }
        public async Task<String> ObtenerDescripcionDiaHorarioPorIDsUsoInterno(int idDia, int idHorario)
        {
            Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.ID == idDia)).FirstOrDefault();
            Horario? horario = (await _horarioRepository.FindByConditionAsync(d => d.ID == idHorario)).FirstOrDefault();

            if (dia == null || horario == null)
            {
                return null;
            }

            return $"{dia.Descripcion} {horario.Descripcion}";
        }
        public async Task<String> ObtenerDescripcionDiaHorarioPorDiaHorario(DiaHorario diaHorario)
        {
            Dia? dia = (await _diaRepository.FindByConditionAsync(d => d.ID == diaHorario.IdDia)).FirstOrDefault();
            Horario? horario = (await _horarioRepository.FindByConditionAsync(d => d.ID == diaHorario.IdHorario)).FirstOrDefault();

            if (dia == null || horario == null)
            {
                return null;
            }

            return $"{dia.Descripcion} {horario.Descripcion}";
        }
    }
}