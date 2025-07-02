using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace GW2NotionSync.Data;

public class Currency {
	/// <summary>The currency ID.</summary>
	public required int Id { get; init; }

	/// <summary>The name of the currency.</summary>
	public required string Name { get; init; }

	/// <summary>The description of the currency as displayed in the wallet panel.</summary>
	public required string Description { get; init; }

	/// <summary>The display order of the currency in the wallet panel.</summary>
	public required int Order { get; init; }

	/// <summary>The URL of the currency icon.</summary>
	public required Uri IconUrl { get; init; }

	/// <summary>The URL of the currency icon.</summary>
	public required string IconPath { get; init; }

	public bool Enabled { get; set; } = true;

	public static Currency FromAPI(GuildWars2.Hero.Wallet.Currency currency) {
		return new Currency { Id = currency.Id, Name = currency.Name, Description = currency.Description, 
			Order = currency.Order, IconUrl = currency.IconUrl, 
			IconPath = $"{Reference.CurrencyIconPath}{currency.Id}.png"};
	}

	public static Dictionary<Currency, int> TotalCurrencies() {
		var dict = new Dictionary<Currency, int>();

		foreach (var currency in Reference.Currencies.Where(x => x.Enabled)) {
			var amount = Reference.Accounts.Where(x => x.Enabled && x.Currencies.ContainsKey(currency)).Sum(account => account.Currencies[currency]);
			dict.Add(currency, amount);
		}
		
		return dict;
	}
}