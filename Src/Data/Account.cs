using System.Collections.Generic;
using Godot;
using GuildWars2.Hero.Wallet;

namespace GW2NotionSync.Data;

public class Account {
	/// <summary>Whether to show this account or not.</summary>
	public bool Enabled { get; set; } = true;
	/// <summary>The set nickname.</summary>
	public string Nickname { get; set; } = string.Empty;
	/// <summary>The account API key.</summary>
	public required string ApiKey { get; init; }
	
	//Data
	/// <summary>Whether the account has been synced at least once.</summary>
	public bool Initialized { get; internal set; }
	/// <summary>The account display name.</summary>
	public string AccountName { get; internal set; } = string.Empty;
	/// <summary>The currencies possessed by this account, as well as their amounts.</summary>
	public Dictionary<Currency, int> Currencies { get; set;  } = new();
	
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