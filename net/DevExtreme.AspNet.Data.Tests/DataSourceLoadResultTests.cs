using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceLoadResultTests {

        [Fact]
        public void DontSerializeDefaultExtras() {
            Assert.Equal(
                "{\"data\":null}",
                JsonConvert.SerializeObject(new LoadResult())
            );
        }

    }

}
