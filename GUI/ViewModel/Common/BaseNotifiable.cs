using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GUI.Annotations;

namespace GUI.ViewModel.Common;

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