using System;
using Godot;

namespace GW2NotionSync.UI;

public partial class SettingsDisplay : HBoxContainer {
	[Export] public Label Label { get; set; }
	public Setting Setting { get; set; }
	public bool IsSetup { get; set; }
	
	protected void _BasePressed() {
		switch (Setting.Category) {
			case SettingCategory.Account:
				break;
			case SettingCategory.Currency:
				Sync.Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(Sync.Instance.SyncCurrenciesEvent));
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		Storage.SaveSettings();
	}
}