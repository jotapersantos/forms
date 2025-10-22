using System;
using CRM.Application.Interfaces.Formularios;
using CRM.Application.Interfaces.Formularios.Modelos;
using CRM.Application.Mappings;
using CRM.Application.Services.Formularios;
using CRM.Application.Services.Formularios.Modelos;
using CRM.Domain.Interfaces;
using CRM.Domain.Interfaces.Formularios;
using CRM.Domain.Interfaces.Formularios.Modelos;
using CRM.Domain.Interfaces.Formularios.Respostas;
using CRM.Infra.Data.Context;
using CRM.Infra.Data.Repositories;
using CRM.Infra.Data.Repositories.Formularios;
using CRM.Infra.Data.Repositories.Formularios.Modelos;
using CRM.Infra.Data.Repositories.Formularios.Respostas;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CRM.Infra.IoC;

public static class DependencyInjection
{
    public static object AddInfraestrutura(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CRMDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions
                    .MigrationsAssembly(typeof(CRMDbContext).Assembly.FullName))
                .LogTo(Console.WriteLine, LogLevel.Information));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
            });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        #region Repository
        services.AddScoped<IFormularioRepository, FormularioRepository>();
        services.AddScoped<IFormularioGabaritoRepository, FormularioGabaritoRepository>();
        services.AddScoped<IModeloRepository, ModeloRepository>();
        services.AddScoped<IFormularioGabaritoRepository, FormularioGabaritoRepository>();
        #endregion

        #region Services
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        services.AddScoped<IModeloService, ModeloService>();
        services.AddScoped<IFormularioService, FormularioService>();
        services.AddScoped<IFormularioGabaritoService,  FormularioGabaritoService>();
        #endregion

        services.AddAutoMapper(typeof(DomainToDtoMappingProfile));

        return services;
    }
}
