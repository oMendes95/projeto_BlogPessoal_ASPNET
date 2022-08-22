using BlogAPI.Src.Modelos;
using System.Threading.Tasks;

namespace BlogAPI.Src.Repositorios
{
    public interface IUsuario
    {
        Task<Usuario> PegarUsuarioPeloEmailAsync(string email);
        Task NovoUsuarioAsync(Usuario usuario);

    }
}
