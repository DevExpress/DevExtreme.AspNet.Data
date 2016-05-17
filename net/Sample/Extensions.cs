using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample {

    static class Extensions {

        public static string ToFullErrorString(this ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState.Values) {
                foreach(var error in entry.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }

    }

}
