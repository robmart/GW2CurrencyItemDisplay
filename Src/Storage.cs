using System.Linq;
using Godot;
using GW2NotionSync.Data;

namespace GW2NotionSync;

public partial class Storage : Control { //TODO: Don't hardcode the paths
	public static void SaveAll() {
		SaveCurrencies();
		SaveAccounts();
	}

	public static void SaveCurrencies() {
		using var saveFile = FileAccess.Open("user://Currencies.save", FileAccess.ModeFlags.Write);
		foreach (var currencyData in Reference.Currencies.Select(currency => currency.Save())) {
			saveFile.StoreLine(Json.Stringify(currencyData));
		}
	}

	public static void SaveAccounts() {
		using var saveFile = FileAccess.Open("user://Accounts.save", FileAccess.ModeFlags.Write);
		foreach (var accountData in Reference.Accounts.Select(account => account.Save())) {
			saveFile.StoreLine(Json.Stringify(accountData));
		}
	}

	public static void LoadAll() {
		LoadCurrencies();
		LoadAccounts();

	}

	public static void LoadCurrencies() {
		if (!FileAccess.FileExists("user://Currencies.save")) return;
		
		using var saveFile = FileAccess.Open("user://Currencies.save", FileAccess.ModeFlags.Read);
		while (saveFile.GetPosition() < saveFile.GetLength()) {
			var jsonString = saveFile.GetLine();
			var json = new Json();
			var parseResult = json.Parse(jsonString);

			if (parseResult != Error.Ok) {
				GD.Print(
					$"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
				continue;
			}

			var nodeData =
				new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
			Currency.Load(nodeData);
		}
		Sync.Instance.EmitSignal(nameof(Sync.Instance.SyncCurrenciesEvent));
	}

	public static void LoadAccounts() {
		if (!FileAccess.FileExists("user://Accounts.save")) return;
		
		using var saveFile = FileAccess.Open("user://Accounts.save", FileAccess.ModeFlags.Read);
		while (saveFile.GetPosition() < saveFile.GetLength()) {
			var jsonString = saveFile.GetLine();
			var json = new Json();
			var parseResult = json.Parse(jsonString);

			if (parseResult != Error.Ok) {
				GD.Print(
					$"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
				continue;
			}

			var nodeData = 
				new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
			Account.Load(nodeData);
		}
		Sync.Instance.EmitSignal(nameof(Sync.Instance.SyncAllAccountEvent));
	}
}