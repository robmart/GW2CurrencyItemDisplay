using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Godot;
using GuildWars2;
using GW2NotionSync.Data;
using HttpClient = System.Net.Http.HttpClient;

namespace GW2NotionSync;

public partial class Sync : Control {
	[Signal]
	public delegate void SyncAllEventEventHandler();
	[Signal]
	public delegate void SyncCurrenciesEventEventHandler();
	[Signal]
	public delegate void SyncAllAccountEventEventHandler();
	[Signal]
	public delegate void SyncAccountEventEventHandler(string apiKey);
	[Signal]
	public delegate void SyncAccountNameEventEventHandler(string apiKey);
	[Signal]
	public delegate void SyncCurrenciesAccountEventEventHandler(string apiKey);
	
	public static Sync Instance;

	public override void _Ready() {
		Instance = this;
		
		base._Ready();
	}
	
	public static void SyncAll() {
		Task.Run(() => SyncAllData());
		Task.Run(SyncAllAccountData);
	}

	//Sync all non-account data, the stuff that doesn't need an API key
	private static async Task SyncAllData(bool forceDownload = false) {
		using var httpClient = new HttpClient();
		var gw2 = new Gw2Client(httpClient);

		await SyncCurrencyData(gw2, forceDownload);

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAllEvent));
	}

	//Syncs the definitions for the currencies
	private static async Task SyncCurrencyData(Gw2Client gw2, bool forceDownload = false) {
		var (currencies, _) = await gw2.Hero.Wallet.GetCurrencies();
		var currencyAdded = false;

		Directory.CreateDirectory(Reference.CurrencyIconPath);

		//Update currencies already in the list, mainly for switching languages
		foreach (var currency in currencies.Where(currency => Reference.Currencies.Any(x => x.Id == currency.Id))) {
			Reference.Currencies.First(x => x.Id == currency.Id).Update(currency);
		}

		//Add currencies not already in the list
		foreach (var newCurrency in from currency in currencies where !currency.Name.Equals(string.Empty) && Reference.Currencies.All(x => x.Id != currency.Id) select Currency.FromAPI(currency)) {
			Reference.Currencies.Add(newCurrency);
			currencyAdded = true;
		}

		//Download the image of the currencies
		//forceDownload is to download even if the file is already there
		foreach (var currency in Reference.Currencies.Where(currency => !File.Exists(currency.IconPath) || forceDownload)) {
			using var client = new WebClient();
			client.DownloadFileAsync(currency.IconUrl, currency.IconPath);
		}

		if (currencyAdded) Storage.SaveCurrencies();

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncCurrenciesEvent));
	}

	//Sync all account related data, this needs API key(s)
	public static async Task SyncAllAccountData() {
		using var httpClient = new HttpClient();
		var gw2 = new Gw2Client(httpClient);

		foreach (var account in Reference.Accounts) {
			await SyncAccountData(gw2, account);
		}

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAllAccountEvent));
	}

	//Syncs account data for a specific account
	public static async Task SyncAccountData(Gw2Client gw2, Account account) {
		await SyncAccountNameData(gw2, account);
		await SyncAccountCurrencyData(gw2, account);
		account.Initialized = true;

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAccountEvent), account.ApiKey);
	}

	//Updates the account display name
	public static async Task SyncAccountNameData(Gw2Client gw2, Account account) {
		var (summary, _) = await gw2.Hero.Account.GetSummary(account.ApiKey);

		account.AccountName = summary.DisplayName;
		
		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAccountNameEvent), account.ApiKey);
	}

	//Gets the amount of each currency held by the account
	private static async Task SyncAccountCurrencyData(Gw2Client gw2, Account account) {
		var (currencies, _) = await gw2.Hero.Wallet.GetWallet(account.ApiKey);

		foreach (var currency in currencies) {
			var newCurrency = Reference.Currencies.First(x => x.Id.Equals(currency.CurrencyId));
			account.Currencies[newCurrency] = currency.Amount;
		}

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncCurrenciesAccountEvent), account.ApiKey);
	}
}