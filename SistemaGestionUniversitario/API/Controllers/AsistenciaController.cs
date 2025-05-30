﻿using Entidades.DTOs.Crear;
using Entidades.DTOs.Modificar;
using Entidades.DTOs.Respuestas;
using Logica.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaLogic _asistenciaLogic;

        public AsistenciaController(IAsistenciaLogic asistenciaLogic)
        {
            _asistenciaLogic = asistenciaLogic;
        }

        [HttpGet("{nombreMateria}")]
        public async Task<IActionResult> ObtenerAsistenciasPorMateria(string nombreMateria)
        {
            List<AsistenciaDTO> asistenciaDTO = await _asistenciaLogic.ObtenerAsistenciasPorMateria(nombreMateria);

            return Ok(asistenciaDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearAsistenciaDTO crearAsistenciaDTO)
        {
            await _asistenciaLogic.AltaAsistencia(crearAsistenciaDTO.idInscripcion, crearAsistenciaDTO.idDiaHorarioMateria, crearAsistenciaDTO.Estado, crearAsistenciaDTO.Fecha);

            return Ok();
        }

        [HttpPut("{dniAlumno}/{nombreMateria}")]
        public async Task<IActionResult> Modificar(string dniAlumno, string nombreMateria, [FromBody] ModificarAsistenciaDTO modificarAsistenciaDTO)
        {
            AsistenciaDTO asistenciaDTO = await _asistenciaLogic.ActualizarAsistencia(dniAlumno, nombreMateria, modificarAsistenciaDTO.Fecha, modificarAsistenciaDTO.Estado);

            if (asistenciaDTO == null)
            {
                return NotFound();
            }

            return Ok(asistenciaDTO);
        }

        [HttpDelete("{dniAlumno}/{nombreMateria}/{fecha}")]
        public async Task<IActionResult> Eliminar(string dniAlumno, string nombreMateria, DateTime fecha)
        {
            try
            {
                await _asistenciaLogic.EliminarAsistencia(dniAlumno, nombreMateria, fecha);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}