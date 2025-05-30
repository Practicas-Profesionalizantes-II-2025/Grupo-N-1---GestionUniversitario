﻿using Entidades.DTOs.Respuestas;
using Logica.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorarioController : ControllerBase
    {
        private readonly IHorarioLogic _horarioLogic;
        
        public HorarioController(IHorarioLogic horarioLogic)
        {
            _horarioLogic = horarioLogic;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHorarios()
        {
            List<HorarioDTO> horarioDTO = await _horarioLogic.ObtenerHorarios();

            return Ok(horarioDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerHorarioId(int id)
        {
            HorarioDTO horarioDTO = await _horarioLogic.ObtenerHorarioId(id);

            if (horarioDTO == null)
            {
                return NotFound();
            }

            return Ok(horarioDTO);
        }
    }
}