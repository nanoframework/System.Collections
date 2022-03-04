//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Collections
{
    internal class Bucket
    {
        public object _key;
        public object _value;
        public uint _hash;

        public Bucket(
            object key,
            object value,
            uint hash)
        {
            _key = key;
            _value = value;
            _hash = hash;
        }
    }
}
