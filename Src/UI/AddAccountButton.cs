using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuildWars2;
using GuildWars2.Authorization;
using GuildWars2.Http;
using GW2NotionSync;
using GW2NotionSync.Data;
using HttpClient = System.Net.Http.HttpClient;

public partial class AddAccountButton : ModulatingButton {
	[Export] public TextEdit Nickname;
	[Export] public TextEdit APIKey;
	[Export] public Label ErrorLabel;
	[Export] public Container AccountContainer;
	[Export] public PackedScene AccountDisplay;
	
	public override void _Pressed() {
		base._Pressed();
		ErrorLabel.Text = "";
		
		if (APIKey.Text == "") {
			Error("Error: API key is empty");
		} else if (Reference.Accounts.Any(x => x.ApiKey == APIKey.Text)) {
			Error("Error: API key is already registered");
		} else {
			Info("Info: Testing API key");

			Task.Run(() => AddNewAPIKey(APIKey.Text));
		}
	}

	private async Task AddNewAPIKey(string key) {
		//Make UI elements unavailable while adding the key
		CallDeferred(BaseButton.MethodName.SetDisabled, true);
		Nickname.CallDeferred(TextEdit.MethodName.SetEditable, false);
		APIKey.CallDeferred(TextEdit.MethodName.SetEditable, false);
		
		using var httpClient = new HttpClient();
		var gw2 = new Gw2Client(httpClient);
		
		var validKey = await TestAPIKey(gw2, key);

		if (validKey) {
			var newAccount = new Account() { ApiKey = key, Enabled = true };
			if (!Nickname.Text.Equals(""))
				newAccount.Nickname = Nickname.Text;
			Reference.Accounts.Add(newAccount);
			await Sync.SyncAccountData(gw2, newAccount);
			//Add a new UI element for the account
			var accountDisplay = AccountDisplay.Instantiate<AccountDisplay>();
			accountDisplay.Account = newAccount;
			AccountContainer.CallDeferred(Node.MethodName.AddChild, accountDisplay);
			
			Storage.SaveAccounts();
			
			//Clear text boxes
			Nickname.CallDeferred(TextEdit.MethodName.Clear);
			APIKey.CallDeferred(TextEdit.MethodName.Clear);
			Success("Success: New account successfully added");
		}
		
		//Make UI elements available again
		CallDeferred(BaseButton.MethodName.SetDisabled, false);
		Nickname.CallDeferred(TextEdit.MethodName.SetEditable, true);
		APIKey.CallDeferred(TextEdit.MethodName.SetEditable, true);
	}

	private async Task<bool> TestAPIKey(Gw2Client gw2, string key) {
		try {
			await gw2.Tokens.GetTokenInfo(key);
			
			Info("Info: API key valid, adding");
			return true;
		}
		//Should only happen when there's a bad key
		catch (BadResponseException e) {
			Error("Error: API key invalid, aborting");
			return false;
		}
	}

	private void Info(string message) {
		ErrorLabel.CallDeferred(Label.MethodName.SetText, message);
		ErrorLabel.CallDeferred(Control.MethodName.AddThemeColorOverride, "font_color", Colors.White);
	}

	private void Success(string message) {
		ErrorLabel.CallDeferred(Label.MethodName.SetText, message);
		ErrorLabel.CallDeferred(Control.MethodName.AddThemeColorOverride, "font_color", Colors.LightGreen);
	}

	private void Error(string message) {
		ErrorLabel.CallDeferred(Label.MethodName.SetText, message);
		ErrorLabel.CallDeferred(Control.MethodName.AddThemeColorOverride, "font_color", Colors.Red);
	}
}
