using ApiAWSMatchUp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetMatchUp.Models;

namespace ApiAWSMatchUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PachangasController : ControllerBase
    {
        private RepositoryMatchUp repo;
        public PachangasController(RepositoryMatchUp repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pachanga>>> GetPachangas()
        {
            return await this.repo.GetPachangasAsync();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<PartidoEquipos>>> PartidosPachanga()
        {
            return await this.repo.ObtenerPartidosPorPachanga();
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idusuario}")]
        public async Task<ActionResult<List<PartidoEquipos>>> PartidosMes(int idusuario)
        {
            return await this.repo.GetPartidosDelMesActual(idusuario);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{idequipo}")]
        public async Task<ActionResult> Create(Pachanga pachanga, int idequipo)
        {
            await this.repo.InsertPachangaAsync(pachanga, idequipo);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("[action]/{local}/{visitante}/{idpartido}")]
        public async Task<ActionResult> UpdateResult(int local, int visitante, int idpartido)
        {
            await this.repo.ActualizarResultadoAsync(local, visitante, idpartido);

            return Ok();
        }
    }
}
