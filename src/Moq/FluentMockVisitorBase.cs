// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Moq
{
	internal abstract class FluentMockVisitorBase : ExpressionVisitor
	{
		/// <summary>
		/// The first method call or member access will be the
		/// last segment of the expression (depth-first traversal),
		/// which is the one we have to Setup rather than FluentMock.
		/// And the last one is the one we have to Mock.Get rather
		/// than FluentMock.
		/// </summary>
		protected bool isFirst;

		protected FluentMockVisitorBase()
		{
			this.isFirst = true;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// Translate differently member accesses over transparent
			// compiler-generated types as they are typically the
			// anonymous types generated to build up the query expressions.
			if (node.Expression.NodeType == ExpressionType.Parameter &&
				node.Expression.Type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				var memberType = node.Member is FieldInfo ?
					((FieldInfo)node.Member).FieldType :
					((PropertyInfo)node.Member).PropertyType;

				// Generate a Mock.Get over the entire member access rather.
				// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
				return Expression.Call(null,
					Mock.GetMethod.MakeGenericMethod(memberType), node);
			}

			// If member is not mock-able, actually, including being a sealed class, etc.?
			if (node.Member is FieldInfo)
				throw new NotSupportedException();

			var lambdaParam = Expression.Parameter(node.Expression.Type, "mock");
			Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, node.Member);
			var targetMethod = GetTargetMethod(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType);
			if (isFirst)
			{
				isFirst = false;
			}

			return TranslateFluent(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType, targetMethod, Visit(node.Expression), lambdaParam, lambdaBody);
		}

		protected override abstract Expression VisitMethodCall(MethodCallExpression node);

		protected override abstract Expression VisitParameter(ParameterExpression node);

		protected abstract Expression TranslateFluent(Type objectType,
		                                              Type returnType,
		                                              MethodInfo targetMethod,
		                                              Expression instance,
		                                              ParameterExpression lambdaParam,
		                                              Expression lambdaBody);

		protected abstract MethodInfo GetTargetMethod(Type objectType, Type returnType);
	}
}
