using MinimalApi.DTOs;
using MinimalApi.Entities;

namespace minimal_api.Interfaces;

public interface IAdminServico{
    Administrador? Login(LoginDTO loginDTO);
}