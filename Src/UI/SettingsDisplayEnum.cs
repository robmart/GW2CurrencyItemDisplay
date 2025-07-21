using System;
using Godot;
using GW2NotionSync.Extension;

namespace GW2NotionSync.UI;

public partial class SettingsDisplayEnum : SettingsDisplay {
	[Export] public MenuButton Options { get; set; }

	public override void _Ready() {
		base._Ready();

		Options.GetPopup().IdPressed += _IdPressed;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		
		// Only runs once when the setting is set
		if (IsSetup || Setting == null) return;
		
		// Sets all the setting data in the UI
		Options.Text = Setting.Value.ToString().UpperCamelToTitleCase();
		foreach (var enumName in Setting.EnumType.GetEnumNames()) {
			Options.GetPopup().AddItem(enumName.UpperCamelToTitleCase());
		}
		Label.Text = Setting.Name.UpperCamelToTitleCase();
		IsSetup = true;
	}

	private void _IdPressed(long id) {
		if (!IsSetup || Setting == null) return;
		Options.Text = Setting.EnumType.GetEnumNames()[id].UpperCamelToTitleCase();
		Setting.Value = (int)id;
		_BasePressed();
	}
}