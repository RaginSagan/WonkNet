using BillisNet.Commnds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace BillisNet.ViewModels
{
    public class BillisNetViewModel : INotifyPropertyChanged
    {
        #region Constructor
        public BillisNetViewModel()
        {
            if (Properties.Settings.Default.ImagesRoute != null &&
                Directory.Exists(Properties.Settings.Default.ImagesRoute))
            {
                ImagesRoute = Properties.Settings.Default.ImagesRoute;
            }
            else
            {
                ImagesRoute = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            if (Properties.Settings.Default.MySubreddits != null &&
                Properties.Settings.Default.MySubreddits.Count > 0)
            {
                MySubreddits = new ObservableCollection<string>(Properties.Settings.Default.MySubreddits.Cast<string>().ToList());
            } else if (Properties.Settings.Default.MySubreddits == null)
            {
                Properties.Settings.Default.MySubreddits = new System.Collections.Specialized.StringCollection();
            }

            if (Properties.Settings.Default.MyMultireddits != null &&
                Properties.Settings.Default.MyMultireddits.Count > 0)
            {
                MyMultireddits = new ObservableCollection<string>(Properties.Settings.Default.MyMultireddits.Cast<string>().ToList());
            } else if (Properties.Settings.Default.MyMultireddits == null)
            {
                Properties.Settings.Default.MyMultireddits = new System.Collections.Specialized.StringCollection();
            }

            dispatcher.Tick += new EventHandler(Dispatcher_Tick);
            dispatcher.Start();
        }
        #endregion

        #region Fields
        Random die = new Random();
        ObservableCollection<string> mySubreddits = new ObservableCollection<string>();
        ObservableCollection<string> myMultireddits = new ObservableCollection<string>();
        string imagesRoute;
        int noOfCycles = 0;
        string subredditToAdd;
        string multiredditUserToAdd;
        string multiredditTitleToAdd;
        string currentlySelectedSubreddit;
        string currentlySelectedMultireddit;
        bool automaticIsChecked = false;
        bool manualIsChecked = false;
        int delaySeconds;
        int delayMinutes;
        int delayHours;
        string snarkMessage;
        bool isCurrentlyDownloadingImages;
        string _currentTime;
        string blinkerSource;
        string blinkerSource2;
        string blinkerSource3;
        int downloadsSinceRestart;

        #region Commands
        DownloadJsonCommand downloadJsonCommand = new DownloadJsonCommand();
        ChangeDownloadPathCommand changeDownloadPathCommand = new ChangeDownloadPathCommand();
        AddSubredditCommand addSubredditCommand = new AddSubredditCommand();
        AddMultiredditCommand addMultiredditCommand = new AddMultiredditCommand();
        MoveSubredditUpCommand moveSubredditUpCommand = new MoveSubredditUpCommand();
        MoveSubredditDownCommand moveSubredditDownCommand = new MoveSubredditDownCommand();
        MoveMultiredditUpCommand moveMultiredditUpCommand = new MoveMultiredditUpCommand();
        MoveMultiredditDownCommand moveMultiredditDownCommand = new MoveMultiredditDownCommand();
        RemoveSubredditCommand removeSubredditCommand = new RemoveSubredditCommand();
        RemoveMultiredditCommand removeMultiredditCommand = new RemoveMultiredditCommand();
        #endregion
        #endregion

        #region Properties
        DispatcherTimer dispatcher = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500)     //The dispatcher timer will block the UI thread when it updates,
        };                                             //so a shorter (than 1s) interval of 1ms improves performance.

        public int DownloadsSinceRestart
        {
            get { return downloadsSinceRestart; }
            set
            {
                downloadsSinceRestart = value;
                OnPropertyChanged("DownloadsSinceRestart");
            }
        }

        public bool IsCurrentlyDownloadingImages
        {
            get { return isCurrentlyDownloadingImages; }
            set
            {
                isCurrentlyDownloadingImages = value;
                OnPropertyChanged("IsCurrentlyDownloadingImages");
            }
        }

        public string SnarkMessage
        {
            get { return snarkMessage; }
            set
            {
                snarkMessage = value;
                OnPropertyChanged("SnarkMessage");
            }
        }

        public int DelaySeconds
        {
            get { return delaySeconds; }
            set
            {
                delaySeconds = value;
                OnPropertyChanged("DelaySeconds");
            }
        }

        public int DelayMinutes
        {
            get { return delayMinutes; }
            set
            {
                delayMinutes = value;
                OnPropertyChanged("DelayMinutes");
            }
        }

        public int DelayHours
        {
            get { return delayHours; }
            set
            {
                delayHours = value;
                OnPropertyChanged("DelayHours");
                // If delayHours > 0, color = blue
            }
        }

        public int NoOfCycles
        {
            get { return noOfCycles; }
            set
            {
                noOfCycles = value;
                OnPropertyChanged("NoOfCycles");
            }
        }

        public string ImagesRoute
        {
            get { return imagesRoute; }
            set
            {
                imagesRoute = value;
                OnPropertyChanged("ImagesRoute");
            }
        }

        public string SubredditToAdd
        {
            get { return subredditToAdd; }
            set
            {
                subredditToAdd = value;
                OnPropertyChanged("SubredditToAdd");
            }
        }

        public string MultiredditUserToAdd
        {
            get { return multiredditUserToAdd; }
            set
            {
                multiredditUserToAdd = value;
                OnPropertyChanged("MultiredditUserToAdd");
            }
        }

        public string MultiredditTitleToAdd
        {
            get { return multiredditTitleToAdd; }
            set
            {
                multiredditTitleToAdd = value;
                OnPropertyChanged("MultiredditTitleToAdd");
            }
        }

        public string CurrentlySelectedSubreddit
        {
            get { return currentlySelectedSubreddit; }
            set
            {
                currentlySelectedSubreddit = value;
                OnPropertyChanged("CurrentlySelectedSubreddit");
            }
        }

        public string CurrentlySelectedMultireddit
        {
            get { return currentlySelectedMultireddit; }
            set
            {
                currentlySelectedMultireddit = value;
                OnPropertyChanged("CurrentlySelectedMultireddit");
            }
        }

        public bool AutomaticIsChecked
        {
            get { return automaticIsChecked; }
            set
            {
                automaticIsChecked = value;
                OnPropertyChanged("AutomaticIsChecked");
            }
        }

        public bool ManualIsChecked
        {
            get { return manualIsChecked; }
            set
            {
                manualIsChecked = value;
                OnPropertyChanged("ManualIsChecked");
            }
        }

        public ObservableCollection<string> MySubreddits
        {
            get { return mySubreddits; }
            set
            {
                mySubreddits = value;
                OnPropertyChanged("MySubreddits");
            }
        }
        public ObservableCollection<string> MyMultireddits
        {
            get { return myMultireddits; }
            set
            {
                myMultireddits = value;
                OnPropertyChanged("MyMultireddits");
            }
        }

        public string CurrentTime
        {
            get
            {
                return _currentTime;
            }
            set
            {
                _currentTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }

        public string BlinkerSource
        {
            get { return blinkerSource; }
            set
            {
                blinkerSource = value;
                OnPropertyChanged("BlinkerSource");
            }
        }

        public string BlinkerSource2
        {
            get { return blinkerSource2; }
            set
            {
                blinkerSource2 = value;
                OnPropertyChanged("BlinkerSource2");
            }
        }

        public string BlinkerSource3
        {
            get { return blinkerSource3; }
            set
            {
                blinkerSource3 = value;
                OnPropertyChanged("BlinkerSource3");
            }
        }

        #region Properties - Commands
        public DownloadJsonCommand DownloadJson
        {
            get { return downloadJsonCommand; }
        }

        public ChangeDownloadPathCommand ChangeDownloadPath
        {
            get { return changeDownloadPathCommand; }
        }

        public AddSubredditCommand AddSubreddit
        {
            get { return addSubredditCommand; }
        }

        public AddMultiredditCommand AddMultireddit
        {
            get { return addMultiredditCommand; }
        }

        public MoveSubredditUpCommand MoveSubredditUp
        {
            get { return moveSubredditUpCommand; }
        }

        public MoveSubredditDownCommand MoveSubredditDown
        {
            get { return moveSubredditDownCommand; }
        }

        public RemoveSubredditCommand RemoveSubreddit
        {
            get { return removeSubredditCommand; }
        }

        public RemoveMultiredditCommand RemoveMultireddit
        {
            get { return removeMultiredditCommand; }
        }

        public MoveMultiredditUpCommand MoveMultiredditUp
        {
            get { return moveMultiredditUpCommand; }
        }

        public MoveMultiredditDownCommand MoveMultiredditDown
        {
            get { return moveMultiredditDownCommand; }
        }
        #endregion
        #endregion

        #region Methods
        private string ReturnGreenIfNotNull(string property)
        {
            if (!String.IsNullOrEmpty(property))
            {
                return "LimeGreen";
            }
            return "White";
        }

        private void DetermineBlinker(int value)
        {
            string stringToReturn = null;
            switch (value)
            {
                case 1:
                case 2:
                case 3:
                    stringToReturn = "Assets/Cube1.png";
                    break;
                case 4:
                case 5:
                case 6:
                    stringToReturn = "Assets/Cube2.png";
                    break;
                case 7:
                case 8:
                case 9:
                    stringToReturn = "Assets/Cube3.png";
                    break;
                default:
                    stringToReturn = "Assets/Cube4.png";
                    break;
            }
            BlinkerSource = stringToReturn;
        }

        private void DetermineBlinker2(int value)
        {
            string stringToReturn = null;
            switch (value)
            {
                case 1:
                case 2:
                case 3:
                    stringToReturn = "Assets/Circle8.png";
                    break;
                case 4:
                case 5:
                case 6:
                    stringToReturn = "Assets/Circle6.png";
                    break;
                case 7:
                case 8:
                case 9:
                    stringToReturn = "Assets/Circle4.png";
                    break;
                default:
                    stringToReturn = "Assets/Circle8.png";
                    break;
            }
            BlinkerSource2 = stringToReturn;
        }

        private void DetermineBlinker3(int value)
        {
            string stringToReturn = null;
            switch (value)
            {
                case 1:
                case 2:
                case 3:
                    stringToReturn = "Assets/CubeA.png";
                    break;
                case 4:
                case 5:
                case 6:
                    stringToReturn = "Assets/CubeB.png";
                    break;
                case 7:
                case 8:
                case 9:
                    stringToReturn = "Assets/CubeC.png";
                    break;
                default:
                    stringToReturn = "Assets/CubeA.png";
                    break;
            }
            BlinkerSource3 = stringToReturn;
        }
        #endregion

        #region Events
        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            DetermineBlinker(die.Next(0, 10));
            DetermineBlinker2(die.Next(0, 10));
            DetermineBlinker3(die.Next(0, 10));
            CurrentTime = DateTime.Now.ToString();
            //If current time matches expected time, launch download loop.
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion

        #region INotifyPropertyChanged
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
