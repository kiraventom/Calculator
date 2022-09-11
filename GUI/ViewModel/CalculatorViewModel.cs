using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using Common.Utils.Exceptions;
using GUI.Annotations;
using Logic;

namespace GUI.ViewModel
{
	public abstract class BaseNotifiable : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetAndRaise<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			RaisePropertyChanged(propertyName);
			return true;
		}
	}

	public class Command<T> : ICommand
	{
		private readonly Action<T> _action;
		private readonly Predicate<T> _predicate;

		public event EventHandler CanExecuteChanged;

		public Command(Action<T> action, Predicate<T> predicate)
		{
			_action = action;
			_predicate = predicate;
		}

		public bool CanExecute(object parameter) => parameter is T t && _predicate(t);

		public void Execute(object parameter)
		{
			ArgumentTypeException.ThrowIfNotTypeOf(parameter, out T t);
			_action(t);
		}
	}

	public class CalculatorViewModel : BaseNotifiable
	{
		private string _old;
		private string _current;

		public string Old
		{
			get => _old;
			private set => SetAndRaise(ref _old, value);
		}

		public string Current
		{
			get => _current;
			private set => SetAndRaise(ref _current, value);
		}
		
		public ICommand InputCommand { get; }

		public CalculatorViewModel()
		{
			InputCommand = new Command<Input>(OnButtonPressed, IsButtonActive);
		}

		// TODO: Create state machine, move all logic to "engine"
		private void OnButtonPressed(Input input)
		{
			
		}
		
		private bool IsButtonActive(Input input)
		{
			return true;
		}
	}

	internal static class DigitInputToNumberConverter
	{
		public static int Convert(Input input)
		{
			if (!Input.Digit.HasFlag(input))
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
				_ => throw new NotSupportedException($"Input {input} has no int interpretation")
			};
		}
	}

	internal static class InputToStringConverter
	{
		public static string Convert(Input input)
		{
			if (Input.Digit.HasFlag(input))
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
				Input.Decimal => ".",
				Input.Clear => "CE",
				Input.ClearAll => "C",
				Input.Backspace => "⇍",

				_ => throw new NotSupportedException($"Input {input} has no string interpretation")
			};
		}
	}

	[ValueConversion(typeof(Input), typeof(string))]
	public class ButtonContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ArgumentTypeException.ThrowIfNotTypeOf(value, out Input input);
			return InputToStringConverter.Convert(input);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}