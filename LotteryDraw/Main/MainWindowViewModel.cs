using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using LotteryDraw.Annotations;
using LotteryDraw.Data;

namespace LotteryDraw.Main
{
    class MainWindowViewModel:INotifyPropertyChanged
    {

        #region Properties

        private IPeopleRespository _repo = new PeopleRepository();

        public ObservableCollection<string> PeopleCollection { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand GenerateCommand { get; private set; }

        public RelayCommand LoadDataCommand { get; private set; }

        public RelayCommand ClearDataCommand { get; private set; }

        public RelayCommand DeleteSelectedCommand { get; private set; }

        private string _NumberOfWinners { get; set; }

        public string NumberOfWinners
        {
            get { return _NumberOfWinners; }
            set
            {
                
                int n;
                if (int.TryParse(value, out n))
                {
                    if (n > 0 && n < PeopleCollection.Count)
                        _NumberOfWinners = n + "";
                    else
                        _NumberOfWinners = "";
                }
                else
                    _NumberOfWinners = "";
                OnPropertyChanged("NumberOfWinners");
                GenerateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _SelectedItem { get; set; }

        public string SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                DeleteSelectedCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        public MainWindowViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            GenerateCommand = new RelayCommand(OnGenerate, CanGenerate);
            LoadDataCommand = new RelayCommand(OnLoadData);
            ClearDataCommand = new RelayCommand(OnClearData, CanClearAllData);
            DeleteSelectedCommand = new RelayCommand(OnDeleteSelected, CanDeleteSelected);


            // PeopleCollection = _repo.GetPoeple();
            PeopleCollection = new ObservableCollection<string>();
            PeopleCollection.CollectionChanged += OnPeopleCollectionChanged;
        }


        #region Command Delegated Methods

        private void OnLoadData()
        {
            PeopleCollection.Clear();

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "Text Files (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                if (filename != "")
                    ProcessText(filename);
            }
        }



        private void OnClearData()
        {
            PeopleCollection.Clear();
        }
        private bool CanClearAllData()
        {
            return PeopleCollection.Count > 0;
        }

        private void OnDeleteSelected()
        {
            PeopleCollection.Remove(SelectedItem);
        }
        private bool CanDeleteSelected()
        {
            return SelectedItem != null;
        }

        private void OnGenerate()
        {
            int numberOfWinners;
            if (Int32.TryParse(NumberOfWinners, out numberOfWinners))
            {
                string winnersAre = "";

                for (int i = 0; i < numberOfWinners; i++)
                {
                    Random randomGenerator = new Random();
                    var luckyWinnerPosition = randomGenerator.Next(0, PeopleCollection.Count);
                    var luckyWinner = PeopleCollection[luckyWinnerPosition];
                    PeopleCollection.Remove(luckyWinner);
                    winnersAre += i + 1 + ": " + luckyWinner + System.Environment.NewLine;
                }
                MessageBox.Show(winnersAre, "Lucky Winners");
            }
            NumberOfWinners = "";

            GenerateCommand.RaiseCanExecuteChanged();

        }

        private bool CanGenerate()
        {
            if (PeopleCollection != null && PeopleCollection.Count > 0 &&
                !string.IsNullOrEmpty(NumberOfWinners) && Int32.Parse(NumberOfWinners) < PeopleCollection.Count)
                return true;
            return false;
        }

        #endregion


        #region Methods

        private void ProcessText(string fileName)
        {
            string[] linesFromFile = System.IO.File.ReadAllLines(fileName);

            if (linesFromFile.Length > 0)
            {
                foreach (string line in linesFromFile)
                {
                    PeopleCollection.Add(line);
                }
            }
        }

        #endregion


        #region Event Handlers

        private void OnPeopleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GenerateCommand.RaiseCanExecuteChanged();
            ClearDataCommand.RaiseCanExecuteChanged();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion







       
    }
}
