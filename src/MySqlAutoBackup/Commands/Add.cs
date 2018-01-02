using System;
using MySql.Data.MySqlClient;
using MySqlAutoBackup.Commands;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Adds the databases for a backup process
    /// </summary>
    class Add : ICommand
    {
        public string Structure
        {
            get { return "add <database:name> <interval:minutes> <zip:true/false> <password>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 4)
            {
                Utils.WriteError("> Missing param:\n> add <database:name> <interval:minutes> <ZIP:true/false> <password>");
                return;
            }

            string databaseName = param[1];
            int interval = 0;
            bool zip = true;
            string zipPassword = "";

            if (!int.TryParse(param[2], out interval) || interval < 1)
            {
                Utils.WriteError("> Invalid value: '" + param[2] + "'. Interval has to be an integer and greater than 0");
                return;
            }

            if (!bool.TryParse(param[3], out zip))
            {
                Utils.WriteError("> Invalid value: '" + param[3] + "'. ZIP has to be true or false!");
                return;
            }
            
            if (param.Length > 4)
                zipPassword = param[4];

            using (MySqlConnection conn = new MySqlConnection(Program.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", conn))
                    {
                        cmd.Parameters.AddWithValue("name", databaseName);
                        if (cmd.ExecuteScalar().ToString() == "0")
                        {
                            Utils.WriteError("> The database '" + databaseName + "' doesn't exist!");
                            return;
                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Utils.WriteError("> Error at adding database:\n" + ex.Message);
                    return;
                }
            }

            if (Utils.GetDatabase(databaseName) == null)
            {
                Program.DatabaseList.Add(new Backup.Database(databaseName, interval, zip, zipPassword));
                Utils.WriteGreen("> Added!");
                return;
            }
            Utils.WriteError("Database is already added!");
        }
    }
}
