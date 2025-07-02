using Godot;
using System;
using GW2NotionSync;

public partial class AccountContainer : VBoxContainer {
	public PackedScene AccountDisplay = GD.Load<PackedScene>("res://Scenes/AccountDisplay.tscn");
	
	public override void _Ready() {
		base._Ready();

		// Create UI elements for all already loaded accounts
		foreach (var account in Reference.Accounts) {
			var accountDisplay = AccountDisplay.Instantiate<AccountDisplay>();
			accountDisplay.Account = account;
			AddChild(accountDisplay);
		}
	}
}
