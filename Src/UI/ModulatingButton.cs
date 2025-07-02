using Godot;
using System;

public partial class ModulatingButton : TextureButton {
	public Color NormalColor = Colors.White;
	[Export] public Color PressedColor = Colors.White;
	[Export] public Color HoverColor = Colors.White;
	[Export] public Color DisabledColor = Colors.White;
	[Export] public Color HoverPressedColor = Colors.White;
	[Export] public Control Tab;
	[Export] public Control Tabs;

	public override void _Ready() {
		base._Ready();

		NormalColor = Modulate;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (IsDisabled()) {
			Modulate = DisabledColor;
		} else if (IsHovered() && IsPressed()) {
			Modulate = HoverPressedColor;
		} else if (IsPressed()) {
			Modulate = PressedColor;
		} else if (IsHovered()) {
			Modulate = HoverColor;
		} else {
			Modulate = NormalColor;	
		}

		if (Tab != null) {
			SetPressedNoSignal(Tab.Visible);
			Disabled = Tab.Visible;
		}
	}

	public override void _Pressed() {
		base._Pressed();
		
		if (Tab == null) return;
		Tab.Visible = true;
		
		if (Tabs == null) return;
		foreach (var child in Tabs.GetChildren()) {
			if (child == Tab || child is not CanvasItem canvas) continue;
			canvas.Visible = false;
		}
	}
}
