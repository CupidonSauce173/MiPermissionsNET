<p align="center">
  <img width="150" height="150" src="https://img.icons8.com/doodle/96/000000/security-checked--v1.png" />
</p>
<h1 align=center>MiPermissionsNET</h1>
<p align="center">MiPermissionsNET is a multi-groups permissions system using MiNET and MySQL.</p>



### Known Issues

[]

| **Feature**                 | **State** |
| -------------------------------- |:----------:|
| Join / Leave Events              | ✔️ |
| Database Structure               | ✔️ |
| Custom CommandSet                | ✔️ |
| Using MiNET Permission Attribute | ✔️ |
| Multi-Groups                     | ✔️ |
| Group Priority                   | ❌ |
| API                              | 🔨 |
| MiPlayer Object                  | ✔️ |
| MiGroup Object                   | ✔️ |
| Multi-Threading                  | ✔️ |
| Aliases                          | ❌ |
| Commands                         | 🔨 |
| Auto-DB Creator                  | ✔️ |

### Prerequisites

- Working MYSQL Server.
- MiNET Server.
- MySqlConnector.dll (provided in the releases page).

### Introduction

This is a multi-groups permissions system working with MiNET and MySQL. This is based on the PHP plugin "PurePerms" for PMMP (only for the commands). This is my first project using MiNET and C# in general. The plugin is **NOT** ready for productions and shall not be used for any kind of server until I say it. This plugin aim to make servers on MiNET easier to develop and where you can use the native MiNET CommandAttributes without having the trouble to create your own attributes and permissions system.

I want the players to be able to have multiple groups where they all have their own permissions and priority, so for example, you could have an admin with it's permissions and another rank, but the rank that will have the highest priority (in that case) could be the admin (so in chat, it would display "admin" instead of a player group).

You will also be able to set permission-per-player.

### MiPlayer Object

The MiPlayer object is where all the data related to a player is stored for this plugin.

| **Property**       | **DataType** | **Description** |
| ------------------ | :----------- | :------------- |
| id                 | int | MiPlayer ID in the database. |
| playTime           | int | How long (in seconds) the player has been playing (in total). |
| isBanned           | bool | To know if the player has been banned. |
| currentIp          | string | Current IP of the player |
| ipList             | List(KeyValuePair)string,string)) | The list of IPs that the player has been using. |
| permissions        | List(string) | List of permissions that the player has. |
| regDate            | DateTime | The date that the player registered in the database. |
| player             | Player | The MiNET Player object. |
| commandContainer   | CommandSet | The list of commands that the player can access. |

### MiGroup Object

The MiGroup object is where all the data related to a group is stored for this plugin.

| **Property**       | **DataType** | **Description** |
| ------------------ | :----------- | :------------- |
| name               | string | Name of the group. |
| id                 | int | Id of the group in the database. |
| permissions        | List(string) | List of permissions that the group has. |
| isDefault          | bool | If the group is the default one. |
| priority           | int | The priority of the group. |
| commandContainer   | Dictionary(string,Command) | List of commands that the group has. |

### Commands & Permissions

The plugin offers few commands, right now, they are all player-commands (can't be executed by the console).

```
Commands related to groups.
/addgroup <group> | Will create & register a new group in the database & server.
/rmgroup <targetGroup> | Will disband a group from the server & database.
/setpriority <targetGroup> <priority> | Will set the priority of a group.
/setdefault <targetGroup> | Will set a group as default.
/setgperm <targetGroup> <permission> | Will set a new permission for a group.
/unsetgperm <targetGroup> <permission> | Will unset a permission from a group.

Commands related to players.
/setpgroup <targetPlayer> <group> | Will give a new group to a player.
/unsetpgroup <targetPlayer> <group> | Will remove a group from a player.
/unsetpperm <targetPlayer> <permission> | Will unset a permission from a player.
/setpperm <targetPlayer> <permission> | Will set a permission to a player.
/resetplayer <targetPlayer> | Will reset all MiPermissionsNET data from a player.
/pinfo <targetPlayer> | Will display a full data sheet of a player (aliases, ips, groups, other player info too) in a message.
/aliases <targetPlayer> | Will display (without the IPs) all the aliases of the player.
```

### Plugin API

```php
# Method to generate a container for all the groups in the MySQL database.
internal void GenerateCommandContainer(MiNetServer server, bool RefreshAllPlayerCommands = false);

# Method to refresh the commandSet for a player.
internal void RefreshPlayerCommnads(Player player);

# Method to refresh the commandSet for all connected players.
internal void RefreshAllPlayerCommands();

# Method to return all registered MiGroups objects.
internal Dictionary<string, MiGroup> GetAllMiGroups();

# Method to update a MiGroup object in the MySQL server.
internal void UpdateMiGroup(MiGroup group);

# Method to update a MiPlayer object in the MySQL server.
internal void UpdateMiPlayer(MiPlayer player);

# Method to set the default MiGroup in the server.
internal void SetDefaultGroup(MiGroup group);

# Method to return the default MiGroup in the server.
internal void GetDefaultGroup();

# Method to attach a group in the groupData dictionary.
internal void AttachGroup(MiGroup group);

# Method to detach a group from the groupData dictionary.
internal void DetachGroup(MiGroup group);

# Method to return a MiGroup object by name.
internal MiGroup GetGroupByName(string name);

# Method to return a MiGroup object by id.
internal MiGroup GetGroupById(int id);

# Method to return a dictionary of MiPlayers objects.
internal Dictionary<string, MiPlayer> GetAllMiPlayers();

# Method to return a MiPlayer by username.
internal MiPlayer GetMiPlayerByName(string name);

# Method to return a MiPlayer by MiNET Player object.
internal MiPlayer getMiPlayer(Player player);

# Method to create a new MiPlayer object and attach it to the MiPlayer list.
internal void CreateMiPlayer(Player player, Dictionary<string, MiGroup> groupData)
```

### Database structure


