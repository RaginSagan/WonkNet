using BillisNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BillisNet.Commnds
{
    public class MoveMultiredditDownCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (_bnvm.MyMultireddits.IndexOf(_bnvm.CurrentlySelectedMultireddit) + 1 != _bnvm.MyMultireddits.Count);
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            MoveDownLogic(_bnvm.MyMultireddits, _bnvm.CurrentlySelectedMultireddit);
            var newMultis = new StringCollection();
            foreach (var item in _bnvm.MyMultireddits)
            {
                newMultis.Add(item);
            }
            Properties.Settings.Default.MyMultireddits = newMultis;
            Properties.Settings.Default.Save();
        }

        private void MoveDownLogic(ObservableCollection<string> collection, string currentMultireddit)
        {
            collection.Move(collection.IndexOf(currentMultireddit), collection.IndexOf(currentMultireddit) + 1);
        }
    }
}
