﻿using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.ResponseModel;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    class GroupHelper<T> {
        readonly static object NULL_KEY = new object();

        IAccessor<T> _accessor;

        public GroupHelper(IAccessor<T> accessor) {
            _accessor = accessor;
        }

        public List<Group> Group(IEnumerable<T> data, IEnumerable<GroupingInfo> groupInfo) {
            var groups = Group(data, groupInfo.First());

            if(groupInfo.Count() > 1) {
                groups = groups
                    .Select(g => new Group {
                        key = g.key,
                        items = Group(g.items.Cast<T>(), groupInfo.Skip(1))
                    })
                    .ToList();
            }

            return groups;
        }


        List<Group> Group(IEnumerable<T> data, GroupingInfo groupInfo) {
            var groupsIndex = new Dictionary<object, Group>();
            var groups = new List<Group>();

            foreach(var item in data) {
                var groupKey = GetKey(item, groupInfo);
                var groupIndexKey = groupKey ?? NULL_KEY;

                Group group;
                if(!groupsIndex.TryGetValue(groupIndexKey, out group)) {
                    var newGroup = new Group { key = groupKey };
                    groupsIndex.Add(groupIndexKey, group = newGroup);
                    groups.Add(newGroup);
                }

                if(group.items == null)
                    group.items = new List<T>();
                group.items.Add(item);
            }

            return groups;
        }

        object GetKey(T obj, GroupingInfo groupInfo) {
            var memberValue = _accessor.Read(obj, groupInfo.Selector);

            var intervalString = groupInfo.GroupInterval;
            if(String.IsNullOrEmpty(intervalString) || memberValue == null)
                return memberValue;

            if(Char.IsDigit(intervalString[0])) {
                var number = Convert.ToDecimal(memberValue);
                var interval = Decimal.Parse(intervalString);
                return number - number % interval;
            }

            switch(intervalString) {
                case "year":
                    return ToDateTime(memberValue).Year;
                case "quarter":
                    return (ToDateTime(memberValue).Month + 2) / 3;
                case "month":
                    return ToDateTime(memberValue).Month;
                case "day":
                    return ToDateTime(memberValue).Day;
                case "dayOfWeek":
                    return (int)ToDateTime(memberValue).DayOfWeek;
                case "hour":
                    return ToDateTime(memberValue).Hour;
                case "minute":
                    return ToDateTime(memberValue).Minute;
                case "second":
                    return ToDateTime(memberValue).Second;
            }

            throw new NotSupportedException();
        }

        static DateTime ToDateTime(object value) {
            if(value is DateTimeOffset offset)
                return offset.DateTime;

#if NET6_0_OR_GREATER
            if(value is DateOnly date)
                return date.ToDateTime(TimeOnly.MinValue);
#endif

            return Convert.ToDateTime(value);
        }
    }

}
