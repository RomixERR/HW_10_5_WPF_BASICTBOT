using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;
using HW_10_5_WPF_BASICTBOT.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace HW_10_5_WPF_BASICTBOT
{
    public class BasicTBot : INotifyPropertyChanged
    {
        private string token;
        private string logFileName;
        public string usersMessagesFileName;
        //h t t p s ://api.telegram.org/bot/{token}/METOD_NAME?argument1=value1&argument2=value2
        public ObservableCollection<UserMessage> userMessages = new ObservableCollection<UserMessage>();
        private string preRequest;
        private long update_id;
        private WebClient client;
        private bool BotRunFlag;
        private Thread thread;
        private MainWindow window;

        public event PropertyChangedEventHandler PropertyChanged;
        private string _msgsLog;
        public string msgsLog
        {
            get { return _msgsLog; }
            set
            {
                _msgsLog += value + "\n";
                OnPropertyChanged();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="token">Принемает как строку апи-токен, так и путь к файлу с токеном</param>
        /// <param name="logFileName">Путь или просто имя к лог-файлу</param>
        public BasicTBot(string usersMessagesFileName)
        {

            BotRunFlag = false;
            this.usersMessagesFileName = usersMessagesFileName;
            Settings.Default.SettingChanging += Default_SettingChanging;
        }

        private void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            usersMessagesFileName = Settings.Default["usersMessagesFileNameTB"].ToString();
        }

        /// <summary>
        /// запуск бота
        /// </summary>
        /// <param name="usersMessagesFileName">Путь или просто имя к Json файлу с сообщениями</param>
        public void Start(string token, string logFileName, MainWindow mWindow)
        {
            if (BotRunFlag) return;
            FileInfo fileInfo;
            try {
                fileInfo = new FileInfo(token);
            }
            catch (System.NotSupportedException)
            {
                Log($"NotSupportedException {token}");
                return;
            }
            
            if (fileInfo.Exists)
            {
                this.token = File.ReadAllText(token);
            }
            else
            {
                this.token = token;
            }
            preRequest = $@"https://api.telegram.org/bot{this.token}/"; //далее METOD_NAME
            update_id = 0;
            this.logFileName = logFileName;
            BotRunFlag = true;
            client = new WebClient() { Encoding = Encoding.UTF8 };
            window = mWindow;
            thread = new Thread(Loop);
            thread.Start();
            Log($"BOT START {DateTime.Now}");
        }
        /// <summary>
        /// Остановка бота
        /// </summary>
        public void Stop()
        {
            if (!BotRunFlag) return;
            BotRunFlag = false;
            client.Dispose();
            Log($"BOT STOP {DateTime.Now}");
        }

        private void Loop()
        {
            string request, response;
            while (BotRunFlag)
            {
                request = $"{preRequest}getUpdates?offset={update_id}";
                JToken[] msgs;
                try
                {
                    response = client.DownloadString(request);
                    JObject pairs = JObject.Parse(response);
                    msgs = pairs["result"].ToArray();
                }catch(Exception e)
                {
                    Log($">>>  ERROR  >>>");
                    Log($"Exception: {e.Message}");
                    Stop();
                    return;
                }

                foreach (JToken msg in msgs)
                {
                    long userId;
                    string first_name;
                    string text;
                    long chatId;

                    update_id = (long)msg["update_id"] + 1;
                    if (msg["message"] != null)
                    {
                        chatId = (long)msg["message"]["chat"]["id"];
                        userId = (long)msg["message"]["from"]["id"];
                        first_name = msg["message"]["from"]["first_name"].ToString();
                        if (msg["message"]["text"] == null)
                        {
                            Log($"===");
                            Log(@"msg[message][text] = NULL or EMPTY");
                            continue;
                        }

                        text = msg["message"]["text"].ToString();
                        Log($"===");
                        Log($"User name: {first_name}, id: {userId}, message: {text}.");
                    }
                    else if(msg["my_chat_member"]!=null)
                    {
                        //chatId = (long)msg["my_chat_member"]["chat"]["id"];
                        userId = (long)msg["my_chat_member"]["from"]["id"];
                        first_name = msg["my_chat_member"]["from"]["first_name"].ToString();
                        text = msg["my_chat_member"]["new_chat_member"]["user"]["first_name"].ToString() + ", status: " + msg["my_chat_member"]["new_chat_member"]["status"].ToString();
                        Log($"===");
                        Log($"User name: {first_name}, id: {userId}, message: {text}.");
                        continue;
                    }
                    else
                    {
                        Log($"===");
                        Log($"Не понятный входной формат");
                        continue;
                    }

                    window.Dispatcher.Invoke(() =>
                    userMessages.Add(new UserMessage()
                   {
                       ChatId = chatId,
                       Message = text,
                       UserName = first_name,
                       dateTime = DateTime.Now
                   })
                    );
                    SaveAppendFileMessage(userMessages.Last(), usersMessagesFileName);
                }
                Thread.Sleep(3000);
            }
        }
        private void SaveAppendFileMessage(UserMessage userMessage,string fileName)
        {
            StreamWriter streamWriter = new StreamWriter(fileName, true, Encoding.UTF8);
            streamWriter.WriteLine( JsonConvert.SerializeObject(userMessage));
            streamWriter.Close();
        }


        public ObservableCollection<UserMessage> LoadFileMessage(string fileName)
        {
            if (!File.Exists(fileName)) return null;
            StreamReader streamReader = new StreamReader(fileName);
            string s;
            ObservableCollection<UserMessage> userMessages = new ObservableCollection<UserMessage>();
            while (!String.IsNullOrEmpty(s = streamReader.ReadLine()))
            {
                try
                {

                    userMessages.Add((UserMessage)JsonConvert.DeserializeObject(s, typeof(UserMessage)));
                }
                catch (Exception e)
                {
                    Log($"LoadFileMessage ERROR: {e.Message}");
                    userMessages = null;
                }
            }
            streamReader.Close();
            return userMessages;
        }


        private void Log(string msg)
        {
            FileInfo fileInfo;
            try
            {
                fileInfo = new FileInfo(logFileName);
            }
            catch (Exception e)
            {
                msgsLog = msg;
                Debug.WriteLine(msg);
                Debug.WriteLine($"ERROR: {e.Message} in LOG!!! {logFileName} LOG No RECORD IN FILE!!!");
                return;
            }

            if (!fileInfo.Exists)
            {
                FileStream stream = File.Create(logFileName);
                stream.Close();
            }
            msgsLog = msg;
            Debug.WriteLine(msg);
            File.AppendAllText(logFileName, $"{msg}\n");
        }
        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        /// <param name="msg">Текстовое сообщение</param>
        /// <param name="chatId">chatId пользователя</param>
        public void SendMessage(string msg, long chatId)
        {
            string req = $"{preRequest}sendMessage?chat_id={chatId}&text={msg}";
            Log($"Ответ {msg}");
            string a;
            if (client == null) return;
            a = client.DownloadString(req);
            if ((bool)JObject.Parse(a)["ok"])
            {
                Log("Сообщение доставлено");
                userMessages.Add(new UserMessage
                {
                    ChatId = 0,
                    Message = msg,
                    UserName = "BOT",
                    dateTime = DateTime.Now,
                    brush = Brushes.SlateBlue
                });
                SaveAppendFileMessage(userMessages.Last(), usersMessagesFileName);
            }
        }

        

    }
}
