using System;

namespace DalamudPluginProjectTemplate
{
	public enum RollState : uint
	{
		UpToNeed,
		UpToGreed,
		UpToPass,
		Rolled = 17U,
		NoLoot = 26U
	}
}
