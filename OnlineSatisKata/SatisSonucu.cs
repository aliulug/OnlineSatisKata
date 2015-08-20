namespace OnlineSatisKata
{
	public class SatisSonucu
	{
		public SatisSonucu(bool satisBasarili, SatisHataTipi satisHataTipi = SatisHataTipi.Tanimsiz)
		{
			SatisBasarili = satisBasarili;
			HataTipi = satisHataTipi;
		}

		public SatisHataTipi HataTipi { get; set; }
		public bool SatisBasarili { get; set; }
	}
}