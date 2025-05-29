using ApiAWSMatchUp.Helpers;
using ApiAWSMatchUp.Models;
using ApiAWSMatchUp.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NugetMatchUp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiAWSMatchUp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryMatchUp repo;
        private HelperActionServicesOAuth helper;
        public AuthController(RepositoryMatchUp repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            User user = await this.repo.LogInEmpleadosAsync(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                SigningCredentials credentials =
                    new SigningCredentials
                    (this.helper.GetKeyToken(),
                    SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow
                        );

                return Ok(new
                {
                    response = new JwtSecurityTokenHandler()
                    .WriteToken(token)
                });
            }
        }
    }
}
