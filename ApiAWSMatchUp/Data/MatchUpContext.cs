using NugetMatchUp.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAWSMatchUp.Data
{
    public class MatchUpContext: DbContext
    {
        public MatchUpContext(DbContextOptions<MatchUpContext> options): base(options) { }

        public DbSet<Deporte> Deportes { get; set; }
        public DbSet<Pachanga> Pachangas { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<UsuarioEquipo> UsuariosEquipo { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<PachangaPartido> PachangaPartido { get; set; }
    }
}
