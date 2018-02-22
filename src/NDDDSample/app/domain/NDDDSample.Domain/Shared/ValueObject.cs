namespace NDDDSample.Domain.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        private readonly List<PropertyInfo> _properties;

        protected ValueObject()
        {
            _properties = new List<PropertyInfo>();
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != GetType()) return false;

            return Equals(obj as T);
        }

        public bool Equals(T other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            foreach (var property in _properties)
            {
                var oneValue = property.GetValue(this, null);
                var otherValue = property.GetValue(other, null);

                if (null == oneValue || null == otherValue) return false;
                if (false == oneValue.Equals(otherValue)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 29;
            foreach (var property in _properties)
            {
                var propertyValue = property.GetValue(this, null);
                if (null == propertyValue)
                    continue;

                hashCode = hashCode ^ propertyValue.GetHashCode();
            }

            return hashCode;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var property in _properties)
            {
                var propertyValue = property.GetValue(this, null);
                if (null == propertyValue)
                    continue;

                stringBuilder.Append(propertyValue.ToString());
            }

            return stringBuilder.ToString();
        }

        protected void RegisterProperty(Expression<Func<T, Object>> expression)
        {
            MemberExpression memberExpression;
            if (ExpressionType.Convert == expression.Body.NodeType)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new InvalidOperationException("Invalid member expression");
            }

            _properties.Add(memberExpression.Member as PropertyInfo);
        }
    }
}
