using Datos.Repositories.Contracts;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;

namespace Logica.Implementations
{
    public class ExamenLogic : IExamenLogic
    {
        private IExamenRepository _examenRepository;
        private IMateriaRepository _materiaRepository;
        private IDiaHorarioRepository _diaHorarioRepository;
        private IDiaHorarioLogic _diaHorarioLogic;


        public ExamenLogic(IExamenRepository examenRepository, IMateriaRepository materiaRepository, IDiaHorarioRepository diaHorarioRepository, IDiaHorarioLogic diaHorarioLogic)
        {
            _examenRepository = examenRepository;
            _materiaRepository = materiaRepository;
            _diaHorarioRepository = diaHorarioRepository;
            _diaHorarioLogic = diaHorarioLogic;
        }

        public async Task AltaExamen(string nombreMateria, string descripcionDiaHorario, string tipoExamen, DateTime fechaExistente)
        {
            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).FirstOrDefault();
            DiaHorario? diaHorarioExistente = await _diaHorarioLogic.ObtenerDiaHorarioPorDescripcionUsoInterno(descripcionDiaHorario);
            
            if (materiaExistente == null)
            {
                throw new ArgumentNullException("El examen debe estar vinculado a una materia.");
            }

            if (diaHorarioExistente == null)
            {
                throw new ArgumentNullException("El examen debe estar vinculado a un dia y horario.");
            }

            if (!ValidacionesCampos.TipoExamenEsValido(tipoExamen))
            {
                throw new ArgumentNullException("El tipo de examen no es valido.");
            }
            if (fechaExistente == null)
            {
                throw new ArgumentNullException("El examen debe estar vinculado a una fecha.");
            }

            Examen? examenExistente = (await _examenRepository.FindByConditionAsync(p => p.Materia == materiaExistente && p.DiaHorario == diaHorarioExistente)).FirstOrDefault();
            
            if (examenExistente != null)
            {
                throw new InvalidOperationException("Ya existe un examen de la materia seleccionada en el dia y hora ingresado.");
            }

            Examen examenNuevo = new Examen()
            {
                Tipo = tipoExamen,
                Materia = materiaExistente,
                DiaHorario = diaHorarioExistente,
                Fecha = fechaExistente
            };

            await _examenRepository.AddAsync(examenNuevo);
            await _examenRepository.SaveAsync();
        }
        public async Task<ExamenDTO> ActualizacionExamen(string nombreMateria, string descripcionDiaHorario, int idNuevoDiaHorario, DateTime fecha)
        {
            DiaHorario? diaHorario = await _diaHorarioLogic.ObtenerDiaHorarioPorDescripcionUsoInterno(descripcionDiaHorario);
            if (diaHorario == null)
            {
                throw new ArgumentNullException("La materia no tiene un examen para el dia y horario ingresado o el dia y horario son incorrectos.");
            }

            Examen? examenExistente = (await _examenRepository.FindByConditionAsync(p => p.Materia.Nombre == nombreMateria && p.DiaHorario.ID == diaHorario.ID)).FirstOrDefault();

            if (examenExistente == null)
            {
                throw new ArgumentNullException("El examen que se quiere actualizar no existe.");
            }
            
            DiaHorario? nuevoDiaHorario = (await _diaHorarioRepository.FindByConditionAsync(dh => dh.ID == idNuevoDiaHorario)).FirstOrDefault();

            if (nuevoDiaHorario == null)
            {
                throw new ArgumentNullException("El dia y horario al que se quiere cambiar el examen no existe.");
            }
            if (fecha == null)
            {
                throw new ArgumentNullException("La fecha que se quiere actualizar no existe.");
            }

            examenExistente.DiaHorario = nuevoDiaHorario;

            _examenRepository.Update(examenExistente);
            await _examenRepository.SaveAsync();

            ExamenDTO examenExistenteDTO = new ExamenDTO()
            {
                ID = examenExistente.ID,
                NombreMateria = examenExistente.Materia.Nombre,
                DescripcionDiaHorario = await _diaHorarioLogic.ObtenerDescripcionDiaHorarioPorIDsUsoInterno(examenExistente.DiaHorario.IdDia, examenExistente.DiaHorario.IdHorario),
                Fecha = fecha
            };

            return examenExistenteDTO;
        }
        public async Task BajaExamen(string nombreMateria, string descripcionDiaHorario, DateTime fecha)
        {
            DiaHorario? diaHorario = await _diaHorarioLogic.ObtenerDiaHorarioPorDescripcionUsoInterno(descripcionDiaHorario);
            if (diaHorario == null)
            {
                throw new ArgumentNullException("La materia no tiene un examen para el dia y horario ingresado o el dia y horario son incorrectos.");
            }

            Examen? examenEliminar = (await _examenRepository.FindByConditionAsync(p => p.Materia.Nombre == nombreMateria && p.DiaHorario.ID == diaHorario.ID)).FirstOrDefault();

            if (examenEliminar == null)
            {
                throw new InvalidOperationException("El examen que se desea eliminar no existe.");
            }

            _examenRepository.Remove(examenEliminar);
            await _examenRepository.SaveAsync();
        }
        public async Task<List<ExamenDTO>> ObtenerExamenes()
        {
            try
            {
                List<Examen> listaExamenes = (await _examenRepository.FindAllAsync()).ToList();

                if (listaExamenes == null)
                {
                    return null;
                }

                List<ExamenDTO> listaExamenesDTO = new List<ExamenDTO>();
                foreach(Examen examen in listaExamenes)
                {
                    listaExamenesDTO.Add(new ExamenDTO()
                    {
                        ID = examen.ID,
                        Tipo=examen.Tipo,
                        NombreMateria = examen.Materia.Nombre,
                        DescripcionDiaHorario = await _diaHorarioLogic.ObtenerDescripcionDiaHorarioPorIDsUsoInterno(examen.DiaHorario.IdDia, examen.DiaHorario.IdHorario),
                        Fecha = examen.Fecha,
                    });
                }

                return listaExamenesDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<ExamenDTO>> ObtenerExamenesPorMateria(string nombreMateria)
        {
            List<Examen> listaExamenes = (await _examenRepository.FindByConditionAsync(t => t.Materia.Nombre == nombreMateria)).ToList();

            List<ExamenDTO> listaExamenesDTO = new List<ExamenDTO>();
            foreach (Examen examen in listaExamenes)
            {
                listaExamenesDTO.Add(new ExamenDTO()
                {
                    ID = examen.ID,
                    Tipo = examen.Tipo,
                    NombreMateria = examen.Materia.Nombre,
                    DescripcionDiaHorario = await _diaHorarioLogic.ObtenerDescripcionDiaHorarioPorIDsUsoInterno(examen.DiaHorario.IdDia, examen.DiaHorario.IdHorario),
                    Fecha = examen.Fecha,
                });
            }

            return listaExamenesDTO;
        }
    }
}