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
    public class MoveSubredditUpCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (!(_bnvm.MySubreddits.IndexOf(_bnvm.CurrentlySelectedSubreddit) - 1 < 0));
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            MoveDownLogic(_bnvm.MySubreddits, _bnvm.CurrentlySelectedSubreddit);
            var newSubs = new StringCollection();
            foreach (var item in _bnvm.MySubreddits)
            {
                newSubs.Add(item);
            }
            Properties.Settings.Default.MySubreddits = newSubs;
            Properties.Settings.Default.Save();
        }

        private void MoveDownLogic(ObservableCollection<string> collection, string currentSubreddit)
        {
            collection.Move(collection.IndexOf(currentSubreddit), collection.IndexOf(currentSubreddit) - 1);
        }
    }
}
