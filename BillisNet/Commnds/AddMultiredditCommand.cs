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
    public class AddMultiredditCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (!String.IsNullOrWhiteSpace(_bnvm.MultiredditUserToAdd) &&
                !String.IsNullOrWhiteSpace(_bnvm.MultiredditTitleToAdd));
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            StringBuilder builder = new StringBuilder();
            builder.Append(_bnvm.MultiredditUserToAdd + "/m/" + _bnvm.MultiredditTitleToAdd);
            var newMultireddit = builder.ToString();
            _bnvm.MyMultireddits.Add(newMultireddit);
            Properties.Settings.Default.MyMultireddits.Add(newMultireddit);
            Properties.Settings.Default.Save();
            var die = new Random();
            try
            {
                _bnvm.SnarkMessage = String.Format(SubredditSnark.subredditSnarkList.ElementAt(die.Next(0, SubredditSnark.subredditSnarkList.Count)), _bnvm.MultiredditTitleToAdd);
            }
            catch (Exception ex)
            {
                _bnvm.SnarkMessage = "Got it, chief!";
            }
            _bnvm.MultiredditUserToAdd = null;
            _bnvm.MultiredditTitleToAdd = null;
        }
    }
}
