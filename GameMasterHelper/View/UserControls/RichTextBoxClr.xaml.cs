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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameMasterHelper.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RichTextBoxClr.xaml
    /// </summary>
    public partial class RichTextBoxClr : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public RichTextBoxClr()
        {
            DataContext = this;
            InitializeComponent();
            if (this.Text.Length > 0)
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private string p_placeholder;

        public string Placeholder
        {
            get { return p_placeholder; }
            set { p_placeholder = value; OnPropertyChanged(); }
        }

        public string Text 
        {   get
            {
                string text = new TextRange(rtbBase.Document.ContentStart,
                              rtbBase.Document.ContentEnd).Text;
                return (text.Length >= 2) ? text.Remove(text.Length - 2, 2) : text;
            }
            set
            {
                if(value != string.Empty) 
                {
                    tbPlaceholder.Visibility = Visibility.Hidden;
                }
                else 
                {
                    tbPlaceholder.Visibility = Visibility.Visible;
                }
                var values = value.Split('\r');
                rtbBase.Document.Blocks.Clear();
                foreach (var item in values)
                {
                    rtbBase.Document.Blocks.Add(new Paragraph(new Run(item.Replace("\n", ""))));
                }
            }
        }

        private void rtbBase_GotFocus(object sender, RoutedEventArgs e)
        {
            tbPlaceholder.Visibility = Visibility.Hidden;
        }

        private void rtbBase_LostFocus(object sender, RoutedEventArgs e)
        {
            if(this.Text.Length == 0)
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            
        }
    }
}
