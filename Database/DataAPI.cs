#pragma warning disable IDE0044
using MiPermissionsNET.Objects;
using MySqlConnector;
using System.Collections.Generic;


namespace MiPermissionsNET.Database
{
    public class DataAPI
    {
        private MySqlConnection db = null;
        private API api = null;
        private MiPermissionsNET plugin;

        /// <summary>
        /// MySQL database API to get the database and do queries.
        /// </summary>
        /// <param name="pl"></param>
        public DataAPI(MiPermissionsNET pl)
        {
            if (db == null)
            {
                MySqlConnection conn = new("Server=127.0.0.1;Database=MiPermissions;uid=root;pwd=(V&432JNM(@!#UIFN9*v");
                db = conn;
                db.Open();
            }
            if (api == null) api = new(pl);
            if (plugin == null) plugin = pl;
        }

        /// <summary>
        /// Returns the database connection to make queries.
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetDatabase()
        {
            return db;
        }

        /// <summary>
        /// Will create all groups in the database + attach them to the groupData.
        /// </summary>
        public void ConstructGroupData()
        {
            using MySqlCommand query = new("SELECT id,group_name,permissions,is_default,priority FROM MiGroups", db);
            using var results = query.ExecuteReader();

            while (results.Read())
            {
                MiGroup group = new();

                string[] rawPermissions = results.IsDBNull(results.GetOrdinal("permissions")) ? null : results.GetString("permissions").Split(",");
                if (rawPermissions != null)
                {
                    List<string> permissions = new();
                    foreach (string permission in rawPermissions)
                    {
                        permissions.Add(permission);
                    }
                    group.Permissions = permissions;
                }

                group.Id = results.GetInt32("id");
                group.Name = results.GetString("group_name");
                group.IsDefault = results.GetBoolean("is_default");
                group.Priority = results.GetInt32("priority");

                plugin.groupData.Add(group.Name.ToLower(), group);
                if (group.IsDefault) api.SetDefaultGroup(group);
            }
        }
    }
}
