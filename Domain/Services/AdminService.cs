using Microsoft.EntityFrameworkCore;
using minimal_api.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Entities;
using MinimalApi.Infra.DB;

namespace minimal_api.Services;

public class AdministradorServico : IAdminServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto){
        _contexto = contexto;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}