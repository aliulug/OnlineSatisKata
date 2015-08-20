namespace OnlineSatisKata
{
	public class SatisMotoru
	{
		private readonly IUrunDepoYoneticisi _depoYoneticisi;
		private readonly ISanalPosVekili _posVekili;
		private readonly ISmsYoneticisi _smsYoneticisi;

		public SatisMotoru(IUrunDepoYoneticisi depoYoneticisi, ISanalPosVekili posVekili, ISmsYoneticisi smsYoneticisi)
		{
			_depoYoneticisi = depoYoneticisi;
			_posVekili = posVekili;
			_smsYoneticisi = smsYoneticisi;
		}

		public SatisSonucu SatisYap(SiparisBilgileri siparisBilgileri)
		{
			bool smsOnayiGerekir = _smsYoneticisi.BuSatisIcinSmsOnayiGerekiyorMu(siparisBilgileri);
			if (smsOnayiGerekir)
				return new SatisSonucu(false, SatisHataTipi.SmsOnayiGerekiyor);

			return satisIslemleriniYap(siparisBilgileri);
		}

		private SatisSonucu satisIslemleriniYap(SiparisBilgileri siparisBilgileri)
		{
			bool urunBloklandi = _depoYoneticisi.UrunBlokla(siparisBilgileri);
			if (!urunBloklandi)
				return new SatisSonucu(false, SatisHataTipi.UrunStoktaYok);

			bool kartCekimiBasarili = _posVekili.BankayaCekimIstegiGonder(siparisBilgileri);

			if (!kartCekimiBasarili)
			{
				_depoYoneticisi.UrunBloguKaldir(siparisBilgileri);
				return new SatisSonucu(false, SatisHataTipi.KartCekimiBasarisiz);
			}

			_depoYoneticisi.BloguSatisaCevir(siparisBilgileri);
			return new SatisSonucu(true);
		}

		public SatisSonucu SmsOnayKoduIleSatisiTamamla(SmsOnayKodu smsOnayKodu, SiparisBilgileri siparisBilgileri)
		{
			bool onayKoduDogru = _smsYoneticisi.OnayKoduDogruMu(smsOnayKodu, siparisBilgileri);
			if (!onayKoduDogru)
				return new SatisSonucu(false, SatisHataTipi.SmsOnayKoduHatali);

			return satisIslemleriniYap(siparisBilgileri);
		}
	}
}