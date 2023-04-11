using AutoMapper;
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class CustomAccessorCompilers {
        public delegate Expression CompilerFunc(Expression expr, string accessorText);

        static readonly ICollection<CompilerFunc> _compilers = new List<CompilerFunc>();

        private static AccessorLibrary CustomAccessorLibrary = new AccessorLibrary();

        public static void Register(CompilerFunc compilerFunc) {
            _compilers.Add(compilerFunc);
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
            Register((target, accessorText) => {
                var accessor = CustomAccessorLibrary.Get(target, target.Type.Name, accessorText);
                if(accessor != null) return accessor;

                return null;
            });
        }

        internal static Expression TryCompile(Expression expr, string accessorText) {
            if(_compilers.Count < 1)
                return null;

            foreach(var compiler in _compilers) {
                var result = compiler(expr, accessorText);
                if(result != null)
                    return result;
            }

            return null;
        }

#if DEBUG
        internal static void Clear() {
            _compilers.Clear();
        }
#endif

    }

    public class AccessorLibrary {
        Dictionary<string, Dictionary<string, Accessor>> _dctAccessors = new Dictionary<string, Dictionary<string, Accessor>>();
        public AccessorLibrary() {
        }

        public void Add(string TypeName, string PropertyName, LambdaExpression ResolveExpression) {
            var _accessor = new Accessor(TypeName, PropertyName, ResolveExpression);
            if(_dctAccessors.ContainsKey(TypeName)) {
                var expressionForType = _dctAccessors[TypeName];
                expressionForType[PropertyName] = _accessor;
            } else _dctAccessors.Add(TypeName, new Dictionary<string, Accessor>() { [PropertyName] = _accessor });
        }

        public void Add<T>(string PropertyName, LambdaExpression ResolveExpression) {
            Add(typeof(T).Name, PropertyName, ResolveExpression);
        }

        public void Add<T, U>(string PropertyName, Expression<Func<T, U>> ResolveExpression) {
            Add(typeof(T).Name, PropertyName, ResolveExpression);
        }

        public Expression Get(Expression target, string TypeName, string PropertyName) {
            if(_dctAccessors.ContainsKey(TypeName)) {
                var expressionForType = _dctAccessors[TypeName];
                if(expressionForType.ContainsKey(PropertyName)) {
                    var expression = expressionForType[PropertyName].ResolveExpression;
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
        public LambdaExpression ResolveExpression { get; set; }
        public Accessor() {
        }
        public Accessor(string typeName, string propertyName, LambdaExpression resolveExpression) {
            TypeName = typeName;
            PropertyName = propertyName;
            ResolveExpression = resolveExpression;
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
