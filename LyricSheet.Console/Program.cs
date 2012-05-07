using System.Collections.Generic;
using System.Windows.Forms;

namespace LyricSheet.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var l = new LyricSheet();

			List<Song> lyrics = l.GetLyrics("Disc 1");

			lyrics.Sort(new Song.SongLengthComparer());

			var printDialog = new PrintPreviewDialog();

			printDialog.Document = new LyricsSheetDocument(lyrics);
			printDialog.ShowDialog();

			var pd = new PrintDialog();
			pd.Document = new LyricsSheetDocument(lyrics);
			if (pd.ShowDialog() == DialogResult.OK)
			{
				pd.Document.Print();
			}
		}
	}
}