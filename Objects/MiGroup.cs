using MiNET.Plugins;
using System.Collections.Generic;

namespace MiPermissionsNET.Objects
{
    public class MiGroup
    {
        private string name;
        private int id;
        private List<string> permissions = new();
        private bool isDefault;
        private int priority;
        private Dictionary<string, Command> commandContainer = new(); // key = name value = command

        /// <summary>
        /// To check if a group has a permission.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            if (permissions.Contains(permission))
            {
                return true;
            }
            return false;
        }

        public void AddCommand(Command command, bool refresh = false)
        {
            if (commandContainer.ContainsKey(command.Name)) return;
            commandContainer.Add(command.Name, command);

            if (refresh)
            {
                //Implement refresh commands for players
            }
        }

        public void RemoveCommand(Command command, bool refresh = false)
        {
            if (commandContainer.ContainsKey(command.Name))
                commandContainer.Remove(command.Name);

            if (refresh)
            {
                //Implement refresh commands for players
            }
        }

        public Dictionary<string, Command> CommandContainer
        {
            get { return commandContainer; }
            set { commandContainer = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<string> Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }

        public bool IsDefault
        {
            get { return isDefault; }
            set { isDefault = value; }
        }
    }
}