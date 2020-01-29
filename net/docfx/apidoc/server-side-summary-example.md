---
uid: DevExtreme.AspNet.Data.Aggregation.CustomAggregators
remarks: *content
---
When implementing a custom data aggregator, derive it from the [`Aggregator<T>`](xref:DevExtreme.AspNet.Data.Aggregation.Aggregator`1) class as shown in the following example. The custom aggregator in it calculates total sales:

[!code-csharp[Main](../../DevExtreme.AspNet.Data.Tests/TotalSalesAggregator.cs#class)]

Then, register the aggregator under a string identifier at the application's start (for instance, in Global.asax or Startup.cs):

```csharp
CustomAggregators.RegisterAggregator("totalSales", typeof(TotalSalesAggregator<>));
```

> [!Important]
> Custom aggregators are ignored if the LINQ provider groups data and calculates summaries. Set [RemoteGrouping](xref:DevExtreme.AspNet.Data.DataSourceLoadOptionsBase.RemoteGrouping) to `false` for custom aggregators to apply.