using System.Collections.Generic;
using iTunesLib;

namespace LyricSheet
{
	public class LyricSheet
	{
		private iTunesApp _itunes;

		private iTunesApp iTunes
		{
			get { return _itunes ?? (_itunes = new iTunesApp()); }
		}

		public List<Song> GetLyrics(string name)
		{
			var playlist = GetPlaylist(name);

			var trackEnum = playlist.Tracks.GetEnumerator();

			var result = new List<Song>();

			while (trackEnum.MoveNext())
			{
				var track = trackEnum.Current as IITFileOrCDTrack;

				if (track != null)
				{
					result.Add(new Song(track.Lyrics, track.Name, track.Artist));
				}
			}

			return result;
		}

		private IITPlaylist GetPlaylist(string name)
		{
			IITPlaylistCollection playlistCollection = iTunes.LibrarySource.Playlists;

			return playlistCollection.ItemByName[name];
		}
	}
}