using System;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySqlAutoBackup.Backup;
using MySqlAutoBackup.Commands;
namespace MySqlAutoBackup
{
    class Program
    {
        /// <summary>Connection string of the MySQL server</summary>
        public static string ConnectionString;

        /// <summary>List of added databases</summary>
        public static List<Database> DatabaseList;

        /// <summary>Settings from config.conf</summary>
        public static Dictionary<string, string> Settings;

        /// <summary>Commands of the console</summary>
        public static Dictionary<string, ICommand> Commands;

        static void Main(string[] args)
        {
            while (!Utils.LoadMySqlSettings())
            {
                Console.ReadLine();
            }

            DatabaseList = new List<Database>();
            Console.Clear();
            Console.Write("Connection to MySQL-Server...");
            Utils.WriteColor("SUCCESFULL!\n", ConsoleColor.Green);
            Utils.WriteColor("Enter 'help' to show all commands available!\n", ConsoleColor.Cyan);
            Utils.LoadCommands();
            Utils.LoadTitleUpdater(5);
            
            while (true)
            {
                string[] cmd = Console.ReadLine().Split(' ');

                if (Commands.ContainsKey(cmd[0]))
                    Commands[cmd[0]].Execute(cmd);
                else if (cmd[0] == "help")
                {
                    Utils.WriteColor("Commands:\n", ConsoleColor.Cyan);
                    foreach (ICommand command in Commands.Values)
                        Console.WriteLine("- {0}", command.Structure);
                    Console.WriteLine("- clear");
                }
                else if (cmd[0] == "clear")
                    Console.Clear();
                else
                    Utils.WriteError("Invalid command '" + cmd[0] + "'.\nType 'help' to see all commands!");

            }
        }
    }
}
