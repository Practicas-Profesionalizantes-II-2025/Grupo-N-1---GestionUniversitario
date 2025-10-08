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
        IDiaLogic _diaLogic;

        public AsistenciaLogic(IAsistenciaRepository asistenciaRepository, IAlumnoRepository alumnoRepository, IMateriaRepository materiaRepository, IInscripcionRepository inscripcionRepository, IDiaHorarioRepository diaHorarioRepository, IDiaRepository diaRepository, IHorarioRepository horarioRepository, IDiaHorarioMateriaRepository diaHorarioMateriaRepository, IDiaLogic diaLogic)
        {
            _asistenciaRepository = asistenciaRepository;
            _alumnoRepository = alumnoRepository;
            _materiaRepository = materiaRepository;
            _inscripcionRepository = inscripcionRepository;
            _diaHorarioRepository = diaHorarioRepository;
            _diaRepository = diaRepository;
            _horarioRepository = horarioRepository;
            _diaHorarioMateriaRepository = diaHorarioMateriaRepository;
            _diaLogic = diaLogic;
        }

        public async Task AltaAsistencia(int idinscripcion, string nombreMateria, bool estado, DateTime fecha)
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

            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
            if (materiaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la materia.");
            }

            List<string> listaDiasMateria = new List<string>();
            foreach (DiaHorario diaHorario in materiaExistente.DiaHorario)
            {
                string diaDescripcion = await _diaLogic.ObtenerDescripcionDiaPorIDUsoInterno(diaHorario.IdDia);
                if(diaDescripcion == "Lunes")
                {
                    diaDescripcion = "Monday";
                }
                else if(diaDescripcion == "Martes")
                {
                    diaDescripcion = "Tuesday";
                }
                else if(diaDescripcion == "Miércoles")
                {
                    diaDescripcion = "Wednesday";
                }
                else if(diaDescripcion == "Jueves")
                {
                    diaDescripcion = "Thursday";
                }
                else if(diaDescripcion == "Viernes")
                {
                    diaDescripcion = "Friday";
                }
                else
                {
                    diaDescripcion = "";
                }

                listaDiasMateria.Add(diaDescripcion);
            }

            DayOfWeek diaSemanaFechaIngresada = fecha.DayOfWeek;

            if (!listaDiasMateria.Contains(diaSemanaFechaIngresada.ToString()))
            {
                throw new InvalidOperationException("La fecha ingresada no corresponde a un día de clase de la materia.");
            }

            Asistencia? asistenciaExistente = (await _asistenciaRepository.FindByConditionAsync(p => p.Fecha == fecha && p.IdInscripcion == idinscripcion)).FirstOrDefault();
            if (asistenciaExistente != null)
            {
                asistenciaExistente.Estado = estado;

                _asistenciaRepository.Update(asistenciaExistente);
                await _asistenciaRepository.SaveAsync();
            }
            else
            {
                Asistencia asistenciaNueva = new Asistencia()
                {
                    IdInscripcion = idinscripcion,
                    Estado = estado,
                    Fecha = fecha
                };

                await _asistenciaRepository.AddAsync(asistenciaNueva);
                await _asistenciaRepository.SaveAsync();
            }
        }

        public async Task EliminarAsistencia(string nombreMateria, DateTime fecha)
        {
            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
            if (materiaExistente == null)
            {
                throw new InvalidOperationException("No se encontró la materia.");
            }

            List<Inscripcion> inscripcionesExistentes = (await _inscripcionRepository.FindByConditionAsync(i => i.IdMateria == materiaExistente.ID)).ToList();
            if (inscripcionesExistentes.Count == 0)
            {
                throw new InvalidOperationException("No hay inscripciones en esa materia.");
            }

            List<int> idsInscripciones = inscripcionesExistentes.Select(i => i.ID).ToList();
            List<Asistencia> asistencias = (await _asistenciaRepository.FindByConditionAsync(a => a.Fecha == fecha && idsInscripciones.Contains(a.IdInscripcion))).ToList();

            if (asistencias.Count == 0)
            {
                throw new InvalidOperationException("No se encontraron asistencias para esa fecha.");
            }

            foreach (Asistencia asistencia in asistencias)
            {
                _asistenciaRepository.Remove(asistencia);
            }

            await _asistenciaRepository.SaveAsync();
        }

        // ------ Se deja codigo por si se cambia la forma de tomar la asistencia, pero por el momento no se usa ------
        #region
        //public async Task<AsistenciaDTO> ActualizarAsistencia(string dniAlumno, string nombreMateria, DateTime fecha, bool estado)
        //{
        //    Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario.DNI == dniAlumno)).SingleOrDefault();
        //    if (alumnoExistente == null)
        //    {
        //        throw new InvalidOperationException("No se encontró un alumno con ese DNI.");
        //    }

        //    Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).SingleOrDefault();
        //    if (materiaExistente == null)
        //    {
        //        throw new InvalidOperationException("No se encontró la materia.");
        //    }

        //    Inscripcion? inscripcionExistente = (await _inscripcionRepository.FindByConditionAsync(i => i.IdAlumno == alumnoExistente.ID && i.IdMateria == materiaExistente.ID)).SingleOrDefault();
        //    if (inscripcionExistente == null)
        //    {
        //        throw new InvalidOperationException("El alumno no está inscripto en esa materia.");
        //    }

        //    Asistencia? asistenciaExistente = (await _asistenciaRepository.FindByConditionAsync(a => a.Fecha == fecha && a.IdInscripcion == inscripcionExistente.ID)).SingleOrDefault();
        //    if (asistenciaExistente == null)
        //    {
        //        throw new InvalidOperationException("No se encontró la asistencia para ese día.");
        //    }

        //    asistenciaExistente.Estado = estado;

        //    _asistenciaRepository.Update(asistenciaExistente);
        //    await _asistenciaRepository.SaveAsync();

        //    var asistenciaDTO = new AsistenciaDTO
        //    {
        //        ID = asistenciaExistente.ID,
        //        DniAlumno = alumnoExistente.Usuario.DNI,
        //        NombreMateria = materiaExistente.Nombre,
        //        Estado = asistenciaExistente.Estado,
        //        Fecha = asistenciaExistente.Fecha
        //    };

        //    return asistenciaDTO;
        //}
        #endregion

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

                if (asistencia.Fecha.Year != DateTime.Now.Year)
                    continue;

                // Crear DTO
                AsistenciaDTO asistenciaDTO = new AsistenciaDTO
                {
                    ID = asistencia.ID,
                    DniAlumno = alumno.Usuario.DNI,
                    NombreMateria = materia.Nombre,
                    Estado = asistencia.Estado,
                    Fecha = asistencia.Fecha,
                    NombreAlumno = alumno.Usuario.Nombre,
                    ApellidoAlumno = alumno.Usuario.Apellido,
                    IdInscripcion = inscripcion.ID
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