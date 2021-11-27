#pragma warning disable IDE0044
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiPermissionsNET.Database;
using MiPermissionsNET.Objects;
using MiPermissionsNET.Utils;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;


namespace MiPermissionsNET
{
    /// <summary>
    /// API Class to interact with the API library of the plugin.
    /// </summary>
    public class API
    {
        private MiPermissionsNET plugin;
        private MiGroup defaultGroup;

        public API(MiPermissionsNET pl)
        {
            plugin = pl;
        }

        /// <summary>
        /// Generates command container for the multi-groups system. DO NOT USE AFTER LOADING THE SERVER.
        /// If you need to regenerate the command container, please call the command this way :
        /// Thread RegenerateThread = new (() => api.GenerateCommandContainer(server, true));
        /// RegenerateThread.Start();
        /// </summary>
        /// <param name="server"></param>
        internal void GenerateCommandContainer(MiNetServer server, bool RefreashAllPlayerCommands = false)
        {
            CommandSet commands = server.PluginManager.Commands;
            foreach (object pl in server.PluginManager.Plugins)
            {
                MethodInfo[] methods = pl.GetType().GetMethods();
                foreach (MethodInfo method in methods)
                {
                    if (Attribute.GetCustomAttribute(method, typeof(CommandAttribute), false) is not CommandAttribute commandAttribute) continue;
                    if (string.IsNullOrEmpty(commandAttribute.Name.ToLower())) commandAttribute.Name = method.Name.ToLower();

                    plugin.commandPermissions.Add(commandAttribute.Permission, commands[commandAttribute.Name.ToLower()]);
                    foreach (MiGroup group in plugin.groupData.Values)
                    {
                        if (group.Permissions.Contains(commandAttribute.Permission))
                        {
                            if (commands.ContainsKey(commandAttribute.Name.ToLower()))
                                group.AddCommand(commands[commandAttribute.Name.ToLower()]);
                        }
                    }
                }
            }
            if (RefreashAllPlayerCommands)
                new Thread(new ThreadStart(RefreshAllPlayerCommands)).Start();
        }

        /// <summary>
        /// To refresh the commandSet of one player.
        /// </summary>
        /// <param name="player"></param>
        internal void RefreshPlayerCommands(Player player)
        {
            MiPlayer miPlayer = GetMiPlayer(player);
            if (miPlayer != null)
            {
                List<MiGroup> groups = miPlayer.MiGroups;
                var miPlayerCmdContainer = new Dictionary<string, Command>(); // key = name of command, value = Command.

                foreach (MiGroup group in groups)
                {
                    Dictionary<string, Command> groupCmdSet = group.CommandContainer;
                    miPlayerCmdContainer.MergeCommandContainers(groupCmdSet);
                }

                foreach (string perm in miPlayer.Permissions)
                {
                    if (plugin.commandPermissions.ContainsKey(perm))
                    {
                        if (miPlayerCmdContainer.ContainsKey(plugin.commandPermissions[perm].Name)) continue;
                        miPlayerCmdContainer.Add(plugin.commandPermissions[perm].Name, plugin.commandPermissions[perm]);
                    }
                }

                McpeAvailableCommands commandList = McpeAvailableCommands.CreateObject();
                CommandSet CommandSet = new();
                foreach (Command cmd in miPlayerCmdContainer.Values) CommandSet.Add(cmd.Name, cmd);
                commandList.CommandSet = CommandSet;
                miPlayer.CommandContainer = CommandSet;
                miPlayer.Player.SendPacket(commandList);
            }
        }

