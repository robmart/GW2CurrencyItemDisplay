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
		
		if (IsSetup || Currency == null) return;
		
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
	}
}
