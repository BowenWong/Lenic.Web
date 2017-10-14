using System;
using System.ComponentModel;

namespace Lenic.Framework.Common.Extensions
{
    /// <summary>
    /// object extension
    /// </summary>
    public static class ObjectExtension
    {
        #region Type Convert Extensions

        private static string typeIConvertibleFullName = typeof(IConvertible).FullName;

        /// <summary>
        /// ����ǰʵ����������ת��Ϊ T ����.
        /// </summary>
        /// <typeparam name="T">Ŀ������.</typeparam>
        /// <param name="obj">��ǰʵ��.</param>
        /// <returns>
        /// ת����ɵ� T ���͵�һ��ʵ������.
        /// </returns>
        public static T ChangeType<T>(this object obj)
        {
            return ChangeType(obj, default(T));
        }

        /// <summary>
        /// ����ǰʵ����������ת��Ϊ T ����.
        /// </summary>
        /// <typeparam name="T">Ŀ������.</typeparam>
        /// <param name="obj">��ǰʵ��.</param>
        /// <param name="defaultValue">ת��ʧ��ʱ�ķ���ֵ.</param>
        /// <returns>
        /// ת����ɵ� T ���͵�һ��ʵ������.
        /// </returns>
        /// <exception cref="System.ApplicationException">convert error.</exception>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static T ChangeType<T>(this object obj, T defaultValue)
        {
            if (obj != null && !(obj is DBNull))
            {
                if (obj is T)
                    return (T)obj;

                var sourceType = obj.GetType();
                var targetType = typeof(T);

                if (targetType.IsEnum)
                    return (T)Enum.Parse(targetType, obj.ToString(), true);

                if (sourceType.GetInterface(typeIConvertibleFullName) != null &&
                    targetType.GetInterface(typeIConvertibleFullName) != null)
                    return (T)Convert.ChangeType(obj, targetType);

                var converter = TypeDescriptor.GetConverter(obj);
                if (converter != null && converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(obj, targetType);

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(sourceType))
                    return (T)converter.ConvertFrom(obj);

                throw new ApplicationException("convert error.");
            }
            throw new ArgumentNullException("obj");
        }

        /// <summary>
        /// ����ǰʵ����������ת��Ϊ T ����.
        /// </summary>
        /// <typeparam name="T">Ŀ������.</typeparam>
        /// <param name="obj">��ǰʵ��.</param>
        /// <param name="defaultValue">ת��ʧ��ʱ�ķ���ֵ.</param>
        /// <param name="ignoreException">�������Ϊ <c>true</c> ��ʾ�����쳣��Ϣ, ֱ�ӷ���ȱʡֵ.</param>
        /// <returns>
        /// ת����ɵ� T ���͵�һ��ʵ������.
        /// </returns>
        public static T ChangeType<T>(this object obj, T defaultValue, bool ignoreException)
        {
            if (ignoreException)
            {
                try
                {
                    return ChangeType<T>(obj, defaultValue);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return ChangeType<T>(obj, defaultValue);
        }

        #endregion Type Convert Extensions

        /// <summary>
        /// ���õ�ǰ������ٽ��䷵�أ����ɶ�ʵ��������и�ֵ��������ǰʵ��Ϊ��ʱ���� action ������
        /// </summary>
        /// <typeparam name="T">�����ö�������͡�</typeparam>
        /// <param name="obj">��ǰ�����õ�ʵ������</param>
        /// <param name="action">��Ҫִ�е����ò�����</param>
        /// <returns>������ɺ��ʵ������</returns>
        public static T Setup<T>(this T obj, Action<T> action) where T : class
        {
            if (object.ReferenceEquals(obj, null))
                return obj;

            action(obj);
            return obj;
        }
    }
}