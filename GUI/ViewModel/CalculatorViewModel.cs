using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using Common.Utils.Exceptions;
using GUI.ViewModel.Common;
using Logic.StateMachine;

namespace GUI.ViewModel
{
	public class CalculatorViewModel : BaseNotifiable
	{
		private readonly StateMachine _stateMachine;
		private readonly Command<Input> _inputCommand;

		public string EnteredValue
		{
			get
			{
				var str = _stateMachine.EnteredOperandString + ' ' + _stateMachine.BinaryOperation.AsString();

				if (_stateMachine.ResultIsReady) 
					str += ' ' + _stateMachine.CurrentOperandString;

				return str;
			}
		}

		public string CurrentValue => _stateMachine.ResultIsReady ? _stateMachine.Result.ToString(CultureInfo.InvariantCulture) : _stateMachine.CurrentOperandString;

		public ICommand InputCommand => _inputCommand;

		public CalculatorViewModel()
		{
			_stateMachine = new StateMachine();
			_inputCommand = new Command<Input>(OnButtonPressed, IsButtonActive);
		}
		
		private void OnButtonPressed(Input input)
		{
			_stateMachine.ReceiveInput(input);
			RaisePropertyChanged(nameof(EnteredValue));
			RaisePropertyChanged(nameof(CurrentValue));
			_inputCommand.RaiseCanExecuteChanged();
		}

		private bool IsButtonActive(Input input) => _stateMachine.AllowedInput.HasFlag(input);
	}

	[ValueConversion(typeof(Input), typeof(string))]
	public class ButtonContentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ArgumentTypeException.ThrowIfNotTypeOf(value, out Input input);
			return input.AsString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}