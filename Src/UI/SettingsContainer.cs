using Godot;
using System;
using System.Linq;
using GW2NotionSync;
using GW2NotionSync.UI;

public partial class SettingsContainer : VBoxContainer {
	[Export] public PackedScene SettingSceneBool;
	[Export] public PackedScene SettingSceneEnum;
	[Export] public SettingCategory  SettingCategory;

	public override void _Ready() {
		base._Ready();

		foreach (var setting in Reference.Settings.Where(x => x.Category == SettingCategory)) {
			switch (setting.Type) {
				case SettingType.Bool:
					var boolScene = SettingSceneBool.Instantiate<SettingsDisplayBool>();
					boolScene.Setting = setting;
					AddChild(boolScene);
					break;
				case SettingType.Enum:
					var enumScene = SettingSceneEnum.Instantiate<SettingsDisplayEnum>();
					enumScene.Setting = setting;
					AddChild(enumScene);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
