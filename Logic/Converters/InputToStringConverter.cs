using Logic.StateMachine;

namespace Logic.Converters;

public static class InputToStringConverter
{
	public static string Convert(Input input)
	{
		if (input == Input.None)
			return string.Empty;

		if (input.Is(Input.Digit))
			return DigitInputToNumberConverter.Convert(input).ToString();

		return input switch
		{
			Input.Plus => "+",
			Input.Minus => "-",
			Input.Multiply => "×",
			Input.Divide => "÷",
			Input.Equals => "=",
			Input.OneByX => "1÷x",
			Input.Percent => "%",
			Input.SquarePower => "x²",
			Input.SquareRoot => "√x",
			Input.Sign => "±",
			Input.DecimalPoint => ".",
			Input.Clear => "CE",
			Input.ClearAll => "C",
			Input.Backspace => "⇍",

			_ => throw new NotSupportedException($"Input '{input}' has no string interpretation")
		};
	}
}