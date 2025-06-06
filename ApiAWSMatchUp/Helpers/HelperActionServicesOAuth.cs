﻿using ApiAWSMatchUp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace ApiAWSMatchUp.Helpers
{
    public class HelperActionServicesOAuth
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public HelperActionServicesOAuth(IConfiguration configuration, string secretKey)
        {
            string secretJson = HelperSecretManager.GetSecretAsync().GetAwaiter().GetResult();
            Secrets keys = JsonConvert.DeserializeObject<Secrets>(secretJson);
            this.Issuer = keys.Issuer;
            this.Audience = keys.Audience;
            this.SecretKey = secretKey;
        }

        //NECESITAMOS UN METODO PARA GENERAR EL TOKEN
        //DICHO TOKEN SE BASA EN NUESTRO SECRET KEY
        public SymmetricSecurityKey GetKeyToken()
        {
            //CONVERTIMOS EL SECRET KEY A BYTES
            byte[] data =
                Encoding.UTF8.GetBytes(this.SecretKey);
            //DEVOLVEMOS LA KEY GENERADA A PARTIR DE LOS BYTES
            return new SymmetricSecurityKey(data);
        }

        //ESTA CLASE LA HEMOS CREADO TAMBIEN PARA QUITAR CODIGO 
        //DEL PROGRAM
        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options =
                new Action<JwtBearerOptions>(options =>
                {
                    //INDICAMOS QUE DEBEMOS VALIDAR PARA EL TOKEN
                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Issuer,
                        ValidAudience = this.Audience,
                        IssuerSigningKey = this.GetKeyToken()
                    };
                });
            return options;
        }

        //TODA SEGURIDAD SIEMPRE ESTA BASADA EN UN SCHEMA
        public Action<AuthenticationOptions>
            GetAuthenticateSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }
    }
}
