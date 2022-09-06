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
        static async Task Main(string[] args)
        {

            string fileData = File.ReadAllText("C:\\Users\\Zver\\Desktop\\data.json");

            ChatBot chatBot = JsonConvert.DeserializeObject<ChatBot>(fileData);
            var botClient = new TelegramBotClient(chatBot.token);
            
            botClient.StartReceiving(Update, Error);


            Console.ReadLine();
        }

        async private static Task Update(ITelegramBotClient client, Update update, CancellationToken arg3)
        {
            var message = update.Message;
            if (message != null)
            {
                if (message.Text == "Открыть папку")
                {
                    string path = "C:\\Users\\Zver\\Documents";
                    Process.Start(new ProcessStartInfo { FileName="explorer", Arguments= $"/n, /select, {path}" });
                    Console.WriteLine("OK");
                    
                }
                await client.SendTextMessageAsync(message.Chat.Id, "Сообщение");
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