        /// <summary>
        /// Will refresh the commands for all the connected players.
        /// Needs to be moved to another thread.
        /// </summary>
        internal void RefreshAllPlayerCommands()
        {
            foreach (MiPlayer miPlayer in plugin.playerData.Values)
            {
                List<MiGroup> groups = miPlayer.MiGroups;
                var miPlayerCmdContainer = new Dictionary<string, Command>(); // key = name of command, value = Command.

                foreach (MiGroup group in groups)
                {
                    Dictionary<string, Command> groupCmdSet = group.CommandContainer;
                    miPlayerCmdContainer.MergeCommandContainers(groupCmdSet);
                }

                foreach (string perm in miPlayer.Permissions)
                {
                    if (plugin.commandPermissions.ContainsKey(perm))
                    {
                        if (miPlayerCmdContainer.ContainsKey(plugin.commandPermissions[perm].Name)) continue;
                        miPlayerCmdContainer.Add(plugin.commandPermissions[perm].Name, plugin.commandPermissions[perm]);
                    }
                }

                McpeAvailableCommands commandList = McpeAvailableCommands.CreateObject();
                CommandSet CommandSet = new();
                foreach (Command cmd in miPlayerCmdContainer.Values) CommandSet.Add(cmd.Name, cmd);
                commandList.CommandSet = CommandSet;
                miPlayer.CommandContainer = CommandSet;
                miPlayer.Player.SendPacket(commandList);
            }
        }

        internal Dictionary<string, MiGroup> GetAllMiGroups()
        {
            return plugin.groupData;
        }

