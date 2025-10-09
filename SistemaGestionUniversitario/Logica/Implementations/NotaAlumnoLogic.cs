using Datos.Repositories.Contracts;
using Entidades.DTOs.Respuestas;
using Entidades.Entities;
using Logica.Contracts;

namespace Logica.Implementations
{
    public class NotaAlumnoLogic : INotaAlumnoLogic
    {
        private INotaAlumnoRepository _notaAlumnoRepository;
        private IAlumnoRepository _alumnoRepository;
        private IExamenRepository _examenRepository;

        public NotaAlumnoLogic(INotaAlumnoRepository notaAlumnoRepository, IAlumnoRepository alumnoRepository, IExamenRepository examenRepository)
        {
            _notaAlumnoRepository = notaAlumnoRepository;
            _alumnoRepository = alumnoRepository;
            _examenRepository = examenRepository;
        }

        public async Task AltaNotaAlumno(int nota, string dniAlumno, int idExamen)
        {  
            if (nota < 0 || nota > 10)
            {
                throw new ArgumentNullException("La nota ingresada debe tener un valor entre 0 y 10.");
            }

            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.DNI == dniAlumno)).FirstOrDefault();

            if (alumnoExistente == null)
            {
                throw new ArgumentNullException("El alumno seleccionado no existe.");
            }

            Examen? examenExistente = (await _examenRepository.FindByConditionAsync(p => p.ID == idExamen)).FirstOrDefault();

            if (examenExistente == null)
            {
                throw new ArgumentNullException("El examen seleccionado no existe.");
            }

            NotaAlumno? notaAlumnoExistente = (await _notaAlumnoRepository.FindByConditionAsync(p => p.IdAlumno == alumnoExistente.ID && p.IdExamen == idExamen)).FirstOrDefault();

            if (notaAlumnoExistente != null)
            {
                throw new ArgumentNullException("Ya se le asigno una nota al alumno ingresado en el examen seleccionado.");
            }

            NotaAlumno notaAlumnoAgregar = new NotaAlumno()
            {
                Nota = nota,
                IdAlumno = alumnoExistente.ID,
                IdExamen = idExamen,
            };

