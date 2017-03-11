using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTooth.ViewModel
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
