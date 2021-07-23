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
- MySqlConnector.dll (provided in the releases page)

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
| ipList             | List<KeyValuePair<string,string>> | The list of IPs that the player has been using. |
| permissions        | List<string> | List of permissions that the player has. |
| regDate            | DateTime | The date that the player registered in the database. |
| player             | Player | The MiNET Player object. |
| commandContainer   | CommandSet | The list of commands that the player can access. |

### MiGroup Object