        /// <summary>
        /// Will update the MiGroup data in the MySQL server.
        /// </summary>
        /// <param name="group"></param>
        internal void UpdateMiGroup(MiGroup group)
        {
            // Prepare data for MySQL.
            string permissions = string.Join(",", group.Permissions);
            // Send data to MySQL.
            DataAPI db = new(plugin);
            MySqlCommand query = new("UPDATE MiGroups SET group_name=@Name,permissions=@Permissions,is_default=@IsDefault,priority=@Priority", db.GetDatabase());
            query.Parameters.AddWithValue("@Name", group.Name);
            query.Parameters.AddWithValue("@Permissions", permissions);
            query.Parameters.AddWithValue("@IsDefault", group.IsDefault);
            query.Parameters.AddWithValue("@Priority", group.Priority);
            query.Prepare();
            query.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// To update the MiPlayer object in the database.
        /// </summary>
        /// <param name="player"></param>
        internal void UpdateMiPlayer(MiPlayer player)
        {
            // Send datat to MYSQL.
            DataAPI db = new(plugin);
            MySqlCommand query = new(
                "UPDATE MiPlayers" +
                  "INNER JOIN MiPlayersInfo" +
                    "ON MiPlayers.id=MiPlayersInfo.player_id" +
                  "play_time=@PlayTime,is_banned=@IsBanned" +
                "WHERE usernam=@Username", db.GetDatabase());

            // Add all the paramters to the query.
            query.Parameters.AddWithValue("@Username", player.Player.Username);
            query.Parameters.AddWithValue("@PlayTime", player.PlayTime);
            query.Parameters.AddWithValue("@IsBanned", player.IsBanned);
            query.Prepare();
            query.ExecuteNonQuery();
        }

        /// <summary>
        /// To set the default group in the server.
        /// </summary>
        /// <param name="group"></param>
        internal void SetDefaultGroup(MiGroup group)
        {
            defaultGroup = group;
        }

        /// <summary>
        /// To get the default group in the serevr.
        /// </summary>
        /// <returns></returns>
        internal MiGroup GetDefaultGroup()
        {
            return defaultGroup;
        }

        // MiGroup API section.

        /// <summary>
        /// Will attach a group in the groupData dictionary.
        /// </summary>
        /// <param name="group"></param>
        internal void AttachGroup(MiGroup group)
        {
            plugin.groupData.Add(group.Name.ToLower(), group);
        }

        /// <summary>
        /// Will detach a group from the groupData dictionary.
        /// </summary>
        /// <param name="group"></param>
        internal void DetachGroup(MiGroup group)
        {
            if (!plugin.groupData.ContainsKey(group.Name.ToLower())) return;
            plugin.groupData.Remove(group.Name.ToLower());
        }

        /// <summary>
        /// To get a group by group name. Example VIP.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MiGroup GetGroupByName(string name)
        {
            if (plugin.groupData.ContainsKey(name.ToLower()))
            {
                return plugin.groupData[name.ToLower()];
            }
            return null;
        }

        /// <summary>
        /// Will get a group by ID. example 1.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal MiGroup GetGroupById(int id)
        {
            foreach (KeyValuePair<string, MiGroup> entry in GetAllMiGroups())
            {
                if (entry.Value.Id == id) return entry.Value;
            }
            throw new Exception($"Couldn't find any group with the ID: {id}, maybe some groups are missing?");
        }

        // MiPlayer API section.

        /// <summary>
        /// To get all the MiPlayers registered in the server.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, MiPlayer> GetAllMiPlayers()
        {
            return plugin.playerData;
        }

        /// <summary>
        /// Will get a MiPlayer by name. Example 'CupidonSauce173'.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MiPlayer GetMiPlayerByName(string name)
        {
            if (plugin.playerData.ContainsKey(name))
            {
                return plugin.playerData[name];
            }
            return null;
        }

        /// <summary>
        /// Will get a MiPlayer object from an Player object.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal MiPlayer GetMiPlayer(Player player)
        {
            if (plugin.playerData.ContainsKey(player.Username))
            {
                return plugin.playerData[player.Username];
            }
            return null;
        }

        /// <summary>
        /// Creating a new MiPlayer Entity and attaching it to the MiPlayer list.
        /// </summary>
        /// <param name="player"></param>
        internal void CreateMiPlayer(Player player, Dictionary<string, MiGroup> groupData)
        {
            DataAPI dataApi = new(plugin);
            MySqlCommand userQuery = new(
                "SELECT MiPlayers.id,username,reg_date,play_time,is_banned FROM MiPlayers " +
                  "INNER JOIN MiPlayersInfo " +
                    "ON MiPlayers.id=MiPlayersInfo.player_id " +
                "WHERE username=@UserName", dataApi.GetDatabase());
            MySqlCommand aliasesQuery = new(
                "SELECT MiPlayers.id,username,ip FROM MiPlayers " +
                  "INNER JOIN MiPlayersInfo ON MiPlayers.id=MiPlayersInfo.player_id " +
                "WHERE ip IN (" +
                  "SELECT ip FROM MiPlayers " +
                  "INNER JOIN MiPlayersInfo " +
                    "ON MiPlayers.id=MiPlayersInfo.player_id " +
                "WHERE username=@Username)", dataApi.GetDatabase());
            MySqlCommand groupsQuery = new(
                "SELECT group_id FROM PlayerGroups " +
                  "INNER JOIN MiPlayers " +
                    "ON PlayerGroups.player_id=MiPlayers.id " +
                "WHERE player_id=@Id", dataApi.GetDatabase());
            userQuery.Parameters.AddWithValue("@UserName", player.Username);
            aliasesQuery.Parameters.AddWithValue("@Username", player.Username);

            userQuery.Prepare();
            aliasesQuery.Prepare();

            using MySqlDataReader results = userQuery.ExecuteReader();
            int playTime = 0;
            bool set = false;
            MiPlayer MiPlayer = new();
            while (results.Read())
            {
                playTime += results.GetInt32("play_time");
                if (!set)
                {
                    set = true;
                    MiPlayer.Player = player;
                    MiPlayer.Id = results.GetInt32("id");
                    MiPlayer.IsBanned = results.GetBoolean("is_banned");
                    MiPlayer.RegDate = results.GetDateTime("reg_date");
                }
            }
            MiPlayer.PlayTime = playTime;
            results.Close();

            using MySqlDataReader aliasesResults = aliasesQuery.ExecuteReader();
            List<KeyValuePair<string, string>> ipList = new();
            while (aliasesResults.Read())
            {
                KeyValuePair<string, string> ip = new(aliasesResults.GetString("username"), aliasesResults.GetString("ip"));
                ipList.Add(ip);
            }
            MiPlayer.IpList = ipList;
            aliasesResults.Close();

            groupsQuery.Parameters.AddWithValue("@Id", MiPlayer.Id);
            groupsQuery.Prepare();
            using MySqlDataReader groupsResults = groupsQuery.ExecuteReader();

            List<MiGroup> groupList = new();
            while (groupsResults.Read())
            {
                foreach (MiGroup g in groupData.Values)
                {
                    if (g.Id == groupsResults.GetInt32("group_id")) groupList.Add(g);
                }
            }
            int highestPriority = groupList.Min(group => group.Priority);
            MiGroup frontGroup = groupList.First(group => group.Priority == highestPriority);

            MiPlayer.MiGroups = groupList;
            MiPlayer.FrontGroup = frontGroup;
            groupsResults.Close();

            plugin.playerData.Add(player.Username, MiPlayer);
            RefreshPlayerCommands(player);
        }
    }
}