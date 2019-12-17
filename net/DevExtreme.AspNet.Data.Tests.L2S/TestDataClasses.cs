using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests.L2S {

    partial class TestDataContext {
        static TestDataContext INSTANCE;

        public static void Exec(Action<TestDataContext> action) {
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

                INSTANCE.ExecuteCommand(
                    $@"create table {nameof(RemoteGroupCount_DataItem)} (
                        {nameof(RemoteGroupCount_DataItem.ID)} int identity primary key,
                        {nameof(RemoteGroupCount_DataItem.G1)} int not null,
                        {nameof(RemoteGroupCount_DataItem.G2)} int not null
                    )"
                );

                INSTANCE.ExecuteCommand(
                    $@"create table {nameof(Summary_DataItem)} (
                        {nameof(Summary_DataItem.ID)} int identity primary key,
                        {nameof(Summary_DataItem.Group1)} nvarchar,
                        {nameof(Summary_DataItem.Group2)} nvarchar,
                        {nameof(Summary_DataItem.Value)} int
                    )"
                );

                INSTANCE.ExecuteCommand(
                    $@"create table {nameof(GenericTestDataItem)} (
                        {nameof(GenericTestDataItem.ID)} int identity primary key,
                        {nameof(GenericTestDataItem.Num)} int
                    )"
                );
            }

            action(INSTANCE);
        }

        public void PurgeGenericTestTable() {
            ExecuteCommand("delete from " + nameof(GenericTestDataItem));
        }
    }

    partial class RemoteGroupingStress_DataItem : RemoteGroupingStressHelper.IEntity {
    }

    partial class RemoteGroupCount_DataItem : RemoteGroupCountTestHelper.IEntity {
    }

    partial class Summary_DataItem : SummaryTestHelper.IEntity {
    }

}
