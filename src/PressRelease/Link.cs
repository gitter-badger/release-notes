using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PressRelease
{
	public struct Link
	{
		public Link( string rel, string uri ) : this(rel, new Uri( uri ) )
		{
		}

		public Link( string rel, Uri uri ) : this()
		{
			Uri = uri;
			Rel = rel;
		}

		public Uri Uri { get; }
		public string Rel { get; }
	}
}
