
namespace RoboTooth.ViewModel.Commands
{
    /// <summary>
    /// Interface for invoking the CanExecuteChanged event
    /// on commands.
    /// </summary>
    public interface ICanExecuteChangedInvoker
    {
        void InvokeCanExecuteChanged();
    }
}
