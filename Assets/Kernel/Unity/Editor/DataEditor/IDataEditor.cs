namespace Kernel.Edit
{
	public interface IDataEditor
	{
		void SetData(object data);
		void OnGUI(ref bool changed);
		void OnDrawGizmos();
		bool IsFolded { get; set; }
	}

	///直接从这个类继承就行，data就自带了，不用每次强制转换
	public class DataEditorBase<T> : IDataEditor where T : class, new()
	{
		protected T Data
		{
			get;
			set;
		}

		public bool IsFolded { get; set; } = true;

		public virtual void SetData(object data)
		{
			Data = data as T;
		}
		
		///重载这个方法
		public virtual void OnGUI(ref bool changed)
		{
		}

		///重载这个方法
		public virtual void OnDrawGizmos()
		{
		}
	}
}