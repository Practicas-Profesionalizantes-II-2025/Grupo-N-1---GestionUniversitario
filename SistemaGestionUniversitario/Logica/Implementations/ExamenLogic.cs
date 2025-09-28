using Datos.Repositories.Contracts;
using Datos.Repositories.Implementations;
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
        private IDiaRepository _diaRepository;
        private IHorarioRepository _horarioRepository;

        public ExamenLogic(IExamenRepository examenRepository, IMateriaRepository materiaRepository, IDiaHorarioRepository diaHorarioRepository, IDiaHorarioLogic diaHorarioLogic, IDiaRepository diaRepository, IHorarioRepository horarioRepository)
        {
            _examenRepository = examenRepository;
            _materiaRepository = materiaRepository;
            _diaHorarioRepository = diaHorarioRepository;
            _diaHorarioLogic = diaHorarioLogic;
            _diaRepository = diaRepository;
            _horarioRepository = horarioRepository;
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
        public async Task<ExamenDTO> ActualizacionExamen(int id,string nombreMateria ,string descripcionDiaHorario, string Tipo, DateTime fecha)
        {
            var partes = descripcionDiaHorario.Split(' ', 2, StringSplitOptions.TrimEntries);
            if (partes.Length < 2)
                throw new InvalidOperationException("La descripción del horario no tiene el formato esperado: 'Día HH:mm - HH:mm'");

            string nombreDia = partes[0];
            string franjaHoraria = partes[1];

            // 1. ID del día
            var diaId = (await _diaRepository
                .FindByConditionAsync(d => d.Descripcion == nombreDia))
                .Select(d => d.ID)
                .FirstOrDefault();

            // 2. ID del horario (por ejemplo por hora de inicio y fin)
            var horarioId = (await _horarioRepository
                .FindByConditionAsync(h => h.Descripcion== franjaHoraria))
                .Select(h => h.ID)
                .FirstOrDefault();

            // 3. ID de la tabla intermedia DiaHorario
            var diaHorarioId = (await _diaHorarioRepository
                .FindByConditionAsync(dh => dh.IdDia == diaId && dh.IdHorario == horarioId))
                .Select(dh => dh.ID)
                .FirstOrDefault();

            DiaHorario? diaHorarioDescripcion = await _diaHorarioLogic.ObtenerDiaHorarioPorDescripcionUsoInterno(descripcionDiaHorario);

            if (diaHorarioId == null)
            {
                throw new ArgumentNullException("La materia no tiene un examen para el dia y horario ingresado o el dia y horario son incorrectos.");
            }

            int materiaId = (int)(await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).Select(m => (int?)m.ID).FirstOrDefault();
            Materia? materiaNombre = (await _materiaRepository
                .FindByConditionAsync(m => m.Nombre == nombreMateria))
                .FirstOrDefault(); // devuelve null si no existe

            if (materiaId == null)
            {
                throw new ArgumentNullException("La materia no tiene un examen para el dia y horario ingresado o el dia y horario son incorrectos.");
            }

            if (fecha == null)
            {
                throw new ArgumentNullException("La fecha que se quiere actualizar no existe.");
            }

            Examen? examenExistente = (await _examenRepository
                .FindByConditionAsync(e => e.ID == id))
                .SingleOrDefault();

            examenExistente.Materia= materiaNombre;
            examenExistente.DiaHorario = diaHorarioDescripcion;
            examenExistente.Tipo = Tipo;
            examenExistente.Fecha = fecha;

            _examenRepository.Update(examenExistente);
            await _examenRepository.SaveAsync();

            ExamenDTO examenExistenteDTO = new ExamenDTO()
            {
                ID = examenExistente.ID,
                NombreMateria = materiaNombre.Nombre,
                DescripcionDiaHorario = descripcionDiaHorario,
                Tipo = Tipo,
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