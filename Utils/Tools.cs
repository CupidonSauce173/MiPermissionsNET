using System;
using System.Collections.Generic;

namespace MiPermissionsNET.Utils
{
    public static class DictionaryTools
    {
        public static Dictionary<Key, Value> MergeCommandContainers<Key, Value>(this Dictionary<Key, Value> left, Dictionary<Key, Value> right)
        {
            if (left == null) throw new ArgumentNullException("Can't merge into a null dictionary.");
            else if (right == null) return left;

            foreach (var kvp in right) if (!left.ContainsKey(kvp.Key)) left.Add(kvp.Key, kvp.Value);
            return left;
        }
    }
}
