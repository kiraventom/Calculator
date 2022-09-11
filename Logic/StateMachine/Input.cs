using Logic.Converters;

namespace Logic.StateMachine;

[Flags]
public enum Input
{
	None = 0,

	Digit = Zero | NotZero,
	NotZero = One | Two | Three | Four | Five | Six | Seven | Eight | Nine,
	Zero = 1,
	One = 1 << 2,
	Two = 1 << 3,
	Three = 1 << 4,
	Four = 1 << 5,
	Five = 1 << 6,
	Six = 1 << 7,
	Seven = 1 << 8,
	Eight = 1 << 9,
	Nine = 1 << 10,

	BinaryOperation = Plus | Minus | Multiply | Divide,
	Plus = 1 << 11,
	Minus = 1 << 12,
	Multiply = 1 << 13,
	Divide = 1 << 14,

	Equals = 1 << 15,

	UnaryOperation = OneByX | Percent | SquarePower | SquareRoot,
	OneByX = 1 << 16,
	Percent = 1 << 17,
	SquarePower = 1 << 18,
	SquareRoot = 1 << 19,
	
	Sign = 1 << 20,
	DecimalPoint = 1 << 21,

	Erasing = Clear | ClearAll | Backspace,
	Clear = 1 << 22,
	ClearAll = 1 << 23,
	Backspace = 1 << 24
}

public static class InputExtensions
{
	public static bool Is(this Input input, Input complexInput) => input != Input.None && complexInput.HasFlag(input);
	public static bool IsNot(this Input input, Input complexInput) => !input.Is(complexInput);

	public static string AsString(this Input input) => InputToStringConverter.Convert(input);
}