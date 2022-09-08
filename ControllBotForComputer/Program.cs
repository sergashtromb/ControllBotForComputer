using System;
using Telegram.Bot;
using System.IO;
using Telegram.Bot.Types;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using File = System.IO.File;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyBot
{


    public class ChatBot
    {

        public string token;

        public Dictionary<string, Dictionary<string, string>> command;

    }

    internal class Program
    {
        static string fileData = File.ReadAllText("C:\\Users\\Zver\\Desktop\\data.json");
        static ChatBot chatBot = JsonConvert.DeserializeObject<ChatBot>(fileData);

        static string com = "";
        static async Task Main(string[] args)
        {

            
            var botClient = new TelegramBotClient(chatBot.token);
            
            botClient.StartReceiving(Update, Error);


            Console.ReadLine();
        }

        async private static Task Update(ITelegramBotClient client, Update update, CancellationToken arg3)
        {
            
            var message = update.Message;
            if (message != null)
            {
                if(com == "")
                {
                    if (message.Text == "Открыть папку")
                    {

                        await client.SendTextMessageAsync(message.Chat.Id, "Введите путь");
                        com = "PathOpenDir";
                        Console.WriteLine("com != \"\"");
                    }
                    else if (message.Text == "Узнать текущие процессы")
                    {

                        Process[] processes = Process.GetProcesses();
                        string listProcesses = "";
                        for (int i = 0; i < processes.Length; i++)
                        {
                            if (processes[i].MainWindowHandle != IntPtr.Zero)
                            {
                                listProcesses += "\n" + processes[i].ProcessName;
                            }

                        }
                        await client.SendTextMessageAsync(message.Chat.Id, listProcesses);
                        Console.WriteLine("OK");
                    }
                    else if (message.Text == "Узнать количество процессов")
                    {
                        Process[] processes = Process.GetProcesses();
                        await client.SendTextMessageAsync(message.Chat.Id, processes.Length.ToString());
                    }
                    else
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, message.Text);
                    }
                } 
                else if(com == "PathOpenDir")
                {
                    if(message.Text != "")
                    {
                        Console.WriteLine("Path");
                        string path = message.Text.Replace("/", "\\");
                        Console.WriteLine(path);
                        try
                        {
                            Process.Start(new ProcessStartInfo { FileName = "explorer", Arguments = $"/n, /select, {path}" });
                        }
                        catch (Exception)
                        {

                            await client.SendTextMessageAsync(message.Chat.Id, "Не удалось");
                        }
                        
                        Console.WriteLine("OK");
                        com = "";
                    }
                }
                
            }
        }

    

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
