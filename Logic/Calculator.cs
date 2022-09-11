using Logic.StateMachine;

namespace Logic
{
	public static class Calculator
	{
		public static Answer Calculate(Expression expression)
		{
			var result = expression.Operation switch
			{
				Input.Plus => expression.FirstOperand + expression.SecondOperand,
				Input.Minus => expression.FirstOperand - expression.SecondOperand,
				Input.Multiply => expression.FirstOperand * expression.SecondOperand,
				Input.Divide => expression.FirstOperand / expression.SecondOperand,
				_ => throw new NotSupportedException($"Unknown binary operation '{expression.Operation}'")
			};

			return new Answer(result);
		}
	}

	public class Answer
	{
		public double Value { get; }

		internal Answer(double value)
		{
			Value = value;
		}
	}

	public class Expression
	{
		public double FirstOperand { get; }
		public double SecondOperand { get; }
		public Input Operation { get; }

		internal Expression(double firstOperand, double secondOperand, Input operation)
		{
			FirstOperand = firstOperand;
			SecondOperand = secondOperand;
			Operation = operation;
		}
	}
}