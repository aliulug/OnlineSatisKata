namespace OnlineSatisKata
{
	public interface IUrunDepoYoneticisi
	{
		bool UrunBlokla(SiparisBilgileri siparisBilgileri);
		void UrunBloguKaldir(SiparisBilgileri siparisBilgileri);
		void BloguSatisaCevir(SiparisBilgileri siparisBilgileri);
	}
}