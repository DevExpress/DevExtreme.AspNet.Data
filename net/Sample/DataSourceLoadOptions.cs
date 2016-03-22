using DevExtreme.AspNet.Data;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample {

    [ModelBinder(BinderType = typeof(DataSourceLoadOptionsBinder))]
    public class DataSourceLoadOptions : DataSourceLoadOptionsBase {
    }

    public class DataSourceLoadOptionsBinder : IModelBinder {

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext) {
            var values = DataSourceLoadOptionsBase.ALL_KEYS.ToDictionary(
                i => i,
                i => bindingContext.ValueProvider.GetValue(i).FirstOrDefault()
            );

            return ModelBindingResult.SuccessAsync(
                bindingContext.ModelName, 
                DataSourceLoadOptionsBase.Parse<DataSourceLoadOptions>(values)
            );
        }

    }

}
