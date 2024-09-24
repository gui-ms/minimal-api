using minimal_api.Domain.DTOs;
using MinimalApi.DTOs;
using MinimalApi.Entities;

namespace minimal_api.Interfaces;

public interface IAdminServico{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    Administrador? BuscaPorId(int id);
    List<Administrador> Todos(int? pagina);
}