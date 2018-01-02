using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Removes an added database from the list
    /// </summary>
    class Remove : ICommand
    {
        public string Structure
        {
            get { return "remove <database:name>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 2)
            {
                Utils.WriteError("> Missing param:\n> remove <database:name>");
                return;
            }

            Database db = Utils.GetDatabase(param[1]);
            if (db == null)
            {
                Utils.WriteError("> Database hasn't been added.");
                return;
            }

            if (db.ThreadStarted)
                db.Stop();

            Program.DatabaseList.Remove(db);
            Utils.WriteGreen("> Database removed from the list!");
        }
    }
}
