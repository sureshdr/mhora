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
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Globalization;
namespace mhora
{

	public class EqualStrength: Exception
	{
	}

	public interface IStrengthNakshatra
	{
		bool stronger (Nakshatra m, Nakshatra n);
	}

	public interface IStrengthGraha
	{
		bool stronger (Body.Name m, Body.Name n);
	}

	public interface IStrengthRasi
	{
		bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb);
	}
	
	abstract public class BaseStrength
	{
		protected ArrayList std_grahas;
		protected Division dtype;
		protected ArrayList std_div_pos;
		protected Horoscope h;
		protected bool bUseSimpleLords;

		protected Body.Name GetStrengthLord (ZodiacHouse.Name zh)
		{
			if (bUseSimpleLords)
				return Basics.SimpleLordOfZodiacHouse(zh);
			else
				return h.LordOfZodiacHouse(new ZodiacHouse(zh), dtype);
		}
		protected Body.Name GetStrengthLord (ZodiacHouse zh)
		{
			return GetStrengthLord (zh.value);
		}
		protected BaseStrength (Horoscope _h, Division _dtype, bool _bUseSimpleLords)
		{
			h = _h;
			dtype = _dtype;
			bUseSimpleLords = _bUseSimpleLords;
			std_div_pos = h.CalculateDivisionPositions (dtype);
		}
		protected int numGrahasInZodiacHouse (ZodiacHouse.Name zh) 
		{
			int num = 0;
			foreach (DivisionPosition dp in std_div_pos)
			{
				if (dp.type != BodyType.Name.Graha)
					continue;
				if (dp.zodiac_house.value == zh) 
					num = num + 1;
			}
			return num;
		}

		protected double karakaLongitude (Body.Name b)
		{
			double lon = h.getPosition(b).longitude.toZodiacHouseOffset();
			if (b == Body.Name.Rahu || b == Body.Name.Ketu)
				lon = 30.0 - lon;
			return lon;
		}
		protected Body.Name findAtmaKaraka () 
		{
			Body.Name[] karakaBodies =
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
				Body.Name.Jupiter, Body.Name.Venus, Body.Name.Saturn, Body.Name.Rahu
			};
			double lon = 0.0;
			Body.Name ret = Body.Name.Sun;
			foreach (Body.Name bn in karakaBodies) 
			{
				double offset = karakaLongitude (bn);
				if (offset > lon) lon = offset;
				ret = bn;
			}
			return ret;
		}

		public ArrayList findGrahasInHouse (ZodiacHouse.Name zh)
		{
			ArrayList ret = new ArrayList();
			foreach (DivisionPosition dp in std_div_pos) 
			{
				if (dp.type == BodyType.Name.Graha && 
					dp.zodiac_house.value == zh)
				{
					ret.Add (dp.name);
				}
			}
			return ret;
		}
	}

	
	// Stronger rasi has larger number of grahas
	// Stronger graha is in such a rasi
	public class StrengthByConjunction : BaseStrength, IStrengthRasi, IStrengthGraha
	{		
		public StrengthByConjunction (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int numa = this.numGrahasInZodiacHouse (za);
			int numb = this.numGrahasInZodiacHouse (zb);
			if (numa > numb) return true;
			if (numb > numa) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			return stronger (
				h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value,
				h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value
				);
		}
	} 

	
	// Stronger rasi has larger number of grahas in kendras
	// Stronger graha is in such a rasi
	public class StrengthByKendraConjunction: BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByKendraConjunction (Horoscope h, Division dtype)
				: base (h, dtype, true) {}

		public int value (ZodiacHouse.Name _zh)
		{
			int[] kendras = new int[4] {1, 4, 7, 10};
			int numGrahas=0;
			ZodiacHouse zh = new ZodiacHouse(_zh);
			foreach (int i in kendras)
			{
				numGrahas += this.numGrahasInZodiacHouse(zh.add(i).value);
			}
			return numGrahas;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int numa = value (za);
			int numb = value (zb);
			if (numa > numb) return true;
			if (numb > numa) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			return stronger (
				h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value,
				h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value
				);
		}
	}


	// Stronger rasi is the first one
	// Stronger graha is the first one
	public class StrengthByFirst: BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByFirst (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			return true;
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			return true;
		}
	}



	// Stronger rasi has larger number of exalted planets - debilitated planets
	// Stronger planet is exalted or not debilitated
	public class StrengthByExaltation : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByExaltation (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public int value (ZodiacHouse.Name zn) 
		{
			int ret = 0;
			foreach (DivisionPosition dp in std_div_pos)
			{
				if (dp.type != BodyType.Name.Graha) continue;
				if (dp.zodiac_house.value != zn)continue;

				if (dp.isExaltedPhalita()) ret++;
				else if (dp.isDebilitatedPhalita()) ret--;
			}
			return ret;
		}
		public int value (Body.Name b)
		{
			if (h.getPosition(b).toDivisionPosition(dtype).isExaltedPhalita()) return 1;
			else if (h.getPosition(b).toDivisionPosition(dtype).isDebilitatedPhalita()) return -1;
			return 0;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int vala = value (za);
			int valb = value (zb);

			if (vala > valb) return true;
			if (valb > vala) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			int valm = value (m);
			int valn = value (n);

			if (valm > valn) return true;
			if (valn > valm) return false;
			throw new EqualStrength();
		}
	}


	// Stronger rasi has more planets in moola trikona
	// Stronger planet is in moola trikona rasi
	public class StrengthByMoolaTrikona : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByMoolaTrikona (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public int value (ZodiacHouse.Name zn) 
		{
			int ret = 0;
			foreach (DivisionPosition dp in std_div_pos)
			{
				if (dp.type != BodyType.Name.Graha) continue;
				if (dp.zodiac_house.value != zn)continue;
				ret += value (dp.name);
			}
			return ret;
		}
		public int value (Body.Name b)
		{
			if (h.getPosition(b).toDivisionPosition(dtype).isInMoolaTrikona()) return 1;
			return 0;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int vala = value (za);
			int valb = value (zb);

			if (vala > valb) return true;
			if (valb > vala) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			int valm = value (m);
			int valn = value (n);

			if (valm > valn) return true;
			if (valn > valm) return false;
			throw new EqualStrength();
		}
	}


	// Stronger rasi has more planets in own house
	// Stronger planet is in own house
	public class StrengthByOwnHouse : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByOwnHouse (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public int value (ZodiacHouse.Name zn) 
		{
			int ret = 0;
			foreach (DivisionPosition dp in std_div_pos)
			{
				if (dp.type != BodyType.Name.Graha) continue;
				if (dp.zodiac_house.value != zn)continue;
				ret += value (dp.name);
			}
			return ret;
		}
		public int value (Body.Name b)
		{
			if (h.getPosition(b).toDivisionPosition(dtype).isInOwnHouse()) return 1;
			return 0;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int vala = value (za);
			int valb = value (zb);

			if (vala > valb) return true;
			if (valb > vala) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			int valm = value (m);
			int valn = value (n);

			if (valm > valn) return true;
			if (valn > valm) return false;
			throw new EqualStrength();
		}
	}



	// Stronger rasi has a graha which has traversed larger longitude
	// Stronger graha has traversed larger longitude in its house
	public class StrengthByLongitude : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByLongitude (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			Body.Name[] karakaBodies =
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
				Body.Name.Jupiter, Body.Name.Venus, Body.Name.Saturn, Body.Name.Rahu
			};

			double lona = 0.0, lonb = 0.0;
			foreach (Body.Name bn in karakaBodies) 
			{
				DivisionPosition div = h.getPosition(bn).toDivisionPosition(new Division(Basics.DivisionType.Rasi));
				double offset = karakaLongitude (bn);
				if (div.zodiac_house.value == za && offset > lona)
					lona = offset;
				else if (div.zodiac_house.value == zb && offset > lonb)
					lonb = offset;
			}
			if (lona > lonb) return true;
			if (lonb > lona) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			double lonm = karakaLongitude (m);
			double lonn = karakaLongitude (n);
			if (lonm > lonn) return true;
			if (lonn > lonm) return false;
			throw new EqualStrength();
		}
	}


	// Stronger rasi contains AK
	// Stronger graha is AK
	public class StrengthByAtmaKaraka : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByAtmaKaraka (Horoscope h, Division dtype)
			: base (h, dtype, true) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			ArrayList ala = findGrahasInHouse (za);
			ArrayList alb = findGrahasInHouse (zb);
			Body.Name ak = findAtmaKaraka();
			foreach (Body.Name ba in ala) 
			{
				if (ba == ak) return true;
			}
			foreach (Body.Name bb in alb)
			{
				if (bb == ak) return false;
			}
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			Body.Name ak = findAtmaKaraka();
			if (m == ak) return true;
			if (n == ak) return false;
			throw new EqualStrength();
		}
	}


	// Stronger rasi's lord has traversed larger longitude
	public class StrengthByLordsLongitude : BaseStrength, IStrengthRasi
	{
		public StrengthByLordsLongitude (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			Body.Name lora = this.GetStrengthLord(za);
			Body.Name lorb = this.GetStrengthLord(zb);
			double offa = this.karakaLongitude(lora);
			double offb = this.karakaLongitude(lorb);
			if (offa > offb) return true;
			if (offb > offa) return false;
			throw new EqualStrength();
		}
	}


	// Stronger rasi's lord is AK
	public class StrengthByLordIsAtmaKaraka : BaseStrength, IStrengthRasi
	{
		public StrengthByLordIsAtmaKaraka (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			Body.Name lora = this.GetStrengthLord(za); 
			Body.Name lorb = this.GetStrengthLord(zb);
			Body.Name ak = findAtmaKaraka();
			if (lora == ak) return true;
			if (lorb == ak) return false;
			throw new EqualStrength();
		}
	}

	// Stronger rasi's lord by nature (moveable, fixed, dual)
	// Stronger graha's dispositor in such a rasi
	public class StrengthByLordsNature : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByLordsNature (Horoscope h, Division dtype)
			: base (h, dtype, true) {}
		public int naturalValueForRasi (ZodiacHouse.Name zha)
		{
			Body.Name bl = h.LordOfZodiacHouse(zha, dtype);
			ZodiacHouse.Name zhl = h.getPosition(bl).toDivisionPosition(dtype).zodiac_house.value;

			int[] vals = new int[] {3,1,2}; // dual, move, fix
			return vals[(int)zhl % 3];
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int[] vals = new int[] {3,1,2}; // dual, move, fix
			int a = this.naturalValueForRasi(za);
			int b = this.naturalValueForRasi(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			ZodiacHouse.Name za = h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value;
			ZodiacHouse.Name zb = h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value;
			return stronger (za, zb);
		}
	}

	// Stronger rasi by nature (moveable, fixed, dual)
	// Stronger graha in such a rasi
	public class StrengthByRasisNature : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByRasisNature (Horoscope h, Division dtype)
			: base (h, dtype, true) {}
		public int naturalValueForRasi (ZodiacHouse.Name zha)
		{
			int[] vals = new int[] {3,1,2}; // dual, move, fix
			return vals[(int)zha % 3];
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int[] vals = new int[] {3,1,2}; // dual, move, fix
			int a = this.naturalValueForRasi(za);
			int b = this.naturalValueForRasi(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			ZodiacHouse.Name za = h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value;
			ZodiacHouse.Name zb = h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value;
			return stronger (za, zb);
		}
	}


	// Stronger rasi has its lord in a house of different oddity
	// Stronger graha in such a rasi
	public class StrengthByLordInDifferentOddity : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByLordInDifferentOddity (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		protected int oddityValueForZodiacHouse (ZodiacHouse.Name zh)
		{
			Body.Name lname = this.GetStrengthLord(zh);
			BodyPosition lbpos = h.getPosition(lname);
			DivisionPosition ldpos = h.CalculateDivisionPosition(lbpos, dtype);
			ZodiacHouse zh_lor = ldpos.zodiac_house;

			//System.Console.WriteLine("   DiffOddity {0} {1} {2}", zh.ToString(), zh_lor.value.ToString(), (int)zh %2==(int)zh_lor.value%2);
			if ((int)zh % 2 == (int)zh_lor.value % 2)
				return 0;

			return 1;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int a = this.oddityValueForZodiacHouse(za);
			int b = this.oddityValueForZodiacHouse(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name ba, Body.Name bb)
		{
			ZodiacHouse.Name za = h.getPosition(ba).toDivisionPosition(dtype).zodiac_house.value;
			ZodiacHouse.Name zb = h.getPosition(bb).toDivisionPosition(dtype).zodiac_house.value;
			return stronger (za, zb);
		}
	}


	// Stronger rasi has more conjunctions/rasi drishtis of Jupiter, Mercury and Lord
	// Stronger graha is in such a rasi
	public class StrengthByAspectsRasi : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByAspectsRasi (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		protected int value (ZodiacHouse zj, ZodiacHouse zm, ZodiacHouse.Name zx)
		{
			int ret = 0;
			ZodiacHouse zh = new ZodiacHouse(zx);

			Body.Name bl = this.GetStrengthLord(zx);
			ZodiacHouse zl = h.getPosition(bl).toDivisionPosition(dtype).zodiac_house;

			if (zj.RasiDristi(zh) || zj.value == zh.value) ret++;
			if (zm.RasiDristi(zh) || zm.value == zh.value) ret++;
			if (zl.RasiDristi(zh) || zl.value == zh.value) ret++;
			return ret;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			ZodiacHouse zj = h.getPosition(Body.Name.Jupiter).toDivisionPosition(dtype).zodiac_house;
			ZodiacHouse zm = h.getPosition(Body.Name.Mercury).toDivisionPosition(dtype).zodiac_house;

			int a = this.value(zj, zm, za);
			int b = this.value(zj, zm, zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			ZodiacHouse.Name zm = h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value;
			ZodiacHouse.Name zn = h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value;
			return stronger (zm, zn);
		}
	}


	// Stronger rasi has its Lord in its house
	// Stronger graha is in its own house
	public class StrengthByLordInOwnHouse : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByLordInOwnHouse (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		protected int value (ZodiacHouse.Name _zh)
		{
			int ret=0;

			ZodiacHouse zh = new ZodiacHouse(_zh);
			Body.Name bl = this.GetStrengthLord(zh);
			DivisionPosition pl = h.getPosition(bl).toDivisionPosition(dtype);
			DivisionPosition pj = h.getPosition(Body.Name.Jupiter).toDivisionPosition(dtype);
			DivisionPosition pm = h.getPosition(Body.Name.Mercury).toDivisionPosition(dtype);

			if (pl.GrahaDristi(zh)) ret++;
			if (pj.GrahaDristi(zh)) ret++;
			if (pm.GrahaDristi(zh)) ret++;
			return ret;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int a = this.value(za);
			int b = this.value(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			ZodiacHouse.Name zm = h.getPosition(m).toDivisionPosition(dtype).zodiac_house.value;
			ZodiacHouse.Name zn = h.getPosition(n).toDivisionPosition(dtype).zodiac_house.value;
			return stronger (zm, zn);
		}
	}

	public class StrengthByVimsottariDasaLength : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByVimsottariDasaLength (Horoscope h, Division dtype)
			: base (h, dtype, false) {}
		protected double value (ZodiacHouse.Name zh)
		{
			double length = 0;
			foreach (BodyPosition bp in h.positionList)
			{
				if (bp.type == BodyType.Name.Graha)
					length = Math.Max(length, VimsottariDasa.LengthOfDasa(bp.name));
			}
			return length;
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb)
		{
			double a = value (za);
			double b = value (zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			double a = VimsottariDasa.LengthOfDasa(m);
			double b = VimsottariDasa.LengthOfDasa(n);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
	}

	// Stronger graha has longer length
	// Stronger rasi has a graha with longer length placed therein
	public class StrengthByKarakaKendradiGrahaDasaLength : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByKarakaKendradiGrahaDasaLength (Horoscope h, Division dtype)
			: base (h, dtype, false) { }
		protected double value (ZodiacHouse.Name zh)
		{
			double length = 0;
			foreach (BodyPosition bp in h.positionList)
			{
				if (bp.type == BodyType.Name.Graha)
				{
					DivisionPosition dp = bp.toDivisionPosition(dtype);
					length = Math.Max(length, KarakaKendradiGrahaDasa.LengthOfDasa(h, dtype, bp.name, dp));
				}
			}
			return length;
		}
		protected double value (Body.Name b)
		{
			DivisionPosition dp = h.getPosition(b).toDivisionPosition(dtype);
			return KarakaKendradiGrahaDasa.LengthOfDasa(h, dtype, b, dp);
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb)
		{
			double a = value(za);
			double b = value(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			double a = value (m);
			double b = value (n);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
	}

	// Stronger rasi has a larger narayana dasa length
	// Stronger graha is in such a rasi
	public class StrengthByNarayanaDasaLength : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByNarayanaDasaLength (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}
		protected int value (ZodiacHouse.Name _zh)
		{
			Body.Name bl = this.GetStrengthLord(_zh);
			DivisionPosition pl = h.getPosition(bl).toDivisionPosition(dtype);
			return NarayanaDasa.NarayanaDasaLength  (new ZodiacHouse(_zh), pl);
		}
		protected int value (Body.Name bm)
		{
			ZodiacHouse.Name zm = h.getPosition(bm).toDivisionPosition(dtype).zodiac_house.value;
			return value(zm);
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int a = this.value(za);
			int b = this.value(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			int a = this.value(m);
			int b = this.value(n);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();

		}

	}


	// Stronger rasi has more graha drishtis of Jupiter, Mercury and Lord
	// Stronger graha is in such a rasi
	public class StrengthByAspectsGraha : BaseStrength, IStrengthRasi, IStrengthGraha
	{
		public StrengthByAspectsGraha (Horoscope h, Division dtype, bool bSimpleLord)
			: base (h, dtype, bSimpleLord) {}

		protected int value (ZodiacHouse.Name _zh)
		{
			int val = 0;
			Body.Name bl = this.GetStrengthLord(_zh);
			DivisionPosition dl = h.getPosition(bl).toDivisionPosition(dtype);
			DivisionPosition dj = h.getPosition(Body.Name.Jupiter).toDivisionPosition(dtype);
			DivisionPosition dm = h.getPosition(Body.Name.Mercury).toDivisionPosition(dtype);

			ZodiacHouse zh = new ZodiacHouse(_zh);
			if (dl.GrahaDristi(zh) || dl.zodiac_house.value == _zh) val++;
			if (dj.GrahaDristi(zh) || dj.zodiac_house.value == _zh) val++;
			if (dm.GrahaDristi(zh) || dm.zodiac_house.value == _zh) val++;

			return val;
		}
		protected int value (Body.Name bm)
		{
			return value (h.getPosition(bm).toDivisionPosition(dtype).zodiac_house.value);
		}
		public bool stronger (ZodiacHouse.Name za, ZodiacHouse.Name zb) 
		{
			int a = this.value(za);
			int b = this.value(zb);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();
		}
		public bool stronger (Body.Name m, Body.Name n)
		{
			int a = this.value(m);
			int b = this.value(n);
			if (a > b) return true;
			if (a < b) return false;
			throw new EqualStrength();

		}
	}



	public class FindStronger
	{
		Horoscope h;
		Division dtype;
		ArrayList rules;
		bool bUseSimpleLords;


		public FindStronger (Horoscope _h, Division _dtype, ArrayList _rules, bool _UseSimpleLords) 
		{
			h = _h;
			dtype = _dtype;
			rules = _rules;
			bUseSimpleLords = _UseSimpleLords;

		}
		public FindStronger (Horoscope _h, Division _dtype, ArrayList _rules) 
		{
			h = _h;
			dtype = _dtype;
			rules = _rules;
			bUseSimpleLords = false;
		}

		static private StrengthOptions GetStrengthOptions (Horoscope h)
		{
			if (h.strength_options == null)
				return MhoraGlobalOptions.Instance.SOptions;
			else
				return h.strength_options;
		}
		static public ArrayList RulesNaisargikaDasaRasi (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).NaisargikaDasaRasi);
		}
		static public ArrayList RulesNarayanaDasaRasi (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).NarayanaDasaRasi);
		}
		static public ArrayList RulesKarakaKendradiGrahaDasaRasi (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).KarakaKendradiGrahaDasaRasi);
		}
		static public ArrayList RulesKarakaKendradiGrahaDasaGraha (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).KarakaKendradiGrahaDasaGraha);
		}
		static public ArrayList RulesKarakaKendradiGrahaDasaColord (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).KarakaKendradiGrahaDasaColord);
		}
		static public ArrayList RulesMoolaDasaRasi (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).MoolaDasaRasi);
		}
		static public ArrayList RulesNavamsaDasaRasi (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).NavamsaDasaRasi);
		}
		static public ArrayList RulesJaiminiFirstRasi (Horoscope h)
		{
			ArrayList Rules = new ArrayList();
			Rules.Add(ERasiStrength.AtmaKaraka);
			Rules.Add(ERasiStrength.Conjunction);
			Rules.Add(ERasiStrength.Exaltation);
			Rules.Add(ERasiStrength.MoolaTrikona);
			Rules.Add(ERasiStrength.OwnHouse);
			Rules.Add(ERasiStrength.RasisNature);
			Rules.Add(ERasiStrength.LordIsAtmaKaraka);
			Rules.Add(ERasiStrength.LordsLongitude);
			Rules.Add(ERasiStrength.LordInDifferentOddity);
			return Rules;
		}
		static public ArrayList RulesJaiminiSecondRasi (Horoscope h)
		{
			ArrayList Rules = new ArrayList();
			Rules.Add(ERasiStrength.AspectsRasi);
			return Rules;
		}

		static public ArrayList RulesNaisargikaDasaGraha (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).NaisargikaDasaGraha);
		}
		static public ArrayList RulesVimsottariGraha (Horoscope h)
		{
			ArrayList Rules = new ArrayList();
			Rules.Add(EGrahaStrength.KendraConjunction);
			Rules.Add(EGrahaStrength.First);
			return Rules;
		}

		static public ArrayList RulesStrongerCoLord (Horoscope h)
		{
			return new ArrayList(FindStronger.GetStrengthOptions(h).Colord);
		}

		public OrderedZodiacHouses[] ResultsZodiacKendras (ZodiacHouse.Name _zh)
		{
			OrderedZodiacHouses[] zRet = new OrderedZodiacHouses[3];
			ZodiacHouse zh = new ZodiacHouse(_zh);
			ZodiacHouse.Name[] zh1 = new ZodiacHouse.Name[4] { zh.add(1).value, zh.add(4).value, zh.add(7).value, zh.add(10).value };
			ZodiacHouse.Name[] zh2 = new ZodiacHouse.Name[4] { zh.add(2).value, zh.add(5).value, zh.add(8).value, zh.add(11).value };
			ZodiacHouse.Name[] zh3 = new ZodiacHouse.Name[4] { zh.add(3).value, zh.add(6).value, zh.add(9).value, zh.add(12).value };
			zRet[0] = this.getOrderedHouses(zh1);
			zRet[1] = this.getOrderedHouses(zh2);
			zRet[2] = this.getOrderedHouses(zh3);
			return zRet;
		}
		public ZodiacHouse.Name [] ResultsKendraRasis(ZodiacHouse.Name _zh)
		{
			ZodiacHouse.Name[] zRet = new ZodiacHouse.Name[12];
			ZodiacHouse zh = new ZodiacHouse(_zh);
			ZodiacHouse.Name[] zh1 = new ZodiacHouse.Name[4] { zh.add(1).value, zh.add(4).value, zh.add(7).value, zh.add(10).value };
			ZodiacHouse.Name[] zh2 = new ZodiacHouse.Name[4] { zh.add(2).value, zh.add(5).value, zh.add(8).value, zh.add(11).value };
			ZodiacHouse.Name[] zh3 = new ZodiacHouse.Name[4] { zh.add(3).value, zh.add(6).value, zh.add(9).value, zh.add(12).value };
			getOrderedRasis(zh1).CopyTo(zRet, 0);
			getOrderedRasis(zh2).CopyTo(zRet, 4);
			getOrderedRasis(zh3).CopyTo(zRet, 8);
			return zRet;
		}
		public ZodiacHouse.Name[] ResultsFirstSeventhRasis ()
		{
			ZodiacHouse.Name[] zRet = new ZodiacHouse.Name[12];
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Ari, ZodiacHouse.Name.Lib}).CopyTo(zRet, 0);
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Tau, ZodiacHouse.Name.Sco}).CopyTo(zRet, 2);
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Gem, ZodiacHouse.Name.Sag}).CopyTo(zRet, 4);
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Can, ZodiacHouse.Name.Cap}).CopyTo(zRet, 6);
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Leo, ZodiacHouse.Name.Aqu}).CopyTo(zRet, 8);
			getOrderedRasis(new ZodiacHouse.Name[]{ZodiacHouse.Name.Vir, ZodiacHouse.Name.Pis}).CopyTo(zRet, 10);
			return zRet;
		}


		public Body.Name[] getOrderedGrahas ()
		{
			Body.Name[] grahas = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
				Body.Name.Jupiter, Body.Name.Venus, Body.Name.Saturn,
				Body.Name.Rahu, Body.Name.Ketu
			};
			return getOrderedGrahas (grahas);
		}
		public Body.Name[] getOrderedGrahas (Body.Name[] grahas)
		{
			if (grahas.Length <= 1)
				return grahas;

			for (int i=0; i<grahas.Length-1; i++)
			{
				for (int j=0; j<grahas.Length-1; j++) 
				{
					if (false == this.CmpGraha(grahas[j], grahas[j+1], false))
					{
						Body.Name temp = grahas[j];
						grahas[j] = grahas[j+1];
						grahas[j+1] = temp;
					}
				}
			}
			return grahas;
		}

		public ZodiacHouse.Name[] getOrderedRasis ()
		{
			ZodiacHouse.Name[] rasis = new ZodiacHouse.Name[] 
			{
				ZodiacHouse.Name.Ari, ZodiacHouse.Name.Tau, ZodiacHouse.Name.Gem,
				ZodiacHouse.Name.Can, ZodiacHouse.Name.Leo, ZodiacHouse.Name.Vir,
				ZodiacHouse.Name.Lib, ZodiacHouse.Name.Sco, ZodiacHouse.Name.Sag,
				ZodiacHouse.Name.Cap, ZodiacHouse.Name.Aqu, ZodiacHouse.Name.Pis
			};
			return getOrderedRasis (rasis);
		}
		public OrderedZodiacHouses getOrderedHouses (ZodiacHouse.Name[] rasis)
		{
			ZodiacHouse.Name[] zh_ordered = getOrderedRasis(rasis);
			OrderedZodiacHouses oz = new OrderedZodiacHouses();
			foreach (ZodiacHouse.Name zn in zh_ordered)
				oz.houses.Add(zn);
			return oz;
		}
		public ZodiacHouse.Name[] getOrderedRasis (ZodiacHouse.Name[] rasis)
		{
			if (rasis.Length < 2) return rasis;
	
			int length = rasis.Length;
			for (int i=0; i<length; i++) 
			{
				for (int j=0; j<length-1; j++) 
				{
					//System.Console.WriteLine ("Comparing {0} and {1}", i, j);
					if (false == this.CmpRasi(rasis[j], rasis[j+1], false)) 
					{
						ZodiacHouse.Name temp = rasis[j];
						rasis[j] = rasis[j+1];
						rasis[j+1] = temp;
					}
				}
			}
			return rasis;
		}

		public ZodiacHouse.Name StrongerRasi (ZodiacHouse.Name za, ZodiacHouse.Name zb, bool bSimpleLord, ref int winner) 
		{
			if (CmpRasi (za, zb, bSimpleLord, ref winner)) return za;
			return zb;
		}

		public ZodiacHouse.Name StrongerRasi (ZodiacHouse.Name za, ZodiacHouse.Name zb, bool bSimpleLord) 
		{
			int winner = 0;
			return StrongerRasi(za, zb, bSimpleLord, ref winner);
		}
		public Body.Name StrongerGraha (Body.Name m, Body.Name n, bool bSimpleLord)
		{
			int winner = 0;
			return StrongerGraha (m, n, bSimpleLord, ref winner);
		}
		public Body.Name StrongerGraha (Body.Name m, Body.Name n, bool bSimpleLord, ref int winner)
		{
			if (CmpGraha (m, n, bSimpleLord, ref winner)) return m;
			return n;
		}
		public ZodiacHouse.Name WeakerRasi (ZodiacHouse.Name za, ZodiacHouse.Name zb, bool bSimpleLord) 
		{
			if (CmpRasi (za, zb, bSimpleLord)) return zb;
			return za;
		}
		public Body.Name WeakerGraha (Body.Name m, Body.Name n, bool bSimpleLord)
		{
			if (CmpGraha (m, n, bSimpleLord)) return n;
			return m;
		}
		public bool CmpRasi (ZodiacHouse.Name za, ZodiacHouse.Name zb, bool bSimpleLord)
		{
			int winner = 0;
			return CmpRasi (za, zb, bSimpleLord, ref winner);
		}

		// Maintain numerical values for forward compatibility
		[TypeConverter(typeof(EnumDescConverter))]
		public enum ERasiStrength
		{
			[Description("Giving up: Arbitrarily choosing one")]				First,
			[Description("Rasi has more grahas in it")]							Conjunction, 
			[Description("Rasi contains more exalted grahas")]					Exaltation, 
			[Description("Rasi has a graha with higher longitude offset")]		Longitude,
			[Description("Rasi contains Atma Karaka")]							AtmaKaraka, 
			[Description("Rasi's lord is Atma Karaka")]							LordIsAtmaKaraka,
			[Description("Rasi is stronger by nature")]							RasisNature, 
			[Description("Rasi has more rasi drishtis of lord, Mer, Jup")]		AspectsRasi,
			[Description("Rasi has more graha drishtis of lord, Mer, Jup")]		AspectsGraha, 
			[Description("Rasi's lord is in a rasi of different oddity")]		LordInDifferentOddity, 
			[Description("Rasi's lord has a higher longitude offset")]			LordsLongitude,
			[Description("Rasi has longer narayana dasa length")]				NarayanaDasaLength, 
			[Description("Rasi has a graha in moolatrikona")]					MoolaTrikona, 
			[Description("Rasi's lord is place there")]							OwnHouse,
			[Description("Rasi has more grahas in kendras")]					KendraConjunction,
			[Description("Rasi's dispositor is stronger by nature")]			LordsNature,
			[Description("Rasi has a graha with longer karaka kendradi graha dasa length")] KarakaKendradiGrahaDasaLength,
			[Description("Rasi has a graha with longer vimsottari dasa length")]			VimsottariDasaLength
		};

		public bool CmpRasi (ZodiacHouse.Name za, ZodiacHouse.Name zb, bool bSimpleLord, ref int winner)
		{
			bool bRet = false;
			bool bFound = true;
			winner = 0;

			//System.Console.WriteLine("Rasi: {0} {1}", za.ToString(), zb.ToString());
			foreach (ERasiStrength s in rules)
			{
				//System.Console.WriteLine("Rasi::{0}", s);
				switch (s)
				{
				case ERasiStrength.Conjunction:
						try 
						{ 
							bRet = new StrengthByConjunction (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.Exaltation:
						try 
						{ 
							bRet = new StrengthByExaltation (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.Longitude:
						try 
						{ 
							bRet = new StrengthByLongitude (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.AtmaKaraka:
						try 
						{ 
							bRet = new StrengthByAtmaKaraka (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.LordIsAtmaKaraka:
						try
						{
							bRet = new StrengthByLordIsAtmaKaraka(h, dtype, bSimpleLord).stronger(za,zb);
							goto found;
						}
						catch {winner++;} break;
					case ERasiStrength.RasisNature:
						try 
						{ 
							bRet = new StrengthByRasisNature (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.LordsNature:
						try 
						{ 
							bRet = new StrengthByLordsNature (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.AspectsRasi:
						try 
						{ 
							bRet = new StrengthByAspectsRasi (h, dtype, bSimpleLord).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.AspectsGraha:
						try 
						{ 
							bRet = new StrengthByAspectsGraha (h, dtype, bSimpleLord).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.LordInDifferentOddity:
						try 
						{ 
							bRet = new StrengthByLordInDifferentOddity (h, dtype, bSimpleLord).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.LordsLongitude:
						try 
						{ 
							bRet = new StrengthByLordsLongitude (h, dtype, bSimpleLord).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.NarayanaDasaLength:
						try 
						{ 
							bRet = new StrengthByNarayanaDasaLength (h, dtype, bSimpleLord).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.VimsottariDasaLength:
						try 
						{ 
							bRet = new StrengthByVimsottariDasaLength (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;
					case ERasiStrength.MoolaTrikona:
						try 
						{ 
							bRet = new StrengthByMoolaTrikona (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.OwnHouse:
						try 
						{ 
							bRet = new StrengthByOwnHouse (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.KendraConjunction:
						try 
						{ 
							bRet = new StrengthByKendraConjunction (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					case ERasiStrength.KarakaKendradiGrahaDasaLength:
						try
						{
							bRet = new StrengthByKarakaKendradiGrahaDasaLength(h, dtype).stronger(za, zb);
							goto found;
						}
						catch {winner++;} break;
					case ERasiStrength.First:
						try 
						{ 
							bRet = new StrengthByFirst (h, dtype).stronger(za, zb); 
							goto found;
						}
						catch {winner++;} break;	
					default:
						throw new Exception("Unknown Rasi Strength Rule");
				}
			}
			bFound = false;

			found:
				if (bFound == true) return bRet;
			return true;

		}

		// Maintain numerical values for forward compatibility
		[TypeConverter(typeof(EnumDescConverter))]
		public enum EGrahaStrength
		{
			[Description("Giving up: Arbitrarily choosing one")]					First,
			[Description("Graha is conjunct more grahas")]							Conjunction, 
			[Description("Graha is exalted")]										Exaltation, 
			[Description("Graha has higher longitude offset")]						Longitude, 
			[Description("Graha is Atma Karaka")]									AtmaKaraka, 
			[Description("Graha is in a rasi with stronger nature")]				RasisNature, 
			[Description("Graha has more rasi drishti of dispositor, Jup, Mer")]	AspectsRasi,
			[Description("Graha has more graha drishti of dispositor, Jup, Mer")]	AspectsGraha, 
			[Description("Graha has a larger narayana dasa length")]				NarayanaDasaLength, 
			[Description("Graha is in its moola trikona rasi")]						MoolaTrikona, 
			[Description("Graha is in own house")]									OwnHouse,
			[Description("Graha is not in own house")]								NotInOwnHouse,
			[Description("Graha's dispositor is in own house")]						LordInOwnHouse,
			[Description("Graha has more grahas in kendras")]						KendraConjunction,
			[Description("Graha's dispositor is in a rasi with stronger nature")]	LordsNature,
			[Description("Graha's dispositor is in a rasi with different oddify")]	LordInDifferentOddity,
			[Description("Graha has a larger Karaka Kendradi Graha Dasa length")]   KarakaKendradiGrahaDasaLength,
			[Description("Graha has a larger Vimsottari Dasa length")]				VimsottariDasaLength
		}

		public bool CmpGraha (Body.Name m, Body.Name n, bool bSimpleLord)
		{
			int winner=0;
			return CmpGraha (m, n, bSimpleLord, ref winner);
		}
		public bool CmpGraha (Body.Name m, Body.Name n, bool bSimpleLord, ref int winner)
		{
			bool bRet = false;
			bool bFound = true;
			winner = 0;
			foreach (EGrahaStrength s in rules) 
			{
				//Console.WriteLine("Trying {0}. Curr is {1}", s, winner);
				switch (s) 
				{
					case EGrahaStrength.Conjunction:
						try { 
							bRet = new StrengthByConjunction (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.Exaltation:
						try 
						{ 
							bRet = new StrengthByExaltation (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.Longitude:
						try 
						{ 
							bRet = new StrengthByLongitude (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.AtmaKaraka:
						try 
						{ 
							bRet = new StrengthByAtmaKaraka (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.RasisNature:
						try 
						{ 
							bRet = new StrengthByRasisNature (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.LordsNature:
						try 
						{ 
							bRet = new StrengthByLordsNature (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.AspectsRasi:
						try 
						{ 
							bRet = new StrengthByAspectsRasi (h, dtype, bSimpleLord).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.AspectsGraha:
						try 
						{ 
							bRet = new StrengthByAspectsGraha (h, dtype, bSimpleLord).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.NarayanaDasaLength:
						try 
						{ 
							bRet = new StrengthByNarayanaDasaLength (h, dtype, bSimpleLord).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.VimsottariDasaLength:
						try 
						{ 
							bRet = new StrengthByVimsottariDasaLength (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.MoolaTrikona:
						try 
						{ 
							bRet = new StrengthByMoolaTrikona (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.OwnHouse:
						try 
						{ 
							bRet = new StrengthByOwnHouse (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.NotInOwnHouse:
						try 
						{ 
							bRet = !(new StrengthByOwnHouse (h, dtype).stronger(m, n)); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.LordInOwnHouse:
						try
						{
							bRet = new StrengthByLordInOwnHouse (h, dtype, bSimpleLord).stronger(m, n);
							goto found;
						}
						catch {winner++;} break;
					case EGrahaStrength.LordInDifferentOddity:
						try
						{
							bRet = new StrengthByLordInDifferentOddity (h, dtype, bSimpleLord).stronger(m, n);
							goto found;
						}
						catch {winner++;} break;
					case EGrahaStrength.KendraConjunction:
						try 
						{ 
							bRet = new StrengthByKendraConjunction (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					case EGrahaStrength.KarakaKendradiGrahaDasaLength:
						try
						{
							bRet = new StrengthByKarakaKendradiGrahaDasaLength (h, dtype).stronger(m, n);
							goto found;
						}
						catch {winner++;} break;
					case EGrahaStrength.First:
						try 
						{ 
							bRet = new StrengthByFirst (h, dtype).stronger(m, n); 
							goto found;
						}
						catch {winner++;} break;	
					default:
						throw new Exception("Unknown Graha Strength Rule");
				}			
			}
			bFound = false;

			found:
				if (bFound == true) 
				{
					return bRet;
				}
				winner++;
				return true;
		}
	}

}
	
