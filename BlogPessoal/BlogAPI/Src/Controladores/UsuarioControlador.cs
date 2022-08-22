//using BlogPessoal.src.dtos;
using Microsoft.AspNetCore.Http;
//using BlogPessoal.src.repositorios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogAPI.Src.Modelos;
using BlogAPI.Src.Repositorios;
using BlogAPI.Src.Servicos;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BlogAPI.Src.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsuarioControlador : ControllerBase
    {
            #region Atributos
            private readonly IUsuario _repositorio;
            private readonly IAutenticacao _servicos;
            #endregion 

            #region Construtores
            public UsuarioControlador(IUsuario repositorio, IAutenticacao servicos)
            {
                _repositorio = repositorio;
                _servicos = servicos;
            }
            #endregion

            #region Métodos
              [HttpGet("email/{emailUsuario}")]
            public async Task<ActionResult> PegarUsuarioPeloEmailAsync([FromRoute] string emailUsuario)
            {
                var usuario = await _repositorio.PegarUsuarioPeloEmailAsync(emailUsuario);
                if (usuario == null) return NotFound(new
                {
                    Mensagem = "Usuario não encontrado" });
                    return Ok(usuario);
            }

              [HttpPost("cadastrar")]
              [AllowAnonymous]
            public async Task<ActionResult> NovoUsuarioAsync([FromBody] Usuario usuario)
            {
                try
                {
                    await _servicos.CriarUsuarioSemDuplicarAsync(usuario);
                    return Created($"api/Usuarios/email/{usuario.Email}", usuario);
                }
                catch (Exception ex)
                {
                    return Unauthorized(ex.Message);
                }
            }
              [HttpPost("logar")]
              [AllowAnonymous]
            public async Task<ActionResult> LogarAsync([FromBody] Usuario usuario)
            {
                var auxiliar = await _repositorio.PegarUsuarioPeloEmailAsync(usuario.Email);
                if (auxiliar == null) return Unauthorized(new
            {
                Mensagem = "E-mail invalido"
            });
                if (auxiliar.Senha != _servicos.CodificarSenha(usuario.Senha))
                return Unauthorized(new { Mensagem = "Senha invalida" });
                var token = "Bearer " + _servicos.GerarToken(auxiliar);
                return Ok(new { Usuario = auxiliar, Token = token });
            }


        #endregion
    }
}




