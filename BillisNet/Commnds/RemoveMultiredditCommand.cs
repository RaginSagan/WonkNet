using BillisNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BillisNet.Commnds
{
    public class RemoveMultiredditCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (_bnvm.CurrentlySelectedMultireddit != null);
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            _bnvm.MyMultireddits.RemoveAt(_bnvm.MyMultireddits.IndexOf(_bnvm.CurrentlySelectedMultireddit));
            var newMultis = new StringCollection();
            foreach (var item in _bnvm.MyMultireddits)
            {
                newMultis.Add(item);
            }
            Properties.Settings.Default.MyMultireddits = newMultis;
            Properties.Settings.Default.Save();
        }
    }
}
