﻿using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests.L2S {

    partial class TestDataContext {
        static readonly object LOCK = new object();
        static TestDataContext INSTANCE;

        public static void Exec(Action<TestDataContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    var helper = new SqlServerTestDbHelper("L2S");
                    helper.ResetDatabase();

                    INSTANCE = new TestDataContext(helper.ConnectionString);

                    INSTANCE.ExecuteCommand(
                        $@"create table {nameof(RemoteGroupingStress_DataItem)} (
                            {nameof(RemoteGroupingStress_DataItem.ID)} int identity primary key,
                            {nameof(RemoteGroupingStress_DataItem.Num)} int not null,
                            {nameof(RemoteGroupingStress_DataItem.NullNum)} int,
                            {nameof(RemoteGroupingStress_DataItem.Date)} datetime2 not null,
                            {nameof(RemoteGroupingStress_DataItem.NullDate)} datetime2
                        )"
                    );
                }

                action(INSTANCE);
            }
        }

    }

    partial class RemoteGroupingStress_DataItem : RemoteGroupingStressHelper.IEntity {
    }

}
