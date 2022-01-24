// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;
using System.Linq.Expressions;

using Moq.Async;

namespace Moq
{
	internal sealed class InnerMockSetup : SetupWithOutParameterSupport
	{
		private readonly object returnValue;

		public InnerMockSetup(Expression originalExpression, Mock mock, InvocationShape expectation, object returnValue)
			: base(originalExpression, mock, expectation)
		{
			Debug.Assert(Awaitable.TryGetResultRecursive(returnValue) is IMocked);

			this.returnValue = returnValue;

			this.MarkAsVerifiable();
		}

		protected override void ExecuteCore(IInvocation invocation)
		{
			invocation.ReturnValue = this.returnValue;
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			returnValue = this.returnValue;
			return true;
		}

		protected override void ResetCore()
		{
			this.InnerMock.MutableSetups.Reset();
		}

		protected override void VerifySelf()
		{
		}
	}
}
