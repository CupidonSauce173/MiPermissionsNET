using MiNET;
using MiNET.Net;
using MiNET.Plugins.Attributes;
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
        public static Packet HandleCommands(McpeCommandRequest msg, Player player)
        {
            string commandName = Regex.Split(msg.command, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(s => s.Trim('"')).ToArray()[0].Trim('/');
            if (plugin.GetAPI().GetMiPlayer(player).CommandContainer.ContainsKey(commandName.ToLower()) != true) return null;
            return msg;
        }

        /// <summary>
        /// This will handle the chat system (I have to add the enable/disable option for it.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        [PacketHandler]
        public static Packet HandleChat(McpeText packet, Player player)
        {
            if (packet.message.StartsWith("/") || packet.message.StartsWith(".")) return packet;
            player.Level.BroadcastMessage($"§d[§a{plugin.GetAPI().GetMiPlayer(player).FrontGroup.Name}§d]§e {player.Username} §f> §a {packet.message}");
            return null;
        }
    }
}
