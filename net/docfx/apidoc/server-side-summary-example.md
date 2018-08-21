---
uid: DevExtreme.AspNet.Data.Aggregation.CustomAggregators
remarks: *content
---
When implementing a custom data aggregator, derive from the [`Aggregator<T>`](/api/DevExtreme.AspNet.Data.Aggregation.Aggregator-1.html) class as shown in the following example. The custom aggregator in it calculates total sales:

[!code-csharp[Main](../../DevExtreme.AspNet.Data.Tests/TotalSalesAggregator.cs#class)]

Then, register the aggregator under a string identifier at the application's start (for instance, in Global.asax or Startup.cs):

```csharp
CustomAggregators.RegisterAggregator("totalSales", typeof(TotalSalesAggregator<>));
```

> [!Important]
> Custom aggregators get ignored if the LINQ provider groups data and calculates summaries. Set [`RemoteGrouping`](/api/DevExtreme.AspNet.Data.DataSourceLoadOptionsBase.html#DevExtreme_AspNet_Data_DataSourceLoadOptionsBase_RemoteGrouping) to `false` for custom aggregators to apply.