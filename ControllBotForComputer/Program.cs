using System;
using Telegram.Bot;
using System.IO;
using Telegram.Bot.Types;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using File = System.IO.File;

namespace MyBot
{


    public struct ChatBot
    {

        public string token;

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
                await client.SendTextMessageAsync(message.Chat.Id, "Сообщение");
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
