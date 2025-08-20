using Datos.Repositories.Contracts;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;

namespace Logica.Implementations
{
    public class InscripcionLogic : IInscripcionLogic
    {
        private IInscripcionRepository _inscripcionRepository;
        private IMateriaRepository _materiaRepository;
        private IAlumnoRepository _alumnoRepository;

        public InscripcionLogic(IInscripcionRepository inscripcionRepository, IMateriaRepository materiaRepository, IAlumnoRepository alumnoRepository)
        {
            _inscripcionRepository = inscripcionRepository;
            _materiaRepository = materiaRepository;
            _alumnoRepository = alumnoRepository;
        }

        public async Task AltaInscripcion(string dniAlumno, string nombreMateria)
        {
            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.DNI == dniAlumno)).FirstOrDefault();

            if (alumnoExistente == null)
            {
                throw new ArgumentNullException("La inscripcion debe estar vinculada a un alumno con DNI valido.");
            }

            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).FirstOrDefault();
            if (materiaExistente == null)
            {
                throw new ArgumentNullException("La inscripcion debe estar vinculada a una materia con nombre valido.");
            }

            Inscripcion? inscripcionExistente = (await _inscripcionRepository.FindByConditionAsync(p => p.IdMateria == materiaExistente.ID && p.IdAlumno == alumnoExistente.ID)).FirstOrDefault();

            if (inscripcionExistente != null)
            {
                throw new ArgumentNullException("El alumno ya esta inscripto a esta materia.");
            }

            Inscripcion inscripcionNueva = new Inscripcion()
            {
                IdAlumno = alumnoExistente.ID,
                IdMateria = materiaExistente.ID,
                Estado = false
            };

            await _inscripcionRepository.AddAsync(inscripcionNueva);
            await _inscripcionRepository.SaveAsync();
        }
        public async Task BajaInscripcion(string dniAlumno, string nombreMateria)
        {
            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.DNI == dniAlumno)).FirstOrDefault();

            if (alumnoExistente == null)
            {
                throw new ArgumentNullException("La inscripcion debe estar vinculada a un alumno con DNI valido.");
            }

            Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(m => m.Nombre == nombreMateria)).FirstOrDefault();
            if (materiaExistente == null)
            {
                throw new ArgumentNullException("La inscripcion debe estar vinculada a una materia con nombre valido.");
            }

            Inscripcion? inscripcionExistente = (await _inscripcionRepository.FindByConditionAsync(p => p.IdMateria == materiaExistente.ID && p.IdAlumno == alumnoExistente.ID)).FirstOrDefault();

            if (inscripcionExistente == null)
            {
                throw new InvalidOperationException("La inscripcion que se desea dar de baja no existe.");
            }

            _inscripcionRepository.Remove(inscripcionExistente);
            await _inscripcionRepository.SaveAsync();
        }
        public async Task<List<InscripcionDTO>> ObtenerInscripciones()
        {
            try
            {
                List<Inscripcion> listaInscripciones = (await _inscripcionRepository.FindAllAsync()).ToList();

                if (listaInscripciones == null)
                {
                    return null;
                }

                List<InscripcionDTO> listaInscripcionesDTO = new List<InscripcionDTO>();
                foreach (Inscripcion inscripcion in listaInscripciones)
                {
                    Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.ID == inscripcion.IdAlumno)).FirstOrDefault();

                    if (alumnoExistente == null)
                    {
                        throw new ArgumentNullException("ID de alumno invalido.");
                    }

                    Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(p => p.ID == inscripcion.IdMateria)).FirstOrDefault();
                    
                    if (materiaExistente == null)
                    {
                        throw new ArgumentNullException("ID de materia invalido.");
                    }

                    InscripcionDTO inscripcionDTO = new InscripcionDTO()
                    {
                        ID = inscripcion.ID,
                        IdAlumno = alumnoExistente.ID,
                        NombreAlumno = alumnoExistente.Usuario.Nombre,
                        ApellidoAlumno = alumnoExistente.Usuario.Apellido,
                        DNIAlumno = alumnoExistente.Usuario.DNI,
                        Estado = inscripcion.Estado ? "Aprobado" : "En Curso",
                        IdMateria = materiaExistente.ID,
                        NombreMateria = materiaExistente.Nombre
                    };

                    listaInscripcionesDTO.Add(inscripcionDTO);
                }

                return listaInscripcionesDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<InscripcionDTO>> ObtenerInscripcionesMateria(string nombreMateria)
        {
            try
            {
                Materia? materiaFiltro = (await _materiaRepository.FindByConditionAsync(p => p.Nombre == nombreMateria)).FirstOrDefault();

                if (materiaFiltro == null)
                {
                    throw new ArgumentNullException("El nombre ingresado no pertenece a ninguna materia existente.");
                }

                List<Inscripcion> listaInscripciones = (await _inscripcionRepository.FindByConditionAsync(p => p.IdMateria == materiaFiltro.ID)).ToList();

                if (listaInscripciones == null)
                {
                    return new List<InscripcionDTO>();
                }

                List<InscripcionDTO> listaInscripcionesDTO = new List<InscripcionDTO>();
                foreach (Inscripcion inscripcion in listaInscripciones)
                {
                    Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.ID == inscripcion.IdAlumno)).FirstOrDefault();

                    if (alumnoExistente == null)
                    {
                        throw new ArgumentNullException("ID de alumno invalido.");
                    }

                    InscripcionDTO inscripcionDTO = new InscripcionDTO()
                    {
                        ID = inscripcion.ID,
                        IdAlumno = alumnoExistente.ID,
                        NombreAlumno = alumnoExistente.Usuario.Nombre,
                        ApellidoAlumno = alumnoExistente.Usuario.Apellido,
                        DNIAlumno = alumnoExistente.Usuario.DNI,
                        Estado = inscripcion.Estado ? "Aprobado" : "En Curso",
                        IdMateria = materiaFiltro.ID,
                        NombreMateria = materiaFiltro.Nombre
                    };

                    listaInscripcionesDTO.Add(inscripcionDTO);
                }

                return listaInscripcionesDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<InscripcionDTO>> ObtenerInscripcionesDNI(string dni)
        {
            try
            {
                Alumno? alumnoFiltro = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.DNI == dni)).FirstOrDefault();

                if (alumnoFiltro == null)
                {
                    throw new ArgumentNullException("El DNI ingresado no pertenece a ningun alumno existente.");
                }

                List<Inscripcion> listaInscripciones = (await _inscripcionRepository.FindByConditionAsync(p => p.IdAlumno == alumnoFiltro.ID)).ToList();

                if (listaInscripciones == null)
                {
                    return new List<InscripcionDTO>();
                }

                List<InscripcionDTO> listaInscripcionesDTO = new List<InscripcionDTO>();
                foreach (Inscripcion inscripcion in listaInscripciones)
                {
                    Materia? materiaExistente = (await _materiaRepository.FindByConditionAsync(p => p.ID == inscripcion.IdMateria)).FirstOrDefault();

                    if (materiaExistente == null)
                    {
                        throw new ArgumentNullException("ID de materia invalido.");
                    }

                    InscripcionDTO inscripcionDTO = new InscripcionDTO()
                    {
                        ID = inscripcion.ID,
                        IdAlumno = alumnoFiltro.ID,
                        NombreAlumno = alumnoFiltro.Usuario.Nombre,
                        ApellidoAlumno = alumnoFiltro.Usuario.Apellido,
                        DNIAlumno = alumnoFiltro.Usuario.DNI,
                        Estado = inscripcion.Estado ? "Aprobado" : "En Curso",
                        IdMateria = materiaExistente.ID,
                        NombreMateria = materiaExistente.Nombre
                    };

                    listaInscripcionesDTO.Add(inscripcionDTO);
                }

                return listaInscripcionesDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
    }
}