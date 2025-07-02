using Godot;
using System;
using GW2NotionSync;

public partial class CurrencyContainer : VBoxContainer {
	[Export] public PackedScene CurrencyScene;
	public override void _Ready() {
		base._Ready();

		Sync.Instance.SyncCurrenciesEvent += SyncCurrencies;
	}

	private void SyncCurrencies() {
		
	}
}
