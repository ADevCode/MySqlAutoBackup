using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Creates a manual backup
    /// </summary>
    class ManualBackup : ICommand
    {
        public string Structure
        {
            get { return "backup <database:name> <zip:true/false> <password>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 3)
            {
                Utils.WriteError("> Missing param:\n> backup <database:name> <zip:true/false> <password>");
                return;
            }

            bool zip;
            if (!bool.TryParse(param[2], out zip))
            {
                Utils.WriteError("> Invalid value: '" + param[2] + "'. ZIP has to be true or false!");
                return;
            }

            string password = "";
            if (param.Length > 3)
                password = param[3];
            
            Database db = Utils.GetDatabase(param[1]);
            if (db == null)
                new Database(param[1], 0).Backup(true, zip, password);
            else
                db.Backup(true, zip, password);
        }
    }
}
