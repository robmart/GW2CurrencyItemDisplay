using Godot;
using System;
using GW2NotionSync;
using GW2NotionSync.Data;

public partial class AccountDisplay : VBoxContainer {
	[Export] public Label Nickname;
	[Export] public Label DisplayName;
	[Export] public CheckBox Enabled;
	
	public Account Account { get; set; }
	private bool IsSetup { get; set; } = false;

	public override void _Ready() {
		base._Ready();

		Enabled.Pressed += _EnableToggled;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (IsSetup || Account == null) return;
		
		IsSetup = true;
		Nickname.Text = Account.Nickname.Equals("") ? "<None>" : Account.Nickname;
		DisplayName.Text = Account.AccountName;
		Enabled.ButtonPressed = Account.Enabled;
	}

	private void _EnableToggled() {
		if (Account == null) return;
		
		Account.Enabled = Enabled.ButtonPressed;
		Sync.Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(Sync.Instance.SyncAllEvent));
	}
}
