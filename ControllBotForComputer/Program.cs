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
using Telegram.Bot.Types.InputFiles;

namespace MyBot
{


    public class ChatBot
    {

        public string token;

        public int Id;

    }

    internal class Program
    {
        static string fileData = File.ReadAllText("C:\\Users\\Zver\\Desktop\\data.json");
        static ChatBot chatBot = JsonConvert.DeserializeObject<ChatBot>(fileData);

        static string com = "";
        static string fileName = "";
        static string dir = "";         
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
                if(message.Chat.Id != chatBot.Id)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Доступ запрещен!");
                    await client.SendTextMessageAsync(chatBot.Id, "Попытка доступа!");
                    Console.WriteLine("Попытка доступа!");
                }
                else
                {                    
                    if (com == "")
                    {                        
                        if (message.Text == "Открыть папку")
                        {

                            await client.SendTextMessageAsync(message.Chat.Id, "Введите путь");
                            com = "PathOpenDir";
                            Console.WriteLine("com != \"\"");
                        }
                        else if (message.Text == "Узнать текущие процессы")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, ProccessesString());
                            await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        }
                        else if (message.Text == "Узнать количество процессов")
                        {
                            Process[] processes = Process.GetProcesses();
                            await client.SendTextMessageAsync(message.Chat.Id, processes.Length.ToString());
                            await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        }
                        else if (message.Text == "Найти файл")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Введите название файла");
                            com = "SearchFile";
                        }
                        else if (message.Text == "Остановить процесс")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, ProccessesString());
                            await client.SendTextMessageAsync(message.Chat.Id, "Укажите имя процесса");
                            com = "KillProccess";
                        } 
                        else if (message.Text == "Запустить процесс") 
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Укажи название .exe");
                            com = "StartProccess";
                        } 
                        else if (message.Text == "Загрузить файл")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Введи название файла");
                            com = "FileForLoad";
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        }
                    }
                    else if (com == "PathOpenDir")
                    {
                        if (message.Text != "")
                        {
                            dir = message.Text.Replace("/", "\\");
                            try
                            {
                                Process.Start(new ProcessStartInfo { FileName = "explorer", Arguments = $"/n, /select, {dir}" });
                            }
                            catch (Exception)
                            {

                                await client.SendTextMessageAsync(message.Chat.Id, "Не удалось");
                            }
                            com = "";
                            dir = "";
                            await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        }
                    }
                    else if (com == "SearchFile")
                    {
                        fileName = message.Text;
                        await client.SendTextMessageAsync(message.Chat.Id, "Введите примерную директорию");
                        com = "SearchDirs";
                    }
                    else if (com == "SearchDirs")
                    {
                        string[] allFiles = new string[0];
                        dir = message.Text.Replace("/", "\\");
                        InputInDirectory(Directory.GetDirectories(dir), fileName, ref allFiles);
                        string repl = "";
                        for (int i = 0; i < allFiles.Length; i++)
                        {
                            repl += allFiles[i] + "\n";
                        }
                        if (repl != "")
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, repl);
                        }
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Нет такого файла");
                        }

                        dir = "";
                        com = "";
                        fileName = "";
                        await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                    }
                    else if (com == "KillProccess")
                    {
                        string proccessName = message.Text;
                        try
                        {
                            foreach (Process proc in Process.GetProcessesByName(proccessName))
                            {
                                proc.Kill();
                                await client.SendTextMessageAsync(message.Chat.Id, "Процесс остановлен");
                            }
                        }
                        catch (Exception ex)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Не удалось завершить процесс");
                        }
                        await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        com = "";
                    } 
                    else if (com == "StartProccess")
                    {
                        string fileName = message.Text;
                        try
                        {
                            Process.Start(fileName);
                            await client.SendTextMessageAsync(message.Chat.Id, $"Процесс {fileName} запущен");
                        }
                        catch (Exception ex)
                        {

                            await client.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
                        }
                        com = "";
                    } 
                    else if (com == "LoadFile")
                    {
                        string path = message.Text.Replace("/", "\\"); 
                        if (new FileInfo(path).Length < (50*1024*1024))
                        {
                            await using Stream stream = File.OpenRead($"{path}");
                            await client.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, fileName), "Файл загружен");
                            await client.SendTextMessageAsync(message.Chat.Id, "Жду команду");
                        } 
                        else
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Файл превышает 50 MB");
                        }
                        
                        com = "";
                        fileName = "";
                    } 
                    else if (com == "FileForLoad")
                    {
                        fileName = message.Text;
                        await client.SendTextMessageAsync(message.Chat.Id, "Укажи путь нахождения файла");
                        com = "LoadFile";
                    }

                }
                
            }
        }
        public static void Insert(ref string[] arr, string elem)
        {
            string[] newArr = new string[arr.Length + 1];
            newArr[arr.Length] = elem;
            for (int i = 0; i < arr.Length; i++)
                newArr[i] = arr[i];
            arr = newArr;
        }
        public static void InputInDirectory(string[] dirs, string searchFile, ref string[] file)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                try
                {
                    FindFile(Directory.GetFiles(dirs[i]), searchFile, ref file);
                    InputInDirectory(Directory.GetDirectories(dirs[i]), searchFile, ref file);
                }
                catch (Exception)
                {}
            }
        }

        public static void FindFile(string[] arr, string fileName, ref string[] openFile)
        {
            foreach (string value in arr)
            {
                string[] arrPath = value.Split('\\');
                if (Equals(arrPath[arrPath.Length - 1], fileName))
                {
                    Insert(ref openFile, value);
                }
            }
        } 


        public static string ProccessesString()
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
            return listProcesses;
            
        }
        
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
