using System.Threading.Tasks;

namespace MusicAndMind2.Services
{
    public interface ISmsSender
    {
        Task SendAsync(string to, string message);
    }
}
