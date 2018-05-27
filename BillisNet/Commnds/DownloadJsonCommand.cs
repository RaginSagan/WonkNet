using BillisNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BillisNet.Snark;
using System.IO;
using BillisNet.Models;

namespace BillisNet.Commnds
{
    public class DownloadJsonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            return (_bnvm.MySubreddits.Count > 0 ||
                _bnvm.MyMultireddits.Count > 0 &&
                !_bnvm.IsCurrentlyDownloadingImages);
        }

        public async void Execute(object parameter)
        {
            var _bnvm = parameter as BillisNetViewModel;
            _bnvm.IsCurrentlyDownloadingImages = true;
            await Task.Run(() =>
            {
                var die = new Random();
                PreppedWebClient preppedClient = new PreppedWebClient();
                foreach (var subreddit in _bnvm.MySubreddits)
                {
                    try
                    {
                        string url;
                        var downloadString = @"http://www.reddit.com/r/" + subreddit + @".json?limit=1000";
                        var thisSubreddit = JObject.Parse(preppedClient.DownloadString(downloadString));
                        for (int i = 0; i < thisSubreddit["data"]["children"].Count(); i++)
                        {
                            string firstUrl = thisSubreddit["data"]["children"][i]["data"]["url"].ToString();
                            if (firstUrl.Substring(0, 8).Equals("https://"))
                            {
                                url = firstUrl.Remove(4, 1);
                            }
                            else
                            {
                                url = firstUrl;
                            }
                            string id = thisSubreddit["data"]["children"][i]["data"]["id"].ToString();
                            string author = thisSubreddit["data"]["children"][i]["data"]["author"].ToString();

                            string linkExtension = url.Split('.').Last();
                            if (linkExtension == "jpg" ||
                                linkExtension == "png" ||
                                linkExtension == "gif" ||
                                linkExtension == "gifv")
                            {
                                string destinationFileName = Path.Combine(_bnvm.ImagesRoute, (subreddit + "_" + id + "_" + author + "." + linkExtension));
                                if (!Directory.GetFiles(_bnvm.ImagesRoute).Contains(destinationFileName))
                                {
                                        preppedClient.DownloadFile(url, destinationFileName);
                                    _bnvm.SnarkMessage = $"Retrieved {destinationFileName}!";
                                    _bnvm.DownloadsSinceRestart++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                foreach (var multireddit in _bnvm.MyMultireddits)
                {
                    try
                    {
                        string url;
                        var multiredditBeginning = multireddit.Split(new string[] { "/m/" }, StringSplitOptions.None).First();
                        var multiredditEnd = multireddit.Split(new string[] { "/m/" }, StringSplitOptions.None).Last();
                        var downloadString = @"http://www.reddit.com/user/" + multiredditBeginning 
                        + @"/m/" + multiredditEnd + @".json?limit=1000";
                        var thisMultireddit = JObject.Parse(preppedClient.DownloadString(downloadString));
                        for (int i = 0; i < thisMultireddit["data"]["children"].Count(); i++)
                        {
                            string firstUrl = thisMultireddit["data"]["children"][i]["data"]["url"].ToString();
                            if (firstUrl.Substring(0, 8).Equals("https://"))
                            {
                                url = firstUrl.Remove(4, 1);
                            }
                            else
                            {
                                url = firstUrl;
                            }
                            string id = thisMultireddit["data"]["children"][i]["data"]["id"].ToString();
                            string author = thisMultireddit["data"]["children"][i]["data"]["author"].ToString();

                            string linkExtension = url.Split('.').Last();
                            if (linkExtension == "jpg" ||
                                linkExtension == "png" ||
                                linkExtension == "gif" ||
                                linkExtension == "gifv")
                            {
                                string destinationFileName = Path.Combine(_bnvm.ImagesRoute, ("mlti-" + multiredditBeginning + "_" + multiredditEnd + "_" + id + "_" + author + "." + linkExtension));
                                if (!Directory.GetFiles(_bnvm.ImagesRoute).Contains(destinationFileName))
                                {
                                    preppedClient.DownloadFile(url, destinationFileName);
                                    _bnvm.SnarkMessage = $"Retrieved {destinationFileName}!";
                                    _bnvm.DownloadsSinceRestart++;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                _bnvm.NoOfCycles++;
                _bnvm.IsCurrentlyDownloadingImages = false;
                preppedClient.Dispose();
                _bnvm.SnarkMessage = "What now, chief?";
            });
        }
        //JObject o = JObject.Parse(tester);
        //System.Windows.Forms.MessageBox.Show(o["data"]["children"][10]["data"]["url"].ToString());
    }
}

