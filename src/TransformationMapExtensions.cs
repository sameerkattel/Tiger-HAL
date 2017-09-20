﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using static Tiger.Hal.Properties.Resources;

namespace Tiger.Hal
{
    /// <summary>Extends the functionality of the <see cref="ITransformationMap{T}"/> interface.</summary>
    [PublicAPI]
    public static class TransformationMapExtensions
    {
        /// <summary>Creates a link for the given type.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the link.
        /// </param>
        /// <param name="relation">The name of the link relation to establish.</param>
        /// <param name="linkSelector">
        /// A function that creates a <see cref="LinkBuilder"/>
        /// from a value of type <typeparamref name="T"/>.
        /// </param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Link<T>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Func<T, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

            return transformationMap.Link(relation.AbsoluteUri, linkSelector);
        }

        /// <summary>Creates a collection of links for the given type.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the link.
        /// </param>
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
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collectionSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Link<T, TMember>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Func<T, IEnumerable<TMember>> collectionSelector,
            [NotNull] Func<TMember, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (collectionSelector == null) { throw new ArgumentNullException(nameof(collectionSelector)); }
            if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

            return transformationMap.Link(relation.AbsoluteUri, collectionSelector, linkSelector);
        }

        /// <summary>Creates a collection of links for the given type.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the link.
        /// </param>
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
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collectionSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Link<T, TMember>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Func<T, IEnumerable<TMember>> collectionSelector,
            [NotNull] Func<T, TMember, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (collectionSelector == null) { throw new ArgumentNullException(nameof(collectionSelector)); }

            return transformationMap.Link(relation.AbsoluteUri, collectionSelector, linkSelector);
        }

        /// <summary>Creates a collection of links for the given type.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the link.
        /// </param>
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
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dictionarySelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Link<T, TKey, TValue>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Func<T, IDictionary<TKey, TValue>> dictionarySelector,
            [NotNull] Func<TKey, TValue, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (dictionarySelector == null) { throw new ArgumentNullException(nameof(dictionarySelector)); }

            return transformationMap.Link(relation.AbsoluteUri, dictionarySelector, linkSelector);
        }

        /// <summary>Creates a collection of links for the given type.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the link.
        /// </param>
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
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dictionarySelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Link<T, TKey, TValue>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Func<T, IDictionary<TKey, TValue>> dictionarySelector,
            [NotNull] Func<T, TKey, TValue, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (dictionarySelector == null) { throw new ArgumentNullException(nameof(dictionarySelector)); }

            return transformationMap.Link(relation.AbsoluteUri, dictionarySelector, linkSelector);
        }

        /// <summary>Creates an embed for the given type, using only the main object.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the embed.
        /// </param>
        /// <typeparam name="TMember">The type of the selected value.</typeparam>
        /// <param name="relation">The name of the link relation to establish.</param>
        /// <param name="memberSelector">A function selecting a top-level member to embed.</param>
        /// <param name="linkSelector">
        /// A function that creates a <see cref="LinkBuilder"/>
        /// from a value of type <typeparamref name="T"/>.
        /// </param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="relation"/> is not an absolute <see cref="Uri"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="memberSelector"/> is malformed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="linkSelector"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static ITransformationMap<T> Embed<T, TMember>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Expression<Func<T, TMember>> memberSelector,
            [NotNull] Func<T, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }
            if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

            return transformationMap.Embed(relation.AbsoluteUri, memberSelector, linkSelector);
        }

        /// <summary>Creates an embed for the given type, using the main object and the selected object.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the embed.
        /// </param>
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
        public static ITransformationMap<T> Embed<T, TMember>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Uri relation,
            [NotNull] Expression<Func<T, TMember>> memberSelector,
            [NotNull] Func<T, TMember, LinkBuilder> linkSelector)
        {
            if (relation == null) { throw new ArgumentNullException(nameof(relation)); }
            if (!relation.IsAbsoluteUri) { throw new ArgumentException(RelativeRelationUri, nameof(relation)); }
            if (memberSelector == null) { throw new ArgumentNullException(nameof(memberSelector)); }
            if (linkSelector == null) { throw new ArgumentNullException(nameof(linkSelector)); }

            return transformationMap.Embed(relation.AbsoluteUri, memberSelector, linkSelector);
        }

        /// <summary>Causes a member not to be represented in the HAL+JSON serialization of a value.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the ignore.
        /// </param>
        /// <typeparam name="T1">The type of the member selected by <paramref name="memberSelector1"/>.</typeparam>
        /// <param name="memberSelector1">
        /// A function selecting a top-level member of type <typeparamref name="T1"/> to ignore.
        /// </param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector1"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector1"/> is malformed.</exception>
        [NotNull]
        public static ITransformationMap<T> Ignore<T, T1>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Expression<Func<T, T1>> memberSelector1)
        {
            if (memberSelector1 == null) { throw new ArgumentNullException(nameof(memberSelector1)); }

            switch (memberSelector1.Body)
            {
                case MemberExpression me:
                    return transformationMap.Ignore(me.Member.Name);
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector1));
            }
        }

        /// <summary>Causes members not to be represented in the HAL+JSON serialization of a value.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the ignore.
        /// </param>
        /// <typeparam name="T1">The type of the member selected by <paramref name="memberSelector1"/>.</typeparam>
        /// <typeparam name="T2">The type of the member selected by <paramref name="memberSelector2"/>.</typeparam>
        /// <param name="memberSelector1">
        /// A function selecting a top-level member of type <typeparamref name="T1"/> to ignore.
        /// </param>
        /// <param name="memberSelector2">
        /// A function selecting a top-level member of type <typeparamref name="T2"/> to ignore.
        /// </param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector1"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector1"/> is malformed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector2"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector2"/> is malformed.</exception>
        [NotNull]
        public static ITransformationMap<T> Ignore<T, T1, T2>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Expression<Func<T, T1>> memberSelector1,
            [NotNull] Expression<Func<T, T2>> memberSelector2)
        {
            if (memberSelector1 == null) { throw new ArgumentNullException(nameof(memberSelector1)); }
            if (memberSelector2 == null) { throw new ArgumentNullException(nameof(memberSelector2)); }

            switch (memberSelector1.Body)
            {
                case MemberExpression me:
                    transformationMap.Ignore(me.Member.Name);
                    break;
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector1));
            }

            switch (memberSelector2.Body)
            { // todo(cosborn) Allow indexing, in the case of collections and dictionaries?
                case MemberExpression me:
                    transformationMap.Ignore(me.Member.Name);
                    break;
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector2));
            }

            return transformationMap;
        }

        /// <summary>Causes members not to be represented in the HAL+JSON serialization of a value.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the ignore.
        /// </param>
        /// <typeparam name="T1">The type of the member selected by <paramref name="memberSelector1"/>.</typeparam>
        /// <typeparam name="T2">The type of the member selected by <paramref name="memberSelector2"/>.</typeparam>
        /// <typeparam name="T3">The type of the member selected by <paramref name="memberSelector3"/>.</typeparam>
        /// <param name="memberSelector1">
        /// A function selecting a top-level member of type <typeparamref name="T1"/> to ignore.
        /// </param>
        /// <param name="memberSelector2">
        /// A function selecting a top-level member of type <typeparamref name="T2"/> to ignore.
        /// </param>
        /// <param name="memberSelector3">
        /// A function selecting a top-level member of type <typeparamref name="T3"/> to ignore.
        /// </param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector1"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector1"/> is malformed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector2"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector2"/> is malformed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelector3"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelector3"/> is malformed.</exception>
        [NotNull]
        public static ITransformationMap<T> Ignore<T, T1, T2, T3>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull] Expression<Func<T, T1>> memberSelector1,
            [NotNull] Expression<Func<T, T2>> memberSelector2,
            [NotNull] Expression<Func<T, T3>> memberSelector3)
        {
            if (memberSelector1 == null) { throw new ArgumentNullException(nameof(memberSelector1)); }
            if (memberSelector2 == null) { throw new ArgumentNullException(nameof(memberSelector2)); }
            if (memberSelector3 == null) { throw new ArgumentNullException(nameof(memberSelector3)); }

            switch (memberSelector1.Body)
            {
                case MemberExpression me:
                    transformationMap.Ignore(me.Member.Name);
                    break;
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector1));
            }

            switch (memberSelector2.Body)
            {
                case MemberExpression me:
                    transformationMap.Ignore(me.Member.Name);
                    break;
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector2));
            }

            switch (memberSelector3.Body)
            {
                case MemberExpression me:
                    transformationMap.Ignore(me.Member.Name);
                    break;
                default:
                    throw new ArgumentException(MalformedValueSelector, nameof(memberSelector3));
            }

            return transformationMap;
        }

        /// <summary>Causes members not to be represented in the HAL+JSON serialization of a value.</summary>
        /// <typeparam name="T">The type being transformed.</typeparam>
        /// <param name="transformationMap">
        /// The transformation map to which to add the ignore.
        /// </param>
        /// <param name="memberSelectors">A collection of functions, each selecting a top-level member to ignore.</param>
        /// <returns>The modified transformation map.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="memberSelectors"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">A member of <paramref name="memberSelectors"/> is malformed.</exception>
        [NotNull]
        public static ITransformationMap<T> Ignore<T>(
            [NotNull] this ITransformationMap<T> transformationMap,
            [NotNull, ItemNotNull] params Expression<Func<T, object>>[] memberSelectors)
        {
            if (memberSelectors == null) { throw new ArgumentNullException(nameof(memberSelectors)); }

            void IgnoreCore(Expression e)
            {
                switch (e)
                {
                    case UnaryExpression ue when ue.NodeType == ExpressionType.Convert:
                        /* note(cosborn)
                         * Because we have to fall back to Expression<Func<T, object>>,
                         * value types will be wrapped in a Convert call by the compiler.
                         */
                        IgnoreCore(ue.Operand);
                        break;
                    case MemberExpression me:
                        transformationMap.Ignore(me.Member.Name);
                        break;
                    default:
                        throw new ArgumentException(MalformedValueSelector, nameof(memberSelectors))
                        {
                            Data =
                            {
                                ["selector"] = e
                            }
                        };
                }
            }

            foreach (var memberSelector in memberSelectors)
            {
                IgnoreCore(memberSelector.Body);
            }

            return transformationMap;
        }
    }
}