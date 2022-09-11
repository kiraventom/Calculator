using System.Globalization;

namespace Logic.StateMachine;

public enum State
{
	WaitingForFirstOperand,
	EnteringFirstOperandOrOperation,
	WaitingForSecondOperand,
	EnteringSecondOperandOrEquals,
	ShowingResult,
}

public class StateMachine
{
	// TODO: Replace operand strings with StringBuilders
	public string EnteredOperandString { get; private set; } = string.Empty;
	public string CurrentOperandString { get; private set; } = string.Empty;

	public Input BinaryOperation { get; private set; } = Input.None;
	public bool ResultIsReady { get; private set; }

	// TODO fix this
	public double EnteredOperand => double.TryParse(EnteredOperandString, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
		? result
		: double.NaN;

	public double CurrentOperand => double.TryParse(CurrentOperandString, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) 
		? result
		: double.NaN;

	public double Result { get; private set; }

	public State CurrentState => GetCurrentState();

	public Input AllowedInput => GetAllowedInput(CurrentState);

	public void ReceiveInput(Input input)
	{
		if (input.IsNot(AllowedInput))
			throw new NotSupportedException($"Input {input} is not allowed in state {CurrentState}");

		ProcessInput(input);
	}

	private void ProcessInput(Input input)
	{
		switch (input)
		{
			case Input.Zero:
			case Input.One:
			case Input.Two:
			case Input.Three:
			case Input.Four:
			case Input.Five:
			case Input.Six:
			case Input.Seven:
			case Input.Eight:
			case Input.Nine:
			case Input.DecimalPoint:
				CurrentOperandString += input.AsString();
				break;

			case Input.Plus:
			case Input.Minus:
			case Input.Multiply:
			case Input.Divide:
				BinaryOperation = input;
				EnteredOperandString = CurrentOperandString;
				CurrentOperandString = string.Empty;
				break;

			case Input.Equals:
				var expression = new Expression(EnteredOperand, CurrentOperand, BinaryOperation);
				Result = Calculator.Calculate(expression).Value;
				ResultIsReady = true;
				break;
				
			case Input.OneByX:
			case Input.Percent:
			case Input.SquarePower:
			case Input.SquareRoot:
				// TODO
				break;

			case Input.Sign:
				CurrentOperandString = CurrentOperand switch
				{
					> 0 => CurrentOperandString.Insert(0, Input.Minus.AsString()),
					< 0 => CurrentOperandString[1..],
					_ => CurrentOperandString
				};

				break;
				
			case Input.Clear:
				CurrentOperandString = string.Empty;

				break;
			case Input.ClearAll:
				CurrentOperandString = string.Empty;
				EnteredOperandString = string.Empty;
				Result = double.MinValue;
				ResultIsReady = false;
				BinaryOperation = Input.None;
				break;

			case Input.Backspace:
				CurrentOperandString = CurrentOperandString[..^1];
				break;

			default:
				throw new NotSupportedException($"Don't know how to process input {input}");
		}
	}

	private State GetCurrentState()
	{
		return (CurrentOperandString.Length == 0) switch
		{
			true when EnteredOperandString.Length == 0 => State.WaitingForFirstOperand,
			true when EnteredOperandString.Length != 0 => State.WaitingForSecondOperand,
			false when EnteredOperandString.Length == 0 => State.EnteringFirstOperandOrOperation,
			false when EnteredOperandString.Length != 0 => State.EnteringSecondOperandOrEquals,
			false when EnteredOperandString.Length != 0 && ResultIsReady => State.ShowingResult,
			_ => throw new NotSupportedException("Can't figure out current state")
		};
	}

	private Input GetAllowedInput(State state)
	{
		Input allowedInput = Input.None;

		switch (state)
		{
			case State.WaitingForFirstOperand:
				allowedInput |= Input.Digit;
				break;

			case State.WaitingForSecondOperand:
				allowedInput |= Input.Digit | Input.ClearAll;
				break;

			case State.EnteringFirstOperandOrOperation:
				allowedInput |= Input.Erasing | Input.Sign;

				// if there is no decimal point, can add decimal point
				if (!CurrentOperandString.Contains(Input.DecimalPoint.AsString()))
					allowedInput |= Input.DecimalPoint;

				// if operand does not end on decimal point, can use binary operation
				if (!CurrentOperandString.EndsWith(Input.DecimalPoint.AsString()))
					allowedInput |= Input.BinaryOperation;

				// if operand is not equal to 0 or ends with decimal point, can add digit
				if (Math.Abs(CurrentOperand) != 0 || CurrentOperandString.EndsWith(Input.DecimalPoint.AsString()))
					allowedInput |= Input.Digit;
				break;
				
			case State.EnteringSecondOperandOrEquals:
				allowedInput |= Input.Erasing | Input.Sign;

				// if there is no decimal point, can add decimal point
				if (!CurrentOperandString.Contains(Input.DecimalPoint.AsString()))
					allowedInput |= Input.DecimalPoint;

				// TODO fix division on zero
				// if operand does not end on decimal point, can use equals
				if (!CurrentOperandString.EndsWith(Input.DecimalPoint.AsString()))
					allowedInput |= Input.Equals;

				// if operand is not equal to 0 or ends with decimal point, can add digit
				if (Math.Abs(CurrentOperand) != 0 || CurrentOperandString.EndsWith(Input.DecimalPoint.AsString()))
					allowedInput |= Input.Digit;
				break;

			case State.ShowingResult:
				allowedInput |= Input.ClearAll;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}

		return allowedInput;
	}
}