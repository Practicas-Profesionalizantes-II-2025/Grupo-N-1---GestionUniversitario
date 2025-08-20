using Logica.Contracts;
using Datos.Repositories.Contracts;
using Entidades.Entities;
using Entidades.DTOs.Respuestas;

namespace Logica.Implementations
{
    public class AsistenciaLogic : IAsistenciaLogic
    {
        IAsistenciaRepository _asistenciaRepository;
        IAlumnoRepository _alumnoRepository;
        IMateriaRepository _materiaRepository;
        IInscripcionRepository _inscripcionRepository;
        IDiaHorarioRepository _diaHorarioRepository;
        IDiaRepository _diaRepository;
        IHorarioRepository _horarioRepository;
        IDiaHorarioMateriaRepository _diaHorarioMateriaRepository;

        public AsistenciaLogic(IAsistenciaRepository asistenciaRepository, IAlumnoRepository alumnoRepository, IMateriaRepository materiaRepository, IInscripcionRepository inscripcionRepository, IDiaHorarioRepository diaHorarioRepository, IDiaRepository diaRepository, IHorarioRepository horarioRepository, IDiaHorarioMateriaRepository diaHorarioMateriaRepository)
        {
            _asistenciaRepository = asistenciaRepository;
            _alumnoRepository = alumnoRepository;
            _materiaRepository = materiaRepository;
            _inscripcionRepository = inscripcionRepository;
            _diaHorarioRepository = diaHorarioRepository;
            _diaRepository = diaRepository;
            _horarioRepository = horarioRepository;
            _diaHorarioMateriaRepository = diaHorarioMateriaRepository;
        }

        public async Task AltaAsistencia(int idinscripcion, int iddiahorariomateria, bool estado, DateTime fecha)
        {
            List<string> camposErroneos = new List<string>();

            if (fecha == default)
            {
                camposErroneos.Add("Fecha");
            }

            if (camposErroneos.Count > 0)
            {
                throw new ArgumentException("Los siguientes campos son inválidos: ", string.Join(", ", camposErroneos));
            }

            Asistencia? asistenciaExistente = (await _asistenciaRepository.FindByConditionAsync(p => p.IdDiaHorarioMateria == iddiahorariomateria && p.IdInscripcion == idinscripcion)).FirstOrDefault();
            if (asistenciaExistente != null)
            {
                throw new InvalidOperationException("Ya existe una asistencia registrada para la misma materia, día y hora.");
            }

            Asistencia asistenciaNueva = new Asistencia()
            {
                IdInscripcion = idinscripcion,
                IdDiaHorarioMateria = iddiahorariomateria,
                Estado = estado,
                Fecha = fecha,
            };

            await _asistenciaRepository.AddAsync(asistenciaNueva);
            await _asistenciaRepository.SaveAsync();
        }
        public async Task<AsistenciaDTO> ActualizarAsistencia(string dniAlumno, string nombreMateria, DateTime fecha, bool estado)
        {
            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario.DNI == dniAlumno)).SingleOrDefault();
            if (alumnoExistente == null)
            {
                throw new InvalidOperationException("No se encontró un alumno con ese DNI.");
            }

            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
            if (materiaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la materia.");
            }

            Inscripcion? inscripcionExistente = (await _inscripcionRepository.FindByConditionAsync(i => i.IdAlumno == alumnoExistente.ID && i.IdMateria == materiaExistente.ID)).SingleOrDefault();
            if (inscripcionExistente == null)
            {
                throw new InvalidOperationException("El alumno no está inscripto en esa materia.");
            }

            Asistencia? asistenciaExistente = (await _asistenciaRepository.FindByConditionAsync(a => a.Fecha == fecha && a.IdInscripcion == inscripcionExistente.ID)).SingleOrDefault();
            if (asistenciaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la asistencia para ese día.");
            }

            asistenciaExistente.Estado = estado;

            _asistenciaRepository.Update(asistenciaExistente);
            await _asistenciaRepository.SaveAsync();

            var asistenciaDTO = new AsistenciaDTO
            {
                ID = asistenciaExistente.ID,
                DniAlumno = alumnoExistente.Usuario.DNI,
                NombreMateria = materiaExistente.Nombre,
                Estado = asistenciaExistente.Estado,
                Fecha = asistenciaExistente.Fecha
            };

