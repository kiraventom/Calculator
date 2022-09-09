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
		private Operation _operation;

		public void SetFirstOperand(decimal number) => _first = number;

		public void SetSecondOperand(decimal number) => _second = number;

		public void SetOperation(Operation operation) => _operation = operation;
	}

	public enum Result
	{
		Success,
		DivideByZero,
		IncompleteExpression
	}

	public enum Operation
	{
		Plus,
		Minus,
		Multiply,
		Divide,
		Equals,
		OneByX,
		Clear,
		ClearAll,
		Backspace,
		Percent,
		Power,
		Root,
		Sign,
		Decimal
	}
}