            await _notaAlumnoRepository.AddAsync(notaAlumnoAgregar);
            await _notaAlumnoRepository.SaveAsync();
        }
        public async Task<NotaAlumnoDTO> ActualizacionNotaAlumno(int nota, string dniAlumno, int idExamen)
        {
            if (nota < 0 || nota > 10)
            {
                throw new ArgumentNullException("La nota ingresada debe tener un valor entre 0 y 10.");
            }

            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(p => p.Usuario.DNI == dniAlumno)).FirstOrDefault();

            if (alumnoExistente == null)
            {
                throw new ArgumentNullException("El alumno seleccionado no existe.");
            }

            Examen? examenExistente = (await _examenRepository.FindByConditionAsync(p => p.ID == idExamen)).FirstOrDefault();

            if (examenExistente == null)
            {
                throw new ArgumentNullException("El examen seleccionado no existe.");
            }

            NotaAlumno? notaAlumnoExistente = (await _notaAlumnoRepository.FindByConditionAsync(p => p.IdAlumno == alumnoExistente.ID && p.IdExamen == idExamen)).FirstOrDefault();

            if (notaAlumnoExistente == null)
            {
                throw new ArgumentNullException("El alumno ingresado en el examen seleccionado no tiene ninguna nota para actualizar.");
            }

            notaAlumnoExistente.Nota = nota;
            notaAlumnoExistente.IdAlumno = alumnoExistente.ID;
            notaAlumnoExistente.IdExamen = idExamen;

            _notaAlumnoRepository.Update(notaAlumnoExistente);
            await _notaAlumnoRepository.SaveAsync();

            NotaAlumnoDTO notaAlumnoExistenteDTO = new NotaAlumnoDTO()
            {
                ID = notaAlumnoExistente.ID,
                AlumnoNombre = $"{alumnoExistente.Usuario.Nombre} {alumnoExistente.Usuario.Apellido}",
                ExamenMateriaNombre = examenExistente.Materia.Nombre,
                ExamenTipo = examenExistente.Tipo,
                Nota = notaAlumnoExistente.Nota
            };

            return notaAlumnoExistenteDTO;
        }
        public async Task BajaNotaAlumno(string dniAlumno, int idExamen)
        {
            Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario.DNI == dniAlumno)).FirstOrDefault();
            if (alumnoExistente == null)
            {
                throw new ArgumentNullException($"El alumno con DNI {dniAlumno} no existe.");
            }

            NotaAlumno? notaAlumnoEliminar = (await _notaAlumnoRepository.FindByConditionAsync(p => p.IdAlumno == alumnoExistente.ID && p.IdExamen == idExamen)).FirstOrDefault();

            if (notaAlumnoEliminar == null)
            {
                throw new ArgumentException("La nota que se desea eliminar no existe.");
            }

            _notaAlumnoRepository.Remove(notaAlumnoEliminar);
            await _notaAlumnoRepository.SaveAsync();
        }
        public async Task<List<NotaAlumnoDTO>> ObtenerNotas()
        {
            try
            {
                List<NotaAlumno> listaNotas = (await _notaAlumnoRepository.FindAllAsync()).ToList();

                if (listaNotas == null)
                {
                    return null;
                }

                List<NotaAlumnoDTO> listaNotasDTO = new List<NotaAlumnoDTO>();
                foreach (NotaAlumno nota in listaNotas)
                {
                    Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.ID == nota.IdAlumno)).FirstOrDefault();
                    Examen? examen = (await _examenRepository.FindByConditionAsync(a => a.ID == nota.IdExamen)).FirstOrDefault();


                    NotaAlumnoDTO notaDTO = new NotaAlumnoDTO()
                    {
                        ID = nota.ID,
                        AlumnoNombre = $"{alumno.Usuario.Nombre} {alumno.Usuario.Apellido}",
                        ExamenMateriaNombre = examen.Materia.Nombre,
                        ExamenTipo = examen.Tipo,
                        Nota = nota.Nota
                    };

                    listaNotasDTO.Add(notaDTO);
                }

                return listaNotasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<NotaAlumnoDTO>> ObtenerNotasPorMateria(string nombreMateria)
        {
            try
            {
                List<Examen> listaExamenes = (await _examenRepository.FindByConditionAsync(e => e.Materia.Nombre == nombreMateria)).ToList();

                //HashSet<int> mejora mucho el rendimiento de busqueda en comparacion con List<int>
                HashSet<int> idsExamenes = listaExamenes.Select(e => e.ID).ToHashSet();

                List<NotaAlumno> listaNotas = (await _notaAlumnoRepository.FindAllAsync()).Where(n => idsExamenes.Contains(n.IdExamen)).ToList();

                if (listaNotas == null || listaNotas.Count == 0)
                {
                    return new List<NotaAlumnoDTO>();
                }

                List<NotaAlumnoDTO> listaNotasDTO = new List<NotaAlumnoDTO>();
                foreach (NotaAlumno nota in listaNotas)
                {
                    Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.ID == nota.IdAlumno)).FirstOrDefault();
                    Examen? examen = (await _examenRepository.FindByConditionAsync(a => a.ID == nota.IdExamen)).FirstOrDefault();

                    NotaAlumnoDTO notaDTO = new NotaAlumnoDTO()
                    {
                        ID = nota.ID,
                        AlumnoNombre = $"{alumno.Usuario.Nombre} {alumno.Usuario.Apellido}",
                        ExamenMateriaNombre = examen.Materia.Nombre,
                        ExamenTipo = examen.Tipo,
                        Nota = nota.Nota
                    };

                    listaNotasDTO.Add(notaDTO);
                }

                return listaNotasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<NotaAlumnoDTO>> ObtenerNotasPorAlumno(string dniAlumno)
        {
            try
            {
                Alumno? alumnoExistente = (await _alumnoRepository.FindByConditionAsync(a => a.Usuario.DNI == dniAlumno)).FirstOrDefault();
                if (alumnoExistente == null)
                {
                    throw new ArgumentNullException($"El alumno con DNI {dniAlumno} no existe.");
                }

                List<NotaAlumno> listaNotas = (await _notaAlumnoRepository.FindByConditionAsync(n => n.IdAlumno == alumnoExistente.ID)).ToList();

                if (listaNotas == null)
                {
                    return null;
                }

                List<NotaAlumnoDTO> listaNotasDTO = new List<NotaAlumnoDTO>();
                foreach (NotaAlumno nota in listaNotas)
                {
                    Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.ID == nota.IdAlumno)).FirstOrDefault();
                    Examen? examen = (await _examenRepository.FindByConditionAsync(a => a.ID == nota.IdExamen)).FirstOrDefault();


                    NotaAlumnoDTO notaDTO = new NotaAlumnoDTO()
                    {
                        ID = nota.ID,
                        AlumnoNombre = $"{alumno.Usuario.Nombre} {alumno.Usuario.Apellido}",
                        ExamenMateriaNombre = examen.Materia.Nombre,
                        ExamenTipo = examen.Tipo,
                        Nota = nota.Nota
                    };

                    listaNotasDTO.Add(notaDTO);
                }

                return listaNotasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            };
        }
        public async Task<List<NotaAlumnoDTO>> ObtenerNotasPorExamen(int idexamen)
        {
            
            try
            {
                Examen? examenExistente = (await _examenRepository.FindByConditionAsync(a => a.ID== idexamen)).FirstOrDefault();
                if (examenExistente == null)
                {
                    throw new ArgumentNullException($"El examen con ID {idexamen} no existe.");
                }

                List<NotaAlumno> listaNotas = (await _notaAlumnoRepository.FindByConditionAsync(n => n.IdExamen == examenExistente.ID)).ToList();

                if (listaNotas == null)
                {
                    return null;
                }

                List<NotaAlumnoDTO> listaNotasDTO = new List<NotaAlumnoDTO>();
                foreach (NotaAlumno nota in listaNotas)
                {
                    Alumno? alumno = (await _alumnoRepository.FindByConditionAsync(a => a.ID == nota.IdAlumno)).FirstOrDefault();
                    Examen? examen = (await _examenRepository.FindByConditionAsync(a => a.ID == nota.IdExamen)).FirstOrDefault();


                    NotaAlumnoDTO notaDTO = new NotaAlumnoDTO()
                    {
                        ID = nota.ID,
                        AlumnoNombre = $"{alumno.Usuario.Nombre} {alumno.Usuario.Apellido}",
                        ExamenMateriaNombre = examen.Materia.Nombre,
                        ExamenTipo = examen.Tipo,
                        Nota = nota.Nota
                    };

                    listaNotasDTO.Add(notaDTO);
                }

                return listaNotasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
            ;
        }
    }
}