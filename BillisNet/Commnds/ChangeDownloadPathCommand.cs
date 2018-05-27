using BillisNet.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace BillisNet.Commnds
{
    public class ChangeDownloadPathCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (!_bnvm.IsCurrentlyDownloadingImages);
        }

        public void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (Properties.Settings.Default.ImagesRoute != null &&
                Directory.Exists(Properties.Settings.Default.ImagesRoute))
            {
                folderBrowser.SelectedPath = Properties.Settings.Default.ImagesRoute;
            }
            DialogResult result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                _bnvm.ImagesRoute = folderBrowser.SelectedPath;
                Properties.Settings.Default.ImagesRoute = folderBrowser.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
