using Godot;
using System;
using GW2NotionSync;
using GW2NotionSync.Data;

public partial class CurrencyDisplay : VBoxContainer {
	[Export] public RichTextLabel Name;
	[Export] public CheckBox Enabled;
	
	public Currency Currency { get; set; }
	private bool IsSetup { get; set; } = false;

	public override void _Ready() {
		base._Ready();
		
		Enabled.Pressed += _EnableToggled;
	}

	public override void _Process(double delta) {
		base._Process(delta);
		
		// Only runs once when the currency is set
		if (IsSetup || Currency == null) return;
		
		// Sets all the currency data in the UI
		Enabled.ButtonPressed = Currency.Enabled;
		IsSetup = true;
		var image = new ImageTexture();
		image.SetImage(Image.LoadFromFile(Currency.IconPath));
		Name.AddImage(image, 32, 32);
		Name.AppendText($" {Currency.Name}");
	}

	private void _EnableToggled() {
		if (Currency == null) return;
		
		Currency.Enabled = Enabled.ButtonPressed;
		Sync.Instance.CallDeferred(GodotObject.MethodName.EmitSignal, nameof(Sync.Instance.SyncCurrenciesEvent));
		Storage.SaveCurrencies();
	}
}
