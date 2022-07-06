using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using DalamudPluginProjectTemplate.Attributes;

namespace DalamudPluginProjectTemplate
{
	public class Plugin : IDalamudPlugin, IDisposable
	{
		[PluginService]
		public static CommandManager? CommandManager { get; set; }
		[PluginService]
		public static DalamudPluginInterface? pi { get; set; }
		[PluginService]
		public static SigScanner? SigScanner { get; set; }
		[PluginService]
		public static ChatGui? ChatGui { get; set; }

		public static List<LootItem> LootItems
		{
			get
			{
				return (from i in ReadArray<LootItem>(lootsAddr + 16, 16)
					where i.Valid
					select i).ToList();
			}
		}

		public string Name
		{
			get
			{
				return "LootMaster";
			}
		}

        public Plugin()
		{
			lootsAddr = SigScanner.GetStaticAddressFromSig("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 89 44 24 60", 0);
			rollItemRaw = Marshal.GetDelegateForFunctionPointer<RollItemRaw>(SigScanner.ScanText("41 83 F8 ?? 0F 83 ?? ?? ?? ?? 48 89 5C 24 08"));
			config = (pi.GetPluginConfig() as Configuration) ?? new Configuration();
			config.Initialize(pi);
			ui = new PluginUI();
			pi.UiBuilder.Draw += ui.Draw;
			pi.UiBuilder.OpenConfigUi += delegate()
			{
				PluginUI pluginUI = ui;
				pluginUI.IsVisible = !pluginUI.IsVisible;
			};
			commandManager = new PluginCommandManager<Plugin>(this, pi);
		}

		private void RollItem(RollOption option, int index)
		{
			LootItem lootItem = LootItems[index];
			PluginLog.Information(string.Format("{0} [{1}] {2} Id: {3:X} rollState: {4} rollOption: {5}", new object[] { option, index, lootItem.ItemId, lootItem.ObjectId, lootItem.RollState, lootItem.RolledState }), Array.Empty<object>());
			rollItemRaw(lootsAddr, option, (uint)index);
		}
		
		[Command("/need")]
		[HelpMessage("Trying to Need on all items.")]
		public void NeedCommand(string command, string args)
		{
			new Thread(() =>
			{
				int needCount = 0;
				int greedCount = 0;
				for (int i = 0; i < LootItems.Count; i++)
				{
					if (!LootItems[i].Rolled)
					{
						if (LootItems[i].RollState == RollState.UpToNeed)
						{
							RollItem(RollOption.Need, i);
							needCount++;
						}
						else
						{
							RollItem(RollOption.Greed, i);
							greedCount++;
						}
					}
					Thread.Sleep(1500);
				}
				if (config.EnableChatLogMessage)
				{
					ChatGui.Print(new SeString(new List<Payload>
					{
						new TextPayload("Need "),
						new UIForegroundPayload(575),
						new TextPayload(needCount.ToString()),
						new UIForegroundPayload(0),
						new TextPayload(" item" + ((needCount > 1) ? "s" : "") + ", greed "),
						new UIForegroundPayload(575),
						new TextPayload(greedCount.ToString()),
						new UIForegroundPayload(0),
						new TextPayload(" item" + ((greedCount > 1) ? "s" : "") + ".")
					}));
				}
			}).Start();
		}

		[Command("/greed")]
		[HelpMessage("Greed all items.")]
		public void GreedCommand(string command, string args)
		{
			new Thread(() =>
			{
				int greedCount = 0;
				for (int i = 0; i < LootItems.Count; i++)
				{
					if (!LootItems[i].Rolled)
					{
						RollItem(RollOption.Greed, i);
						greedCount++;
					}
					Thread.Sleep(1500);
				}
				if (config.EnableChatLogMessage)
				{
					ChatGui.Print(new SeString(new List<Payload>
					{
						new TextPayload("Greed "),
						new UIForegroundPayload(575),
						new TextPayload(greedCount.ToString()),
						new UIForegroundPayload(0),
						new TextPayload(" item" + ((greedCount > 1) ? "s" : "") + ".")
					}));
				}
			}).Start();
		}

		[Command("/pass")]
		[HelpMessage("Pass all item.")]
		public void PassCommand(string command, string args)
		{
			new Thread(() =>
			{
				int passCount = 0;
				for (int i = 0; i < LootItems.Count; i++)
				{
					if (!LootItems[i].Rolled)
					{
						RollItem(RollOption.Pass, i);
						passCount++;
					}
					Thread.Sleep(1500);
				}
				if (config.EnableChatLogMessage)
				{
					ChatGui.Print(new SeString(new List<Payload>
					{
						new TextPayload("Pass "),
						new UIForegroundPayload(575),
						new TextPayload(passCount.ToString()),
						new UIForegroundPayload(0),
						new TextPayload(" item" + ((passCount > 1) ? "s" : "") + ".")
					}));
				}
			}).Start();
		}

		[Command("/passall")]
		[HelpMessage("Pass All, including bid items.")]
		public void PassAllCommand(string command, string args)
		{
			new Thread(() =>
			{
				int passCount = 0;
				for (int i = 0; i < LootItems.Count; i++)
				{
					if (LootItems[i].RolledState != RollOption.Pass)
					{
						RollItem(RollOption.Pass, i);
						passCount++;
					}
					Thread.Sleep(1500);
				}
				if (config.EnableChatLogMessage)
				{
					ChatGui.Print(new SeString(new List<Payload>
					{
						new TextPayload("Pass all "),
						new UIForegroundPayload(575),
						new TextPayload(passCount.ToString()),
						new UIForegroundPayload(0),
						new TextPayload(" item" + ((passCount > 1) ? "s" : "") + ".")
					}));
				}
			}).Start();
		}

		public static T[] ReadArray<T>(IntPtr unmanagedArray, int length) where T : struct
		{
			int size = Marshal.SizeOf(typeof(T));
			T[] mangagedArray = new T[length];
			for (int i = 0; i < length; i++)
			{
				IntPtr ins = new(unmanagedArray.ToInt64() + (i * size));
				mangagedArray[i] = Marshal.PtrToStructure<T>(ins);
			}
			return mangagedArray;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			commandManager.Dispose();
			pi.SavePluginConfig(config);
			pi.UiBuilder.Draw -= ui.Draw;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static Configuration config;

		private static IntPtr lootsAddr;

		internal static RollItemRaw rollItemRaw;

		private PluginCommandManager<Plugin> commandManager;

		private PluginUI ui;

		internal delegate void RollItemRaw(IntPtr lootIntPtr, RollOption option, uint lootItemIndex);
	}
}
