﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogAPI.Src.Contextos;
using BlogAPI.Src.Modelos;
//using BlogPessoal.src.contextos;
//using BlogPessoal.src.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Src.Repositorios.Implementacoes
{
    /// <summary>
    /// <para>Resumo: Classe responsavel por implementar IPostagem</para>
    /// <para>Criado por: Generation</para>
    /// <para>Versão: 1.0</para>
    /// <para>Data: 12/05/2022</para>
    /// </summary>
    public class PostagemRepositorio : IPostagem
    {
        #region Atributos
        private readonly BlogPessoalContexto _contexto;
        #endregion Atributos
        #region Construtores
        public PostagemRepositorio(BlogPessoalContexto contexto)
        {
            _contexto = contexto;
        }
        #endregion Construtores
        #region Métodos

        /// <summary>
        /// <para>Resumo: Método assíncrono para salvar uma nova postagem</para>
        /// </summary>
        /// <param name="postagem">Construtor para cadastrar postagem</param>
        /// <exception cref="Exception">Id não pode ser nulo</exception>
        public async Task NovaPostagemAsync(Postagem postagem)
        {
             if (!ExisteUsuarioId(postagem.Criador.Id)) throw new Exception("Id do usuário não encontrado");
             if (!ExisteTemaId(postagem.Tema.Id)) throw new Exception("Id do tema não encontrado");
                await _contexto.Postagens.AddAsync(
                   new Postagem
                   {
                     Titulo = postagem.Titulo,
                     Descricao = postagem.Descricao,
                     Foto = postagem.Foto,
                     Criador = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id ==
                     postagem.Criador.Id),
                     Tema = await _contexto.Temas.FirstOrDefaultAsync(t => t.Id ==
                     postagem.Tema.Id)
                   });
                         await _contexto.SaveChangesAsync();
                     // funções auxiliares
                         bool ExisteUsuarioId(int id)
                         {
                           var auxiliar = _contexto.Usuarios.FirstOrDefault(u => u.Id == id);
                           return auxiliar != null;
                         }
                         bool ExisteTemaId(int id)
                         {
                           var auxiliar = _contexto.Temas.FirstOrDefault(t => t.Id == id);
                           return auxiliar != null;
                         }
        }

        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar todas postagens</para>
        /// </summary>
        /// <return>Lista PostagemModelo></return>
        public async Task<List<Postagem>> PegarTodasPostagensAsync()
        {
            return await _contexto.Postagens
            .Include(p => p.Criador)
            .Include(p => p.Tema)
            .ToListAsync();
        }
        /// <summary>
        /// <para>Resumo: Método assíncrono para pegar uma postagem pelo Id</para>
        /// </summary>
        /// <param name="id">Id da postagem</param>
        /// <return>PostagemModelo</return>
        /// <exception cref="Exception">Id não pode ser nulo</exception>
        public async Task<Postagem> PegarPostagemPeloIdAsync(int id)
        {
            if (!ExisteId(id)) throw new Exception("Id da postagem não encontrado");
            return await _contexto.Postagens
            .Include(p => p.Criador)
            .Include(p => p.Tema)
            .FirstOrDefaultAsync(p => p.Id == id);
            bool ExisteId(int id)
            {
                var auxiliar = _contexto.Postagens.FirstOrDefault(u => u.Id == id);
                return auxiliar != null;
            }
        }
        /// <summary>
        /// <para>Resumo: Método assíncrono para atualizar uma postagem</para>
        /// </summary>
        /// <param name="postagem">Construtor para atualizar postagem</param>
        /// <exception cref="Exception">Id não pode ser nulo</exception>
        public async Task AtualizarPostagemAsync(Postagem postagem)
        {
            if (!ExisteTemaId(postagem.Tema.Id)) throw new Exception("Id do tema não encontrado");

            var postagemExistente = await PegarPostagemPeloIdAsync(postagem.Id);
            postagemExistente.Titulo = postagem.Titulo;
            postagemExistente.Descricao = postagem.Descricao;
            postagemExistente.Foto = postagem.Foto;
            postagemExistente.Tema = await _contexto.Temas.FirstOrDefaultAsync(t => t.Id
            == postagem.Tema.Id);
            _contexto.Postagens.Update(postagemExistente);
            await _contexto.SaveChangesAsync();
            
            // funções auxiliares
            bool ExisteTemaId(int id)
            {
                var auxiliar = _contexto.Temas.FirstOrDefault(t => t.Id == id);
                return auxiliar != null;
            }
        }
        public async Task DeletarPostagemAsync(int id)
        {
            _contexto.Postagens.Remove(await PegarPostagemPeloIdAsync(id));
            await _contexto.SaveChangesAsync();
        }
        #endregion Métodos
    }
}