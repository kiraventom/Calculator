using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Logic;

public class ObservableStringBuilder
{
	private readonly StringBuilder _stringBuilder = new();

	public event Action ValueChanged;

	public char this[int index] => _stringBuilder[index];
	public int Length => _stringBuilder.Length;

	public ObservableStringBuilder Append(object value)
	{
		var oldLength = _stringBuilder.Length;
		_stringBuilder.Append(value);
		if (_stringBuilder.Length != oldLength)
			ValueChanged?.Invoke();

		return this;
	}

	public ObservableStringBuilder Insert(int index, object value)
	{
		var oldLength = _stringBuilder.Length;
		_stringBuilder.Insert(index, value);
		if (_stringBuilder.Length != oldLength)
			ValueChanged?.Invoke();
		
		return this;
	}

	public ObservableStringBuilder Remove(int start, int length)
	{
		var oldLength = _stringBuilder.Length;
		_stringBuilder.Remove(start, length);
		if (_stringBuilder.Length != oldLength)
			ValueChanged?.Invoke();

		return this;
	}

	public void Clear()
	{
		var oldLength = _stringBuilder.Length;
		_stringBuilder.Clear();
		if (_stringBuilder.Length != oldLength)
			ValueChanged?.Invoke();
	}

	public bool Contains(char ch)
	{
		for (int i = 0; i < Length; i++)
		{
			if (this[i] == ch)
				return true;
		}

		return false;
	}

	public override string ToString() => _stringBuilder.ToString();
	
	public bool Equals(string str) => _stringBuilder.Equals(str);

	public override int GetHashCode() => _stringBuilder.GetHashCode();
}