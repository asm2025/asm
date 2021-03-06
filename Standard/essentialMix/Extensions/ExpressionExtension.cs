using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using essentialMix.Collections;
using JetBrains.Annotations;
using essentialMix.Linq.Expressions;
using essentialMix.Reflection;

namespace essentialMix.Extensions
{
	public static class ExpressionExtension
	{
		private static readonly IReadOnlySet<ExpressionType> __convertExpressions = new ReadOnlySet<ExpressionType>(new HashSet<ExpressionType>
		{
			ExpressionType.Convert
			, ExpressionType.ConvertChecked
			, ExpressionType.TypeAs
			, ExpressionType.Unbox
		});

		private static readonly IReadOnlySet<ExpressionType> __unaryExpressions = __convertExpressions.Union(new []
		{
			ExpressionType.Decrement
			, ExpressionType.Increment
			, ExpressionType.IsFalse
			, ExpressionType.IsTrue
			, ExpressionType.Negate
			, ExpressionType.NegateChecked
			, ExpressionType.Not
			, ExpressionType.OnesComplement
			, ExpressionType.PostDecrementAssign
			, ExpressionType.PostIncrementAssign
			, ExpressionType.PreDecrementAssign
			, ExpressionType.PreIncrementAssign
			, ExpressionType.Quote
			, ExpressionType.Throw
			, ExpressionType.UnaryPlus
		}).AsReadOnlySet();

		private static readonly IReadOnlySet<ExpressionType> __simplifyExpressions = __convertExpressions.Union(new []
		{
			ExpressionType.Lambda
			, ExpressionType.Invoke
		}).AsReadOnlySet();

		private class ArgumentExtractor : ExpressionVisitor
		{
			private readonly List<object> _list = new List<object>();
			private readonly Expression _expression;

			/// <inheritdoc />
			private ArgumentExtractor([NotNull] Expression expression)
			{
				_expression = expression;
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				foreach (Expression argument in node.Arguments)
				{
					object obj = GetValue(argument);
					_list.Add(obj);
				}

				return node;
			}

			[NotNull]
			private IReadOnlyCollection<object> ExtractValues()
			{
				_list.Clear();
				Visit(_expression);
				return _list.AsReadOnly();
			}

			public static IReadOnlyCollection<object> ExtractValues(Expression expression)
			{
				expression = expression.RemoveUnary();
				if (expression == null)
					return null;
				ArgumentExtractor visitor = new ArgumentExtractor(expression);
				return visitor.ExtractValues();
			}

			private static object GetValue(Expression node)
			{
				return GetExpressionLocal(node);

				object GetExpressionLocal(Expression expression)
				{
					expression = Simplify(expression);
					if (expression == null)
						return null;

					if (node is IArgumentProvider provider)
					{
						List<object> list = new List<object>(provider.ArgumentCount);

						for (int i = 0; i < provider.ArgumentCount; i++)
						{
							Expression exp = provider.GetArgument(i);
							list.Add(GetExpressionLocal(exp));
						}

						return list;
					}

					object value;

					try
					{
						value = Compile(expression)();
					}
					catch
					{
						value = expression.Type.Default();
					}

					return value;
				}
			}

