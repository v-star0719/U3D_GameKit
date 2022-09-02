using System;
using System.Collections.Generic;

namespace MemoryProfilerEx
{
    // Decompiled with JetBrains decompiler
    // Type: UnityEditor.MemoryElement
    // Assembly: UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
    // MVID: 16D986CC-6F77-4459-A0B1-B49FCCFE505E
    // Assembly location: D:\Program Files\2019.4.7f1\Editor\Data\Managed\UnityEditor.dll

    [Serializable]
    class MemoryElement
    {
        public List<MemoryElement> children = (List<MemoryElement>)null;
        public MemoryElement parent = (MemoryElement)null;
        public ObjectInfo memoryInfo;
        public long totalMemory;
        public int totalChildCount;
        public string name;
        public bool expanded;
        public string description;

        public MemoryElement()
        {
            this.children = new List<MemoryElement>();
        }

        public MemoryElement(string n)
        {
            this.expanded = false;
            this.name = n;
            this.children = new List<MemoryElement>();
            this.description = "";
        }

        public MemoryElement(ObjectInfo memInfo, bool finalize)
        {
            this.expanded = false;
            this.memoryInfo = memInfo;
            this.name = this.memoryInfo.name;
            this.totalMemory = memInfo != null ? memInfo.memorySize : 0L;
            this.totalChildCount = 1;
            if(!finalize)
                return;
            this.children = new List<MemoryElement>();
        }

        public MemoryElement(string n, List<MemoryElement> groups)
        {
            this.name = n;
            this.expanded = false;
            this.description = "";
            this.totalMemory = 0L;
            this.totalChildCount = 0;
            this.children = new List<MemoryElement>();
            foreach(MemoryElement group in groups)
                this.AddChild(group);
        }

        public void ExpandChildren()
        {
            if(this.children != null)
                return;
            this.children = new List<MemoryElement>();
            for(int index = 0; index < this.ReferenceCount(); ++index)
                this.AddChild(new MemoryElement(this.memoryInfo.referencedBy[index], false));
        }

        public int AccumulatedChildCount()
        {
            return this.totalChildCount;
        }

        public int ChildCount()
        {
            if(this.children != null)
                return this.children.Count;
            return this.ReferenceCount();
        }

        public int ReferenceCount()
        {
            return this.memoryInfo == null || this.memoryInfo.referencedBy == null ? 0 : this.memoryInfo.referencedBy.Count;
        }

        public void AddChild(MemoryElement node)
        {
            if(node == this)
                throw new Exception("Should not AddChild to itself");
            this.children.Add(node);
            node.parent = this;
            this.totalMemory += node.totalMemory;
            this.totalChildCount += node.totalChildCount;
        }

        public int GetChildIndexInList()
        {
            for(int index = 0; index < this.parent.children.Count; ++index)
            {
                if(this.parent.children[index] == this)
                    return index;
            }
            return this.parent.children.Count;
        }

        public MemoryElement GetPrevNode()
        {
            int index = this.GetChildIndexInList() - 1;
            if(index < 0)
                return this.parent;
            MemoryElement child = this.parent.children[index];
            while(child.expanded)
                child = child.children[child.children.Count - 1];
            return child;
        }

        public MemoryElement GetNextNode()
        {
            if(this.expanded && this.children.Count > 0)
                return this.children[0];
            int index1 = this.GetChildIndexInList() + 1;
            if(index1 < this.parent.children.Count)
                return this.parent.children[index1];
            for(MemoryElement parent = this.parent; parent.parent != null; parent = parent.parent)
            {
                int index2 = parent.GetChildIndexInList() + 1;
                if(index2 < parent.parent.children.Count)
                    return parent.parent.children[index2];
            }
            return (MemoryElement)null;
        }

        public MemoryElement GetRoot()
        {
            if(this.parent != null)
                return this.parent.GetRoot();
            return this;
        }

        public MemoryElement FirstChild()
        {
            return this.children[0];
        }

        public MemoryElement LastChild()
        {
            if(!this.expanded)
                return this;
            return this.children[this.children.Count - 1].LastChild();
        }
    }

}
