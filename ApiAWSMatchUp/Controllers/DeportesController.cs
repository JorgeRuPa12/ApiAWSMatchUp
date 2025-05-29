using ApiAWSMatchUp.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetMatchUp.Models;

namespace ApiAWSMatchUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeportesController : ControllerBase
    {
        private RepositoryMatchUp repo;
        public DeportesController(RepositoryMatchUp repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Deporte>>> GetDeportes()
        {
            return await this.repo.GetDeportes();
        }
    }
}
