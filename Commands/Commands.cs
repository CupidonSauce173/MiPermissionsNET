using System.Threading;
using System.Collections.Generic;
using System.Linq;

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

        // Todo

        /**
         * /resetplayer
         * /pinfo
         * /aliases
         **/

        // Not implemented commands.

        [Command(Name = "aliases", Description = "To get the list of aliases linked to a player.", Permission = "aliases.MiPermissionsNET")]
        public static void Aliases(Player player, string playerTarget)
        {
            player.SendMessage("Will be implemented in another update.");
        }

        [Command(Name = "pinfo", Description = "To get a list of every information related to a player.", Permission = "pinfo.MiPermissionsNET")]
        public static void PInfo(Player player, string playerTarget)
        {
            player.SendMessage("Will be implemented in another update.");
        }

        [Command(Name = "resetplayer", Description = "To delete everything in the database related to that player (including aliases)", Permission = "resetplayer.MiPermissionsNET")]
        public static void ResetPlayer(Player player, string playerTarget)
        {
            player.SendMessage("Will be implemented in another update.");
        }

        [Command(Name = "setpperm", Description = "To set a new permission to a player", Permission = "setpperm.MiPermissionsNET")]
        public static void SetPPerm(Player player, string playerTarget, string permission)
        {
            MiPlayer miPlayer = plugin.GetAPI().GetMiPlayerByName(playerTarget);
            if (miPlayer != null)
            {
                if (miPlayer.Permissions.Contains(permission))
                {
                    player.SendMessage($"{playerTarget} already have the permission {permission}!");
                    return;
                }
                else miPlayer.Permissions.Add(permission);
            }

            Thread MySqlThread = new(() =>
            {
                if (miPlayer != null)
                {
                    string permString = string.Join(',', miPlayer.Permissions);
                    MySqlCommand sqlCommand = new("UPDATE FROM MiPlayers permissions=@Permissions WHERE id=@PlayerId LIMIT 1;", dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@Permissions", permString);
                    sqlCommand.Parameters.AddWithValue("@PlayerId", miPlayer.Id);
                    sqlCommand.Prepare();
                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand sqlCommand = new("SELECT permisisons FROM MiPlayers WHERE username=@TargetUser LIMIT 1;", dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@TargetUser", playerTarget);
                    sqlCommand.Prepare();

                    using MySqlDataReader results = sqlCommand.ExecuteReader();
                    while (results.Read())
                    {
                        List<string> permList = results.GetString("permissions").Split(',').ToList();
                        permList.Add(permission);
                        MySqlCommand updateCommand = new("UPDATE FROM MiPlayers permissions=@Permissions WHERE username=@TargetUser", dbApi.GetDatabase());
                        updateCommand.Parameters.AddWithValue("@Permissions", string.Join(',', permList));
                        updateCommand.Parameters.AddWithValue("@TargetUser", playerTarget);
                        updateCommand.Prepare();
                        updateCommand.ExecuteNonQuery();
                    }
                    results.Close();
                }
                player.SendMessage($"You gave the permission {permission} to the player {playerTarget} with success!");
            });
            MySqlThread.Start();
        }

        [Command(Name = "unsetpperm", Description = "To unset a permission from a player", Permission = "unsetpperm.MiPermissionsNET")]
        public static void UnsetPPerm(Player player, string playerTarget, string permission)
        {
            MiPlayer miPlayer = plugin.GetAPI().GetMiPlayerByName(playerTarget);
            if (miPlayer != null)
            {
                if(miPlayer.Permissions.Contains(permission) != true)
                {
                    player.SendMessage($"{playerTarget} doesn't have the permission {permission}");
                    return;
                }
                else miPlayer.Permissions.Remove(permission);
            }

            Thread MySqlThread = new(() =>
            {
                if(miPlayer != null)
                {
                    string permString = string.Join(',', miPlayer.Permissions);
                    MySqlCommand sqlCommand = new("UPDATE FROM MiPlayers permissions=@Permissions WHERE id=@PlayerId LIMIT 1;", dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@Permissions", permString);
                    sqlCommand.Parameters.AddWithValue("@PlayerId", miPlayer.Id);
                    sqlCommand.Prepare();
                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand sqlCommand = new("SELECT permisisons FROM MiPlayers WHERE username=@TargetUser LIMIT 1;", dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@TargetUser", playerTarget);
                    sqlCommand.Prepare();

                    using MySqlDataReader results = sqlCommand.ExecuteReader();
                    while (results.Read())
                    {
                        List<string> permList = results.GetString("permissions").Split(',').ToList();
                        permList.Remove(permission);
                        MySqlCommand updateCommand = new("UPDATE FROM MiPlayers permissions=@Permissions WHERE username=@TargetUser", dbApi.GetDatabase());
                        updateCommand.Parameters.AddWithValue("@Permissions", string.Join(',', permList));
                        updateCommand.Parameters.AddWithValue("@TargetUser", playerTarget);
                        updateCommand.Prepare();
                        updateCommand.ExecuteNonQuery();
                    }
                    results.Close();
                }
                player.SendMessage($"You removed the permission {permission} from the player {playerTarget} with success!");
            });
            MySqlThread.Start();
        }

        // Implemented commands.

        [Command(Name = "unsetgperm", Description = "To unset a permission from a group", Permission = "unsetgperm.MiPermissionsNET")]
        public static void UnsetGPerm(Player player, string groupTarget, string permission)
        {
            MiGroup group = plugin.GetAPI().GetGroupByName(groupTarget);
            if (group == null)
            {
                player.SendMessage($"Group {groupTarget} is not found! Are you sure you target the right group?");
                return;
            }

            if (group.Permissions.Contains(permission) != true)
            {
                player.SendMessage($"{groupTarget} doesn't have the permission {permission}");
                return;
            }

            Thread MySqlThread = new(() =>
            {
                group.Permissions.Remove(permission);
                string permString = string.Join(',', group.Permissions);
                MySqlCommand sqlCommand = new("UPDATE MiGroups permissions=@Permissions WHERE id=@GroupId", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupId", group.Id);
                sqlCommand.Parameters.AddWithValue("@Permissions", permString);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
                player.SendMessage($"The permisison {permission} has been removed with success from the group {group.Name}!");
            });
            MySqlThread.Start();
        }

        [Command(Name = "setgperm", Description = "To set a new permission to a group", Permission = "setgperm.MiPermissionsNET")]
        public static void SetGPerm(Player player, string groupTarget, string permission)
        {
            MiGroup group = plugin.GetAPI().GetGroupByName(groupTarget);
            if (group == null)
            {
                player.SendMessage($"Group {groupTarget} is not found! Are you sure you target the right group?");
                return;
            }

            if (group.Permissions.Contains(permission))
            {
                player.SendMessage($"{groupTarget} already has the permission {permission}");
                return;
            }

            Thread MySqlThread = new(() =>
            {
                group.Permissions.Add(permission);
                string permString = string.Join(",", group.Permissions);
                MySqlCommand sqlCommand = new("UPDATE MiGroups permissions=@Permissions WHERE id=@GroupId", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupId", group.Id);
                sqlCommand.Parameters.AddWithValue("@Permissions", permString);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
                player.SendMessage($"The permisison {permission} has been added with success to the group {group.Name}!");
            });
            MySqlThread.Start();
        }

        [Command(Name = "setpgroup", Description = "Add a group to a player", Permission = "setpgroup.MiPermissionsNET")]
        public static void SetPGroup(Player player, string playerTarget, string groupTarget)
        {
            MiGroup group = plugin.GetAPI().GetGroupByName(groupTarget);

            if (group == null)
            {
                player.SendMessage($"Group {groupTarget} is not found! Are you sure you target the right group?");
                return;
            }

            Thread MySqlThread = new(() =>
            {
                MiPlayer miPlayer = plugin.GetAPI().GetMiPlayerByName(playerTarget);
                if(miPlayer == null)
                {
                    MySqlCommand sqlCommand = new(
                        "INSERT INTO PlayerGroups (player_id,group_id) " +
                        "SELECT (SELECT id FROM MiPlayers WHERE username=@PlayerTarget),@GroupId " +
                        "WHERE NOT EXISTS (SELECT player_id,group_id FROM PlayerGroups WHERE player_id=" +
                        "(SELECT id FROM MiPlayers WHERE username=@PlayerTarget) AND group_id=@GroupId) LIMIT 1;", dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@PlayerTarget", playerTarget);
                    sqlCommand.Parameters.AddWithValue("@GroupId", group.Id);
                    sqlCommand.Prepare();
                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand sqlCommand = new(
                        "INSERT INTO PlayerGroups (player_id,group_id) " +
                        "SELECT @PlayerId,@GroupId WHERE NOT EXISTS " +
                        "(SELECT player_id,group_id FROM PlayerGroups WHERE player_id=@PlayerId AND group_id = @GroupId) LIMIT 1; ",
                        dbApi.GetDatabase());
                    sqlCommand.Parameters.AddWithValue("@PlayerId", miPlayer.Id);
                    sqlCommand.Parameters.AddWithValue("@GroupId", group.Id);
                    sqlCommand.Prepare();
                    sqlCommand.ExecuteNonQuery();
                    miPlayer.MiGroups.Add(group);
                }
                player.SendMessage($"Added {groupTarget} to {playerTarget} successfully!");
            });
            MySqlThread.Start();
        }

        [Command(Name = "setdefault", Description = "To set a group as default group (new players will get this group automatically)", Permission = "setdefault.MiPermissionsNET")]
        public static void SetDefault(Player player, string groupTarget)
        {
            MiGroup group = plugin.GetAPI().GetGroupByName(groupTarget);

            if (group == null)
            {
                player.SendMessage($"Group {groupTarget} is not found! Are you sure you target the right group?");
                return;
            }
            if (group.IsDefault)
            {
                player.SendMessage($"Group {groupTarget} is already the default group!");
                return;
            }

            Thread MySqlThread = new(() =>
            {
                MySqlCommand sqlCommand = new(
                    "UPDATE MiGroups SET is_default = true WHERE id = @NewGroupId; " +
                    "UPDATE MiGroups SET is_default = false WHERE id = @OldGroupId; ",
                    dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@NewGroupId", group.Id);
                sqlCommand.Parameters.AddWithValue("@OldGroupId", plugin.GetAPI().GetDefaultGroup().Id);
                sqlCommand.ExecuteNonQuery();
                plugin.GetAPI().SetDefaultGroup(group);

                player.SendMessage($"{group.Name} has been set as new default group with success!");
            });
            MySqlThread.Start();
        }

        [Command(Name = "setpriority", Description = "To set the priority of a group (greater the number is, smaller the priority is, example : 1 = highest, 10 = lowest)", Permission = "setpriority.MiPermissionsNET")]
        public static void SetPriority(Player player, string groupTarget, int priority)
        {
            MiGroup group = plugin.GetAPI().GetGroupByName(groupTarget);
            if (group == null)
            {
                player.SendMessage($"Group {groupTarget} is not found! Are you sure you target the right group?");
                return;
            }
            if (group.Priority == priority)
            {
                player.SendMessage($"The priority of {groupTarget} is already {priority}!");
                return;
            }
            Thread MySqlThread = new(() =>
            {
                MySqlCommand sqlCommand = new("UPDATE MiGroups SET priority = @Priority WHERE id = @GroupId", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@Priority", priority);
                sqlCommand.Parameters.AddWithValue("@GroupId", group.Id);
                sqlCommand.ExecuteNonQuery();
                group.Priority = priority;

                player.SendMessage($"The priority of {groupTarget} has been updated to {priority} with success!");
            });
            MySqlThread.Start();
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

            Thread createGroupThread = new(() =>
            {
                MySqlCommand sqlCommand = new(
                    "INSERT INTO MiGroups (group_name, priority) VALUES (@GroupName, @Priority); " +
                    "SELECT id FROM MiGroups WHERE group_name = @GroupName LIMIT 1;", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupName", groupName);
                sqlCommand.Parameters.AddWithValue("@Priority", priority);
                sqlCommand.Prepare();

                using MySqlDataReader results = sqlCommand.ExecuteReader();
                MiGroup miGroup = new();
                while (results.Read())
                {
                    miGroup.Name = groupName;
                    miGroup.Id = results.GetInt32("id");
                    miGroup.Priority = priority;
                }
                results.Close();

                player.SendMessage($"New MiGroup: {groupName} has been registered in the database and the server. Has the ID {miGroup.Id}");
            });
            createGroupThread.Start();
        }

        [Command(Name = "rmgroup", Description = "Will disband a MiGroup from the server and the database", Permission = "MiPermissionsNET.rmgroup")]
        public static void Rmgroup(Player player, string groupTarget)
        {
            MiGroup miGroup = plugin.GetAPI().GetGroupByName(groupTarget);
            if (miGroup == null)
            {
                player.SendMessage($"Group {groupTarget} doesn't exists. Are you sure you are targetting the right MiGroup?");
                return;
            }
            if (miGroup.IsDefault)
            {
                player.SendMessage($"Can't remove group {groupTarget}, this is the default group. Please default another group first with /defaultgroup <groupName>.");
                return;
            }

            Thread disbandGroupThread = new(() =>
            {
                MySqlCommand sqlCommand = new(
                    "DELETE PlayerGroups FROM PlayerGroups " +
                      "INNER JOIN MiGroups ON MiGroups.id = PlayerGroups.group_id " +
                        "WHERE MiGroups.id = @GroupId; " +
                    "DELETE FROM MiGroups WHERE id = @GroupId;", dbApi.GetDatabase());
                sqlCommand.Parameters.AddWithValue("@GroupId", miGroup.Id);
                sqlCommand.ExecuteNonQuery();
                plugin.GetAPI().DetachGroup(miGroup);

                foreach (MiPlayer player in plugin.playerData.Values)
                {
                    foreach (MiGroup group in player.MiGroups)
                    {
                        if (group.Id != miGroup.Id) continue;
                        if (player.MiGroups.Count <= 1) player.MiGroups.Add(plugin.GetAPI().GetDefaultGroup());
                        player.MiGroups.Remove(group);
                        return;
                    }
                }
                plugin.GetAPI().RefreshAllPlayerCommands();
                player.SendMessage($"MiGroup {groupTarget} has been removed.");
            });
            disbandGroupThread.Start();
        }
    }
}