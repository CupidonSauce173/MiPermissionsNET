using System.Threading;

using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;

using MiPermissionsNET.Objects;
using MiPermissionsNET.Database;

using MySqlConnector;

namespace MiPermissionsNET.Commands
{
    class Commands
    {
        private static MiPermissionsNET plugin;
        private static DataAPI dbApi;
        public Commands(MiPermissionsNET pl)
        {
            plugin = pl;
            dbApi = new(pl);
        }

        [Command(Name = "setgperm", Description = "To set a new permission to a group", Permission = "setgperm.MiPermissionsNET")]
        public static void SetGPerm(Player player, string groupTarget, string permission)
        {

        }

        [Command(Name = "setpperm", Description = "To set a new permission to a player", Permission = "setpperm.MiPermissionsNET")]
        public static void SetPPerm(Player player, string playerTarget, string permission)
        {

        }

        [Command(Name = "unsetgperm", Description = "To unset a permission from a group", Permission = "unsetgperm.MiPermissionsNET")]
        public static void UnsetGPerm(Player player, string groupTarget, string permission)
        {

        }

        [Command(Name = "unsetpperm", Description = "To unset a permission from a player", Permission = "unsetpperm.MiPermissionsNET")]
        public static void UnsetPPerm(Player player, string playerTarget, string permission)
        {

        }

        [Command(Name = "resetplayer", Description = "To delete everything in the database related to that player (including aliases)", Permission = "resetplayer.MiPermissionsNET")]
        public static void ResetPlayer(Player player, string playerTarget)
        {

        }

        [Command(Name = "setdefault", Description = "To set a group as default group (new players will get this group automatically)", Permission = "setdefault.MiPermissionsNET")]
        public static void SetDefault(Player player, string groupTarget)
        {

        }

        [Command(Name = "setpriority", Description = "To set the priority of a group (greater the number is, smaller the priority is, example : 1 = highest, 10 = lowest)", Permission = "setpriority.MiPermissionsNET")]
        public static void SetPriority(Player player, string groupTarget, int priority)
        {

        }

        [Command(Name = "aliases", Description = "To get the list of aliases linked to a player.", Permission = "aliases.MiPermissionsNET")]
        public static void Aliases(Player player, string playerTarget)
        {

        }
        
        [Command(Name = "pînfo", Description = "To get a list of every information related to a player.", Permission = "pinfo.MiPermissionsNET")]
        public static void PInfo(Player player, string playerTarget)
        {

        }

        [Command(Name = "addgroup", Description = "Will register a new MiGroup instance in the database and in the server.", Permission = "MiPermissionsNET.addgroup")]
        public static void AddGroup(Player player, string groupName, int priority)
        {
            if (plugin.GetAPI().GetGroupByName(groupName) != null)
            {
                player.SendMessage($"The group {groupName} already exists. Notice: The plugin lower all characters.");
                return;
            }
            player.SendMessage($"MiPermissionsNET is registering the new MiGroup named: {groupName}...");

            // Starting new thread to create the MiGroup.
            Thread createGroupThread = new(() =>
            {
                // Creating query + registering new group into MySQL.
                // Yeah I know I limit to 1 but tbh we never know what could even happen so I prefer to put some more protections.
                MySqlCommand sqlCommand = new(
                    "INSERT INTO MiGroups (group_name, priority) VALUES (@GroupName, @Priority); " +
                    "SELECT id FROM MiGroups WHERE group_name = @GroupName LIMIT 1;", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupName", groupName);
                sqlCommand.Parameters.AddWithValue("@Priority", priority);
                sqlCommand.Prepare();

                // Getting Index.
                using MySqlDataReader results = sqlCommand.ExecuteReader();

                // Creating and populating miGroup data.
                MiGroup miGroup = new();
                while (results.Read())
                {
                    miGroup.Name = groupName;
                    miGroup.Id = results.GetInt32("id");
                    miGroup.Priority = priority;
                }

                player.SendMessage($"New MiGroup: {groupName} has been registered in the database and the server. Has the ID {miGroup.Id}");
            });
            createGroupThread.Start();
        }

        [Command(Name = "rmgroup", Description = "Will disband a MiGroup from the server and the database", Permission = "MiPermissionsNET.rmgroup")]
        public static void Rmgroup(Player player, string groupTarget)
        {
            MiGroup miGroup = plugin.GetAPI().GetGroupByName(groupTarget);
            if(miGroup == null)
            {
                player.SendMessage($"Group {groupTarget} doesn't exists. Are you sure you are targetting the right MiGroup?");
                return;
            }
            if (miGroup.IsDefault)
            {
                player.SendMessage($"Can't remove group {groupTarget}, this is the default group. Please default another group first with /defaultgroup <groupName>.");
                return;
            }

            // Starting new thread to remove the group from the MySQL Server.
            Thread disbandGroupThread = new(() =>
            {
                // Removing group from MySQL.
                MySqlCommand sqlCommand = new(
                    // Deleting every rows connected to miGroup.id in PlayerGroups
                    "DELETE PlayerGroups FROM PlayerGroups " +
                      "INNER JOIN MiGroups ON MiGroups.id = PlayerGroups.group_id " +
                        "WHERE MiGroups.id = @GroupId; " +
                    // Deleting the MiGroup.
                    "DELETE FROM MiGroups WHERE id = @GroupId;", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupId", miGroup.Id);
                sqlCommand.ExecuteNonQuery();

                // Detaching it from the server.
                plugin.GetAPI().DetachGroup(miGroup);

                // Looking for every MiPlayers in the server and removing the group. If they don't have any other group, giving them the default one.
                foreach(MiPlayer player in plugin.playerData.Values)
                {
                    foreach(MiGroup group in player.MiGroups)
                    {
                        if (group.Id != miGroup.Id) continue;
                        // If player has one or less than 1 group, it will add the default group to the MiPlayer, Then remove the other group.
                        if(player.MiGroups.Count <= 1) player.MiGroups.Add(plugin.GetAPI().GetDefaultGroup());
                        player.MiGroups.Remove(group);
                        return;
                    }
                }
                // Refreshing player commandSets for all players.
                plugin.GetAPI().RefreshAllPlayerCommands();
                player.SendMessage($"MiGroup {groupTarget} has been removed.");
            });
            disbandGroupThread.Start();
        }
    }
}
