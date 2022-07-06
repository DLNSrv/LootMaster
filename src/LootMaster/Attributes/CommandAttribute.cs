using System;

namespace DalamudPluginProjectTemplate.Attributes
{
	// Token: 0x0200000B RID: 11
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000029FB File Offset: 0x00000BFB
		public string Command { get; }

		// Token: 0x06000027 RID: 39 RVA: 0x00002A03 File Offset: 0x00000C03
		public CommandAttribute(string command)
		{
			this.Command = command;
		}
	}
}
