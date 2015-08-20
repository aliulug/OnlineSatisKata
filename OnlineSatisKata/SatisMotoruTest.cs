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
		private ISanalPosVekili _posVekili;
		private ISmsYoneticisi _smsYoneticisi;
		private SatisMotoru _satisMotoru;
		private SiparisBilgileri _siparisBilgileri;
		private SmsOnayKodu _smsOnayKodu;

		[SetUp]
		public void test_ilk_ayarlar()
		{
			_depoYoneticisi = Substitute.For<IUrunDepoYoneticisi>();
			_posVekili = Substitute.For<ISanalPosVekili>();
			_smsYoneticisi = Substitute.For<ISmsYoneticisi>();
			_satisMotoru = new SatisMotoru(_depoYoneticisi, _posVekili, _smsYoneticisi);
			_siparisBilgileri = new SiparisBilgileri();
			_smsOnayKodu = new SmsOnayKodu();
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

		[Test]
		public void satista_urun_bloklanirsa_bankaya_cekim_istegi_gonderilir()
		{
			_depoYoneticisi.UrunBlokla(_siparisBilgileri).Returns(true);

			SatisSonucu satisSonucu = _satisMotoru.SatisYap(_siparisBilgileri);

			_posVekili.Received().BankayaCekimIstegiGonder(_siparisBilgileri);
		}

		[Test]
		public void satista_urun_bloklanir_ama_kart_cekilemesse_hata_doner_blokaj_kalkar()
		{
			_depoYoneticisi.UrunBlokla(_siparisBilgileri).Returns(true);
			_posVekili.BankayaCekimIstegiGonder(_siparisBilgileri).Returns(false);

			SatisSonucu satisSonucu = _satisMotoru.SatisYap(_siparisBilgileri);

			Assert.AreEqual(SatisHataTipi.KartCekimiBasarisiz, satisSonucu.HataTipi);
			Assert.IsFalse(satisSonucu.SatisBasarili);
			_depoYoneticisi.Received().UrunBloguKaldir(_siparisBilgileri);
		}

		[Test]
		public void satista_urun_bloklanir_kart_cekilebilirse_blok_satisa_cevrilsin_true_donsun()
		{
			_depoYoneticisi.UrunBlokla(_siparisBilgileri).Returns(true);
			_posVekili.BankayaCekimIstegiGonder(_siparisBilgileri).Returns(true);

			SatisSonucu satisSonucu = _satisMotoru.SatisYap(_siparisBilgileri);

			Assert.IsTrue(satisSonucu.SatisBasarili);
			_depoYoneticisi.Received().BloguSatisaCevir(_siparisBilgileri);
		}

		[Test]
		public void satista_sms_onayi_gerekiyorsa_sms_onay_kodu_gonderilir_ve_hic_islem_yapilmaz()
		{
			_smsYoneticisi.BuSatisIcinSmsOnayiGerekiyorMu(_siparisBilgileri).Returns(true);

			SatisSonucu satisSonucu = _satisMotoru.SatisYap(_siparisBilgileri);

			Assert.IsFalse(satisSonucu.SatisBasarili);
			Assert.AreEqual(SatisHataTipi.SmsOnayiGerekiyor, satisSonucu.HataTipi);
			_depoYoneticisi.DidNotReceive().UrunBlokla(_siparisBilgileri);
		}

		[Test]
		public void onay_kodu_girildiginde_kod_dogrulanir()
		{	
			_satisMotoru.SmsOnayKoduIleSatisiTamamla(_smsOnayKodu, _siparisBilgileri);

			_smsYoneticisi.Received().OnayKoduDogruMu(_smsOnayKodu, _siparisBilgileri);
		}

		[Test]
		public void onay_kodu_hataliysa_hata_doner_ve_satis_islemleri_yapilmaz()
		{
			_smsYoneticisi.OnayKoduDogruMu(_smsOnayKodu, _siparisBilgileri).Returns(false);

			SatisSonucu satisSonucu = _satisMotoru.SmsOnayKoduIleSatisiTamamla(_smsOnayKodu, _siparisBilgileri);

			Assert.IsFalse(satisSonucu.SatisBasarili);
			Assert.AreEqual(SatisHataTipi.SmsOnayKoduHatali, satisSonucu.HataTipi);
			_posVekili.DidNotReceive().BankayaCekimIstegiGonder(_siparisBilgileri);

		}

		[Test]
		public void onay_kodu_dogru_ise_satis_islemleri_gerceklesir_true_doner()
		{
			_smsYoneticisi.OnayKoduDogruMu(_smsOnayKodu, _siparisBilgileri).Returns(true);
			_depoYoneticisi.UrunBlokla(_siparisBilgileri).Returns(true);
			_posVekili.BankayaCekimIstegiGonder(_siparisBilgileri).Returns(true);

			SatisSonucu satisSonucu = _satisMotoru.SmsOnayKoduIleSatisiTamamla(_smsOnayKodu, _siparisBilgileri);

			Assert.IsTrue(satisSonucu.SatisBasarili);
			_depoYoneticisi.Received().BloguSatisaCevir(_siparisBilgileri);
		}
	}
}
