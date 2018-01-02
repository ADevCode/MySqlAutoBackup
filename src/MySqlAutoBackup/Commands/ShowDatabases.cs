using System;
using System.Data;
using MySql.Data.MySqlClient;
using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Prints all available database of the MySQL server
    /// </summary>
    class ShowDatabases : ICommand
    {
        public string Structure
        {
            get { return "databases"; }
        }

        public void Execute(string[] param)
        {
            using (MySqlConnection conn = new MySqlConnection(Program.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(new MySqlCommand("SHOW DATABASES", conn)))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        foreach (DataRow row in table.Rows)
                        {
                            Database db = Utils.GetDatabase(row[0].ToString());
                            if (db == null)
                                Console.WriteLine("> " + row[0]);
                            else if (db.ThreadStarted)
                                Utils.WriteColor("> " + db.Name + "\n", ConsoleColor.Green);
                            else
                                Utils.WriteColor("> " + db.Name + "\n", ConsoleColor.Magenta);

                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Utils.WriteError("> Error at showing databases:\n> " + ex.Message);
                }
            }
        }
    }
}
