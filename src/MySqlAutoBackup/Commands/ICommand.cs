namespace MySqlAutoBackup.Commands
{
    interface ICommand
    {
        /// <summary>
        /// Structure of the command
        /// </summary>
        string Structure { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="param"></param>
        void Execute(string[] param);
    }
}
