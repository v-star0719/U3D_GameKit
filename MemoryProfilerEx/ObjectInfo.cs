using System.Collections.Generic;

namespace MemoryProfilerEx
{
    // Decompiled with JetBrains decompiler
    // Type: UnityEditor.ObjectInfo
    // Assembly: UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
    // MVID: 16D986CC-6F77-4459-A0B1-B49FCCFE505E
    // Assembly location: D:\Program Files\2019.4.7f1\Editor\Data\Managed\UnityEditor.dll
    class ObjectInfo
    {
        public int instanceId;
        public long memorySize;
        public int reason;
        public List<ObjectInfo> referencedBy;
        public string name;
        public string className;
    }
}