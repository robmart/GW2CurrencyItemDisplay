using Godot;

namespace GW2NotionSync;

public partial class Settings : Control {
	public static CurrencyDisplayMode CurrencyDisplayMode { get; set; } = CurrencyDisplayMode.NameAndIcon;
	public static bool HideEmptyCurrencies { get; set; } = true;
	
	public static bool ShouldDisplayCurrencyIcon => CurrencyDisplayMode is CurrencyDisplayMode.NameAndIcon or CurrencyDisplayMode.Icon;
	public static bool ShouldDisplayCurrencyName => CurrencyDisplayMode is CurrencyDisplayMode.NameAndIcon or CurrencyDisplayMode.Name;
}

public enum CurrencyDisplayMode {
	Name,
	Icon,
	NameAndIcon
}