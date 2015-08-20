using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace OnlineSatisKata
{
	[TestFixture]
	class SatisMotoruTest
	{
		private IUrunDepoYoneticisi _depoYoneticisi;
		private SatisMotoru _satisMotoru;
		private SiparisBilgileri _siparisBilgileri;

		[SetUp]
		public void test_ilk_ayarlar()
		{
			_depoYoneticisi = Substitute.For<IUrunDepoYoneticisi>();
			_satisMotoru = new SatisMotoru(_depoYoneticisi);
			_siparisBilgileri = new SiparisBilgileri();
		}

		[Test]
		public void satis_yapildiginda_urun_bloklanir()
		{
			_satisMotoru.SatisYap(_siparisBilgileri);

			_depoYoneticisi.Received().UrunBlokla(_siparisBilgileri);
		}

		[Test]
		public void satista_urun_bloklanamassa_hata_doner()
		{
			_depoYoneticisi.UrunBlokla(_siparisBilgileri).Returns(false);

			SatisSonucu satisSonucu = _satisMotoru.SatisYap(_siparisBilgileri);

			Assert.AreEqual(SatisHataTipi.UrunStoktaYok, satisSonucu.HataTipi);
			Assert.IsFalse(satisSonucu.SatisBasarili);
		}
	}

	public enum SatisHataTipi
	{
		Tanimsiz,
		UrunStoktaYok
	}

	public class SatisSonucu
	{
		public SatisSonucu(bool satisBasarili, SatisHataTipi satisHataTipi)
		{
			SatisBasarili = satisBasarili;
			HataTipi = satisHataTipi;
		}

		public SatisHataTipi HataTipi { get; set; }
		public bool SatisBasarili { get; set; }
	}

	public class SiparisBilgileri
	{
	}

	public class SatisMotoru
	{
		private readonly IUrunDepoYoneticisi _depoYoneticisi;

		public SatisMotoru(IUrunDepoYoneticisi depoYoneticisi)
		{
			_depoYoneticisi = depoYoneticisi;
		}

		public SatisSonucu SatisYap(SiparisBilgileri siparisBilgileri)
		{
			bool urunBloklandi =_depoYoneticisi.UrunBlokla(siparisBilgileri);
			if (!urunBloklandi)
				return new SatisSonucu(false, SatisHataTipi.UrunStoktaYok);

			return null;
		}
	}

	public interface IUrunDepoYoneticisi
	{
		bool UrunBlokla(SiparisBilgileri siparisBilgileri);
	}
}
