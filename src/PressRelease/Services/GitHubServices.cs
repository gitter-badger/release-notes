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
			var nextPageUri = new Uri( "user/repos?&per_page=100", UriKind.Relative );

			var result = Enumerable.Empty<string>();
			do
			{
				var info = await _httpClient.GetAsync( nextPageUri );
				if ( !info.IsSuccessStatusCode )
				{
					throw new InvalidOperationException( "API call failed" );
				}

				nextPageUri = info.Headers.LinkHeaders()
					.Where( l => l.Rel == "next" )
					.Select( l => l.Uri )
					.SingleOrDefault();

				var asString = await info.Content.ReadAsStringAsync();
				var arr = JArray.Parse( asString );
				result = result.Concat( arr.Select( a => a.SelectToken( "name" ).ToString() ) );
			} while ( nextPageUri != null );
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
	}
}
