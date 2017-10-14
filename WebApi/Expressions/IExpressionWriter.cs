using System.Linq.Expressions;

namespace Lenic.Web.WebApi.Expressions
{
    /// <summary>
    /// ���ʽ���� OData Э��ת���Ľӿ�
    /// </summary>
    public interface IExpressionWriter
    {
        /// <summary>
        /// ת�����ʽ���� OData Э���ַ�����
        /// </summary>
        /// <param name="expression">һ����Ҫת���� <see cref="Expression" /> ��ʵ������</param>
        /// <returns>ת����ɵ�Ŀ���ַ�����OData ��ʽ</returns>
        string Write(Expression expression);
    }
}