using Godot;
using System;
using GW2NotionSync;

public partial class Startup : Node {
	public override void _Ready() {
		base._Ready();
		
		Storage.LoadAll();
		Sync.SyncAll();
	}
}
