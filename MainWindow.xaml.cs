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
            bot = new BasicTBot();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PropWindow propWindow = new PropWindow();
            switch ((sender as MenuItem).Name)
            {
                case "props":
                    propWindow.Show();
                    break;
                case "start":
                    bot.Start(token: @"E:\C#\SKILLBOXC#\HW9\tokenTestBot.txt",
                        logFileName: "LogFileBot.txt",
                        usersMessagesFileName: "000",
                        mWindow: this
                        );
                    break;
                case "stop":
                    bot.Stop();
                    break;
                case "exit":
                    break;
            }
        }
    }
}
