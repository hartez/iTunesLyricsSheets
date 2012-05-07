using System;
using System.Collections.Generic;
using System.IO;

namespace LyricSheet
{
	public class Song
	{
		public Song(string lyrics, string title, string artist)
		{
			Lyrics = lyrics;
			Title = title;
			Artist = artist;
		}

		public string Lyrics { get; set; }
		public string Title { get; set; }
		public string Artist { get; set; }

		public int CountLines()
		{
			int lines = 0;

			using (var sr = new StringReader(Lyrics))
			{
				while (sr.ReadLine() != null)
				{
					lines++;
				}
			}

			if (!String.IsNullOrEmpty(Artist))
			{
				lines += 1;
			}

			if (!String.IsNullOrEmpty(Title))
			{
				lines += 1;
			}

			if(!String.IsNullOrEmpty(Artist + Title))
			{
				lines += 1;
			}

			return lines;
		}

		public string FindLongestLine()
		{
			string longestLine = String.Empty;

			using (var sr = new StringReader(Lyrics))
			{
				string currentLine;
				while ((currentLine = sr.ReadLine()) != null)
				{
					if (currentLine.Length > longestLine.Length)
					{
						longestLine = currentLine;
					}
				}
			}

			if (!String.IsNullOrEmpty(Artist))
			{
				if (Artist.Length > longestLine.Length)
				{
					longestLine = Artist;
				}
			}

			if (!String.IsNullOrEmpty(Title))
			{
				if (Title.Length > longestLine.Length)
				{
					longestLine = Title;
				}
			}

			return longestLine;
		}

		public Song Subsong(int start, int length)
		{
			int index = 0;
			int linesAccumulated = 0;

			string lyrics = String.Empty;

			using (var sr = new StringReader(Lyrics))
			{
				string currentLine;
				while ((currentLine = sr.ReadLine()) != null)
				{
					if (index >= start && linesAccumulated <= length)
					{
						lyrics += currentLine + "\n";
						linesAccumulated += 1;
					}

					index++;
				}
			}

			return new Song(lyrics, Title, Artist);
		}

		#region Nested type: SongLengthComparer

		public class SongLengthComparer : IComparer<Song>
		{
			#region IComparer<Song> Members

			public int Compare(Song x, Song y)
			{
				return y.CountLines().CompareTo(x.CountLines());
			}

			#endregion
		}

		#endregion
	}
}