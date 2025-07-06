using Godot;
using System;
using System.Threading.Tasks;
using GW2NotionSync;

public partial class RefreshButton : TextureButton {
	[Export] public Control HiddenWhenVisible { get; set; }
	[Export] public AnimatedSprite2D Sprite { get; set; }

	public override void _Ready() {
		base._Ready();

		Sync.Instance.StartSyncEvent += _StartSync;
		Sprite.AnimationLooped += _AnimationLoop;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		Visible = !HiddenWhenVisible.Visible;
	}

	public override void _Pressed() {
		base._Pressed();

		if (!Sync.SyncControl.IsSyncing) {
			Task.Run(Sync.SyncAllAccountData);
		}
	}

	private void _StartSync() {
		Sprite.Play();
	}

	private void _AnimationLoop() {
		if (!Sync.SyncControl.IsSyncing) Sprite.Pause();
	}
}
