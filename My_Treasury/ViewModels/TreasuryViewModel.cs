using My_Treasury.Models;
using My_Treasury.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace My_Treasury.ViewModels
{
    public class TreasuryViewModel : ObservableObject
    {
        private string _selectedCurrencySymbol;
        public string SelectedCurrencySymbol
        {
            get { return _selectedCurrencySymbol; }
            set { OnPropertyChanged(ref _selectedCurrencySymbol, value); }
        }
        /// Last saved treasury value
        private string _lastSavedValue;
        public string LastSavedValue
        {
            get { return _lastSavedValue; }
            set { OnPropertyChanged(ref _lastSavedValue, value); }
        }
        /// Last saved date
        private string _lastSavedDate;
        public string LastSavedDate
        {
            get { return _lastSavedDate; }
            set { OnPropertyChanged(ref _lastSavedDate, value); }
        }
        /// Treasury Input
        private string _treasuryInput;
        public string TreasuryInput
        {
            get { return _treasuryInput; }
            set { OnPropertyChanged(ref _treasuryInput, value); }
        }
        /// Allow or not allow adding symbol
        private bool _isAddBtnEnabled;
        public bool IsAddBtnEnabled
        {
            get
            {
                if (AddingPropertiesVisibility == Visibility.Visible)
                    _isAddBtnEnabled = false;
                else
                    _isAddBtnEnabled = true;

                return _isAddBtnEnabled;
            }
            set { OnPropertyChanged(ref _isAddBtnEnabled, value); }
        }
        /// If there is a currency in the collection then allow it otherwise don't
        private bool _isRemoveBtnEnabled;
        public bool IsRemoveBtnEnabled
        {
            get
            {
                if (CurrencyCollection.Count == 0)
                    _isRemoveBtnEnabled = false;

                return _isRemoveBtnEnabled;
            }
            set { OnPropertyChanged(ref _isRemoveBtnEnabled, value); }
        }
        /// If an item selected allow it otherwise don't
        private Visibility _isDeleteItemBtnEnabled;
        public Visibility IsDeleteItemBtnEnabled
        {
            get
            {
                if (SelectedTreasury == null)
                    _isDeleteItemBtnEnabled = Visibility.Hidden;

                return _isDeleteItemBtnEnabled;
            }
            set { OnPropertyChanged(ref _isDeleteItemBtnEnabled, value); }
        }
        /// If an item selected allow it otherwise don't
        private Visibility _isClearBtnEnabled;
        public Visibility IsClearBtnEnabled
        {
            get
            {
                if (TreasuryCollection.Count == 0)
                    _isClearBtnEnabled = Visibility.Hidden;

                return _isClearBtnEnabled;
            }
            set { OnPropertyChanged(ref _isClearBtnEnabled, value); }
        }
        private Visibility _addingPropertiesVisibility;
        public Visibility AddingPropertiesVisibility
        {
            get { return _addingPropertiesVisibility; }
            set
            {
                OnPropertyChanged(ref _addingPropertiesVisibility, value);
                OnPropertyChanged("IsAddBtnEnabled");
            }
        }
        /// Fields that used only in code
        private string[] getCurrencySymbol;
        private int lastSavedInt;
        private int numberLimit = 100000;
        //////////
        /// *** Currency adding *** ///
        public string _currencySymbolHolder;
        public string CurrencySymbolHolder
        {
            get
            {
                if(_currencySymbolHolder != null)
                _currencySymbolHolder = _currencySymbolHolder.Trim();
                return _currencySymbolHolder;
            }
            set { OnPropertyChanged(ref _currencySymbolHolder, value); }
        }
        public string _currencyTitleHolder;
        public string CurrencyTitleHolder
        {
            get { return _currencyTitleHolder; }
            set { OnPropertyChanged(ref _currencyTitleHolder, value); }
        }

        /// Collection for currencies
        private ObservableCollection<string> _currencyCollection;
        public ObservableCollection<string> CurrencyCollection
        {
            get { return _currencyCollection; }
            set { OnPropertyChanged(ref _currencyCollection, value); }
        }
        /// Collection for history
        private ObservableCollection<MyTreasuryInfo> _treasuryCollection;
        public ObservableCollection<MyTreasuryInfo> TreasuryCollection
        {
            get { return _treasuryCollection; }
            set { OnPropertyChanged(ref _treasuryCollection, value); }
        }
        /// Selected treasury data by list view
        private MyTreasuryInfo _selectedTreasury;
        public MyTreasuryInfo SelectedTreasury
        {
            get { return _selectedTreasury; }
            set { OnPropertyChanged(ref _selectedTreasury, value); }
        }
        /// Selected currency data by list view
        private string _selectedCurrencyItem;
        public string SelectedCurrencyItem
        {
            get { return _selectedCurrencyItem; }
            set { OnPropertyChanged(ref _selectedCurrencyItem, value); }
        }

        /// Commands
        public ICommand SaveCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand AddCurrency { get; set; }
        public ICommand CancelAddingCurrencyCommand { get; set; }
        public ICommand RemoveCurrency { get; set; }
        public ICommand RemoveCurrencyApprove { get; set; }
        public ICommand CloseCurrencyPopupCommand { get; set; }
        public ICommand RemoveItem { get; set; }
        public ICommand RemoveItemApprove { get; set; }
        public ICommand CloseItemPopupCommand { get; set; }
        public ICommand ClearAllItem { get; set; }
        public ICommand ClearItemApprove { get; set; }
        public ICommand CloseClearPopupCommand { get; set; }

        /// Objects
        private MainWindow _MyMainWindow;
        private JsonCurrencyDataService _jsonDataService;
        private CurrencyDeletePopup _CurrencyDeleteView;
        private ItemDeletePopup _ItemDeleteView;
        private ItemClearAllPopup _ItemClearAllView;

        public TreasuryViewModel(MainWindow mw)
        {
            _MyMainWindow = mw;
            _jsonDataService = new JsonCurrencyDataService();
            CurrencyCollection = _jsonDataService.GetCurrencyData().ToObservableCollection();
            TreasuryCollection = _jsonDataService.GetTreasuryData().ToObservableCollection();
            GetTreasuryAndDateValues();

            SelectedCurrencyItem = _jsonDataService.GetSelectedCurrencyData();
            AddingPropertiesVisibility = Visibility.Collapsed;
            IsRemoveBtnEnabled = true;

            GetSelectedCurrencyValue();

            SaveCommand = new RelayCommand(SaveTreasuryData);
            AddCommand = new RelayCommand(AllowAddingCurrency);
            AddCurrency = new RelayCommand(AddNewCurrencyToCollection);
            CancelAddingCurrencyCommand = new RelayCommand(CancelAddingCurrency);
            /// Remove Curreny button
            RemoveCurrency = new RelayCommand(RemoveCurrencyCommanding);
            RemoveCurrencyApprove = new RelayCommand(RemoveCurrencyFromCollection);
            CloseCurrencyPopupCommand = new RelayCommand(CloseCurrencyPopup);
            /// Remove Item button
            RemoveItem = new RelayCommand(RemoveItemCommanding);
            RemoveItemApprove = new RelayCommand(RemoveItemFromCollection);
            CloseItemPopupCommand = new RelayCommand(CloseItemPopup);
            /// Clear button
            ClearAllItem = new RelayCommand(ClearAllItemsCommanding);
            ClearItemApprove = new RelayCommand(ClearAllFromCollection);
            CloseClearPopupCommand = new RelayCommand(CloseClearPopup);

        }
        private void GetTreasuryAndDateValues()
        {
            if(TreasuryCollection.Count != 0)
            {
                LastSavedValue = TreasuryCollection[0].TreasuryValue;
                LastSavedDate = TreasuryCollection[0].DateValue;
            }
        }
        public void GetSelectedCurrencyValue()
        {
            if (!string.IsNullOrEmpty(SelectedCurrencyItem))
            {
                getCurrencySymbol = SelectedCurrencyItem.Split('-');
                SelectedCurrencySymbol = getCurrencySymbol[0];
            } else
            {
                if(CurrencyCollection.Count == 0)
                {
                    SelectedCurrencySymbol = string.Empty;
                }
                else
                {
                    getCurrencySymbol = CurrencyCollection[0].Split('-');
                    SelectedCurrencySymbol = getCurrencySymbol[0];
                }
            }
            _MyMainWindow.SelectLastSelectedItem(GetSelectedCurrencyIndex());
        }
        public int GetSelectedCurrencyIndex()
        {
            if (!string.IsNullOrEmpty(SelectedCurrencyItem))
            {
                getCurrencySymbol = SelectedCurrencyItem.Split('-');
                SelectedCurrencySymbol = getCurrencySymbol[0];

                int count = 0;
                foreach (var item in CurrencyCollection)
                {
                    var symbol = item.Split('-')[0];
                    if (SelectedCurrencySymbol == symbol)
                    {
                        return count;
                    }
                    count++;
                }
            }
            return 0;
        }
        public void SaveSelectedCurrency()
        {
            _jsonDataService.SaveSelectedCurrencyData(SelectedCurrencyItem);
        }
        private void SaveTreasuryData()
        {
            /// If input is null or empty, show a message and quit.
            if (string.IsNullOrWhiteSpace(TreasuryInput))
            {
                MessageBox.Show("Please, enter a valid number.");
                return;
            }
            if ( TreasuryInput.Contains('.') || TreasuryInput.Contains(','))
            {
                /// Remove punctuation marks
                string fixedInput;
                fixedInput = Regex.Replace(TreasuryInput, @"[.,]", "");

                if (!Int32.TryParse(fixedInput, out lastSavedInt))
                {
                    if (lastSavedInt >= numberLimit)
                    {
                        MessageBox.Show("Number should be less than 100.000!");
                        TreasuryInput = string.Empty;
                    }
                    else
                        MessageBox.Show("Please, enter a valid number.");
                }
                else
                {
                    if (lastSavedInt >= numberLimit)
                    {
                        MessageBox.Show("Number should be less than 100.000!");
                        TreasuryInput = string.Empty;
                    }
                    else
                    {
                        LastSavedValue = SelectedCurrencySymbol.Trim() + lastSavedInt;
                        DateTime dateTime = DateTime.UtcNow.Date;
                        LastSavedDate = dateTime.ToString("dd/MM/yyyy") + " - " + DateTime.Now.DayOfWeek;

                        MyTreasuryInfo info = new MyTreasuryInfo
                        {
                            TreasuryValue = LastSavedValue,
                            DateValue = LastSavedDate
                        };
                        TreasuryCollection.Insert(0, info);
                        TreasuryInput = string.Empty;
                    }
                }
            }
            else
            {
                /// If used only integer numbers
                if (!Int32.TryParse(TreasuryInput, out lastSavedInt))
                {
                    if (lastSavedInt >= numberLimit)
                    {
                        MessageBox.Show("Number should be less than 100.000!");
                        TreasuryInput = string.Empty;
                    }
                    else
                        MessageBox.Show("Please, enter a valid number.");
                }
                else
                {
                    if (lastSavedInt >= numberLimit)
                    {
                        MessageBox.Show("Number should be less than 100.000!");
                        TreasuryInput = string.Empty;
                    }
                    else
                    {
                        LastSavedValue = SelectedCurrencySymbol.Trim() + lastSavedInt;
                        DateTime dateTime = DateTime.UtcNow.Date;
                        LastSavedDate = dateTime.ToString("dd/MM/yyyy") + " - " + DateTime.Now.DayOfWeek;

                        MyTreasuryInfo info = new MyTreasuryInfo
                        {
                            TreasuryValue = LastSavedValue,
                            DateValue = LastSavedDate
                        };
                        TreasuryCollection.Insert(0, info);
                        TreasuryInput = string.Empty;
                    }
                }
            }
            IsClearBtnEnabled = Visibility.Visible;
            _jsonDataService.SaveTreasuryData(TreasuryCollection); /// Saves treasury data to the hard drive
        }
        private void AllowAddingCurrency()
        {
            AddingPropertiesVisibility = Visibility.Visible;
        }
        private void CancelAddingCurrency()
        {
            AddingPropertiesVisibility = Visibility.Collapsed;
            CurrencySymbolHolder = string.Empty;
            CurrencyTitleHolder = string.Empty;
        }
        private void AddNewCurrencyToCollection()
        {
            if (!string.IsNullOrEmpty(CurrencySymbolHolder) && !string.IsNullOrEmpty(CurrencyTitleHolder))
            {
                string NewCurrencyMade = CurrencySymbolHolder + " - " + CurrencyTitleHolder;
                CurrencyCollection.Add(NewCurrencyMade);
                CurrencySymbolHolder = string.Empty;
                CurrencyTitleHolder = string.Empty;
                AddingPropertiesVisibility = Visibility.Collapsed;
                int lastItemIndex = CurrencyCollection.Count -1;
                _MyMainWindow.SelectLastAvailableItem(lastItemIndex);
                IsRemoveBtnEnabled = true;
                _jsonDataService.SaveCurrencyData(CurrencyCollection); /// Saves currency data to the hard drive
            }
            else
                MessageBox.Show("Please, fill both boxes.");
        }
        /// <summary>
        /// REMOVE CURRENCY
        /// </summary>
        private void RemoveCurrencyCommanding()
        {
            _CurrencyDeleteView = new CurrencyDeletePopup();
            _CurrencyDeleteView.DataContext = this;
            _CurrencyDeleteView.Show();
        }
        private void RemoveCurrencyFromCollection()
        {
            CurrencyCollection.Remove(SelectedCurrencyItem);
            _MyMainWindow.SelectFirstItem(); /// Selects first item after removing
            _CurrencyDeleteView.Close();
            _jsonDataService.SaveCurrencyData(CurrencyCollection); /// Saves currency data to the hard drive

            if (CurrencyCollection.Count == 0)
                IsRemoveBtnEnabled = false;
        }
        private void CloseCurrencyPopup()
        {
            _CurrencyDeleteView.Close();
        }
        /// <summary>
        /// REMOVE ITEM
        /// </summary>
        private void RemoveItemCommanding()
        {
            _ItemDeleteView = new ItemDeletePopup();
            _ItemDeleteView.DataContext = this;
            _ItemDeleteView.Show();
        }
        private void RemoveItemFromCollection()
        {
            TreasuryCollection.Remove(SelectedTreasury);
            if(TreasuryCollection.Count == 0)
            {
                LastSavedValue = string.Empty;
                LastSavedDate = string.Empty;
                IsClearBtnEnabled = Visibility.Visible;
            } else
            {
                LastSavedValue = TreasuryCollection[0].TreasuryValue;
                LastSavedDate = TreasuryCollection[0].DateValue;
            }

            _ItemDeleteView.Close();
            _jsonDataService.SaveTreasuryData(TreasuryCollection); /// Saves treasury data to the hard drive
        }
        private void CloseItemPopup()
        {
            _ItemDeleteView.Close();
        }
        /// <summary>
        /// CLEAR ALL ITEMS
        /// </summary>
        private void ClearAllItemsCommanding()
        {
            _ItemClearAllView = new ItemClearAllPopup();
            _ItemClearAllView.DataContext = this;
            _ItemClearAllView.Show();
        }
        private void ClearAllFromCollection()
        {
            TreasuryCollection.Clear();
            LastSavedValue = string.Empty;
            LastSavedDate = string.Empty;
            IsClearBtnEnabled = Visibility.Visible;

            _ItemClearAllView.Close();
            _jsonDataService.SaveTreasuryData(TreasuryCollection); /// Saves treasury data to the hard drive
        }
        private void CloseClearPopup()
        {
            _ItemClearAllView.Close();
        }
    }

    public static class ExtendedObservableCollection
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> _thisCollection)
        {
            ObservableCollection<T> myObservableCollection = new ObservableCollection<T>();
            foreach (var item in _thisCollection)
            {
                myObservableCollection.Add(item);
            }
            return myObservableCollection;
        }
    }
}
