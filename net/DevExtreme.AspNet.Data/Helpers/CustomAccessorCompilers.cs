using AutoMapper;
using AutoMapper.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class CustomAccessorCompilers {
        public delegate Expression CompilerFunc(Expression expr, string accessorText);
        public delegate Expression CompilerFuncWithContext(Expression expr, string accessorText, object runtimeContext);

        static readonly ICollection<CompilerFuncWithContext> _compilersWithContext = new List<CompilerFuncWithContext>();

        private static AccessorLibrary CustomAccessorLibrary = new AccessorLibrary();

        public static void Register(CompilerFunc compilerFunc) {
            _compilersWithContext.Add((target, accessorText, runtimeContext) => compilerFunc(target, accessorText));
        }
        public static void RegisterWithContext(CompilerFuncWithContext compilerFunc) {
            _compilersWithContext.Add(compilerFunc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="PropertyName"></param>
        /// <param name="ResolveExpression"></param>
        public static void Register<T, U>(string PropertyName, Expression<Func<T, U>> ResolveExpression) {
            CustomAccessorLibrary.Add(PropertyName, ResolveExpression);
        }
        public static void RegisterContext<T, U>(string PropertyName, Func<object, Expression<Func<T, U>>> ResolveExprFunc) {
            CustomAccessorLibrary.Add<T>(PropertyName, ResolveExprFunc);
        }

        public static void RegisterContext<T>(string PropertyName, Func<object, LambdaExpression> ResolveExprFunc) {
            CustomAccessorLibrary.Add<T>(PropertyName, ResolveExprFunc);
        }

        public static void RegisterAutomapperProfiles(IMapper mapper) {

            //Get all automapper config maps and create expressions that can be parsed by devex.aspnet.data
            //Create a dictionary of dictionaries so they can be (efficiently) extracted by the devex library 
            //by type and property name
            var allTypeMaps = mapper.ConfigurationProvider.Internal().GetAllTypeMaps();
            foreach(TypeMap map in allTypeMaps) {
                var propertyMaps = map.PropertyMaps;

                foreach(PropertyMap propertyMap in propertyMaps) {
                    string modelMemberType = propertyMap?.TypeMap?.SourceType?.Name;
                    string destinationName = propertyMap?.DestinationName;
                    var exp = propertyMap.CustomMapExpression;
                    if(modelMemberType != null && destinationName != null && exp == null) continue;

                    CustomAccessorLibrary.Add(modelMemberType, destinationName, exp);
                }
            }
            RegisterAccessorFinder();
        }

        private static void RegisterAccessorFinder() {

            //Register the method that finds the right accessor from the dictionary. This will provide
            //an accessor for the registered Automapper and 
            RegisterWithContext((target, accessorText, runtimeContext) => {
                var accessor = CustomAccessorLibrary.Get(target, target.Type.Name, accessorText, runtimeContext);
                if(accessor != null) return accessor;

                return null;
            });
        }

        internal static Expression TryCompile(Expression expr, string accessorText, object runtimeContext) {
            if(_compilersWithContext.Count >= 1) {
                foreach(var compiler in _compilersWithContext) {
                    var result = compiler(expr, accessorText, runtimeContext);
                    if(result != null)
                        return result;
                }
            }

            return null;
        }

#if DEBUG
        internal static void Clear() {
            _compilersWithContext.Clear();
        }
#endif

    }

    public class AccessorLibrary {
        ConcurrentDictionary<string, ConcurrentDictionary<string, Accessor>> _dctAccessors = new ConcurrentDictionary<string, ConcurrentDictionary<string, Accessor>>();
        public AccessorLibrary() {
        }

        public void Add(string TypeName, string PropertyName, LambdaExpression ResolveExpression) {
            var _accessor = new Accessor(TypeName, PropertyName, ResolveExpression);
            if(_dctAccessors.ContainsKey(TypeName)) {
                var expressionForType = _dctAccessors[TypeName];
                expressionForType[PropertyName] = _accessor;
            } else {
                var accessorDct = new ConcurrentDictionary<string, Accessor>() { [PropertyName] = _accessor };
                _dctAccessors.TryAdd(TypeName, accessorDct);
            }
        }

        public void Add(string TypeName, string PropertyName, Func<object, LambdaExpression> ResolveExprFunc) {
            var _accessor = new Accessor(TypeName, PropertyName, ResolveExprFunc);
            if(_dctAccessors.ContainsKey(TypeName)) {
                var expressionForType = _dctAccessors[TypeName];
                expressionForType[PropertyName] = _accessor;
            } else {
                var accessorDct = new ConcurrentDictionary<string, Accessor>() { [PropertyName] = _accessor };
                _dctAccessors.TryAdd(TypeName, accessorDct);
            }
        }
        public void Add<T>(string PropertyName, Func<object, LambdaExpression> ResolveExprFunc) {
            Add(typeof(T).Name, PropertyName, ResolveExprFunc);
        }

        public void Add<T>(string PropertyName, LambdaExpression ResolveExpression) {
            Add(typeof(T).Name, PropertyName, ResolveExpression);
        }

        public void Add<T, U>(string PropertyName, Expression<Func<T, U>> ResolveExpression) {
            Add(typeof(T).Name, PropertyName, ResolveExpression);
        }

        public Expression Get(Expression target, string TypeName, string PropertyName, object runtimeContext) {
            if(_dctAccessors.ContainsKey(TypeName)) {
                var expressionForType = _dctAccessors[TypeName];
                if(expressionForType.ContainsKey(PropertyName)) {
                    var expression = expressionForType[PropertyName].GetResolvedExpression(runtimeContext);
                    return new ParameterVisitor(expression.Parameters, target as ParameterExpression)
                        .VisitAndConvert(expression.Body, PropertyName);
                }
            }

            return null;
        }
    }

    public class Accessor {
        public string TypeName { get; set; }
        public string PropertyName { get; set; }
        Func<object, LambdaExpression> ResolveExpression { get; set; }

        public Accessor() {
        }
        public Accessor(string typeName, string propertyName, LambdaExpression resolveExpression) {
            TypeName = typeName;
            PropertyName = propertyName;
            ResolveExpression = (o) => resolveExpression;
        }
        public Accessor(string typeName, string propertyName, Func<object, LambdaExpression> resolveExpressionFunc) {
            TypeName = typeName;
            PropertyName = propertyName;
            ResolveExpression = resolveExpressionFunc;
        }

        public LambdaExpression GetResolvedExpression(object runtimeContext) {
            return ResolveExpression(runtimeContext);
        }
    }

    public class ParameterVisitor : ExpressionVisitor {
        private readonly ReadOnlyCollection<ParameterExpression> _from;
        private readonly ParameterExpression _to;
        public ParameterVisitor(
            ReadOnlyCollection<ParameterExpression> from,
            ParameterExpression to) {
            if(from == null) throw new ArgumentNullException("from");
            if(to == null) throw new ArgumentNullException("to");
            this._from = from;
            this._to = to;
        }
        protected override Expression VisitParameter(ParameterExpression node) {
            for(int i = 0; i < _from.Count; i++) {
                if(node == _from[i]) return _to;
            }
            return node;
        }
    }
}
