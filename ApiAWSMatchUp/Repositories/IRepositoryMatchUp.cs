using NugetMatchUp.Models;

namespace ApiAWSMatchUp.Repositories
{
    public interface IRepositoryMatchUp
    {
        // Deportes
        Task<List<Deporte>> GetDeportes();

        // Equipos
        Task<List<Equipo>> GetEquiposAsync();
        Task InsertEquipoAsync(Equipo equipo);
        Task<List<Equipo>> GetEquiposUsuarioAysnc(int idusuario);
        Task<EquipoDetalle> GetEquipoDetalleAsync(int idequipo);
        Task UnirseEquipoAsync(int idequipo, int idusuario);
        Task SalirseEquipoAsync(int idequipo, int idusuario);
        Task UnirEquipoPartido(int idequipo, int idpartido);

        // Pachangas
        Task<List<Pachanga>> GetPachangasAsync();
        Task InsertPachangaAsync(Pachanga pachanga, int idequipo);
        Task<List<PartidoEquipos>> ObtenerPartidosPorPachanga();
        Task<List<PartidoEquipos>> GetPartidosDelMesActual(int idUsuario);
        Task ActualizarResultadoAsync(int local, int visitante, int idpartido);

        // Usuarios
        Task InsertUser(string email, string name, string password);
        Task<User> LogInEmpleadosAsync(string correo, string password);
        Task UpdateImagenUser(string imagen, int idusuario);
    }
}
