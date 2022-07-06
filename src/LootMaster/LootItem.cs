using System.Runtime.InteropServices;
namespace DalamudPluginProjectTemplate
{
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	public struct LootItem
	{
		public bool Rolled
		{
			get
			{
				return RolledState > 0U;
			}
		}

		public bool Valid
		{
			get
			{
				return ObjectId != 3758096384U && ObjectId > 0U;
			}
		}

		[FieldOffset(0)]
		public uint ObjectId;

		[FieldOffset(8)]
		public uint ItemId;

		[FieldOffset(32)]
		public RollState RollState;

		[FieldOffset(36)]
		public RollOption RolledState;

		[FieldOffset(44)]
		public float LeftRollTime;

		[FieldOffset(32)]
		public float TotalRollTime;

		[FieldOffset(60)]
		public uint Index;
	}
}
