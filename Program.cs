using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Interfaces;
using minimal_api.Services;
using MinimalApi.DTOs;
using MinimalApi.Entities;
using MinimalApi.Infra.DB;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdminServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null){
        return Results.Ok("Login com sucesso");
    }
    else
        return Results.Unauthorized();
}).WithTags("Admin");


app.MapGet("/administradores/", ([FromQuery] int? pagina, IAdminServico administradorServico) => {
    var adms = new List<AdminModelView>();
    var administradores = administradorServico.Todos(pagina);

    foreach(var adm in administradores){
        adms.Add(new AdminModelView{
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(administradores);
}).WithTags("Admin");


app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdminServico administradorServico) => {
    var administrador = administradorServico.BuscaPorId(id);

    if(administrador == null) return Results.NotFound();

    return Results.Ok(new AdminModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).WithTags("Admin");


app.MapPost("/administradores", ([FromBody] AdminDTO adminDTO, IAdminServico administradorServico) => {
    var validacao = new ValidationError{
        Mensagens = new List<string>()
    };
    
    if(string.IsNullOrEmpty(adminDTO.Email)){
        validacao.Mensagens.Add("Email não pode ser vazio!");
    }
    if(string.IsNullOrEmpty(adminDTO.Senha)){
        validacao.Mensagens.Add("Senha não pode ser vazia!");
    }
    if(adminDTO.Perfil == null){
        validacao.Mensagens.Add("Perfil não pode ser vazio!");
    }

    if(validacao.Mensagens.Count > 0 ){
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador{
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
    };
    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdminModelView{
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).WithTags("Admin");
#endregion

#region Veiculos
ValidationError validaDTO(VeiculoDTO veiculoDTO){
    var validacao = new ValidationError{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(veiculoDTO.Nome)){
        validacao.Mensagens.Add("O nome não pode ser vazio!");
    }

    if(string.IsNullOrEmpty(veiculoDTO.Marca)){
        validacao.Mensagens.Add("A marca não pode ser vazia!");
    }

    if(veiculoDTO.Ano < 1900){
        validacao.Mensagens.Add("Veiculo impossivel de existir");
    }

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0 ){
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculo");


app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) => {
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculo");


app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).WithTags("Veiculo");


app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");


app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null) return Results.NotFound();

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0 ){
        return Results.BadRequest(validacao);
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
