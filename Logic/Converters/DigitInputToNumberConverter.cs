using Logic.StateMachine;

namespace Logic.Converters;

public static class DigitInputToNumberConverter
{
	public static int Convert(Input input)
	{
		if (input.IsNot(Input.Digit))
			throw new NotSupportedException($"{input} is not a {Input.Digit}");

		return input switch
		{
			Input.Zero => 0,
			Input.One => 1,
			Input.Two => 2,
			Input.Three => 3,
			Input.Four => 4,
			Input.Five => 5,
			Input.Six => 6,
			Input.Seven => 7,
			Input.Eight => 8,
			Input.Nine => 9,
			_ => throw new NotSupportedException($"Input '{input}' has no int interpretation")
		};
	}
}