using System;
using System.Threading.Tasks;

namespace RoboTooth.ViewModel.Commands
{
    internal class AsyncCommand : Command
    {
        public AsyncCommand(Func<object, bool> CanExecute, Action<object> Execute) : base(CanExecute, Execute)
        {
        }

        public override void Execute(object parameter)
        {
            Task.Factory.StartNew(() => base.Execute(parameter));
        }
    }
}
