using Godot;
using System;
using GW2NotionSync;

public partial class CurrencyContainer : VBoxContainer {
	[Export] public PackedScene CurrencyScene;
	public override void _Ready() {
		base._Ready();

		Sync.Instance.SyncCurrenciesEvent += SyncCurrencies;
	}

	// Creates UI elements in the settings menu for each currency
	//Runs every time the currencies are changed
	private void SyncCurrencies() {
		foreach (var child in GetChildren()) {
			child.QueueFree();
		}

		foreach (var currency in Reference.Currencies) {
			var currencyScene = CurrencyScene.Instantiate<CurrencyDisplay>();
			currencyScene.Currency = currency;
			AddChild(currencyScene);
		}
	}
}
