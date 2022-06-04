using System;

namespace DalamudPluginProjectTemplate.Attributes
{
	// Token: 0x0200000A RID: 10
	[AttributeUsage(AttributeTargets.Method)]
	public class AliasesAttribute : Attribute
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000029E4 File Offset: 0x00000BE4
		public string[] Aliases { get; }

		// Token: 0x06000025 RID: 37 RVA: 0x000029EC File Offset: 0x00000BEC
		public AliasesAttribute(params string[] aliases)
		{
			this.Aliases = aliases;
		}
	}
}
