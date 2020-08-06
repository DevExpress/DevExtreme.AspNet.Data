using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Types {

    class AnonTypeFacade {
        readonly IReadOnlyCollection<Expression> MemberExpressions;

        public AnonTypeFacade(IReadOnlyCollection<Expression> memberExpressions) {
            MemberExpressions = memberExpressions;
        }

        public int MemberCount => MemberExpressions.Count;

        public Expression CreateNewExpression(AnonTypeNewTweaks tweaks) {
            return AnonType.CreateNewExpression(MemberExpressions, tweaks);
        }

        public MemberExpression CreateMemberAccessor(Expression expr, int index) {
            return Expression.Field(expr, AnonType.IndexToField(index));
        }
    }

}
