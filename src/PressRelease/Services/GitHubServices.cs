using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PressRelease.Services
{
	public interface IGitHubClient
	{
		Task<string> GetBranchesAsync( string accessToken );
		Task<IEnumerable<string>> GetRepositoriesAsync( string accessToken );
	}

	public class GitHubClient : IGitHubClient, IDisposable
	{
		public GitHubClient()
		{
			_httpClient = new HttpClient
			{
				BaseAddress = new Uri( "https://api.github.com/" ),
				DefaultRequestHeaders =
				{
					{ "user-agent", "Custom" }
				}
			};
		}

		public async Task<string> GetBranchesAsync( string accessToken )
		{
			var info = await _httpClient.GetAsync( $"user/repos?access_token={accessToken}" );
			return await info.Content.ReadAsStringAsync();
		}

		public async Task<IEnumerable<string>> GetRepositoriesAsync( string accessToken )
		{
			var info = await _httpClient.GetAsync( $"user/repos?type=all&access_token={accessToken}" );
			if ( !info.IsSuccessStatusCode )
			{
				throw new InvalidOperationException( "API call failed" );
			}
			var asString = await info.Content.ReadAsStringAsync();
			var arr = JArray.Parse( asString );
			return new string[0];
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
