﻿// Accord Core Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

#if NET35
namespace Accord
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Minimum SortedSet implementation for .NET 3.5 to
    ///   make Accord.NET work. This is not a complete implementation.
    /// </summary>
    /// 
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    internal class SortedSet<T> : SortedList<T, int>, IEnumerable<T>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="SortedSet&lt;T&gt;"/> class.
        /// </summary>
        /// 
        public SortedSet()
            : base() { }

        /// <summary>
        ///   Determines whether the set contains the specified value.
        /// </summary>
        /// 
        /// <param name="value">The value.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if this object contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// 
        public bool Contains(T value)
        {
            return base.ContainsKey(value);
        }

        /// <summary>
        ///   Adds the specified value.
        /// </summary>
        /// 
        /// <param name="value">The value.</param>
        /// 
        public void Add(T value)
        {
            if (!ContainsKey(value))
                base.Add(value, 0);
        }

        /// <summary>
        ///   Gets the enumerator.
        /// </summary>
        /// 
        public new IEnumerator<T> GetEnumerator()
        {
            SortedList<T, int> b = this;

            foreach (var item in b)
                yield return item.Key;
        }
    }
}
#endif
