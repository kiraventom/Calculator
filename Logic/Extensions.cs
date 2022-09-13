using System.Text;
using Logic.Converters;
using Logic.StateMachine;

namespace Logic;

public static class Extensions
{
	public static bool Is(this Input input, Input complexInput) => input != Input.None && complexInput.HasFlag(input);
	public static bool IsNot(this Input input, Input complexInput) => !input.Is(complexInput);

	public static string AsString(this Input input) => InputToStringConverter.Convert(input);

	public static char AsChar(this Input input)
	{
		var asString = input.AsString();
		return asString.Length > 1
			? throw new NotSupportedException("Can't convert to char")
			: asString[0];
	}
}