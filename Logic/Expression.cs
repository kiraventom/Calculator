namespace Logic
{
	public static class Calculator
	{
		public static Answer Calculate(Expression expression)
		{
			return new Answer(Result.Success, 42);
		}
	}

	public class Answer
	{
		public decimal Value { get; }
		public Result Result { get; }

		internal Answer(Result result, decimal value)
		{
			Result = result;
			Value = value;
		}
	}

	public class Expression
	{
		private decimal _first;
		private decimal _second;
		private Input _input;

		internal Expression()
		{
			
		}

		public void SetFirstOperand(decimal number) => _first = number;

		public void SetSecondOperand(decimal number) => _second = number;

		public void SetOperation(Input input) => _input = input;
	}

	public enum Result
	{
		Success,
		DivideByZero,
		IncompleteExpression
	}

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

		UnaryOperation = Equals | OneByX | Percent | SquarePower | SquareRoot,
		Equals = 1 << 15,
		OneByX = 1 << 16,
		Percent = 1 << 17,
		SquarePower = 1 << 18,
		SquareRoot = 1 << 19,

		Modifier = Sign | Decimal,
		Sign = 1 << 20,
		Decimal = 1 << 21,

		Erasing = Clear | ClearAll | Backspace,
		Clear = 1 << 22,
		ClearAll = 1 << 23,
		Backspace = 1 << 24
	}
}