            return asistenciaDTO;
        }
        public async Task EliminarAsistencia(string dniAlumno, string nombreMateria, DateTime fecha)
        {
            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario.DNI == dniAlumno)).SingleOrDefault();
            if (alumnoExistente == null)
            {
                throw new InvalidOperationException("No se encontró un alumno con ese DNI.");
            }

            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
            if (materiaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la materia.");

            }

            Inscripcion? inscripcionExistente = (await _inscripcionRepository.FindByConditionAsync(i => i.IdAlumno == alumnoExistente.ID && i.IdMateria == materiaExistente.ID)).SingleOrDefault();
            if (inscripcionExistente == null)
            {
                throw new InvalidOperationException("El alumno no está inscripto en esa materia.");
            }

            Asistencia? asistenciaExistente = (await _asistenciaRepository.FindByConditionAsync(a => a.Fecha == fecha && a.IdInscripcion == inscripcionExistente.ID)).SingleOrDefault();
            if (asistenciaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la asistencia para ese día.");
            }

            _asistenciaRepository.Remove(asistenciaExistente);
            await _asistenciaRepository.SaveAsync();
        }
        public async Task<List<AsistenciaDTO>> ObtenerAsistenciasPorMateria(string nombreMateria)
        {
            // 1. Buscar la materia
            Materia? materia = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
            if (materia == null)
            {
                throw new InvalidOperationException("No se encontró la materia.");
            }

            // 2. Buscar inscripciones "EN CURSO" de esa materia
            List<Inscripcion> inscripciones = (await _inscripcionRepository.FindByConditionAsync(i => i.IdMateria == materia.ID && i.Estado == false)).ToList();
            if (!inscripciones.Any())
                return new List<AsistenciaDTO>();

            // 3. Obtener todos los IDs de inscripciones
            var idsInscripciones = inscripciones.Select(i => i.ID).ToHashSet();

            // 4. Buscar todas las asistencias que correspondan a esas inscripciones
            var asistencias = (await _asistenciaRepository.FindAllAsync()).Where(a => idsInscripciones.Contains(a.IdInscripcion)).ToList();

            List<AsistenciaDTO> resultado = new();

            foreach (Asistencia asistencia in asistencias)
            {
                // Obtener datos relacionados

                Inscripcion inscripcion = inscripciones.First(i => i.ID == asistencia.IdInscripcion);

                Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.ID == inscripcion.IdAlumno)).FirstOrDefault();
                if (alumno == null || alumno.Usuario == null)
                    continue;

                // Crear DTO
                AsistenciaDTO asistenciaDTO = new AsistenciaDTO
                {
                    ID = asistencia.ID,
                    DniAlumno = alumno.Usuario.DNI,
                    NombreMateria = materia.Nombre,
                    Estado = asistencia.Estado,
                    Fecha = asistencia.Fecha
                };

                resultado.Add(asistenciaDTO);
            }

            return resultado;
        }
        public async Task<List<AsistenciaDTO>> ObtenerInasistenciasPorAlumno(string dni)
        {
            // 1. Buscar la materia
            Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario != null && a.Usuario.DNI == dni)).FirstOrDefault();
            if (alumno == null)
            {
                throw new InvalidOperationException("No se encontró el alumno con el DNI ingresado.");
            }

            // 2. Buscar inscripciones "EN CURSO" de esa materia
            List<Inscripcion> inscripciones = (await _inscripcionRepository.FindByConditionAsync(i => i.IdAlumno == alumno.ID && i.Estado == false)).ToList();
            if (!inscripciones.Any())
            {
                return new List<AsistenciaDTO>();
            }

            // 3. Obtener todos los IDs de inscripciones
            var idsInscripciones = inscripciones.Select(i => i.ID).ToHashSet();

            // 4. Buscar todas las asistencias que correspondan a esas inscripciones
            var asistencias = (await _asistenciaRepository.FindAllAsync()).Where(a => idsInscripciones.Contains(a.IdInscripcion) && a.Estado == false).ToList();

            List<AsistenciaDTO> resultado = new();

            foreach (Asistencia asistencia in asistencias)
            {
                // Obtener datos relacionados

                Inscripcion inscripcion = inscripciones.First(i => i.ID == asistencia.IdInscripcion);

                Materia? materia = (await _materiaRepository.FindByConditionAsync(a => a.ID == inscripcion.IdMateria)).FirstOrDefault();
                if (materia == null)
                {
                    throw new InvalidOperationException($"No se encontró la materia con ID {inscripcion.IdMateria} para la inscripción {inscripcion.ID}.");
                }

                // Crear DTO
                AsistenciaDTO asistenciaDTO = new AsistenciaDTO
                {
                    ID = asistencia.ID,
                    DniAlumno = alumno.Usuario.DNI,
                    NombreMateria = materia.Nombre,
                    Estado = asistencia.Estado,
                    Fecha = asistencia.Fecha
                };

                resultado.Add(asistenciaDTO);
            }

            return resultado;
        }
    }
}