using System;
using System.IO;
using System.Threading;
using MySql.Data.MySqlClient;
using ICSharpCode.SharpZipLib.Zip;
namespace MySqlAutoBackup.Backup
{
    class Database
    {
        public string Name; // Name of the database
        public int Interval; // Interval in which a backup is created
        private Thread BackupThread; // The backup thread
        public bool ThreadStarted; // Thread status
        public bool Zip; // Enable zip
        public string ZipPassword; // zip password

        /// <summary>
        /// Constructor
        /// </summary>
        public Database(string name, int interval, bool zip = true, string zipPassword = "")
        {
            this.Name = name;
            this.Interval = interval;
            this.Zip = zip;
            this.ZipPassword = zipPassword;
        }

        /// <summary>
        /// Backup process
        /// </summary>
        /// <param name="manual">
        /// If manual is true, then the ZIP settings are taken from the parameters instead of the class variables.
        /// </param>
        public void Backup(bool manual = false, bool manualZip = true, string manualPassword = "")
        {
            DateTime timeNow = DateTime.Now;
            using (MySqlConnection conn = new MySqlConnection(Program.ConnectionString + "database=" + this.Name))
            {
                using (MySqlBackup mb = new MySqlBackup(new MySqlCommand("", conn)))
                {
                    if (!Directory.Exists("backups"))
                        Directory.CreateDirectory("backups");

                    string fileName = timeNow.ToString("HH-mm-ss");
                    string folder = "backups/" + this.Name + "/" + timeNow.ToString("dd-MM-yyyy");
                    if (!Directory.Exists("backups/" + this.Name))
                        Directory.CreateDirectory("backups/" + this.Name);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    try
                    {
                        conn.Open();
                        mb.ExportToFile(folder + "/" + fileName + ".sql");
                        conn.Close();

                        if ((manual == true) ? manualZip : this.Zip)
                        {
                            if (File.Exists(folder + "/" + fileName + ".sql"))
                            {
                                ZipFile zfile = ZipFile.Create(folder + "/" + fileName + ".zip");
                                zfile.Password = (manual == true) ? manualPassword : this.ZipPassword;
                                zfile.BeginUpdate();
                                zfile.Add(folder + "/" + fileName + ".sql", this.Name + ".sql");

                                zfile.CommitUpdate();
                                zfile.Close();
                                File.Delete(folder + "/" + fileName + ".sql"); // delete the unzipped file
                            }
                            else
                            {
                                Utils.WriteError("[" + this.Name + "]: File hasn't been saved!");
                            }
                        }
                        Utils.WriteGreen("[" + this.Name + "]: " + timeNow.ToString("dd.MM.yyyy - HH:mm:ss") + " BACKUP CREATED! (" + DateTime.Now.Subtract(timeNow).Seconds + "s)");
                    }
                    catch (Exception ex)
                    {
                        Utils.WriteError("[" + this.Name + "]: Error at creating backup: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Starts the thread
        /// </summary>
        public void Start()
        {
            BackupThread = new Thread(delegate() {
                while(true)
                {
                    Backup();
                    Thread.Sleep(Interval * 60000);
                }
            });
            BackupThread.Start();
            ThreadStarted = true;
        }


        /// <summary>
        /// Stops the thread
        /// </summary>
        public void Stop()
        {
            BackupThread.Abort();
            ThreadStarted = false;
        }
    }
}
