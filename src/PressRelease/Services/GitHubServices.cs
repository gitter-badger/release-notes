using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PressRelease.Services
{
	public class GitHubServices
	{
		public GitHubServices(string accessToken)
		{
			_accessToken = accessToken;
		}

		public async Task<string> GetUserAsync()
		{
			using ( var client = new HttpClient() )
			{
				client.BaseAddress = new Uri( "https://api.github.com/" );
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add( "user-agent", "Custom" );

				var info = await client.GetAsync( $"user?access_token={_accessToken}" );
				return await info.Content.ReadAsStringAsync();
			}
		}

		private readonly string _accessToken;
	}
}
