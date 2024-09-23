using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Interfaces;
using minimal_api.Services;
using MinimalApi.DTOs;
using MinimalApi.Infra.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminServico administradorServico) => {
    if (administradorServico.Login(loginDTO) != null){
        return Results.Ok("Login com sucesso");
    }
    else
        return Results.Unauthorized();
});


app.Run();

