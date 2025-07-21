using System.Collections.Generic;

namespace GW2NotionSync.Extension;

public static class StringExtensions {
	public static string UpperCamelToTitleCase(this string str) {
		var result = new List<char>();
		var chars = str.Replace(" ", "").ToCharArray();

		for (var i = 0; i < chars.Length; i++)
		{
			if (i != 0 && char.IsUpper(chars[i]))
			{
				result.Add(' ');
			}

			result.Add(chars[i]);
		}

		return string.Concat(result);
	}
}