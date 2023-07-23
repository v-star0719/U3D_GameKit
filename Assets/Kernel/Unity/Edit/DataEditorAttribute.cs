using System;

namespace Kernel.Edit
{
	//指定一个类的Editor。纯编辑器用。
	[AttributeUsage(AttributeTargets.Class)]
	public class DataEditorAttribute : Attribute
	{
		public Type DataType;

		public DataEditorAttribute(Type t)
		{
			DataType = t;
		}
	}
}