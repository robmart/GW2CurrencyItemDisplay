using System.Collections.Generic;
using GuildWars2.Hero.Wallet;

namespace GW2NotionSync.Data;

public class Account {
	public bool Enabled { get; set; } = true;
	public string Nickname { get; set; } = "";
	public required string ApiKey { get; init; }
	
	//Data
	public bool Initialized { get; internal set; }
	public string AccountName { get; internal set; }
	public Dictionary<Currency, int> Currencies { get; } = new();
}