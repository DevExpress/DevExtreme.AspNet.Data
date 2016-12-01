using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    static class EFSorting {

        static readonly IEnumerable<Type> ORDERED_SORTABLE_TYPES = new[] {
            // https://msdn.microsoft.com/en-us/library/ee382832(v=vs.110).aspx

            // types usually used for keys
            typeof(Int32), typeof(Int64), typeof(Guid), typeof(String),

            // other types
            typeof(Boolean), typeof(Decimal), typeof(Double),
            typeof(DateTime), typeof(DateTimeOffset),
            typeof(Byte), typeof(SByte), typeof(Single), typeof(Int16)
        };

        class Candidate {
            public readonly MemberInfo Member;
            public readonly Type Type;

            public Candidate(MemberInfo member, Type type) {
                Member = member;
                Type = Utils.StripNullableType(type);
            }
        }

        public static string FindSortableMember(Type entityType) {
            var candidates = Enumerable.Concat(
                entityType.GetRuntimeProperties()
                    .Where(i => i.CanRead && i.CanWrite && i.GetGetMethod(true).IsPublic && i.GetSetMethod(true).IsPublic)
                    .Select(i => new Candidate(i, i.PropertyType)),
                entityType.GetRuntimeFields()
                    .Where(i => i.IsPublic)
                    .Select(i => new Candidate(i, i.FieldType))
            );

            var codeFirstId = candidates.FirstOrDefault(IsEFCodeFirstConventionalKey);
            if(codeFirstId != null)
                return codeFirstId.Member.Name;

            return ORDERED_SORTABLE_TYPES.SelectMany(type => candidates.Where(c => c.Type == type)).FirstOrDefault()?.Member.Name;
        }

        static bool IsEFCodeFirstConventionalKey(Candidate candidate) {
            var member = candidate.Member;
            var memberName = member.Name;

            if(String.Compare(memberName, "id", true) != 0 && String.Compare(memberName, member.DeclaringType.Name + "id", true) != 0)
                return false;

            return ORDERED_SORTABLE_TYPES.Contains(candidate.Type);
        }

    }

}
