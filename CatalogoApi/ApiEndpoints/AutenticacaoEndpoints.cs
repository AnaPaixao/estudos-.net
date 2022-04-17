using CatalogoApi.Models;
using CatalogoApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogoApi.ApiEndpoints;

public static class AutenticacaoEndpoints {
    public static void MapAutenticacaoEndpoints(this WebApplication app) {
        app.MapPost("/login", [AllowAnonymous] (UserModel UserModel, ITokenService tokenService) => {
  if(UserModel == null) {
    return Results.BadRequest("Login Inválido");
  }

  if(UserModel.UserName == "ana" && UserModel.Password == "qwert") {
    var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
        app.Configuration["Jwt:Issuer"],
        app.Configuration["Jwt:Audience"],
        UserModel);
        return Results.Ok(new {token = tokenString});
    
  } else {
    return Results.BadRequest("Login Inválido");
  }

}).Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status200OK)
.WithName("Login")
.WithTags("Autenticacao");
    }
}