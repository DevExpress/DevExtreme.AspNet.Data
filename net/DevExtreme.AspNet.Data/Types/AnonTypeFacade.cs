using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Types {

    class AnonTypeFacade {
        readonly IReadOnlyCollection<Expression> MemberExpressions;

        public AnonTypeFacade(IReadOnlyCollection<Expression> memberExpressions) {
            MemberExpressions = memberExpressions;
        }

        public int MemberCount => MemberExpressions.Count;

        bool UseBuiltInTypes => MemberCount <= AnonType.MAX_SIZE;

        public Expression CreateNewExpression(AnonTypeNewTweaks tweaks) {
            if(UseBuiltInTypes)
                return AnonType.CreateNewExpression(MemberExpressions, tweaks);

            var typeArguments = MemberExpressions.Select(i => i.Type).ToArray();
            var type = DynamicClassBridge.CreateType(typeArguments);
            return AnonType.CreateNewExpression(type, typeArguments, MemberExpressions, false);
        }

        public MemberExpression CreateMemberAccessor(Expression expr, int index) {
            var name = AnonType.IndexToField(index);
            return UseBuiltInTypes
                ? Expression.Field(expr, name)
                : Expression.Property(expr, name);
        }
    }

}
