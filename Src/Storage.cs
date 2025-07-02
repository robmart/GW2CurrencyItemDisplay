using Godot;
using GW2NotionSync.Data;

namespace GW2NotionSync;

public partial class Storage : Control {
	public static void SaveAll() {
		using var saveFile = FileAccess.Open("user://Save.save", FileAccess.ModeFlags.Write);
		foreach (var account in Reference.Accounts) {
			var accountData = account.Save();

			saveFile.StoreLine(Json.Stringify(accountData));
		}
	}

	public static void LoadAll() {
		if (!FileAccess.FileExists("user://Save.save")) {
			return; // Error! We don't have a save to load.
		}
		
		using var saveFile = FileAccess.Open("user://Save.save", FileAccess.ModeFlags.Read);
		while (saveFile.GetPosition() < saveFile.GetLength()) {
			var jsonString = saveFile.GetLine();var json = new Json();
			var parseResult = json.Parse(jsonString);        
			
			if (parseResult != Error.Ok) {
				GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
				continue;
			}
			
			var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
			switch (nodeData["Type"].AsString()) {
				case "Account":
					Account.Load(nodeData);
					break;
			}
		}
	}
}