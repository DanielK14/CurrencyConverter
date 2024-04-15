This is a simple currency converter application made to practice coding in C#. For the application to work you need to make a local database and link the database by changing the connectionstring in the app.config file.
To create a local datebase you can follow the following steps:
  Open solution explorer.
  Right-click on the application name. Select Add >> New Folder
  Set folder name Database.
  Right-click on Database folder select Add >> New Item
  Select a service-based database under the data menu. Set the name of the database CurrencyConverter. Database extension is .mdf.
  Click on the add button.
  After clicking on the add button, our database is created and shown in solution explorer under the database folder.
  The database can open by double-clicking in the Server Explorer, which roughly corresponds to the SQL Server Management Studio.

As an example you can add the following values to the database to start with:
  amount  name
  1       Crowns
  3       Florens 
  2       Orens
  10      RDR Dollar
  0.0267  Imperial Credits
  0.015   PokéYen
