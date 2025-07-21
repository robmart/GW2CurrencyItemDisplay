using System;
using Godot;
using GW2NotionSync.Extension;

namespace GW2NotionSync.UI;

public partial class SettingsDisplayBool : SettingsDisplay {
	[Export] public CheckBox EnabledBox { get; set; }

	public override void _Ready() {
		base._Ready();

		EnabledBox.Pressed += _Pressed;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		
		// Only runs once when the setting is set
		if (IsSetup || Setting == null) return;
		
		// Sets all the setting data in the UI
		EnabledBox.ButtonPressed = (bool)Setting.Value;
		Label.Text = Setting.Name.UpperCamelToTitleCase();
		IsSetup = true;
	}

	private void _Pressed() {
		if (!IsSetup || Setting == null) return;
		Setting.Value = EnabledBox.IsPressed();
		_BasePressed();
	}
}