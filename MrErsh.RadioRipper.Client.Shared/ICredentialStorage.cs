using JetBrains.Annotations;
using MrErsh.RadioRipper.Model.Dto;

namespace MrErsh.RadioRipper.Client.Services
{
    public interface ICredentialStorage
    {
        [CanBeNull]
        LoginDto GetLoginInfo();
        void Store([NotNull] string userName, [NotNull] string password);
        void Clear();
    }
}
