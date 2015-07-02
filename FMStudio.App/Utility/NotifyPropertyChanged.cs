using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FMStudio.App.Utility
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public void Notify(Expression<Func<object>> property)
        {
            var propertyInfo = GetMemberInfo(property);

            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyInfo.Member.Name));
                });
            }
        }

        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}