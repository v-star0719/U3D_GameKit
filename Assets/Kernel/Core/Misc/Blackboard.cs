using System;
using System.Collections.Generic;

namespace Kernel.Core
{
    public class Blackboard
    {
        private Dictionary<int, int> intValues = new Dictionary<int, int>();
        private Dictionary<int, string> stringValues = new Dictionary<int, string>();
        private Dictionary<int, float> floatValues = new Dictionary<int, float>();
        private Dictionary<int, Object> objValues = new Dictionary<int, Object>();

        public int GetInt(int key, int defaultVal)
        {
            intValues.TryGetValue(key, out defaultVal);
            return defaultVal;
        }

        public string GetString(int key, string defaultVal)
        {
            stringValues.TryGetValue(key, out defaultVal);
            return defaultVal;
        }

        public float GetFloat(int key, float defaultVal)
        {
            floatValues.TryGetValue(key, out defaultVal);
            return defaultVal;
        }

        public Object GetObject(int key)
        {
            Object obj = null;
            objValues.TryGetValue(key, out obj);
            return obj;
        }

        public void SetInt(int key, int val)
        {
            intValues[key] = val;
        }

        public void SetString(int key, string val)
        {
            stringValues[key] = val;
        }

        public void SetFloat(int key, float val)
        {
            floatValues[key] = val;
        }

        public void SetObject(int key, Object obj)
        {
            objValues[key] = obj;
        }
    }
}