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

            if(!_accessors.ContainsKey(selector)) {
                var param = CreateItemParam();

                _accessors[selector] = Expression.Lambda<Func<T, object>>(
                    Expression.Convert(CompileAccessorExpression(param, selector), typeof(Object)),
                    param
                ).Compile();
            }

            return _accessors[selector](obj);
        }
    }

}
