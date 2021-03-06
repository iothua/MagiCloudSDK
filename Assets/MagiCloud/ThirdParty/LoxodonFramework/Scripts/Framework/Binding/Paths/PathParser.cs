﻿using System;
using System.Reflection;
using System.Linq.Expressions;
#if UNITY_IOS
using Loxodon.Framework.Binding.Expressions;
#endif

namespace Loxodon.Framework.Binding.Paths
{
    public class PathParser : IPathParser
    {
        public virtual Path Parse(string pathText)
        {
            return new TextPathParser(pathText.Replace(" ", "")).Parse();
        }

        public virtual Path Parse(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Path path = new Path();
            var body = expression.Body as MemberExpression;
            if (body != null)
            {
                this.Parse(body, path);
                return path;
            }

            var method = expression.Body as MethodCallExpression;
            if (method != null)
            {
                this.Parse(method, path);
                return path;
            }

            var unary = expression.Body as UnaryExpression;
            if (unary != null && unary.NodeType == ExpressionType.Convert)
            {
                this.Parse(unary.Operand, path);
                return path;
            }

            throw new ArgumentException("Invalid argument", "expression");
        }

        private void Parse(Expression expression, Path path)
        {
            if (expression == null || !(expression is MemberExpression || expression is MethodCallExpression))
                return;

            if (expression is MemberExpression)
            {
                MemberExpression me = (MemberExpression)expression;
                var memberInfo = me.Member;
                if (memberInfo.IsStatic())
                {
                    path.Prepend(new MemberNode(memberInfo));
                    path.Prepend(new TypeNode(memberInfo.DeclaringType));
                    return;
                }
                else {
                    path.Prepend(new MemberNode(memberInfo));
                    if (me.Expression != null)
                        this.Parse(me.Expression, path);
                    return;
                }
            }

            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCall = (MethodCallExpression)expression;
                if (methodCall.Method.Name.Equals("get_Item") && methodCall.Arguments.Count == 1)
                {
                    var argument = methodCall.Arguments[0];
                    if (!(argument is ConstantExpression))
                    {
                        argument = ConvertMemberAccessToConstant(argument);
                        //throw new NotSupportedException();
                    }                  

                    object value = (argument as ConstantExpression).Value;
                    if (value is string)
                    {
                        path.PrependIndexed((string)value);
                    }
                    else if (value is Int32)
                    {
                        path.PrependIndexed((int)value);
                    }
                    if (methodCall.Object != null)
                        this.Parse(methodCall.Object, path);
                    return;
                }

                if (methodCall.Method.Name.Equals("CreateDelegate") && methodCall.Arguments.Count == 3)
                {
                    var info = (MethodInfo)(methodCall.Arguments[2] as ConstantExpression).Value;
                    if (info.IsStatic)
                    {
                        path.Prepend(new MemberNode(info));
                        path.Prepend(new TypeNode(info.DeclaringType));
                        return;
                    }
                    else
                    {
                        path.Prepend(new MemberNode(info));
                        this.Parse(methodCall.Arguments[1], path);
                        return;
                    }
                }

                if (methodCall.Method.ReturnType.Equals(typeof(void)))
                {
                    var info = methodCall.Method;
                    if (info.IsStatic)
                    {
                        path.Prepend(new MemberNode(info));
                        path.Prepend(new TypeNode(info.DeclaringType));
                        return;
                    }
                    else
                    {
                        path.Prepend(new MemberNode(info));
                        if (methodCall.Object != null)
                            this.Parse(methodCall.Object, path);
                        return;
                    }
                }

                throw new NotSupportedException("Expressions of type " + expression.Type + " are not supported.");
            }
        }

        private static Expression ConvertMemberAccessToConstant(Expression argument)
        {
            if (argument is ConstantExpression)
                return argument;

            try
            {
                var boxed = Expression.Convert(argument, typeof(object));
#if !UNITY_IOS
                var fun = Expression.Lambda<Func<object>>(boxed).Compile();
                var constant = fun();
#else
                var fun = (Func<object[],object>)Expression.Lambda<Func<object>>(boxed).DynamicCompile();
                var constant = fun(new object[] { });
#endif

                var constExpr = Expression.Constant(constant);
                return constExpr;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual Path ParseStaticPath(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var current = expression.Body;
            var unary = current as UnaryExpression;
            if (unary != null)
                current = unary.Operand;

            if (current is MemberExpression)
            {
                Path path = new Path();
                this.Parse(current, path);
                return path;
            }

            if (current is MethodCallExpression)
            {
                Path path = new Path();
                this.Parse(current, path);
                return path;
            }

            throw new ArgumentException("Invalid argument", "expression");
        }


        public virtual Path ParseStaticPath(string pathText)
        {
            string typeName = this.ParserTypeName(pathText);
            string memberName = this.ParserMemberName(pathText);
            Type type = TypeFinderUtils.FindType(typeName);

            Path path = new Path();
            path.Append(new TypeNode(type));
            path.Append(new MemberNode(memberName));
            return path;
        }

        protected string ParserTypeName(string pathText)
        {
            if (pathText == null)
                throw new ArgumentNullException("pathText");

            pathText = pathText.Replace(" ", "");
            if (string.IsNullOrEmpty(pathText))
                throw new ArgumentException("The pathText is empty");

            int index = pathText.LastIndexOf('.');
            if (index <= 0)
                throw new ArgumentException("pathText");

            return pathText.Substring(0, index);
        }

        protected string ParserMemberName(string pathText)
        {
            if (pathText == null)
                throw new ArgumentNullException("pathText");

            pathText = pathText.Replace(" ", "");
            if (string.IsNullOrEmpty(pathText))
                throw new ArgumentException("The pathText is empty");

            int index = pathText.LastIndexOf('.');
            if (index <= 0)
                throw new ArgumentException("pathText");

            return pathText.Substring(index + 1);
        }

        public virtual string ParseMemberName(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var method = expression.Body as MethodCallExpression;
            if (method != null)
                return method.Method.Name;

            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Invalid argument", "expression");

            if (!(body.Expression is ParameterExpression))
                throw new NotSupportedException("Invalid argument");

            return body.Member.Name;
        }
    }
}
