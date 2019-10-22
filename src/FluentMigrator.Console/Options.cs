// ***********************************************************************
// Assembly         : Migrate
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Options.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************

// Compile With:
//   gmcs -debug+ -r:System.Core Options.cs -o:NDesk.Options.dll
//   gmcs -debug+ -d:LINQ -r:System.Core Options.cs -o:NDesk.Options.dll
//
// The LINQ version just changes the implementation of
// OptionSet.Parse(IEnumerable<string>), and confers no semantic changes.

//
// A Getopt::Long-inspired option parsing library for C#.
//
// NDesk.Options.OptionSet is built upon a key/value table, where the
// key is a option format string and the value is a delegate that is
// invoked when the format string is matched.
//
// Option format strings:
//  Regex-like BNF Grammar:
//    name: .+
//    type: [=:]
//    sep: ( [^{}]+ | '{' .+ '}' )?
//    aliases: ( name type sep ) ( '|' name type sep )*
//
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.  `=' or `:' need only be defined on one
// alias, but if they are provided on more than one they must be consistent.
//
// Each alias portion may also end with a "key/value separator", which is used
// to split option values if the option accepts > 1 value.  If not specified,
// it defaults to '=' and ':'.  If specified, it can be any character except
// '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
// used (i.e. the separate values should be distinct arguments), then "{}"
// should be used as the separator.
//
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The current option requires a value (i.e. not a Option type of ':')
//
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
//
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options are a single character
//   - at most one of the bundled options accepts a value, and the value
//     provided starts from the next character to the end of the string.
//
// This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
// as '-Dname=value'.
//
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by OptionSet.Parse() unchanged and unprocessed.
//
// Unprocessed options are returned from OptionSet.Parse().
//
// Examples:
//  int verbose = 0;
//  OptionSet p = new OptionSet ()
//    .Add ("v", v => ++verbose)
//    .Add ("name=|value=", v => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
//
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.
// It would also print out "A" and "B" to standard output.
// The returned array would contain the string "extra".
//
// C# 3.0 collection initializers are supported and encouraged:
//  var p = new OptionSet () {
//    { "h|?|help", v => ShowHelp () },
//  };
//
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//
//  var p = new OptionSet () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
//
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new OptionSet () {
//        { "a", s => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
//

// ReSharper disable All

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

namespace Mono.Options
{
    /// <summary>
    /// Class OptionValueCollection.
    /// Implements the <see cref="System.Collections.IList" />
    /// Implements the <see cref="System.Collections.Generic.IList{System.String}" />
    /// </summary>
    /// <seealso cref="System.Collections.IList" />
    /// <seealso cref="System.Collections.Generic.IList{System.String}" />
    public class OptionValueCollection : IList, IList<string>
    {

        /// <summary>
        /// The values
        /// </summary>
        List<string> values = new List<string>();
        /// <summary>
        /// The c
        /// </summary>
        OptionContext c;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionValueCollection"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
        internal OptionValueCollection(OptionContext c)
        {
            this.c = c;
        }

        #region ICollection
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index) { (values as ICollection).CopyTo(array, index); }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        bool ICollection.IsSynchronized { get { return (values as ICollection).IsSynchronized; } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value>The synchronize root.</value>
        object ICollection.SyncRoot { get { return (values as ICollection).SyncRoot; } }
        #endregion

        #region ICollection<T>
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(string item) { values.Add(item); }
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList" />.
        /// </summary>
        public void Clear() { values.Clear(); }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />.</returns>
        public bool Contains(string item) { return values.Contains(item); }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(string[] array, int arrayIndex) { values.CopyTo(array, arrayIndex); }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns><see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(string item) { return values.Remove(item); }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return values.Count; } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly { get { return false; } }
        #endregion

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return values.GetEnumerator(); }
        #endregion

        #region IEnumerable<T>
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator() { return values.GetEnumerator(); }
        #endregion

        #region IList
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.</returns>
        int IList.Add(object value) { return (values as IList).Add(value); }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns><see langword="true" /> if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, <see langword="false" />.</returns>
        bool IList.Contains(object value) { return (values as IList).Contains(value); }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
        int IList.IndexOf(object value) { return (values as IList).IndexOf(value); }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />.</param>
        void IList.Insert(int index, object value) { (values as IList).Insert(index, value); }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />.</param>
        void IList.Remove(object value) { (values as IList).Remove(value); }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void IList.RemoveAt(int index) { (values as IList).RemoveAt(index); }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        bool IList.IsFixedSize { get { return false; } }
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        object IList.this[int index] { get { return this[index]; } set { (values as IList)[index] = value; } }
        #endregion

        #region IList<T>
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(string item) { return values.IndexOf(item); }
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, string item) { values.Insert(index, item); }
        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index) { values.RemoveAt(index); }

        /// <summary>
        /// Asserts the valid.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="InvalidOperationException">OptionContext.Option is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">index</exception>
        /// <exception cref="Mono.Options.OptionException"></exception>
        private void AssertValid(int index)
        {
            if (c.Option == null)
                throw new InvalidOperationException("OptionContext.Option is null.");
            if (index >= c.Option.MaxValueCount)
                throw new ArgumentOutOfRangeException("index");
            if (c.Option.OptionValueType == OptionValueType.Required &&
                    index >= values.Count)
                throw new OptionException(string.Format(
                            c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName),
                        c.OptionName);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public string this[int index]
        {
            get
            {
                AssertValid(index);
                return index >= values.Count ? null : values[index];
            }
            set
            {
                values[index] = value;
            }
        }
        #endregion

        /// <summary>
        /// Converts to list.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> ToList()
        {
            return new List<string>(values);
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <returns>System.String[].</returns>
        public string[] ToArray()
        {
            return values.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Join(", ", values.ToArray());
        }
    }

    /// <summary>
    /// Class OptionContext.
    /// </summary>
    public class OptionContext
    {
        /// <summary>
        /// The option
        /// </summary>
        private Option option;
        /// <summary>
        /// The name
        /// </summary>
        private string name;
        /// <summary>
        /// The index
        /// </summary>
        private int index;
        /// <summary>
        /// The set
        /// </summary>
        private OptionSet set;
        /// <summary>
        /// The c
        /// </summary>
        private OptionValueCollection c;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionContext"/> class.
        /// </summary>
        /// <param name="set">The set.</param>
        public OptionContext(OptionSet set)
        {
            this.set = set;
            this.c = new OptionValueCollection(this);
        }

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        /// <value>The option.</value>
        public Option Option
        {
            get { return option; }
            set { option = value; }
        }

        /// <summary>
        /// Gets or sets the name of the option.
        /// </summary>
        /// <value>The name of the option.</value>
        public string OptionName
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the index of the option.
        /// </summary>
        /// <value>The index of the option.</value>
        public int OptionIndex
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets the option set.
        /// </summary>
        /// <value>The option set.</value>
        public OptionSet OptionSet
        {
            get { return set; }
        }

        /// <summary>
        /// Gets the option values.
        /// </summary>
        /// <value>The option values.</value>
        public OptionValueCollection OptionValues
        {
            get { return c; }
        }
    }

    /// <summary>
    /// Enum OptionValueType
    /// </summary>
    public enum OptionValueType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The optional
        /// </summary>
        Optional,
        /// <summary>
        /// The required
        /// </summary>
        Required,
    }

    /// <summary>
    /// Class Option.
    /// </summary>
    public abstract class Option
    {
        /// <summary>
        /// The prototype
        /// </summary>
        string prototype, description;
        /// <summary>
        /// The names
        /// </summary>
        string[] names;
        /// <summary>
        /// The type
        /// </summary>
        OptionValueType type;
        /// <summary>
        /// The count
        /// </summary>
        int count;
        /// <summary>
        /// The separators
        /// </summary>
        string[] separators;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        protected Option(string prototype, string description)
            : this(prototype, description, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        /// <param name="maxValueCount">The maximum value count.</param>
        /// <exception cref="ArgumentNullException">prototype</exception>
        /// <exception cref="ArgumentException">Cannot be the empty string. - prototype</exception>
        /// <exception cref="ArgumentException">Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
        ///                             "OptionValueType.Optional. - maxValueCount</exception>
        /// <exception cref="ArgumentException">maxValueCount</exception>
        /// <exception cref="ArgumentException">The default option handler '<>' cannot require values. - prototype</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxValueCount</exception>
        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
                throw new ArgumentNullException("prototype");
            if (prototype.Length == 0)
                throw new ArgumentException("Cannot be the empty string.", "prototype");
            if (maxValueCount < 0)
                throw new ArgumentOutOfRangeException("maxValueCount");

            this.prototype = prototype;
            this.names = prototype.Split('|');
            this.description = description;
            this.count = maxValueCount;
            this.type = ParsePrototype();

            if (this.count == 0 && type != OptionValueType.None)
                throw new ArgumentException(
                        "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
                            "OptionValueType.Optional.",
                        "maxValueCount");
            if (this.type == OptionValueType.None && maxValueCount > 1)
                throw new ArgumentException(
                        string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
                        "maxValueCount");
            if (Array.IndexOf(names, "<>") >= 0 &&
                    ((names.Length == 1 && this.type != OptionValueType.None) ||
                    (names.Length > 1 && this.MaxValueCount > 1)))
                throw new ArgumentException(
                        "The default option handler '<>' cannot require values.",
                        "prototype");
        }

        /// <summary>
        /// Gets the prototype.
        /// </summary>
        /// <value>The prototype.</value>
        public string Prototype { get { return prototype; } }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get { return description; } }
        /// <summary>
        /// Gets the type of the option value.
        /// </summary>
        /// <value>The type of the option value.</value>
        public OptionValueType OptionValueType { get { return type; } }
        /// <summary>
        /// Gets the maximum value count.
        /// </summary>
        /// <value>The maximum value count.</value>
        public int MaxValueCount { get { return count; } }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns>System.String[].</returns>
        public string[] GetNames()
        {
            return (string[])names.Clone();
        }

        /// <summary>
        /// Gets the value separators.
        /// </summary>
        /// <returns>System.String[].</returns>
        public string[] GetValueSeparators()
        {
            if (separators == null)
                return new string[0];
            return (string[])separators.Clone();
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="c">The c.</param>
        /// <returns>T.</returns>
        /// <exception cref="Mono.Options.OptionException"></exception>
        protected static T Parse<T>(string value, OptionContext c)
        {
            Type tt = typeof(T);
            bool nullable = tt.IsValueType && tt.IsGenericType &&
                !tt.IsGenericTypeDefinition &&
                tt.GetGenericTypeDefinition() == typeof(Nullable<>);
            Type targetType = nullable ? tt.GetGenericArguments()[0] : typeof(T);
            TypeConverter conv = TypeDescriptor.GetConverter(targetType);
            T t = default(T);
            try
            {
                if (value != null)
                    t = (T)conv.ConvertFromString(value);
            }
            catch (Exception e)
            {
                throw new OptionException(
                        string.Format(
                            c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."),
                            value, targetType.Name, c.OptionName),
                        c.OptionName, e);
            }
            return t;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>The names.</value>
        internal string[] Names { get { return names; } }
        /// <summary>
        /// Gets the value separators.
        /// </summary>
        /// <value>The value separators.</value>
        internal string[] ValueSeparators { get { return separators; } }

        /// <summary>
        /// The name terminator
        /// </summary>
        static readonly char[] NameTerminator = new char[] { '=', ':' };

        /// <summary>
        /// Parses the prototype.
        /// </summary>
        /// <returns>OptionValueType.</returns>
        /// <exception cref="ArgumentException">Empty option names are not supported. - prototype</exception>
        /// <exception cref="ArgumentException">prototype</exception>
        /// <exception cref="ArgumentException">prototype</exception>
        private OptionValueType ParsePrototype()
        {
            char type = '\0';
            List<string> seps = new List<string>();
            for (int i = 0; i < names.Length; ++i)
            {
                string name = names[i];
                if (name.Length == 0)
                    throw new ArgumentException("Empty option names are not supported.", "prototype");

                int end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                    continue;
                names[i] = name.Substring(0, end);
                if (type == '\0' || type == name[end])
                    type = name[end];
                else
                    throw new ArgumentException(
                            string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]),
                            "prototype");
                AddSeparators(name, end, seps);
            }

            if (type == '\0')
                return OptionValueType.None;

            if (count <= 1 && seps.Count != 0)
                throw new ArgumentException(
                        string.Format("Cannot provide key/value separators for Options taking {0} value(s).", count),
                        "prototype");
            if (count > 1)
            {
                if (seps.Count == 0)
                    this.separators = new string[] { ":", "=" };
                else if (seps.Count == 1 && seps[0].Length == 0)
                    this.separators = null;
                else
                    this.separators = seps.ToArray();
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }

        /// <summary>
        /// Adds the separators.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="end">The end.</param>
        /// <param name="seps">The seps.</param>
        /// <exception cref="ArgumentException">prototype</exception>
        /// <exception cref="ArgumentException">prototype</exception>
        /// <exception cref="ArgumentException">prototype</exception>
        private static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            int start = -1;
            for (int i = end + 1; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                            throw new ArgumentException(
                                    string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                    "prototype");
                        start = i + 1;
                        break;
                    case '}':
                        if (start == -1)
                            throw new ArgumentException(
                                    string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                    "prototype");
                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                            seps.Add(name[i].ToString());
                        break;
                }
            }
            if (start != -1)
                throw new ArgumentException(
                        string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                        "prototype");
        }

        /// <summary>
        /// Invokes the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        public void Invoke(OptionContext c)
        {
            OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        /// <summary>
        /// Called when [parse complete].
        /// </summary>
        /// <param name="c">The c.</param>
        protected abstract void OnParseComplete(OptionContext c);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Prototype;
        }
    }

    /// <summary>
    /// Class OptionException.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class OptionException : Exception
    {
        /// <summary>
        /// The option
        /// </summary>
        private string option;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="optionName">Name of the option.</param>
        public OptionException(string message, string optionName)
            : base(message)
        {
            this.option = optionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="innerException">The inner exception.</param>
        public OptionException(string message, string optionName, Exception innerException)
            : base(message, innerException)
        {
            this.option = optionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.option = info.GetString("OptionName");
        }

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        /// <value>The name of the option.</value>
        public string OptionName
        {
            get { return this.option; }
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OptionName", option);
        }
    }

    /// <summary>
    /// Delegate OptionAction
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public delegate void OptionAction<TKey, TValue>(TKey key, TValue value);

    /// <summary>
    /// Class OptionSet.
    /// Implements the <see cref="System.Collections.ObjectModel.KeyedCollection{System.String, Mono.Options.Option}" />
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.KeyedCollection{System.String, Mono.Options.Option}" />
    public class OptionSet : KeyedCollection<string, Option>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSet"/> class.
        /// </summary>
        public OptionSet()
            : this(delegate(string f) { return f; })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSet"/> class.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        public OptionSet(Converter<string, string> localizer)
        {
            this.localizer = localizer;
        }

        /// <summary>
        /// The localizer
        /// </summary>
        Converter<string, string> localizer;

        /// <summary>
        /// Gets the message localizer.
        /// </summary>
        /// <value>The message localizer.</value>
        public Converter<string, string> MessageLocalizer
        {
            get { return localizer; }
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        /// <exception cref="ArgumentNullException">option</exception>
        /// <exception cref="InvalidOperationException">Option has no names!</exception>
        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
                throw new ArgumentNullException("option");
            if (item.Names != null && item.Names.Length > 0)
                return item.Names[0];
            // This should never happen, as it's invalid for Option to be
            // constructed w/o any names.
            throw new InvalidOperationException("Option has no names!");
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, Option item)
        {
            base.InsertItem(index, item);
            AddImpl(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Option p = Items[index];
            // KeyedCollection.RemoveItem() handles the 0th item
            for (int i = 1; i < p.Names.Length; ++i)
            {
                Dictionary.Remove(p.Names[i]);
            }
        }

        /// <summary>
        /// Replaces the item at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be replaced.</param>
        /// <param name="item">The new item.</param>
        protected override void SetItem(int index, Option item)
        {
            base.SetItem(index, item);
            RemoveItem(index);
            AddImpl(item);
        }

        /// <summary>
        /// Adds the implementation.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <exception cref="ArgumentNullException">option</exception>
        private void AddImpl(Option option)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            List<string> added = new List<string>(option.Names.Length);
            try
            {
                // KeyedCollection.InsertItem/SetItem handle the 0th name.
                for (int i = 1; i < option.Names.Length; ++i)
                {
                    Dictionary.Add(option.Names[i], option);
                    added.Add(option.Names[i]);
                }
            }
            catch (Exception)
            {
                foreach (string name in added)
                    Dictionary.Remove(name);
                throw;
            }
        }

        /// <summary>
        /// Adds the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns>OptionSet.</returns>
        public new OptionSet Add(Option option)
        {
            base.Add(option);
            return this;
        }

        /// <summary>
        /// Class ActionOption. This class cannot be inherited.
        /// Implements the <see cref="Mono.Options.Option" />
        /// </summary>
        /// <seealso cref="Mono.Options.Option" />
        sealed class ActionOption : Option
        {
            /// <summary>
            /// The action
            /// </summary>
            Action<OptionValueCollection> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption"/> class.
            /// </summary>
            /// <param name="prototype">The prototype.</param>
            /// <param name="description">The description.</param>
            /// <param name="count">The count.</param>
            /// <param name="action">The action.</param>
            /// <exception cref="ArgumentNullException">action</exception>
            public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
                : base(prototype, description, count)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            /// <summary>
            /// Called when [parse complete].
            /// </summary>
            /// <param name="c">The c.</param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(c.OptionValues);
            }
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add(string prototype, Action<string> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            Option p = new ActionOption(prototype, description, 1,
                    delegate(OptionValueCollection v) { action(v[0]); });
            base.Add(p);
            return this;
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add(string prototype, OptionAction<string, string> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        /// <exception cref="ArgumentNullException">action</exception>
        public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            Option p = new ActionOption(prototype, description, 2,
                    delegate(OptionValueCollection v) { action(v[0], v[1]); });
            base.Add(p);
            return this;
        }

        /// <summary>
        /// Class ActionOption. This class cannot be inherited.
        /// Implements the <see cref="Mono.Options.Option" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <seealso cref="Mono.Options.Option" />
        sealed class ActionOption<T> : Option
        {
            /// <summary>
            /// The action
            /// </summary>
            Action<T> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption{T}"/> class.
            /// </summary>
            /// <param name="prototype">The prototype.</param>
            /// <param name="description">The description.</param>
            /// <param name="action">The action.</param>
            /// <exception cref="ArgumentNullException">action</exception>
            public ActionOption(string prototype, string description, Action<T> action)
                : base(prototype, description, 1)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            /// <summary>
            /// Called when [parse complete].
            /// </summary>
            /// <param name="c">The c.</param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(Parse<T>(c.OptionValues[0], c));
            }
        }

        /// <summary>
        /// Class ActionOption. This class cannot be inherited.
        /// Implements the <see cref="Mono.Options.Option" />
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <seealso cref="Mono.Options.Option" />
        sealed class ActionOption<TKey, TValue> : Option
        {
            /// <summary>
            /// The action
            /// </summary>
            OptionAction<TKey, TValue> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="prototype">The prototype.</param>
            /// <param name="description">The description.</param>
            /// <param name="action">The action.</param>
            /// <exception cref="ArgumentNullException">action</exception>
            public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action)
                : base(prototype, description, 2)
            {
                if (action == null)
                    throw new ArgumentNullException("action");
                this.action = action;
            }

            /// <summary>
            /// Called when [parse complete].
            /// </summary>
            /// <param name="c">The c.</param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(
                        Parse<TKey>(c.OptionValues[0], c),
                        Parse<TValue>(c.OptionValues[1], c));
            }
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype">The prototype.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add<T>(string prototype, Action<T> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add<T>(string prototype, string description, Action<T> action)
        {
            return Add(new ActionOption<T>(prototype, description, action));
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="prototype">The prototype.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// Adds the specified prototype.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="prototype">The prototype.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        /// <returns>OptionSet.</returns>
        public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
        {
            return Add(new ActionOption<TKey, TValue>(prototype, description, action));
        }

        /// <summary>
        /// Creates the option context.
        /// </summary>
        /// <returns>OptionContext.</returns>
        protected virtual OptionContext CreateOptionContext()
        {
            return new OptionContext(this);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> Parse(IEnumerable<string> arguments)
        {
            OptionContext c = CreateOptionContext();
            c.OptionIndex = -1;
            bool process = true;
            List<string> unprocessed = new List<string>();
            Option def = Contains("<>") ? this["<>"] : null;
            foreach (string argument in arguments)
            {
                ++c.OptionIndex;
                if (argument == "--")
                {
                    process = false;
                    continue;
                }
                if (!process)
                {
                    Unprocessed(unprocessed, def, c, argument);
                    continue;
                }
                if (!Parse(argument, c))
                    Unprocessed(unprocessed, def, c, argument);
            }
            if (c.Option != null)
                c.Option.Invoke(c);
            return unprocessed;
        }

        /// <summary>
        /// Unprocesseds the specified extra.
        /// </summary>
        /// <param name="extra">The extra.</param>
        /// <param name="def">The definition.</param>
        /// <param name="c">The c.</param>
        /// <param name="argument">The argument.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
        {
            if (def == null)
            {
                extra.Add(argument);
                return false;
            }
            c.OptionValues.Add(argument);
            c.Option = def;
            c.Option.Invoke(c);
            return false;
        }

        /// <summary>
        /// The value option
        /// </summary>
        private readonly Regex ValueOption = new Regex(
            @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

        /// <summary>
        /// Gets the option parts.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="flag">The flag.</param>
        /// <param name="name">The name.</param>
        /// <param name="sep">The sep.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">argument</exception>
        protected bool GetOptionParts(string argument, out string flag, out string name, out string sep, out string value)
        {
            if (argument == null)
                throw new ArgumentNullException("argument");

            flag = name = sep = value = null;
            Match m = ValueOption.Match(argument);
            if (!m.Success)
            {
                return false;
            }
            flag = m.Groups["flag"].Value;
            name = m.Groups["name"].Value;
            if (m.Groups["sep"].Success && m.Groups["value"].Success)
            {
                sep = m.Groups["sep"].Value;
                value = m.Groups["value"].Value;
            }
            return true;
        }

        /// <summary>
        /// Parses the specified argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool Parse(string argument, OptionContext c)
        {
            if (c.Option != null)
            {
                ParseValue(argument, c);
                return true;
            }

            string f, n, s, v;
            if (!GetOptionParts(argument, out f, out n, out s, out v))
                return false;

            Option p;
            if (ContainsKey(n))
            {
                p = GetOptionForKey(n);
                c.OptionName = f + n;
                c.Option = p;
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        c.OptionValues.Add(n);
                        c.Option.Invoke(c);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        ParseValue(v, c);
                        break;
                }
                return true;
            }
            // no match; is it a bool option?
            if (ParseBool(argument, n, c))
                return true;
            // is it a bundled option?
            if (ParseBundledValue(f, string.Concat(n + s + v), c))
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified key contains key; otherwise, <c>false</c>.</returns>
        private bool ContainsKey(string key)
        {
            return this.SelectMany(op => op.Names.Select(n => n.ToLower())).Contains(key.ToLower());
        }

        /// <summary>
        /// Gets the option for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Option.</returns>
        private Option GetOptionForKey(string key)
        {
            return this.SingleOrDefault(op => op.Names.Select(n => n.ToLower()).Contains(key.ToLower()));
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="c">The c.</param>
        /// <exception cref="Mono.Options.OptionException"></exception>
        private void ParseValue(string option, OptionContext c)
        {
            if (option != null)
                foreach (string o in c.Option.ValueSeparators != null
                        ? option.Split(c.Option.ValueSeparators, StringSplitOptions.None)
                        : new string[] { option })
                {
                    c.OptionValues.Add(o);
                }
            if (c.OptionValues.Count == c.Option.MaxValueCount ||
                    c.Option.OptionValueType == OptionValueType.Optional)
                c.Option.Invoke(c);
            else if (c.OptionValues.Count > c.Option.MaxValueCount)
            {
                throw new OptionException(localizer(string.Format(
                                "Error: Found {0} option values when expecting {1}.",
                                c.OptionValues.Count, c.Option.MaxValueCount)),
                        c.OptionName);
            }
        }

        /// <summary>
        /// Parses the bool.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="n">The n.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ParseBool(string option, string n, OptionContext c)
        {
            Option p;
            string rn;
            if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
                    Contains((rn = n.Substring(0, n.Length - 1))))
            {
                p = this[rn];
                string v = n[n.Length - 1] == '+' ? option : null;
                c.OptionName = option;
                c.Option = p;
                c.OptionValues.Add(v);
                p.Invoke(c);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the bundled value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="n">The n.</param>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="Mono.Options.OptionException"></exception>
        /// <exception cref="InvalidOperationException">Unknown OptionValueType: " + p.OptionValueType</exception>
        private bool ParseBundledValue(string f, string n, OptionContext c)
        {
            if (f != "-")
                return false;
            for (int i = 0; i < n.Length; ++i)
            {
                Option p;
                string opt = f + n[i].ToString();
                string rn = n[i].ToString();
                if (!Contains(rn))
                {
                    if (i == 0)
                        return false;
                    throw new OptionException(string.Format(localizer(
                                    "Cannot bundle unregistered option '{0}'."), opt), opt);
                }
                p = this[rn];
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        Invoke(c, opt, n, p);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        {
                            string v = n.Substring(i + 1);
                            c.Option = p;
                            c.OptionName = opt;
                            ParseValue(v.Length != 0 ? v : null, c);
                            return true;
                        }
                    default:
                        throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
                }
            }
            return true;
        }

        /// <summary>
        /// Invokes the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="option">The option.</param>
        private static void Invoke(OptionContext c, string name, string value, Option option)
        {
            c.OptionName = name;
            c.Option = option;
            c.OptionValues.Add(value);
            option.Invoke(c);
        }

        /// <summary>
        /// The option width
        /// </summary>
        private const int OptionWidth = 29;

        /// <summary>
        /// Writes the option descriptions.
        /// </summary>
        /// <param name="o">The o.</param>
        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (Option p in this)
            {
                int written = 0;
                if (!WriteOptionPrototype(o, p, ref written))
                    continue;

                if (written < OptionWidth)
                    o.Write(new string(' ', OptionWidth - written));
                else
                {
                    o.WriteLine();
                    o.Write(new string(' ', OptionWidth));
                }

                bool indent = false;
                string prefix = new string(' ', OptionWidth + 2);
                foreach (string line in GetLines(localizer(GetDescription(p.Description))))
                {
                    if (indent)
                        o.Write(prefix);
                    o.WriteLine(line);
                    indent = true;
                }
            }
        }

        /// <summary>
        /// Writes the option prototype.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="p">The p.</param>
        /// <param name="written">The written.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
        {
            string[] names = p.Names;

            int i = GetNextOptionIndex(names, 0);
            if (i == names.Length)
                return false;

            if (names[i].Length == 1)
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }
            else
            {
                Write(o, ref written, "      --");
                Write(o, ref written, names[0]);
            }

            for (i = GetNextOptionIndex(names, i + 1);
                    i < names.Length; i = GetNextOptionIndex(names, i + 1))
            {
                Write(o, ref written, ", ");
                Write(o, ref written, names[i].Length == 1 ? "-" : "--");
                Write(o, ref written, names[i]);
            }

            if (p.OptionValueType == OptionValueType.Optional ||
                    p.OptionValueType == OptionValueType.Required)
            {
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("["));
                }
                Write(o, ref written, localizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
                string sep = p.ValueSeparators != null && p.ValueSeparators.Length > 0
                    ? p.ValueSeparators[0]
                    : " ";
                for (int c = 1; c < p.MaxValueCount; ++c)
                {
                    Write(o, ref written, localizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
                }
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("]"));
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the index of the next option.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        static int GetNextOptionIndex(string[] names, int i)
        {
            while (i < names.Length && names[i] == "<>")
            {
                ++i;
            }
            return i;
        }

        /// <summary>
        /// Writes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="n">The n.</param>
        /// <param name="s">The s.</param>
        static void Write(TextWriter o, ref int n, string s)
        {
            n += s.Length;
            o.Write(s);
        }

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="maxIndex">The maximum index.</param>
        /// <param name="description">The description.</param>
        /// <returns>System.String.</returns>
        private static string GetArgumentName(int index, int maxIndex, string description)
        {
            if (description == null)
                return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
            string[] nameStart;
            if (maxIndex == 1)
                nameStart = new string[] { "{0:", "{" };
            else
                nameStart = new string[] { "{" + index + ":" };
            for (int i = 0; i < nameStart.Length; ++i)
            {
                int start, j = 0;
                do
                {
                    start = description.IndexOf(nameStart[i], j);
                } while (start >= 0 && j != 0 ? description[j++ - 1] == '{' : false);
                if (start == -1)
                    continue;
                int end = description.IndexOf("}", start);
                if (end == -1)
                    continue;
                return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
            }
            return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="InvalidOperationException">Invalid option description: " + description</exception>
        private static string GetDescription(string description)
        {
            if (description == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder(description.Length);
            int start = -1;
            for (int i = 0; i < description.Length; ++i)
            {
                switch (description[i])
                {
                    case '{':
                        if (i == start)
                        {
                            sb.Append('{');
                            start = -1;
                        }
                        else if (start < 0)
                            start = i + 1;
                        break;
                    case '}':
                        if (start < 0)
                        {
                            if ((i + 1) == description.Length || description[i + 1] != '}')
                                throw new InvalidOperationException("Invalid option description: " + description);
                            ++i;
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append(description.Substring(start, i - start));
                            start = -1;
                        }
                        break;
                    case ':':
                        if (start < 0)
                            goto default;
                        start = i + 1;
                        break;
                    default:
                        if (start < 0)
                            sb.Append(description[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private static IEnumerable<string> GetLines(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                yield return string.Empty;
                yield break;
            }
            int length = 80 - OptionWidth - 1;
            int start = 0, end;
            do
            {
                end = GetLineEnd(start, length, description);
                char c = description[end - 1];
                if (char.IsWhiteSpace(c) && end != description.Length)
                    --end;
                bool writeContinuation = end != description.Length && !IsEolChar(c);
                string line = description.Substring(start, end - start) +
                        (writeContinuation ? "-" : "");
                yield return line;
                start = end;
                if (char.IsWhiteSpace(c))
                    ++start;
                length = 80 - OptionWidth - 2 - 1;
            } while (end < description.Length);
        }

        /// <summary>
        /// Determines whether [is eol character] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is eol character] [the specified c]; otherwise, <c>false</c>.</returns>
        private static bool IsEolChar(char c)
        {
            return !char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Gets the line end.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="description">The description.</param>
        /// <returns>System.Int32.</returns>
        private static int GetLineEnd(int start, int length, string description)
        {
            int end = System.Math.Min(start + length, description.Length);
            int sep = -1;
            for (int i = start + 1; i < end; ++i)
            {
                if (description[i] == '\n')
                    return i + 1;
                if (IsEolChar(description[i]))
                    sep = i + 1;
            }
            if (sep == -1 || end == description.Length)
                return end;
            return sep;
        }
    }
}

