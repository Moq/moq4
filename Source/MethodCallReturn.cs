﻿//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Language;
using Moq.Language.Flow;
using Moq.Properties;
using Moq.Proxy;

namespace Moq
{
	internal interface IMethodCallReturn
	{
		bool ProvidesReturnValue();
	}

	internal sealed partial class MethodCallReturn<TMock, TResult> : MethodCall, IMethodCallReturn, ISetup<TMock, TResult>, ISetupGetter<TMock, TResult>, IReturnsResult<TMock>
		where TMock : class
	{
		// This enum exists for reasons of optimizing memory usage.
		// Previously this class had two `bool` fields, `hasReturnValue` and `callBase`.
		// Using an enum allows us to combine them into a single field.
		private enum ReturnValueKind : byte
		{
			None = 0,
			Explicit,
			CallBase,
		}

		private Delegate valueDel;
		private Action<object[]> afterReturnCallback;
		private ReturnValueKind returnValueKind;

		public MethodCallReturn(Mock mock, Condition condition, Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(mock, condition, originalExpression, method, arguments)
		{
		}

		public bool ProvidesReturnValue() => this.returnValueKind != ReturnValueKind.None;

		public IVerifies Raises(Action<TMock> eventExpression, EventArgs args)
		{
			return this.Raises(eventExpression, () => args);
		}

		public IVerifies Raises(Action<TMock> eventExpression, Func<EventArgs> func)
		{
			return this.RaisesImpl(eventExpression, func);
		}

		public IVerifies Raises(Action<TMock> eventExpression, params object[] args)
		{
			return this.RaisesImpl(eventExpression, args);
		}

		public IReturnsResult<TMock> Returns(Delegate valueFunction)
		{
			// If `TResult` is `Delegate`, that is someone is setting up the return value of a method
			// that returns a `Delegate`, then we have arrived here because C# picked the wrong overload:
			// We don't want to invoke the passed delegate to get a return value; the passed delegate
			// already is the return value.
			if (typeof(TResult) == typeof(Delegate))
			{
				return this.Returns(() => (TResult)(object)valueFunction);
			}

			// The following may seem overly cautious, but we don't throw an `ArgumentNullException`
			// here because Moq has been very forgiving with incorrect `Returns` in the past.
			if (valueFunction == null)
			{
				return this.Returns(() => default(TResult));
			}

			if (valueFunction.GetMethodInfo().ReturnType == typeof(void))
			{
				throw new ArgumentException(Resources.InvalidReturnsCallbackNotADelegateWithReturnType, nameof(valueFunction));
			}

			SetReturnDelegate(valueFunction);
			return this;
		}

		public IReturnsResult<TMock> Returns(Func<TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IReturnsResult<TMock> Returns(TResult value)
		{
			Returns(() => value);
			return this;
		}

		public IReturnsResult<TMock> CallBase()
		{
			this.returnValueKind = ReturnValueKind.CallBase;
			return this;
		}

		IReturnsThrows<TMock, TResult> ICallback<TMock, TResult>.Callback(Delegate callback)
		{
			base.Callback(callback);
			return this;
		}

		IReturnsThrowsGetter<TMock, TResult> ICallbackGetter<TMock, TResult>.Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TMock, TResult> Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		private void SetReturnDelegate(Delegate value)
		{
			if (value != null && (this.Mock.Switches & Switches.ValidateReturnsDelegateSignatures) != 0)
			{
				SetReturnDelegate_ValidateSignature(value);
			}

			this.valueDel = value;
			this.returnValueKind = ReturnValueKind.Explicit;
		}

		private void SetReturnDelegate_ValidateSignature(Delegate value)
		{
			var valueMethod = value.GetMethodInfo();

			var valueMethodParameters = valueMethod.GetParameters();
			if (valueMethodParameters.Length > 0)
			{
				var expectedParameters = this.Method.GetParameters();
				if (!value.HasCompatibleParameterList(expectedParameters))
				{
					var actualParameters = value.GetMethodInfo().GetParameters();
					throw new ArgumentException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.InvalidCallbackParameterMismatch,
						string.Join(", ", expectedParameters.Select(p => p.ParameterType.Name)),
						string.Join(", ", actualParameters.Select(p => p.ParameterType.Name))));
				}
			}

			if (!this.Method.ReturnType.IsAssignableFrom(valueMethod.ReturnType))
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InvalidCallbackReturnTypeMismatch,
					this.Method.ReturnType.Name,
					valueMethod.ReturnType.Name));
			}
		}

		protected override void SetCallbackWithoutArguments(Action callback)
		{
			if (this.ProvidesReturnValue())
			{
				this.afterReturnCallback = delegate { callback(); };
			}
			else
			{
				base.SetCallbackWithoutArguments(callback);
			}
		}

		protected override void SetCallbackWithArguments(Delegate callback)
		{
			if (this.ProvidesReturnValue())
			{
				this.afterReturnCallback = delegate(object[] args) { callback.InvokePreserveStack(args); };
			}
			else
			{
				base.SetCallbackWithArguments(callback);
			}
		}

		public override void Execute(ICallContext call)
		{
			base.Execute(call);

			if (this.returnValueKind == ReturnValueKind.CallBase)
			{
				call.InvokeBase();
			}
			else if (this.valueDel != null)
			{
				call.ReturnValue = this.valueDel.HasCompatibleParameterList(new ParameterInfo[0])
					? valueDel.InvokePreserveStack()                //we need this, for the user to be able to use parameterless methods
					: valueDel.InvokePreserveStack(call.Arguments); //will throw if parameters mismatch
			}
			else
			{
				call.ReturnValue = default(TResult);
			}

			this.afterReturnCallback?.Invoke(call.Arguments);
		}
	}
}
