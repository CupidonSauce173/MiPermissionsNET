#pragma warning disable IDE0044

using MiNET;
using MiNET.Worlds;


using System;

namespace MiPermissionsNET
{
    /// <summary>
    /// Events class to handle all the player events.
    /// </summary>
    class Events
    {
        private API api;
        private MiPermissionsNET plugin;

        public Events(MiPermissionsNET pl)
        {
            api = new(plugin);
            plugin = pl;
        }

        /// <summary>
        /// Player join event (will create MiPlayer object and send welcome message to the player).
        /// </summary>
        /// <param name="o"></param>
        /// <param name="eventArgs"></param>
        public void OnPlayerJoin(object o, PlayerEventArgs eventArgs)
        {
            Level level = eventArgs.Level;
            if (level == null) throw new ArgumentNullException(nameof(eventArgs.Level));

            Player player = eventArgs.Player;
            if (player == null) throw new ArgumentNullException(nameof(eventArgs.Player));

            player.SendMessage("Welcome to MiNET server!");

            plugin.CreateMiPlayer(player);
        }

        public void OnPlayerLeave(object o, PlayerEventArgs eventArgs)
        {
            Player player = eventArgs.Player;
            if (player == null) throw new ArgumentNullException(nameof(eventArgs.Player));

            plugin.playerData.Remove(player.Username);
        }
    }
}
