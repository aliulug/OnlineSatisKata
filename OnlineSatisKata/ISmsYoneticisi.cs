namespace OnlineSatisKata
{
	public interface ISmsYoneticisi
	{
		bool BuSatisIcinSmsOnayiGerekiyorMu(SiparisBilgileri siparisBilgileri);
		bool OnayKoduDogruMu(SmsOnayKodu smsOnayKodu, SiparisBilgileri siparisBilgileri);
	}
}