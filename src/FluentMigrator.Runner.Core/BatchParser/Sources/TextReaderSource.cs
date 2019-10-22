// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextReaderSource.cs" company="FluentMigrator Project">
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
using System.IO;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.BatchParser.Sources
{
    /// <summary>
    /// A <see cref="ITextSource" /> implementation that uses a <see cref="TextReader" /> as source.
    /// </summary>
    public class TextReaderSource : ITextSource, IDisposable
    {
        /// <summary>
        /// The reader
        /// </summary>
        private readonly TextReader _reader;
        /// <summary>
        /// The is owner
        /// </summary>
        private readonly bool _isOwner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReaderSource" /> class.
        /// </summary>
        /// <param name="reader">The text reader to use</param>
        /// <remarks>This function doesn't take ownership of the <paramref name="reader" />.</remarks>
        public TextReaderSource([NotNull] TextReader reader)
            : this(reader, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReaderSource" /> class.
        /// </summary>
        /// <param name="reader">The text reader to use</param>
        /// <param name="takeOwnership"><c>true</c> when the <see cref="TextReaderSource" /> should become the owner of the <paramref name="reader" /></param>
        public TextReaderSource([NotNull] TextReader reader, bool takeOwnership)
        {
            _reader = reader;
            _isOwner = takeOwnership;
        }

        /// <inheritdoc />
        public ILineReader CreateReader()
        {
            var currentLine = _reader.ReadLine();
            if (currentLine == null)
            {
                return null;
            }

            return new LineReader(_reader, currentLine, 0);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isOwner)
            {
                _reader.Dispose();
            }
        }

        /// <summary>
        /// Class LineReader.
        /// Implements the <see cref="FluentMigrator.Runner.BatchParser.ILineReader" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.BatchParser.ILineReader" />
        private class LineReader : ILineReader
        {
            /// <summary>
            /// The reader
            /// </summary>
            [NotNull]
            private readonly TextReader _reader;

            /// <summary>
            /// Initializes a new instance of the <see cref="LineReader"/> class.
            /// </summary>
            /// <param name="reader">The reader.</param>
            /// <param name="currentLine">The current line.</param>
            /// <param name="index">The index.</param>
            public LineReader([NotNull] TextReader reader, [NotNull] string currentLine, int index)
            {
                _reader = reader;
                Line = currentLine;
                Index = index;
            }

            /// <summary>
            /// Gets the current line
            /// </summary>
            /// <value>The line.</value>
            public string Line { get; }

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
                        var line = _reader.ReadLine();
                        if (line == null)
                            return null;
                        currentIndex = 0;
                        currentLine = line;
                        remaining = currentLine.Length;
                    } while (length >= remaining && length != 0);
                }

                currentIndex += length;
                return new LineReader(_reader, currentLine, currentIndex);
            }
        }
    }
}
