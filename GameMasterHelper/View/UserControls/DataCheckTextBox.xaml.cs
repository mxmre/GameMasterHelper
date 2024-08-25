using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameMasterHelper.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DataCheckTextBox.xaml
    /// </summary>
    public partial class DataCheckTextBox : UserControl, INotifyPropertyChanged
    {
        public delegate bool DataCheckEventHandler(string Data);

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public event DataCheckEventHandler DataCheckEvent;

        private bool DefaultDataCheck(string Data) => false;
        private void _dtc(object sender, TextChangedEventArgs e) { }

        private Brush p_RedBrush;
        private Brush p_GreenBrush;

        private Brush p_sRedBrush;
        private Brush p_sGreenBrush;

        private bool p_checkSuccess;

        public bool Success
        {
            get { return p_checkSuccess; }
            set { p_checkSuccess = value; }
        }

        private string p_placeholder;

        public string Placeholder
        {
            get { return p_placeholder; }
            set { p_placeholder = value; OnPropertyChanged(); }
        }

        private string p_notifyText;

        public string NotifyText
        {
            get { return p_notifyText; }
            set { p_notifyText = value; OnPropertyChanged(); }
        }


        public DataCheckTextBox()
        {
            DataContext = this;
            p_RedBrush = new SolidColorBrush(Color.FromArgb(178, 255, 0, 0));
            p_GreenBrush = new SolidColorBrush(Color.FromArgb(178, 0, 255, 0));
            p_sRedBrush = new SolidColorBrush(Color.FromArgb(51, 255, 0, 0));
            p_sGreenBrush = new SolidColorBrush(Color.FromArgb(51, 0, 255, 0));
            InitializeComponent();
            Success = false;
            //CollapseErrorFrame();
            fError.Visibility = Visibility.Collapsed;
            tbError.Visibility = Visibility.Collapsed;
            DataCheckEvent = DefaultDataCheck;
        }
        public void CheckData()
        {
            if (DataCheckEvent(tbWithDataCheck.Text))
            {
                CollapseErrorFrame();
                Success = true;
            }
            else
            {
                ShowErrorFrame();
                Success = false;
            }
        }
        void UpdateText()
        {
            if (tbWithDataCheck.Text == string.Empty)
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;

            }
            CheckData();
        }
        private void tbWithDataCheck_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateText();
        }

        public string Text { get => tbWithDataCheck.Text;
            set
            {
                tbWithDataCheck.Text = value;
                UpdateText();
            } }

        static string DataTrue = "✓";
        static string DataFalse = "X";
        private void CollapseErrorFrame()
        {
            tbResult.Foreground = p_GreenBrush;
            tbResult.Text = DataTrue;
            tbWithDataCheck.Background = p_sGreenBrush;
            fError.Visibility = Visibility.Collapsed;
            tbError.Visibility = Visibility.Collapsed;
        }
        private void ShowErrorFrame()
        {
            tbResult.Foreground = p_RedBrush;
            tbResult.Text = DataFalse;
            tbWithDataCheck.Background = p_sRedBrush;
            tbError.Visibility = Visibility.Visible;
            fError.Visibility = Visibility.Visible;
            tbError.Text = NotifyText;
        }
    }
}
