﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public interface IAccessor<T> {
        object Read(T container, string selector);
    }

}
