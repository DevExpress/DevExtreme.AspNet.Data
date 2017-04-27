using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DynamicBinder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace DevExtreme.AspNet.Data {

    static class DynamicBindingHelper {

        public static bool ShouldUseDynamicBinding(Type type) {
            return type == typeof(object) || typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);
        }

        public static Expression CompileGetMember(Expression target, string clientExpr) {
            var binder = DynamicBinder.GetMember(CSharpBinderFlags.None, clientExpr, typeof(DynamicBindingHelper), new[] {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            });
            return DynamicExpression.Dynamic(binder, typeof(object), target);
        }

    }

}
