using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    static class EFSorting {

        static readonly ICollection<Type> LIKELY_KEY_TYPES = new[] {
            typeof(Int32), typeof(Int64), typeof(Guid)
        };

        static readonly ICollection<Type> OTHER_SORTABLE_TYPES = new[] {
            // https://msdn.microsoft.com/en-us/library/ee382832(v=vs.110).aspx
            typeof(String), typeof(Boolean), typeof(DateTime), typeof(Decimal),
            typeof(Double), typeof(DateTimeOffset), typeof(Byte), typeof(Single),
            typeof(Int16), typeof(SByte)
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
                    .Where(i => i.CanRead && i.CanWrite && i.GetMethod.IsPublic && i.SetMethod.IsPublic)
                    .Select(i => new Candidate(i, i.PropertyType)),
                entityType.GetRuntimeFields()
                    .Where(i => i.IsPublic)
                    .Select(i => new Candidate(i, i.FieldType))
            );

            return (
                candidates.FirstOrDefault(HasKeyAttr)
                ?? candidates.FirstOrDefault(i => LIKELY_KEY_TYPES.Contains(i.Type))
                ?? candidates.FirstOrDefault(i => OTHER_SORTABLE_TYPES.Contains(i.Type))
            )?.Member.Name;
        }

        static bool HasKeyAttr(Candidate candidate) {
            return candidate.Member.CustomAttributes.Any(i => i.AttributeType.FullName == "System.ComponentModel.DataAnnotations.KeyAttribute");
        } 

    }

}
