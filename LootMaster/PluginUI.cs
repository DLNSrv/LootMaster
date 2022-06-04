using System;
using ImGuiNET;

namespace DalamudPluginProjectTemplate
{
	public class PluginUI
	{
        [Obsolete]
        public void Draw()
		{
			if (!IsVisible)
			{
				return;
			}
			if (ImGui.Begin("LootMaster Config", ref IsVisible, 96))
			{
				ImGui.TextUnformatted("명령어를 사용하여 전리품 입찰: ");
				if (ImGui.BeginTable("lootlootlootlootloot", 2))
				{
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("/need");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("가능한 한 모든 아이템에 선입찰을 시도합니다.");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("/greed");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("모든 아이템에 입찰합니다.");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("/pass");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("입찰하지 않은 모든 아이템을 포기합니다.");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("/passall");
					ImGui.TableNextColumn();
					ImGui.TextUnformatted("이미 입찰한 아이템을 포함해 모두 포기합니다.");
					ImGui.EndTable();
				}
				ImGui.Spacing();
				ImGui.Separator();
				ImGui.Checkbox("채팅에 전리품 목록 표시", ref Plugin.config.EnableChatLogMessage);
				ImGui.End();
			}
		}

		public bool IsVisible;
	}
}
