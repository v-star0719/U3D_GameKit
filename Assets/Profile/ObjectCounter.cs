using UnityEngine;
using System.Collections.Generic;


namespace CamelGames.Tools.Profile
{
    public class ObjectCounter
    {
        private List<Dictionary<System.Type, int>> snapshotList = new List<Dictionary<System.Type, int>>();

        public Dictionary<System.Type, int> GenerateSnapshot()
        {
            var snapshot = new Dictionary<System.Type, int>();
            foreach (var obj in Resources.FindObjectsOfTypeAll<Object>())
            {
                var type = obj.GetType();
                int count;
                snapshot.TryGetValue(type, out count);
                snapshot[type] = count + 1;
            }

            return snapshot;
        }
    }
}
