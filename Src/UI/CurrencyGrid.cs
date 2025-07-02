using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using GW2NotionSync;
using GW2NotionSync.Data;

public partial class CurrencyGrid : Tree {
	public override void _Ready() {
		base._Ready();

		Sync.Instance.SyncCurrenciesEvent += SyncCurrencies;
		Sync.Instance.SyncAllEvent += SyncCurrencies;
	}

	private void SyncCurrencies() {
		Clear();
		var totalCurrency = Settings.HideEmptyCurrencies ? Currency.TotalCurrencies().Where(x => x.Value > 0).ToDictionary() : Currency.TotalCurrencies();
		var currencies = Reference.Currencies.Where(totalCurrency.ContainsKey).ToList();
		
		Columns = 1 + totalCurrency.Count();

		//Not sure why this is needed...
		var columnSizeOffset = 32;
		var iconSize = Settings.ShouldDisplayCurrencyIcon ? GetThemeConstant("icon_max_width") : 0;

		#region First Row
		
		var item = CreateItem();
		
		//Account Column
		const string accountString = "Account";
		item.SetText(0, accountString);
		item.SetTextAlignment(0, HorizontalAlignment.Center);
		var accountWidth = columnSizeOffset + GetStringSize(accountString);
		SetColumnCustomMinimumWidth(0, accountWidth);
		
		//
		for (var i = 1; i < currencies.Count + 1; i++) {
			var currency = currencies[i-1];

			var image = new ImageTexture();
			image.SetImage(Image.LoadFromFile(currency.IconPath));

			var columnWidth = columnSizeOffset + iconSize + GetStringSize(currency.Name, true);
			if (columnWidth > GetColumnWidth(i))
				SetColumnCustomMinimumWidth(i, columnWidth);
			if (Settings.ShouldDisplayCurrencyIcon)
				item.SetIcon(i, image);
			if (Settings.ShouldDisplayCurrencyName)
				item.SetText(i, currency.Name);
			item.SetTooltipText(i, currency.Name);
		}

		#endregion

		#region Total pt1
		
		var totalItem = CreateItem();
		
		//Account Column
		const string totalString = "Total";
		totalItem.SetText(0, totalString);
		totalItem.SetTextAlignment(0, HorizontalAlignment.Left);

		#endregion
		
		foreach (var account in Reference.Accounts.Where(x => x.Enabled && x.Initialized)) {
			var item1 = CreateItem();
		
			//Account Column
			var accountName = account.Nickname.Equals("") ? account.AccountName : account.Nickname;
			item1.SetText(0, accountName);
			item1.SetTextAlignment(0, HorizontalAlignment.Left);
			var accountNameWidth = columnSizeOffset + GetStringSize(accountName);
			if (accountNameWidth > GetColumnWidth(0))
				SetColumnCustomMinimumWidth(0, accountNameWidth);
		
			//
			for (var i = 1; i < currencies.Count + 1; i++) {
				var currency = currencies[i-1];
				var hasCurrency = account.Currencies.ContainsKey(currency);
			
				var columnWidth = columnSizeOffset  + GetStringSize(hasCurrency ? account.Currencies[currency].ToString() : 0.ToString());
				if (columnWidth > GetColumnWidth(i))
					SetColumnCustomMinimumWidth(i, columnWidth);
				item1.SetText(i, hasCurrency ? account.Currencies[currency].ToString() : 0.ToString());
			}
		}

		#region Total pt2
		
		for (var i = 1; i < currencies.Count + 1; i++) {
			var currency = currencies[i-1];
			
			var columnWidth = columnSizeOffset  + GetStringSize(totalCurrency[currency].ToString());
			if (columnWidth > GetColumnWidth(i))
				SetColumnCustomMinimumWidth(i, columnWidth);
			totalItem.SetText(i, totalCurrency[currency].ToString());
		}

		#endregion
	}

	private int GetStringSize(string text, bool displayMode = false) {
		return (int)GetThemeFont("font").GetStringSize(
			displayMode && (Settings.ShouldDisplayCurrencyName) || !displayMode ? text : "", 
			fontSize: GetThemeFontSize("font_size")).X;
	}
}
