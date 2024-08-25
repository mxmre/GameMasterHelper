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
    /// Логика взаимодействия для TextBoxWithPlaceholder.xaml
    /// </summary>
    public partial class TextBoxWithPlaceholder : UserControl, INotifyPropertyChanged
    {
        public TextBoxWithPlaceholder()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string p_placeholder;

        public string Placeholder
        {
            get { return p_placeholder; }
            set { p_placeholder = value; OnPropertyChanged(); }
        }


        public string Text
        {
            get { return this.tbBase.Text; }
            set { this.tbBase.Text = value; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void tbBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbBase.Text == string.Empty)
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;

            }
        }
    }
}
