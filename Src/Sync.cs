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
	public delegate void SyncAnyEventEventHandler();
	[Signal]
	public delegate void SyncCurrenciesEventEventHandler();
	[Signal]
	public delegate void SyncAccountNameEventEventHandler();
	
	public static Sync Instance;

	public override void _Ready() {
		Instance = this;
		
		base._Ready();
	}

	public static void SyncAll() {
		Task.Run(() => SyncAllData());
		Task.Run(SyncAllAccountData);
	}

	private static async Task SyncAllData(bool forceDownload = false) {
		using var httpClient = new HttpClient();
		var gw2 = new Gw2Client(httpClient);

		await SyncCurrencyData(gw2, forceDownload);

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAllEvent));
	}

	private static async Task SyncCurrencyData(Gw2Client gw2, bool forceDownload = false) {
		var (currencies, _) = await gw2.Hero.Wallet.GetCurrencies();

		Directory.CreateDirectory(Reference.CurrencyIconPath);

		foreach (var newCurrency in from currency in currencies where !currency.Name.Equals(string.Empty) select Currency.FromAPI(currency)) {
			Reference.Currencies.Add(newCurrency);

			if (File.Exists(newCurrency.IconPath) && !forceDownload) continue;
				
			using var client = new WebClient();
			client.DownloadFileAsync(newCurrency.IconUrl, newCurrency.IconPath);
		}

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncCurrenciesEvent));
	}

	public static async Task SyncAllAccountData() {
		using var httpClient = new HttpClient();
		var gw2 = new Gw2Client(httpClient);

		foreach (var account in Reference.Accounts.Where(x => x.Enabled)) {
			await SyncAccountData(gw2, account);
		}

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAllEvent));
	}

	public static async Task SyncAccountData(Gw2Client gw2, Account account) {
		await SyncAccountNameData(gw2, account);
		await SyncAccountCurrencyData(gw2, account);
		account.Initialized = true;
	}

	public static async Task SyncAccountNameData(Gw2Client gw2, Account account) {
		var (summary, _) = await gw2.Hero.Account.GetSummary(account.ApiKey);

		account.AccountName = summary.DisplayName;
		
		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncAccountNameEvent));
	}

	private static async Task SyncAccountCurrencyData(Gw2Client gw2, Account account) {
		var (currencies, _) = await gw2.Hero.Wallet.GetWallet(account.ApiKey);

		foreach (var currency in currencies) {
			var newCurrency = Reference.Currencies.First(x => x.Id.Equals(currency.CurrencyId));
			account.Currencies.Add(newCurrency, currency.Amount);
		}

		Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(SyncCurrenciesEvent));
	}
}