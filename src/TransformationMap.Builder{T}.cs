﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using static Tiger.Hal.Properties.Resources;

namespace Tiger.Hal
{
    /// <content>The builder for further transformations beyond "self".</content>
    public sealed partial class TransformationMap
    {
        /// <summary>Configures a created transformation map.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        [PublicAPI]
        public class Builder<T>
            : ITransformationMap
        {
            readonly Dictionary<string, ILinkInstruction> _links = new Dictionary<string, ILinkInstruction>();
            readonly List<IEmbedInstruction> _embeds = new List<IEmbedInstruction>();
            readonly List<IHoistInstruction> _hoists = new List<IHoistInstruction>();
            readonly List<string> _ignores = new List<string>();

            /// <summary>Initializes a new instance of the <see cref="Builder{T}"/> class.</summary>
            /// <param name="selfSelector">
            /// A function that creates a <see cref="LinkBuilder"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            internal Builder([NotNull] Func<T, LinkBuilder> selfSelector)
            {
                _links["self"] = new LinkInstruction<T>(selfSelector);
            }

            /// <inheritdoc/>
            IReadOnlyDictionary<string, ILinkInstruction> ITransformationMap.LinkInstructions => _links;

            /// <inheritdoc/>
            IReadOnlyCollection<IEmbedInstruction> ITransformationMap.EmbedInstructions => _embeds;

            /// <inheritdoc/>
            IReadOnlyCollection<IHoistInstruction> ITransformationMap.HoistInstructions => _hoists;

            /// <inheritdoc/>
            IReadOnlyCollection<string> ITransformationMap.IgnoreInstructions => _ignores;

            #region Link

            /// <summary>Creates a link for the given type.</summary>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> Link(
                [NotNull] string relation,
                [NotNull] Func<T, LinkBuilder> linkSelector)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

                _links[relation] = new LinkInstruction<T>(linkSelector);
                return this;
            }

