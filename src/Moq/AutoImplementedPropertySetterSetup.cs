// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Setup used by <see cref="Mock.SetupAllProperties(Mock)"/> for property setters.
	/// </summary>
	internal sealed class AutoImplementedPropertySetterSetup : Setup
	{
		private Action<object> setter;

		public AutoImplementedPropertySetterSetup(Mock mock, LambdaExpression originalExpression, MethodInfo method, Action<object> setter)
			: base(mock, new InvocationShape(originalExpression, method, new Expression[] { It.IsAny(method.GetParameterTypes().Last()) }), originalSetup: null)
		{
			this.setter = setter;
		}

		public override void Execute(Invocation invocation)
		{
			this.IsMatched = true;
			this.setter.Invoke(invocation.Arguments[0]);
			invocation.Return();
		}

		public override MockException TryVerifyAll() => null;
	}
}
