using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using DalamudPluginProjectTemplate.Attributes;

namespace DalamudPluginProjectTemplate
{
	public class PluginCommandManager<THost> : IDisposable
	{
		public PluginCommandManager(THost host, DalamudPluginInterface pluginInterface)
		{
			this.pluginInterface = pluginInterface;
			this.host = host;
			pluginCommands = (from method in host.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where method.GetCustomAttribute<CommandAttribute>() != null
				select method).SelectMany(new Func<MethodInfo, IEnumerable<ValueTuple<string, CommandInfo>>>(GetCommandInfoTuple)).ToArray();
			Array.Reverse(pluginCommands);
			AddCommandHandlers();
		}

		private void AddCommandHandlers()
		{
			for (int i = 0; i < pluginCommands.Length; i++)
			{
				ValueTuple<string, CommandInfo> valueTuple = pluginCommands[i];
				string command = valueTuple.Item1;
				CommandInfo commandInfo = valueTuple.Item2;
				Plugin.CommandManager.AddHandler(command, commandInfo);
			}
		}

		private void RemoveCommandHandlers()
		{
			for (int i = 0; i < pluginCommands.Length; i++)
			{
				string command = pluginCommands[i].Item1;
				Plugin.CommandManager.RemoveHandler(command);
			}
		}

		private IEnumerable<ValueTuple<string, CommandInfo>> GetCommandInfoTuple(MethodInfo method)
		{
			CommandInfo.HandlerDelegate handlerDelegate = (CommandInfo.HandlerDelegate)Delegate.CreateDelegate(typeof(CommandInfo.HandlerDelegate), host, method);
			CommandAttribute command = handlerDelegate.Method.GetCustomAttribute<CommandAttribute>();
			AliasesAttribute aliases = handlerDelegate.Method.GetCustomAttribute<AliasesAttribute>();
			HelpMessageAttribute helpMessage = handlerDelegate.Method.GetCustomAttribute<HelpMessageAttribute>();
			DoNotShowInHelpAttribute doNotShowInHelp = handlerDelegate.Method.GetCustomAttribute<DoNotShowInHelpAttribute>();
			CommandInfo commandInfo = new(handlerDelegate)
			{
				HelpMessage = (((helpMessage != null) ? helpMessage.HelpMessage : null) ?? string.Empty),
				ShowInHelp = (doNotShowInHelp == null)
			};
			List<ValueTuple<string, CommandInfo>> commandInfoTuples = new()
            {
				new ValueTuple<string, CommandInfo>(command.Command, commandInfo)
			};
			if (aliases != null)
			{
				for (int i = 0; i < aliases.Aliases.Length; i++)
				{
					commandInfoTuples.Add(new ValueTuple<string, CommandInfo>(aliases.Aliases[i], commandInfo));
				}
			}
			return commandInfoTuples;
		}

		public void Dispose()
		{
			RemoveCommandHandlers();
		}

		private readonly DalamudPluginInterface pluginInterface;

		private readonly ValueTuple<string, CommandInfo>[] pluginCommands;

		private readonly THost host;
	}
}
