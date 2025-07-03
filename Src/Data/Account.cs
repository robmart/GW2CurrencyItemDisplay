using System.Collections.Generic;
using System.Linq;
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
		 var save = new Godot.Collections.Dictionary<string, Variant>() {
			{ "Type", "Account" },
			{ "Nickname", Nickname },
			{ "ApiKey", ApiKey },
			{ "Enabled", Enabled },
		};

		if (AccountName == string.Empty) return save;
		
		save.Add("AccountName", AccountName);

		var currencies = new Godot.Collections.Dictionary<int, int>();
		foreach (var currency in Currencies) {
			currencies.Add(currency.Key.Id, currency.Value);
		}
		
		save.Add("Currencies", currencies);

		return save;
	}
	
	public static void Load(Godot.Collections.Dictionary<string, Variant> save) {
		var loadedAccount = new Account() {
			Enabled = save["Enabled"].AsBool(), Nickname = save["Nickname"].AsString(),
			ApiKey = save["ApiKey"].AsString()
		};
		Reference.Accounts.Add(loadedAccount);
		
		if (!save.ContainsKey("AccountName")) return;
		
		loadedAccount.AccountName = save["AccountName"].AsString();
		
		var loadedCurrencies = save["Currencies"].AsGodotDictionary<int, int>();
		foreach (var currencyPair in loadedCurrencies) {
			loadedAccount.Currencies.Add(Reference.Currencies.First(x => x.Id == currencyPair.Key), currencyPair.Value);
		}
		
		loadedAccount.Initialized = true;
	}
}