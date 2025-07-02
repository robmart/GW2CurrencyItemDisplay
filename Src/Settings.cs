using System;
using System.ComponentModel;
using System.Linq;
using Godot;

namespace GW2NotionSync;

public partial class Settings : Control {
	public static bool ShouldDisplayCurrencyIcon => Reference.Settings.First(x => x.Name.Equals("CurrencyDisplayMode")).Value is CurrencyDisplayMode.NameAndIcon or CurrencyDisplayMode.Icon;
	public static bool ShouldDisplayCurrencyName => Reference.Settings.First(x => x.Name.Equals("CurrencyDisplayMode")).Value is CurrencyDisplayMode.NameAndIcon or CurrencyDisplayMode.Name;
	public static bool HideEmptyCurrencies => (bool)Reference.Settings.First(x => x.Name.Equals("HideEmptyCurrency")).Value;

	public override void _Ready() {
		base._Ready();
		
		Reference.Settings.Add(new Setting { Name = "CurrencyDisplayMode", Type = SettingType.Enum, EnumType = typeof(CurrencyDisplayMode), Category = SettingCategory.Currency, Value = CurrencyDisplayMode.NameAndIcon });
		Reference.Settings.Add(new Setting { Name = "HideEmptyCurrency", Type = SettingType.Bool, Value = false, Category = SettingCategory.Currency });
	}
}

public class Setting {
	private object _value;
	private readonly Type _enumType;

	public required string Name { get; init; }
	public required SettingType Type { get; init; }
	public required SettingCategory Category { get; init; }

	public Type EnumType {
		get => _enumType;
		init {
			if (!value.IsEnum) throw new ArgumentException("EnumType must be a valid enum");
			_enumType = value;
		}
	}

	public Setting() {
		if (Type.Equals(SettingType.Enum) && EnumType == null) {
			throw new ArgumentException("Enum must be set");
		}
	}

	public object Value {
		get => _value;
		set {
			if (value == null) throw new ArgumentException("Value must not be null");
			switch (Type) {
				case SettingType.Bool:
					if (value is not bool) throw new ArgumentException("Value should be a bool"); 
					break;
				case SettingType.Enum:
					if (!Enum.IsDefined(EnumType, value)) throw new ArgumentException($"{value} is not defined in {EnumType.Name}"); 
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			_value = value;
		}
	}
}

public enum SettingType {
	Bool,
	Enum
}

public enum SettingCategory {
	Account,
	Currency
}

public enum CurrencyDisplayMode {
	Name,
	Icon,
	NameAndIcon
}