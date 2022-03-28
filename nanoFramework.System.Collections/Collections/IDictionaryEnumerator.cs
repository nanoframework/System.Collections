//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Collections
{
    /// <summary>
    /// Enumerates the elements of a nongeneric dictionary.
    /// </summary>
    public interface IDictionaryEnumerator : IEnumerator
    {
        /// <summary>
        /// Gets the key of the current dictionary entry.
        /// </summary>
        /// <value>The key of the current element of the enumeration.</value>
        object Key
        {
            get;
        }

        /// <summary>
        /// Gets the value of the current dictionary entry.
        /// </summary>
        /// <value>The value of the current element of the enumeration.</value>
        object Value
        {
            get;
        }

        /// <summary>
        /// Gets both the key and the value of the current dictionary entry.
        /// </summary>
        /// <value>A <see cref="DictionaryEntry"/> containing both the key and the value of the current dictionary entry.</value>
        DictionaryEntry Entry
        {
            get;
        }
    }
}
