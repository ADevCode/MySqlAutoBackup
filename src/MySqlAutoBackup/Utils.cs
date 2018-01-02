using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;
using MySqlAutoBackup.Backup;
using MySqlAutoBackup.Commands;
namespace MySqlAutoBackup
{
    class Utils
    {
        /// <summary>
        /// Returns the database object from the list
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <returns>Null, if it database hasn't been added</returns>
        public static Database GetDatabase(string name)
        {
            foreach (Database db in Program.DatabaseList)
                if (db.Name == name)
                    return db;
            return null;
        }

        /// <summary>
        /// Fills the dictionary with classes of commands
        /// </summary>
        public static void LoadCommands()
        {
            Program.Commands = new Dictionary<string, ICommand>();
            Program.Commands.Add("add", new Add());
            Program.Commands.Add("start", new Start());
            Program.Commands.Add("stop", new Stop());
            Program.Commands.Add("change", new Change());
            Program.Commands.Add("remove", new Remove());
            Program.Commands.Add("backup", new ManualBackup());
            Program.Commands.Add("databases", new ShowDatabases());
        }

        /// <summary>
        /// Reads the MySql settings from config.conf
        /// </summary>
        /// <returns>
        /// True, if connection was succesfull. 
        /// False, if error occured at reading config or establishing the connection.
        /// </returns>
        public static bool LoadMySqlSettings()
        {
            StreamReader rw;
            try
            {
                rw = new StreamReader("config.conf");
                Program.Settings = new Dictionary<string, string>();
                while (!rw.EndOfStream)
                {
                    string line = rw.ReadLine();
                    if (line.Length >= 1 && !line.StartsWith("#"))
                    {
                        int pos = line.IndexOf('=');
                        if (pos != -1)
                        {
                            string param = line.Substring(0, pos);
                            string value = line.Substring(pos + 1);
                            Program.Settings.Add(param, value);
                        }
                    }
                }

                string missingKeys = null;
                if (!Program.Settings.ContainsKey("mysql.host"))
                    missingKeys = "mysql.host; ";
                if (!Program.Settings.ContainsKey("mysql.port"))
                    missingKeys += "mysql.port; ";
                if (!Program.Settings.ContainsKey("mysql.user"))
                    missingKeys += "mysql.user; ";
                if (!Program.Settings.ContainsKey("mysql.password"))
                    missingKeys += "mysql.password; ";
                if (!Program.Settings.ContainsKey("mysql.charset"))
                    missingKeys += "mysql.charset; ";
                if (!Program.Settings.ContainsKey("mysql.convertzerodatetime"))
                    missingKeys += "mysql.convertzerodatetime; ";

                if (missingKeys != null)
                {
                    Utils.WriteError("Missing parameter(s) in config.conf: " + missingKeys);
                    return false;
                }

                bool convertZeroDateTime;
                if (!bool.TryParse(Program.Settings["mysql.convertzerodatetime"], out convertZeroDateTime))
                {
                    Utils.WriteError("Error at reading MySQL-Settings:\n" + "mysql.convertzerodatetime has to be 'true' or 'false'!");
                    Utils.WriteColor("Press 'ENTER' to retry!", ConsoleColor.Yellow);
                    return false;
                }

                Program.ConnectionString = string.Format("server={0};port={1};user={2};pwd={3};charset={4};convertzerodatetime={5};", Program.Settings["mysql.host"], Convert.ToInt32(Program.Settings["mysql.port"]), Program.Settings["mysql.user"], Program.Settings["mysql.password"], Program.Settings["mysql.charset"], Convert.ToBoolean(Program.Settings["mysql.convertzerodatetime"]));

                using (MySqlConnection conn = new MySqlConnection(Program.ConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.WriteError("Error at reading MySQL-Settings:\n" + ex.Message);
                Utils.WriteColor("Press 'ENTER' to retry!", ConsoleColor.Yellow);
                return false;
            }
        }

        /// <summary>
        /// Writes a red colored text to the console
        /// </summary>
        public static void WriteError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Writes a green colored text to the console
        /// </summary>
        public static void WriteGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Writes a text to the color with a specific color
        /// </summary>
        public static void WriteColor(string text, ConsoleColor c1)
        {
            Console.ForegroundColor = c1;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Updates the title with the amount of active backup processes
        /// </summary>
        /// <param name="interval">Interval in seconds</param>
        public static void LoadTitleUpdater(int interval)
        {
            new Thread(delegate()
            {
                while (true)
                {
                    int active = 0;
                    foreach (Database db in Program.DatabaseList)
                        if (db.ThreadStarted) active++;
                    Console.Title = "MySqlAutoBackup: " + active.ToString() + " of " + Program.DatabaseList.Count + " active";
                    Thread.Sleep(interval * 1000);
                }
            }).Start();
        }
    }
}
