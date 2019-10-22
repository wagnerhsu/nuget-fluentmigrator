// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="LinesSource.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.BatchParser.Sources
{
    /// <summary>
    /// A <see cref="ITextSource" /> implementation that uses lines as input
    /// </summary>
    public class LinesSource : ITextSource
    {
        /// <summary>
        /// The batch source
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly IEnumerable<string> _batchSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinesSource" /> class.
        /// </summary>
        /// <param name="batchSource">The collection of lines to be used as source</param>
        public LinesSource([NotNull, ItemNotNull] IEnumerable<string> batchSource)
        {
            _batchSource = batchSource;
        }

        /// <inheritdoc />
        public ILineReader CreateReader()
        {
            var enumerator = _batchSource.GetEnumerator();
            if (!enumerator.MoveNext())
                return null;
            return new LineReader(enumerator, 0);
        }

        /// <summary>
        /// Class LineReader.
        /// Implements the <see cref="FluentMigrator.Runner.BatchParser.ILineReader" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.BatchParser.ILineReader" />
        private class LineReader : ILineReader
        {
            /// <summary>
            /// The enumerator
            /// </summary>
            [NotNull]
            private readonly IEnumerator<string> _enumerator;

            /// <summary>
            /// Initializes a new instance of the <see cref="LineReader"/> class.
            /// </summary>
            /// <param name="enumerator">The enumerator.</param>
            /// <param name="index">The index.</param>
            /// <exception cref="InvalidOperationException">The returned line must not be null</exception>
            public LineReader([NotNull] IEnumerator<string> enumerator, int index)
            {
                _enumerator = enumerator;
                Index = index;
                Line = _enumerator.Current ?? throw new InvalidOperationException("The returned line must not be null");
            }

            /// <summary>
            /// Gets the current line
            /// </summary>
            /// <value>The line.</value>
            public string Line { get; private set; }

            /// <summary>
            /// Gets the current index into the line
            /// </summary>
            /// <value>The index.</value>
            public int Index { get; }

            /// <summary>
            /// Gets the remaining length
            /// </summary>
            /// <value>The length.</value>
            public int Length => Line.Length - Index;

            /// <summary>
            /// Reads a string with the given <paramref name="length" /> from the <see cref="Line" />
            /// </summary>
            /// <param name="length">The length of the string to read from the <see cref="Line" /></param>
            /// <returns>The read string</returns>
            public string ReadString(int length)
            {
                return Line.Substring(Index, length);
            }

            /// <summary>
            /// Advances the specified length.
            /// </summary>
            /// <param name="length">The length.</param>
            /// <returns>ILineReader.</returns>
            /// <exception cref="InvalidOperationException">The returned line must not be null</exception>
            public ILineReader Advance(int length)
            {
                var currentLine = Line;
                var currentIndex = Index;
                var remaining = currentLine.Length - currentIndex;

                if (length >= remaining)
                {
                    do
                    {
                        length -= remaining;
                        if (!_enumerator.MoveNext())
                            return null;

                        currentIndex = 0;
                        currentLine = _enumerator.Current ?? throw new InvalidOperationException("The returned line must not be null");
                        remaining = currentLine.Length;
                    } while (length >= remaining && length != 0);
                }

                Line = currentLine;
                currentIndex += length;
                return new LineReader(_enumerator, currentIndex);
            }
        }
    }
}
