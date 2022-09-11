using System;
using System.Windows.Input;
using Common.Utils.Exceptions;

namespace GUI.ViewModel.Common;

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

	public void Execute(object parameter)
	{
		ArgumentTypeException.ThrowIfNotTypeOf(parameter, out T t);
		_action(t);
	}

	public bool CanExecute(object parameter) => parameter is T t && _predicate(t);

	public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}