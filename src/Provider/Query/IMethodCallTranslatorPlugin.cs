// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public interface IMethodCallTranslatorPlugin
    {
        IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
