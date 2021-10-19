using JetBrains.Annotations;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    public interface IAuthService
    {
        [ItemCanBeNull]
        Task<string> LoginAsync(string userName, string password, bool createNew);
        void LogOut();
        Task TryLoginAsync();

        bool? IsAuthorized { get; }
        string CurrentUserName { get; }
    }
}
