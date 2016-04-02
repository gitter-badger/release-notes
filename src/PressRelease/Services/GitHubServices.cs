using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Http;
using System.Text.RegularExpressions;

namespace PressRelease.Services
{
	public interface IGitHubClient
	{
		Task<string> GetBranchesAsync();
		Task<IReadOnlyCollection<string>> GetAllRepositoriesAsync();
	}

	public class GitHubClient : IGitHubClient, IDisposable
	{
		public GitHubClient( IHttpContextAccessor contextAccessor )
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri( "https://api.github.com/" ),
				DefaultRequestHeaders =
				{
					{ "user-agent", "PressRelease 1.0" },
					{ "Authorization", $"token {contextAccessor.HttpContext.User.FindFirstValue( "access_token" )}" }
				}
			};
		}

		public async Task<string> GetBranchesAsync()
		{
			var info = await _httpClient.GetAsync( $"user/repos" );
			return await info.Content.ReadAsStringAsync();
		}

		public async Task<IReadOnlyCollection<string>> GetAllRepositoriesAsync()
		{
			var nextPageUri = "user/repos?&per_page=100";

			var result = Enumerable.Empty<string>();
			do
			{
				var info = await _httpClient.GetAsync( nextPageUri );
				if ( !info.IsSuccessStatusCode )
				{
					throw new InvalidOperationException( "API call failed" );
				}

				IEnumerable<string> links;
				info.Headers.TryGetValues( "Link", out links );

				var matches = links.SelectMany( l => ParseLinkHeader.Matches( l ).Cast<Match>() );
				var linkUris = ( from match in matches
								 from rel in match.Groups["rel"].Captures.Cast<Capture>()
								 from uri in match.Groups["uri"].Captures.Cast<Capture>()
								 select new
								 {
									 rel = rel.Value,
									 uri = uri.Value
								 } ).ToDictionary( l => l.rel, l => l.uri );

				linkUris.TryGetValue( "next", out nextPageUri );

				var asString = await info.Content.ReadAsStringAsync();
				var arr = JArray.Parse( asString );
				result = result.Concat( arr.Select( a => a.SelectToken( "name" ).ToString() ) );
			} while ( !String.IsNullOrWhiteSpace( nextPageUri ) );
			return result.ToList();
		}

		public void Dispose()
		{
			if ( !_isDisposed )
			{
				_isDisposed = true;
				_httpClient.Dispose();
			}
		}

		private bool _isDisposed;
		private readonly HttpClient _httpClient;
		private static readonly Regex ParseLinkHeader = new Regex( "<(?<uri>[a-zA-Z:/.?=%0-9&+_]*)>; rel=\"(?<rel>[a-z]*)\"", RegexOptions.Compiled );
	}
}
