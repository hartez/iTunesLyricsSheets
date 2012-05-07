namespace LyricSheet
{
	public class SongLayout
	{
		public SongLayout(float fontSize, Song songPage, bool firstPage)
		{
			FontSize = fontSize;
			SongPage = songPage;
			FirstPage = firstPage;
		}

		public float FontSize { get; set; }
		public Song SongPage { get; set; }

		public bool FirstPage { get; set; }
	}
}