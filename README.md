**********************************************************************************************************************************************

This is a fork of Devexpress.AspNet.Data, a fantastic expression building library for querying an API. While designed to work with Devex controls, it also works as the base for free querying of Entity Framework Core and XPO data models.

Its main shortcoming in my use case was a lack of support for Automapper style projections. This fork provides those projections and also changes to support the injection of automapper parameters for use in customfilters.

**********************************************************************************************************************************************

How to use:

The basic functionality is exactly the same as the devex library. The big difference is that the user can:
1. Specify a projection type to the LoadAsync method (ProjectTo in automapper). As long as this type is included in the registered mapping config(s), the projection will occur seamlessly with the results in the returned object. While it is possible to specify an (already projected) IQueryable to the LoadAsync in the base library, you lose the capacity to filter, sort or group on properties of the base object that aren't in the projection. **This library supports querying any property of the base object OR the mapped object**
2. Specify parameters to the LoadAsync method that will be accessible in the custom filters added through CustomFilterCompilers. This is important when you are passing parameters to an automapper ProjectTo that would change filters applied to the base object.
3. Specify that filters apply after a projection using the new DataSourceLoadOption property ProjectBeforeFilter
4. Utilise the devex expression builders for binary expressions by calling CompileNonCustomBinary
5. From Version 1.11, CustomAccessors also have access to the tuntime context (Automapper object)


**********************************************************************************************************************************************

Quick Start:
1. Register your mappings:

In startup register automapper as per usual
    
        public void ConfigureServices(IServiceCollection services)
        {
            ....
            List<Assembly> lstAssembly = new List<Assembly>() { typeof(AutoMapperProfileService).GetTypeInfo().Assembly, typeof(AutoMapperProfileORM).GetTypeInfo().Assembly };
            services.AddAutoMapper(lstAssembly);
            ....
        }

In configure, call CustomAccessorCompilers.RegisterAutomapperProfiles. You can also add other accessors for filtering and sorting if you like.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ....

            CustomFilters.RegisterFilters(); //
            CustomAccessorCompilers.RegisterAutomapperProfiles(app.ApplicationServices.GetService<IMapper>());
            //Additional mappers can be added like this:
            //CustomAccessorLibrary.Add<AreaCode,string>("FirstLetter", t => t.AreaCodeName.FirstOrDefault().ToString().ToUpper());
            //Or like this - a separate static class/method is recommended
            //CustomAccessorCompilers.RegisterContext<Notification, DateTime?>("DateRead", (u) =>
            //{
            //    dynamic dynObj = new DynamicWrapper(u);
            //    if (dynObj.UserId is int)
            //    {
            //        int UserId = dynObj.UserId;
            //        return src => src.NotificationTos.Where(x => x.UserId == UserId).OrderByDescending(x => x.NotificationTypeId).Select(x => x.DateRead).FirstOrDefault();
            //    }
            //    return src => null;
            //});
            ....
        }

2. If you have any custom filters requiring additional context (such as an object that is used in the Projection - refer below), you can register your filters using the RegisterBinaryExpressionCompilerWithContext. This works like devexpresses RegisterBinaryExpressionCompiler method, but includes the context. RegisterBinaryExpressionCompiler will still work for simpler maps. The example uses linqKit



		public static partial class CustomFilters
		{
			public static void RegisterFilters()
			{
				CustomFilterCompilers.RegisterBinaryExpressionCompilerWithContext((info, rtContext) =>
				{
					if (info.DataItemExpression.Type == typeof(Notification))
					{
						if (info.AccessorText == "DateDismissed")
						{
							dynamic dynObj = new DynamicWrapper(rtContext.RuntimeResolutionContext);
							if (int.TryParse(dynObj.UserId.ToString(), out int _ui))
							{
								var pBaseExp = info.DataItemExpression as ParameterExpression;
								var pBaseProperty = Expression.PropertyOrField(pBaseExp, "NotificationTos");

								var _p = Expression.Parameter(typeof(NotificationTo), "nTo");
								var baseResult = rtContext.CompileNonCustomBinary(_p, GetParametersAsList(info));
								var predicate = PredicateBuilder.New(Expression.Lambda<Func<NotificationTo, bool>>(baseResult, _p));
								predicate.And((p) => p.UserId == _ui);

								var result = Expression.Call(
									typeof(Enumerable), "Any", new[] { _p.Type },
									pBaseProperty, predicate);

								var ex22 =  Expression.Lambda(result, pBaseExp) as Expression<Func<Notification, bool>>;
								return CompileWhereExpression(info, ex22).Body;
							}
						}
					}
					return null;
				});
			}
		}

        static IList GetParametersAsList(IBinaryExpressionInfo info)
        {
            var criteria = new List<object>() { info.AccessorText };
            if (!string.IsNullOrWhiteSpace(info.Operation)) criteria.Add(info.Operation);
            criteria.Add(info.Value);
            return criteria;
        }

