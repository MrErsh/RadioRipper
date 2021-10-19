using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Mvvm
{
    public interface INavigable
    {
        Task NavigatedTo();

        Task NavigatedFrom();
    }
}
