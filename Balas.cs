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
	/// Summary description for Balas.
	/// </summary>
	public class ShadBalas
	{
		Horoscope h;
		public ShadBalas(Horoscope _h)
		{
			h = _h;
		}
		private void verifyGraha (Body.Name b)
		{
			int _b = (int)b;
			Debug.Assert(_b >= (int)Body.Name.Sun && _b <= (int)Body.Name.Saturn);
		}
		public double ucchaBala (Body.Name b)
		{
			this.verifyGraha(b);
			Longitude debLon = Body.debilitationDegree (b);
			Longitude posLon = h.getPosition(b).longitude;
			double diff = posLon.sub(debLon).value;
			if (diff > 180) diff = 360 - diff;
			return ((diff / 180.0) * 60.0);
		}
		public bool getsOjaBala (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Moon:
				case Body.Name.Venus:
					return false;
				default:
					return true;
			}
		}
		public double ojaYugmaHelper (Body.Name b, ZodiacHouse zh)
		{
			if (this.getsOjaBala(b))
			{
				if (zh.isOdd())	return 15.0;
				else return 0.0;
			}
			else 
			{
				if (zh.isOdd()) return 0.0;
				else return 15.0;
			}
		}
		public double ojaYugmaRasyAmsaBala (Body.Name b)
		{
			this.verifyGraha(b);
			BodyPosition bp = h.getPosition(b);
			ZodiacHouse zh_rasi = bp.toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;
			ZodiacHouse zh_amsa = bp.toDivisionPosition(new Division(Basics.DivisionType.Navamsa)).zodiac_house;
			double s = 0;
			s += this.ojaYugmaHelper(b, zh_rasi);
			s += this.ojaYugmaHelper(b, zh_amsa);
			return s;
		}
		public double kendraBala (Body.Name b)
		{
			this.verifyGraha(b);
			ZodiacHouse zh_b = h.getPosition(b).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;
			ZodiacHouse zh_l = h.getPosition(Body.Name.Lagna).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;
			int diff = zh_l.numHousesBetween(zh_b);
			switch (diff % 3)
			{
				case 1: return 60;
				case 2: return 30.0;
				case 0: default:
						return 15.0;
			}
		}
		public double drekkanaBala (Body.Name b)
		{
			this.verifyGraha(b);
			int part = h.getPosition(b).partOfZodiacHouse(3);
			if (part == 1 &&
				(b == Body.Name.Sun || b == Body.Name.Jupiter || b == Body.Name.Mars))
				return 15.0;

			if (part == 2 && 
				(b == Body.Name.Saturn || b == Body.Name.Mercury))
				return 15.0;

			if (part == 3 &&
				(b == Body.Name.Moon || b == Body.Name.Venus))
				return 15.0;

			return 0;
		}
		public double digBala (Body.Name b)
		{
			this.verifyGraha(b);
			int[] powerlessHouse = new int[] { 4, 10, 4, 7, 7, 10, 1 };
			Longitude lagLon = h.getPosition(Body.Name.Lagna).longitude;
			Longitude debLon = new Longitude(lagLon.toZodiacHouseBase());
			debLon = debLon.add(powerlessHouse[(int)b]*30.0+15.0);
			Longitude posLon = h.getPosition(b).longitude;

			Console.WriteLine("digBala {0} {1} {2}", b, posLon.value, debLon.value);

			double diff = posLon.sub(debLon).value;
			if (diff > 180) diff = 360 - diff;
			return ((diff / 180.0) * 60.0);
		}
		public double nathonnathaBala (Body.Name b)
		{
			this.verifyGraha(b);

			if (b == Body.Name.Mercury) return 60;

			double lmt_midnight = h.lmt_offset * 24.0;
			double lmt_noon = 12.0 + h.lmt_offset * 24.0;
			double diff = 0;
			if (h.info.tob.time  > lmt_noon)
				diff = lmt_midnight - h.info.tob.time;
			else
				diff = h.info.tob.time - lmt_midnight;

			while (diff < 0) diff += 12.0;
			diff = diff / 12.0 * 60.0;

			if (b == Body.Name.Moon || b == Body.Name.Mars ||
				b == Body.Name.Saturn)
				diff = 60-diff;

			return diff;
		}
		public double pakshaBala (Body.Name b)
		{
			this.verifyGraha(b);

			Longitude mlon = h.getPosition(Body.Name.Moon).longitude;
			Longitude slon = h.getPosition(Body.Name.Sun).longitude;

			double diff = mlon.sub(slon).value;
			if (diff > 180) diff = 360.0 - diff;
			double shubha = diff / 3.0;
			double paapa = 60.0 - shubha;

			switch (b)
			{
				case Body.Name.Sun:
				case Body.Name.Mars:
				case Body.Name.Saturn:
					return paapa;
				case Body.Name.Moon:
					return shubha *2.0;
				default:
				case Body.Name.Mercury:
				case Body.Name.Jupiter:
				case Body.Name.Venus:
					return shubha;
			}
		}
		public double tribhaagaBala (Body.Name b)
		{
			Body.Name ret = Body.Name.Jupiter;
			this.verifyGraha(b);
			if (h.isDayBirth())
			{
				double length = (h.sunset - h.sunrise)/3;
				double offset = h.info.tob.time - h.sunrise;
				int part = (int)(Math.Floor(offset/length));
				switch (part)
				{
					case 0: ret = Body.Name.Mercury; break;
					case 1: ret = Body.Name.Sun; break;
					case 2: ret = Body.Name.Saturn; break;
				}
			}
			else
			{
				double length = (h.next_sunrise + 24.0 - h.sunset)/3;
				double offset = h.info.tob.time - h.sunset;
				if (offset < 0) offset += 24;
				int part = (int)(Math.Floor(offset/length));
				switch (part)
				{
					case 0: ret = Body.Name.Moon; break;
					case 1: ret = Body.Name.Venus; break;
					case 2: ret = Body.Name.Mars; break;
				}
			}
			if (b == Body.Name.Jupiter || b == ret)
				return 60;
			return 0;
		}
		public double naisargikaBala (Body.Name b)
		{
			this.verifyGraha(b);
			switch (b)
			{
				case Body.Name.Sun: return 60;
				case Body.Name.Moon: return 51.43;
				case Body.Name.Mars: return 17.14;
				case Body.Name.Mercury: return 25.70;
				case Body.Name.Jupiter: return 34.28;
				case Body.Name.Venus: return 42.85;
				case Body.Name.Saturn: return 8.57;
			}
			return 0;
		}
		public void kalaHelper (ref Body.Name yearLord, ref Body.Name monthLord)
		{
			double ut_arghana = sweph.swe_julday(1827, 5, 2, - h.info.tz.toDouble() + 12.0/24.0);
			double ut_noon = h.baseUT - h.info.tob.time/24.0 + 12.0/24.0;

			double diff = ut_noon - ut_arghana;
			if (diff >= 0)
			{
				double quo = Math.Floor(diff/360.0);
				diff -= quo * 360.0;
			}
			else 
			{
				double pdiff = -diff;
				double quo = Math.Ceiling(pdiff/360.0);
				diff += quo * 360.0;
			}

			double diff_year = diff;
			while (diff > 30.0) diff -= 30.0;
			double diff_month = diff;
			while (diff > 7) diff -= 7.0;

			yearLord = Basics.weekdayRuler((Basics.Weekday)sweph.swe_day_of_week(ut_noon - diff_year));
			monthLord = Basics.weekdayRuler((Basics.Weekday)sweph.swe_day_of_week(ut_noon - diff_month));
		}
		public double abdaBala (Body.Name b)
		{
			this.verifyGraha(b);
			Body.Name yearLord=Body.Name.Sun, monthLord=Body.Name.Sun;
			this.kalaHelper(ref yearLord, ref monthLord);
			if (yearLord == b) return 15.0;
			return 0.0;
		}
		public double masaBala (Body.Name b)
		{
			this.verifyGraha(b);
			Body.Name yearLord=Body.Name.Sun, monthLord=Body.Name.Sun;
			this.kalaHelper(ref yearLord, ref monthLord);
			if (monthLord == b) return 30.0;
			return 0.0;
		}
		public double varaBala (Body.Name b)
		{
			this.verifyGraha(b);
			if (Basics.weekdayRuler(h.wday) == b) return 45.0;
			return 0.0;
		}
		public double horaBala (Body.Name b)
		{
			this.verifyGraha(b);
			if (h.calculateHora() == b) return 60.0;
			return 0.0;
		}
	}
}
