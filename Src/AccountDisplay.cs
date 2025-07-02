using Godot;
using System;
using GW2NotionSync;
using GW2NotionSync.Data;

public partial class AccountDisplay : VBoxContainer {
	[Export] public Label Nickname;
	[Export] public Label DisplayName;
	[Export] public CheckBox Enabled;
	[Export] public TextureButton Remove;
	
	public Account Account { get; set; }
	private bool IsSetup { get; set; } = false;
	private bool IsPreSetup { get; set; } = false;

	public override void _Ready() {
		base._Ready();

		Enabled.Pressed += _EnableToggled;
		Remove.Pressed += _Remove;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (!IsPreSetup) {
			IsPreSetup = true;
			Nickname.Text = Account.Nickname.Equals(string.Empty) ? "<None>" : Account.Nickname;
			Enabled.ButtonPressed = Account.Enabled;
		}

		if (IsSetup || Account == null || Account.AccountName.Equals(string.Empty)) return;
		
		IsSetup = true;
		DisplayName.Text = Account.AccountName;
	}

	private void _EnableToggled() {
		if (Account == null) return;
		
		Account.Enabled = Enabled.ButtonPressed;
		Sync.Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(Sync.Instance.SyncAllEvent));
	}

	private void _Remove() {
		Reference.Accounts.Remove(Account);
		Sync.Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(Sync.Instance.SyncAllEvent));
		Storage.SaveAll();
		QueueFree();
	}
}
