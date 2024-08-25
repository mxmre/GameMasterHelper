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
    /// Логика взаимодействия для SkillList.xaml
    /// </summary>
    public partial class SkillList : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnSkillsListChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Skills")
                return;
            spMain.Children.Clear();
            int i = 0;
            foreach (var item in Skills)
            {
                var skill = new SkillField(item.Item1, item.Item2);
                skill.mainGrid.Background = i == 0 ? FirstFieldBG: SecondFieldBG;
                spMain.Children.Add(skill);
                ++i; i %= 2;
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Brush p_colorFirstField;

        public Brush FirstFieldBG
        {
            get { return p_colorFirstField; }
            set { p_colorFirstField = value; OnPropertyChanged(); }
        }

        private Brush p_colorSecondField;

        public Brush SecondFieldBG
        {
            get { return p_colorSecondField; }
            set { p_colorSecondField = value; OnPropertyChanged(); }
        }
        private List<Tuple<string, string>> p_skills;

        public List<Tuple<string, string>> Skills
        {
            get { return p_skills; }
            set { p_skills = value; OnPropertyChanged(); }
        }

        public SkillList()
        {
            InitializeComponent();
            p_skills = new List<Tuple<string, string>>();
            PropertyChanged += OnSkillsListChanged;
        }
    }
}
