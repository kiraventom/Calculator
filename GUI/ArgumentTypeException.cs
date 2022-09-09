using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace Common.Utils.Exceptions
{
    public class ArgumentTypeException : Exception
    {
        private string _message;

        private ArgumentTypeException(MemberInfo expectedType, MemberInfo actualType)
        {
            ArgumentNullException.ThrowIfNull(expectedType);
            ExpectedTypeName = expectedType.Name;
            ActualTypeName = actualType?.Name;
        }

        private ArgumentTypeException(string argumentName, MemberInfo expectedType, MemberInfo actualType)
            : this(expectedType, actualType)
        {
            ArgumentName = argumentName;
        }

        public string ExpectedTypeName { get; }

        public string ActualTypeName { get; }

        public string ArgumentName { get; }

        public override string Message
        {
            get
            {
                if (_message is null)
                {
                    var builder = new StringBuilder("Incorrect argument type");
                    if (ExpectedTypeName is not null)
                    {
                        builder.Append(". Expected: '").Append(ExpectedTypeName).Append('\'');
                        if (ActualTypeName is not null)
                        {
                            builder.Append(", actual: '").Append(ActualTypeName).Append('\'');
                        }
                    }

                    if (ArgumentName is not null)
                    {
                        builder.Append(". Argument name: '").Append(ArgumentName).Append('\'');
                    }

                    _message = builder.ToString();
                }

                return _message;
            }
        }

        public static void ThrowIfNotTypeOf<T>(object argument, out T actual, [CallerArgumentExpression("argument")] string argumentName = null)
        {
            if (argument is not T t)
            {
                if (argumentName is null)
                    throw new ArgumentTypeException(typeof(T), argument?.GetType());
                else
                    throw new ArgumentTypeException(argumentName, typeof(T), argument?.GetType());
            }

            actual = t;
        }
    }
}
