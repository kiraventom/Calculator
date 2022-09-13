using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.ViewModel.Common
{
	public class IntTextBox : TextBox
	{
		public static readonly DependencyProperty MinValueProperty = 
			DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(IntTextBox));

		public static readonly DependencyProperty MaxValueProperty = 
			DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(IntTextBox));

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register(nameof(Value), typeof(int), typeof(IntTextBox), 
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

		public int MinValue
		{
			get => (int)GetValue(MinValueProperty);
			set => SetValue(MinValueProperty, value);
		}

		public int MaxValue
		{
			get => (int)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}

		public int Value
		{
			get => (int)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public IntTextBox()
		{
			DataObject.AddPastingHandler(this, OnPaste);
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			e.Handled = !int.TryParse(Text + e.Text, out var number) || !IsInLimits(number);
		}

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			Value = int.Parse(Text);
		}

		private bool IsInLimits(int number) => number >= MinValue && number <= MaxValue;

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var tb = (TextBox)d;
			tb.Text = ((int)e.NewValue).ToString();
		}

		private static void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				bool isInt = int.TryParse((string)e.DataObject.GetData(typeof(string)), out _);
				if (!isInt)
					e.CancelCommand();
			}
		}
	}
}