            /// <summary>
            /// Creates a link for the given type, optionally removing
            /// the specified member from the serialization.
            /// </summary>
            /// <typeparam name="TMember">The return type of <paramref name="memberSelector"/>.</typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="memberSelector">
            /// A function that selects a value of type <typeparamref name="TMember"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/>
            /// from a value of type <typeparamref name="TMember"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="memberSelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> Link<TMember>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, TMember>> memberSelector,
                [NotNull] Func<TMember, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }
                if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

                switch (memberSelector.Body)
                {
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = memberSelector.Compile();
                        _links[relation] = new LinkInstruction<T>(t => linkSelector(valueSelector(t)));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates a collection of links for the given type.</summary>
            /// <typeparam name="TMember">The return type of <paramref name="memberSelector"/>.</typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="memberSelector">
            /// A function that selects a value from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="memberSelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> Link<TMember>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, TMember>> memberSelector,
                [NotNull] Func<T, TMember, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

                switch (memberSelector.Body)
                {
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = memberSelector.Compile();
                        _links[relation] = new LinkInstruction<T>(t => linkSelector(t, valueSelector(t)));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates a link for the given type.</summary>
            /// <typeparam name="TMember">
            /// The member type of the return type of <paramref name="collectionSelector"/>.
            /// </typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="collectionSelector">
            /// A function that selects a collection from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/> from a value of type <typeparamref name="TMember"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="collectionSelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="collectionSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="collectionSelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> LinkMany<TMember>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, IEnumerable<TMember>>> collectionSelector,
                [NotNull] Func<TMember, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (collectionSelector == null) { throw new ArgumentNullException(nameof(collectionSelector)); }

                switch (collectionSelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = collectionSelector.Compile();
                        _links[relation] = new ManyLinkInstruction<T>(t => valueSelector(t).Select(linkSelector));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates a collection of links for the given type.</summary>
            /// <typeparam name="TMember">
            /// The member type of the return type of <paramref name="collectionSelector"/>.
            /// </typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="collectionSelector">
            /// A function that selects a collection of values of type <typeparamref name="TMember"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/> from a value of type <typeparamref name="TMember"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="collectionSelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="collectionSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="collectionSelector"/> is malformed</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> LinkMany<TMember>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, IEnumerable<TMember>>> collectionSelector,
                [NotNull] Func<T, TMember, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (collectionSelector == null) { throw new ArgumentNullException(nameof(collectionSelector)); }

                switch (collectionSelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = collectionSelector.Compile();
                        _links[relation] = new ManyLinkInstruction<T>(
                            t => valueSelector(t).Select(tm => linkSelector(t, tm)));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates a link for the given type.</summary>
            /// <typeparam name="TKey">
            /// The key type of the return type of <paramref name="dictionarySelector"/>.
            /// </typeparam>
            /// <typeparam name="TValue">
            /// The value type of the return type of <paramref name="dictionarySelector"/>.
            /// </typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="dictionarySelector">
            /// A function that selects a dictionary from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/> from a value of type
            /// <typeparamref name="TKey"/> and a value of type <typeparamref name="TValue"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="dictionarySelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="dictionarySelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="dictionarySelector"/> is malformed</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> LinkMany<TKey, TValue>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, IDictionary<TKey, TValue>>> dictionarySelector,
                [NotNull] Func<TKey, TValue, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (dictionarySelector == null) { throw new ArgumentNullException(nameof(dictionarySelector)); }

                switch (dictionarySelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = dictionarySelector.Compile();
                        _links[relation] = new ManyLinkInstruction<T>(
                            t => valueSelector(t).Select(kvp => linkSelector(kvp.Key, kvp.Value)));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates a collection of links for the given type.</summary>
            /// <typeparam name="TKey">
            /// The key type of the return type of <paramref name="dictionarySelector"/>.
            /// </typeparam>
            /// <typeparam name="TValue">
            /// The value type of the return type of <paramref name="dictionarySelector"/>.
            /// </typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="dictionarySelector">
            /// A function that selects a dictionary from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/> from a value of type
            /// <typeparamref name="T"/>, a value of type <typeparamref name="TKey"/>,
            /// and a value of type <typeparamref name="TValue"/>.
            /// </param>
            /// <param name="retain">
            /// A value indicating whether the value selected by <paramref name="dictionarySelector"/>
            /// should be included in the HAL+JSON serialization.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="dictionarySelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="dictionarySelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> LinkMany<TKey, TValue>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, IDictionary<TKey, TValue>>> dictionarySelector,
                [NotNull] Func<T, TKey, TValue, LinkBuilder> linkSelector,
                bool retain = false)
            {
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (dictionarySelector == null) { throw new ArgumentNullException(nameof(dictionarySelector)); }

                switch (dictionarySelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        if (!retain) { _ignores.Add(me.Member.Name); }
                        var valueSelector = dictionarySelector.Compile();
                        _links[relation] = new ManyLinkInstruction<T>(
                            t => valueSelector(t).Select(
                                kvp => linkSelector(t, kvp.Key, kvp.Value)));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            #endregion

            #region Embed

            /// <summary>Creates an embed for the given type, using only the main object.</summary>
            /// <typeparam name="TMember">The type of the selected value.</typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="memberSelector">A function selecting a top-level member to embed.</param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/>
            /// from a value of type <typeparamref name="T"/>.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> Embed<TMember>([NotNull] string relation, [NotNull] Expression<Func<T, TMember>> memberSelector, [NotNull] Func<T, LinkBuilder> linkSelector)
            {
                if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

                // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                switch (memberSelector.Body)
                {
                    case MemberExpression me:
                        var valueSelector = memberSelector.Compile();
                        _links[relation] = new LinkInstruction<T>(linkSelector);
                        _embeds.Add(new MemberEmbedInstruction<T, TMember>(relation, me.Member.Name, valueSelector));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Creates an embed for the given type, using the main object and the selected object.</summary>
            /// <typeparam name="TMember">The type of the selected property.</typeparam>
            /// <param name="relation">The name of the link relation to establish.</param>
            /// <param name="memberSelector">A function selecting a top-level member to embed.</param>
            /// <param name="linkSelector">
            /// A function that creates a <see cref="LinkBuilder"/> from a value of type
            /// <typeparamref name="T"/> and a value of type <typeparamref name="TMember"/>.
            /// </param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
            [NotNull]
            public Builder<T> Embed<TMember>(
                [NotNull] string relation,
                [NotNull] Expression<Func<T, TMember>> memberSelector,
                [NotNull] Func<T, TMember, LinkBuilder> linkSelector)
            {
                if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }
                if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
                if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

                switch (memberSelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        var valueSelector = memberSelector.Compile();
                        _links[relation] = new LinkInstruction<T>(t => linkSelector(t, valueSelector(t)));
                        _embeds.Add(new MemberEmbedInstruction<T, TMember>(relation, me.Member.Name, valueSelector));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            #endregion

            /// <summary>
            /// Hoists a property that would not be present in the HAL representation of an array value
            /// to the containing object value.
            /// </summary>
            /// <typeparam name="TMember">The type of the value to hoist.</typeparam>
            /// <param name="memberSelector">A function selecting a top-level member to hoist.</param>
            /// <returns>The modified transformation map.</returns>
            /// <remarks>
            /// This only has a meaningful effect on types which resolve to a <see cref="JsonArrayContract"/> --
            /// types which are not serialized as objects. Types which are serialized as objects already have
            /// their member keys at the root level.
            /// </remarks>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            [NotNull]
            public Builder<T> Hoist<TMember>([NotNull] Expression<Func<T, TMember>> memberSelector)
            {
                if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }

                switch (memberSelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        var valueSelector = memberSelector.Compile();
                        _hoists.Add(new MemberHoistInstruction<T, TMember>(me.Member.Name, valueSelector));
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }

            /// <summary>Causes a property not to be represented in the HAL+JSON serialization of a value.</summary>
            /// <typeparam name="TMember">The type of the value to ignore.</typeparam>
            /// <param name="memberSelector">A function selecting a top-level member to ignore.</param>
            /// <returns>The modified transformation map.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
            /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
            [NotNull]
            public Builder<T> Ignore<TMember>([NotNull] Expression<Func<T, TMember>> memberSelector)
            {
                if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }

                switch (memberSelector.Body)
                { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                    case MemberExpression me:
                        _ignores.Add(me.Member.Name);
                        return this;
                    default:
                        throw new ArgumentException(MalformedValueSelector);
                }
            }
        }
    }
}
