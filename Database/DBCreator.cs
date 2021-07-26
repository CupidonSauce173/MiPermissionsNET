using MySqlConnector;

namespace MiPermissionsNET.Database
{
    public class DBCreator
    {
        /// <summary>
        /// Classs to create the MySQL data structure.
        /// </summary>
        public DBCreator(MiPermissionsNET pl)
        {
            DataAPI dataApi = new(pl);
            MySqlConnection db = dataApi.GetDatabase();

            if (db.Ping() != true)
            {
                db.Open();
            }

            // Creating the database structure.

            MySqlCommand query = new(

                // Table "MiGroups"
                "CREATE TABLE IF NOT EXISTS MiGroups(" +
                  "id INT AUTO_INCREMENT," +
                  "group_name VARCHAR(255) NOT NULL," +
                  "permissions TEXT," +
                  "is_default BOOLEAN DEFAULT FALSE," +
                  "priority INT NOT NULL" +
                  "PRIMARY KEY (id)" +
                ") ENGINE=InnoDB DEFAULT CHARSET=UTF8MB4;" +
                // Table MiPlayers
                "CREATE TABLE IF NOT EXISTS MiPlayers(" +
                  "id INT AUTO_INCREMENT," +
                  "username VARCHAR(255) NOT NULL," +
                  "reg_date DATETIME DEFAULT CURRENT_TIMESTAMP," +
                  "permissions TEXT," +
                  "PRIMARY KEY (id)" +
                ") ENGINE=InnoDB DEFAULT CHARSET=UTF8MB4;" +
                // Table Playergroups
                "CREATE TABLE IF NOT EXISTS PlayerGroups(" +
                  "player_id INT NOT NULL," +
                  "group_id INT NOT NULL," +
                  "FOREIGN KEY (player_id) REFERENCES MiPlayers(id)," +
                  "FOREIGN KEY (group_id) REFERENCES MiGroups(id)" +
                ") ENGINE=InnoDB DEFAULT CHARSET=UTFMB4;" +
                // Table MiPlayersInfo
                "CREATE TABLE IF NOT EXISTS MiPlayersInfo(" +
                  "id INT AUTO_INCREMENT," +
                  "player_id INT NOT NULL," +
                  "ip VARCHAR(255) NOT NULL," +
                  "play_time INT DEFAULT 0," +
                  "is_banned BOOLEAN DEFAULT FALSE," +
                  "PRIMARY KEY (id)" +
                  "FOREIGN KEY (player_id) REFERENCES MiPlayers(id)" +
                ") ENGINE=InnoDB DEFAULT CHARSET=UTFMB4"
                , db);
            query.ExecuteNonQuery();
        }
    }
}