			[NotNull]
			private static Func<object> Compile([NotNull] Expression expression)
			{
				return Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object))).Compile();
			}
		}

		public static Expression RemoveConvert(this Expression thisValue) { return Simplify(thisValue, __convertExpressions); }
		
		public static Expression RemoveUnary(this Expression thisValue) { return Simplify(thisValue, __unaryExpressions); }

		public static Expression Simplify(this Expression thisValue) { return Simplify(thisValue, __simplifyExpressions); }
		public static Expression Simplify(this Expression thisValue, [NotNull] params ExpressionType[] valuesToRemove) { return valuesToRemove.Length == 0 ? thisValue : Simplify(thisValue, valuesToRemove.AsReadOnlySet()); }
		public static Expression Simplify(this Expression thisValue, [NotNull] IEnumerable<ExpressionType> valuesToRemove) { return Simplify(thisValue, valuesToRemove.AsReadOnlySet()); }
		public static Expression Simplify(this Expression thisValue, [NotNull] IReadOnlySet<ExpressionType> valuesToRemove)
		{
			if (valuesToRemove.Count == 0) return thisValue;

			while (thisValue != null && valuesToRemove.Contains(thisValue.NodeType))
			{
				switch (thisValue.NodeType)
				{
					case ExpressionType.ArrayLength:
					case ExpressionType.Convert:
					case ExpressionType.ConvertChecked:
					case ExpressionType.Decrement:
					case ExpressionType.Increment:
					case ExpressionType.IsFalse:
					case ExpressionType.IsTrue:
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
					case ExpressionType.Not:
					case ExpressionType.OnesComplement:
					case ExpressionType.PostDecrementAssign:
					case ExpressionType.PostIncrementAssign:
					case ExpressionType.PreDecrementAssign:
					case ExpressionType.PreIncrementAssign:
					case ExpressionType.Quote:
					case ExpressionType.Throw:
					case ExpressionType.TypeAs:
					case ExpressionType.UnaryPlus:
					case ExpressionType.Unbox:
						thisValue = ((UnaryExpression)thisValue).Operand;
						break;
					case ExpressionType.Block:
						thisValue = ((BlockExpression)thisValue).Result;
						break;
					case ExpressionType.Goto:
						thisValue = ((GotoExpression)thisValue).Value;
						break;
					case ExpressionType.Invoke:
						thisValue = ((InvocationExpression)thisValue).Expression;
						break;
					case ExpressionType.Lambda:
						thisValue = ((LambdaExpression)thisValue).Body;
						break;
					case ExpressionType.ListInit:
						thisValue = ((ListInitExpression)thisValue).NewExpression;
						break;
					case ExpressionType.Loop:
						thisValue = ((LoopExpression)thisValue).Body;
						break;
					case ExpressionType.MemberAccess:
						thisValue = ((MemberExpression)thisValue).Expression;
						break;
					case ExpressionType.MemberInit:
						thisValue = ((MemberInitExpression)thisValue).NewExpression;
						break;
					case ExpressionType.Switch:
						thisValue = ((SwitchExpression)thisValue).SwitchValue;
						break;
					case ExpressionType.Try:
						thisValue = ((TryExpression)thisValue).Body;
						break;
					case ExpressionType.TypeEqual:
						thisValue = ((TypeBinaryExpression)thisValue).Expression;
						break;
					case ExpressionType.TypeIs:
						thisValue = ((TypeBinaryExpression)thisValue).Expression;
						break;
					default:
						goto ExitLoop;
				}
			}

			ExitLoop:
			return thisValue;
		}

		[NotNull]
		public static IEnumerable<Expression> Enumerate(this Expression thisValue, Predicate<Expression> predicate = null)
		{
			if (thisValue == null)
				return Enumerable.Empty<Expression>();

			List<Type> types = new List<Type>();
			List<Expression> expressions = EnumerateLocal(thisValue, t =>
			{
				if (types.Count > 0 && types[types.Count - 1] == t)
					return;
				types.Add(t);
			})
			.Where(e => e.NodeType == ExpressionType.Parameter || e.NodeType == ExpressionType.MemberAccess)
			.ToList();

			if (expressions.Count == 0)
				return expressions;

			if (types.Count == 0)
			{
				return predicate == null
							? expressions
							: expressions.Where(e => predicate(e));
			}

			types.ForEach((root, i) =>
			{
				Expression x = expressions[i];
				if (root != x.Type)
					throw new MemberAccessException("Member type mismatch.");
				if (x.NodeType != ExpressionType.MemberAccess)
					return;
				MemberExpression mx = (MemberExpression)x;
				Type previous = i > 0 ? types[i - 1] : null;
				if (previous != null && mx.Expression?.Type != previous)
					throw new MemberAccessException("Member type mismatch.");
			});

			return predicate == null ? expressions : expressions.Where(e => predicate(e));

			static IEnumerable<Expression> EnumerateLocal(Expression expression, Action<Type> onNewRootType)
			{
				if (expression == null)
					yield break;

				switch (expression.NodeType)
				{
					case ExpressionType.Convert:
					case ExpressionType.ConvertChecked:
					{
						UnaryExpression unaryExpression = (UnaryExpression)expression;

						foreach (Expression x in EnumerateLocal(unaryExpression.Operand, onNewRootType))
							yield return x;

						break;
					}
					case ExpressionType.Lambda:
					{
						LambdaExpression lambdaExpression = (LambdaExpression)expression;

						if (lambdaExpression.Parameters.Count == 1)
						{
							onNewRootType(lambdaExpression.Parameters[0].Type);

							foreach (Expression x in EnumerateLocal(lambdaExpression.Body, onNewRootType))
								yield return x;
						}

						break;
					}
					case ExpressionType.Call:
					{
						MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

						if (methodCallExpression.Method.Name == "Select"
							&& methodCallExpression.Arguments.Count == 2
							&& methodCallExpression.Arguments[1]?.NodeType == ExpressionType.Lambda)
						{
							foreach (Expression argument in methodCallExpression.Arguments)
							{
								foreach (Expression x in EnumerateLocal(argument, onNewRootType))
									yield return x;
							}
						}
						break;
					}
					case ExpressionType.Parameter:
					{
						onNewRootType(expression.Type);
						yield return expression;
						break;
					}
					case ExpressionType.MemberAccess:
					{
						MemberExpression memberExpression = (MemberExpression)expression;
						Expression mx = memberExpression.Expression ?? /* static class.member access */ throw new MemberAccessException("Expression contains access to a static type member.");
						// new class().member access
						if (mx.NodeType == ExpressionType.New)
							throw new MemberAccessException("Expression contains access to an irrelevant instance.");

						foreach (Expression x in EnumerateLocal(mx, onNewRootType))
							yield return x;

						onNewRootType(memberExpression.Type);
						yield return memberExpression;
						break;
					}
				}
			}
		}

		[NotNull]
		public static IEnumerable<MemberInfo> Members(this Expression thisValue, Predicate<MemberInfo> predicate = null)
		{
			IEnumerable<MemberInfo> enumerable = Enumerate(thisValue)
				.SkipWhile((e, i) => e.NodeType != ExpressionType.Parameter || i < 1)
				.Where(e => e.NodeType == ExpressionType.MemberAccess)
				.Cast<MemberExpression>()
				.Select(e => e.Member)
				.Where(e => e.MemberType == MemberTypes.Property || e.MemberType == MemberTypes.Field);
			if (predicate != null) enumerable = enumerable.Where(e => predicate(e));
			return enumerable;
		}

		[NotNull]
		public static IEnumerable<PropertyInfo> Properties(this Expression thisValue, Predicate<PropertyInfo> predicate = null)
		{
			IEnumerable<PropertyInfo> enumerable = Members(thisValue, e => e.MemberType == MemberTypes.Property)
				.Cast<PropertyInfo>();
			if (predicate != null) enumerable = enumerable.Where(e => predicate(e));
			return enumerable;
		}

		[NotNull]
		public static IEnumerable<FieldInfo> Fields(this Expression thisValue, Predicate<FieldInfo> predicate = null)
		{
			IEnumerable<FieldInfo> enumerable = Members(thisValue, e => e.MemberType == MemberTypes.Field)
				.Cast<FieldInfo>();
			if (predicate != null) enumerable = enumerable.Where(e => predicate(e));
			return enumerable;
		}

		public static MethodBase GetMethod([NotNull] this Expression thisValue) { return GetMethod(thisValue, out _); }
		public static MethodBase GetMethod([NotNull] this Expression thisValue, out IReadOnlyCollection<Expression> arguments)
		{
			arguments = null;

			MethodBase method;

			switch (thisValue.NodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddAssign:
				case ExpressionType.AddAssignChecked:
				case ExpressionType.AddChecked:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.AndAssign:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Assign:
				case ExpressionType.Coalesce:
				case ExpressionType.Decrement:
				case ExpressionType.Divide:
				case ExpressionType.DivideAssign:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.ExclusiveOrAssign:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Increment:
				case ExpressionType.LeftShift:
				case ExpressionType.LeftShiftAssign:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.Modulo:
				case ExpressionType.ModuloAssign:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyAssign:
				case ExpressionType.MultiplyAssignChecked:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.NotEqual:
				case ExpressionType.OnesComplement:
				case ExpressionType.Or:
				case ExpressionType.OrAssign:
				case ExpressionType.OrElse:
				case ExpressionType.Power:
				case ExpressionType.PowerAssign:
				case ExpressionType.RightShift:
				case ExpressionType.RightShiftAssign:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractAssign:
				case ExpressionType.SubtractAssignChecked:
				case ExpressionType.SubtractChecked:
				{
					BinaryExpression expression = (BinaryExpression)thisValue;
					method = expression.Method;
					arguments = new []{ expression.Right };
					break;
				}
				case ExpressionType.Call:
				{
					MethodCallExpression expression = (MethodCallExpression)thisValue;
					method = expression.Method;
					arguments = expression.Arguments;
					break;
				}
				case ExpressionType.Invoke:
				{
					InvocationExpression expression = (InvocationExpression)thisValue;
					method = GetMethod(expression.Expression, out arguments);
					break;
				}
				case ExpressionType.Lambda:
				{
					LambdaExpression expression = (LambdaExpression)thisValue;
					method = GetMethod(expression.Body, out arguments);
					break;
				}
				case ExpressionType.ListInit:
				{
					ListInitExpression expression = (ListInitExpression)thisValue;
					method = GetMethod(expression.NewExpression, out arguments);
					break;
				}
				case ExpressionType.MemberAccess:
				{
					MemberExpression expression = (MemberExpression)thisValue;

					switch (expression.Member.MemberType)
					{
						case MemberTypes.Constructor:
							method = (ConstructorInfo)expression.Member;
							break;
						case MemberTypes.Event:
							method = ((EventInfo)expression.Member).GetRaiseMethod();
							break;
						case MemberTypes.Method:
							method = (MethodInfo)expression.Member;
							break;
						case MemberTypes.Property:
							method = ((PropertyInfo)expression.Member).GetGetMethod();
							break;
						default:
							throw new NotSupportedException();
					}

					break;
				}
				case ExpressionType.MemberInit:
				{
					MemberInitExpression expression = (MemberInitExpression)thisValue;
					method = GetMethod(expression.NewExpression, out arguments);
					break;
				}
				case ExpressionType.New:
				{
					NewExpression expression = (NewExpression)thisValue;
					method = expression.Constructor;
					arguments = expression.Arguments;
					break;
				}
				case ExpressionType.ArrayLength:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.UnaryPlus:
				case ExpressionType.Unbox:
				{
					UnaryExpression expression = (UnaryExpression)thisValue;
					method = expression.Method;
					arguments = new[]
					{
						expression.Operand
					};
					break;
				}
				default:
					method = null;
					break;
			}

			if (arguments == null && method != null)
				arguments = GetArguments(thisValue);
			return method;
		}

		public static IReadOnlyCollection<Expression> GetArguments(this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			if (thisValue == null || !(thisValue is IArgumentProvider provider)) return null;

			// try property first
			Type type = thisValue.GetType();
			PropertyInfo property = type.GetProperty("Arguments", Constants.BF_PUBLIC_INSTANCE, typeof(ReadOnlyCollection<Expression>));
			if (property != null) return (IReadOnlyCollection<Expression>)property.GetValue(thisValue);

			List<Expression> list = new List<Expression>(provider.ArgumentCount);

			for (int i = 0; i < provider.ArgumentCount; i++)
			{
				list.Add(provider.GetArgument(i));
			}

			return list.AsReadOnly();
		}

		public static IReadOnlyCollection<object> GetValues(this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			return thisValue is IArgumentProvider
						? ArgumentExtractor.ExtractValues(thisValue)
						: null;
		}

		[NotNull]
		public static EventInfo GetEvent([NotNull] this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			MemberExpression member = thisValue as MemberExpression ?? throw new ArgumentException($"Expression '{thisValue}' is not an event.");
			EventInfo eventInfo = member.Member as EventInfo ?? throw new ArgumentException($"Expression '{thisValue}' is not an event.");
			return eventInfo;
		}

		[NotNull]
		public static MemberInfo GetMember([NotNull] this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			MemberExpression member = thisValue as MemberExpression ?? throw new ArgumentException($"Expression '{thisValue}' neither refers to a property nor a field.");
			FieldInfo field = member.Member as FieldInfo;
			if (field != null) return field;
			PropertyInfo property = member.Member as PropertyInfo ?? throw new ArgumentException($"Expression '{thisValue}' neither refers to a property nor a field.");
			return property;
		}

		[NotNull]
		public static PropertyInfo GetProperty([NotNull] this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			MemberExpression member = thisValue as MemberExpression ?? throw new ArgumentException($"Expression '{thisValue}' is not a property.");
			PropertyInfo propertyInfo = member.Member as PropertyInfo ?? throw new ArgumentException($"Expression '{thisValue}' is not a property.");
			return propertyInfo;
		}

		[NotNull]
		public static FieldInfo GetField([NotNull] this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			MemberExpression member = thisValue as MemberExpression ?? throw new ArgumentException($"Expression '{thisValue}' is not a field.");
			FieldInfo fieldInfo = member.Member as FieldInfo ?? throw new ArgumentException($"Expression '{thisValue}' is not a field.");
			return fieldInfo;
		}

		public static Expression Expand([NotNull] this Expression thisValue) { return new ExpandVisitor().Visit(thisValue); }

		public static bool IsNullConstant([NotNull] this Expression thisValue)
		{
			thisValue = Simplify(thisValue);
			return thisValue.NodeType == ExpressionType.Constant && ((ConstantExpression)thisValue).Value == null;
		}

		public static bool HasDefaultMembersOnly(this Expression thisValue, [NotNull] IReadOnlyList<PropertyPath> propertyPaths)
		{
			thisValue = Simplify(thisValue);
			NewExpression member = thisValue as NewExpression ?? throw new ArgumentException($"Expression '{thisValue}' is not a member expression.");
			return member.Members == null || !member.Members.Where((t, i) =>
			{
				PropertyPath propertyPath = propertyPaths[i];
				return !string.Equals(t.Name, propertyPath[propertyPath.Count - 1].Name, StringComparison.Ordinal);
			}).Any();
		}

		public static string GetPath(this Expression thisValue, bool placeholders = false) { return GetPath(thisValue, placeholders, out _); }
		public static string GetPath(this Expression thisValue, bool placeholders, out int parametersCount)
		{
			parametersCount = 0;
			if (thisValue == null) return null;

			StringBuilder sb = new StringBuilder();
			bool passedFirst = false;

			foreach (Expression	expression in Enumerate(thisValue))
			{
				switch (expression)
				{
					case ParameterExpression parameter:
						if (!passedFirst) goto NextLoop;
						sb.Append(placeholders
									? $"[{{{parametersCount++}}}]"
									: $"[{parameter.Name}]");
						break;
					case MemberExpression memberExpression:
						sb.Separator('.').Append(memberExpression.Member.Name);
						break;
				}
				NextLoop:
				passedFirst = true;
			}

			return sb.Length > 0
						? sb.ToString()
						: null;
		}

		public static Expression ReplaceExpression([NotNull] this Expression thisValue, [NotNull] Expression searchExpr, [NotNull] Expression replaceExpr)
		{
			return new ExpressionReplacer(searchExpr, replaceExpr).Visit(thisValue);
		}

		public static Expression ReplaceParameter<T>([NotNull] this Expression thisValue, [NotNull] string parameterName, T value)
		{
			return new ParameterReplacer(parameterName, Expression.Constant(value)).Visit(thisValue);
		}

		// Returns the given anonymous method as a lambda expression
		public static Expression ToExpression(this Expression thisValue) { return thisValue; }

		[NotNull]
		public static Expression<Func<TModel, TTo>> Cast<TModel, TFrom, TTo>([NotNull] this Expression<Func<TModel, TFrom>> thisValue)
		{
			Expression castExpr = Expression.ConvertChecked(thisValue.Body, typeof(TTo));
			return Expression.Lambda<Func<TModel, TTo>>(castExpr, thisValue.Parameters);
		}

		public static void Invoke([NotNull] this Expression<Action> thisValue)
		{
			thisValue.Compile().Invoke();
		}

		public static void Invoke<T>([NotNull] this Expression<Action<T>> thisValue, T arg)
		{
			thisValue.Compile().Invoke(arg);
		}

		public static void Invoke<T1, T2>([NotNull] this Expression<Action<T1, T2>> thisValue, T1 arg1, T2 arg2)
		{
			thisValue.Compile().Invoke(arg1, arg2);
		}

		public static void Invoke<T1, T2, T3>([NotNull] this Expression<Action<T1, T2, T3>> thisValue, T1 arg1, T2 arg2, T3 arg3)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3);
		}

		public static void Invoke<T1, T2, T3, T4>([NotNull] this Expression<Action<T1, T2, T3, T4>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4);
		}

		public static void Invoke<T1, T2, T3, T4, T5>([NotNull] this Expression<Action<T1, T2, T3, T4, T5>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
		}

		public static void Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>([NotNull] this Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
		}

		public static TResult Invoke<TResult>([NotNull] this Expression<Func<TResult>> thisValue) { return thisValue.Compile().Invoke(); }

		public static TResult Invoke<T, TResult>([NotNull] this Expression<Func<T, TResult>> thisValue, T arg) { return thisValue.Compile().Invoke(arg); }

		public static TResult Invoke<T1, T2, TResult>([NotNull] this Expression<Func<T1, T2, TResult>> thisValue, T1 arg1, T2 arg2)
		{
			return thisValue.Compile().Invoke(arg1, arg2);
		}

		public static TResult Invoke<T1, T2, T3, TResult>([NotNull] this Expression<Func<T1, T2, T3, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
		}

		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>([NotNull] this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> thisValue, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			return thisValue.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
		}

		internal static PropertyPath MatchSimplePropertyAccess([NotNull] this Expression thisValue, [NotNull] Expression propertyAccessExpression)
		{
			PropertyPath propertyPath = MatchPropertyAccess(thisValue, propertyAccessExpression);
			return !(propertyPath != null) || propertyPath.Count != 1
						? null
						: propertyPath;
		}

		internal static PropertyPath MatchComplexPropertyAccess([NotNull] this Expression thisValue, [NotNull] Expression propertyAccessExpression)
		{
			return MatchPropertyAccess(thisValue, propertyAccessExpression);
		}

		internal static PropertyPath MatchPropertyAccess(this Expression thisValue, [NotNull] Expression propertyAccessExpression)
		{
			List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
			MemberExpression memberExpression;

			do
			{
				memberExpression = Simplify(propertyAccessExpression) as MemberExpression;
				if (memberExpression == null) return null;
				PropertyInfo member = memberExpression.Member as PropertyInfo;
				if (member == null) return null;
				propertyInfoList.Insert(0, member);
				propertyAccessExpression = memberExpression.Expression;
			}
			while (memberExpression.Expression != thisValue);

			return new PropertyPath(propertyInfoList);
		}
	}
}