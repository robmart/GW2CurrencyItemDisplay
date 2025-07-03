using System.Collections.Generic;
using Godot;
using GW2NotionSync.Data;

namespace GW2NotionSync;

public static class Reference {
	// Paths
	public static string UserPath = ProjectSettings.GlobalizePath("user://");
	public static string DataPath = $"{UserPath}/Data/";
	public static string IconPath = $"{DataPath}/Icons/";
	public static string CurrencyIconPath = $"{IconPath}/Currencies/";
	// --Save Paths
	public static string CurrencySavePath = "user://Currencies.save";
	public static string AccountSavePath = "user://Accounts.save";
	
	// Data
	public static List<Account> Accounts = new List<Account>();
	public static List<Currency> Currencies = new List<Currency>();
	public static List<Setting> Settings = new List<Setting>();
}