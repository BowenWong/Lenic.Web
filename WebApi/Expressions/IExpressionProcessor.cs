using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// Զ�̶����ȡ���ӿڹ�����
    /// </summary>
    public interface IExpressionProcessor
    {
        /// <summary>
        /// ��ȡ�����ô�������Զ�̶�������ò�����
        /// </summary>
        RemoteDataParameter DataParameter { get; set; }

        /// <summary>
        /// ��ȡ������ OData Э��ת���ӿڵ�ʵ������
        /// </summary>
        IExpressionWriter Writer { get; set; }

        /// <summary>
        /// ����Զ�̶����ȡ���ӿ�ʵ������
        /// </summary>
        /// <param name="methodCall">һ���������õı��ʽ����</param>
        void Build(MethodCallExpression methodCall);
    }
}