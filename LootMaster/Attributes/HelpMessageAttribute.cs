using System;

namespace DalamudPluginProjectTemplate.Attributes
{
	// Token: 0x0200000C RID: 12
	[AttributeUsage(AttributeTargets.Method)]
	public class HelpMessageAttribute : Attribute
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002A12 File Offset: 0x00000C12
		public string HelpMessage { get; }

		// Token: 0x06000029 RID: 41 RVA: 0x00002A1A File Offset: 0x00000C1A
		public HelpMessageAttribute(string helpMessage)
		{
			this.HelpMessage = helpMessage;
		}
	}
}
