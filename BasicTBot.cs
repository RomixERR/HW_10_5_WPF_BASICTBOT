using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;

namespace HW_10_5_WPF_BASICTBOT
{
    internal class BasicTBot
    {
        private string token;
        private string logFileName;
        private string usersMessagesFileName;
        //h t t p s ://api.telegram.org/bot/{token}/METOD_NAME?argument1=value1&argument2=value2
        private string preRequest;
        private long update_id;
        private WebClient client;
        private bool BotRunFlag;
        private Thread thread;
        private MainWindow window;
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="token">Принемает как строку апи-токен, так и путь к файлу с токеном</param>
        /// <param name="logFileName">Путь или просто имя к лог-файлу</param>
        public BasicTBot()
        {

            BotRunFlag = false;
        }
        /// <summary>
        /// запуск бота
        /// </summary>
        /// <param name="usersMessagesFileName">Путь или просто имя к Json файлу с сообщениями</param>
        public void Start(string token, string logFileName, string usersMessagesFileName,MainWindow mWindow)
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
            //thread.Join(5000);
            //thread.Abort();
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
                    //string sendingMsg = "";
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
                    window.userMessages.Add(new UserMessage()
                   {
                       ChatId = chatId,
                       Message = text,
                       UserName = first_name,
                       dateTime = DateTime.Now
                   })
                    );
                    //sendingMsg = Otvet(text);
                    //SendMessage(sendingMsg, chatId);
                    //Log($"MSG ADD /// Count: {window.userMessages.Count}, UserName: {window.userMessages.Last().UserName}, Message: {window.userMessages.Last().Message} ");

                }
                Thread.Sleep(3000);
            }
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
                Debug.WriteLine(msg);
                Debug.WriteLine($"ERROR: {e.Message} in LOG!!! {logFileName} LOG No RECORD IN FILE!!!");
                return;
            }

            if (!fileInfo.Exists)
            {
                FileStream stream = File.Create(logFileName);
                stream.Close();
            }

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
            a = client.DownloadString(req);
            if ((bool)JObject.Parse(a)["ok"])
            {
                Log("Сообщение доставлено");
            }
        }

        //private string Otvet(string text)
        //{
        //    string sendingMsg;
        //    switch (text.ToLower())
        //    {
        //        case "/start":
        //        case "/help":
        //            sendingMsg = "Привет пользователь!\n" +
        //                "Тут такое дело.\n" +
        //                "Я супер-бот от RomixERR'а.\n" +
        //                "Напиши жора и посмотри что выйдет!\n" +
        //                "Можно написать своё имя, а я дам" +
        //                "тебе 🪐гороскоп🪐 на этот месяц!\n" +
        //                "🌖🌗🌘🌒🌓";
        //            break;
        //        case "жора":
        //            sendingMsg = "🍑 Жорой сыт не будешь!\n" +
        //                "Может лучше гороскоп 🌓? Напиши своё имя и получи персональный гороскоп от Илоны Довыдовой!";
        //            break;
        //        case "дима":
        //        case "dmitry":
        //        case "diman":
        //        case "dima":
        //        case "дмитрий":
        //        case "диман":
        //            sendingMsg = "🌜 У Дмитрия 😺 козерог в водолее а стрелец в говне.\n" +
        //                "Месяц потрясающих свершений и открытий ждёт тебя в этом месяце\n" +
        //                "Если переведёщь полташку RomixERRу на карточку.🌛";
        //            break;
        //        case "рома":
        //        case "роман":
        //        case "ромарио":
        //        case "romixerr":
        //        case "roman":
        //        case "roma":
        //            sendingMsg = "🌜 У Романа 🌚 стрелец в луне а солнце в земле.\n" +
        //                "Романа ждут великие дела по окучиванию самок на грядках бытия жизни\n" +
        //                "Конечно если он найдёт на них всех денег.🌛";
        //            break;
        //        case "витя":
        //        case "виктор":
        //        case "викторио":
        //        case "viktor":
        //        case "vita":
        //            sendingMsg = "🌜 У Виктора 😺 водолей в козероге а жигуль в гараже.\n" +
        //                "Виктора ждут успехи в творчестве и в сварочных делах\n" +
        //                "Если он закинет 50 рублей на карту Роману.🌛";
        //            break;
        //        case "максим":
        //        case "макс":
        //        case "максимушка":
        //        case "maks":
        //        case "maksim":
        //            sendingMsg = "🌜 У Максима 🦈 овен в весах а весы на луне.\n" +
        //                "Макса ожидает великий прилив сил и здоровья\n" +
        //                "Если он закинет палтиник на карту Виктору до четверга.🌛";
        //            break;
        //        case "сергей":
        //        case "серёга":
        //        case "серый":
        //        case "sergey":
        //        case "sergei":
        //            sendingMsg = "🌜 У Серого 🦈 в этом месяце козерог в овене а водолей в альфе-центавре.\n" +
        //                "Сергея ждут потрясающие события в жизни. Много алкаголя и девок\n" +
        //                "Конечно если он не забудет про 50 рублей. И кинет их Саньку на опохмел.🌛";
        //            break;
        //        case "саня":
        //        case "саша":
        //        case "санёк":
        //        case "сашка":
        //        case "александр":
        //        case "aleksandr":
        //        case "sanya":
        //            sendingMsg = "🌜 У Санька 🐵 козерог в водолее а весы в жопе.\n" +
        //                "Санька ждёт похмелье\n" +
        //                "Закиньте кто нибудь Саньку на карту палтос.🌛";
        //            break;
        //        case "рушан":
        //        case "rushan":
        //            sendingMsg = "🌜 У Рушана 🦕 козерог в льве а лев в дупле.\n" +
        //                "Рушану будет хорошо, если он поймает рыбу\n" +
        //                "Большую рыбу.🌛";
        //            break;
        //        case "домир":
        //        case "дамир":
        //        case "damir":
        //            sendingMsg = "🌜 У Дамира 🦕 козерог в носороге а стрелец в берлоге.\n" +
        //                "Дамир станет очень богатым и влиятельным\n" +
        //                "Если конечно съест Большую рыбу.🌛";
        //            break;
        //        case "юра":
        //        case "юрий":
        //        case "юрик":
        //        case "ura":
        //        case "uriy":
        //            sendingMsg = "🌜 У Юры 🐙 близнецы в водолее а козерок в пятом доме марса.\n" +
        //                "Юрий будет дуть, дуть будет Юрий\n" +
        //                "Но тольео есть купит что дуть, иначе будет пить.🌛";
        //            break;
        //        case "антон":
        //        case "антошка":
        //        case "энтони":
        //        case "anton":
        //            sendingMsg = "🌜 У Антона 🐯 марс в третьем доме водолея в близнецах стрельца.\n" +
        //                "Это означает, что Антонио крутой чел и шарит по жизни\n" +
        //                "И это хорошо.🌛";
        //            break;
        //        case "игорь":
        //        case "игорян":
        //        case "igor":
        //            sendingMsg = "🌜 У Игоря 🐯 козерок во льве.\n" +
        //                "Это означает, что Игорю скоро повезёт в лохотроне и он перехитрит систему\n" +
        //                "Скинь полташку на карту Роме и выиграй приз ААаавтомобиль!🌛";
        //            break;
        //        case "андрей":
        //        case "эндрио":
        //        case "андрюха":
        //        case "самоса":
        //        case "бригадир":
        //        case "andrey":
        //            sendingMsg = "🌜 У Андрея 🐶 луна в овне а лев в запое.\n" +
        //                "Андрей! Держи хуй бодрей!\n" +
        //                "Скинь 50 000р на карту Роме и выиграй приз ААаавтомобиль!🌛";
        //            break;
        //        case "иван":
        //        case "шеф":
        //        case "босс":
        //        case "ivan":
        //            sendingMsg = "🌜 У Ивана 🐶 козерог в стрельце а КЦ маслице.\n" +
        //                "Ивану звёзды распологаю наиболее сильным образом\n" +
        //                "Лучше время чтобы скинуть Виктору 100р на новый забор на даче!🌛";
        //            break;
        //        case "иппалит":
        //        case "павсикакий":
        //            sendingMsg = "🌜 У вас деменция.\n" +
        //                "Примите лекарства от мозгов\n" +
        //                "🌛";
        //            break;
        //        default:
        //            sendingMsg = $"Вы написали {text}\n" +
        //                "Список доступных имён:\n" +
        //                "Дима, Рома, Витя, Максим, Юра, Сергей, Саша, Рушан, Дамир, Игорь, Антон, Павсикакий, Иппалит, Андрей, Иван.";
        //            break;
        //    }
        //    return sendingMsg;
        //}

    }
}
