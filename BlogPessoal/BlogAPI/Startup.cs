using BlogAPI.Src.Contextos;
using BlogAPI.Src.Repositorios;
using BlogAPI.Src.Repositorios.Implementacoes;
using BlogAPI.Src.Servicos;
using BlogAPI.Src.Servi�os.Implementacoes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configura��o de Banco de dados
            services.AddDbContext<BlogPessoalContexto>(opt => opt.UseSqlServer(Configuration["ConnectionStringsDev:DefaultConnection"]));

            // Repositorios
            services.AddScoped<IUsuario, UsuarioRepositorio>();
            services.AddScoped<ITema, TemaRepositorio>();
            services.AddScoped<IPostagem, PostagemRepositorio>();

            // Controladores
            services.AddCors();
            services.AddControllers();

            // Configura��o de Servi�os
            services.AddScoped<IAutenticacao, AutenticacaoServicos>();
            // Configura��o do Token Autentica��o JWTBearer
            var chave = Encoding.ASCII.GetBytes(Configuration["Settings:Secret"]);
            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(b =>
            {
                b.RequireHttpsMetadata = false;
                b.SaveToken = true;
                b.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(chave),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            }
            );

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BlogPessoalContexto contexto)

        {
            if (env.IsDevelopment())
            {
                contexto.Database.EnsureCreated();
                app.UseDeveloperExceptionPage();
            }

            // Ambiente de produ��o
            // Rotas
            app.UseRouting();

            app.UseCors(c => c
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            // Autentica��o e Autoriza��o
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
