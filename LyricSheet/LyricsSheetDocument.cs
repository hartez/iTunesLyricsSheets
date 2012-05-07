using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace LyricSheet
{
	public class LyricsSheetDocument : PrintDocument
	{
		private List<Song> _lyrics;

		private List<SongLayout> _printlyrics;

		private List<SongLayout>.Enumerator _lyricEnumerator;

		private int _minFontSize = 8;
		private float _magicLineHeightFactor = 0.6f;
		private float _gutter = 25f;

		public List<SongLayout> SortForBookletPrint(List<SongLayout> orderedPages)
		{
			var pages = orderedPages.Count;

			var sheets = pages / 4;

			if (pages % 4 != 0)
			{
				sheets += 1;
			}

			var bookletpages = new List<SongLayout>(sheets * 4);

			// Start with blank pages
			for (int i = 0; i < bookletpages.Capacity; i++)
			{
				bookletpages.Add(new SongLayout(10, new Song(String.Empty, String.Empty, String.Empty), true));
			}

			var p = pages / 2;
			if (pages % 2 != 0)
			{
				p += 1;
			}

			var factor = -1;
			var scale = 0;

			for (int n = bookletpages.Count - 1; n >= 0; n -= 2)
			{
				var rightIndex = (p + 1) - (factor*scale) - 1;
				var leftIndex = p + (factor*scale) - 1;

				if (rightIndex > -1 && rightIndex < orderedPages.Count)
				{
					bookletpages[n] = orderedPages[rightIndex];
				}

				if (leftIndex > -1 && leftIndex < orderedPages.Count)
				{
					bookletpages[n - 1] = orderedPages[leftIndex];
				}

				if (factor == -1)
				{
					scale += 2;
				}

				factor = -factor;
			}

			return bookletpages;
		}

		private List<SongLayout> ArrangeForPrint(Graphics gdiPage)
		{
			List<SongLayout> result = new List<SongLayout>();

			foreach (var song in _lyrics)
			{
				var lineHeight = 425 / song.CountLines();
				var fontsize = lineHeight * _magicLineHeightFactor;

				if (fontsize < _minFontSize)
				{
					fontsize = _minFontSize;
				}

				// Check to see if the longest line will fit
				var longestLine = song.FindLongestLine();
				var size = gdiPage.MeasureString(longestLine, new Font(fontFamily, fontsize));

				while (size.Width > (DefaultPageSettings.PrintableArea.Width / 2) + _gutter)
				{
					fontsize = fontsize - 0.1f;
					size = gdiPage.MeasureString(longestLine, new Font(fontFamily, fontsize));
				}

				// How many lines can we fit at this font size?
				int linesOnPage = (int)(425 / (fontsize / _magicLineHeightFactor));
				int sofar = 0;
				if (linesOnPage >= song.CountLines())
				{
					result.Add(new SongLayout(fontsize, song, true));
				}
				else
				{
					while (sofar < song.CountLines())
					{
						result.Add(new SongLayout(fontsize, song.Subsong(sofar, linesOnPage), sofar == 0));
						sofar += linesOnPage;
					}
				}

			}

			return result;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LyricsSheetDocument"/> class.
		/// </summary>
		/// <param name="songs">The songs.</param>
		/// <remarks></remarks>
		public LyricsSheetDocument(List<Song> songs)
		{
			_lyrics = songs;
			DefaultPageSettings.Landscape = true;
			DefaultPageSettings.Margins.Left = 75;
			DefaultPageSettings.Margins.Top = 175;
			DefaultPageSettings.Margins.Bottom = 212;
			DefaultPageSettings.Margins.Right = 150;
		}

		private string fontFamily = "Georgia";

		private void PrintSong(SongLayout song, Rectangle bounds, Graphics gdiPage)
		{
			var font = new Font(fontFamily, song.FontSize);
			var lineHeight = song.FontSize / _magicLineHeightFactor;

			float top = bounds.Top;
			float left = bounds.Left + (_gutter);

			if(song.FirstPage)
			{
				var titleFont = new Font(fontFamily, song.FontSize, FontStyle.Bold | FontStyle.Italic);

				gdiPage.DrawString(song.SongPage.Title, titleFont, Brushes.Black,
									   left, bounds.Top);

				gdiPage.DrawString(song.SongPage.Artist, titleFont, Brushes.Black,
									   left, bounds.Top + lineHeight);

				top = top + (3*lineHeight);
			}

			using (var sr = new StringReader(song.SongPage.Lyrics))
			{
				string lineText;
				int lineIndex = 0;
				while ((lineText = sr.ReadLine()) != null)
				{
					gdiPage.DrawString(lineText, font, Brushes.Black,
									   left, (top + (lineIndex * lineHeight)));
					lineIndex++;
				}
			}
		}

		private bool _layedout = false;
		private bool front = true;

		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage(e);
			
			Graphics gdiPage = e.Graphics;

			if(!_layedout)
			{
				_printlyrics = SortForBookletPrint(ArrangeForPrint(gdiPage));

				_lyricEnumerator = _printlyrics.GetEnumerator();
				_lyricEnumerator.MoveNext();

				_layedout = true;
			}

			if(front)
			{
				e.PageSettings.Margins.Left = DefaultPageSettings.Margins.Left;
				e.PageSettings.Margins.Top = DefaultPageSettings.Margins.Top;
				e.PageSettings.Margins.Bottom = DefaultPageSettings.Margins.Bottom;
				e.PageSettings.Margins.Right = DefaultPageSettings.Margins.Right;
			}
			else
			{
				e.PageSettings.Margins.Left = DefaultPageSettings.Margins.Right;
				e.PageSettings.Margins.Top = DefaultPageSettings.Margins.Top;
				e.PageSettings.Margins.Bottom = DefaultPageSettings.Margins.Bottom;
				e.PageSettings.Margins.Right = DefaultPageSettings.Margins.Left;
			}

			// Print the left side
			if (_lyricEnumerator.Current != null)
			{
				var bounds = new Rectangle(e.PageSettings.Margins.Left, e.MarginBounds.Top, e.MarginBounds.Width / 2, e.MarginBounds.Height);
				PrintSong(_lyricEnumerator.Current, bounds, gdiPage);
			}

			// Print the right side
			if (_lyricEnumerator.MoveNext())
			{
				var bounds = new Rectangle(e.PageSettings.Margins.Left + (e.MarginBounds.Width / 2), e.MarginBounds.Top, e.MarginBounds.Width / 2, e.MarginBounds.Height);
				PrintSong(_lyricEnumerator.Current, bounds, gdiPage);
			}

			// If more lines exist, print another page.
			e.HasMorePages = _lyricEnumerator.MoveNext();
			front = !front;
		}
	}
}