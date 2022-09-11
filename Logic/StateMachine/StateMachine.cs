namespace Logic.StateMachine;

public class StateMachine
{
	public State Current { get; private set; } = State.WaitingForFirstOperand;

	//public IEnumerable<Input> AllowedOperations => GetAllowedOperations(Current);

	//private static IEnumerable<Input> GetAllowedOperations(State state)
	//{
	//	return state switch
	//	{
	//		State.WaitingForFirstOperand => Utils.AsIEnumerable(Op),
	//		State.EnteringFirstOperandOrOperation => expr,
	//		State.WaitingForSecondOperand => expr,
	//		State.EnteringSecondOperandOrEquals => publi,
	//		State.ShowingResult => expr,
	//		_ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
	//	};
	//}
}

public enum State
{
	WaitingForFirstOperand,
	EnteringFirstOperandOrOperation,
	WaitingForSecondOperand,
	EnteringSecondOperandOrEquals,
	ShowingResult,
}

internal static class Utils
{
	public static IEnumerable<T> AsIEnumerable<T>(params T[] elements) => elements;
}