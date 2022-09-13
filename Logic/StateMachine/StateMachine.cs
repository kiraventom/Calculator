using System.Globalization;

namespace Logic.StateMachine;

public enum State
{
	WaitingForFirstOperand,
	EnteringFirstOperandOrOperation,
	FirstOperandReachedMaxLength,
	WaitingForSecondOperand,
	EnteringSecondOperandOrEquals,
	SecondOperandReachedMaxLength,
	ShowingResult,
}

public class StateMachine
{
	private const int MaxOperandLength = 10;
	
	private readonly ObservableStringBuilder _enteredOperand;
	private readonly ObservableStringBuilder _currentOperand;
	private Input _binaryOperation = Input.None;
	private State _currentState;

	public Input BinaryOperation
	{
		get => _binaryOperation;
		private set
		{
			var oldValue = _binaryOperation;
			_binaryOperation = value;
			if (oldValue != _binaryOperation)
				OperationChanged?.Invoke();
		}
	}

	public bool ResultIsReady => Answer is not null;

	public Answer Answer { get; private set; }

	public State CurrentState
	{
		get => _currentState;
		private set
		{
			_currentState = value;
			AllowedInput = GetAllowedInput(_currentState);
		}
	}

	public Input AllowedInput { get; private set; }

	public string EnteredOperand => _enteredOperand.ToString();
	public string CurrentOperand => _currentOperand.ToString();

	public event Action EnteredOperandChanged;
	public event Action CurrentOperandChanged;
	public event Action OperationChanged;

	public StateMachine()
	{
		_enteredOperand = new ObservableStringBuilder();
		_currentOperand = new ObservableStringBuilder();

		CurrentState = State.WaitingForFirstOperand;

		_enteredOperand.ValueChanged += () => EnteredOperandChanged?.Invoke();
		_currentOperand.ValueChanged += () => CurrentOperandChanged?.Invoke();
	}

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
				_currentOperand.Append(input.AsString());
				break;

			case Input.Plus:
			case Input.Minus:
			case Input.Multiply:
			case Input.Divide:
				BinaryOperation = input;
				_enteredOperand.Append(_currentOperand);
				_currentOperand.Clear();
				break;

			case Input.Equals:
				var secondOperand = double.Parse(_currentOperand.ToString(), NumberStyles.Any);
				var firstOperand = double.Parse(_enteredOperand.ToString(), NumberStyles.Any);
				var expression = new Expression(firstOperand, secondOperand, BinaryOperation);
				Answer = Calculator.Calculate(expression);
				break;
				
			case Input.OneByX:
			case Input.Percent:
			case Input.SquarePower:
			case Input.SquareRoot:
				// TODO
				break;

			case Input.Sign:
				if (_currentOperand[0] == Input.Minus.AsChar())
					_currentOperand.Remove(0, 1);
				else
					_currentOperand.Insert(0, Input.Minus.AsChar());

				break;
				
			case Input.Clear:
				_currentOperand.Clear();

				break;
			case Input.ClearAll:
				_currentOperand.Clear();
				_enteredOperand.Clear();
				Answer = null;
				BinaryOperation = Input.None;
				break;

			case Input.Backspace:
				_currentOperand.Remove(_currentOperand.Length - 1, 1);
				if (_currentOperand.Length == 1 && _currentOperand[0] == Input.Minus.AsChar())
					_currentOperand.Clear();
				break;

			default:
				throw new NotSupportedException($"Don't know how to process input {input}");
		}

		CurrentState = GetCurrentState();
	}

	private State GetCurrentState()
	{
		return (_currentOperand.Length) switch
		{
			> 0 when _enteredOperand.Length > 0 && ResultIsReady => State.ShowingResult,
			>= MaxOperandLength when  _enteredOperand.Length == 0 => State.FirstOperandReachedMaxLength,
			>= MaxOperandLength when  _enteredOperand.Length > 0 => State.SecondOperandReachedMaxLength,
			0 when _enteredOperand.Length == 0 => State.WaitingForFirstOperand,
			0 when _enteredOperand.Length > 0 => State.WaitingForSecondOperand,
			> 0 when _enteredOperand.Length == 0 => State.EnteringFirstOperandOrOperation,
			> 0 when _enteredOperand.Length > 0 => State.EnteringSecondOperandOrEquals,
			_ => throw new NotSupportedException("Can't figure out current state")
		};
	}

	private Input GetAllowedInput(State state)
	{
		var allowedInput = Input.None;

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
				if (!_currentOperand.Contains(Input.DecimalPoint.AsChar()))
					allowedInput |= Input.DecimalPoint;

				// if operand does not end on decimal point, can use binary operation
				if (_currentOperand[^1] != Input.DecimalPoint.AsChar())
					allowedInput |= Input.BinaryOperation;

				// if operand is not equal to 0 or ends with decimal point, can add digit
				if (!_currentOperand.Equals(Input.Zero.AsString()) || _currentOperand[^1] == Input.DecimalPoint.AsChar())
					allowedInput |= Input.Digit;
				break;
				
			case State.EnteringSecondOperandOrEquals:
				allowedInput |= Input.Erasing | Input.Sign;

				// if there is no decimal point, can add decimal point
				if (!_currentOperand.Contains(Input.DecimalPoint.AsChar()))
					allowedInput |= Input.DecimalPoint;
				
				// if operand does not end on decimal point, can use equals
				if (_currentOperand[^1] != Input.DecimalPoint.AsChar())
					allowedInput |= Input.Equals;

				// if operand is not equal to 0 or ends with decimal point, can add digit
				if (!_currentOperand.Equals(Input.Zero.AsString()) || _currentOperand[^1] == Input.DecimalPoint.AsChar())
					allowedInput |= Input.Digit;
				break;

			case State.FirstOperandReachedMaxLength:
				allowedInput |= Input.Erasing | Input.Sign;

				if (_currentOperand[^1] != Input.DecimalPoint.AsChar())
					allowedInput |= Input.BinaryOperation;

				break;

			case State.SecondOperandReachedMaxLength:
				allowedInput |= Input.Erasing | Input.Sign;

				if (_currentOperand[^1] != Input.DecimalPoint.AsChar())
					allowedInput |= Input.Equals;

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