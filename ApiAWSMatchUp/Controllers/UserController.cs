using ApiAWSMatchUp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NugetMatchUp.Models;

namespace ApiAWSMatchUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private RepositoryMatchUp repo;
        public UserController(RepositoryMatchUp repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [Route("[action]/{email}/{password}")]
        public async Task<ActionResult<User>> Login(string email, string password)
        {
            return await this.repo.LogInEmpleadosAsync(email, password);
        }

        [HttpPost]
        [Route("[action]/{email}/{name}/{password}")]
        public async Task<ActionResult> Register(string email, string name, string password)
        {
            await this.repo.InsertUser(email, name, password);

            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("[action]/{image}/{idusuario}")]
        public async Task<ActionResult> UpdateImage(string image, int idusuario)
        {
            await this.repo.UpdateImagenUser(image, idusuario);

            return Ok();
        }
    }
}
