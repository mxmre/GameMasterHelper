using GameMasterHelper.Logic.DnD;
using GameMasterHelper.Manage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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

namespace GameMasterHelper.Pages.Creatures
{
    /// <summary>
    /// Логика взаимодействия для CreaturesPage.xaml
    /// </summary>
    /// 
    
    public partial class CreaturesPage : Page
    {
        public class ListItemCreature
        {
            public void UpdateFromDnDCreature()
            {
                Name = p_creatureRef.Name;
                Level = p_creatureRef.CreatureLevel;
                Type = p_creatureRef.CreatureType.ToString();
                Aligment = p_creatureRef.CreatureAlignment.ToString();
                Size = p_creatureRef.CreatureSize.ToString();
                CreatureChanged(this);
            }
            public ListItemCreature(Logic.DnD.DnDCreature creature)
            {
                p_creatureRef = creature;
                CreatureChanged += Def;
                UpdateFromDnDCreature();
            }
            private void Def(object sender) { }
            private Logic.DnD.DnDCreature p_creatureRef;

            public Logic.DnD.DnDCreature Creature
            {
                get { return p_creatureRef; }
                set { p_creatureRef = value; UpdateFromDnDCreature(); }
            }

            public delegate void CreatureChangedHandler(object sender);
            public event CreatureChangedHandler CreatureChanged;

            public string Name { get; private set; }
            public string Level { get; private set; }
            public string Type { get; private set; }
            public string Aligment { get; private set; }
            public string Size { get; private set; }

        }
        
        private ObservableCollection<ListItemCreature> p_observableCollectionOfCreatures;

        public ObservableCollection<ListItemCreature> CollectionOfCreatures
        {
            get { return p_observableCollectionOfCreatures; }
            set { p_observableCollectionOfCreatures = value; }
        }

        private CreaturesPageEdit p_creaturesPageEdit;
        private CreaturesPageView p_creaturesPageView;

        void SelectedNewCreature(ListItemCreature creature)
        {
            p_creaturesPageView = new CreaturesPageView(creature);
            p_creaturesPageEdit = new CreaturesPageEdit(p_creaturesPageView);
            EditorTools.Content = new EditorPages(p_creaturesPageView, p_creaturesPageEdit);
        }
        void ClearEditor()
        {
            p_creaturesPageEdit = null;
            p_creaturesPageView = null;
            EditorTools.Content = null;
        }

        public CreaturesPage()
        {
            DataContext = this;
            CollectionOfCreatures = new ObservableCollection<ListItemCreature>();
            InitializeComponent();
            CreaturesList.ItemsSource = CollectionOfCreatures;
            EditorTools.Content = null;

            //CreaturesList.Items.Add(new ListItemCreature());
        }

        private void creatureChanged(object sender)
        {
            var creature = sender as ListItemCreature;
            int index = CollectionOfCreatures.IndexOf(creature);
            CollectionOfCreatures.RemoveAt(index);
            CollectionOfCreatures.Insert(index, creature);
        }
        private void AddToList(DnDCreature creature)
        {
            var addableCreature = new ListItemCreature(creature);
            addableCreature.CreatureChanged += creatureChanged;
            CollectionOfCreatures.Add(addableCreature);
        }
        private void RawAddToList(DnDCreature creature)
        {
            AddToList(creature);
            ProgramDataBase.Creatures.Add(creature);
        }
        private void bnAddToList_Click(object sender, RoutedEventArgs e)
        {
            RawAddToList(new DnDCreature());
        }

        private void bnDelFromList_Click(object sender, RoutedEventArgs e)
        {
            if (CreaturesList.SelectedItems.Count == 0 ||
                MessageBox.Show(new StringBuilder().Append("Вы уверены, что хотите удалить ")
                .Append(CreaturesList.SelectedItems.Count.ToString())
                .Append(" элемент(ов)?").ToString(),
                "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            
            RemoveAllSelectedItemsFromList();
        }
        private void UpdateListView()
        {
            CollectionOfCreatures.Clear();
            foreach (var item in ProgramDataBase.Creatures)
            {
                AddToList(item);
            }
        }
        private void RemoveAllSelectedItemsFromList()
        {
            if (CreaturesList.SelectedItems.Count == 0)
            {
                ClearEditor();
                return;
            }
            var listItem = (ListItemCreature)CreaturesList.SelectedItems[0];
            ProgramDataBase.Creatures.Remove(listItem.Creature);
            CollectionOfCreatures.Remove(listItem);
            RemoveAllSelectedItemsFromList();
        }

        private void CreaturesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CreaturesList.SelectedItems.Count > 0) 
            {
                SelectedNewCreature(CreaturesList.SelectedItems[0] as ListItemCreature);
            }
        }

        private void bnSave_Click(object sender, RoutedEventArgs e)
        {
            ProgramDataBase.SaveCreaturesToFile(ProgramDataBase.Creatures);
        }

        private void bnOpenDefault_Click(object sender, RoutedEventArgs e)
        {
            List<DnDCreature> tmpList = new List<DnDCreature>();
            if(ProgramDataBase.LoadCreaturesFromFile(out tmpList))
            {
                ProgramDataBase.Creatures = tmpList;
                UpdateListView();
            }
            
        }
    }
}
