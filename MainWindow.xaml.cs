using HW_10_5_WPF_BASICTBOT.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            listBox.ItemsSource = userMessages;
            


        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PropWindow propWindow = new PropWindow();

            //Debug.WriteLine(Settings.Default["tokenTB"].ToString());

            switch ((sender as MenuItem).Name)
            {
                case "props":
                    propWindow.Show();
                    break;
                case "start":
                    bot.Start(token: Settings.Default["tokenTB"].ToString() ,
                        logFileName: Settings.Default["logFileNameTB"].ToString(),
                        usersMessagesFileName: Settings.Default["usersMessagesFileNameTB"].ToString(),
                        mWindow: this
                        );
                    break;
                case "stop":
                    bot.Stop();
                    break;
                case "test":
                    userMessages.Add(new UserMessage
                    {
                        ChatId = 33356,
                        dateTime = DateTime.Now,
                        Message= "Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!! Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!",
                        UserName = "Тестестерон Карп-Моргинштернович"
                        
                    }) ;
                    break;
                case "exit":
                    break;
            }
        }
    }
}
