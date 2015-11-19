using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DevExtreme.AspNet.Data {

#if MVC5

    using System.Web.Mvc;

    partial class DataSourceLoadOptionsBinder : IModelBinder {

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            return BindCore(bindingContext.ValueProvider);
        }

        static T ReadValue<T>(IValueProvider provider, string name) {
            var result = provider.GetValue(name);
            if(result == null)
                return default(T);

            return (T)Convert.ChangeType(result.AttemptedValue, typeof(T));
        }
    }

#else

    using Microsoft.AspNet.Mvc.ModelBinding;

    partial class DataSourceLoadOptionsBinder : IModelBinder {

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext) {
            return Task.FromResult(ModelBindingResult.Success(bindingContext.ModelName, BindCore(bindingContext.ValueProvider)));
        }

        static T ReadValue<T>(IValueProvider provider, string name) {
            var result = provider.GetValue(name);
            if(result == ValueProviderResult.None)
                return default(T);

            return (T)Convert.ChangeType(result.FirstValue, typeof(T));
        }
    }

#endif

    partial class DataSourceLoadOptionsBinder {

        DataSourceLoadOptions BindCore(IValueProvider provider) {
            var model = new DataSourceLoadOptions {
                Skip = ReadValue<int>(provider, "skip"),
                Take = ReadValue<int>(provider, "take"),
                IsCountQuery = ReadValue<bool>(provider, "isCountQuery"),
                RequireTotalCount = ReadValue<bool>(provider, "requireTotalCount")
            };

            var filterJson = ReadValue<string>(provider, "filter");
            if(filterJson != null)
                model.Filter = JsonConvert.DeserializeObject<IList>(filterJson);

            var sortJson = ReadValue<string>(provider, "sort");
            if(sortJson != null)
                model.Sort = JsonConvert.DeserializeObject<SortingInfo[]>(sortJson);

            return model;
        }

    }    

}

