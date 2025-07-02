using Godot;
using System;
using System.Text.RegularExpressions;

public partial class ApiKeyEdit : TextEdit {
	public override void _Ready() {
		base._Ready();

		TextChanged += _TextChanged;
	}

	private void _TextChanged() {
		var caretPos = GetCaretColumn();
		//Remove characters invalid in an API key
		Text = Regex.Replace(Text, "[^a-zA-Z0-9-]", "");
		//Make all characters upper case
		Text = Text.ToUpper();
		//Cull the length the the length of an API key
		if (Text.Length > 72) Text = Text.Substring(0, 72);
		SetCaretColumn(caretPos);
	}
}
