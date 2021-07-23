using System;
using System.Collections.Generic;

using MiNET;
using MiNET.Plugins;

namespace MiPermissionsNET.Objects
{
    /// <summary>
    /// Class to create MiPlayer entities.
    /// </summary>
    public class MiPlayer
    {
        private int id;
        private int playTime;
        private bool isBanned;
        private string currentIp;
        private List<KeyValuePair<string,string>> ipList = new();
        private List<string> permissions = new();
        private DateTime regDate;
        private Player player;
        private CommandSet commandContainer = new();

        /// <summary>
        /// To check if a player has a permission.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            foreach (MiGroup group in MiGroups) 
                if (group.Permissions.Contains(permission)) return true;

            if (permissions.Contains(permission)) return true;
            return false;
        }

        /// <summary>
        /// Will add a permission to the player.
        /// </summary>
        /// <param name="permission"></param>
        public void AddPermission(string permission)
        {
            if (permission.Contains(permission)) return;
            permissions.Add(permission);
        }

        /// <summary>
        /// Will remove a permission from a player.
        /// </summary>
        /// <param name="permission"></param>
        public void RemovePermission(string permission)
        {
            if (!permissions.Contains(permission)) return;
            permissions.Remove(permission);
        }

        public CommandSet CommandContainer
        {
            get { return commandContainer; }
            set { commandContainer = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int PlayTime
        {
            get { return playTime; }
            set { playTime = value; }
        }

        public bool IsBanned
        {
            get { return isBanned; }
            set { isBanned = value; }
        }

        public string CurrentIp
        {
            get { return currentIp; }
            set { currentIp = value; }
        }

        public List<KeyValuePair<string,string>> IpList
        {
            get { return ipList; }
            set { ipList = value; }
        }

        public List<string> Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }

        public DateTime RegDate
        {
            get { return regDate; }
            set { regDate = value; }
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        internal List<MiGroup> MiGroups { get; set; }

        internal MiGroup FrontGroup { get; set; }
    }
}