using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;

using MiPermissionsNET.Database;
using MiPermissionsNET.Objects;
using MiPermissionsNET.Utils;

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
        public Dictionary<string, Command> commandPermissions; // key = command name, value = command

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
            server.PluginManager.LoadPacketHandlers(new PacketHandler(this));

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
    }
}
