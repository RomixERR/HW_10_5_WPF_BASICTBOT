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
        public BasicTBot bot;
        PropWindow propWindow;

        public MainWindow()
        {
            InitializeComponent();
            bot = new BasicTBot(usersMessagesFileName: Settings.Default["usersMessagesFileNameTB"].ToString()) ;
            listBox.ItemsSource = bot.userMessages;
            textLog.DataContext = bot;
            propWindow = new PropWindow();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            //Debug.WriteLine(Settings.Default["tokenTB"].ToString());

            switch ((sender as MenuItem).Name)
            {
                case "props":
                    propWindow.Show();
                    break;
                case "start":
                    bot.Start(token: Settings.Default["tokenTB"].ToString() ,
                        logFileName: Settings.Default["logFileNameTB"].ToString(),
                        mWindow: this
                        );
                    break;
                case "stop":
                    bot.Stop();
                    break;
                case "loadMsg":
                    if (bot.userMessages.Count == 0)
                    {
                        ObservableCollection<UserMessage> temp;
                        if  ( (temp = bot.LoadFileMessage(bot.usersMessagesFileName)) != null)
                        {
                            bot.userMessages = temp;
                            listBox.ItemsSource = bot.userMessages;
                        };
                    }
                    break;
                case "test":
                    bot.userMessages.Add(new UserMessage
                    {
                        ChatId = 33356,
                        dateTime = DateTime.Now,
                        Message= "Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!! Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!Бык тупогуб, тупогубенький БЫЧЁК!!!!!!!!!!!",
                        UserName = "Тестестерон Карп-Моргинштернович"
                        
                    }) ;
                    break;
                case "exit":
                    Close();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null) return;
            if (String.IsNullOrEmpty(AnsTB.Text)) return;
            if ((listBox.SelectedItem as UserMessage).ChatId == 0) return;
            bot.SendMessage(AnsTB.Text, (listBox.SelectedItem as UserMessage).ChatId);
        }

        protected override void OnClosed(EventArgs e)
        {
            bot.Stop();
            propWindow.Close();
            base.OnClosed(e);
        }
    }
}
