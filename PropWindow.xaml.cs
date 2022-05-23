using HW_10_5_WPF_BASICTBOT.Properties;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace HW_10_5_WPF_BASICTBOT
{
    /// <summary>
    /// Логика взаимодействия для PropWindow.xaml
    /// </summary>
    public partial class PropWindow : Window
    {
        public PropWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            switch ((sender as Button).Name) {
                case "tokenB":
                    openFileDialog.Title = "Файл токена";
                    if ((bool)openFileDialog.ShowDialog())
                    {
                        Settings.Default["tokenTB"] = openFileDialog.FileName;
                    }
                    break;
                case "logFileNameB":
                    openFileDialog.Title = "Путь к файлу техических логов";
                    if ((bool)openFileDialog.ShowDialog())
                    {
                        Settings.Default["logFileNameTB"] = openFileDialog.FileName;
                    }
                    break;
                case "usersMessagesFileNameB":
                    openFileDialog.Title = "Путь к файлу списка сообщений";
                    if ((bool)openFileDialog.ShowDialog())
                    {
                        Settings.Default["usersMessagesFileNameTB"] = openFileDialog.FileName;
                    }
                    break;
                case "saveB":
                    Close();
                    break;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.Save();
            base.OnClosed(e);
        }

    }
}
