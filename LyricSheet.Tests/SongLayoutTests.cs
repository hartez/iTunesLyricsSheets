using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace LyricSheet.Tests
{
	[TestFixture]
	public class SongLayoutTests
	{
		[Test]
		public void FourSongBookletPrint()
		{
			var orderedTitles = new List<string> {"Song 1", "Song 2", "Song 3", "Song 4"};

			var layouts = new List<SongLayout>();

			foreach (string orderedTitle in orderedTitles)
			{
				layouts.Add(new SongLayout(10, new Song(String.Empty, orderedTitle, String.Empty), true));
			}

			var lsd = new LyricsSheetDocument(new List<Song>());

			List<SongLayout> result = lsd.SortForBookletPrint(layouts);

			var expectedTitles = new List<string> {"Song 4", "Song 1", "Song 2", "Song 3"};

			result.Should().HaveCount(expectedTitles.Count);
			result.Select(sl => sl.SongPage.Title).Should().ContainInOrder(expectedTitles);
		}

		[Test]
		public void TwelveSongBookletPrint()
		{
			var orderedTitles = new List<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"};

			var layouts = new List<SongLayout>();

			foreach (string orderedTitle in orderedTitles)
			{
				layouts.Add(new SongLayout(10, new Song(String.Empty, orderedTitle, String.Empty), true));
			}

			var lsd = new LyricsSheetDocument(new List<Song>());

			List<SongLayout> result = lsd.SortForBookletPrint(layouts);

			var expectedTitles = new List<string> {"12", "1", "2", "11", "10", "3", "4", "9", "8", "5", "6", "7"};

			result.Should().HaveCount(expectedTitles.Count);
			result.Select(sl => sl.SongPage.Title).Should().ContainInOrder(expectedTitles);
		}

		[Test]
		public void ThreeSongBookletPrint()
		{
			var orderedTitles = new List<string> { "1", "2", "3" };

			var layouts = new List<SongLayout>();

			foreach (string orderedTitle in orderedTitles)
			{
				layouts.Add(new SongLayout(10, new Song(String.Empty, orderedTitle, String.Empty), true));
			}

			var lsd = new LyricsSheetDocument(new List<Song>());

			List<SongLayout> result = lsd.SortForBookletPrint(layouts);

			var expectedTitles = new List<string> { "", "1", "2", "3" };

			result.Should().HaveCount(expectedTitles.Count);
			result.Select(sl => sl.SongPage.Title).Should().ContainInOrder(expectedTitles);
		}
	}
}