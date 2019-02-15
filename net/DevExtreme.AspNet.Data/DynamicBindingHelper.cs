﻿using Microsoft.CSharp.RuntimeBinder;
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
        static IEnumerable<CSharpArgumentInfo> EMPTY_ARGUMENT_INFO;

        public static bool ShouldUseDynamicBinding(Type type) {
            if(type == typeof(object))
                return true;

            if(typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type)) {
                var name = type.AssemblyQualifiedName;
                if(name.Contains("f__AnonymousType") && name.Contains("System.Linq.Dynamic.Core.DynamicClasses"))
                    return false;

                return true;
            }

            return false;
        }

        public static Expression CompileGetMember(Expression target, string clientExpr) {
            if(EMPTY_ARGUMENT_INFO == null)
                EMPTY_ARGUMENT_INFO = new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) };

            var binder = DynamicBinder.GetMember(CSharpBinderFlags.None, clientExpr, typeof(DynamicBindingHelper), EMPTY_ARGUMENT_INFO);

            return Expression.Call(
                typeof(Utils).GetMethod(nameof(Utils.UnwrapNewtonsoftValue)),
                DynamicExpression.Dynamic(binder, typeof(object), target)
            );
        }

    }

}
