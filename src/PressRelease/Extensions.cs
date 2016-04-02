using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace PressRelease
{
	public static class Extensions
	{
		public static IReadOnlyCollection<Link> LinkHeaders( this HttpResponseHeaders headers )
		{
			IEnumerable<string> links;
			if ( !headers.TryGetValues( "Link", out links ) ) return Enumerable.Empty<Link>().ToList();

			var matches = links.SelectMany( l => ParseLinkHeaderExpression.Matches( l ).Cast<Match>() );
			var linkUris = from match in matches
						   from rel in match.Groups["rel"].Captures.Cast<Capture>()
						   from uri in match.Groups["uri"].Captures.Cast<Capture>()
						   select new Link( rel.Value, uri.Value );

			return linkUris.ToList();
		}

		private static readonly Regex ParseLinkHeaderExpression = new Regex( "<(?<uri>[a-zA-Z:/.?=%0-9&+_]*)>; rel=\"(?<rel>[a-z]*)\"", RegexOptions.Compiled );
	}
}
