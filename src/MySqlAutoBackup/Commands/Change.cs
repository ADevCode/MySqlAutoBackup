using MySqlAutoBackup.Backup;
namespace MySqlAutoBackup.Commands
{
    /// <summary>
    /// Changes a parameter of an added database
    /// </summary>
    class Change : ICommand
    {
        public string Structure
        {
            get { return "change <database:name> <variable:interval/zip/password> <value>"; }
        }

        public void Execute(string[] param)
        {
            if (param.Length < 3)
            {
                Utils.WriteError("> Missing param:\n> change <database:name> <variable> <value>");
                return;
            }

            Database db = Utils.GetDatabase(param[1]);
            if (db == null)
            {
                Utils.WriteError("> Database hasn't been added!");
                return;
            }

            string variable = param[2].ToLower();
            string value = "";

            if (param.Length > 3)
                value = param[3];

            if (variable == "interval")
            {
                int interval;
                if (!int.TryParse(value, out interval) || interval < 1)
                {
                    Utils.WriteError("> Invalid value: '" + value + "'. Interval has to be numeric and greater than 0");
                    return;
                }
                db.Interval = interval;
                Utils.WriteGreen("> Changed interval!");
            }
            else if (variable == "zip")
            {
                bool zip;
                if (!bool.TryParse(value, out zip))
                {
                    Utils.WriteError("> Invalid value: '" + value + "'. ZIP has to be true or false!");
                    return;
                }
                db.Zip = zip;
                Utils.WriteGreen("> Changed zip!");
            }
            else if (variable == "password")
            {
                db.ZipPassword = value;
                Utils.WriteGreen("> Changed password!");
            }
        }
    }
}
