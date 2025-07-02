using System.Collections.Generic;
using Godot;
using GuildWars2.Hero.Wallet;

namespace GW2NotionSync.Data;

public class Account {
	public bool Enabled { get; set; } = true;
	public string Nickname { get; set; } = string.Empty;
	public required string ApiKey { get; init; }
	
	//Data
	public bool Initialized { get; internal set; }
	public string AccountName { get; internal set; } = string.Empty;
	public Dictionary<Currency, int> Currencies { get; } = new();
	
	public Godot.Collections.Dictionary<string, Variant> Save() {
		return new Godot.Collections.Dictionary<string, Variant>() {
			{ "Type", "Account" },
			{ "Nickname", Nickname },
			{ "ApiKey", ApiKey },
			{ "Enabled", Enabled },
		};
	}
	
	public static void Load(Godot.Collections.Dictionary<string, Variant> save) {
		Reference.Accounts.Add(new Account() { Enabled = save["Enabled"].AsBool(), Nickname = save["Nickname"].AsString(), ApiKey = save["ApiKey"].AsString() });
	}
	
	
}