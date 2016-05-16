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
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace mhora
{
	
	public class ToDate
	{
		public enum DateType :int
		{
			FixedYear, SolarYear, TithiYear, YogaYear,
			TithiPraveshYear, KaranaPraveshYear, YogaPraveshYear, NakshatraPraveshYear
		}
		private double baseUT;
		private double yearLength;
		private double compression;
		private double offset;
		private double spos;
		private double mpos;
		private DateType type;
		Horoscope h;

		public ToDate (double _baseUT, double _yearLength, double _compression, Horoscope _h)
		{
			baseUT = _baseUT;
			yearLength = _yearLength;
			type = DateType.FixedYear;
			compression = _compression;
			h = _h;
		}
		public ToDate(double _baseUT, DateType _type, double _yearLength, double _compression, Horoscope _h) 
		{
			baseUT = _baseUT;
			type = _type;
			yearLength = _yearLength;
			compression = _compression;
			h = _h;
			spos = h.getPosition(Body.Name.Sun).longitude.value;
			mpos = h.getPosition(Body.Name.Moon).longitude.value;
		}
		public void SetOffset (double _offset)
		{
			this.offset = _offset;
		}


		public Moment AddPraveshYears (double years, 
			mhora.ReturnLon returnLonFunc, int numMonths, int numDays)
		{
			double jd=0.0;
			int year=0, month=0, day=0;
			int hour=0, minute=0, second=0;
			double dhour=0, lon=0;
			double soff =0;
			double _years = 0;
			double tYears=0;
			double tMonths=0;
			double tDays=0;
			double jd_st=0;
			bool bDiscard=true;
			Transit t=null;
			Longitude l = null;

			Debug.Assert(years >= 0, "pravesh years only work in the future");
			t = new Transit (h);
			soff = h.getPosition(Body.Name.Sun).longitude.toZodiacHouseOffset();
			_years = years;
			tYears=0;
			tMonths=0;
			tDays=0;
			tYears = Math.Floor(_years);
			_years = (_years - Math.Floor(_years))*numMonths;
			tMonths = Math.Floor(_years);
			_years = (_years - Math.Floor(_years))*numDays;
			tDays = _years;
					
			//Console.WriteLine ("Searching for {0} {1} {2}", tYears, tMonths, tDays);
			lon = spos - soff;
			l = new Longitude(lon);
			jd = t.LinearSearch(h.baseUT + (tYears*365.2425), l, new ReturnLon(t.LongitudeOfSun));
			double yoga_start = returnLonFunc(jd, ref bDiscard).value;
			double yoga_end = returnLonFunc(h.baseUT, ref bDiscard).value;
			jd_st = jd + (yoga_end-yoga_start)/360.0*28.0;
			if (yoga_end < yoga_start) jd_st += 28.0;
			l = new Longitude(yoga_end);
			jd = t.LinearSearch(jd_st, new Longitude(yoga_end), returnLonFunc);
			for (int i=1; (double)i <= tMonths; i++)
			{
				jd = t.LinearSearch(jd+30.0, new Longitude(yoga_end), returnLonFunc);
			}

			l = l.add(new Longitude(tDays * (360.0/(double)numDays)));
			jd_st = jd + tDays; // * 25.0/30.0;
			jd = t.LinearSearch(jd_st, l, returnLonFunc);
			jd += (h.info.tz.toDouble() / 24.0);
			jd += offset;
					
			sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
			Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
			return new Moment (year, month, day, hour, minute, second);								
		}

		public Moment AddYears (double years)
		{
			double jd=0.0;
			int year=0, month=0, day=0;
			int hour=0, minute=0, second=0;
			double dhour=0, lon=0;
			double new_baseut=0.0;
			Transit t=null;
			Longitude l = null;
			if (compression > 0.0)
				years *= compression;
			switch (type) 
			{
				case DateType.FixedYear:
					//Console.WriteLine("Finding {0} fixed years of length {1}", years, yearLength);
					jd = baseUT + (years * yearLength);
					//Console.WriteLine("tz = {0}", (h.info.tz.toDouble()) / 24.0);
					jd += offset;
					sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
					Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
					return new Moment (year, month, day, hour, minute, second);				
				case DateType.SolarYear:
					// Turn into years of 360 degrees, and then search
					years = years * yearLength / 360.0;
					t = new Transit (h);
					if (years >= 0) lon = (years - Math.Floor(years)) * 360.0;
					else lon = (years - Math.Ceiling(years)) * 360.0;
					l = new Longitude(lon + spos);
					jd = t.LinearSearch(h.baseUT + years * 365.2425, l, new ReturnLon(t.LongitudeOfSun));
					jd += (h.info.tz.toDouble() / 24.0);
					jd += offset;
					sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
					Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
					return new Moment (year, month, day, hour, minute, second);									
				case ToDate.DateType.TithiPraveshYear:
					t = new Transit(h);
					return this.AddPraveshYears(years, new ReturnLon(t.LongitudeOfTithiDir), 13, 30);
				case ToDate.DateType.KaranaPraveshYear:
					t = new Transit(h);
					return this.AddPraveshYears(years, new ReturnLon(t.LongitudeOfTithiDir), 13, 60);
				case DateType.YogaPraveshYear:
					t = new Transit(h);
					return this.AddPraveshYears(years, new ReturnLon(t.LongitudeOfSunMoonYogaDir), 15, 27);
				case DateType.NakshatraPraveshYear:
					t = new Transit(h);
					return this.AddPraveshYears(years, new ReturnLon(t.LongitudeOfMoonDir), 13, 27);
				case DateType.TithiYear:
					jd -= (h.info.tz.toDouble()/24.0);
					t = new Transit(h);
					jd = h.baseUT;
					Longitude tithi_base = new Longitude(mpos-spos);
					double days = years * yearLength;
					//Console.WriteLine("Find {0} tithi days", days);
					while (days >= 30*12.0)
					{
						jd = t.LinearSearch(jd+29.52916*12.0, tithi_base, new ReturnLon(t.LongitudeOfTithiDir));
						days -=30*12.0;
					}
					tithi_base = tithi_base.add (new Longitude(days*12.0));
					//Console.WriteLine ("Searching from {0} for {1}", t.LongitudeOfTithiDir(jd+days*28.0/30.0), tithi_base);
					jd = t.LinearSearch(jd+days*28.0/30.0, tithi_base, new ReturnLon(t.LongitudeOfTithiDir));
					jd += (h.info.tz.toDouble() / 24.0);
					jd += offset;
					sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
					Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
					return new Moment (year, month, day, hour, minute, second);		
				case DateType.YogaYear:
					jd -= (h.info.tz.toDouble()/24.0);
					t = new Transit(h);
					jd = h.baseUT;
					Longitude yoga_base = new Longitude(mpos+spos);
					double yogaDays = years * yearLength;
					//Console.WriteLine ("Find {0} yoga days", yogaDays);
					while (yogaDays >= 27*12)
					{
						jd = t.LinearSearch(jd+305, yoga_base, new ReturnLon(t.LongitudeOfSunMoonYogaDir));
						yogaDays -= 27*12;
					}
					yoga_base = yoga_base.add(new Longitude(yogaDays*(360.0/27.0)));
					jd = t.LinearSearch(jd+yogaDays*28.0/30.0, yoga_base, new ReturnLon(t.LongitudeOfSunMoonYogaDir));
					jd += (h.info.tz.toDouble() / 24.0);
					jd += offset;
					sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
					Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
					return new Moment (year, month, day, hour, minute, second);		
				default:
					//years = years * yearLength;
					t = new Transit (h);
					if (years >= 0) lon = (years - Math.Floor(years)) * 4320;
					else lon = (years - Math.Ceiling(years)) * 4320;
					lon *= (yearLength/360.0);
					new_baseut = h.baseUT;
					Longitude tithi = t.LongitudeOfTithi (new_baseut);
					l = tithi.add (new Longitude (lon));
					//Console.WriteLine("{0} {1} {2}", 354.35, 354.35*yearLength/360.0, yearLength);
					double tyear_approx = 354.35*yearLength/360.0; /*357.93765*/
					double lapp = t.LongitudeOfTithi(new_baseut + (years* tyear_approx)).value;
					jd = t.LinearSearch(new_baseut + (years* tyear_approx), l, new ReturnLon(t.LongitudeOfTithiDir));
					jd += offset;
					//jd += (h.info.tz.toDouble() / 24.0);
					sweph.swe_revjul(jd, ref year, ref month, ref day, ref dhour);
					Moment.doubleToHMS (dhour, ref hour, ref minute, ref second);
					return new Moment (year, month, day, hour, minute, second);								

			}
		}
	}

	internal class DasaEntryConverter: ExpandableObjectConverter 
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t) 
		{
			if (t == typeof(string)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, t);
		}
		public override object ConvertFrom(
			ITypeDescriptorContext context, 
			CultureInfo info,
			object value) 
		{
			Trace.Assert (value is string, "DasaEntryConverter::ConvertFrom 1");
			string s = (string) value;

			DasaEntry de = new DasaEntry(Body.Name.Lagna, 0.0, 0.0, 1, "None");
			string[] arr = s.Split (new Char[1] {','});
			if (arr.Length >= 1) de.shortDesc = arr[0];
			if (arr.Length >= 2) de.level = int.Parse(arr[1]);
			if (arr.Length >= 3) de.startUT = double.Parse(arr[2]);
			if (arr.Length >= 4) de.dasaLength = double.Parse(arr[3]);
			if (arr.Length >= 5) de.graha = (Body.Name)int.Parse(arr[4]);
			if (arr.Length >= 6) de.zodiacHouse = (ZodiacHouse.Name)int.Parse(arr[5]);
			return de;
		}

		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Trace.Assert (destType == typeof(string) && value is DasaEntry, "DasaItem::ConvertTo 1");
			DasaEntry de = (DasaEntry)value;
			return ( de.shortDesc.ToString() + "," +
				de.level.ToString() + "," +
				de.startUT.ToString() + "," +
				de.dasaLength.ToString() + "," +
				(int)de.graha + "," +
				(int)de.zodiacHouse);
		}   
	}

	[TypeConverter(typeof(DasaEntryConverter))]
	public class DasaEntry
	{
		public Body.Name graha;
		public ZodiacHouse.Name zodiacHouse;
		public double startUT;
		public double dasaLength; // 1 year = 360 days = 360 degrees is used internally!!!!
		public int level;
		public string shortDesc;

		private void Construct (double _startUT, double _dasaLength, int _level, string _shortDesc)
		{
			startUT = _startUT;
			dasaLength = _dasaLength;
			level = _level;
			shortDesc = _shortDesc;
		}
		public DasaEntry (Body.Name _graha, double _startUT, double _dasaLength, int _level, string _shortDesc)
		{
			graha = _graha;
			Construct (_startUT, _dasaLength, _level, _shortDesc);
		}
		public DasaEntry (ZodiacHouse.Name _zodiacHouse, double _startUT, double _dasaLength, int _level, string _shortDesc)
		{
			zodiacHouse = _zodiacHouse;
			Construct (_startUT, _dasaLength, _level, _shortDesc);
		}
		public DasaEntry ()
		{
			startUT = dasaLength = 0.0;
			level = 1;
			shortDesc = "Jup";
			graha = Body.Name.Jupiter;
			zodiacHouse = ZodiacHouse.Name.Ari;
		}
		public string DasaName 
		{
			get { return shortDesc; }
			set { shortDesc = value; }
		}
		public int DasaLevel
		{
			get { return level; }
			set { level = value; }
		}
		public double StartUT
		{
			get { return startUT; }
			set { startUT = value; }
		}
		public double DasaLength
		{
			get { return dasaLength; }
			set { dasaLength = value; }
		}
		public Body.Name Graha
		{
			get { return graha; }
			set { graha = value; }
		}
		public ZodiacHouse.Name ZHouse
		{
			get { return zodiacHouse; }
			set { zodiacHouse = value; }
		}
	}

	/// <summary>
	/// Interface implemented by all IDasa functions. At the moment the method of
	/// implementation for any level below AntarDasa is assumed to be the same.
	/// </summary>
	public interface IDasa: IUpdateable
	{
		double paramAyus();
		ArrayList Dasa(int cycle);
		ArrayList AntarDasa (DasaEntry pdi);
		string EntryDescription (DasaEntry de, Moment start, Moment end);
		String Description ();
		void DivisionChanged (Division d);
		void recalculateOptions ();
	}


	/// <summary>
	/// A helper interface for all Vimsottari/Ashtottari Dasa like dasas
	/// </summary>
	public interface INakshatraDasa: IDasa
	{
		int numberOfDasaItems ();                 // Number of dasas for 1 cycle
		DasaEntry nextDasaLord (DasaEntry di);      // Order of Dasas
		double lengthOfDasa (Body.Name plt);      // Length of a maha dasa
		Body.Name lordOfNakshatra(Nakshatra n);   // Dasa lord of given nakshatra
	}

	public interface INakshatraTithiDasa: IDasa
	{
		Body.Name lordOfTithi (Longitude l);
		double lengthOfDasa (Body.Name plt);      // Length of a maha dasa
	}

	public interface INakshatraYogaDasa: IDasa
	{
		Body.Name lordOfYoga (Longitude l);
		double lengthOfDasa (Body.Name plt);
	}

	public interface INakshatraKaranaDasa : IDasa
	{
		Body.Name lordOfKarana (Longitude l);
		double lengthOfDasa (Body.Name plt);
	}

	abstract public class Dasa
	{
		static public int NarayanaDasaLength (ZodiacHouse zh, DivisionPosition dp) 
		{
			int length=0;

			if (zh.isOddFooted())
				length = zh.numHousesBetween(dp.zodiac_house);
			else
				length = zh.numHousesBetweenReverse(dp.zodiac_house);
		
			if (dp.isExaltedPhalita())
				length++;
			else if (dp.isDebilitatedPhalita())
				length--;

			length = Basics.normalize_inc(1, 12, length-1);
			return length;
		}



		public event EvtChanged Changed;
		public void DivisionChanged(Division d)
		{
		}
		public void OnChanged ()
		{
			if (Changed != null)
				Changed(this);
		}
		public Recalculate RecalculateEvent;

		public class Options : ICloneable
		{
			private ToDate.DateType DateType;
			private double _YearLength;
			private double _Compression;
			private double _OffsetDays;
			private double _OffsetHours;
			private double _OffsetMins;
			public Options () 
			{
				DateType = ToDate.DateType.SolarYear;
				_YearLength = 360.0;
				_Compression = 0.0;
			}
			[PGDisplayName("Year Type")]
			public ToDate.DateType YearType
			{
				get { return DateType; }
				set { DateType = value; }
			}

			[PGDisplayName("Dasa Compression")]
			public double Compression
			{
				get { return _Compression; }
				set 
				{
					if (value >= 0.0)
						_Compression = value; 
				}
			}
			[PGDisplayName("Year Length")]
			public double YearLength
			{
				get { return _YearLength; }
				set 
				{ 
					if (value >= 0.0)
						_YearLength = value; 
				}
			}
			[PGDisplayName("Offset Dates by Days")]
			public double OffsetDays
			{
				get { return this._OffsetDays; }
				set { this._OffsetDays = value; }
			}
			[PGDisplayName("Offset Dates by Hours")]
			public double OffsetHours
			{
				get { return _OffsetHours; }
				set { _OffsetHours = value; }
			}			
			[PGDisplayName("Offset Dates by Minutes")]
			public double OffsetMinutes
			{
				get { return _OffsetMins; }
				set { _OffsetMins = value; }
			}
			public object Clone ()
			{
				Options o = new Options();
				o.YearLength = YearLength;
				o.DateType = DateType;
				o.Compression = Compression;
				o.OffsetDays = OffsetDays;
				o.OffsetHours = OffsetHours;
				o.OffsetMinutes = OffsetMinutes;
				return o;
			}
			public void Copy (Dasa.Options o)
			{
				this.YearLength = o.YearLength;
				this.DateType = o.DateType;
				this.Compression = o.Compression;
				this.OffsetDays = o.OffsetDays;
				this.OffsetHours = o.OffsetHours;
				this.OffsetMinutes = o.OffsetMinutes;
			}
		}

		public string EntryDescription (DasaEntry de, Moment start, Moment end)
		{
			return "";
		}
	}


	public class KalachakraDasa: Dasa, IDasa
	{
		private Horoscope h;

		private ZodiacHouse[] mzhSavya = new ZodiacHouse[24];
		private ZodiacHouse[] mzhApasavya = new ZodiacHouse[24];

		public enum GroupType
		{
			Savya, SavyaMirrored, Apasavya, ApasavyaMirrored
		}
		public GroupType NakshatraToGroup (Nakshatra n)
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini:
				case Nakshatra.Name.Krittika:
				case Nakshatra.Name.Punarvasu:
				case Nakshatra.Name.Aslesha:
				case Nakshatra.Name.Hasta:
				case Nakshatra.Name.Swati:
				case Nakshatra.Name.Moola:
				case Nakshatra.Name.UttaraShada:
				case Nakshatra.Name.PoorvaBhadra:
					return GroupType.Savya;
				case Nakshatra.Name.Bharani:
				case Nakshatra.Name.Pushya:
				case Nakshatra.Name.Chittra:
				case Nakshatra.Name.PoorvaShada:
				case Nakshatra.Name.Revati:
					return GroupType.SavyaMirrored;
				case Nakshatra.Name.Rohini:
				case Nakshatra.Name.Makha:
				case Nakshatra.Name.Vishaka:
				case Nakshatra.Name.Sravana:
					return GroupType.Apasavya;
				default:
					return GroupType.ApasavyaMirrored;
			}
			switch ((int)n.value % 6)
			{
				case 1:	return GroupType.Savya;
				case 2: return GroupType.SavyaMirrored;
				case 3: return GroupType.Savya;
				case 4: return GroupType.Apasavya;
				case 5: return GroupType.ApasavyaMirrored;
				default: return GroupType.ApasavyaMirrored;
			}
		}
		private void initHelper (Longitude lon, ref ZodiacHouse[] mzhOrder, ref int offset)
		{
			GroupType grp = this.NakshatraToGroup(lon.toNakshatra());
			int pada = lon.toNakshatraPada();

			switch (grp)
			{
				case GroupType.Savya: 
				case GroupType.SavyaMirrored:
					mzhOrder = mzhSavya; 
					break;
				default:
					mzhOrder = mzhApasavya;
					break;
			}

			switch (grp)
			{
				case GroupType.Savya:
				case GroupType.Apasavya:
					offset = 0;
					break;
				default:
					offset = 12;
					break;
			}
			offset = (int)Basics.normalize_exc_lower(0, 24, ((pada-1)*9)+offset);
		}
		public KalachakraDasa (Horoscope _h)
		{
			h = _h;

			ZodiacHouse zAri = new ZodiacHouse(ZodiacHouse.Name.Ari);
			ZodiacHouse zSag = new ZodiacHouse(ZodiacHouse.Name.Sag);
			for (int i=0; i<12; i++)
			{
				mzhSavya[i] = zAri.add(i+1);
				mzhSavya[i+12] = mzhSavya[i].LordsOtherSign();
				mzhApasavya[i] = zSag.add(i+1);
				mzhApasavya[i+12] = mzhApasavya[i].LordsOtherSign();
			}
		}
		public double paramAyus () 
		{
			return 144;
		}
		public double DasaLength (ZodiacHouse zh)
		{
			switch (zh.value)
			{
				case ZodiacHouse.Name.Ari:
				case ZodiacHouse.Name.Sco:
					return 7;
				case ZodiacHouse.Name.Tau:
				case ZodiacHouse.Name.Lib:
					return 16;
				case ZodiacHouse.Name.Gem:
				case ZodiacHouse.Name.Vir:
					return 9;
				case ZodiacHouse.Name.Can:
					return 21;
				case ZodiacHouse.Name.Leo:
					return 5;
				case ZodiacHouse.Name.Sag:
				case ZodiacHouse.Name.Pis:
					return 10;
				case ZodiacHouse.Name.Cap:
				case ZodiacHouse.Name.Aqu:
					return 4;
				default:
					throw new Exception("KalachakraDasa::DasaLength");
			}
		}
		public ArrayList Dasa(int cycle)
		{
			Division dRasi = new Division(Basics.DivisionType.Rasi);
			Longitude mLon = h.getPosition(Body.Name.Moon).extrapolateLongitude(dRasi);

			int offset = 0;
			ZodiacHouse[] zhOrder = null;
			this.initHelper(mLon, ref zhOrder, ref offset);

			ArrayList al = new ArrayList();

			double dasa_length_sum = 0;
			for (int i=0; i<9; i++)
			{
				ZodiacHouse zhCurr = zhOrder[(int)Basics.normalize_exc_lower(0,24,offset+i)];
				double dasa_length = this.DasaLength(zhCurr);
				DasaEntry de = new DasaEntry(zhCurr.value, dasa_length_sum, dasa_length, 1, zhCurr.value.ToString());
				al.Add(de);
				dasa_length_sum += dasa_length;
			}

			double offsetLength = mLon.toNakshatraPadaPercentage() / 100.0 * dasa_length_sum;
	
			foreach (DasaEntry de in al)
			{
				de.startUT -= offsetLength;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			return new ArrayList();
		}
		public string Description ()
		{
			return "Kalachakra Dasa";
		}
		public object GetOptions ()
		{
			return new object();
		}
		public object SetOptions (object o)
		{
			return o;
		}
		public void recalculateOptions ()
		{
		}
	}

	public class NarayanaSamaDasa: NarayanaDasa, IDasa
	{
		public NarayanaSamaDasa (Horoscope _h) :base (_h)
		{
			this.bSama = true;
		}
		new public String Description ()
		{
			return "Narayana Sama Dasa for "
				+ options.Division.ToString() 
				+ " seeded from " + options.SeedRasi.ToString();
		}
	}

	public class NarayanaDasa: Dasa, IDasa
	{
		private Horoscope h;
		public bool bSama;
		public RasiDasaUserOptions options;
		public NarayanaDasa (Horoscope _h)
		{
			h = _h;
			this.bSama = false;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public void recalculateOptions()
		{
			options.recalculate();
		}
		public double paramAyus () 
		{
			return 144;
		}
		Body.Name GetLord (ZodiacHouse zh)
		{
			switch (zh.value)
			{
				case ZodiacHouse.Name.Aqu:
					return options.ColordAqu;
				case ZodiacHouse.Name.Sco:
					return options.ColordSco;
				default:
					return Basics.SimpleLordOfZodiacHouse(zh.value);
			}
		}

		public int DasaLength (ZodiacHouse zh, DivisionPosition dp) 
		{
			if (this.bSama)
				return 12;

			return NarayanaDasa.NarayanaDasaLength(zh, dp);
		}
		public ArrayList Dasa(int cycle)
		{
			int[] order_moveable = new int[] { 1,2,3,4,5,6,7,8,9,10,11,12 };
			int[] order_fixed = new int[] { 1,6,11,4,9,2,7,12,5,10,3,8 };
			int[] order_dual = new int[] { 1,5,9,10,2,6,7,11,3,4,8,12 };

			ArrayList al = new ArrayList (24);
			bool backward = true;

			int[] order;
			switch ((int)options.SeedRasi % 3)
			{
				case 1: order = order_moveable; break;
				case 2: order = order_fixed; break;
				default: order = order_dual; break;
			}
			ZodiacHouse zh_seed = options.getSeed();
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, zh_seed.value, zh_seed.add(7).value);

			if (zh_seed.add(9).isOddFooted() == true)
				backward = false;

			if (options.saturnExceptionApplies(zh_seed.value))
			{
				order = order_moveable;
				backward = false;
			}
			else if (options.ketuExceptionApplies(zh_seed.value))
			{
				backward = !backward;
			}

			double dasa_length_sum = 0.0;
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh_dasa;
				if (backward)
					zh_dasa = zh_seed.addReverse(order[i]);
				else
					zh_dasa = zh_seed.add (order[i]);

				Body.Name dasa_lord = this.GetLord(zh_dasa);
				//gs.strongerForNarayanaDasa(zh_dasa);
				DivisionPosition dlord_dpos = h.CalculateDivisionPosition(h.getPosition(dasa_lord), options.Division);
				double dasa_length = (double)this.DasaLength(zh_dasa, dlord_dpos);

				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			if (bSama == false)
			{
				for (int i=0; i<12; i++)
				{
					DasaEntry di = (DasaEntry)al[i];
					DasaEntry dn = new DasaEntry(di.zodiacHouse, dasa_length_sum, 12.0-di.dasaLength, 1, di.zodiacHouse.ToString());
					dasa_length_sum += dn.dasaLength;
					al.Add (dn);
				}
			}

			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList(12);

			ZodiacHouse zh_first = new ZodiacHouse(pdi.zodiacHouse);
			ZodiacHouse zh_stronger = zh_first.add(1);
			zh_stronger.value = options.findStrongerRasi(options.SeventhStrengths, zh_stronger.value, zh_stronger.add(7).value);

			Body.Name b = this.GetLord(zh_stronger);
			DivisionPosition dp = h.CalculateDivisionPosition(h.getPosition(b), options.Division);
			ZodiacHouse first = dp.zodiac_house;
			bool backward = false;
			if ((int)first.value % 2 == 0)
				backward = true;

			double dasa_start = pdi.startUT;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa;
				if (!backward)
					zh_dasa = first.add(i);
				else
					zh_dasa = first.addReverse(i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_start, pdi.dasaLength / 12.0, pdi.level+1, pdi.shortDesc + " " + zh_dasa.value.ToString());
				al.Add (di);
				dasa_start += pdi.dasaLength / 12.0;
			}
			return al;
		}
		public String Description ()
		{
			return "Narayana Dasa for "
				+ options.Division.ToString() 
				+ " seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			options.CopyFrom(a);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}


	public class RasiDasaUserOptions : ICloneable
	{
		protected Horoscope h;
		protected Division mDtype;
		protected Body.Name mCoLordAqu;
		protected Body.Name mCoLordSco;
		protected OrderedZodiacHouses[] mSeventhStrengths;
		protected OrderedZodiacHouses mSaturnExceptions;
		protected OrderedZodiacHouses mKetuExceptions;
		protected ZodiacHouse.Name mSeed;
		protected int mSeedHouse;
		protected ArrayList mRules;
		

		public void recalculate ()
		{
			this.calculateCoLords();
			this.calculateExceptions();
			this.calculateSeed();
			this.calculateSeventhStrengths();
		}
		public RasiDasaUserOptions (Horoscope _h, ArrayList _rules)
		{
			h = _h;
			mRules = _rules;
			mSeventhStrengths = new OrderedZodiacHouses[6];
			mSaturnExceptions = new OrderedZodiacHouses();
			mKetuExceptions = new OrderedZodiacHouses();
			this.mDtype = new Division(Basics.DivisionType.Rasi);

			this.calculateCoLords();
			this.calculateExceptions();
			this.calculateSeed();
			this.calculateSeventhStrengths();
		}
		[PGNotVisible]
		public Division Division
		{
			get { return this.mDtype; }
			set { this.mDtype = value; }
		}

		[PGDisplayName("Division")]
		public Basics.DivisionType UIVarga
		{
			get { return this.mDtype.MultipleDivisions[0].Varga; }
			set { this.mDtype = new Division(value); }
		}

		[PropertyOrder(99), PGDisplayName("Seed Rasi")]
		[Description("The rasi from which the dasa should be seeded.")]
		public ZodiacHouse.Name SeedRasi
		{
			get { return this.mSeed; }
			set { this.mSeed = value; }
		}
		[PropertyOrder(100), PGDisplayName("Seed House")]
		[Description("House from which dasa should be seeded (reckoned from seed rasi)")]
		public int SeedHouse
		{
			get { return this.mSeedHouse; }
			set { this.mSeedHouse = value; }
		}

		[PropertyOrder(101), PGDisplayName("Lord of Aquarius")]
		public Body.Name ColordAqu
		{
			get { return this.mCoLordAqu; }
			set { this.mCoLordAqu = value; }
		}
		[PropertyOrder(102), PGDisplayName("Lord of Scorpio")]
		public Body.Name ColordSco
		{
			get { return this.mCoLordSco; }
			set { this.mCoLordSco = value; }
		}

		[PropertyOrder(103), PGDisplayName("Rasi Strength Order")]
		public OrderedZodiacHouses[] SeventhStrengths
		{
			get { return this.mSeventhStrengths; }
			set { this.mSeventhStrengths = value; }
		}
		[PropertyOrder(104), PGDisplayName("Rasis with Saturn Exception")]
		public OrderedZodiacHouses SaturnExceptions
		{
			get { return this.mSaturnExceptions; }
			set { this.mSaturnExceptions = value; }
		}
		[PropertyOrder(105), PGDisplayName("Rasis with Ketu Exception")]
		public OrderedZodiacHouses KetuExceptions
		{
			get { return this.mKetuExceptions; }
			set { this.mKetuExceptions = value; }
		}
		virtual public object Clone ()
		{
			RasiDasaUserOptions uo = new RasiDasaUserOptions(h, mRules);
			uo.Division = (Division)this.Division.Clone();
			uo.ColordAqu = this.ColordAqu;
			uo.ColordSco = this.ColordSco;
			uo.mSeed = this.mSeed;
			uo.SeventhStrengths = this.SeventhStrengths;
			uo.KetuExceptions = this.KetuExceptions;
			uo.SaturnExceptions = this.SaturnExceptions;
			uo.SeedHouse = this.SeedHouse;
			return uo;
		}
		virtual public object CopyFrom (object _uo)
		{
			this.CopyFromNoClone(_uo);
			return this.Clone();
		}
		virtual public void CopyFromNoClone (object _uo)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)_uo;
			
			bool bDivisionChanged = false;
			bool bRecomputeChanged = false;

			if (this.Division != uo.Division)
				bDivisionChanged = true;
			if (this.ColordAqu != uo.ColordAqu ||
				this.ColordSco != uo.ColordSco)
				bRecomputeChanged = true;

			this.Division = (Division)uo.Division.Clone();
			this.ColordAqu = uo.ColordAqu;
			this.ColordSco = uo.ColordSco;
			this.mSeed = uo.mSeed;
			this.mSeedHouse = uo.mSeedHouse;
			for (int i=0; i<6; i++)
				this.SeventhStrengths[i] = (OrderedZodiacHouses)uo.SeventhStrengths[i].Clone();
			//this.SeventhStrengths = uo.SeventhStrengths.Clone();
			this.KetuExceptions = (OrderedZodiacHouses)uo.KetuExceptions.Clone();
			this.SaturnExceptions = (OrderedZodiacHouses)uo.SaturnExceptions.Clone();

			if (true == bDivisionChanged)
				this.calculateCoLords();

			if (true == bDivisionChanged || true == bRecomputeChanged)
			{
				this.calculateSeed();
				this.calculateSeventhStrengths();
				this.calculateExceptions();
			} 
		}
		public ZodiacHouse getSeed ()
		{
			return new ZodiacHouse(this.mSeed).add(this.SeedHouse);
		}
		public void calculateSeed ()
		{
			this.mSeed = h.getPosition(Body.Name.Lagna).toDivisionPosition(this.Division).zodiac_house.value;
			this.mSeedHouse = 1;

		}
		public void calculateCoLords ()
		{
			FindStronger fs = new FindStronger(h, mDtype, FindStronger.RulesStrongerCoLord(h));
			this.mCoLordAqu = fs.StrongerGraha(Body.Name.Saturn, Body.Name.Rahu, true);
			this.mCoLordSco = fs.StrongerGraha(Body.Name.Mars, Body.Name.Ketu, true);
		}
		public void calculateExceptions ()
		{
			this.KetuExceptions.houses.Clear();
			this.SaturnExceptions.houses.Clear();

			ZodiacHouse.Name zhKetu = h.getPosition(Body.Name.Ketu).toDivisionPosition(this.Division).zodiac_house.value;
			ZodiacHouse.Name zhSat = h.getPosition(Body.Name.Saturn).toDivisionPosition(this.Division).zodiac_house.value;

			if (zhKetu != zhSat)
			{
				this.mKetuExceptions.houses.Add (zhKetu);
				this.mSaturnExceptions.houses.Add (zhSat);
			} 
			else
			{
				ArrayList rule = new ArrayList();
				rule.Add (FindStronger.EGrahaStrength.Longitude);
				FindStronger fs = new FindStronger(h, this.Division, rule);
				Body.Name b = fs.StrongerGraha(Body.Name.Saturn, Body.Name.Ketu, false);
				if (b == Body.Name.Ketu)
					this.mKetuExceptions.houses.Add (zhKetu);
				else
					this.mSaturnExceptions.houses.Add (zhSat);
			}

		}
		public ZodiacHouse.Name findStrongerRasi (OrderedZodiacHouses[] mList, ZodiacHouse.Name za, ZodiacHouse.Name zb)
		{
			for (int i=0; i<mList.Length; i++)
			{
				for (int j=0; j<mList[i].houses.Count; j++)
				{
					if ((ZodiacHouse.Name)mList[i].houses[j] == za) return za;
					if ((ZodiacHouse.Name)mList[i].houses[j] == zb) return zb;
				}
			}
			return za;
		}
		public bool ketuExceptionApplies (ZodiacHouse.Name zh)
		{
			for (int i=0; i<this.mKetuExceptions.houses.Count; i++)
			{
				if ((ZodiacHouse.Name)this.mKetuExceptions.houses[i] == zh)
					return true;
			}
			return false;
		}
		public bool saturnExceptionApplies (ZodiacHouse.Name zh)
		{
			for (int i=0; i<this.mSaturnExceptions.houses.Count; i++)
			{
				if ((ZodiacHouse.Name)this.mSaturnExceptions.houses[i] == zh)
					return true;
			}
			return false;
		}
		public void calculateSeventhStrengths ()
		{	
			FindStronger fs = new FindStronger(h, mDtype, this.mRules);
			ZodiacHouse zAri = new ZodiacHouse(ZodiacHouse.Name.Ari);
			for (int i=0; i<6; i++)
			{
				this.mSeventhStrengths[i] = new OrderedZodiacHouses();
				ZodiacHouse za = zAri.add(i+1);
				ZodiacHouse zb = za.add(7);
				if (fs.CmpRasi(za.value, zb.value, false))
				{
					this.mSeventhStrengths[i].houses.Add(za.value);
					this.mSeventhStrengths[i].houses.Add(zb.value);
				}
				else
				{
					this.mSeventhStrengths[i].houses.Add(zb.value);
					this.mSeventhStrengths[i].houses.Add(za.value);
				}
			}

		}
	}



	public class TattwaDasa : Dasa, IDasa
	{
		public class UserOptions 
		{
			public enum Tattwa : int
			{ Bhoomi, Jala, Agni, Vayu, Akasha }

			public Tattwa _startTattwa;

			[PGDisplayName("Seed Tattwa")]
			public Tattwa StartTattwa 
			{
				get { return _startTattwa; }
				set { _startTattwa = value; }
			}

		}
		private Horoscope h;
		public TattwaDasa (Horoscope _h)
		{
			h = _h;
		}
		public double paramAyus()
		{
			return ((1.0 / 24.0) / 60.0);
		}
		public void recalculateOptions()
		{
		}
		public ArrayList Dasa (int cycle)
		{
			ArrayList al = new ArrayList();
			
			double day_length = h.next_sunrise + 24.0 - h.sunrise;
			double day_sr = Math.Floor(h.baseUT) + (h.sunrise / 24.0);

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi)
		{
			return new ArrayList();
		}
		public string Description ()
		{
			return "Tattwa Dasa";
		}
		public object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (object o)
		{
			return o;
		}
	}
	public class SudarshanaChakraDasa : Dasa, IDasa
	{
		private Horoscope h;
		public SudarshanaChakraDasa (Horoscope _h)
		{
			h = _h;
		}
		public double paramAyus ()
		{
			return 12;
		}
		public string Description ()
		{
			return "Sudarshana Chakra Dasa";
		}
		public object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (object o)
		{
			return o;
		}
		public void recalculateOptions ()
		{
		}
		public ArrayList Dasa (int cycle)
		{
			ArrayList al = new ArrayList(12);
			double start = cycle * paramAyus();
			ZodiacHouse lzh = h.getPosition(Body.Name.Lagna).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse czh = lzh.add(i);
				al.Add (new DasaEntry(czh.value, start, 1, 1, czh.value.ToString()));
				start += 1;
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry de)
		{
			ArrayList al = new ArrayList(12);
			double start = de.StartUT;
			double length = de.DasaLength / 12.0;
			ZodiacHouse zh = new ZodiacHouse(de.zodiacHouse);
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse czh = zh.add(i);
				al.Add (new DasaEntry(czh.value, start, length, de.level+1, 
					de.shortDesc + " " + czh.value.ToString()));
				start += length;
			}
			return al;
		}
	}

	public class SuDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
		public SuDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 144;
		}
		public void recalculateOptions()
		{
			options.recalculate();
		}
		Body.Name GetLord (ZodiacHouse zh)
		{
			switch (zh.value)
			{
				case ZodiacHouse.Name.Aqu:
					return options.ColordAqu;
				case ZodiacHouse.Name.Sco:
					return options.ColordSco;
				default:
					return Basics.SimpleLordOfZodiacHouse(zh.value);
			}
		}
		int[] order = new int[] { 0,1,4,7,10,2,5,8,11,3,6,9,12 };
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList();
			BodyPosition bp_sl = h.getPosition(mhora.Body.Name.SreeLagna);
			ZodiacHouse zh_seed = bp_sl.toDivisionPosition(options.Division).zodiac_house;
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, zh_seed.value, zh_seed.add(7).value);

			bool bIsForward = zh_seed.isOdd();

			double dasa_length_sum = 0.0;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = null;
				if (bIsForward)
					zh_dasa = zh_seed.add(order[i]);
				else
					zh_dasa = zh_seed.addReverse(order[i]);

				Body.Name bl = this.GetLord(zh_dasa);
				DivisionPosition dp = h.getPosition(bl).toDivisionPosition(options.Division);
				double dasa_length = NarayanaDasa.NarayanaDasaLength(zh_dasa, dp);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			for (int i=0; i<12; i++)
			{
				DasaEntry di_first = ((DasaEntry)al[i]);
				double dasa_length = 12.0 - di_first.dasaLength;
				DasaEntry di = new DasaEntry (di_first.zodiacHouse, dasa_length_sum, dasa_length, 1, di_first.zodiacHouse.ToString());
				al.Add(di);
				dasa_length_sum += dasa_length;
			}

			double cycle_length = (double)cycle * this.paramAyus();
			double offset_length = (bp_sl.longitude.toZodiacHouseOffset() / 30.0) *
				((DasaEntry)al[0]).dasaLength;

			//Console.WriteLine ("Completed {0}, going back {1} of {2} years", bp_sl.longitude.toZodiacHouseOffset() / 30.0,
			//	offset_length, ((DasaEntry)al[0]).dasaLength);

			cycle_length -= offset_length;

			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList();
			ZodiacHouse zh_seed = new ZodiacHouse(pdi.zodiacHouse);

			double dasa_length = pdi.dasaLength / 12.0;
			double dasa_length_sum = pdi.startUT;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = null;
				zh_dasa = zh_seed.addReverse(order[i]);

				DasaEntry di = new DasaEntry(zh_dasa.value, dasa_length_sum, dasa_length, pdi.level+1, 
					pdi.shortDesc + " " + zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}
			return al;
		}
		public String Description ()
		{
			return "Sudasa";
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
	}





	public class NirayaanaShoolaDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		public NirayaanaShoolaDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 96;
		}
		public void recalculateOptions()
		{
			options.recalculate();
		}
		public double getDasaLength (ZodiacHouse zh)
		{
			switch ((int)zh.value % 3)
			{
				case 1: return 7.0;
				case 2: return 8.0;
				case 0: return 9.0;
				default: throw new Exception();
			}
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList();
			ZodiacHouse zh_seed = options.getSeed().add(2);
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, 
				zh_seed.value, zh_seed.add(7).value);

			bool bIsForward = zh_seed.isOdd();

			double dasa_length_sum = 0.0;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = null;
				if (bIsForward)
					zh_dasa = zh_seed.add(i);
				else
					zh_dasa = zh_seed.addReverse(i);

				double dasa_length = this.getDasaLength(zh_dasa);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			NarayanaDasa nd = new NarayanaDasa(h);
			nd.options = this.options;
			return nd.AntarDasa(pdi);
		}
		public String Description ()
		{
			return "Niryaana Shoola Dasa"
				+ " seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}

	public class TrikonaDasa: Dasa, IDasa
	{
		class UserOptions : RasiDasaUserOptions
		{
			protected OrderedZodiacHouses mTrikonaStrengths;
			public OrderedZodiacHouses TrikonaStrengths
			{
				get { return this.mTrikonaStrengths; }
				set { this.mTrikonaStrengths = value; }
			}
			public UserOptions (Horoscope _h, ArrayList _rules) :
				base (_h, _rules)
			{
				this.calculateTrikonaStrengths();
			}
			private void calculateTrikonaStrengths ()
			{
				ZodiacHouse zh = this.getSeed();
				ZodiacHouse.Name[] zh_t = new ZodiacHouse.Name[3] { zh.add(1).value, zh.add(5).value, zh.add(9).value };
				FindStronger fs = new FindStronger(h, this.Division, this.mRules);
				mTrikonaStrengths = fs.getOrderedHouses(zh_t);
			}
			public override object Clone()
			{
				UserOptions uo = new UserOptions(h, this.mRules);
				this.CopyFromNoClone(this);
				uo.mTrikonaStrengths = (OrderedZodiacHouses)this.mTrikonaStrengths.Clone();
				return uo;
			}
			override public object CopyFrom (object _uo)
			{
				UserOptions uo = (UserOptions)_uo;
				if (this.Division != uo.Division ||
					this.ColordAqu != uo.ColordAqu ||
					this.ColordSco != uo.ColordSco) 
				{
					this.calculateTrikonaStrengths();
					this.calculateSeed();
					this.calculateExceptions();
					this.calculateSeventhStrengths();
					this.calculateCoLords();
				}
				base.CopyFromNoClone(_uo);
				return this.Clone();
			}
			new public void recalculate ()
			{
				this.calculateTrikonaStrengths();
				this.calculateSeed();
				this.calculateExceptions();
				this.calculateSeventhStrengths();
				this.calculateCoLords();
			}
		}
		private Horoscope h;
		private UserOptions options;
		public TrikonaDasa (Horoscope _h)
		{
			h = _h;
			options = new UserOptions(h, FindStronger.RulesNavamsaDasaRasi(h));
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public double paramAyus () 
		{
			return 144;
		}

		public DivisionPosition getLordsPosition (ZodiacHouse zh)
		{
			Body.Name b;
			if (zh.value == ZodiacHouse.Name.Sco) b = options.ColordSco;
			else if (zh.value == ZodiacHouse.Name.Aqu) b = options.ColordAqu;
			else b = Basics.SimpleLordOfZodiacHouse(zh.value);

			return h.getPosition(b).toDivisionPosition(options.Division);
		}
		int[] order = {1,5,9,2,6,10,3,7,11,4,8,12};
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (12);
			ZodiacHouse zh_seed = options.getSeed();
			if (options.TrikonaStrengths.houses.Count >= 1)
				zh_seed.value = (ZodiacHouse.Name)options.TrikonaStrengths.houses[0];
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, zh_seed.value, zh_seed.add(7).value);

			bool bIsZodiacal = zh_seed.isOdd();

			double dasa_length_sum = 0.0;
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh_dasa = null;
				if (bIsZodiacal)
					zh_dasa = zh_seed.add (order[i]);
				else
					zh_dasa = zh_seed.addReverse (order[i]);
				double dasa_length = NarayanaDasa.NarayanaDasaLength(zh_dasa, this.getLordsPosition(zh_dasa));


				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			for (int i=0; i<12; i++)
			{
				DasaEntry df = (DasaEntry)al[i];
				double dasa_length = 12.0 - df.dasaLength;
				DasaEntry di = new DasaEntry (df.zodiacHouse, dasa_length_sum, dasa_length, 1, df.shortDesc);
				al.Add (di);
				dasa_length_sum += dasa_length;
			}


			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			NarayanaDasa nd = new NarayanaDasa(h);
			nd.options = this.options;
			return nd.AntarDasa(pdi);
		}
		public String Description ()
		{
			return "Trikona Dasa seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions uo = (UserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			UserOptions newOpts = (UserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}




	public class CharaDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		public CharaDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNavamsaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 144;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public DivisionPosition getLordsPosition (ZodiacHouse zh)
		{
			Body.Name b;
			if (zh.value == ZodiacHouse.Name.Sco) b = options.ColordSco;
			else if (zh.value == ZodiacHouse.Name.Aqu) b = options.ColordAqu;
			else b = Basics.SimpleLordOfZodiacHouse(zh.value);

			return h.getPosition(b).toDivisionPosition(options.Division);
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (12);
			ZodiacHouse zh_seed = options.getSeed();
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, zh_seed.value, zh_seed.add(7).value);

			bool bIsZodiacal = zh_seed.add(9).isOddFooted();

			double dasa_length_sum = 0.0;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = null;
				if (bIsZodiacal)
					zh_dasa = zh_seed.add (i);
				else
					zh_dasa = zh_seed.addReverse (i);
				double dasa_length = NarayanaDasa.NarayanaDasaLength(zh_dasa, this.getLordsPosition(zh_dasa));


				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			for (int i=0; i<12; i++)
			{
				DasaEntry df = (DasaEntry)al[i];
				double dasa_length = 12.0 - df.dasaLength;
				DasaEntry di = new DasaEntry (df.zodiacHouse, dasa_length_sum, dasa_length, 1, df.shortDesc);
				al.Add (di);
				dasa_length_sum += dasa_length;
			}


			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			NarayanaDasa nd = new NarayanaDasa(h);
			nd.options = this.options;
			return nd.AntarDasa(pdi);
		}
		public String Description ()
		{
			return "Chara Dasa seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}



	public class NavamsaDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		public NavamsaDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNavamsaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 108;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (12);
			ZodiacHouse zh_seed = h.getPosition(Body.Name.Lagna).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;

			if (! zh_seed.isOdd())
				zh_seed = zh_seed.AdarsaSign();

			double dasa_length_sum = 0.0;
			double dasa_length = 9.0;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = zh_seed.add (i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList(12);

			ZodiacHouse zh_first = new ZodiacHouse(pdi.zodiacHouse);
			ZodiacHouse zh_stronger = zh_first.add(1);
			if (! zh_stronger.isOdd())
				zh_stronger = zh_stronger.AdarsaSign();

			double dasa_start = pdi.startUT;

			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = zh_stronger.add (i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_start, pdi.dasaLength / 12.0, pdi.level+1, pdi.shortDesc + " " + zh_dasa.value.ToString());
				al.Add (di);
				dasa_start += pdi.dasaLength / 12.0;
			}

			return al;
		}
		public String Description ()
		{
			return "Navamsa Dasa";
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}

	
	public class MandookaDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		public MandookaDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNavamsaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 96;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public int DasaLength (ZodiacHouse zh)
		{
			switch ((int)zh.value % 3)
			{
				case 1: return 7;
				case 2: return 8;
				default: return 9;
			}
		}
		public ArrayList Dasa(int cycle)
		{
			int[] sequence = new int[] { 1,3,5,7,9,11,2,4,6,8,10,12 };
			ArrayList al = new ArrayList (12);
			ZodiacHouse zh_seed = options.getSeed();

			if (zh_seed.isOdd())
				zh_seed = zh_seed.add(3);
			else
				zh_seed = zh_seed.addReverse(3);

			bool bDirZodiacal = true;
			if (! zh_seed.isOdd())
			{
				//zh_seed = zh_seed.AdarsaSign();
				bDirZodiacal = false;
			}

			double dasa_length_sum = 0.0;
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh_dasa = null;
				if (bDirZodiacal)
					zh_dasa = zh_seed.add(sequence[i]);
				else
					zh_dasa = zh_seed.addReverse(sequence[i]);
				double dasa_length = this.DasaLength(zh_dasa);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList(12);

			ZodiacHouse zh_first = new ZodiacHouse(pdi.zodiacHouse);
			ZodiacHouse zh_stronger = zh_first.add(1);
			if (! zh_stronger.isOdd())
				zh_stronger = zh_stronger.AdarsaSign();

			double dasa_start = pdi.startUT;

			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = zh_stronger.add (i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_start, pdi.dasaLength / 12.0, pdi.level+1, pdi.shortDesc + " " + zh_dasa.value.ToString());
				al.Add (di);
				dasa_start += pdi.dasaLength / 12.0;
			}

			return al;
		}
		public String Description ()
		{
			return "Mandooka Dasa (seeded from) " + Basics.numPartsInDivisionString (options.Division);
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}



	public class ShoolaDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		public ShoolaDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 108;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (12);
			ZodiacHouse zh_seed = options.getSeed();
			zh_seed.value = options.findStrongerRasi(options.SeventhStrengths, 
					zh_seed.value, zh_seed.add(7).value);

			double dasa_length_sum = 0.0;
			double dasa_length = 9.0;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = zh_seed.add (i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;
			}

			double cycle_length = (double)cycle * this.paramAyus();
			foreach (DasaEntry di in al)
			{
				di.startUT += cycle_length;
			}

			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList(12);

			ZodiacHouse zh_first = new ZodiacHouse(pdi.zodiacHouse);
			ZodiacHouse zh_stronger = zh_first.add(1);
			zh_stronger.value = options.findStrongerRasi(options.SeventhStrengths,
				zh_first.value, zh_first.add(7).value);

			double dasa_start = pdi.startUT;

			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_dasa = zh_stronger.add (i);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_start, pdi.dasaLength / 12.0, pdi.level+1, pdi.shortDesc + " " + zh_dasa.value.ToString());
				al.Add (di);
				dasa_start += pdi.dasaLength / 12.0;
			}

			return al;
		}
		public String Description ()
		{
			return "Shoola Dasa"
				+ " seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			RasiDasaUserOptions uo = (RasiDasaUserOptions)a;
			options.CopyFrom (uo);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}

	public class DrigDasa: Dasa, IDasa
	{
		private RasiDasaUserOptions options;
		private Horoscope h;
		public DrigDasa (Horoscope _h)
		{
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public double paramAyus () 
		{
			return 144;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		Body.Name GetLord (ZodiacHouse zh)
		{
			switch (zh.value)
			{
				case ZodiacHouse.Name.Aqu:
					return options.ColordAqu;
				case ZodiacHouse.Name.Sco:
					return options.ColordSco;
				default:
					return Basics.SimpleLordOfZodiacHouse(zh.value);
			}
		}

		public void DasaHelper (ZodiacHouse zh, ArrayList al)
		{
			int[] order_moveable = new int[] {5,8,11};
			int[] order_fixed = new int[] {3,6,9};
			int[] order_dual = new int[] {4,7,10};
			bool backward = false;
			if (!zh.isOddFooted())
				backward = true;

			int[] order;
			switch ((int)zh.value % 3)
			{
				case 1: order = order_moveable; break;
				case 2: order = order_fixed; break;
				default: order = order_dual; break;
			}
			al.Add (zh.add (1));
			if (! backward) 
			{
				for (int i=0; i<3; i++)
					al.Add (zh.add(order[i]));
			} 
			else 
			{
				for (int i=2; i>=0; i--)
					al.Add (zh.add(order[i]));
			}
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al_order = new ArrayList (12);
			ZodiacHouse zh_seed = options.getSeed().add(9);

			for (int i=1; i<=4; i++) 
			{
				this.DasaHelper(zh_seed.add(i), al_order);
			}

			ArrayList al = new ArrayList (12);

			double dasa_length_sum = 0.0;
			double dasa_length;
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh_dasa = (ZodiacHouse)al_order[i];
				DivisionPosition dp = h.CalculateDivisionPosition(h.getPosition(this.GetLord(zh_dasa)), new Division(Basics.DivisionType.Rasi));
				dasa_length = NarayanaDasa.NarayanaDasaLength(zh_dasa, dp);
				DasaEntry di = new DasaEntry (zh_dasa.value, dasa_length_sum, dasa_length, 1, zh_dasa.value.ToString());
				al.Add (di);
				dasa_length_sum += dasa_length;

			}


			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			NarayanaDasa nd = new NarayanaDasa(h);
			nd.options = this.options;
			return nd.AntarDasa(pdi);
		}
		public String Description ()
		{
			return "Drig Dasa"
				+ " seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			options.CopyFrom (a);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}



	public class LagnaKendradiRasiDasa: Dasa, IDasa
	{
		private Horoscope h;
		private RasiDasaUserOptions options;
		private Division m_dtype = new Division(Basics.DivisionType.Rasi);

		public LagnaKendradiRasiDasa (Horoscope _h)
		{
			FindStronger fs_rasi = new FindStronger (h, m_dtype, FindStronger.RulesNarayanaDasaRasi(h));
			h = _h;
			options = new RasiDasaUserOptions(h, FindStronger.RulesNarayanaDasaRasi(h));
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		private bool isZodiacal ()
		{
			ZodiacHouse zh_start = options.getSeed();
			zh_start.value = options.findStrongerRasi(options.SeventhStrengths, zh_start.value, zh_start.add(7).value);

			bool forward = zh_start.isOdd();
			if (options.saturnExceptionApplies(zh_start.value))
				return forward;
			if (options.ketuExceptionApplies(zh_start.value))
				forward = !forward;
			return forward;
		}

		public double paramAyus () 
		{
			return 144;
		}

		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (24);
			int[] order = {1, 4, 7, 10, 2, 5, 8, 11, 3, 6, 9, 12};
			double dasa_length_sum = 0;

			ZodiacHouse zh_start = options.getSeed();
			zh_start.value = options.findStrongerRasi(options.SeventhStrengths, zh_start.value, zh_start.add(7).value);

			bool bIsZodiacal = this.isZodiacal();
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh = zh_start.add(1);
				if (bIsZodiacal) zh = zh.add(order[i]);
				else zh = zh.addReverse(order[i]);
				Body.Name lord = h.LordOfZodiacHouse(zh, m_dtype);
				DivisionPosition dp_lord = h.getPosition(lord).toDivisionPosition(m_dtype);
				double dasa_length = NarayanaDasa.NarayanaDasaLength(zh, dp_lord);
				DasaEntry de = new DasaEntry(zh.value, dasa_length_sum, dasa_length, 1, zh.value.ToString());
				al.Add (de);
				dasa_length_sum += dasa_length;
			}
			for (int i=0; i<12; i++)
			{
				DasaEntry de_first = (DasaEntry)al[i];
				double dasa_length = 12.0 - de_first.dasaLength;
				DasaEntry de = new DasaEntry(de_first.zodiacHouse, dasa_length_sum, dasa_length, 1, de_first.shortDesc);
				dasa_length_sum += dasa_length;
				al.Add(de);
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			NarayanaDasa nd = new NarayanaDasa(h);
			nd.options = this.options;
			return nd.AntarDasa(pdi);
		}
		public String Description ()
		{
			return "Lagna Kendradi Rasi Dasa seeded from"
				+ " seeded from " + options.SeedRasi.ToString();
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			options.CopyFrom(a);
			RecalculateEvent();
			return options.Clone();
		}
		new public void DivisionChanged (Division div)
		{
			RasiDasaUserOptions newOpts = (RasiDasaUserOptions)options.Clone();
			newOpts.Division = (Division)div.Clone();
			this.SetOptions(newOpts);
		}
	}

	

	public class TajakaDasa: Dasa, IDasa
	{
		private Horoscope h;
		public TajakaDasa (Horoscope _h)
		{
			h = _h;
		}
		public Object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (Object a)
		{
			return new Object();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			return 60.0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList(60);
			double cycle_start = (double)cycle * this.paramAyus();
			for (int i=0; i<60; i++)
			{
				double start = cycle_start + (double)i;
				DasaEntry di = new DasaEntry(Body.Name.Other, start, 1.0, 1, "Tajaka Year");
				al.Add (di);
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			string[] desc = new String[] { "  Tajaka Month", "    Tajaka 60 hour", "      Tajaka 5 hour", "        Tajaka 25 minute", "          Tajaka 2 minute" };
			if (pdi.level == 6)
				return new ArrayList();

			ArrayList al;
			double start = 0.0, length = 0.0;
			int level = 0;

			al = new ArrayList (12);
			start = pdi.startUT;
			level = pdi.level+1;
			length = pdi.dasaLength / 12.0;
			for (int i=0; i<12; i++)
			{
				DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
				al.Add (di);
				start += length;
			}
			return al;
		}
		public String Description ()
		{
			return "Tajaka Chart Dasa";
		}
	}
	public class KaranaPraveshDasa: Dasa, IDasa
	{
		private Horoscope h;
		public KaranaPraveshDasa (Horoscope _h)
		{
			h = _h;
		}
		public Object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (Object a)
		{
			return new Object();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			return 60.0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList(60);
			double cycle_start = (double)cycle * this.paramAyus();
			for (int i=0; i<60; i++)
			{
				double start = cycle_start + (double)i;
				DasaEntry di = new DasaEntry(Body.Name.Other, start, 1.0, 1, "Karana Pravesh Year");
				al.Add (di);
			}
			return al;
		}

		new public string EntryDescription (DasaEntry pdi, Moment start, Moment end)
		{

			if (pdi.level == 2)
			{
				Longitude l = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				ZodiacHouse zh = l.toZodiacHouse();
				return zh.ToString();
			}
			else if (pdi.level == 3)
			{
				Longitude lSun = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				Longitude lMoon = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Moon));
				Longitude l = lMoon.sub(lSun);
				Karana k = l.toKarana();
				return k.ToString();
			}
			return "";
		}

		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			string[] desc = new String[] { "  Month: ", "    Tithi: " };
			if (pdi.level == 3)
				return new ArrayList();

			ArrayList al;
			double start = 0.0, length = 0.0;
			int level = 0;

			al = null;
			start = pdi.startUT;
			level = pdi.level+1;

			switch (pdi.level)
			{
				case 1:
					al = new ArrayList (13);
					length = pdi.dasaLength / 13.0;
					//Console.WriteLine("AD length is {0}", length);
					for (int i=0; i<13; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						al.Add (di);
						start += length;
					}
					return al;
				case 2:
					al = new ArrayList (60);
					length = pdi.dasaLength / 60.0;
					//Console.WriteLine("PD length is {0}", length);
					for (int i=0; i<60; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						//Console.WriteLine ("PD: Starg {0}, length {1}", start, length);
						al.Add (di);
						start += length;
					}
					return al;
			}
			return new ArrayList();;
		}
		public String Description ()
		{
			return "Karana Pravesh Chart Dasa";
		}
	}


	public class TithiPraveshDasa: Dasa, IDasa
	{
		private Horoscope h;
		public TithiPraveshDasa (Horoscope _h)
		{
			h = _h;
		}
		public Object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (Object a)
		{
			return new Object();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			return 60.0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList(60);
			double cycle_start = (double)cycle * this.paramAyus();
			for (int i=0; i<60; i++)
			{
				double start = cycle_start + (double)i;
				DasaEntry di = new DasaEntry(Body.Name.Other, start, 1.0, 1, "Tithi Pravesh Year");
				al.Add (di);
			}
			return al;
		}

		new public string EntryDescription (DasaEntry pdi, Moment start, Moment end)
		{

			if (pdi.level == 2)
			{
				Longitude l = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				ZodiacHouse zh = l.toZodiacHouse();
				return zh.ToString();
			}
			else if (pdi.level == 3)
			{
				Longitude lSun = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				Longitude lMoon = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Moon));
				Longitude l = lMoon.sub(lSun);
				Tithi t = l.toTithi();
				return t.ToString();
			}
			return "";
		}

		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			string[] desc = new String[] { "  Month: ", "    Tithi: " };
			if (pdi.level == 3)
				return new ArrayList();

			ArrayList al;
			double start = 0.0, length = 0.0;
			int level = 0;

			al = null;
			start = pdi.startUT;
			level = pdi.level+1;

			switch (pdi.level)
			{
				case 1:
					al = new ArrayList (13);
					length = pdi.dasaLength / 13.0;
					//Console.WriteLine("AD length is {0}", length);
					for (int i=0; i<13; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						al.Add (di);
						start += length;
					}
					return al;
				case 2:
					al = new ArrayList (30);
					length = pdi.dasaLength / 30.0;
					//Console.WriteLine("PD length is {0}", length);
					for (int i=0; i<30; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						//Console.WriteLine ("PD: Starg {0}, length {1}", start, length);
						al.Add (di);
						start += length;
					}
					return al;
			}
			return new ArrayList();;
		}
		public String Description ()
		{
			return "Tithi Pravesh Chart Dasa";
		}
	}

	public class NakshatraPraveshDasa: Dasa, IDasa
	{
		private Horoscope h;
		public NakshatraPraveshDasa (Horoscope _h)
		{
			h = _h;
		}
		public Object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (Object a)
		{
			return new Object();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			return 60.0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList(60);
			double cycle_start = (double)cycle * this.paramAyus();
			for (int i=0; i<60; i++)
			{
				double start = cycle_start + (double)i;
				DasaEntry di = new DasaEntry(Body.Name.Other, start, 1.0, 1, "Nakshatra Pravesh Year");
				al.Add (di);
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			string[] desc = new String[] { "  Month: ", "    Yoga: " };
			if (pdi.level == 3)
				return new ArrayList();

			ArrayList al;
			double start = 0.0, length = 0.0;
			int level = 0;

			al = null;
			start = pdi.startUT;
			level = pdi.level+1;

			switch (pdi.level)
			{
				case 1:
					al = new ArrayList (13);
					length = pdi.dasaLength / 13.0;
					//Console.WriteLine("AD length is {0}", length);
					for (int i=0; i<15; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						al.Add (di);
						start += length;
					}
					return al;
				case 2:
					al = new ArrayList (27);
					length = pdi.dasaLength / 27.0;
					//Console.WriteLine("PD length is {0}", length);
					for (int i=0; i<27; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						//Console.WriteLine ("PD: Starg {0}, length {1}", start, length);
						al.Add (di);
						start += length;
					}
					return al;
			}
			return new ArrayList();;
		}
		new public string EntryDescription (DasaEntry pdi, Moment start, Moment end)
		{
			if (pdi.level == 2)
			{
				Longitude l = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				ZodiacHouse zh = l.toZodiacHouse();
				return zh.ToString();
			}
			else if (pdi.level == 3)
			{
				Longitude l = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Moon));
				Nakshatra n = l.toNakshatra();
				return n.toShortString();
			}
			return "";
		}

		public String Description ()
		{
			return "Nakshatra Pravesh Chart Dasa";
		}
	}


	
	public class YogaPraveshDasa: Dasa, IDasa
	{
		private Horoscope h;
		public YogaPraveshDasa (Horoscope _h)
		{
			h = _h;
		}
		public Object GetOptions ()
		{
			return new Object();
		}
		public object SetOptions (Object a)
		{
			return new Object();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			return 60.0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList(60);
			double cycle_start = (double)cycle * this.paramAyus();
			for (int i=0; i<60; i++)
			{
				double start = cycle_start + (double)i;
				DasaEntry di = new DasaEntry(Body.Name.Other, start, 1.0, 1, "Yoga Pravesh Year");
				al.Add (di);
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			string[] desc = new String[] { "  Month: ", "    Yoga: " };
			if (pdi.level == 3)
				return new ArrayList();

			ArrayList al;
			double start = 0.0, length = 0.0;
			int level = 0;

			al = null;
			start = pdi.startUT;
			level = pdi.level+1;

			switch (pdi.level)
			{
				case 1:
					al = new ArrayList (15);
					length = pdi.dasaLength / 15.0;
					//Console.WriteLine("AD length is {0}", length);
					for (int i=0; i<15; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						al.Add (di);
						start += length;
					}
					return al;
				case 2:
					al = new ArrayList (27);
					length = pdi.dasaLength / 27.0;
					//Console.WriteLine("PD length is {0}", length);
					for (int i=0; i<27; i++)
					{
						DasaEntry di = new DasaEntry (Body.Name.Other, start, length, level, desc[level-2]);
						//Console.WriteLine ("PD: Starg {0}, length {1}", start, length);
						al.Add (di);
						start += length;
					}
					return al;
			}
			return new ArrayList();;
		}
		new public string EntryDescription (DasaEntry pdi, Moment start, Moment end)
		{
			if (pdi.level == 2)
			{
				Longitude l = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				ZodiacHouse zh = l.toZodiacHouse();
				return zh.ToString();
			}
			else if (pdi.level == 3)
			{
				Longitude lSun = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Sun));
				Longitude lMoon = Basics.CalculateBodyLongitude(start.toUniversalTime(), sweph.BodyNameToSweph(Body.Name.Moon));
				Longitude l = lMoon.add(lSun);

				// this seems wrong. Why should we need to go to the next yoga here?
				SunMoonYoga y = l.toSunMoonYoga().add(2);
				return y.ToString();
			}
			return "";
		}

		public String Description ()
		{
			return "Yoga Pravesh Chart Dasa";
		}
	}


	public class NaisargikaRasiDasa: Dasa, IDasa
	{
		public class UserOptions :ICloneable
		{
			[PGDisplayName("Life Expectancy")]
			public enum ParamAyusType: int 
			{
				Short, Middle, Long
			}
			private ParamAyusType paramAyus;
			public UserOptions () 
			{
				ParamAyus = ParamAyusType.Middle;
			}
			[PGDisplayName("Total Param Ayus")]
			public ParamAyusType ParamAyus
			{
				get { return paramAyus; }
				set { paramAyus = value; }
			}
			public Object Clone () 
			{
				UserOptions uo = new UserOptions();
				uo.paramAyus = this.paramAyus;
				return uo;
			}
		}

		private Horoscope h;
		private UserOptions options;
		public NaisargikaRasiDasa (Horoscope _h)
		{
			h = _h;
			options = new UserOptions();
		}
		public void recalculateOptions ()
		{
		}
		public double paramAyus () 
		{
			switch (this.options.ParamAyus) 
			{
				case UserOptions.ParamAyusType.Long: return 120.0;
				case UserOptions.ParamAyusType.Middle: return 108.0;
				default: return 96.0;
			}
		}
		public ArrayList Dasa(int cycle)
		{
			int[] order = new int[] {4, 2, 8, 10, 12, 6, 5, 11, 1, 7, 9, 3};
			int[] short_length = new int[] { 9, 7, 8};
			ArrayList al = new ArrayList (9);

			double cycle_start = paramAyus() * (double)cycle;
			double curr = 0.0;
			double dasa_length;
			ZodiacHouse zlagna = h.getPosition(Body.Name.Lagna).longitude.toZodiacHouse();
			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh = zlagna.add (order[i]);
				switch (this.options.ParamAyus) 
				{
					case UserOptions.ParamAyusType.Long: dasa_length = 10.0; break;
					case UserOptions.ParamAyusType.Middle: dasa_length = 9.0; break;
					default: 
						int mod = ((int)zh.value) % 3;
						dasa_length = short_length[mod];
						break;
				}
				al.Add (new DasaEntry (zh.value, cycle_start + curr, dasa_length, 1, zh.value.ToString()));
				curr += dasa_length;

			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			return new ArrayList();
		}
		public String Description ()
		{
			return "Naisargika Rasi Dasa";
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions uo = (UserOptions)a;
			this.options.ParamAyus = uo.ParamAyus;
			RecalculateEvent();
			return options.Clone();
		}

	}


	public class KarakaKendradiGrahaDasa: Dasa, IDasa
	{
		public class UserOptions :ICloneable
		{
			Horoscope h;
			ArrayList std_div_pos;
			public Division dtype = new Division(Basics.DivisionType.Rasi);
			protected Body.Name mSeedBody;
			OrderedGrahas mGrahasStrengths;
			OrderedZodiacHouses[] mZodiacStrengths;

			public UserOptions (Horoscope _h) 
			{
				h = _h;
				std_div_pos = h.CalculateDivisionPositions(dtype);
				this.recalculate();
			}
			public void recalculate ()
			{
				this.CalculateSeedBody();
				this.CalculateRasiStrengths();
				this.CalculateGrahaStrengths();
			}
			public void CompareAndRecalculate (UserOptions newOpts)
			{
				if (newOpts.SeedBody != this.SeedBody)
				{
					newOpts.CalculateRasiStrengths();
					newOpts.CalculateGrahaStrengths();
					return;
				}

				for (int i=0; i<3; i++)
				{
					if (newOpts.mZodiacStrengths[i].houses.Count != this.mZodiacStrengths[i].houses.Count)
					{
						newOpts.CalculateGrahaStrengths();
						return;
					}
					for (int j=0; j<newOpts.mZodiacStrengths[i].houses.Count; j++)
					{
						if ((ZodiacHouse.Name)newOpts.mZodiacStrengths[i].houses[j] !=
							(ZodiacHouse.Name)this.mZodiacStrengths[i].houses[j])
						{
							newOpts.CalculateGrahaStrengths();
							return;
						}
					}	
				}
			}

			public void CalculateSeedBody ()
			{
				ArrayList al_k = new ArrayList();
				for (int i=(int)mhora.Body.Name.Sun; i<=(int)Body.Name.Rahu; i++)
				{
					mhora.Body.Name b = (mhora.Body.Name)i;
					BodyPosition bp = h.getPosition(b);
					BodyKarakaComparer bkc = new BodyKarakaComparer(bp);
					al_k.Add (bkc);
				}
				al_k.Sort();

				BodyPosition bp_ak = ((BodyKarakaComparer)al_k[0]).GetPosition;
				this.SeedBody = bp_ak.name;
			}
			public void CalculateRasiStrengths()
			{
				OrderedZodiacHouses[] zRet = new OrderedZodiacHouses[3];
				ZodiacHouse zh = h.getPosition(this.SeedBody).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;

				ZodiacHouse.Name[] zh_k = new ZodiacHouse.Name[4] { zh.add(1).value, zh.add(4).value, zh.add(7).value, zh.add(10).value };
				ZodiacHouse.Name[] zh_p = new ZodiacHouse.Name[4] { zh.add(2).value, zh.add(5).value, zh.add(8).value, zh.add(11).value };
				ZodiacHouse.Name[] zh_a = new ZodiacHouse.Name[4] { zh.add(3).value, zh.add(6).value, zh.add(9).value, zh.add(12).value };
				
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesKarakaKendradiGrahaDasaRasi(h));
				zRet[0] = fs.getOrderedHouses(zh_k);
				zRet[1] = fs.getOrderedHouses(zh_p);
				zRet[2] = fs.getOrderedHouses(zh_a);

				ZodiacHouse.Name zh_sat = h.getPosition(Body.Name.Saturn).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value;
				ZodiacHouse.Name zh_ket = h.getPosition(Body.Name.Ketu).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value;

				bool bIsForward = zh.isOdd();
				if (zh_sat != zh_ket && zh_sat == zh.value)
					bIsForward = true;
				else if (zh_sat != zh_ket && zh_ket == zh.value)
					bIsForward = false;
				else if (zh_sat == zh_ket && zh_sat == zh.value)
				{
					ArrayList rule = new ArrayList();
					rule.Add (FindStronger.EGrahaStrength.Longitude);
					FindStronger fs2 = new FindStronger(h, new Division(Basics.DivisionType.Rasi), rule);
					bIsForward = fs2.CmpGraha(Body.Name.Saturn, Body.Name.Ketu, false);
				}


				this.mZodiacStrengths = new OrderedZodiacHouses[3];
				this.mZodiacStrengths[0] = zRet[0];
				if (bIsForward)
				{
					this.mZodiacStrengths[1] = zRet[1];
					this.mZodiacStrengths[2] = zRet[2];
				} 
				else
				{
					this.mZodiacStrengths[1] = zRet[2];
					this.mZodiacStrengths[2] = zRet[1];
				}
			}
			public void CalculateGrahaStrengths()
			{
				StrengthByConjunction fs_temp = new StrengthByConjunction (h, dtype);
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesKarakaKendradiGrahaDasaGraha(h));
				this.mGrahasStrengths = new OrderedGrahas();
				foreach (OrderedZodiacHouses oz in this.mZodiacStrengths)
				{
					foreach (ZodiacHouse.Name zn in oz.houses)
					{
						ArrayList temp = fs_temp.findGrahasInHouse (zn);
						Body.Name[] temp_arr = new Body.Name[temp.Count];
						for (int i=0; i< temp.Count; i++)
							temp_arr[i] = (Body.Name)temp[i];
						Body.Name[] sorted = fs.getOrderedGrahas(temp_arr);
						foreach (Body.Name bn in sorted)
							this.mGrahasStrengths.grahas.Add (bn);
					}
				}
			}

			[Category("Strengths1 Seed")]
			[PGDisplayName("Seed Body")]
			public Body.Name SeedBody
			{
				get { return this.mSeedBody; }
				set { this.mSeedBody = value; }
			}


			[Category("Strengths3 Grahas")]
			[PGDisplayName("Graha strength order")]
			public OrderedGrahas GrahaStrengths
			{
				get { return this.mGrahasStrengths; }
				set { this.mGrahasStrengths = value; }
			}

			[Category("Strengths2 Rasis")]
			[PGDisplayName("Rasi strength order")]
			public OrderedZodiacHouses[] RasiStrengths
			{
				get { return this.mZodiacStrengths; }
				set { this.mZodiacStrengths = value; }
			}


			public Object Clone () 
			{
				UserOptions uo = new UserOptions(h);
				uo.mGrahasStrengths = (OrderedGrahas)this.mGrahasStrengths.Clone();
				uo.mZodiacStrengths = new OrderedZodiacHouses[3];
				uo.mSeedBody = this.mSeedBody;
				for (int i=0; i<3; i++)
					uo.mZodiacStrengths[i] = (OrderedZodiacHouses)this.mZodiacStrengths[i].Clone();
					
				return uo;
			}
		}

		private Horoscope h;
		private UserOptions options;
		public KarakaKendradiGrahaDasa (Horoscope _h)
		{
			h = _h;
			options = new UserOptions(h);
			vd = new VimsottariDasa(h);
		}
		public double paramAyus () 
		{
			return 120.0;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		VimsottariDasa vd = null;
		public double lengthOfDasa (Body.Name plt)
		{
			DivisionPosition dp_plt = h.getPosition(plt).toDivisionPosition(new Division(Basics.DivisionType.Rasi));
			return KarakaKendradiGrahaDasa.LengthOfDasa(h, options.dtype, plt, dp_plt);
		}
		public static double LengthOfDasa (Horoscope h, Division dtype, Body.Name plt, DivisionPosition dp_plt)
		{
			double length = 0;

			// Count to moola trikona - 1.
			// Use Aqu / Sco as MT houses for Rahu / Ketu
			//DivisionPosition dp_plt = h.getPosition(plt).toDivisionPosition(new Division(Basics.DivisionType.Rasi));
			ZodiacHouse zh_plt = dp_plt.zodiac_house;
			ZodiacHouse zh_mt = Basics.getMoolaTrikonaRasi(plt);
			
			if (plt == Body.Name.Rahu) zh_mt.value = ZodiacHouse.Name.Aqu;
			if (plt == Body.Name.Ketu) zh_mt.value = ZodiacHouse.Name.Sco;

			int diff = zh_plt.numHousesBetween(zh_mt);
			length = (double)(diff-1);

			// exaltation / debilitation correction
			if (dp_plt.isExaltedPhalita())
				length+=1.0;
			else if (dp_plt.isDebilitatedPhalita())
				length-=1.0;

			if (plt == h.LordOfZodiacHouse(zh_plt, dtype))
				length = 12.0;

			// subtract this length from the vimsottari lengths
			length = VimsottariDasa.LengthOfDasa(plt) - length;

			// Zero length = full vimsottari length.
			// If negative, make it positive
			if (length == 0) length = VimsottariDasa.LengthOfDasa(plt);
			else if (length < 0) length *= -1;

			return length;
		}
		public ArrayList Dasa(int cycle)
		{
			double cycle_start = paramAyus() * (double)cycle;
			double curr = 0.0;
			ArrayList al = new ArrayList (24);
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				double dasaLength = this.lengthOfDasa(b);
				al.Add (new DasaEntry (b, cycle_start + curr, dasaLength, 1, Body.toShortString(b)));
				curr += dasaLength;
			}

			int numDasas = al.Count;
			for (int i=0; i< numDasas; i++)
			{
				DasaEntry de = (DasaEntry)al[i];
				double dasaLength = de.dasaLength-vd.lengthOfDasa(de.graha);
				if (dasaLength < 0) dasaLength *= -1;
				al.Add (new DasaEntry (de.graha, cycle_start + curr, dasaLength, 1, Body.toShortString(de.graha)));
				curr += dasaLength;

			}
			
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList();
			double curr = pdi.startUT;

			ArrayList bOrder = new ArrayList();
			bool bFound = false;
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				if (b != pdi.graha && bFound == false) continue;
				bFound = true;
				bOrder.Add (b);
			}
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				if (b == pdi.graha) break;
				bOrder.Add (b);
			}


			double dasaLength = pdi.dasaLength / 9.0;
			foreach (Body.Name b in bOrder)
			{
				al.Add (new DasaEntry(b, curr, dasaLength, pdi.level+1, pdi.shortDesc + " " + Body.toShortString(b)));
				curr += dasaLength;
			}
			return al;
		}
		public String Description ()
		{
			return String.Format ("Karaka Kendradi Graha Dasa seeded from {0}", options.SeedBody);
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions newOpts = (UserOptions)a;
			this.options.CompareAndRecalculate(newOpts);
			this.options = newOpts;
			RecalculateEvent();
			return options.Clone();
		}

	}


	public class MoolaDasa: Dasa, IDasa
	{
		public class UserOptions :ICloneable
		{
			Horoscope h;
			ArrayList std_div_pos;
			public Division dtype = new Division(Basics.DivisionType.Rasi);
			protected Body.Name mSeedBody;
			OrderedGrahas mGrahasStrengths;
			OrderedZodiacHouses[] mZodiacStrengths;

			public UserOptions (Horoscope _h) 
			{
				h = _h;
				std_div_pos = h.CalculateDivisionPositions(dtype);
				this.mSeedBody = Body.Name.Lagna;
				this.CalculateRasiStrengths();
				this.CalculateGrahaStrengths();
			}
			public void recalculate ()
			{
				this.CalculateRasiStrengths();
				this.CalculateGrahaStrengths();
			}
			public void CompareAndRecalculate (UserOptions newOpts)
			{
				if (newOpts.SeedBody != this.SeedBody)
				{
					newOpts.CalculateRasiStrengths();
					newOpts.CalculateGrahaStrengths();
					return;
				}

				for (int i=0; i<3; i++)
				{
					if (newOpts.mZodiacStrengths[i].houses.Count != this.mZodiacStrengths[i].houses.Count)
					{
						newOpts.CalculateGrahaStrengths();
						return;
					}
					for (int j=0; j<newOpts.mZodiacStrengths[i].houses.Count; j++)
					{
						if ((ZodiacHouse.Name)newOpts.mZodiacStrengths[i].houses[j] !=
							(ZodiacHouse.Name)this.mZodiacStrengths[i].houses[j])
						{
							newOpts.CalculateGrahaStrengths();
							return;
						}
					}	
				}
			}

			public void CalculateRasiStrengths()
			{
				OrderedZodiacHouses[] zRet = new OrderedZodiacHouses[3];
				ZodiacHouse zh = h.getPosition(this.SeedBody).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;

				ZodiacHouse.Name[] zh_k = new ZodiacHouse.Name[4] { zh.add(1).value, zh.add(4).value, zh.add(7).value, zh.add(10).value };
				ZodiacHouse.Name[] zh_p = new ZodiacHouse.Name[4] { zh.add(2).value, zh.add(5).value, zh.add(8).value, zh.add(11).value };
				ZodiacHouse.Name[] zh_a = new ZodiacHouse.Name[4] { zh.add(3).value, zh.add(6).value, zh.add(9).value, zh.add(12).value };
				
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesMoolaDasaRasi(h));
				zRet[0] = fs.getOrderedHouses(zh_k);
				zRet[1] = fs.getOrderedHouses(zh_p);
				zRet[2] = fs.getOrderedHouses(zh_a);

				ZodiacHouse.Name zh_sat = h.getPosition(Body.Name.Saturn).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value;
				ZodiacHouse.Name zh_ket = h.getPosition(Body.Name.Ketu).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value;

				bool bIsForward = zh.isOdd();
				if (zh_sat != zh_ket && zh_sat == zh.value)
					bIsForward = true;
				else if (zh_sat != zh_ket && zh_ket == zh.value)
					bIsForward = false;
				else if (zh_sat == zh_ket && zh_sat == zh.value)
				{
					ArrayList rule = new ArrayList();
					rule.Add (FindStronger.EGrahaStrength.Longitude);
					FindStronger fs2 = new FindStronger(h, new Division(Basics.DivisionType.Rasi), rule);
					bIsForward = fs2.CmpGraha(Body.Name.Saturn, Body.Name.Ketu, false);
				}


				this.mZodiacStrengths = new OrderedZodiacHouses[3];
				this.mZodiacStrengths[0] = zRet[0];
				if (bIsForward)
				{
					this.mZodiacStrengths[1] = zRet[1];
					this.mZodiacStrengths[2] = zRet[2];
				} 
				else
				{
					this.mZodiacStrengths[1] = zRet[2];
					this.mZodiacStrengths[2] = zRet[1];
				}
			}
			public void CalculateGrahaStrengths()
			{
				StrengthByConjunction fs_temp = new StrengthByConjunction (h, dtype);
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesNaisargikaDasaGraha(h));
				this.mGrahasStrengths = new OrderedGrahas();
				foreach (OrderedZodiacHouses oz in this.mZodiacStrengths)
				{
					foreach (ZodiacHouse.Name zn in oz.houses)
					{
						ArrayList temp = fs_temp.findGrahasInHouse (zn);
						Body.Name[] temp_arr = new Body.Name[temp.Count];
						for (int i=0; i< temp.Count; i++)
							temp_arr[i] = (Body.Name)temp[i];
						Body.Name[] sorted = fs.getOrderedGrahas(temp_arr);
						foreach (Body.Name bn in sorted)
							this.mGrahasStrengths.grahas.Add (bn);
					}
				}
			}

			[Category("Strengths1 Seed")]
			[PGDisplayName("Seed Body")]
			public Body.Name SeedBody
			{
				get { return this.mSeedBody; }
				set { this.mSeedBody = value; }
			}


			[Category("Strengths3 Grahas")]
			[PGDisplayName("Graha strength order")]
			public OrderedGrahas GrahaStrengths
			{
				get { return this.mGrahasStrengths; }
				set { this.mGrahasStrengths = value; }
			}

			[Category("Strengths2 Rasis")]
			[PGDisplayName("Rasi strength order")]
			public OrderedZodiacHouses[] RasiStrengths
			{
				get { return this.mZodiacStrengths; }
				set { this.mZodiacStrengths = value; }
			}


			public Object Clone () 
			{
				UserOptions uo = new UserOptions(h);
				uo.mGrahasStrengths = (OrderedGrahas)this.mGrahasStrengths.Clone();
				uo.mZodiacStrengths = new OrderedZodiacHouses[3];
				uo.mSeedBody = this.mSeedBody;
				for (int i=0; i<3; i++)
					uo.mZodiacStrengths[i] = (OrderedZodiacHouses)this.mZodiacStrengths[i].Clone();
					
				return uo;
			}
		}

		private Horoscope h;
		private UserOptions options;
		public MoolaDasa (Horoscope _h)
		{
			h = _h;
			options = new UserOptions(h);
			vd = new VimsottariDasa(h);
		}
		public double paramAyus () 
		{
			return 120.0;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		VimsottariDasa vd = null;
		public double lengthOfDasa (Body.Name plt)
		{
			double length = 0;

			// Count to moola trikona - 1.
			// Use Aqu / Sco as MT houses for Rahu / Ketu
			DivisionPosition dp_plt = h.getPosition(plt).toDivisionPosition(new Division(Basics.DivisionType.Rasi));
			ZodiacHouse zh_plt = dp_plt.zodiac_house;
			ZodiacHouse zh_mt = Basics.getMoolaTrikonaRasi(plt);
			if (plt == Body.Name.Rahu) zh_mt.value = ZodiacHouse.Name.Aqu;
			if (plt == Body.Name.Ketu) zh_mt.value = ZodiacHouse.Name.Sco;
			int diff = zh_plt.numHousesBetween(zh_mt);
			length = (double)(diff-1);

			// exaltation / debilitation correction
			if (dp_plt.isExaltedPhalita())
				length+=1.0;
			else if (dp_plt.isDebilitatedPhalita())
				length-=1.0;

			// subtract this length from the vimsottari lengths
			length = vd.lengthOfDasa(plt) - length;

			// Zero length = full vimsottari length.
			// If negative, make it positive
			if (length == 0) length = vd.lengthOfDasa(plt);
			else if (length < 0) length *= -1;

			return length;
		}
		public ArrayList Dasa(int cycle)
		{
			double cycle_start = paramAyus() * (double)cycle;
			double curr = 0.0;
			ArrayList al = new ArrayList (24);
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				double dasaLength = this.lengthOfDasa(b);
				al.Add (new DasaEntry (b, cycle_start + curr, dasaLength, 1, Body.toShortString(b)));
				curr += dasaLength;
			}

			int numDasas = al.Count;
			for (int i=0; i< numDasas; i++)
			{
				DasaEntry de = (DasaEntry)al[i];
				double dasaLength = de.dasaLength-vd.lengthOfDasa(de.graha);
				al.Add (new DasaEntry (de.graha, cycle_start + curr, dasaLength, 1, Body.toShortString(de.graha)));
				curr += dasaLength;

			}
			
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			ArrayList al = new ArrayList();
			double curr = pdi.startUT;

			ArrayList bOrder = new ArrayList();
			bool bFound = false;
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				if (b != pdi.graha && bFound == false) continue;
				bFound = true;
				bOrder.Add (b);
			}
			foreach (Body.Name b in options.GrahaStrengths.grahas)
			{
				if (b == pdi.graha) break;
				bOrder.Add (b);
			}


			foreach (Body.Name b in bOrder)
			{
				double dasaLength = vd.lengthOfDasa(b) / vd.paramAyus() * pdi.dasaLength;
				al.Add (new DasaEntry(b, curr, dasaLength, pdi.level+1, pdi.shortDesc + " " + Body.toShortString(b)));
				curr += dasaLength;
			}
			return al;
		}
		public String Description ()
		{
			return String.Format ("Moola Dasa seeded from {0}", options.SeedBody);
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions newOpts = (UserOptions)a;
			this.options.CompareAndRecalculate(newOpts);
			this.options = newOpts;
			RecalculateEvent();
			return options.Clone();
		}

	}




	public class NaisargikaGrahaDasa: Dasa, IDasa
	{
		public class UserOptions :ICloneable
		{
			Horoscope h;
			ArrayList std_div_pos;
			public Division dtype = new Division(Basics.DivisionType.Rasi);
			protected Body.Name mLordAqu;
			protected Body.Name mLordSco;
			OrderedGrahas[] mGrahasStrengths;
			OrderedZodiacHouses[] mZodiacStrengths;
			bool bExcludeNodes;
			bool bExcludeDasaLord;
			bool bExclude_3_10;
			bool bExclude_2_6_11_12;

			public UserOptions (Horoscope _h) 
			{
				h = _h;
				std_div_pos = h.CalculateDivisionPositions(dtype);
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesStrongerCoLord(h));
				mLordSco = fs.StrongerGraha(Body.Name.Mars, Body.Name.Ketu, true);
				mLordAqu = fs.StrongerGraha(Body.Name.Saturn, Body.Name.Rahu, true);
				this.bExcludeNodes = true;
				this.bExcludeDasaLord = true;
				this.bExclude_3_10 = false;
				this.bExclude_2_6_11_12 = false;
				this.CalculateRasiStrengths();
				this.CalculateGrahaStrengths();
			}
			public void recalculate ()
			{
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesStrongerCoLord(h));
				mLordSco = fs.StrongerGraha(Body.Name.Mars, Body.Name.Ketu, true);
				mLordAqu = fs.StrongerGraha(Body.Name.Saturn, Body.Name.Rahu, true);
				this.CalculateRasiStrengths();
				this.CalculateGrahaStrengths();
			}
			public void CompareAndRecalculate (UserOptions newOpts)
			{
				if (newOpts.mLordAqu != this.mLordAqu ||
					newOpts.mLordSco != this.mLordSco)
				{
					newOpts.CalculateRasiStrengths();
					newOpts.CalculateGrahaStrengths();
					return;
				}

				for (int i=0; i<3; i++)
				{
					if (newOpts.mZodiacStrengths[i].houses.Count != this.mZodiacStrengths[i].houses.Count)
					{
						newOpts.CalculateGrahaStrengths();
						return;
					}
					for (int j=0; j<newOpts.mZodiacStrengths[i].houses.Count; j++)
					{
						if ((ZodiacHouse.Name)newOpts.mZodiacStrengths[i].houses[j] !=
							(ZodiacHouse.Name)this.mZodiacStrengths[i].houses[j])
						{
							newOpts.CalculateGrahaStrengths();
							return;
						}
					}
					
				}
			}

			public void CalculateRasiStrengths()
			{
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesNaisargikaDasaRasi(h));
				this.mZodiacStrengths = fs.ResultsZodiacKendras(h.CalculateDivisionPosition(h.getPosition(Body.Name.Lagna), dtype).zodiac_house.value);
			}
			public void CalculateGrahaStrengths()
			{
				StrengthByConjunction fs_temp = new StrengthByConjunction (h, dtype);
				FindStronger fs = new FindStronger(h, dtype, FindStronger.RulesNaisargikaDasaGraha(h));
				this.mGrahasStrengths = new OrderedGrahas[3];
				for (int i=0; i<mZodiacStrengths.Length; i++)
				{
					this.mGrahasStrengths[i] = new OrderedGrahas();
					OrderedZodiacHouses oz = this.mZodiacStrengths[i];
					foreach (ZodiacHouse.Name zn in oz.houses)
					{
						ArrayList temp = fs_temp.findGrahasInHouse(zn);
						Body.Name[] temp_arr = new Body.Name[temp.Count];
						for (int j=0; j< temp.Count; j++)
							temp_arr[j] = (Body.Name)temp[j];
						Body.Name[] sorted = fs.getOrderedGrahas(temp_arr);
						foreach (Body.Name bn in sorted)
							this.mGrahasStrengths[i].grahas.Add (bn);					}
				}

			}

			[Category("1: Colord")]
			[PropertyOrder(1), PGDisplayName("Colord")]
			[Description("Is Ketu or Mars the stronger lord of Scorpio?")]
			public Body.Name Lord_Sco
			{
				get { return this.mLordSco; }
				set { this.mLordSco = value; }
			}

			[Category("1: Colord")]
			[PropertyOrder(2), PGDisplayName("Lord of Aquarius")]
			[Description("Is Rahu or Saturn the stronger lord of Aquarius?")]
			public Body.Name Lord_Aqu
			{
				get { return mLordAqu; }
				set { this.mLordAqu = value; }
			}

			[Category("2: Strengths")]
			[PropertyOrder(2), PGDisplayName("Graha strength order")]
			public OrderedGrahas[] GrahaStrengths
			{
				get { return this.mGrahasStrengths; }
				set { this.mGrahasStrengths = value; }
			}

			[Category("2: Strengths")]
			[PropertyOrder(1), PGDisplayName("Rasi strength order")]
			public OrderedZodiacHouses[] RasiStrengths
			{
				get { return this.mZodiacStrengths; }
				set { this.mZodiacStrengths = value; }
			}

			[Category("4: Exclude Antardasas")]
			[PGDisplayName("Exclude Rahu / Ketu")]
			public bool ExcludeNodes
			{
				get { return this.bExcludeNodes; }
				set { this.bExcludeNodes = value; }
			}

			[Category("4: Exclude Antardasas")]
			[PGDisplayName("Exclude dasa lord")]
			public bool ExcludeDasaLord
			{
				get { return this.bExcludeDasaLord; }
				set { this.bExcludeDasaLord = value; }
			}

			[Category("4: Exclude Antardasas")]
			[PGDisplayName("Exclude grahas in 3rd & 10th")]
			public bool Exclude_3_10
			{
				get { return this.bExclude_3_10; }
				set { this.bExclude_3_10 = value; }
			}

			[Category("4: Exclude Antardasas")]
			[PGDisplayName("Exclude grahas in 2nd, 6th, 11th & 12th")]
			public bool Exclude_2_6_11_12
			{
				get { return this.bExclude_2_6_11_12; }
				set { this.bExclude_2_6_11_12 = value; }
			}

			public Object Clone () 
			{
				UserOptions uo = new UserOptions(h);
				uo.mLordSco = this.mLordSco;
				uo.mLordAqu = this.mLordAqu;
				uo.mZodiacStrengths = new OrderedZodiacHouses[3];
				for (int i=0; i<3; i++)
				{
					uo.mZodiacStrengths[i] = (OrderedZodiacHouses)this.mZodiacStrengths[i].Clone();
					uo.mGrahasStrengths[i] = (OrderedGrahas)this.mGrahasStrengths[i].Clone();
				}
				uo.bExcludeDasaLord = this.bExcludeDasaLord;
				uo.bExcludeNodes = this.bExcludeNodes;
				uo.bExclude_2_6_11_12 = this.bExclude_2_6_11_12;
				uo.bExclude_3_10 = this.bExclude_3_10;
				return uo;
			}
		}

		private Horoscope h;
		private UserOptions options;
		public NaisargikaGrahaDasa (Horoscope _h)
		{
			h = _h;
			options = new UserOptions(h);
		}
		public double paramAyus () 
		{
			return 120.0;
		}
		public void recalculateOptions ()
		{
			options.recalculate();
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 20;
				case Body.Name.Moon: return 1;
				case Body.Name.Mars: return 2;
				case Body.Name.Mercury: return 9;
				case Body.Name.Jupiter: return 18;
				case Body.Name.Venus: return 20;
				case Body.Name.Saturn: return 50;
				case Body.Name.Lagna: return 0;
			}
			Trace.Assert (false, "NaisargikaGrahaDasa::lengthOfDasa");
			return 0;
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (36);
			Body.Name[] order = new Body.Name[] 
				{
					Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
					Body.Name.Venus, Body.Name.Jupiter,	Body.Name.Sun,
					Body.Name.Saturn, Body.Name.Lagna
				};

			double cycle_start = paramAyus() * (double)cycle;
			double curr = 0.0;
			foreach (Body.Name bn in order) 
			{
				double dasaLength = lengthOfDasa (bn);
				al.Add (new DasaEntry (bn, cycle_start + curr, dasaLength, 1, Body.toShortString(bn)));
				curr += dasaLength;
			}
			
			return al;
		}
		private bool ExcludeGraha (DasaEntry pdi, Body.Name graha)
		{
			if (options.ExcludeDasaLord == true && 
				(graha == pdi.graha))
				return true;

			if (options.ExcludeNodes == true &&
				(graha == Body.Name.Rahu ||
				(graha == Body.Name.Ketu)))
				return true;
		
			int diff = 0;
			if (options.Exclude_3_10 || options.Exclude_2_6_11_12)
			{
				ZodiacHouse zhDasa = h.getPosition(pdi.graha).toDivisionPosition(options.dtype).zodiac_house;
				ZodiacHouse zhAntar = h.getPosition(graha).toDivisionPosition(options.dtype).zodiac_house;
				diff = zhDasa.numHousesBetween(zhAntar);
			}

			if (options.Exclude_3_10 == true &&	(diff == 3 || diff == 10))
				return true;

			if (options.Exclude_2_6_11_12 == true && 
				(diff == 2 || diff == 6 || diff == 11 || diff == 12))
				return true;

			return false;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			OrderedGrahas orderedAntar = new OrderedGrahas();
			ZodiacHouse lzh = h.getPosition(pdi.graha).toDivisionPosition(options.dtype).zodiac_house;
			int kendra_start = (int)Basics.normalize_exc_lower(0, 3,((int)lzh.value % 3));
			for (int i=kendra_start; i<=2; i++)
			{
				foreach (Body.Name b in this.options.GrahaStrengths[i].grahas)
					orderedAntar.grahas.Add(b);
			}
			for (int i=0; i<kendra_start; i++)
			{
				foreach (Body.Name b in this.options.GrahaStrengths[i].grahas)
					orderedAntar.grahas.Add(b);
			}

			int size = orderedAntar.grahas.Count;
			double[] antarLengths = new double[size];
			double totalAntarLengths = 0.0;
			ArrayList ret = new ArrayList(size-1);


			for (int i=0; i<size; i++)
			{

				if (this.ExcludeGraha(pdi, (Body.Name)orderedAntar.grahas[i]))
					continue;

				int diff = lzh.numHousesBetween(h.getPosition(
					(Body.Name)orderedAntar.grahas[i]).toDivisionPosition(options.dtype).zodiac_house);
				switch (diff)
				{
					case 7: 
						antarLengths[i] = 12.0; 
						totalAntarLengths += antarLengths[i];
						break;		
					case 1: 
						antarLengths[i] = 42.0; 
						totalAntarLengths += antarLengths[i];
						break;
					case 4:	case 8: 
						antarLengths[i] = 21.0;
						totalAntarLengths += antarLengths[i];
						break;
					case 5:	case 9: 
						antarLengths[i] = 28.0;
						totalAntarLengths += antarLengths[i];
						break;
					case 2: case 3: case 6: case 10: case 11: case 12:
						antarLengths[i] = 84.0;
						totalAntarLengths += antarLengths[i];
						break;
					default:
						Trace.Assert(false, "Naisargika Dasa Antardasa lengths Internal Error 1");
						break;
				}
			}
			double curr = pdi.startUT;
			for (int i=0; i<size; i++)
			{
				Body.Name bn = (Body.Name)orderedAntar.grahas[i];

				if (this.ExcludeGraha(pdi, bn))
					continue;

				double length = (antarLengths[i] / totalAntarLengths) * pdi.dasaLength;
				string desc = pdi.shortDesc + " " + Body.toShortString(bn);
				ret.Add(new DasaEntry(bn, curr, length, pdi.level+1, desc));
				curr += length;
			}
			return ret;
		}
		public String Description ()
		{
			return "Naisargika Graha Dasa (SR)";
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions newOpts = (UserOptions)a;
			this.options.CompareAndRecalculate(newOpts);
			this.options = newOpts;
			RecalculateEvent();
			return options.Clone();
		}

	}



	public class NaisargikaGrahaDasaSP: Dasa, IDasa
	{
		public class UserOptions :ICloneable
		{
			public UserOptions () 
			{
			}
			public Object Clone () 
			{
				UserOptions uo = new UserOptions();
				return uo;
			}
		}

		private Horoscope h;
		private UserOptions options;
		public NaisargikaGrahaDasaSP (Horoscope _h)
		{
			h = _h;
			options = new UserOptions();
		}
		public double paramAyus () 
		{
			return 108.0;
		}
		public void recalculateOptions ()
		{
		}
		public ArrayList Dasa(int cycle)
		{
			ArrayList al = new ArrayList (36);
			Body.Name[] order = new Body.Name[] 
				{
					Body.Name.Moon, Body.Name.Mercury, Body.Name.Mars,
					Body.Name.Venus, Body.Name.Jupiter,	Body.Name.Sun,
					Body.Name.Ketu,	Body.Name.Rahu,	Body.Name.Saturn };

			double cycle_start = paramAyus() * (double)cycle;
			double curr = 0.0;
			for (int i=0; i<3; i++) 
			{
				foreach (Body.Name bn in order) 
				{
					al.Add (new DasaEntry (bn, cycle_start + curr, 4.0, 1, bn.ToString()));
					curr += 4.0;
				}
			}
			return al;
		}
		public ArrayList AntarDasa (DasaEntry pdi) 
		{
			return new ArrayList();
		}
		public String Description ()
		{
			return "Naisargika Graha Dasa (SP)";
		}
		public Object GetOptions ()
		{
			return this.options.Clone();
		}
		public object SetOptions (Object a)
		{
			UserOptions uo = (UserOptions)a;
			if (RecalculateEvent != null)
				RecalculateEvent();
			return options.Clone();
		}

	}

	
	
	/// <summary>
	/// Base class to be implemented by Vimsottari/Ashtottari like dasas
	/// </summary>
	abstract public class NakshatraDasa: Dasa
	{
		abstract public Object GetOptions ();
		abstract public Object SetOptions (Object a);
		protected INakshatraDasa common;
		protected INakshatraTithiDasa tithiCommon;
		protected INakshatraYogaDasa yogaCommon;
		protected INakshatraKaranaDasa karanaCommon;

		/// <summary>
		/// Returns the antardasas
		/// </summary>
		/// <param name="pdi">The current dasa item whose antardasas should be calculated</param>
		/// <returns></returns>
		protected ArrayList _AntarDasa (DasaEntry pdi)
		{
			int numItems = common.numberOfDasaItems();
			ArrayList ditems = new ArrayList (numItems);
			DasaEntry curr = new DasaEntry (pdi.graha, pdi.startUT, 0, pdi.level + 1, "");
			for (int i=0; i<numItems; i++) 
			{
				double dlength = (common.lengthOfDasa (curr.graha) / common.paramAyus()) * pdi.dasaLength;
				string desc = pdi.shortDesc + " " + Body.toShortString (curr.graha);
				DasaEntry di = new DasaEntry (curr.graha, curr.startUT, dlength, curr.level, desc);
				ditems.Add (di);
				curr = common.nextDasaLord (di);
				curr.startUT = di.startUT + dlength;
			}
			return ditems;
		}

		/// <summary>
		/// Given a Lontigude, a Nakshatra Offset and Cycle number, calculate the maha dasa
		/// </summary>
		/// <param name="lon">The seed longitude (eg. Moon for Utpanna)</param>
		/// <param name="offset">The seed start (eg. 5 for Utpanna)</param>
		/// <param name="cycle">The cycle number. eg which 120 year cycle? 0 for "current"</param>
		/// <returns></returns>
		protected ArrayList _Dasa (Longitude lon, int offset, int cycle) 
		{
			ArrayList ditems = new ArrayList (common.numberOfDasaItems());
			Nakshatra n = (lon.toNakshatra()).add (offset);
			Body.Name g = common.lordOfNakshatra (n);
			double perc_traversed = lon.percentageOfNakshatra();
			double start = (cycle * common.paramAyus()) - (perc_traversed / 100.0 * common.lengthOfDasa(g));
			//System.Console.WriteLine ("{0} {1} {2}", common.lengthOfDasa(g), perc_traversed, start);

			// Calculate a "seed" dasaItem, to make use of the AntarDasa function
			DasaEntry di = new DasaEntry (g, start, common.paramAyus(), 0, "");
			return _AntarDasa (di);
		}

		protected ArrayList _TithiDasa (Longitude lon, int offset, int cycle)
		{
			//ArrayList ditems = new ArrayList(tithiCommon.numberOfDasaItems());
			lon = lon.add(new Longitude((cycle*(offset-1))*12.0));
			Body.Name g = tithiCommon.lordOfTithi(lon);

			double tithiOffset = lon.value;
			while (tithiOffset >= 12.0) tithiOffset -= 12.0;
			double perc_traversed = tithiOffset / 12.0;
			double start = (cycle * tithiCommon.paramAyus()) - (perc_traversed * tithiCommon.lengthOfDasa(g));
			DasaEntry di = new DasaEntry(g, start, tithiCommon.paramAyus(), 0, "");
			return _AntarDasa (di);
		}

		protected ArrayList _YogaDasa (Longitude lon, int offset, int cycle)
		{
			lon = lon.add(new Longitude((cycle*(offset-1))*(360.0/27.0)));
			Body.Name g = yogaCommon.lordOfYoga(lon);

			double yogaOffset = lon.toSunMoonYogaOffset();
			double perc_traversed = yogaOffset / (360.0/27.0);
			double start = (cycle * common.paramAyus()) - (perc_traversed * common.lengthOfDasa(g));
			DasaEntry di = new DasaEntry(g, start, common.paramAyus(), 0, "");
			return _AntarDasa (di);
		}

		protected ArrayList _KaranaDasa (Longitude lon, int offset, int cycle)
		{
			lon = lon.add(new Longitude((cycle*(offset-1))*(360.0/60.0)));
			Body.Name g = karanaCommon.lordOfKarana(lon);

			double karanaOffset = lon.toKaranaOffset();
			double perc_traversed = karanaOffset / (360.0/60.0);
			double start = (cycle * common.paramAyus()) - (perc_traversed * common.lengthOfDasa(g));
			DasaEntry di = new DasaEntry(g, start, common.paramAyus(), 0, "");
			return _AntarDasa (di);
		}
		
		public void recalculateOptions ()
		{
		}
	}

	public class VimsottariDasa : NakshatraDasa, INakshatraDasa
	{
		override public Object GetOptions ()
		{
			return this.options.Clone();
		}
		override public object SetOptions (Object a)
		{
			UserOptions uo = (UserOptions)a;
			bool bRecalculate = false;
			if (options.SeedBody != uo.SeedBody)
			{	
				options.SeedBody = uo.SeedBody;
				bRecalculate = true;
			}
			if (options.div != uo.div)
			{
				options.div = uo.div;
				bRecalculate = true;
			}

			if (bRecalculate == true && RecalculateEvent != null)
				RecalculateEvent();

			return options.Clone();
		}

		public ArrayList Dasa(int cycle)
		{
			return _Dasa (horoscope.getPosition(options.start_graha).extrapolateLongitude(options.div), options.nakshatra_offset, cycle);
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public class UserOptions : ICloneable
		{
			public Division div = new Division(Basics.DivisionType.Rasi);
			public Body.Name start_graha;
			public int nakshatra_offset;
			public StartBodyType user_start_graha;
			public Object Clone ()
			{
				UserOptions options = new UserOptions();
				options.start_graha = start_graha;
				options.nakshatra_offset = nakshatra_offset;
				options.SeedBody = SeedBody;
				options.div = (Division)div.Clone();
				return options;
			}

			[TypeConverter(typeof(EnumDescConverter))]
			public enum StartBodyType : int
			{ 
				[Description("Lagna's sphuta")]							Lagna, 
				[Description("Moon's sphuta")]							Moon, 
				[Description("Jupiter's sphuta")]						Jupiter,
				[Description("Utpanna - 5th tara from Moon's sphuta")]	Utpanna, 
				[Description("Kshema - 4th tara from Moon's sphuta")]	Kshema, 
				[Description("Adhana - 8th tara from Moon's sphuta")]	Aadhaana, 
				[Description("Mandi's sphuta")]							Maandi, 
				[Description("Gulika's sphuta")]						Gulika 
			}


			[PGDisplayName("Varga")]
			public Basics.DivisionType Varga
			{
				get { return this.div.MultipleDivisions[0].Varga ; }
				set { this.div = new Division(value); }
			}

			[PGDisplayName("Seed Nakshatra")]
			public StartBodyType SeedBody
			{
				get { return user_start_graha; }
				set 
				{
					user_start_graha = value;
					switch (value)
					{
						case StartBodyType.Lagna:
							start_graha = Body.Name.Lagna;
							nakshatra_offset = 1;
							break;
						case StartBodyType.Jupiter:
							start_graha = Body.Name.Jupiter;
							nakshatra_offset = 1;
							break;
						case StartBodyType.Moon:
							start_graha = Body.Name.Moon;
							nakshatra_offset = 1;
							break;
						case StartBodyType.Utpanna:
							start_graha = Body.Name.Moon;
							nakshatra_offset = 5;
							break;
						case StartBodyType.Kshema:
							start_graha = Body.Name.Moon;
							nakshatra_offset = 4;
							break;
						case StartBodyType.Aadhaana:
							start_graha = Body.Name.Moon;
							nakshatra_offset = 8;
							break;
						case StartBodyType.Maandi:
							start_graha = Body.Name.Maandi;
							nakshatra_offset = 1;
							break;
						case StartBodyType.Gulika:
							start_graha = Body.Name.Gulika;
							nakshatra_offset = 1;
							break;
					}
				}
			}
		}
		public UserOptions options;
		public Horoscope horoscope;
		public String Description ()
		{
			return ("Vimsottari Dasa Seeded from " + options.SeedBody.ToString());
		}
		public VimsottariDasa (Horoscope h)
		{
			common = this;
			options = new UserOptions();
			horoscope = h;

			FindStronger fs_graha = new FindStronger(h, new Division(Basics.DivisionType.BhavaPada), FindStronger.RulesVimsottariGraha(h));
			Body.Name stronger = fs_graha.StrongerGraha(Body.Name.Moon, Body.Name.Lagna, false);

			if (stronger == Body.Name.Lagna)
				options.SeedBody = UserOptions.StartBodyType.Lagna;
			else
				options.SeedBody = UserOptions.StartBodyType.Moon;
			h.Changed += new EvtChanged(ChangedHoroscope);
		}
		public void ChangedHoroscope (Object o)
		{
			Horoscope h = (Horoscope) o;
			OnChanged();
		}
		public double paramAyus ()
		{
			return 120.0;
		}
		public int numberOfDasaItems ()
		{
			return 9;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Rahu;
				case Body.Name.Rahu : return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Saturn;
				case Body.Name.Saturn: return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Ketu;
				case Body.Name.Ketu : return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Sun;
			}
			Trace.Assert (false, "VimsottariDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			return VimsottariDasa.LengthOfDasa(plt);
		}
		public static double LengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 6;
				case Body.Name.Moon: return 10;
				case Body.Name.Mars: return 7;
				case Body.Name.Rahu: return 18;
				case Body.Name.Jupiter: return 16;
				case Body.Name.Saturn: return 19;
				case Body.Name.Mercury: return 17;
				case Body.Name.Ketu: return 7;
				case Body.Name.Venus: return 20;
			}
			Trace.Assert (false, "Vimsottari::lengthOfDasa");
			return 0;
		}
		
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			return VimsottariDasa.LordOfNakshatra(n);
		}
		public static Body.Name LordOfNakshatra (Nakshatra n)
		{
			Body.Name[] lords = new Body.Name[9] 
				{
					Body.Name.Mercury, 
					Body.Name.Ketu, Body.Name.Venus, Body.Name.Sun,
					Body.Name.Moon, Body.Name.Mars, Body.Name.Rahu,
					Body.Name.Jupiter, Body.Name.Saturn
				};
			int nak_val = ((int)n.value) % 9;
			return lords[nak_val];
		}
		new public void DivisionChanged (Division div)
		{
			UserOptions uoNew = (UserOptions)this.options.Clone();
			uoNew.div = (Division)div.Clone();
			this.SetOptions(uoNew);
		}
	}


	// Wrapper around ChaturashitiSamaDasa
	public class KaranaChaturashitiSamaDasa : NakshatraDasa, INakshatraDasa, INakshatraKaranaDasa
	{
		private Horoscope h;
		ChaturashitiSamaDasa cd;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			Longitude mMoon = h.getPosition(Body.Name.Moon).longitude;
			Longitude mSun = h.getPosition(Body.Name.Sun).longitude;
			return _KaranaDasa (mMoon.sub(mSun), 1, cycle);
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Karana Chaturashiti-Sama Dasa");
		}
		public KaranaChaturashitiSamaDasa (Horoscope _h)
		{
			common = this;
			karanaCommon = this;
			h = _h;
			cd = new ChaturashitiSamaDasa(h);
		}

		public double paramAyus ()
		{
			return cd.paramAyus();
		}
		public int numberOfDasaItems ()
		{
			return cd.numberOfDasaItems();
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return cd.nextDasaLord(di);
		}
		public double lengthOfDasa (Body.Name plt)
		{
			return cd.lengthOfDasa(plt);

		}
		public Body.Name lordOfNakshatra (Nakshatra n)
		{
			return cd.lordOfNakshatra(n);
		}
		public Body.Name lordOfKarana(Longitude l) 
		{
			return l.toKarana().getLord();
		}
	}

	// Wrapper around ashtottari dasa that starts the initial dasa
	// based on the tithi. We do not reimplement ashtottari dasa 
	// semantics here.
	public class TithiAshtottariDasa : NakshatraDasa, INakshatraDasa, INakshatraTithiDasa
	{
		private Horoscope h;
		private AshtottariDasa ad;
		private UserOptions options;

		public class UserOptions : ICloneable
		{
			public int mTithiOffset = 1;
			public bool bExpungeTravelled = true;
			public UserOptions ()
			{
				this.mTithiOffset = 1;
				this.bExpungeTravelled = true;
			}
			public Object Clone ()
			{
				UserOptions options = new UserOptions ();
				options.mTithiOffset = this.mTithiOffset;
				options.bExpungeTravelled = this.bExpungeTravelled;
				return options;
			}
			public Object SetOptions (Object b)
			{
				if (b is UserOptions)
				{
					UserOptions uo = (UserOptions) b;
					this.mTithiOffset = uo.mTithiOffset;
					this.bExpungeTravelled = uo.bExpungeTravelled;
				}
				return this.Clone();
			}

			[PGNotVisible]
			public bool UseTithiRemainder 
			{
				get { return this.bExpungeTravelled; }
				set { this.bExpungeTravelled = value; }
			}
			public int TithiOffset
			{
				get { return this.mTithiOffset; }
				set 
				{
					if (value >= 1 && value <= 30)
						this.mTithiOffset = value;
				}
			}
		}

		override public Object GetOptions ()
		{
			return this.options.Clone();
		}
		override public object SetOptions (Object a)
		{
			this.options = (UserOptions)this.options.SetOptions(a);
			if (this.RecalculateEvent != null)
				this.RecalculateEvent();
			return this.options.Clone();
		}
		public ArrayList Dasa(int cycle)
		{
			
			Longitude mpos = h.getPosition(Body.Name.Moon).longitude;
			Longitude spos = h.getPosition(Body.Name.Sun).longitude;

			Longitude tithi = mpos.sub(spos);
			if (options.UseTithiRemainder == false)
			{
				double offset = tithi.value;
				while (offset >= 12.0) offset -= 12.0;
				tithi = tithi.sub (new Longitude(offset));
			}
			return _TithiDasa(tithi, options.TithiOffset, cycle);
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return String.Format("({0}) Tithi Ashtottari Dasa", this.options.TithiOffset);
			
		}
		public TithiAshtottariDasa (Horoscope _h)
		{
			this.options = new UserOptions();
			tithiCommon = this;
			common = this;
			h = _h;
			ad = new AshtottariDasa(h);
		}

		public double paramAyus ()
		{
			return ad.paramAyus();
		}
		public int numberOfDasaItems ()
		{
			return ad.numberOfDasaItems();
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return ad.nextDasaLord(di);
		}

		public double lengthOfDasa (Body.Name plt)
		{
			return ad.lengthOfDasa(plt);

		}
		public Body.Name lordOfNakshatra (Nakshatra n)
		{
			Debug.Assert(false, "TithiAshtottari::lordOfNakshatra");
			return Body.Name.Sun;
		}
		public Body.Name lordOfTithi (Longitude l)
		{
			return l.toTithi().getLord();
		}
	}


	

	// Wrapper around vimsottari dasa that starts the initial dasa
	// based on the yoga
	public class YogaVimsottariDasa : NakshatraDasa, INakshatraDasa, INakshatraYogaDasa
	{
		private Horoscope h;
		private VimsottariDasa vd;
		private UserOptions options;

		public class UserOptions : ICloneable
		{
			public bool bExpungeTravelled = true;
			public UserOptions ()
			{
				this.bExpungeTravelled = true;
			}
			public Object Clone ()
			{
				UserOptions options = new UserOptions ();
				options.bExpungeTravelled = this.bExpungeTravelled;
				return options;
			}
			public Object SetOptions (Object b)
			{
				if (b is UserOptions)
				{
					UserOptions uo = (UserOptions) b;
					this.bExpungeTravelled = uo.bExpungeTravelled;
				}
				return this.Clone();
			}

			[PGNotVisible]
			public bool UseYogaRemainder
			{
				get { return this.bExpungeTravelled; }
				set { this.bExpungeTravelled = value; }
			}
		}

		override public Object GetOptions ()
		{
			return this.options.Clone();
		}
		override public object SetOptions (Object a)
		{
			this.options = (UserOptions)this.options.SetOptions(a);
			if (this.RecalculateEvent != null)
				this.RecalculateEvent();
			return this.options.Clone();
		}
		public ArrayList Dasa(int cycle)
		{
			Transit t = new Transit(h);
			Longitude l = t.LongitudeOfSunMoonYoga(h.baseUT);
			return this._YogaDasa(l, 1, cycle);
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return String.Format("Yoga Vimsottari Dasa");
			
		}
		public Body.Name lordOfYoga (Longitude l)
		{
			return l.toSunMoonYoga().getLord();
		}
		public YogaVimsottariDasa (Horoscope _h)
		{
			this.options = new UserOptions();
			common = this;
			yogaCommon = this;
			h = _h;
			vd = new VimsottariDasa(h);
		}

		public double paramAyus ()
		{
			return vd.paramAyus();
		}
		public int numberOfDasaItems ()
		{
			return vd.numberOfDasaItems();
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return vd.nextDasaLord(di);
		}

		public double lengthOfDasa (Body.Name plt)
		{
			return vd.lengthOfDasa(plt);

		}
		public Body.Name lordOfNakshatra (Nakshatra n)
		{
			throw new Exception();
			return Body.Name.Lagna;
		}
	}

	
	
	public class YoginiDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Yogini Dasa");
		}
		public YoginiDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 36.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Moon: return Body.Name.Sun;
				case Body.Name.Sun: return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Saturn;
				case Body.Name.Saturn: return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Rahu;
				case Body.Name.Rahu : return Body.Name.Moon;
			}
			Trace.Assert (false, "YoginiDasa::nextDasaLord");
			return Body.Name.Sun;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Moon: return 1;
				case Body.Name.Sun: return 2;
				case Body.Name.Jupiter: return 3;
				case Body.Name.Mars: return 4;
				case Body.Name.Mercury: return 5;
				case Body.Name.Saturn: return 6;
				case Body.Name.Venus: return 7;
				case Body.Name.Rahu: return 8;
			}
			Trace.Assert (false, "YoginiDasa::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[]
			{
				Body.Name.Moon, Body.Name.Sun, Body.Name.Jupiter, Body.Name.Mars,
				Body.Name.Mercury, Body.Name.Saturn, Body.Name.Venus, Body.Name.Rahu
			};

			int index = ((int)n.value+3)%8;
			if (index == 0) index =  8;
			index--;
			return (Body.Name)(lords[index]);
		}
	}



	public class AshtottariDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Ashtottari Dasa");
		}
		public AshtottariDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 108.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Jupiter;
				case Body.Name.Jupiter: return Body.Name.Rahu;
				case Body.Name.Rahu : return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Sun;
			}
			Trace.Assert (false, "AshtottariDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 6;
				case Body.Name.Moon: return 15;
				case Body.Name.Mars: return 8;
				case Body.Name.Mercury: return 17;
				case Body.Name.Saturn: return 10;
				case Body.Name.Jupiter: return 19;
				case Body.Name.Rahu: return 12;
				case Body.Name.Venus: return 21;
			}
			Trace.Assert (false, "Ashtottari::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			switch (n.value)
			{
				case Nakshatra.Name.Aswini : return Body.Name.Rahu ;
				case Nakshatra.Name.Bharani : return Body.Name.Rahu ;
				case Nakshatra.Name.Krittika : return Body.Name.Venus ;
				case Nakshatra.Name.Rohini : return Body.Name.Venus ;
				case Nakshatra.Name.Mrigarirsa : return Body.Name.Venus ;
				case Nakshatra.Name.Aridra : return Body.Name.Sun ;
				case Nakshatra.Name.Punarvasu : return Body.Name.Sun ;
				case Nakshatra.Name.Pushya : return Body.Name.Sun ;
				case Nakshatra.Name.Aslesha : return Body.Name.Sun ;
				case Nakshatra.Name.Makha : return Body.Name.Moon ;
				case Nakshatra.Name.PoorvaPhalguni : return Body.Name.Moon ;
				case Nakshatra.Name.UttaraPhalguni : return Body.Name.Moon ;
				case Nakshatra.Name.Hasta : return Body.Name.Mars ;
				case Nakshatra.Name.Chittra : return Body.Name.Mars ;
				case Nakshatra.Name.Swati : return Body.Name.Mars ;
				case Nakshatra.Name.Vishaka : return Body.Name.Mars ;
				case Nakshatra.Name.Anuradha : return Body.Name.Mercury ;
				case Nakshatra.Name.Jyestha : return Body.Name.Mercury ;
				case Nakshatra.Name.Moola : return Body.Name.Mercury ;
				case Nakshatra.Name.PoorvaShada : return Body.Name.Saturn ;
				case Nakshatra.Name.UttaraShada : return Body.Name.Saturn ;
				case Nakshatra.Name.Sravana : return Body.Name.Saturn ;
				case Nakshatra.Name.Dhanishta : return Body.Name.Jupiter ;
				case Nakshatra.Name.Satabisha : return Body.Name.Jupiter ;
				case Nakshatra.Name.PoorvaBhadra : return Body.Name.Jupiter ;
				case Nakshatra.Name.UttaraBhadra : return Body.Name.Rahu ;
				case Nakshatra.Name.Revati : return Body.Name.Rahu ;
			}
			Trace.Assert (false, "AshtottariDasa::LordOfNakshatra");
			return Body.Name.Lagna;
		}
	}

	public class ShodashottariDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Shodashottari Dasa");
		}
		public ShodashottariDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 116.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Mars;
				case Body.Name.Mars: return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Ketu;
				case Body.Name.Ketu : return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Sun;
			}
			Trace.Assert (false, "ShodashottariDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 11;
				case Body.Name.Mars: return 12;
				case Body.Name.Jupiter: return 13;
				case Body.Name.Saturn: return 14;
				case Body.Name.Ketu: return 15;
				case Body.Name.Moon: return 16;
				case Body.Name.Mercury: return 17;
				case Body.Name.Venus: return 18;
			}
			Trace.Assert (false, "Shodashottari::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[8] 
			{
				Body.Name.Sun, Body.Name.Mars, Body.Name.Jupiter,
				Body.Name.Saturn, Body.Name.Ketu, Body.Name.Moon,
				Body.Name.Mercury, Body.Name.Venus
			};				
			int nak_val = ((int)n.value);
			int pus_val = (int)Nakshatra.Name.Pushya;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - pus_val);
			int diff_off = diff_val % 8;
			return lords[diff_off];
		}
	}
	public class DwadashottariDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Dwadashottari Dasa");
		}
		public DwadashottariDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 112.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Jupiter;
				case Body.Name.Jupiter: return Body.Name.Ketu;
				case Body.Name.Ketu : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Rahu;
				case Body.Name.Rahu : return Body.Name.Mars;
				case Body.Name.Mars: return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Moon;
				case Body.Name.Moon : return Body.Name.Sun;
			}
			Trace.Assert (false, "DwadashottariDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 7;
				case Body.Name.Jupiter: return 9;
				case Body.Name.Ketu: return 11;
				case Body.Name.Mercury: return 13;
				case Body.Name.Rahu: return 15;
				case Body.Name.Mars: return 17;
				case Body.Name.Saturn: return 19;
				case Body.Name.Moon: return 21;
			}
			Trace.Assert (false, "Dwadashottari::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[8] 
			{
				Body.Name.Sun, Body.Name.Jupiter, Body.Name.Ketu,
				Body.Name.Mercury, Body.Name.Rahu, Body.Name.Mars,
				Body.Name.Saturn, Body.Name.Moon
			};				
			int nak_val = ((int)n.value);
			int rev_val = (int)Nakshatra.Name.Revati;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				rev_val - nak_val);
			int diff_off = diff_val % 8;
			return lords[diff_off];
		}
	}
	public class PanchottariDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Panchottari Dasa");
		}
		public PanchottariDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 105.0;
		}
		public int numberOfDasaItems ()
		{
			return 7;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Mercury;
				case Body.Name.Mercury: return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Sun;
			}
			Trace.Assert (false, "DwadashottariDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 12;
				case Body.Name.Mercury: return 13;
				case Body.Name.Saturn: return 14;
				case Body.Name.Mars: return 15;
				case Body.Name.Venus: return 16;
				case Body.Name.Moon: return 17;
				case Body.Name.Jupiter: return 18;
			}
			Trace.Assert (false, "Panchottari::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[7] 
			{
				Body.Name.Sun, Body.Name.Mercury, Body.Name.Saturn,
				Body.Name.Mars, Body.Name.Venus, Body.Name.Moon,
				Body.Name.Jupiter
			};				
			int nak_val = ((int)n.value);
			int anu_val = (int)Nakshatra.Name.Anuradha;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - anu_val);
			int diff_off = diff_val % 7;
			return lords[diff_off];
		}
	}
	public class ShatabdikaDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Shatabdika Dasa");
		}
		public ShatabdikaDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 100.0;
		}
		public int numberOfDasaItems ()
		{
			return 7;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Mars;
				case Body.Name.Mars: return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Sun;
			}
			Trace.Assert (false, "ShatabdikaDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 5;
				case Body.Name.Moon: return 5;
				case Body.Name.Venus: return 10;
				case Body.Name.Mercury: return 10;
				case Body.Name.Jupiter: return 20;
				case Body.Name.Mars: return 20;
				case Body.Name.Saturn: return 30;
			}
			Trace.Assert (false, "ShatabdikaDasa::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[7] 
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Venus,
				Body.Name.Mercury, Body.Name.Jupiter, Body.Name.Mars,
				Body.Name.Saturn
			};				
			int nak_val = ((int)n.value);
			int rev_val = (int)Nakshatra.Name.Revati;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - rev_val);
			int diff_off = diff_val % 7;
			return lords[diff_off];
		}
	}

	public class ChaturashitiSamaDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Chaturashiti-Sama Dasa");
		}
		public ChaturashitiSamaDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 84.0;
		}
		public int numberOfDasaItems ()
		{
			return 7;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Venus;
				case Body.Name.Venus: return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Sun;
			}
			Trace.Assert (false, "Chaturashiti Sama Dasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 12;
				case Body.Name.Moon: return 12;
				case Body.Name.Mars: return 12;
				case Body.Name.Mercury: return 12;
				case Body.Name.Jupiter: return 12;
				case Body.Name.Venus: return 12;
				case Body.Name.Saturn: return 12;
			}
			Trace.Assert (false, "ChaturashitiSama Dasa::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[7] 
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars,
				Body.Name.Mercury, Body.Name.Jupiter, Body.Name.Venus,
				Body.Name.Saturn
			};				
			int nak_val = ((int)n.value);
			int sva_val = (int)Nakshatra.Name.Swati;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - sva_val);
			int diff_off = diff_val % 7;
			return lords[diff_off];
		}
	}
	public class DwisaptatiSamaDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("Dwisaptati Sama Dasa");
		}
		public DwisaptatiSamaDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 72.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Body.Name.Moon;
				case Body.Name.Moon: return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Venus;
				case Body.Name.Venus: return Body.Name.Saturn;
				case Body.Name.Saturn : return Body.Name.Rahu;
				case Body.Name.Rahu: return Body.Name.Sun;
			}
			Trace.Assert (false, "DwisaptatiSamaDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Sun: return 9;
				case Body.Name.Moon: return 9;
				case Body.Name.Mars: return 9;
				case Body.Name.Mercury: return 9;
				case Body.Name.Jupiter: return 9;
				case Body.Name.Venus: return 9;
				case Body.Name.Saturn: return 9;
				case Body.Name.Rahu: return 9;
			}
			Trace.Assert (false, "DwisaptatiSamaDasa::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[8] 
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars,
				Body.Name.Mercury, Body.Name.Jupiter, Body.Name.Venus,
				Body.Name.Saturn, Body.Name.Rahu
			};				
			int nak_val = ((int)n.value);
			int moo_val = (int)Nakshatra.Name.Moola;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - moo_val);
			int diff_off = diff_val % 8;
			return lords[diff_off];
		}
	}
	public class ShatTrimshaSamaDasa : NakshatraDasa, INakshatraDasa
	{
		private Horoscope h;
		override public Object GetOptions ()
		{
			return new Object();
		}
		override public object SetOptions (Object a)
		{
			return new object();
		}
		public ArrayList Dasa(int cycle)
		{
			return _Dasa (h.getPosition(Body.Name.Moon).longitude, 1, cycle );
		}
		public ArrayList AntarDasa (DasaEntry di)
		{
			return _AntarDasa (di);
		}
		public String Description ()
		{
			return ("ShatTrimsha Sama Dasa");
		}
		public ShatTrimshaSamaDasa (Horoscope _h)
		{
			common = this;
			h = _h;
		}

		public double paramAyus ()
		{
			return 36.0;
		}
		public int numberOfDasaItems ()
		{
			return 8;
		}
		public DasaEntry nextDasaLord (DasaEntry di) 
		{
			return new DasaEntry (nextDasaLordHelper(di.graha), 0, 0, di.level, "");
		}
		private Body.Name nextDasaLordHelper (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Moon: return Body.Name.Sun;
				case Body.Name.Sun: return Body.Name.Jupiter;
				case Body.Name.Jupiter : return Body.Name.Mars;
				case Body.Name.Mars : return Body.Name.Mercury;
				case Body.Name.Mercury : return Body.Name.Saturn;
				case Body.Name.Saturn: return Body.Name.Venus;
				case Body.Name.Venus : return Body.Name.Rahu;
				case Body.Name.Rahu: return Body.Name.Moon;
			}
			Trace.Assert (false, "ShatTrimshaSamaDasa::nextDasaLord");
			return Body.Name.Lagna;
		}
		public double lengthOfDasa (Body.Name plt)
		{
			switch (plt)
			{
				case Body.Name.Moon: return 1;
				case Body.Name.Sun: return 2;
				case Body.Name.Jupiter: return 3;
				case Body.Name.Mars: return 4;
				case Body.Name.Mercury: return 5;
				case Body.Name.Saturn: return 6;
				case Body.Name.Venus: return 7;
				case Body.Name.Rahu: return 8;
			}
			Trace.Assert (false, "ShatTrimshaSamaDasa::lengthOfDasa");
			return 0;
		}
		public Body.Name lordOfNakshatra(Nakshatra n) 
		{
			Body.Name[] lords = new Body.Name[8] 
			{
				Body.Name.Moon, Body.Name.Sun, Body.Name.Jupiter,
				Body.Name.Mars, Body.Name.Mercury, Body.Name.Saturn,
				Body.Name.Venus, Body.Name.Rahu
			};				
			int nak_val = ((int)n.value);
			int shr_val = (int)Nakshatra.Name.Sravana;
			int diff_val = Basics.normalize_inc(
				(int)Nakshatra.Name.Aswini, (int)Nakshatra.Name.Revati, 
				nak_val - shr_val);
			int diff_off = diff_val % 8;
			return lords[diff_off];
		}
	}
}
