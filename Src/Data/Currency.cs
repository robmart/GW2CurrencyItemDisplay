using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Godot;

namespace GW2NotionSync.Data;

public class Currency {
	/// <summary>The currency ID.</summary>
	public required int Id { get; init; }

	/// <summary>The name of the currency.</summary>
	public required string Name { get; set; }

	/// <summary>The description of the currency as displayed in the wallet panel.</summary>
	public required string Description { get; set; }

	/// <summary>The display order of the currency in the wallet panel.</summary>
	public required int Order { get; set; }

	/// <summary>The URL of the currency icon.</summary>
	public required Uri IconUrl { get; set; }

	/// <summary>The local path of the currency icon.</summary>
	public string IconPath => $"{Reference.CurrencyIconPath}{Id}.png";

	/// <summary>Whether to show this currency or not.</summary>
	public bool Enabled { get; set; } = true;

	/// <summary>Update fields with new data from the API.</summary>
	public void Update(GuildWars2.Hero.Wallet.Currency currency) {
		Name = currency.Name;
		Description = currency.Description;
		Order = currency.Order;
		IconUrl = currency.IconUrl;
	}
	
	/// <summary>Create a new currency from API data.</summary>
	public static Currency FromAPI(GuildWars2.Hero.Wallet.Currency currency) {
		return new Currency { Id = currency.Id, Name = currency.Name, Description = currency.Description, 
			Order = currency.Order, IconUrl = currency.IconUrl};
	}

	public Godot.Collections.Dictionary<string, Variant> Save() {
		return new Godot.Collections.Dictionary<string, Variant>() {
			{ "Id", Id },
			{ "Name", Name },
			{ "Description", Description },
			{ "Order", Order },
			{ "IconUrl", IconUrl.ToString() },
			{ "Enabled", Enabled },
		};
	}
	
	public static void Load(Godot.Collections.Dictionary<string, Variant> save) {
		Reference.Currencies.Add(new Currency { Id = save["Id"].AsInt32(), Name = save["Name"].AsString(), 
			Order = save["Order"].AsInt32(),  Description = save["Description"].AsString(), 
			IconUrl = new Uri(save["IconUrl"].AsString()), Enabled = save["Enabled"].AsBool()});
	}

	/// <summary>Gets a dictionary with all currencies and their total amounts.</summary>
	public static Dictionary<Currency, int> TotalCurrencies() {
		var dict = new Dictionary<Currency, int>();

		foreach (var currency in Reference.Currencies.Where(x => x.Enabled)) {
			var amount = Reference.Accounts.Where(x => x.Enabled && x.Currencies.ContainsKey(currency)).Sum(account => account.Currencies[currency]);
			dict.Add(currency, amount);
		}
		
		return dict;
	}
}