3. Call your projection from LoadAsyncDto (or LoadDto)

        return await DataSourceLoader.LoadAsync<T, TDto>(source, options, mapper, MapParameters);
    
    mapper is your IMapper for automap
    MapParameters are an object (usually null) that can be used for injecting context into a Projection. It is the second parameter of Automappers ProjectTo.



**********************************************************************************************************************************************

How it works:
- Beyond the mundane, there are three principle things here:
-- RegisterAutomapperProfiles iterates through every automapper map and creates a library of custom expressions that is resolved by a CompilerFunc registered using the base library's CustomAccessorCompilers.Register method. When expressions for sorting, filtering etc are constructed for a type, this library is queried to assemble any properties existing in the automap.
-- LoadAsync<T, TDto> fetches any map between T and TDto and constructs a select expression that is then injected into the devexpress expression resolution for select as if it was passed as an option. The output object is typed to the TDto.
-- The CompileNonCustomBinary is available in CustomFilters registered using RegisterBinaryExpressionCompilerWithContext. This means the devexpress Expression compiler for all operators such as "=", "<>" "<" ">" "contains" etc can be resolved without explicitly doing it yourself. This method is in the instance of FilterExpressionCompiler passed into the custom filter (second parameter)

**********************************************************************************************************************************************




# DevExtreme ASP.NET Data

[![CI](https://github.com/DevExpress/DevExtreme.AspNet.Data/actions/workflows/ci.yml/badge.svg?branch=master&event=push)](https://github.com/DevExpress/DevExtreme.AspNet.Data/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data/branch/master/graph/badge.svg)](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data)
[![NuGet](https://img.shields.io/nuget/v/DevExtreme.AspNet.Data.svg?maxAge=43200)](https://www.nuget.org/packages/DevExtreme.AspNet.Data)
[![npm](https://img.shields.io/npm/v/devextreme-aspnet-data.svg?maxAge=43200)](https://www.npmjs.com/package/devextreme-aspnet-data)
[![npm nojquery](https://img.shields.io/npm/v/devextreme-aspnet-data-nojquery.svg?maxAge=43200&label=npm+nojquery)](https://www.npmjs.com/package/devextreme-aspnet-data-nojquery)

This library enables [DevExtreme client-side widgets](https://js.devexpress.com) to perform CRUD operations via ASP.NET controllers and handlers and allows you to delegate all data-intensive operations to the server:

* a widget sends data loading options (filtering, grouping, sorting, etc.) to the server;
* the server processes data according to these options;
* the processed data is sent back to the widget.

Can be used with:

* [DevExtreme client-side widgets](https://js.devexpress.com)
* [DevExtreme-based ASP.NET Core controls](https://docs.devexpress.com/AspNetCore/400263)
* [DevExtreme ASP.NET MVC 5 controls](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/)

## Installation and Configuration

`DevExtreme.AspNet.Data` consists of server-side and client-side parts. The following topics explain how to install and configure these parts:

- [Server Side Configuration](docs/server-side-configuration.md)
- [Client Side with jQuery](docs/client-side-with-jquery.md)
- [Client Side without jQuery (Angular, etc.)](docs/client-side-without-jquery.md)

## CI Builds

We recommend that you use [release builds](https://github.com/DevExpress/DevExtreme.AspNet.Data/releases). However, you can also use [CI builds](docs/using-ci-builds.md) to get urgent bug fixes or to test unreleased functionality.
