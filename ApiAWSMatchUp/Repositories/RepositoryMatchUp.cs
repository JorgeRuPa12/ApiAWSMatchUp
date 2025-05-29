using ApiAWSMatchUp.Data;
using ApiAWSMatchUp.Helpers;
using NugetMatchUp.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ApiAWSMatchUp.Repositories
{
    public class RepositoryMatchUp: IRepositoryMatchUp
    {
        private MatchUpContext context;
        private string containerUrl;
        public RepositoryMatchUp(MatchUpContext context)
        {
            this.context = context;
            this.containerUrl = "https://storagematchup.blob.core.windows.net/imagenes";
        }
        #region Deportes

        public async Task<List<Deporte>> GetDeportes()
        {
            var consulta = from datos in this.context.Deportes
                           select datos;

            return await consulta.ToListAsync();
        }
        #endregion

        #region Equipos 
        public async Task<List<Equipo>> GetEquiposAsync()
        {
            List<Equipo> equipos = await this.context.Equipos.ToListAsync();
            foreach (Equipo c in equipos)
            {
                if (!c.Emblema.StartsWith("http"))
                {
                    string imagePath = c.Emblema;
                    c.Emblema = this.containerUrl + "/escudos/" + imagePath;
                }
            }
            return equipos;
        }

        public async Task InsertEquipoAsync(Equipo equipo)
        {
            int maxId = await this.context.Equipos.AnyAsync()
                ? await this.context.Equipos.MaxAsync(x => x.Id)
                : 0;

            Equipo equipoObj = new Equipo
            {
                Id = maxId + 1,
                Nombre = equipo.Nombre,
                Color = equipo.Color,
                Deporte = equipo.Deporte,
                Emblema = equipo.Emblema,
                IdAdmin = equipo.IdAdmin,
            };

            UsuarioEquipo usuarioEquipoObj = new UsuarioEquipo
            {
                IdUsuario = equipoObj.IdAdmin,
                IdEquipo = equipoObj.Id
            };

            await this.context.AddAsync(equipoObj);
            await this.context.AddAsync(usuarioEquipoObj);

            await this.context.SaveChangesAsync();
        }

        public async Task<List<Equipo>> GetEquiposUsuarioAysnc(int idusuario)
        {
            List<Equipo> equipos =  await (from ue in this.context.UsuariosEquipo
                          join e in this.context.Equipos on ue.IdEquipo equals e.Id
                          where ue.IdUsuario == idusuario
                          select e).ToListAsync();

            foreach (Equipo c in equipos)
            {
                if (!c.Emblema.StartsWith("http"))
                {
                    string imagePath = c.Emblema;
                    c.Emblema = this.containerUrl + "/escudos/" + imagePath;
                }
            }

            return equipos;
        }

        public async Task<EquipoDetalle> GetEquipoDetalleAsync(int idequipo)
        {
            var jugadores = await (from ue in this.context.UsuariosEquipo
                                   join u in this.context.Users on ue.IdUsuario equals u.Id
                                   where ue.IdEquipo == idequipo
                                   select u).ToListAsync();
            foreach (User c in jugadores)
            {
                if (!c.Imagen.StartsWith("http"))
                {
                    string imagePath = c.Imagen;
                    c.Imagen = this.containerUrl + "/users/" + imagePath;
                }
            }
            int numJugadores = jugadores.Count();
            Equipo team = await this.context.Equipos.Where(z => z.Id == idequipo).FirstOrDefaultAsync();
            team.Emblema =  this.containerUrl + "/escudos/" + team.Emblema;

            EquipoDetalle equipoDetalle = new EquipoDetalle
            {
                Jugadores = jugadores,
                Detalles = team,
                NumJugadores = numJugadores
            };
            return equipoDetalle;
        }

        public async Task UnirseEquipoAsync(int idequipo, int idusuario)
        {
            UsuarioEquipo model = new UsuarioEquipo
            {
                IdEquipo = idequipo,
                IdUsuario = idusuario
            };

            await this.context.UsuariosEquipo.AddAsync(model);
            await this.context.SaveChangesAsync();
        }

        public async Task SalirseEquipoAsync(int idequipo, int idusuario)
        {
            UsuarioEquipo model = new UsuarioEquipo
            {
                IdEquipo = idequipo,
                IdUsuario = idusuario
            };

            this.context.UsuariosEquipo.Remove(model);
            await this.context.SaveChangesAsync();
        }

        public async Task UnirEquipoPartido(int idequipo, int idpartido)
        {
            Partido partido = await this.context.Partidos.Where(z => z.Id == idpartido).FirstOrDefaultAsync();

            partido.EquipoVisitante = idequipo;
            await this.context.SaveChangesAsync();
        }
        #endregion

        #region Pachanga
        public async Task<List<Pachanga>> GetPachangasAsync()
        {
            var consulta = from datos in this.context.Pachangas
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task InsertPachangaAsync(Pachanga pachanga, int idequipo)
        {
            int maxIdPachanga = await this.context.Pachangas.AnyAsync()
                ? await this.context.Pachangas.MaxAsync(x => x.Id)
                : 0;

            int maxIPartido = await this.context.Partidos.AnyAsync()
                ? await this.context.Partidos.MaxAsync(x => x.Id)
                : 0;

            int iddeporte = await this.context.Equipos
                .Where(e => e.Id == idequipo)
                .Select(e => e.Deporte)
                .FirstOrDefaultAsync();

            if (iddeporte != 0)
            {
                Deporte deporte = await this.context.Deportes
                    .Where(z => z.Id == iddeporte)
                    .FirstOrDefaultAsync();

                Pachanga pachangaI = new Pachanga
                {
                    Id = maxIdPachanga + 1,
                    Nombre = pachanga.Nombre,
                    Ganador = pachanga.Ganador,
                    Deporte = iddeporte,
                    UbiLatitud = pachanga.UbiLatitud,
                    UbiLongitud = pachanga.UbiLongitud,
                    UbiProvincia = pachanga.UbiProvincia,
                    Inscripcion = pachanga.Inscripcion,
                    Estado = pachanga.Estado,
                    Acceso = pachanga.Acceso,
                    Fecha = pachanga.Fecha,
                };
                await this.context.Pachangas.AddAsync(pachangaI);

                Partido partido = new Partido
                {
                    Id = maxIPartido + 1,
                    Fecha = pachanga.Fecha,
                    EquipoLocal = idequipo,
                    EquipoVisitante = null,
                    Resultado = "Por Determinar",
                    UbiLatitud = pachanga.UbiLatitud,
                    UbiLongitud = pachanga.UbiLongitud,
                    UbiProvincia = pachanga.UbiProvincia,
                    Tiempo = deporte.Tiempo
                };
                await this.context.Partidos.AddAsync(partido);
                await this.context.SaveChangesAsync();  // Guarda primero el partido

                PachangaPartido PP = new PachangaPartido
                {
                    IdPachanga = pachangaI.Id,
                    IdPartido = partido.Id,  // Ahora el Id es válido en la BD
                    Estado = "Pendiente"
                };

                await this.context.PachangaPartido.AddAsync(PP);
                await this.context.SaveChangesAsync();
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<List<PartidoEquipos>> ObtenerPartidosPorPachanga()
        {

            List<Partido> partidos = await this.context.Partidos
               .OrderBy(p => p.Fecha) // Ordenar por fecha ascendente
               .ToListAsync();
            List<PartidoEquipos> partidosLista = new List<PartidoEquipos>();

            foreach (Partido par in partidos)
            {
                Equipo local = await this.context.Equipos.Where(z => z.Id == par.EquipoLocal).FirstOrDefaultAsync(); 
                if (!local.Emblema.StartsWith("http"))
                {
                    string imagePath = local.Emblema;
                    local.Emblema = this.containerUrl + "/escudos/" + imagePath;
                }
                Equipo visitante = await this.context.Equipos.Where(z => z.Id == par.EquipoVisitante).FirstOrDefaultAsync(); 
                if (visitante == null)
                {
                    
                }
                else
                {
                    if (!visitante.Emblema.StartsWith("http"))
                    {
                        string imagePath = visitante.Emblema;
                        visitante.Emblema = this.containerUrl + "/escudos/" + imagePath;
                    }
                }
                PachangaPartido pp = await this.context.PachangaPartido.Where(z => z.IdPartido == par.Id).FirstOrDefaultAsync();
                Pachanga pachanga = await this.context.Pachangas.Where(z => z.Id == pp.IdPachanga).FirstOrDefaultAsync();
                PartidoEquipos pe = new PartidoEquipos
                {
                    Match = par,
                    Pacha = pachanga,
                    Local = local,
                    Visitante = visitante
                };
                partidosLista.Add(pe);
            }

            return partidosLista;
        }

        public async Task<List<PartidoEquipos>> GetPartidosDelMesActual(int idUsuario)
        {
            DateTime primerDiaDelMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1).AddDays(-1);

            // Obtener los equipos del usuario
            var equiposUsuario = await this.context.UsuariosEquipo
                .Where(ue => ue.IdUsuario == idUsuario)
                .Select(ue => ue.IdEquipo)
                .ToListAsync();

            // Obtener las pachangas del mes donde el usuario tenga un equipo
            List<Pachanga> pachangas = await this.context.Pachangas
            .Where(p => p.Fecha >= primerDiaDelMes && p.Fecha <= ultimoDiaDelMes)
            .Join(this.context.PachangaPartido,
                  pach => pach.Id,
                  pp => pp.IdPachanga,
                  (pach, pp) => new { pach, pp })
            .Join(this.context.Partidos,
                  temp => temp.pp.IdPartido,
                  partido => partido.Id,
                  (temp, partido) => new { temp.pach, partido })
            .Where(temp => equiposUsuario.Contains(temp.partido.EquipoLocal)
                        || (temp.partido.EquipoVisitante.HasValue && equiposUsuario.Contains(temp.partido.EquipoVisitante.Value)))
            .OrderBy(temp => temp.pach.Fecha)
            .Select(temp => temp.pach)  // Solo seleccionamos Pachanga
            .ToListAsync();


            List<PartidoEquipos> partidosLista = new List<PartidoEquipos>();

            foreach (Pachanga pac in pachangas)
            {
                PachangaPartido pp = await this.context.PachangaPartido.Where(z => z.IdPachanga == pac.Id).FirstOrDefaultAsync();
                Partido partido = await this.context.Partidos.Where(z => z.Id == pp.IdPartido).FirstOrDefaultAsync();
                Equipo local = await this.context.Equipos.Where(z => z.Id == partido.EquipoLocal).FirstOrDefaultAsync();
                if (!local.Emblema.StartsWith("http"))
                {
                    string imagePath = local.Emblema;
                    local.Emblema = this.containerUrl + "/escudos/" + imagePath;
                }
                Equipo visitante = await this.context.Equipos.Where(z => z.Id == partido.EquipoVisitante).FirstOrDefaultAsync();
                if (visitante == null)
                {
                    
                }
                else
                {
                    if (!visitante.Emblema.StartsWith("http"))
                    {
                        string imagePath = visitante.Emblema;
                        visitante.Emblema = this.containerUrl + "/escudos/" + imagePath;
                    }
                }
                PartidoEquipos pe = new PartidoEquipos
                {
                    Match = partido,
                    Pacha = pac,
                    Local = local,
                    Visitante = visitante
                };
                partidosLista.Add(pe);
            }

            return partidosLista;
        }

        public async Task ActualizarResultadoAsync(int local, int visitante, int idpartido)
        {
            Partido partido = await this.context.Partidos.Where(z => z.Id == idpartido).FirstOrDefaultAsync();

            partido.Resultado = local + " - " + visitante;

            await this.context.SaveChangesAsync();
        }
        #endregion

        #region Users
        public async Task InsertUser(string email ,string name, string password)
        {
            int maxId = await this.context.Users.AnyAsync()
                ? await this.context.Users.MaxAsync(x => x.Id)
                : 0;
            string salt = HelperCryptography.GenerateSalt();
            User userObj = new User
            {
                Id = maxId + 1,
                Nombre = name,
                Email = email,
                Imagen = "defaultuser.jpg",
                Salt = salt,
                Pass = HelperCryptography.EncryptPassword(password, salt),
                Rol = "2",

            };
            await this.context.Users.AddAsync(userObj);
            await this.context.SaveChangesAsync();
        }

        public async Task<User> LogInEmpleadosAsync(string correo, string password)
        {
            User user = await this.context.Users.Where(x => x.Email == correo).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }
            else
            {
                user.Imagen = this.containerUrl + "/users/" + user.Imagen;
                string salt = user.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passBytes = user.Pass;
                bool response = HelperCryptography.CompararArrays(temp, passBytes);
                if (response == true)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task UpdateImagenUser(string imagen, int idusuario)
        {
            User user = await this.context.Users.Where(z => z.Id == idusuario).FirstOrDefaultAsync();
            user.Imagen = imagen;

            await this.context.SaveChangesAsync();
        }
        #endregion
    }
}
