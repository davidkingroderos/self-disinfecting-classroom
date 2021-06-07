using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Windows;
using Bogus;

namespace Self_Disinfecting_Classroom
{
    public class Program
    {
        // Initialize
        public static readonly string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static List<Card> cards;
        public static Random random = new Random();
        public static List<Room> rooms;
        public static bool isRunning = true;
        public static StringBuilder sb = new StringBuilder();
        public static bool commandStarted = false;
        public static string command;

        public static void Main(string[] args)
        {
            Console.Title = "Self Disinfecting Classroom";
            StartProgram();

            while (isRunning)
            {
                Console.Write("");
            }

            StartCommand();
        }

        
        // Start The Program Method
        public async static void StartProgram()
        {
            Console.WriteLine("Launching Self Disinfecting Classroom...");
            Thread.Sleep(2000);

            // Start

            var detectUsersOnDatabaseTask = DetectUsersOnDatabase();
            var launchClassroomsTask = LaunchClassrooms();

            cards = await detectUsersOnDatabaseTask;
            rooms = await launchClassroomsTask;

            InitializeDefaultValues();
            
            Console.WriteLine("System is ready!");
            isRunning = false;
            commandStarted = true;
        }

        public static void StartCommand()
        {
            // Write the arrow before the command
            while (commandStarted)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(">");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(">");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("~");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write("$ ");
                Console.ResetColor();
                command = Console.ReadLine().ToLower();

                ExecuteCommand(command);

                StartCommand();
            }
        }

        // Execute the command
        public static void ExecuteCommand(string command)
        {
            if (command.Length == 0)
            {
                return;
            }

            string[] commands = command.Split(' ');
            if (commands.Length == 1)
            {
                switch (commands[0])
                {
                    case "clear":
                        Console.Clear();
                        break;
                    case "help":
                        Console.WriteLine("No");
                        break;
                    default:
                        Console.WriteLine("Unknown Command.");
                        break;
                }
            }
            else if (commands.Length == 2)
            {
                switch (commands[0])
                {
                    // check {room number}
                    case "check":
                        var checkRoom = rooms.Find((x) => x.RoomNumber.Equals(commands[1], StringComparison.CurrentCultureIgnoreCase));
                        if (checkRoom == null)
                        {
                            Console.WriteLine($"Cannot find the room {commands[1].ToUpper()}");
                        }
                        else
                        {
                            Console.WriteLine(checkRoom.ToString());
                        }
                        break;
                    // disinfect {room number}
                    case "disinfect":
                        var disinfectRoom = rooms.Find((x) => x.RoomNumber.Equals(commands[1], StringComparison.CurrentCultureIgnoreCase));
                        if (disinfectRoom == null)
                        {
                            Console.WriteLine($"Cannot find the room {commands[1].ToUpper()}");
                        }
                        else
                        {
                            if (disinfectRoom.IsDisinfecting == true)
                            {
                                Console.WriteLine($"The room {disinfectRoom.RoomNumber} is already disinfecting.");
                            }
                            else
                            {
                                Console.Write($"There are users still logged in this room. Are you sure you want to disinfect room {disinfectRoom.RoomNumber}? [y/n]: ");
                                string input = Console.ReadLine().ToLower();
                                if (input == "y" || input == "yes")
                                {
                                    disinfectRoom.Disinfect();
                                }
                                else
                                {
                                    Console.WriteLine("Disinfection cancelled.");
                                }
                            }
                        }
                        break;
                    case "logout":
                        int logoutId;
                        bool parseSuccess = int.TryParse(commands[1], out logoutId);
                        var logoutCard = parseSuccess ? cards.Find((x) => x.ID == int.Parse(commands[1])) : null;
                        if (logoutCard == null)
                        {
                            Console.WriteLine($"Cannot find the user ID {commands[1]}");
                        }
                        else
                        {
                            if (logoutCard.LoggedRoom == "Outside")
                            {
                                Console.WriteLine($"The user {logoutCard.FullName} is already outside");
                            }
                            else
                            {
                                var logoutRoom = rooms.Find((x) => x.RoomNumber == logoutCard.LoggedRoom);
                                logoutCard.ManualLogout(logoutRoom);
                            }
                        }
                        break;
                    case "info":
                        int findId;
                        parseSuccess = int.TryParse(commands[1], out findId);
                        var infoCard = parseSuccess ? cards.Find((x) => x.ID == int.Parse(commands[1])) : null;
                        if (infoCard == null)
                        {
                            Console.WriteLine($"Cannot find the user ID {commands[1]}");
                        }
                        else
                        {
                            Console.WriteLine(infoCard.ToString());
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown Command.");
                        break;
                }
            }
            else if (commands.Length == 3)
            {
                switch (commands[0])
                {
                    case "login":
                        int loginId;
                        bool parseSuccess = int.TryParse(commands[1], out loginId);
                        var loginCard = parseSuccess ? cards.Find((x) => x.ID == loginId) : null;
                        if (loginCard == null)
                        {
                            Console.WriteLine($"Cannot find the user ID {commands[1]}");
                            break;
                        }
                        var findRoom = rooms.Find((x) => x.RoomNumber.Equals(commands[2], StringComparison.CurrentCultureIgnoreCase));
                        if (findRoom == null)
                        {
                            Console.WriteLine($"Cannot find the room {commands[2].ToUpper()}");
                            break;
                        }
                        loginCard.ManualLogin(findRoom);
                        break;
                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Unknown Command.");
            }
        }

        #region LaunchMethods
        // Launch All Classrooms
        public async static Task<List<Room>> LaunchClassrooms()
        {
            List<Room> roomsList = new List<Room>();
            Console.WriteLine("Launching Classrooms...");
            await Task.Delay(1000);

            for (int i = 65; i <= 66; i++)
            {
                for (int j = 1; j <= 5; j++)
                {
                    for (int x = 0; x <= 9; x++)
                    {
                        roomsList.Add(new Room(string.Format($"{(char)i}{j}0{x}")));
                        await Task.Delay(random.Next(0, 50));
                    }
                }
            }

            return roomsList;
        }

        // Detect All Users On Database
        public async static Task<List<Card>> DetectUsersOnDatabase()
        {
            List<Card> cardsList = new List<Card>();
            Console.WriteLine("Detecting Users On Database...");
            await Task.Delay(1000);

            for (int i = 1; i <= 100; i++)
            {
                cardsList.Add(new Card(i, 
                    new Bogus.DataSets.Name().FirstName(), 
                    new Bogus.DataSets.Name().FirstName(), 
                    new Bogus.Randomizer().Hexadecimal(36), 
                    random.Next(100) >= 50 ? true : false));
                await Task.Delay(random.Next(0, 50));
            }

            return cardsList;
        }
        #endregion

        public static void InitializeDefaultValues()
        {
            Dictionary<int, Room> alternateRoom = new Dictionary<int, Room>();
            alternateRoom.Add(0, rooms.Find((x) => x.RoomNumber == "A509"));
            alternateRoom.Add(1, rooms.Find((x) => x.RoomNumber == "A408"));
            alternateRoom.Add(2, rooms.Find((x) => x.RoomNumber == "B307"));
            alternateRoom.Add(3, rooms.Find((x) => x.RoomNumber == "B206"));
            for (int i = 0, j = 0; i < cards.Count; i++, j++)
            {
                cards[i].Login(alternateRoom[j % 4]);
            }
        }
    }
}
