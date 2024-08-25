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
    /// Логика взаимодействия для SkillField.xaml
    /// </summary>
    public partial class SkillField : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string p_fieldName;

        public string FieldName
        {
            get { return p_fieldName; }
            set { p_fieldName = value; OnPropertyChanged(); }
        }
        private string p_fieldText;

        public string FieldText
        {
            get { return p_fieldText; }
            set { p_fieldText = value; OnPropertyChanged(); }
        }


        public SkillField()
        {
            InitializeComponent();
        }
        public SkillField(string name, string text) : this()
        {
            tbText.Text = text;
            tbName.Text = name;
            FieldName = name;
            FieldText = text;
        }
    }
}
