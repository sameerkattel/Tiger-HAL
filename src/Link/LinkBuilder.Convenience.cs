﻿using System;
using JetBrains.Annotations;
using Tavis.UriTemplates;

namespace Tiger.Hal
{
    /// <content>Convenience methods for creating <see cref="Tiger.Hal.LinkBuilder"/> instances.</content>
    public partial class LinkBuilder
    {
        /// <summary>Creates a link from an ASP.NET MVC route.</summary>
        /// <param name="routeName">The name of the route for which to generate a link.</param>
        /// <param name="routeValues">The route values to use when generating a link.</param>
        /// <returns>A link builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="routeName"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static LinkBuilder Route([NotNull] string routeName, [CanBeNull] object routeValues = null) =>
            new Routed(routeName, routeValues);

        /// <summary>Creates a templated link from a URI template.</summary>
        /// <param name="template">The template that will become the value of <see cref="Link.Href"/>.</param>
        /// <returns>A link builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static LinkBuilder Template([NotNull] UriTemplate template) => new Templated(template);

        /// <summary>Creates a templated link from a URI template.</summary>
        /// <param name="template">The template that will become the value of <see cref="Link.Href"/>.</param>
        /// <returns>A link builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static LinkBuilder Template([NotNull] string template) => new Templated(template);

        /// <summary>Creates a link from a constant URI.</summary>
        /// <param name="href">The URI that will become the value of <see cref="Link.Href"/>.</param>
        /// <returns>A link builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="href"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static LinkBuilder Const([NotNull] Uri href) => new Constant(href);

        /// <summary>Creates a link from a constant URI.</summary>
        /// <param name="href">The URI that will become the value of <see cref="Link.Href"/>.</param>
        /// <returns>A link builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="href"/> is <see langword="null"/>.</exception>
        /// <exception cref="UriFormatException"><paramref name="href"/> does not represent a valid URI.</exception>
        [NotNull]
        public static LinkBuilder Const([NotNull] string href) => new Constant(href);
    }
}
