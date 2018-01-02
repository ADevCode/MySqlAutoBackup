using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Starts the backup process of the added database
    /// </summary>
    class Start : ICommand
    {
        public string Structure
        {
            get { return "start <database:name>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 2)
            {
                Utils.WriteError("> Missing param: start <database:name>");
                return;
            }

            Database db = Utils.GetDatabase(param[1]);
            if (db == null)
            {
                Utils.WriteError("> Database isn't added!");
                return;
            }

            if (!db.ThreadStarted)
            {
                db.Start();
                Utils.WriteGreen("> Started!");
            }
            else
            {
                Utils.WriteError("> The backup process is already running!");
            }
        }
    }
}
