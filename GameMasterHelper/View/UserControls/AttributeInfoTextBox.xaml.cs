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
    /// Логика взаимодействия для AttributeInfoTextBox.xaml
    /// </summary>
    public partial class AttributeInfoTextBox : UserControl, INotifyPropertyChanged
    {
        public AttributeInfoTextBox()
        {
            DataContext = this;
            InitializeComponent();
            
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string p_attrValue;

        public string AttrValue
        {
            get { return p_attrValue; }
            set { p_attrValue = value; OnPropertyChanged(); }
        }

        private string p_attrMod;

        public string AttrMod
        {
            get { return p_attrMod; }
            set { p_attrMod = value; OnPropertyChanged(); }
        }

        private string p_text;

        public string Text
        {
            get { return p_text; }
            set { p_text = value; OnPropertyChanged(); }
        }


        private Brush p_brush;

        public Brush Fill
        {
            get { return p_brush; }
            set { p_brush = value; OnPropertyChanged(); }
        }

        private string p_info;

        public string Information
        {
            get { return p_info; }
            set { p_info = value; OnPropertyChanged(); }
        }


    }
}
