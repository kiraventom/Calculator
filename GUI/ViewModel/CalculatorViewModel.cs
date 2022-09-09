using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents.DocumentStructures;
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

		public ICommand NumberCommand { get; }
		public ICommand OperationCommand { get; }

		public CalculatorViewModel()
		{
			NumberCommand = new Command<int>(OnNumberPressed, CanNumberBePressed);
			OperationCommand = new Command<Operation>(OnOperationPressed, CanOperationBePressed);
		}

		// TODO: Create state machine, move all logic to "engine"
		private void OnOperationPressed(Operation operation)
		{
			switch (operation)
			{
				case Operation.Plus:
				case Operation.Minus:
				case Operation.Multiply:
				case Operation.Divide:
					Old = Current + OperationConverter.ConvertBack(operation);
					Current = null;
					break;

				case Operation.Equals:
					Old += Current + OperationConverter.ConvertBack(Operation.Equals);
					Current = Calculator.Calculate(new Expression()).Value.ToString();
					break;

				case Operation.OneByX:
					break;

				case Operation.Clear:
					Current = null;
					break;

				case Operation.ClearAll:
					Old = null;
					Current = null;
					break;

				case Operation.Backspace:
					Current = Current.Remove(Current.Length - 1, 1);
					break;

				case Operation.Percent:
					break;

				case Operation.Power:
					break;

				case Operation.Root:
					break;

				case Operation.Sign:
					break;

				case Operation.Decimal:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		// TODO: Numbers like 00000
		private void OnNumberPressed(int number)
		{
			Current += number;
		}

		private bool CanOperationBePressed(Operation operation)
		{
			return operation switch
			{
				Operation.Plus => true,
				Operation.Minus => true,
				Operation.Multiply => true,
				Operation.Divide => true,
				Operation.Equals => true,
				Operation.OneByX => false,
				Operation.Clear => true,
				Operation.ClearAll => true,
				Operation.Backspace => true,
				Operation.Percent => false,
				Operation.Power => false,
				Operation.Root => false,
				Operation.Sign => false,
				Operation.Decimal => false,
				_ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
			};
		}

		private bool CanNumberBePressed(int number)
		{
			return number is >= 0 and < 10;
		}
	}

	public class OperationConverter : IValueConverter
	{
		private static readonly OperationConverter Instance = new();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var operationString = (string)value;
			return operationString switch
			{
				"+" => Operation.Plus,
				"-" => Operation.Minus,
				"×" => Operation.Multiply,
				"÷" => Operation.Divide,
				"1÷x" => Operation.OneByX,
				"=" => Operation.Equals,
				"CE" => Operation.Clear,
				"C" => Operation.ClearAll,
				"⇍" => Operation.Backspace,
				"%" => Operation.Percent,
				"x²" => Operation.Power,
				"√x" => Operation.Root,
				"±" => Operation.Sign,
				"." => Operation.Decimal,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown operation {value}")
			};
		}

		public static Operation Convert(string parameter) => (Operation)Instance.Convert(parameter, typeof(string), null, CultureInfo.InvariantCulture);

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var operation = (Operation)value;
			return operation switch
			{
				Operation.Plus => "+",
				Operation.Minus => "-",
				Operation.Multiply => "×",
				Operation.Divide => "÷",
				Operation.OneByX => "1÷x",
				Operation.Equals => "=",
				Operation.Clear => "CE",
				Operation.ClearAll => "C",
				Operation.Backspace => "⇍",
				Operation.Percent => "%",
				Operation.Power => "x²",
				Operation.Root => "√x",
				Operation.Sign => "±",
				Operation.Decimal => ".",
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown operation {value}")
			};
		}

		public static string ConvertBack(Operation parameter) => (string)Instance.ConvertBack(parameter, typeof(string), null, CultureInfo.InvariantCulture);
	}

	public class NumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var numberString = (string)value;
			return int.TryParse(numberString, out var number) ? number : throw new NotSupportedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}