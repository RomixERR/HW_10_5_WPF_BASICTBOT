using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace HW_10_5_WPF_BASICTBOT
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BasicTBot bot;
        public ObservableCollection<UserMessage> userMessages = new ObservableCollection<UserMessage>();

        public MainWindow()
        {
            InitializeComponent();
            bot = new BasicTBot(@"E:\C#\SKILLBOXC#\HW9\tokenTestBot.txt", "LogFileBot.txt");
        }


        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            bot.Start("000", this);
        }

        private void StopBtnClick(object sender, RoutedEventArgs e)
        {
            bot.Stop();
        }
    }
}
