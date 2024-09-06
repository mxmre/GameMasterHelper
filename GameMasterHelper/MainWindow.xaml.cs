using GameMasterHelper.Logic.DnD;
using GameMasterHelper.Manage;
using GameMasterHelper.Pages.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace GameMasterHelper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CreaturesPage p_creaturesPage;

        public int xAmples = 1;
        public MainWindow()
        {
            DataManager.Init();
            InitializeComponent();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            p_creaturesPage = new CreaturesPage();
            CreaturesFrame.Content = p_creaturesPage;
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            string errStr = new StringBuilder()
                .AppendLine("Handler caught : " + e.Message)
                .Append( string.Format("Runtime terminating: {0}", args.IsTerminating))
                .ToString();
            Console.WriteLine(errStr);
            MessageBox.Show(errStr, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
    }
}
