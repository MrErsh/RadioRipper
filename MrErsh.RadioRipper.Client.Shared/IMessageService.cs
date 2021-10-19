using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    public interface IMessageService
    {
        Task<bool> ShowMessageAsync(string title, string message);
        Task ShowErrorAsync(string title, string text);
    }
}
