# MySqlAutoBackup
MySqlAutoBackup is a command line tool for an automated backup creation of multiple databases.  
It can save the dumped database as a plain **.sql** file or as a compressed **.zip** file with an optional password.

## Libraries
The tool is built on the **.NET Framework 4.0** and uses following libraries:
- **MySqlBackup.NET** (https://github.com/MySqlBackupNET/MySqlBackup.Net) for database dumping
- **SharpZipLib** (https://github.com/icsharpcode/SharpZipLib) for compression

## Get Started
Download the binary release of the software and extract it.  
Open the **config.conf** file and enter your MySQL settings and start the software.  
If everything was succesfull, you can use the commands listed below.

## Commands
All commands can be listed by typing **help** into the console.  
Here are all available commands:

- ### add database:name interval:minutes zip:true/false password
  This command adds the database for a backup process.  
  **database:name** is the name of the database  
  **interval:minutes** is the interval in which a backup is created  
  **zip:true/fase** is a boolean which enables the zip compression  
  **password** is the password which is used when the zip is enabled
  
- ### start database:name
  Starts the backup process if the database was added.  
  **database:name** is the name of the database  
- ### stop database:name
  Stops the backup process if the process was started.  
  **database:name** is the name of the database  
- ### change database:name variable:interval/zip/password value
  Changes a parameter of the added database.  
  **database:name** is the name of the database  
  **variable:interval/zip/password** is the parameter that has to be changed (it can be 'interval', 'zip' or 'password')  
  **value** is the new value of the parameter
- ### remove database:name
  Removes the database from the backup process.  
  The backup process stops automatically.  
  **database:name** is the name of the database that has to be removed
- ### backup database:name zip:true/false password
  Creates a manual backup independent if the database was added or not.  
  **database:name** is the name of the database    
  **zip:true/false** is a boolean which enables the zip compression  
  **password** is the password which is used when the zip is enabled 
- ### databases
  Shows all available databases of the MySQL server.  
  The name of the databases is colored in magenta if it has been added.   
  If the backup process of an added databases was started, then the database name is colored in green.
- ### clear
  Clears the console.
