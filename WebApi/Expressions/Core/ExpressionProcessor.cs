using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions.Core
{
    public class ExpressionProcessor : IExpressionProcessor
    {
        public RemoteDataParameter DataParameter { get; set; }

        public IExpressionWriter Writer { get; set; }

        public void Build(MethodCallExpression methodCall)
        {
            if (methodCall == null)
                return;

            var methodName = methodCall.Method.Name;
            switch (methodName)
            {
                case "First":
                case "FirstOrDefault":
                    {
                        DataParameter.TakeParameter = "1";
                        Build(methodCall.Arguments[0] as MethodCallExpression);
                        if (methodCall.Arguments.Count >= 2)
                            WriteArgument(methodCall.Arguments[1]);
                    }
                    break;

                case "Count":
                case "LongCount":
                case "Any":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);
                        if (methodCall.Arguments.Count >= 2)
                            WriteArgument(methodCall.Arguments[1]);
                    }
                    break;

                case "Single":
                case "SingleOrDefault":
                    throw new NotSupportedException("��ʹ�� Count �� First/FirstOrDefault �滻��������ܣ�");

                case "Last":
                case "LastOrDefault":
                    throw new NotSupportedException("��ʹ�� OrderByDescending �� First/FirstOrDefault �滻��������ܣ�");

                case "Where":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);
                        WriteArgument(methodCall.Arguments[1]);
                    }
                    break;

                case "Select":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);
                        if (!string.IsNullOrWhiteSpace(DataParameter.SelectParameter))
                            throw new NotSupportedException("��֧�ֶ���ת��������ת����ʹ�� AsEnumerable/ToArray �Ȳ�ѯ��ֹ������У�");

                        var unaryExpression = methodCall.Arguments[1] as UnaryExpression;
                        if (unaryExpression != null)
                        {
                            var lambdaExpression = unaryExpression.Operand as LambdaExpression;
                            if (lambdaExpression != null)
                                ResolveProjection(lambdaExpression);
                        }
                    }

                    break;

                case "OrderBy":
                case "ThenBy":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var item = Writer.Write(methodCall.Arguments[1]);
                        DataParameter.OrderByParameter.Add(item);
                    }

                    break;

                case "OrderByDescending":
                case "ThenByDescending":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var visit = Writer.Write(methodCall.Arguments[1]);
                        DataParameter.OrderByParameter.Add(visit + " desc");
                    }

                    break;

                case "Take":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        DataParameter.TakeParameter = Writer.Write(methodCall.Arguments[1]);
                    }

                    break;

                case "Skip":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        DataParameter.SkipParameter = Writer.Write(methodCall.Arguments[1]);
                    }

                    break;

                case "Expand":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var expression = methodCall.Arguments[1];

                        var objectMember = Expression.Convert(expression, typeof(object));
                        var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

                        DataParameter.ExpandParameter = getterLambda().ToString();
                    }

                    break;

                default:
                    throw new NotSupportedException("OData Э�鲻֧�� LINQ ��չ������" + methodName);
            }
        }

        private void ResolveProjection(LambdaExpression lambdaExpression)
        {
            var selectFunction = lambdaExpression.Body as NewExpression;
            if (selectFunction != null)
            {
                var members = selectFunction.Members.Select(x => x.Name).ToArray();
                var args = selectFunction.Arguments.OfType<MemberExpression>()
                                         .Select(x => x.Member.Name)
                                         .ToArray();
                if (members.Intersect(args).Count() != members.Length)
                    throw new NotSupportedException("��֧�ֳ�Ա����ӳ�䣡");

                DataParameter.SelectParameter = string.Join(",", args);
            }
            else
            {
                var propertyExpression = lambdaExpression.Body as MemberExpression;
                if (propertyExpression != null)
                    DataParameter.SelectParameter = propertyExpression.Member.Name;
            }
        }

        private void WriteArgument(Expression expression)
        {
            var filter = Writer.Write(expression);
            if (string.IsNullOrWhiteSpace(filter))
                return;

            DataParameter.FilterParameter = string.IsNullOrWhiteSpace(DataParameter.FilterParameter) ? filter : string.Format("({0}) and ({1})", DataParameter.FilterParameter, filter);
        }
    }
}