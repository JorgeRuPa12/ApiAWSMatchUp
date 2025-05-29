using ApiAWSMatchUp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetMatchUp.Models;

namespace ApiAWSMatchUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private RepositoryMatchUp repo;
        public EquiposController(RepositoryMatchUp repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            return await this.repo.GetEquiposAsync();
        }

        [Authorize]
        [HttpGet]
        [Route("{idusuario}")]
        public async Task<ActionResult<List<Equipo>>> GetEquipos(int idusuario)
        {
            return await this.repo.GetEquiposUsuarioAysnc(idusuario);
        }

        [HttpGet]
        [Route("[action]/{idequipo}")]
        public async Task<ActionResult<EquipoDetalle>> Details(int idequipo)
        {
            return await this.repo.GetEquipoDetalleAsync(idequipo);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Create(Equipo equipo)
        {
            await this.repo.InsertEquipoAsync(equipo);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{idequipo}/{idusuario}")]
        public async Task<ActionResult> Join(int idequipo, int idusuario)
        {
            await this.repo.UnirseEquipoAsync(idequipo, idusuario);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{idequipo}/{idusuario}")]
        public async Task<ActionResult> Leave(int idequipo, int idusuario)
        {
            await this.repo.SalirseEquipoAsync(idequipo, idusuario);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{idequipo}/{idpartido}")]
        public async Task<ActionResult> JoinToMatch(int idequipo, int idpartido)
        {
            await this.repo.UnirEquipoPartido(idequipo, idpartido);

            return Ok();
        }
    }
}
