using Sandbox;
using System.Linq;
using System.Text;
using System.Collections.Generic;

partial class SandboxGame
{
	[ServerCmd( "rbx_kick" )]
	public static void Kick(string name)
	{
		if ( !ConsoleSystem.Caller.HasPermission( "kick" ) ) return;
		name = name.ToLower();

		var cls = Client.All.Where( c => c.Name.ToLower().Contains( name ) );
		var count = cls.Count();
		if (count == 1)
		{
			var cl = cls.First();
			cl.Kick();
			Log.Info( $"Kicked {cl.Name}({cl.SteamId})" );
		}
		else if(count == 0)
		{
			Log.Info("No users found");
		}
		else
		{
			Log.Info($"Found {count} users, no user kicked, use kick_id with the steamid");
			foreach ( var cl in cls )
				Log.Info( $"{cl.Name}({cl.SteamId})" );
		}
	}

	[ServerCmd( "rbx_kick_id")]
	public static void KickID(ulong id)
	{
		if ( !ConsoleSystem.Caller.HasPermission( "kick" ) ) return;

		var cl = Client.All.FirstOrDefault( c => c.SteamId == id );
		if ( cl != null )
		{
			cl.Kick();
			Log.Info( $"Kicked {cl.Name}({cl.SteamId})" );
		}
		else Log.Info( "User not found" );
	}

	[ServerCmd( "rbx_ban" )]
	public static void Ban( string name )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "ban" ) ) return;
		name = name.ToLower();

		var cls = Client.All.Where( c => c.Name.ToLower().Contains( name ) );
		var count = cls.Count();
		if ( count == 1 )
		{
			var cl = cls.First();
			cl.Ban();
			Log.Info( $"Banned {cl.Name}({cl.SteamId})" );
		}
		else if ( count == 0 )
		{
			Log.Info( "No users found" );
		}
		else
		{
			Log.Info( $"Found {count} users, no user banned, use ban_id with the steamid" );
			foreach ( var cl in cls )
				Log.Info( $"{cl.Name}({cl.SteamId})" );
		}
	}

	[ServerCmd( "rbx_ban_id" )]
	public static void BanID( ulong id )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "ban" ) ) return;

		var cl = Client.All.FirstOrDefault( c => c.SteamId == id );
		if ( cl != null )
		{
			cl.Ban();
			Log.Info( $"Banned {cl.Name}({cl.SteamId})" );
		}
		else
		{
			Log.Info( $"User not found, {id} added to banlist" );
			AddToBanList( id );
		}
	}

	const string BanlistPath = "banlist";

	public static bool IsBanned( ulong id )
		=> GetBanList().Any( b => b == id);

	public static IEnumerable<ulong> GetBanList()
	{
		if ( FileSystem.Data.FileExists( BanlistPath ) )
		{
			var lines = FileSystem.Data.ReadAllText( BanlistPath );
			foreach(var line in lines.Split("\n"))
			{
				ulong id;
				if(ulong.TryParse(line, out id))
				{
					yield return id;
				}
			}
		}
		else FileSystem.Data.WriteAllText( BanlistPath, "" );
	}
	public static void AddToBanList(ulong id)
	{
		if ( FileSystem.Data.FileExists( BanlistPath ) )
		{
			var bytes = FileSystem.Data.ReadAllBytes( BanlistPath );
			var stream = FileSystem.Data.OpenWrite( BanlistPath );
			stream.Write( bytes );
			stream.Write( Encoding.UTF8.GetBytes( id.ToString() ) );
			stream.Write( Encoding.UTF8.GetBytes( "\n" ) );
			stream.Close();
		}
		else FileSystem.Data.WriteAllText( BanlistPath, "" );
	}
}

public static class ClientExtensions
{
	public static void Ban( this Client c )
	{
		if(c.IsListenServerHost)
		{
			Log.Error( "Can't ban host!" );
			return;
		}
		c.Kick();
		SandboxGame.AddToBanList( c.SteamId );
	}

	public static bool IsBanned( this Client c )
		=> SandboxGame.IsBanned( c.SteamId );
}
