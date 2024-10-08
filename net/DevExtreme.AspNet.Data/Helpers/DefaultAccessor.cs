﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    class DefaultAccessor<T> : ExpressionCompiler, IAccessor<T> {
        IDictionary<string, Func<T, object>> _accessors;

        public DefaultAccessor()
            : base(typeof(T), true) {
        }

        public object Read(T obj, string selector) {
            if(_accessors == null)
                _accessors = new Dictionary<string, Func<T, object>>();

            Func<T, object> func;
            if(!_accessors.TryGetValue(selector, out func)) {
                var param = CreateItemParam();
                _accessors.Add(selector, func = Expression.Lambda<Func<T, object>>(
                    Expression.Convert(CompileAccessorExpression(param, selector), typeof(Object)),
                    param
                ).Compile());
            }

            return func(obj);
        }
    }

}
