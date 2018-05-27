using BillisNet.Snark;
using BillisNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BillisNet.Commnds
{
    public class AddSubredditCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (_bnvm.SubredditToAdd != null && (!_bnvm.MySubreddits.Any(x => x == _bnvm.SubredditToAdd)));
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            _bnvm.MySubreddits.Add(_bnvm.SubredditToAdd);
            Properties.Settings.Default.MySubreddits.Add(_bnvm.SubredditToAdd);
            Properties.Settings.Default.Save();
            var die = new Random();
            try
            {
                _bnvm.SnarkMessage = String.Format(SubredditSnark.subredditSnarkList.ElementAt(die.Next(0, SubredditSnark.subredditSnarkList.Count)), _bnvm.SubredditToAdd);
            }
            catch (Exception ex)
            {
                _bnvm.SnarkMessage = "Got it, chief!";
            }
            _bnvm.SubredditToAdd = null;
        }
    }
}
