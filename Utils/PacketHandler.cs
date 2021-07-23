using MiNET;
using MiNET.Net;
using MiNET.Plugins.Attributes;

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiPermissionsNET.Utils
{
    class PacketHandler
    {
        private static MiPermissionsNET plugin;

        public PacketHandler(MiPermissionsNET pl)
        {
            plugin = pl;
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
            Console.WriteLine("test");
            string commandName = Regex.Split(msg.command, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(s => s.Trim('"')).ToArray()[0].Trim('/');
            if (plugin.GetAPI().GetMiPlayer(player).CommandContainer.ContainsKey(commandName.ToLower()) != true) return null;
            return msg;
        }

        [PacketHandler]
        public Packet HandleChat(McpeText msg, Player player)
        {
            Console.WriteLine("test");
            if (msg.message.StartsWith("/") || msg.message.StartsWith(".")) return msg;
            player.Level.BroadcastMessage($"§d[§a{plugin.GetAPI().GetMiPlayer(player).FrontGroup.Name}§d]§e {player.Username} §f> §a");
            return null;
        }
    }
}
