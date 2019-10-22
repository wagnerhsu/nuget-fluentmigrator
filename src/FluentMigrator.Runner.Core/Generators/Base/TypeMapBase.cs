// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TypeMapBase.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace FluentMigrator.Runner.Generators.Base
{
    /// <summary>
    /// Class TypeMapBase.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.ITypeMap" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.ITypeMap" />
    public abstract class TypeMapBase : ITypeMap
    {
        /// <summary>
        /// The templates
        /// </summary>
        private readonly Dictionary<DbType, SortedList<int, string>> _templates = new Dictionary<DbType, SortedList<int, string>>();
        /// <summary>
        /// The size placeholder
        /// </summary>
        private const string SizePlaceholder = "$size";
        /// <summary>
        /// The precision placeholder
        /// </summary>
        protected const string PrecisionPlaceholder = "$precision";

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapBase"/> class.
        /// </summary>
        protected TypeMapBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            SetupTypeMaps();
        }

        /// <summary>
        /// Setups the type maps.
        /// </summary>
        protected abstract void SetupTypeMaps();

        /// <summary>
        /// Sets the type map.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="template">The template.</param>
        protected void SetTypeMap(DbType type, string template)
        {
            EnsureHasList(type);
            _templates[type][-1] = template;
        }

        /// <summary>
        /// Sets the type map.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="template">The template.</param>
        /// <param name="maxSize">The maximum size.</param>
        protected void SetTypeMap(DbType type, string template, int maxSize)
        {
            EnsureHasList(type);
            _templates[type][maxSize] = template;
        }

        /// <inheritdoc />
        public virtual string GetTypeMap(DbType type, int? size, int? precision)
        {
            if (!_templates.ContainsKey(type))
                throw new NotSupportedException($"Unsupported DbType '{type}'");

            if (size == null)
                return ReplacePlaceholders(_templates[type][-1], size: 0, precision);

            var sizeValue = size.Value;
            foreach (var entry in _templates[type])
            {
                int capacity = entry.Key;
                string template = entry.Value;

                if (sizeValue <= capacity)
                    return ReplacePlaceholders(template, sizeValue, precision);
            }

            throw new NotSupportedException($"Unsupported DbType '{type}'");
        }

        /// <inheritdoc />
        [Obsolete]
        public virtual string GetTypeMap(DbType type, int size, int precision)
        {
            return GetTypeMap(type, (int?)size, precision);
        }

        /// <summary>
        /// Ensures the has list.
        /// </summary>
        /// <param name="type">The type.</param>
        private void EnsureHasList(DbType type)
        {
            if (!_templates.ContainsKey(type))
                _templates.Add(type, new SortedList<int, string>());
        }

        /// <summary>
        /// Replaces the placeholders.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="size">The size.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>System.String.</returns>
        private string ReplacePlaceholders(string value, int? size, int? precision)
        {
            if (size != null)
            {
                value = value.Replace(SizePlaceholder, size.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (precision != null)
            {
                value = value.Replace(PrecisionPlaceholder, precision.Value.ToString(CultureInfo.InvariantCulture));
            }

            return value;
        }

    }
}
