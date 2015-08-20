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
		[Test]
		public void satis_yapildiginda_urun_bloklanir()
		{
			IUrunDepoYoneticisi depoYoneticisi = Substitute.For<IUrunDepoYoneticisi>();
			SatisMotoru satisMotoru = new SatisMotoru(depoYoneticisi);
			SiparisBilgileri siparisBilgileri = new SiparisBilgileri();
			satisMotoru.SatisYap(siparisBilgileri);

			depoYoneticisi.Received().UrunBlokla(siparisBilgileri);
		}

		[Test]
		public void satista_urun_bloklanamassa_hata_doner()
		{
			IUrunDepoYoneticisi depoYoneticisi = Substitute.For<IUrunDepoYoneticisi>();
			SatisMotoru satisMotoru = new SatisMotoru(depoYoneticisi);
			SiparisBilgileri siparisBilgileri = new SiparisBilgileri();
			depoYoneticisi.UrunBlokla(siparisBilgileri).Returns(false);

			SatisSonucu satisSonucu = satisMotoru.SatisYap(siparisBilgileri);

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
