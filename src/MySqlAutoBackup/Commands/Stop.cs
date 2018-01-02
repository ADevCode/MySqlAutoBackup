using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Stops the backup process of the added database
    /// </summary>
    class Stop : ICommand
    {
        public string Structure
        {
            get { return "stop <database:name>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 2)
            {
                Utils.WriteError("> Missing param: stop <database:name>");
                return;
            }

            string databaseName = param[1];
            Database db = Utils.GetDatabase(databaseName);

            if (db == null)
            {
                Utils.WriteError("> Database hasn't been added!");
                return;
            }

            if (db.ThreadStarted)
            {
                db.Stop();
                Utils.WriteGreen("> Stopped!");
            }
            else
            {
                Utils.WriteError("> The backup process hasn't been started!");
            }
        }
    }
}
