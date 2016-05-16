/******
Copyright (C) 2005 Ajit Krishnan (http://www.mudgala.com)

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
******/

using System;
using System.Diagnostics;

namespace mhora
{
	/// <summary>
	/// Summary description for Kutas.
	/// </summary>

	public class KutaBhutaNakshatra
	{
		public static int getMaxScore ()
		{
			return 1;
		}
		public static int getScore (Nakshatra m, Nakshatra n)
		{
			EType a = getType(m);
			EType b = getType(n);
			if (a == b) return 1;
			
			if ((a == EType.IFire && b == EType.IAir) ||
				(a == EType.IAir && b == EType.IFire)) return 1;

			if (a == EType.IEarth || b == EType.IEarth) return 1;

			return 0;

		}
		public enum EType
		{
			IEarth, IWater, IFire, IAir, IEther
		};
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Krittika:
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Mrigarirsa:
					return EType.IEarth;
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.PoorvaPhalguni:
					return EType.IWater;
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Chittra:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Vishaka:
					return EType.IFire;
				case Nakshatra.Name.Anuradha:
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.UttaraShada:
				case Nakshatra.Name.Sravana:
					return EType.IAir;
				case Nakshatra.Name.Dhanishta:
				case Nakshatra.Name.Satabisha:
				case Nakshatra.Name.PoorvaBhadra:
				case Nakshatra.Name.UttaraBhadra:
				case Nakshatra.Name.Revati:
					return EType.IEther;
			}
			Debug.Assert(false, "KutaBhutaNakshatra::getType");
			return EType.IAir;
		}
	}
	public class KutaVihanga
	{
		public enum EDominator
		{
			IEqual, IMale, IFemale
		};
		public enum EType
		{
			IBharandhaka, IPingala, ICrow, ICock, IPeacock
		};
		public static EDominator getDominator (Nakshatra m, Nakshatra n)
		{
			EType em = getType(m);
			EType en = getType(n);

			EType[] order = new EType[] 
			{
				EType.IPeacock, EType.ICock, EType.ICrow, EType.IPingala
			};
			if (em == en) return EDominator.IEqual;
			for (int i=0; i<order.Length; i++)
			{
				if (em == order[i]) return EDominator.IMale;
				if (en == order[i]) return EDominator.IFemale;
			}
			return EDominator.IEqual;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Krittika:
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Mrigarirsa:
					return EType.IBharandhaka;
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.PoorvaPhalguni:
					return EType.IPingala;
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Chittra:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Vishaka:
				case Nakshatra.Name.Anuradha:
					return EType.ICrow;
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.UttaraShada:
				case Nakshatra.Name.Sravana:
					return EType.ICock;
				case Nakshatra.Name.Dhanishta:
				case Nakshatra.Name.Satabisha:
				case Nakshatra.Name.PoorvaBhadra:
				case Nakshatra.Name.UttaraBhadra:
				case Nakshatra.Name.Revati:
					return EType.IPeacock;
			}
			Debug.Assert(false, "KutaVibhanga::getType");
			return EType.IBharandhaka;
		}
	}
	public class KutaGotra
	{
		public enum EType
		{
			IMarichi, IVasishtha, IAngirasa, IAtri, IPulastya,
			IPulaha, IKretu
		};
		public static int getScore (Nakshatra m, Nakshatra n)
		{
			if (getType(m) == getType(n)) return 0;
			return 1;
		}
		public static int getMaxScore ()
		{
			return 1;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Swati:
					return EType.IMarichi;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Vishaka:
				case Nakshatra.Name.Sravana:
					return EType.IVasishtha;
				case Nakshatra.Name.Krittika:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.Anuradha:
				case Nakshatra.Name.Dhanishta:
					return EType.IAngirasa;
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.PoorvaPhalguni:
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Satabisha:
					return EType.IAtri;
				case Nakshatra.Name.Mrigarirsa:
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.PoorvaBhadra:
					return EType.IPulastya;
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.UttaraBhadra:
					return EType.IPulaha;
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.Chittra:
				case Nakshatra.Name.UttaraShada:
				case Nakshatra.Name.Revati:
					return EType.IKretu;
			}
			Debug.Assert(false, "KutaGotra::getType");
			return EType.IAngirasa;
		}
	}
	public class KutaNadi
	{
		public enum EType
		{
			IVata, IPitta, ISleshma
		};
		public static int getMaxScore ()
		{
			return 2;
		}
		public static int getScore (Nakshatra m, Nakshatra n)
		{
			EType ea = getType(m);
			EType eb = getType(n);
			if (ea != eb) return 2;
			if (ea == EType.IVata || ea == EType.ISleshma) return 1;
			return 0;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.Satabisha:
				case Nakshatra.Name.PoorvaBhadra:
					return EType.IVata;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Mrigarirsa:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.PoorvaPhalguni:
				case Nakshatra.Name.Chittra:
				case Nakshatra.Name.Anuradha:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.Dhanishta:
				case Nakshatra.Name.UttaraBhadra:
					return EType.IPitta;
			}
			return EType.ISleshma;
		}
	}
	public class KutaRajju
	{
		public enum EType
		{
			IKantha, IKati, IPada, ISiro, IKukshi
		};
		public static int getScore (Nakshatra m, Nakshatra n)
		{
			if (getType(m) != getType(n)) return 1;
			return 0;
		}
		public static int getMaxScore ()
		{
			return 1;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Sravana:
				case Nakshatra.Name.Satabisha:
					return EType.IKantha;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.PoorvaPhalguni:
				case Nakshatra.Name.Anuradha:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.UttaraBhadra:
					return EType.IKati;
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.Revati:
					return EType.IPada;
				case Nakshatra.Name.Mrigarirsa:
				case Nakshatra.Name.Dhanishta:
				case Nakshatra.Name.Chittra:
					return EType.ISiro;
			}
			return EType.IKukshi;
		}
		
	}
	public class KutaVedha
	{
		public enum EType
		{
			IAswJye, IBhaAnu, IKriVis, IRohSwa, IAriSra,
			IPunUsh, IPusPsh, IAslMoo, IMakRev, IPphUbh,
			IUphPbh, IHasSat, IMriDha, IChi
		}
		public static int getScore (Nakshatra m, Nakshatra n)
		{
			if (getType(m) == getType(n)) return 0;
			return 1;
		}
		public static int getMaxScore ()
		{
			return 1;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Jyestha:
					return EType.IAswJye;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Anuradha:
					return EType.IBhaAnu;
				case Nakshatra.Name.Krittika:
				case Nakshatra.Name.Vishaka:
					return EType.IKriVis;
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Swati:
					return EType.IRohSwa;
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.Sravana:
					return EType.IAriSra;
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.UttaraShada:
					return EType.IPunUsh;
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.PoorvaShada:
					return EType.IPusPsh;
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Moola:
					return EType.IAslMoo;
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.Revati:
					return EType.IMakRev;
				case Nakshatra.Name.PoorvaPhalguni:
				case Nakshatra.Name.UttaraBhadra:
					return EType.IPphUbh;
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.PoorvaBhadra:
					return EType.IUphPbh;
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Satabisha:
					return EType.IHasSat;
				case Nakshatra.Name.Mrigarirsa:
				case Nakshatra.Name.Dhanishta:
					return EType.IMriDha;
				case Nakshatra.Name.Chittra:
					return EType.IChi;
			}
			Debug.Assert(false, "KutaVedha::getType");
			return EType.IAriSra;
		}
	}
	public class KutaGana
	{
		public enum EType
		{
			IDeva, INara, IRakshasa
		};
		public static int getScore (Nakshatra m, Nakshatra f)
		{
			EType em = KutaGana.getType(m);
			EType ef = KutaGana.getType(f);

			if (em == ef) return 5;
			if (em == EType.IDeva && ef == EType.INara) return 4;
			if (em == EType.IRakshasa && ef == EType.INara) return 3;
			if (em == EType.INara && ef == EType.IDeva) return 2;
			return 1;
		}
		public static int getMaxScore ()
		{
			return 5;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Mrigarirsa:
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Anuradha:
				case Nakshatra.Name.Sravana:
				case Nakshatra.Name.Revati:
					return EType.IDeva;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Aridra:
				case Nakshatra.Name.PoorvaPhalguni:
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.UttaraShada:
				case Nakshatra.Name.PoorvaBhadra:
				case Nakshatra.Name.UttaraBhadra:
					return EType.INara;
			}
			return EType.IRakshasa;
		}
	}
	public class KutaVarna
	{
		public enum EType
		{
			IBrahmana, IKshatriya, IVaishya, ISudra, IAnuloma, IPratiloma
		};
		public static int getMaxScore ()
		{
			return 2;
		}
		public static int getScore (Nakshatra m, Nakshatra f)
		{
			EType em = getType(m);
			EType ef = getType(f);
			if (em == ef) return 2;
			if (em == EType.IBrahmana && 
				(ef == EType.IKshatriya || ef == EType.IVaishya || ef == EType.ISudra)) 
				return 1;
			if (em == EType.IKshatriya &&
				(ef == EType.IVaishya || ef == EType.ISudra))
				return 1;
			if (em == EType.IVaishya && ef == EType.ISudra) return 1;
			if (em == EType.IAnuloma && ef != EType.IPratiloma) return 1;
			if (ef == EType.IAnuloma && em != EType.IAnuloma) return 1;
			return 0;
		}
		public static EType getType (Nakshatra n)
		{
			switch (((int)n.value)%6)
			{
				case 1: return EType.IBrahmana;
				case 2: return EType.IKshatriya;
				case 3: return EType.IVaishya;
				case 4: return EType.ISudra;
				case 5: return EType.IAnuloma;
				case 0: return EType.IPratiloma;
				case 6: return EType.IPratiloma;
			}
			Debug.Assert(false, "KutaVarna::getType");
			return EType.IAnuloma;
		}
	}

	public class KutaRasiYoni
	{
		public enum EType
		{
			IPakshi, IReptile, IPasu, INara
		};
		public static EType getType (ZodiacHouse z)
		{
			switch (z.value)
			{
				case ZodiacHouse.Name.Cap:
				case ZodiacHouse.Name.Pis:
					return EType.IPakshi;
				case ZodiacHouse.Name.Can:
				case ZodiacHouse.Name.Sco:
					return EType.IReptile;
				case ZodiacHouse.Name.Ari:
				case ZodiacHouse.Name.Tau:
				case ZodiacHouse.Name.Leo:
					return EType.IPasu;
			}
			return EType.INara;
		}
	}

	public class GhatakaLagnaOpp
	{
		static public bool checkLagna (ZodiacHouse janma, ZodiacHouse same)
		{
			ZodiacHouse.Name ja = janma.value;
			ZodiacHouse.Name gh = ZodiacHouse.Name.Ari;
			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = ZodiacHouse.Name.Lib; break;
				case ZodiacHouse.Name.Tau: gh = ZodiacHouse.Name.Sco; break;
				case ZodiacHouse.Name.Gem: gh = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Can: gh = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Leo: gh = ZodiacHouse.Name.Can; break;
				case ZodiacHouse.Name.Vir: gh = ZodiacHouse.Name.Vir; break;
				case ZodiacHouse.Name.Lib: gh = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Sco: gh = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Sag: gh = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Cap: gh = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Aqu: gh = ZodiacHouse.Name.Sag; break;
				case ZodiacHouse.Name.Pis: gh = ZodiacHouse.Name.Aqu; break;
			}
			return same.value == gh;
		}
	}


	public class GhatakaLagnaSame
	{
		static public bool checkLagna (ZodiacHouse janma, ZodiacHouse same)
		{
			ZodiacHouse.Name ja = janma.value;
			ZodiacHouse.Name gh = ZodiacHouse.Name.Ari;
			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Tau: gh = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Gem: gh = ZodiacHouse.Name.Can; break;
				case ZodiacHouse.Name.Can: gh = ZodiacHouse.Name.Lib; break;
				case ZodiacHouse.Name.Leo: gh = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Vir: gh = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Lib: gh = ZodiacHouse.Name.Vir; break;
				case ZodiacHouse.Name.Sco: gh = ZodiacHouse.Name.Sco; break;
				case ZodiacHouse.Name.Sag: gh = ZodiacHouse.Name.Sag; break;
				case ZodiacHouse.Name.Cap: gh = ZodiacHouse.Name.Aqu; break;
				case ZodiacHouse.Name.Aqu: gh = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Pis: gh = ZodiacHouse.Name.Leo; break;
			}
			return same.value == gh;
		}
	}

	public class GhatakaStar
	{
		static public bool checkStar (ZodiacHouse janmaRasi, Nakshatra nak)
		{
			ZodiacHouse.Name ja = janmaRasi.value;
			Nakshatra.Name gh = Nakshatra.Name.Aswini;
			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = Nakshatra.Name.Makha; break;
				case ZodiacHouse.Name.Tau: gh = Nakshatra.Name.Hasta; break;
				case ZodiacHouse.Name.Gem: gh = Nakshatra.Name.Swati; break;
				case ZodiacHouse.Name.Can: gh = Nakshatra.Name.Anuradha; break;
				case ZodiacHouse.Name.Leo: gh = Nakshatra.Name.Moola; break;
				case ZodiacHouse.Name.Vir: gh = Nakshatra.Name.Sravana; break;
				case ZodiacHouse.Name.Lib: gh = Nakshatra.Name.Satabisha; break;
				case ZodiacHouse.Name.Sco: gh = Nakshatra.Name.Revati; break;
				// FIXME dveja nakshatra?????
				case ZodiacHouse.Name.Sag: gh = Nakshatra.Name.Revati; break;
				case ZodiacHouse.Name.Cap: gh = Nakshatra.Name.Rohini; break;
				case ZodiacHouse.Name.Aqu: gh = Nakshatra.Name.Aridra; break;
				case ZodiacHouse.Name.Pis: gh = Nakshatra.Name.Aslesha; break;
			}
			return nak.value == gh;
		}
	}

	public class GhatakaDay
	{
		static public bool checkDay (ZodiacHouse janmaRasi, Basics.Weekday wd)
		{
			ZodiacHouse.Name ja = janmaRasi.value;
			Basics.Weekday gh = Basics.Weekday.Sunday;
			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = Basics.Weekday.Sunday; break;
				case ZodiacHouse.Name.Tau: gh = Basics.Weekday.Saturday; break;
				case ZodiacHouse.Name.Gem: gh = Basics.Weekday.Monday; break;
				case ZodiacHouse.Name.Can: gh = Basics.Weekday.Wednesday; break;
				case ZodiacHouse.Name.Leo: gh = Basics.Weekday.Saturday; break;
				case ZodiacHouse.Name.Vir: gh = Basics.Weekday.Saturday; break;
				case ZodiacHouse.Name.Lib: gh = Basics.Weekday.Thursday; break;
				case ZodiacHouse.Name.Sco: gh = Basics.Weekday.Friday; break;
				case ZodiacHouse.Name.Sag: gh = Basics.Weekday.Friday; break;
				case ZodiacHouse.Name.Cap: gh = Basics.Weekday.Tuesday; break;
				case ZodiacHouse.Name.Aqu: gh = Basics.Weekday.Thursday; break;
				case ZodiacHouse.Name.Pis: gh = Basics.Weekday.Friday; break;
			}
			return wd == gh;
		}
	}

	public class GhatakaTithi
	{
		static public bool checkTithi (ZodiacHouse janmaRasi, Tithi t)
		{
			ZodiacHouse.Name ja = janmaRasi.value;
			Tithi.NandaType gh = Tithi.NandaType.Nanda;
			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = Tithi.NandaType.Nanda; break;
				case ZodiacHouse.Name.Tau: gh = Tithi.NandaType.Purna; break;
				case ZodiacHouse.Name.Gem: gh = Tithi.NandaType.Bhadra; break;
				case ZodiacHouse.Name.Can: gh = Tithi.NandaType.Bhadra; break;
				case ZodiacHouse.Name.Leo: gh = Tithi.NandaType.Jaya; break;
				case ZodiacHouse.Name.Vir: gh = Tithi.NandaType.Purna; break;
				case ZodiacHouse.Name.Lib: gh = Tithi.NandaType.Rikta; break;
				case ZodiacHouse.Name.Sco: gh = Tithi.NandaType.Nanda; break;
				case ZodiacHouse.Name.Sag: gh = Tithi.NandaType.Jaya; break;
				case ZodiacHouse.Name.Cap: gh = Tithi.NandaType.Rikta; break;
				case ZodiacHouse.Name.Aqu: gh = Tithi.NandaType.Jaya; break;
				case ZodiacHouse.Name.Pis: gh = Tithi.NandaType.Purna; break;
			}
			return t.toNandaType() == gh;
		}
	}

	public class GhatakaMoon
	{
		static public bool checkGhataka (ZodiacHouse janmaRasi, ZodiacHouse chandraRasi)
		{
			ZodiacHouse.Name ja = janmaRasi.value;
			ZodiacHouse.Name ch = chandraRasi.value;

			ZodiacHouse.Name gh = ZodiacHouse.Name.Ari;

			switch (ja)
			{
				case ZodiacHouse.Name.Ari: gh = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Tau: gh = ZodiacHouse.Name.Vir; break;
				case ZodiacHouse.Name.Gem: gh = ZodiacHouse.Name.Aqu; break;
				case ZodiacHouse.Name.Can: gh = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Leo: gh = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Vir: gh = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Lib: gh = ZodiacHouse.Name.Sag; break;
				case ZodiacHouse.Name.Sco: gh = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Sag: gh = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Cap: gh = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Aqu: gh = ZodiacHouse.Name.Sag; break;
				case ZodiacHouse.Name.Pis: gh = ZodiacHouse.Name.Aqu; break;
			}

			return ch == gh;
		}
	}
	public class KutaNakshatraYoni 
	{
		public enum EType
		{
			IHorse, IElephant, ISheep, ISerpent, IDog, ICat, IRat, ICow,
			IBuffalo, ITiger, IHare, IMonkey, ILion, IMongoose
		};
		public enum ESex
		{
			IMale, IFemale
		};

		public static ESex getSex (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Vishaka:
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.PoorvaBhadra:
				case Nakshatra.Name.UttaraShada:
					return ESex.IMale;
			}
			return ESex.IFemale;
		}
		public static EType getType (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Satabisha:
					return EType.IHorse;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Revati:
					return EType.IElephant;
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Krittika:
					return EType.ISheep;
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Mrigarirsa:
					return EType.ISerpent;
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.Aridra:
					return EType.IDog;
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Punarvasu:
					return EType.ICat;
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.PoorvaPhalguni:
					return EType.IRat;
				case Nakshatra.Name.UttaraPhalguni:
				case Nakshatra.Name.UttaraBhadra:
					return EType.ICow;
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Hasta:
					return EType.IBuffalo;
				case Nakshatra.Name.Vishaka:
				case Nakshatra.Name.Chittra:
					return EType.ITiger;
				case Nakshatra.Name.Jyestha:
				case Nakshatra.Name.Anuradha:
					return EType.IHare;
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.Sravana:
					return EType.IMonkey;
				case Nakshatra.Name.PoorvaBhadra:
				case Nakshatra.Name.Dhanishta:
					return EType.ILion;
				case Nakshatra.Name.UttaraShada:
					return EType.IMongoose;

			}



			Debug.Assert(false, "KutaNakshatraYoni::getType");
			return EType.IHorse;
		}
	}


}
