using System.Text;

namespace Kernel.Lang.Extension
{
	public static class ExtendedStringBuilder
	{
		public static void AppendLineEx(this StringBuilder sb, string content)
		{
			if (sb != null)
				sb.AppendLine(content);
		}

		public static void AppendEx(this StringBuilder sb, string content)
		{
			if (sb != null)
				sb.Append(content);
		}
	}
}