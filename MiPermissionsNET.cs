using MiPermissionsNET.Database;
using MiPermissionsNET.Commands;
using MiPermissionsNET.Objects;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;

using log4net;
using System.Collections.Generic;
using MiNET.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace MiPermissionsNET
{
    /// <summary>
    /// Main class of the plugin.
    /// </summary>
    [Plugin(Author = "CupidonSauce173", PluginName = "MiPermissionsNET", PluginVersion = "0.0.1", Description = "Permissions manager for OpenMiNET using MySQL as database.")]
    public class MiPermissionsNET : Plugin
    {
        private API api;
        public MiNetServer server;
        public Dictionary<string, MiPlayer> playerData;
        public Dictionary<string, MiGroup> groupData;

        // Contains the permission and the command.
        public Dictionary<string, Command> commandPermissions;

        protected override void OnEnable()
        {
            // Init variables
            server = Context.Server;
            api = new(this);
            Events events = new(this);
            playerData = new();
            groupData = new();
            commandPermissions = new();

            // Load assembly
            Assembly.LoadFrom("MySqlConnector.dll");

            // Registers Command
            server.PluginManager.LoadCommands(new Commands.Commands(this));

            // Constructing all groups.
            DataAPI dataApi = new(this);
            dataApi.ConstructGroupData();
           
            // Register events
            server.PlayerFactory.PlayerCreated += (sender, args) =>
            {
                Player player = args.Player;
                player.PlayerJoin += events.OnPlayerJoin;
                player.PlayerLeave += events.OnPlayerLeave;
            }; 

            // Generate a Command Container for each groups.
            api.GenerateCommandContainer(server);
            Console.WriteLine("MiPermissionsNET has been enabled (Console.WriteLine)");
            Debug.WriteLine("MiPermissionsNET has been enabled! (Debug.WriteLine)");
        }

        /// <summary>
        /// This will handle all the command request from the clients to see if they have it in their command container.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        [PacketHandler]
        public Packet HandleCommands(McpeCommandRequest msg, Player player)
        {
            string commandName = Regex.Split(msg.command, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(s => s.Trim('"')).ToArray()[0].Trim('/');
            if (GetAPI().GetMiPlayer(player).CommandContainer.ContainsKey(commandName.ToLower()) != true) return null;
            return msg;
        }


        /// <summary>
        /// Will create a new MiPlayer object.
        /// </summary>
        /// <param name="player"></param>
        public void CreateMiPlayer(Player player)
        {
            // Create MiPlayer object
            Thread MiPlayerThread = new(() => api.CreateMiPlayer(player, groupData));
            MiPlayerThread.Start();
        }

        public override void OnDisable()
        {
            // Stopping MySQL connection.
            Console.WriteLine("Closing MiPermissionNET MySQl Connection....");
            DataAPI dataApi = new(this);
            dataApi.GetDatabase().Close();
        }

        /// <summary>
        /// Returns the API class from MiPermissionsNET class.
        /// </summary>
        /// <returns></returns>
        public API GetAPI()
        {
            return api;
        }

        /// <summary>
        /// Will return the MiNETServer instance.
        /// </summary>
        /// <returns></returns>
        public MiNetServer GetServer()
        {
            return server;
        }
    }
}
