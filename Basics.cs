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
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace mhora
{
	public class PGDisplayName : Attribute
	{
		public string DisplayName;
		public PGDisplayName(string _display)
		{
			this.DisplayName=_display;
		}
	}
	public class PGNotVisible: Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyOrderAttribute : Attribute
	{
		//
		// Simple attribute to allow the order of a property to be specified
		//
		private int _order;
		public PropertyOrderAttribute(int order)
		{
			_order = order;
		}

		public int Order
		{
			get
			{
				return _order;
			}
		}
	}

	public delegate void EvtChanged (Object h);
	public delegate void SetOptionsDelegate (Object sender);
	public delegate void Recalculate ();

	/// <summary>
	/// An interface which should be used by those whose properties
	/// should be updateable using the mhoraOptions form. 
	/// </summary>
	public interface IUpdateable
	{
		Object GetOptions();
		Object SetOptions (Object a);
	}

	/// <summary>
	/// Interface will return a HoraInfo object specifying all birth
	/// time information required for a single chart.
	/// </summary>
	public interface IFileToHoraInfo
	{
		HoraInfo toHoraInfo ();
		void ToFile (HoraInfo hi);
	}

	/// <summary>
	/// Class deals with reading the Jhd file specification
	/// used by Jagannatha Hora
	/// </summary>
	/// 

	public class Mhd : IFileToHoraInfo
	{
		private string fname;
		public Mhd (string fileName)
		{
			fname = fileName;
		}
		public HoraInfo toHoraInfo ()
		{
			try 
			{
				HoraInfo hi = new HoraInfo();
				FileStream sOut;
				sOut = new FileStream(fname, FileMode.Open, FileAccess.Read);
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
				hi = (HoraInfo)formatter.Deserialize(sOut);
				sOut.Close();
				return hi;
			}
			catch 
			{
				MessageBox.Show("Unable to read file");
				return new HoraInfo();
			}		
		}

		public void ToFile (HoraInfo hi)
		{
			FileStream sOut = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.Write);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize (sOut, hi);
			sOut.Close();
		}


	}

	public class Jhd : IFileToHoraInfo
	{
		private string fname;
		public Jhd (string fileName)
		{
			fname = fileName;
		}
		private static int readIntLine (StreamReader sr)
		{
			String s = sr.ReadLine();
			return int.Parse(s);
		}
		private static void writeHMSInfoLine (StreamWriter sw, HMSInfo hi)
		{
			string q;
			if (hi.direction == HMSInfo.dir_type.NS && hi.degree >= 0)
				q = "";
			else if (hi.direction == HMSInfo.dir_type.NS)
				q = "-";
			else if (hi.direction == HMSInfo.dir_type.EW && hi.degree >= 0)
				q = "-";
			else q = "";
			int thour = hi.degree >= 0 ? hi.degree : -hi.degree;
			string w = q + thour.ToString() + "." + numToString(hi.minute) + numToString (hi.second) + "00";			
			sw.WriteLine(w);
		}
		private static HMSInfo readHmsLineInfo (StreamReader sr, bool negate, HMSInfo.dir_type dir)
		{
			int h=0, m=0, s=0;
			readHmsLine (sr, ref h, ref m, ref s);
			if (negate) h *= -1;
			return new HMSInfo(h, m, s, dir);
		}
		private static void readHmsLine (StreamReader sr, ref int hour, ref int minute, ref int second)
		{
			String s = sr.ReadLine();
			Regex re = new Regex("[0-9]*$");
			Match m = re.Match (s);
			String s2 = m.Value;

			if (s[0] == '|') s = new string(s.ToCharArray(1, s.Length-1));
			double dhour = double.Parse (s);
			dhour = dhour < 0 ? Math.Ceiling (dhour) : Math.Floor (dhour);
			hour = (int)dhour;
			minute = int.Parse(s2.Substring(0, 2));
			double _second = 0.0;
			if (s2.Length > 5)
				_second = (double.Parse(s2.Substring(2,4)) / 10000.0) * 60.0;
			second = (int)_second;
		}
		private static Moment readMomentLine (StreamReader sr)
		{
			int month = readIntLine (sr);
			int day = readIntLine (sr);
			int year = readIntLine (sr);

			int hour=0, minute=0, second=0;
			readHmsLine (sr, ref hour, ref minute, ref second);
			return new Moment (year, month, day, hour, minute, second);
		}
		private static string numToString (int _n)
		{
			int n = _n < 0 ? -_n : _n;
			string s;
			if (n < 10) s = "0" + n.ToString();
			else s = n.ToString();
			return s;
		}
		private static void writeMomentLine (StreamWriter sw, Moment m)
		{
			sw.WriteLine (m.month);
			sw.WriteLine (m.day);
			sw.WriteLine (m.year);

			sw.WriteLine (m.hour.ToString() + "." + numToString(m.minute) + numToString(m.second) + "00");
		}
		public HoraInfo toHoraInfo ()
		{
			StreamReader sr = File.OpenText (fname);
			Moment m = readMomentLine (sr);
			HMSInfo tz = readHmsLineInfo (sr, true, HMSInfo.dir_type.EW);
			HMSInfo lon = readHmsLineInfo (sr, true, HMSInfo.dir_type.EW);
			HMSInfo lat = readHmsLineInfo (sr, false, HMSInfo.dir_type.NS);
			HoraInfo hi = new HoraInfo(m, lat, lon, tz);
			hi.FileType = HoraInfo.EFileType.JagannathaHora;
			//hi.name = File.fname;
			return hi;
		}
		public void ToFile (HoraInfo h)
		{
			StreamWriter sw = new StreamWriter(fname, false);
			writeMomentLine (sw, h.tob);
			writeHMSInfoLine (sw, h.tz);
			writeHMSInfoLine (sw, h.lon);
			writeHMSInfoLine (sw, h.lat);
			sw.WriteLine ("0.000000");
			sw.Flush();
			sw.Close();
		}
	}

	/// <summary>
	/// Simple functions that don't belong anywhere else
	/// </summary>
	public class Basics
	{
		/// <summary>
		/// Normalize a number between bounds
		/// </summary>
		/// <param name="lower">The lower bound of normalization</param>
		/// <param name="upper">The upper bound of normalization</param>
		/// <param name="x">The value to be normalized</param>
		/// <returns>The normalized value of x, where lower <= x <= upper </returns>
		public static int normalize_inc (int lower, int upper, int x) 
		{
			int size = upper - lower + 1;
			while (x > upper) x -= size;
			while (x < lower) x += size;
			Trace.Assert (x >= lower && x <= upper, "Basics.normalize failed");
			return x;
		}

		/// <summary>
		/// Normalize a number between bounds
		/// </summary>
		/// <param name="lower">The lower bound of normalization</param>
		/// <param name="upper">The upper bound of normalization</param>
		/// <param name="x">The value to be normalized</param>
		/// <returns>The normalized value of x, where lower = x <= upper </returns>
		public static double normalize_exc (double lower, double upper, double x) 
		{
			double size = upper - lower;
			while (x > upper) x -= size;
			while (x <= lower) x += size;
			Trace.Assert (x >= lower && x <= upper, "Basics.normalize failed");
			return x;
		}

		public static double normalize_exc_lower (double lower, double upper, double x) 
		{
			double size = upper - lower;
			while (x >= upper) x -= size;
			while (x < lower) x += size;
			Trace.Assert (x >= lower && x <= upper, "Basics.normalize failed");
			return x;
		}

		public static ZodiacHouse getMoolaTrikonaRasi (Body.Name b)
		{
			ZodiacHouse.Name z = ZodiacHouse.Name.Ari;
			switch (b)
			{
				case Body.Name.Sun: z = ZodiacHouse.Name.Leo; break;
				case Body.Name.Moon: z = ZodiacHouse.Name.Tau; break;
				case Body.Name.Mars: z = ZodiacHouse.Name.Ari; break;
				case Body.Name.Mercury: z = ZodiacHouse.Name.Vir; break;
				case Body.Name.Jupiter: z = ZodiacHouse.Name.Sag; break;
				case Body.Name.Venus: z = ZodiacHouse.Name.Lib; break;
				case Body.Name.Saturn: z = ZodiacHouse.Name.Aqu; break;
				case Body.Name.Rahu: z = ZodiacHouse.Name.Vir; break;
				case Body.Name.Ketu: z = ZodiacHouse.Name.Pis; break;
			}
			return new ZodiacHouse (z);
		}
		public static Weekday bodyToWeekday (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return Weekday.Sunday;
				case Body.Name.Moon: return Weekday.Monday;
				case Body.Name.Mars: return Weekday.Tuesday;
				case Body.Name.Mercury: return Weekday.Wednesday;
				case Body.Name.Jupiter: return Weekday.Thursday;
				case Body.Name.Venus: return Weekday.Friday;
				case Body.Name.Saturn: return Weekday.Saturday;
			}
			Debug.Assert(false, string.Format("bodyToWeekday({0})", b));
			throw new Exception();
		}
		public static Body.Name weekdayRuler (Weekday w)
		{
			switch (w)
			{
				case Weekday.Sunday: return Body.Name.Sun;
				case Weekday.Monday: return Body.Name.Moon;
				case Weekday.Tuesday: return Body.Name.Mars;
				case Weekday.Wednesday: return Body.Name.Mercury;
				case Weekday.Thursday: return Body.Name.Jupiter;
				case Weekday.Friday: return Body.Name.Venus;
				case Weekday.Saturday: return Body.Name.Saturn;
				default:
					Debug.Assert(false, "Basics::weekdayRuler");
					return Body.Name.Sun;
			}
		}

		// This matches the sweph definitions for easy conversion
		public enum Weekday : int
		{
			Monday=0, Tuesday=1, Wednesday=2, Thursday=3, Friday=4, Saturday=5, Sunday=6
		}

		public static string weekdayToShortString (Weekday w)
		{
			switch (w)
			{
				case Weekday.Sunday: return "Su";
				case Weekday.Monday: return "Mo";
				case Weekday.Tuesday: return "Tu";
				case Weekday.Wednesday: return "We";
				case Weekday.Thursday: return "Th";
				case Weekday.Friday: return "Fr";
				case Weekday.Saturday: return "Sa";
			}
			return "";
		}
		/// <summary>
		/// Enumeration of the various division types. Where a varga has multiple
		/// definitions, each of these should be specified separately below
		/// </summary>
		[TypeConverter(typeof(EnumDescConverter))]
		public enum DivisionType : int
		{
			[Description("D-1: Rasi")]						Rasi=0, 
			[Description("D-9: Navamsa")]					Navamsa,
			[Description("D-2: Hora (Parashara)")]			HoraParasara, 
			[Description("D-2: Hora (Jagannatha)")]			HoraJagannath, 
			[Description("D-2: Hora (Parivritti)")]			HoraParivrittiDwaya, 
			[Description("D-2: Hora (Kashinatha)")]			HoraKashinath,
			[Description("D-3: Dreshkana (Parashara)")]		DrekkanaParasara, 
			[Description("D-3: Dreshkana (Jagannatha)")]	DrekkanaJagannath, 
			[Description("D-3: Dreshkana (Somnatha)")]		DrekkanaSomnath, 
			[Description("D-3: Dreshkana (Parivritti)")]	DrekkanaParivrittitraya,
			[Description("D-4: Chaturthamsa")]				Chaturthamsa, 
			[Description("D-5: Panchamsa")]					Panchamsa, 
			[Description("D-6: Shashtamsa")]				Shashthamsa, 
			[Description("D-7: Saptamsa")]					Saptamsa, 
			[Description("D-8: Ashtamsa")]					Ashtamsa, 
			[Description("D-8: Ashtamsa (Raman)")]			AshtamsaRaman,
			[Description("D-10: Dasamsa")]					Dasamsa,
			[Description("D-11: Rudramsa (Rath)")]			Rudramsa, 
			[Description("D-11: Rudramsa (Raman)")]			RudramsaRaman,
			[Description("D-12: Dwadasamsa")]				Dwadasamsa, 
			[Description("D-16: Shodasamsa")]				Shodasamsa, 
			[Description("D-20: Vimsamsa")]					Vimsamsa, 
			[Description("D-24: Chaturvimsamsa")]			Chaturvimsamsa, 
			[Description("D-27: Nakshatramsa")]				Nakshatramsa,
			[Description("D-30: Trimsamsa (Parashara)")]	Trimsamsa, 
			[Description("D-30: Trimsamsa (Parivritti)")]	TrimsamsaParivritti, 
			[Description("D-30: Trimsamsa (Simple)")]		TrimsamsaSimple,
			[Description("D-40: Khavedamsa")]				Khavedamsa, 
			[Description("D-45: Akshavedamsa")]				Akshavedamsa, 
			[Description("D-60: Shashtyamsa")]				Shashtyamsa,
			[Description("D-108: Ashtottaramsa (Regular)")]			Ashtottaramsa, 
			[Description("D-150: Nadiamsa (Equal Division)")]		Nadiamsa, 
			[Description("D-150: Nadiamsa (Chandra Kala Nadi)")]	NadiamsaCKN,
			[Description("D-9-12: Navamsa-Dwadasamsa (Composite)")]		NavamsaDwadasamsa, 
			[Description("D-12-12: Dwadasamsa-Dwadasamsa (Composite)")]	DwadasamsaDwadasamsa,
			[Description("D-1: Bhava (9 Padas)")]			BhavaPada, 
			[Description("D-1: Bhava (Equal Length)")]		BhavaEqual, 
			[Description("D-1: Bhava (Alcabitus)")]			BhavaAlcabitus, 
			[Description("D-1: Bhava (Axial)")]				BhavaAxial, 
			[Description("D-1: Bhava (Campanus)")]			BhavaCampanus, 
			[Description("D-1: Bhava (Koch)")]				BhavaKoch, 
			[Description("D-1: Bhava (Placidus)")]			BhavaPlacidus, 
			[Description("D-1: Bhava (Regiomontanus)")]		BhavaRegiomontanus, 
			[Description("D-1: Bhava (Sripati)")]			BhavaSripati,
			[Description("Regular: Parivritti")]			GenericParivritti, 
			[Description("Regular: Shashtamsa (d-6)")]      GenericShashthamsa,
			[Description("Regular: Saptamsa (d-7)")]		GenericSaptamsa,
			[Description("Regular: Dasamsa (d-10)")]        GenericDasamsa,
			[Description("Regular: Dwadasamsa (d-12)")]		GenericDwadasamsa,
			[Description("Regular: Chaturvimsamsa (d-24)")] GenericChaturvimsamsa,
			[Description("Trikona: Drekkana (d-3)")]		GenericDrekkana,
			[Description("Trikona: Shodasamsa (d-16)")]     GenericShodasamsa,
			[Description("Trikona: Vimsamsa (d-20)")]		GenericVimsamsa,
			[Description("Kendra: Chaturthamsa (d-4)")]		GenericChaturthamsa,
			[Description("Kendra: Nakshatramsa (d-27)")]	GenericNakshatramsa
		}

		public enum Muhurta : int
		{
			Rudra=1, Ahi, Mitra, Pitri, Vasu, Ambu, Visvadeva, Abhijit, Vidhata, Puruhuta,
			Indragni, Nirriti, Varuna, Aryaman, Bhaga, Girisa, Ajapada, Ahirbudhnya, Pushan, Asvi,
			Yama, Agni, Vidhaatri, Chanda, Aditi, Jiiva, Vishnu, Arka, Tvashtri, Maruta
		}

		static public Nakshatra28.Name NakLordOfMuhurta (Muhurta m)
		{
			switch (m)
			{
				case Muhurta.Rudra: return Nakshatra28.Name.Aridra;
				case Muhurta.Ahi: return Nakshatra28.Name.Aslesha;
				case Muhurta.Mitra: return Nakshatra28.Name.Anuradha;
				case Muhurta.Pitri: return Nakshatra28.Name.Makha;
				case Muhurta.Vasu: return Nakshatra28.Name.Dhanishta;
				case Muhurta.Ambu: return Nakshatra28.Name.PoorvaShada;
				case Muhurta.Visvadeva: return Nakshatra28.Name.UttaraShada;
				case Muhurta.Abhijit: return Nakshatra28.Name.Abhijit;
				case Muhurta.Vidhata: return Nakshatra28.Name.Rohini;
				case Muhurta.Puruhuta: return Nakshatra28.Name.Jyestha;
				case Muhurta.Indragni: return Nakshatra28.Name.Vishaka;
				case Muhurta.Nirriti: return Nakshatra28.Name.Moola;
				case Muhurta.Varuna: return Nakshatra28.Name.Satabisha;
				case Muhurta.Aryaman: return Nakshatra28.Name.UttaraPhalguni;
				case Muhurta.Bhaga: return Nakshatra28.Name.PoorvaPhalguni;
				case Muhurta.Girisa: return Nakshatra28.Name.Aridra;
				case Muhurta.Ajapada: return Nakshatra28.Name.PoorvaBhadra;
				case Muhurta.Ahirbudhnya: return Nakshatra28.Name.UttaraBhadra;
				case Muhurta.Pushan: return Nakshatra28.Name.Revati;
				case Muhurta.Asvi: return Nakshatra28.Name.Aswini;
				case Muhurta.Yama: return Nakshatra28.Name.Bharani;
				case Muhurta.Agni: return Nakshatra28.Name.Krittika;
				case Muhurta.Vidhaatri: return Nakshatra28.Name.Rohini;
				case Muhurta.Chanda: return Nakshatra28.Name.Mrigarirsa;
				case Muhurta.Aditi: return Nakshatra28.Name.Punarvasu;
				case Muhurta.Jiiva: return Nakshatra28.Name.Pushya;
				case Muhurta.Vishnu: return Nakshatra28.Name.Sravana;
				case Muhurta.Arka: return Nakshatra28.Name.Hasta;
				case Muhurta.Tvashtri: return Nakshatra28.Name.Chittra;
				case Muhurta.Maruta: return Nakshatra28.Name.Swati;
			}
			Debug.Assert (false, string.Format("Basics::NakLordOfMuhurta Unknown Muhurta {0}", m));
			return Nakshatra28.Name.Aswini;
		}
		public static string variationNameOfDivision (Division d)
		{
			if (d.MultipleDivisions.Length > 1)
				return "Composite";

			switch (d.MultipleDivisions[0].Varga)
			{
				case DivisionType.Rasi: 
					return "Rasi";
				case DivisionType.Navamsa: 
					return "Navamsa";
				case DivisionType.HoraParasara:
					return "Parasara";
				case DivisionType.HoraJagannath:
					return "Jagannath";
				case DivisionType.HoraParivrittiDwaya:
					return "Parivritti";
				case DivisionType.HoraKashinath:
					return "Kashinath";
				case DivisionType.DrekkanaParasara:
					return "Parasara";
				case DivisionType.DrekkanaJagannath:
					return "Jagannath";
				case DivisionType.DrekkanaParivrittitraya:
					return "Parivritti";
				case DivisionType.DrekkanaSomnath:
					return "Somnath";
				case DivisionType.Chaturthamsa: 
					return "";
				case DivisionType.Panchamsa:
					return "";
				case DivisionType.Shashthamsa:
					return "";
				case DivisionType.Saptamsa:
					return "";
				case DivisionType.Ashtamsa:
					return "Rath";
				case DivisionType.AshtamsaRaman:
					return "Raman";
				case DivisionType.Dasamsa:
					return "";
				case DivisionType.Rudramsa:
					return "Rath";
				case DivisionType.RudramsaRaman:
					return "Raman";
				case DivisionType.Dwadasamsa:
					return "";
				case DivisionType.Shodasamsa:
					return "";
				case DivisionType.Vimsamsa:
					return "";
				case DivisionType.Chaturvimsamsa:
					return "";
				case DivisionType.Nakshatramsa:
					return "";
				case DivisionType.Trimsamsa:
					return "";
				case DivisionType.TrimsamsaParivritti:
					return "Parivritti";
				case DivisionType.TrimsamsaSimple:
					return "Simple";
				case DivisionType.Khavedamsa:
					return "";
				case DivisionType.Akshavedamsa:
					return "";
				case DivisionType.Shashtyamsa:
					return "";
				case DivisionType.Ashtottaramsa:
					return "";
				case DivisionType.Nadiamsa:
					return "Equal Size";
				case DivisionType.NadiamsaCKN:
					return "Chandra Kala";
				case DivisionType.NavamsaDwadasamsa:
					return "Composite";
				case DivisionType.DwadasamsaDwadasamsa:
					return "Composite";
				case DivisionType.BhavaPada:
					return "9 Padas";
				case DivisionType.BhavaEqual:
					return "Equal Houses";
				case DivisionType.BhavaAlcabitus:
					return "Alcabitus";
				case DivisionType.BhavaAxial:
					return "Axial";
				case DivisionType.BhavaCampanus:
					return "Campanus";
				case DivisionType.BhavaKoch:
					return "Koch";
				case DivisionType.BhavaPlacidus:
					return "Placidus";
				case DivisionType.BhavaRegiomontanus:
					return "Regiomontanus";
				case DivisionType.BhavaSripati:
					return "Sripati";
				case DivisionType.GenericParivritti:
					return "Parivritti";
				case DivisionType.GenericShashthamsa:
					return "Regular: Shashtamsa";
				case DivisionType.GenericSaptamsa:
					return "Regular: Saptamsa";
				case DivisionType.GenericDasamsa:
					return "Regular: Dasamsa";
				case DivisionType.GenericDwadasamsa:
					return "Regular: Dwadasamsa";
				case DivisionType.GenericChaturvimsamsa:
					return "Regular: Chaturvimsamsa";
				case DivisionType.GenericChaturthamsa:
					return "Kendra: Chaturtamsa";
				case DivisionType.GenericNakshatramsa:
					return "Kendra: Nakshatramsa";
				case DivisionType.GenericDrekkana:
					return "Trikona: Drekkana";
				case DivisionType.GenericShodasamsa:
					return "Trikona: Shodasamsa";
				case DivisionType.GenericVimsamsa:
					return "Trikona: Vimsamsa";
			}
			Debug.Assert(false, string.Format("Basics::numPartsInBasics.DivisionType. Unknown Division {0}", d.MultipleDivisions[0].Varga));
			return "";
		}

		public static string numPartsInDivisionString (Division div)
		{
			string sRet = "D";
			foreach (Division.SingleDivision dSingle in div.MultipleDivisions)
			{
				sRet = string.Format("{0}-{1}", sRet, numPartsInDivision(dSingle));
			}
			return sRet;
		}
		public static int numPartsInDivision (Division div)
		{
			int parts = 1;
			foreach (Division.SingleDivision dSingle in div.MultipleDivisions)
			{
				parts *= numPartsInDivision(dSingle);
			}
			return parts;
		}
		public static int numPartsInDivision (Division.SingleDivision dSingle)
		{
			
			switch (dSingle.Varga)
			{
				case DivisionType.Rasi: 
					return 1;
				case DivisionType.Navamsa: 
					return 9;
				case DivisionType.HoraParasara:
				case DivisionType.HoraJagannath:
				case DivisionType.HoraParivrittiDwaya:
				case DivisionType.HoraKashinath:
					return 2;
				case DivisionType.DrekkanaParasara:
				case DivisionType.DrekkanaJagannath:
				case DivisionType.DrekkanaParivrittitraya:
				case DivisionType.DrekkanaSomnath:
					return 3;
				case DivisionType.Chaturthamsa: 
					return 4;
				case DivisionType.Panchamsa:
					return 5;
				case DivisionType.Shashthamsa:
					return 6;
				case DivisionType.Saptamsa:
					return 7;
				case DivisionType.Ashtamsa:
				case DivisionType.AshtamsaRaman:
					return 8;
				case DivisionType.Dasamsa:
					return 10;
				case DivisionType.Rudramsa:
				case DivisionType.RudramsaRaman:
					return 11;
				case DivisionType.Dwadasamsa:
					return 12;
				case DivisionType.Shodasamsa:
					return 16;
				case DivisionType.Vimsamsa:
					return 20;
				case DivisionType.Chaturvimsamsa:
					return 24;
				case DivisionType.Nakshatramsa:
					return 27;
				case DivisionType.Trimsamsa:
				case DivisionType.TrimsamsaParivritti:
				case DivisionType.TrimsamsaSimple:
					return 30;
				case DivisionType.Khavedamsa:
					return 40;
				case DivisionType.Akshavedamsa:
					return 45;
				case DivisionType.Shashtyamsa:
					return 60;
				case DivisionType.Ashtottaramsa:
					return 108;
				case DivisionType.Nadiamsa:
				case DivisionType.NadiamsaCKN:
					return 150;
				case DivisionType.NavamsaDwadasamsa:
					return 108;
				case DivisionType.DwadasamsaDwadasamsa:
					return 144;
				case DivisionType.BhavaPada:
				case DivisionType.BhavaEqual:
				case DivisionType.BhavaAlcabitus:
				case DivisionType.BhavaAxial:
				case DivisionType.BhavaCampanus:
				case DivisionType.BhavaKoch:
				case DivisionType.BhavaPlacidus:
				case DivisionType.BhavaRegiomontanus:
				case DivisionType.BhavaSripati:
					return 1;
				default:
					return dSingle.NumParts;
			}
		}

		public static Division[] Shadvargas ()
		{
			return new Division[]
			{
				new Division(DivisionType.Rasi), 
				new Division(DivisionType.HoraParasara), 
				new Division(DivisionType.DrekkanaParasara),
				new Division(DivisionType.Navamsa), 
				new Division(DivisionType.Dwadasamsa), 
				new Division(DivisionType.Trimsamsa)
			};
		}

		public static Division[] Saptavargas ()
		{
			return new Division[]
			{
				new Division(DivisionType.Rasi), 
				new Division(DivisionType.HoraParasara), 
				new Division(DivisionType.DrekkanaParasara),
				new Division(DivisionType.Saptamsa),
				new Division(DivisionType.Navamsa), 
				new Division(DivisionType.Dwadasamsa), 
				new Division(DivisionType.Trimsamsa)
			};
		}

		public static Division[] Dasavargas ()
		{
			return new Division[]
			{
				new Division(DivisionType.Rasi), 
				new Division(DivisionType.HoraParasara), 
				new Division(DivisionType.DrekkanaParasara),
				new Division(DivisionType.Saptamsa), 
				new Division(DivisionType.Navamsa), 
				new Division(DivisionType.Dasamsa),
				new Division(DivisionType.Dwadasamsa), 
				new Division(DivisionType.Shodasamsa), 
				new Division(DivisionType.Trimsamsa),
				new Division(DivisionType.Shashtyamsa)
			};
		}

		public static Division[] Shodasavargas ()
		{
			return new Division[]
			{
				new Division(DivisionType.Rasi), 
				new Division(DivisionType.HoraParasara), 
				new Division(DivisionType.DrekkanaParasara),
				new Division(DivisionType.Chaturthamsa), 
				new Division(DivisionType.Saptamsa), 
				new Division(DivisionType.Navamsa), 
				new Division(DivisionType.Dasamsa), 
				new Division(DivisionType.Dwadasamsa), 
				new Division(DivisionType.Shodasamsa), 
				new Division(DivisionType.Vimsamsa), 
				new Division(DivisionType.Chaturvimsamsa), 
				new Division(DivisionType.Nakshatramsa),
				new Division(DivisionType.Trimsamsa), 
				new Division(DivisionType.Khavedamsa), 
				new Division(DivisionType.Akshavedamsa),
				new Division(DivisionType.Shashtyamsa)
			};
		}

		/// <summary>
		/// Specify the Lord of a ZodiacHouse. The owernership of the nodes is not considered
		/// </summary>
		/// <param name="zh">The House whose lord should be returned</param>
		/// <returns>The lord of zh</returns>
		public static Body.Name SimpleLordOfZodiacHouse (ZodiacHouse.Name zh) 
		{
			switch (zh) 
			{
				case ZodiacHouse.Name.Ari: return Body.Name.Mars;
				case ZodiacHouse.Name.Tau: return Body.Name.Venus;
				case ZodiacHouse.Name.Gem: return Body.Name.Mercury;
				case ZodiacHouse.Name.Can: return Body.Name.Moon;
				case ZodiacHouse.Name.Leo: return Body.Name.Sun; 
				case ZodiacHouse.Name.Vir: return Body.Name.Mercury;
				case ZodiacHouse.Name.Lib: return Body.Name.Venus;
				case ZodiacHouse.Name.Sco: return Body.Name.Mars;
				case ZodiacHouse.Name.Sag: return Body.Name.Jupiter;
				case ZodiacHouse.Name.Cap: return Body.Name.Saturn;
				case ZodiacHouse.Name.Aqu: return Body.Name.Saturn;
				case ZodiacHouse.Name.Pis: return Body.Name.Jupiter;
			}
			
			Trace.Assert (false, 
			string.Format ("Basics.SimpleLordOfZodiacHouse for {0} failed", (int)zh));
			return Body.Name.Other;
		}


		public static Longitude CalculateBodyLongitude (double ut, int ipl)
		{
			double[] xx = new Double[6]{0,0,0,0,0,0};
			try
			{
				sweph.swe_calc_ut(ut, ipl, 0, xx);
				return new Longitude(xx[0]);
			}
			catch (SwephException exc)
			{
				System.Console.WriteLine ( "Sweph: {0}\n", exc.status);
				throw new System.Exception("");
			}
		}

		/// <summary>
		/// Calculated a BodyPosition for a given time and place using the swiss ephemeris
		/// </summary>
		/// <param name="ut">The time for which the calculations should be performed</param>
		/// <param name="ipl">The Swiss Ephemeris body Name</param>
		/// <param name="body">The local application body name</param>
		/// <param name="type">The local application body type</param>
		/// <returns>A BodyPosition class</returns>
		/// 
		public static BodyPosition CalculateSingleBodyPosition (double ut, int ipl, Body.Name body, BodyType.Name type, Horoscope h) 
		{
			if (body == Body.Name.Lagna)
			{
				BodyPosition b = new BodyPosition(h, body, type, new Longitude(sweph.swe_lagna(ut)), 0, 0, 0, 0, 0);
				return b;
			}
			double[] xx = new Double[6] {0,0,0,0,0,0};
			try 
			{
				sweph.swe_calc_ut (ut, ipl, 0, xx);

				BodyPosition b = new BodyPosition (h, body, type, new Longitude(xx[0]), xx[1], xx[2],
					xx[3], xx[4], xx[5]);
				return b;
			} 
			catch (SwephException exc) 
			{
				System.Console.WriteLine ( "Sweph: {0}\n", exc.status);
				throw new System.Exception("");
			}
		}


		/// <summary>
		/// Given a HoraInfo object (all required user inputs), calculate a list of
		/// all bodypositions we will ever require
		/// </summary>
		/// <param name="h">The HoraInfo object</param>
		/// <returns></returns>
		public static ArrayList CalculateBodyPositions (Horoscope h, double sunrise) 
		{
			HoraInfo hi = h.info;
			HoroscopeOptions o = h.options;

			StringBuilder serr = new StringBuilder (256);
			string ephe_path = MhoraGlobalOptions.Instance.HOptions.EphemerisPath;

			// The order of the array must reflect the order define in Basics.GrahaIndex
			ArrayList std_grahas = new ArrayList (20);
			
			sweph.swe_set_ephe_path (ephe_path);
			double julday_ut = sweph.swe_julday (hi.tob.year, hi.tob.month, hi.tob.day,
				hi.tob.time - hi.tz.toDouble());
			//	h.tob.hour + (((double)h.tob.minute) / 60.0) + (((double)h.tob.second) / 3600.0));
			//	(h.tob.time / 24.0) + (h.tz.toDouble()/24.0));
				//(h.tob.hour/24.0) + (((double)h.tob.minute) / 60.0) + (((double)h.tob.second) / 3600.0));
			//julday_ut = julday_ut - (h.tz.toDouble() / 24.0);
			
			int swephRahuBody = sweph.SE_MEAN_NODE;
			if (o.nodeType == HoroscopeOptions.ENodeType.True)
				swephRahuBody = sweph.SE_TRUE_NODE;

			int addFlags = 0;
			if (o.grahaPositionType == HoroscopeOptions.EGrahaPositionType.True)
				addFlags = sweph.SEFLG_TRUEPOS;

			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_SUN, Body.Name.Sun, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_MOON, Body.Name.Moon, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_MARS, Body.Name.Mars, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_MERCURY, Body.Name.Mercury, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_JUPITER, Body.Name.Jupiter, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_VENUS, Body.Name.Venus, BodyType.Name.Graha, h));
			std_grahas.Add (CalculateSingleBodyPosition (julday_ut, sweph.SE_SATURN, Body.Name.Saturn, BodyType.Name.Graha, h));
			BodyPosition rahu = CalculateSingleBodyPosition (julday_ut, swephRahuBody, Body.Name.Rahu, BodyType.Name.Graha, h);

			BodyPosition ketu = CalculateSingleBodyPosition (julday_ut, swephRahuBody, Body.Name.Ketu, BodyType.Name.Graha, h);
			ketu.longitude = rahu.longitude.add (new Longitude (180.0));
			std_grahas.Add (rahu);
			std_grahas.Add (ketu);

			double asc = sweph.swe_lagna(julday_ut);
			std_grahas.Add (new BodyPosition (h, Body.Name.Lagna, BodyType.Name.Lagna, new Longitude (asc), 0, 0, 0, 0, 0));

			double ista_ghati = normalize_exc( 0.0, 24.0, hi.tob.time - sunrise) * 2.5;
			Longitude gl_lon = ((BodyPosition)std_grahas[0]).longitude.add(new Longitude(ista_ghati * 30.0));
			Longitude hl_lon = ((BodyPosition)std_grahas[0]).longitude.add(new Longitude(ista_ghati * 30.0/ 2.5));
			Longitude bl_lon = ((BodyPosition)std_grahas[0]).longitude.add(new Longitude(ista_ghati * 30.0/ 5.0));

			double vl = ista_ghati * 5.0;
			while (ista_ghati > 12.0) ista_ghati -= 12.0;
			Longitude vl_lon = ((BodyPosition)std_grahas[0]).longitude.add(new Longitude(vl * 30.0));

			std_grahas.Add (new BodyPosition (h, Body.Name.BhavaLagna, BodyType.Name.SpecialLagna, bl_lon,0,0,0,0,0));
			std_grahas.Add (new BodyPosition (h, Body.Name.HoraLagna, BodyType.Name.SpecialLagna, hl_lon,0,0,0,0,0));
			std_grahas.Add (new BodyPosition (h, Body.Name.GhatiLagna, BodyType.Name.SpecialLagna, gl_lon, 0,0,0,0,0));
			std_grahas.Add (new BodyPosition (h, Body.Name.VighatiLagna, BodyType.Name.SpecialLagna, vl_lon,0,0,0,0,0));			


			return std_grahas;
		}
	}

	/// <summary>
	/// Mutually exclusive classes of BodyTypes
	/// </summary>
	public class BodyType 
	{
		public enum Name : int
		{
			Lagna, Graha, NonLunarNode,
			SpecialLagna, ChandraLagna,
			BhavaArudha, BhavaArudhaSecondary, GrahaArudha,
			Varnada, Upagraha, Sahama, Other
		}
	}

	/// <summary>
	///  A compile-time list of every body we will use in this program
	/// </summary>
	public class Body
	{
		[TypeConverter(typeof(EnumDescConverter))]
		public enum Name :int 
		{
			// Do NOT CHANGE ORDER WITHOUT CHANING NARAYANA DASA ETC
			// RELY ON EXPLICIT EQUAL CONVERSION FOR STRONGER CO_LORD ETC
			Sun=0, Moon=1, Mars=2, Mercury=3, Jupiter=4, Venus=5, Saturn=6, 
			Rahu=7, Ketu=8, Lagna=9, 

			// And now, we're no longer uptight about the ordering :-)
			[Description("Bhava Lagna")]			BhavaLagna, 
			[Description("Hora Lagna")]				HoraLagna, 
			[Description("Ghati Lagna")]			GhatiLagna, 
			[Description("Sree Lagna")]				SreeLagna, 
													Pranapada,
			[Description("Vighati Lagna")]			VighatiLagna, 
			Dhuma, Vyatipata, Parivesha, Indrachapa, Upaketu,
			Kala, Mrityu, ArthaPraharaka, YamaGhantaka, Gulika, Maandi,
			[Description("Chandra Ayur Lagna")]		ChandraAyurLagna,
			MrityuPoint, Other,
			AL, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, UL,
			
		}
		public static int toInt (Body.Name b)
		{
			return ((int)b);
		}
		public static Longitude exaltationDegree (Body.Name b)
		{
			int _b = (int)b;
			Debug.Assert (_b >= (int)Name.Sun && _b <= (int)Name.Saturn);
			double d = 0;
			switch (b)
			{
				case Name.Sun: d = 10; break;
				case Name.Moon: d = 33; break;
				case Name.Mars: d = 298; break;
				case Name.Mercury: d = 165; break;
				case Name.Jupiter: d = 95; break;
				case Name.Venus: d = 357; break; 
				case Name.Saturn: d = 200; break;
			}
			return new Longitude(d);
		}
		public static Longitude debilitationDegree (Body.Name b)
		{
			return exaltationDegree (b).add(180);
		}
		public static string toString (Body.Name b)
		{
			switch (b)
			{
				case Name.Lagna: return "Lagna";
				case Name.Sun: return "Sun";
				case Name.Moon: return "Moon";
				case Name.Mars: return "Mars";
				case Name.Mercury: return "Mercury";
				case Name.Jupiter: return "Jupiter";
				case Name.Venus: return "Venus";
				case Name.Saturn: return "Saturn";
				case Name.Rahu: return "Rahu";
				case Name.Ketu: return "Ketu";
			}
			return "";
		}
		public static string toShortString(Body.Name b) 
		{
			switch (b)
			{
				case Name.Lagna: return "As";
				case Name.Sun: return "Su";
				case Name.Moon: return "Mo";
				case Name.Mars: return "Ma";
				case Name.Mercury: return "Me";
				case Name.Jupiter: return "Ju";
				case Name.Venus: return "Ve";
				case Name.Saturn: return "Sa";
				case Name.Rahu: return "Ra";
				case Name.Ketu: return "Ke";
				case Name.AL: return "AL";
				case Name.A2: return "A2";
				case Name.A3: return "A3";
				case Name.A4: return "A4";
				case Name.A5: return "A5";
				case Name.A6: return "A6";
				case Name.A7: return "A7";
				case Name.A8: return "A8";
				case Name.A9: return "A9";
				case Name.A10: return "A10";
				case Name.A11: return "A11";
				case Name.UL: return "UL";
				case Name.GhatiLagna: return "GL";
				case Name.BhavaLagna: return "BL";
				case Name.HoraLagna: return "HL";
				case Name.VighatiLagna: return "ViL";
				case Name.SreeLagna: return "SL";
				case Name.Pranapada: return "PL";
			}
			Trace.Assert (false, "Basics.Body.toShortString");
			return "   ";
		}
	}


	public class Tithi
	{
		[TypeConverter(typeof(EnumDescConverter))]
		public enum Name : int
		{
			[Description("Shukla Pratipada")]		ShuklaPratipada=1, 
			[Description("Shukla Dvitiya")]			ShuklaDvitiya, 
			[Description("Shukla Tritiya")]			ShuklaTritiya, 
			[Description("Shukla Chaturti")]		ShuklaChaturti, 
			[Description("Shukla Panchami")]		ShuklaPanchami,
			[Description("Shukla Shashti")]			ShuklaShashti, 
			[Description("Shukla Saptami")]			ShuklaSaptami, 
			[Description("Shukla Ashtami")]			ShuklaAshtami, 
			[Description("Shukla Navami")]			ShuklaNavami, 
			[Description("Shukla Dashami")]			ShuklaDasami,
			[Description("Shukla Ekadasi")]			ShuklaEkadasi, 
			[Description("Shukla Dwadasi")]			ShuklaDvadasi, 
			[Description("Shukla Trayodasi")]		ShuklaTrayodasi, 
			[Description("Shukla Chaturdasi")]		ShuklaChaturdasi, 
			[Description("Paurnami")]				Paurnami,
			[Description("Krishna Pratipada")]		KrishnaPratipada, 
			[Description("Krishna Dvitiya")]		KrishnaDvitiya, 
			[Description("Krishna Tritiya")]		KrishnaTritiya, 
			[Description("Krishna Chaturti")]		KrishnaChaturti, 
			[Description("Krishna Panchami")]		KrishnaPanchami,
			[Description("Krishna Shashti")]		KrishnaShashti, 
			[Description("Krishna Saptami")]		KrishnaSaptami, 
			[Description("Krishna Ashtami")]		KrishnaAshtami, 
			[Description("Krishna Navami")]			KrishnaNavami, 
			[Description("Krishna Dashami")]		KrishnaDasami,
			[Description("Krishna Ekadasi")]		KrishnaEkadasi, 
			[Description("Krishna Dwadasi")]		KrishnaDvadasi, 
			[Description("Krishna Trayodasi")]		KrishnaTrayodasi, 
			[Description("Krishna Chaturdasi")]		KrishnaChaturdasi, 
			[Description("Amavasya")]				Amavasya
		}

		public enum NandaType : int
		{	Nanda, Bhadra, Jaya, Rikta, Purna }

		public string ToUnqualifiedString ()
		{
			switch (mValue)
			{
				case Name.KrishnaPratipada:
				case Name.ShuklaPratipada: return "Prathama";
				case Name.KrishnaDvitiya:
				case Name.ShuklaDvitiya: return "Dvitiya";
				case Name.KrishnaTritiya:
				case Name.ShuklaTritiya: return "Tritiya";
				case Name.KrishnaChaturti:
				case Name.ShuklaChaturti: return "Chaturthi";
				case Name.KrishnaPanchami:
				case Name.ShuklaPanchami: return "Panchami";
				case Name.KrishnaShashti:
				case Name.ShuklaShashti: return "Shashti";
				case Name.KrishnaSaptami:
				case Name.ShuklaSaptami: return "Saptami";
				case Name.KrishnaAshtami:
				case Name.ShuklaAshtami: return "Ashtami";
				case Name.KrishnaNavami:
				case Name.ShuklaNavami: return "Navami";
				case Name.KrishnaDasami:
				case Name.ShuklaDasami: return "Dashami";
				case Name.KrishnaEkadasi:
				case Name.ShuklaEkadasi: return "Ekadashi";
				case Name.KrishnaDvadasi:
				case Name.ShuklaDvadasi: return "Dwadashi";
				case Name.KrishnaTrayodasi:
				case Name.ShuklaTrayodasi: return "Trayodashi";
				case Name.KrishnaChaturdasi:
				case Name.ShuklaChaturdasi: return "Chaturdashi";
				case Name.Paurnami: return "Poornima";
				case Name.Amavasya: return "Amavasya";
			}
			return "";
		}
		public override string ToString()
		{
			return EnumDescConverter.GetEnumDescription(mValue);
		}


		private Name mValue;
		public Tithi (Name _mValue)
		{
			mValue = (Name)Basics.normalize_inc(1, 30, (int)_mValue); 
		}
		public Name value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		public Tithi add (int i)
		{
			int tnum = Basics.normalize_inc(1, 30, (int)this.value+i-1);
			return new Tithi((Name)tnum);
		}
		public Tithi addReverse (int i)
		{
			int tnum = Basics.normalize_inc(1, 30, (int)this.value -i +1);
			return new Tithi((Name)tnum);
		}	
		public Body.Name getLord ()
		{
			// 1 based index starting with prathama
			int t = (int)this.value;

			//Console.WriteLine ("Looking for lord of tithi {0}", t);
			// check for new moon and full moon 
			if (t == 30) return Body.Name.Rahu;
			if (t == 15) return Body.Name.Saturn;

			// coalesce pakshas
			if (t >= 16) t -= 15;
			switch (t)
			{
				case 1: case 9:	return Body.Name.Sun;
				case 2: case 10: return Body.Name.Moon;
				case 3: case 11: return Body.Name.Mars;
				case 4: case 12: return Body.Name.Mercury;
				case 5: case 13: return Body.Name.Jupiter;
				case 6: case 14: return Body.Name.Venus;
				case 7: return Body.Name.Saturn;
				case 8: return Body.Name.Rahu;
			}
			Debug.Assert(false, "Tithi::getLord");
			return Body.Name.Sun;
		}

		public NandaType toNandaType ()
		{
			// 1 based index starting with prathama
			int t = (int)this.value;

			// check for new moon and full moon 

			if (t == 30 || t == 15) return NandaType.Purna;

			// coalesce pakshas
			if (t >= 16) t -= 15;
			switch (t)
			{
				case 1: case 6:	case 11: return NandaType.Nanda;
				case 2: case 7: case 12: return NandaType.Bhadra;
				case 3: case 8: case 13: return NandaType.Jaya;
				case 4: case 9: case 14: return NandaType.Rikta;
				case 5: case 10: return NandaType.Purna;
			}
			Debug.Assert(false, "Tithi::toNandaType");
			return NandaType.Nanda;
		}
	}

	public class Karana
	{
		public enum Name: int
		{
			Kimstughna=1,
			Bava1, Balava1, Kaulava1, Taitula1, Garija1, Vanija1, Vishti1,
			Bava2, Balava2, Kaulava2, Taitula2, Garija2, Vanija2, Vishti2,
			Bava3, Balava3, Kaulava3, Taitula3, Garija3, Vanija3, Vishti3,
			Bava4, Balava4, Kaulava4, Taitula4, Garija4, Vanija4, Vishti4,
			Bava5, Balava5, Kaulava5, Taitula5, Garija5, Vanija5, Vishti5,
			Bava6, Balava6, Kaulava6, Taitula6, Garija6, Vanija6, Vishti6,
			Bava7, Balava7, Kaulava7, Taitula7, Garija7, Vanija7, Vishti7,
			Bava8, Balava8, Kaulava8, Taitula8, Garija8, Vanija8, Vishti8,
			Sakuna, Chatushpada, Naga
		}
		private Name mValue;
		public Karana (Name _mValue)
		{
			mValue = (Name)Basics.normalize_inc(1, 60, (int)_mValue);
		}
		public Name value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		public Karana add (int i)
		{
			int tnum = Basics.normalize_inc(1, 60, (int)this.value+i-1);
			return new Karana((Name)tnum);
		}
		public Karana addReverse (int i)
		{
			int tnum = Basics.normalize_inc(1, 60, (int)this.value -i +1);
			return new Karana((Name)tnum);
		}	
		public override string ToString()
		{
			return this.value.ToString();
		}
		public Body.Name getLord ()
		{
			switch (this.value)
			{
				case Name.Kimstughna: return Body.Name.Moon;
				case Name.Sakuna: return Body.Name.Mars;
				case Name.Chatushpada: return Body.Name.Sun;
				case Name.Naga: return Body.Name.Venus;
				default:
				{
					int vn = Basics.normalize_inc(1, 7, (int)this.value - 1);
					switch (vn)
					{
						case 1: return Body.Name.Sun;
						case 2: return Body.Name.Moon;
						case 3: return Body.Name.Mars;
						case 4: return Body.Name.Mercury;
						case 5: return Body.Name.Jupiter;
						case 6: return Body.Name.Venus;
						default: return Body.Name.Saturn;
					}
				}
			}
		}

	}

	public class SunMoonYoga
	{
		public enum Name : int
		{
			Vishkambha=1, Preeti, Aayushmaan, Saubhaagya, Sobhana, Atiganda, Sukarman, Dhriti, Shoola, Ganda,
			Vriddhi, Dhruva, Vyaaghaata, Harshana, Vajra, Siddhi, Vyatipaata, Variyan, Parigha, Shiva, 
			Siddha, Saadhya, Subha, Sukla, Brahma, Indra, Vaidhriti
		}
		private Name mValue;
		public SunMoonYoga (SunMoonYoga.Name _mvalue)
		{
			mValue = _mvalue; 
		}
		public Name value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		public int normalize ()
		{
			return Basics.normalize_inc(1, 27, (int)this.value);
		}
		public SunMoonYoga add (int i)
		{
			int snum = Basics.normalize_inc(1, 27, (int)this.value+i-1);
			return new SunMoonYoga((Name)snum);
		}
		public SunMoonYoga addReverse (int i)
		{
			int snum = Basics.normalize_inc(1, 27, (int)this.value -i +1);
			return new SunMoonYoga((Name)snum);
		}
		public Body.Name getLord ()
		{
			switch ((int)this.value % 9)
			{
				case 1: return Body.Name.Saturn;
				case 2: return Body.Name.Mercury;
				case 3: return Body.Name.Ketu;
				case 4: return Body.Name.Venus;
				case 5: return Body.Name.Sun;
				case 6: return Body.Name.Moon;
				case 7: return Body.Name.Mars;
				case 8: return Body.Name.Rahu;
				default: return Body.Name.Jupiter;
			}
		}
		public override string ToString()
		{
			return this.value.ToString();
		}

	}


	/// <summary>
	/// A list of nakshatras, and related helper functions
	/// </summary>
	public class Nakshatra28
	{
		public enum Name :int
		{
			Aswini=1, Bharani=2, Krittika=3, Rohini=4, Mrigarirsa=5,
			Aridra=6, Punarvasu=7, Pushya=8, Aslesha=9,
			Makha=10, PoorvaPhalguni=11, UttaraPhalguni=12, Hasta=13,
			Chittra=14, Swati=15, Vishaka=16, Anuradha=17, Jyestha=18,
			Moola=19, PoorvaShada=20, UttaraShada=21, Abhijit=22, Sravana=23, Dhanishta=24,
			Satabisha=25, PoorvaBhadra=26, UttaraBhadra=27, Revati=28
		}
		private Nakshatra28.Name m_nak;
		public Nakshatra28.Name value 
		{
			get { return m_nak; }
			set { m_nak = value; }
		}
		public Nakshatra28 (Nakshatra28.Name nak) 
		{
			m_nak = nak;
		}
		public int normalize ()
		{
			return Basics.normalize_inc (1, 28, (int)this.value);
		}
		public Nakshatra28 add (int i)
		{
			int snum = Basics.normalize_inc (1, 28, (int)this.value + i -1);
			return new Nakshatra28 ((Nakshatra28.Name) snum);
		}		
	}

	public class BodyKarakaComparer : IComparable 
	{
		BodyPosition bpa;
		public BodyPosition GetPosition
		{
			get { return bpa; }
			set { this.bpa = value; }
		}
		public BodyKarakaComparer (BodyPosition _bp)
		{
			bpa = _bp;
		}
		public double getOffset ()
		{
			double off = bpa.longitude.toZodiacHouseOffset();
			if (bpa.name == Body.Name.Rahu)
				off = 30.0 - off;
			return off;
		}
		public int CompareTo (object obj)
		{
			Debug.Assert (obj is BodyKarakaComparer);
			double offa = this.getOffset();
			double offb = ((BodyKarakaComparer)obj).getOffset();
			return offb.CompareTo(offa);
		}
	}

	public class Nakshatra
	{
		// int values should not be changed. 
		// used in kalachakra dasa, and various other places.
		public enum Name :int
		{
			Aswini=1, Bharani=2, Krittika=3, Rohini=4, Mrigarirsa=5,
			Aridra=6, Punarvasu=7, Pushya=8, Aslesha=9,
			Makha=10, PoorvaPhalguni=11, UttaraPhalguni=12, Hasta=13,
			Chittra=14, Swati=15, Vishaka=16, Anuradha=17, Jyestha=18,
			Moola=19, PoorvaShada=20, UttaraShada=21, Sravana=22, Dhanishta=23,
			Satabisha=24, PoorvaBhadra=25, UttaraBhadra=26, Revati=27
		}
		public override string ToString()
		{
			switch (m_nak)
			{
				case Name.Aswini: return "Aswini";
				case Name.Bharani: return "Bharani";
				case Name.Krittika: return "Krittika";
				case Name.Rohini: return "Rohini";
				case Name.Mrigarirsa: return "Mrigasira";
				case Name.Aridra: return "Ardra";
				case Name.Punarvasu: return "Punarvasu";
				case Name.Pushya: return "Pushyami";
				case Name.Aslesha: return "Aslesha";
				case Name.Makha: return "Makha";
				case Name.PoorvaPhalguni: return "P.Phalguni";
				case Name.UttaraPhalguni: return "U.Phalguni";
				case Name.Hasta: return "Hasta";
				case Name.Chittra: return "Chitta";
				case Name.Swati: return "Swati";
				case Name.Vishaka: return "Visakha";
				case Name.Anuradha: return "Anuradha";
				case Name.Jyestha: return "Jyeshtha";
				case Name.Moola: return "Moola";
				case Name.PoorvaShada: return "P.Ashada";
				case Name.UttaraShada: return "U.Ashada";
				case Name.Sravana: return "Sravana";
				case Name.Dhanishta: return "Dhanishta";
				case Name.Satabisha: return "Shatabisha";
				case Name.PoorvaBhadra: return "P.Bhadra";
				case Name.UttaraBhadra: return "U.Bhadra";
				case Name.Revati: return "Revati";
				default: return "---";
			}		
		}

		public string toShortString ()
		{
			switch (m_nak)
			{
				case Name.Aswini: return "Asw";
				case Name.Bharani: return "Bha";
				case Name.Krittika: return "Kri";
				case Name.Rohini: return "Roh";
				case Name.Mrigarirsa: return "Mri";
				case Name.Aridra: return "Ari";
				case Name.Punarvasu: return "Pun";
				case Name.Pushya: return "Pus";
				case Name.Aslesha: return "Asl";
				case Name.Makha: return "Mak";
				case Name.PoorvaPhalguni: return "P.Ph";
				case Name.UttaraPhalguni: return "U.Ph";
				case Name.Hasta: return "Has";
				case Name.Chittra: return "Chi";
				case Name.Swati: return "Swa";
				case Name.Vishaka: return "Vis";
				case Name.Anuradha: return "Anu";
				case Name.Jyestha: return "Jye";
				case Name.Moola: return "Moo";
				case Name.PoorvaShada: return "P.Ash";
				case Name.UttaraShada: return "U.Ash";
				case Name.Sravana: return "Sra";
				case Name.Dhanishta: return "Dha";
				case Name.Satabisha: return "Sat";
				case Name.PoorvaBhadra: return "P.Bh";
				case Name.UttaraBhadra: return "U.Bh";
				case Name.Revati: return "Rev";
				default: return "---";
			}
			
		}
		private Nakshatra.Name m_nak;
		public Nakshatra.Name value 
		{
			get { return m_nak; }
			set { m_nak = value; }
		}
		public Nakshatra (Nakshatra.Name nak) 
		{
			m_nak = (Nakshatra.Name)Basics.normalize_inc(1, 27, (int)nak);
		}
		public int normalize ()
		{
			return Basics.normalize_inc (1, 27, (int)this.value);
		}
		public Nakshatra add (int i)
		{
			int snum = Basics.normalize_inc (1, 27, (int)this.value + i -1);
			return new Nakshatra ((Nakshatra.Name) snum);
		}	
		public Nakshatra addReverse (int i)
		{
			int snum = Basics.normalize_inc (1, 27, (int)this.value - i +1);
			return new Nakshatra ((Nakshatra.Name) snum);
		}	
	}

	/// <summary>
	/// A package of longitude related functions. These are useful enough that
	/// I have justified using an object instead of a simple double value type
	/// </summary>
	/// 
	internal class LongitudeConverter: ExpandableObjectConverter 
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
			Trace.Assert (value is string, "LongitudeConverter::ConvertFrom 1");
			string s = (string) value;

			string[] arr = s.Split (new Char[3] {'.',' ',':'});

			double lonValue = 0;
			if (arr.Length >= 1) lonValue = int.Parse(arr[0]);
			if (arr.Length >= 2)
			{
				switch (arr[1].ToLower())
				{
					case "ari": lonValue += 0.0; break;
					case "tau": lonValue += 30.0; break;
					case "gem": lonValue += 60.0; break;
					case "can": lonValue += 90.0; break;
					case "leo": lonValue += 120.0; break;
					case "vir": lonValue += 150.0; break;
					case "lib": lonValue += 180.0; break;
					case "sco": lonValue += 210.0; break;
					case "sag": lonValue += 240.0; break;
					case "cap": lonValue += 270.0; break;
					case "aqu": lonValue += 300.0; break;
					case "pis": lonValue += 330.0; break;
				}
			}
			double divider = 60;
			for (int i=2; i<arr.Length; i++)
			{
				lonValue += (double.Parse(arr[i]) / divider);
				divider *= 60.0;
			}

			return new Longitude(lonValue);
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Trace.Assert (destType == typeof(string) && value is Longitude, "Longitude::ConvertTo 1");
			Longitude lon = (Longitude)value;
			return lon.ToString();
		}   
	}



	[Serializable]
	[TypeConverter(typeof(LongitudeConverter))] 
	public class Longitude
	{
		private double m_lon;
		public Longitude (double lon) 
		{
			while (lon > 360.0) lon -= 360.0;
			while (lon < 0) lon += 360.0;
			m_lon = lon;
			//m_lon = Basics.normalize_exc (0, 360, lon);
			
		}
		public double value 
		{ 
			get { return m_lon; } 
			set 
			{
				Trace.Assert (value >= 0 && value <= 360);
				m_lon = value; 
			} 
		}
		public Longitude add (Longitude b) 
		{
			return new Longitude (Basics.normalize_exc_lower (0, 360, value + b.value));
		}
		public Longitude add (double b)
		{
			return this.add (new Longitude (b));
		}
		public Longitude sub (Longitude b)
		{
			return new Longitude (Basics.normalize_exc_lower (0, 360, value - b.value));
		}
		public Longitude sub (double b)
		{
			return this.sub (new Longitude(b));
		}
		public double normalize ()
		{
			return Basics.normalize_exc_lower (0, 360, this.value);
		}
		public bool isBetween (Longitude cusp_lower, Longitude cusp_higher)
		{
			double diff1 = this.sub(cusp_lower).value;
			double diff2 = this.sub(cusp_higher).value;

			bool bRet = (cusp_higher.sub(cusp_lower).value <= 180) && diff1 <= diff2;

			Console.WriteLine ("Is it true that {0} < {1} < {2}? {3}", this, cusp_lower, cusp_higher, bRet);
			return bRet;
		}
		public SunMoonYoga toSunMoonYoga ()
		{
			int smIndex = (int)(Math.Floor(this.value / (360.0/27.0))+1);
			SunMoonYoga smYoga = new SunMoonYoga((SunMoonYoga.Name)smIndex);
			return smYoga;
		}
		public double toSunMoonYogaBase ()
		{
			int num = (int)(toSunMoonYoga().value);
			double cusp = ((double)(num-1))*(360.0/27.0);
			return cusp;
		}
		public double  toSunMoonYogaOffset ()
		{
			return (this.value - this.toSunMoonYogaBase());
		}
		public Tithi toTithi ()
		{
			int tIndex = (int)(Math.Floor(this.value / (360.0/30.0))+1);
			Tithi t = new Tithi((Tithi.Name)tIndex);
			return t;
		}
		public Karana toKarana ()
		{
			int kIndex = (int)(Math.Floor(this.value / (360.0/60.0))+1);
			Karana k = new Karana((Karana.Name)kIndex);
			return k;
		}
		public double toKaranaBase ()
		{
			int num = (int)(toKarana().value);
			double cusp = ((double)(num-1))*(360.0/60.0);
			return cusp;
		}
		public double toKaranaOffset ()
		{
			return (this.value - this.toKaranaBase());
		}
		public double toTithiBase ()
		{
			int num = (int)(toTithi().value);
			double cusp = ((double)(num-1))*(360.0/30.0);
			return cusp;
		}
		public double toTithiOffset ()
		{
			return (this.value - this.toTithiBase());
		}
		public Nakshatra toNakshatra () 
		{
			int snum = (int)((System.Math.Floor (this.value / (360.0 / 27.0))) + 1.0);
			return new Nakshatra ((Nakshatra.Name)snum);
		}
		public double toNakshatraBase()
		{
			int num = (int)(toNakshatra().value);
			double cusp = ((double)(num-1))*(360.0/27.0);
			return cusp;
		}
		public Nakshatra28 toNakshatra28 ()
		{
			int snum = (int)((System.Math.Floor (this.value / (360.0 / 27.0))) + 1.0);

			Nakshatra28 ret = new Nakshatra28((Nakshatra28.Name)snum);
			if (snum >= (int)Nakshatra28.Name.Abhijit)
				ret = ret.add(2);
			if (this.value >= 270+(6.0+40.0/60.0) && 
				this.value <= 270+(10.0+53.0/60.0+20.0/3600.0))
				ret.value = Nakshatra28.Name.Abhijit;

			return ret;
		}
		public ZodiacHouse toZodiacHouse () 
		{
			int znum = (int)(System.Math.Floor(this.value / 30.0)+ 1.0);
			return new ZodiacHouse ((ZodiacHouse.Name)znum);
		}
		public double toZodiacHouseBase ()
		{
			int znum = (int)(toZodiacHouse().value);
			double cusp = ((double)(znum - 1)) * 30.0;
			return cusp;
		}
		public double toZodiacHouseOffset () 
		{
			int znum = (int)(toZodiacHouse().value);
			double cusp = ((double)(znum - 1)) * 30.0;
			double ret = this.value - cusp;
			Trace.Assert (ret >= 0.0 && ret <= 30.0);
			return ret;
		}
		public double percentageOfZodiacHouse ()
		{
			double offset = toZodiacHouseOffset ();
			double perc = offset / 30.0 * 100;
			Trace.Assert (perc >=0 && perc <= 100);
			return perc;
		}
		public double toNakshatraOffset () 
		{
			int znum = (int)(toNakshatra().value);
			double cusp = ((double)(znum - 1)) * (360.0 / 27.0);
			double ret = this.value - cusp;
			Trace.Assert (ret >= 0.0 && ret <= (360.0 / 27.0));
			return ret;
		}
		public double percentageOfNakshatra ()
		{
			double offset = toNakshatraOffset ();
			double perc = offset / (360.0/27.0) * 100;
			Trace.Assert (perc >=0 && perc <= 100);
			return perc;
		}
		public int toNakshatraPada ()
		{
			double offset = toNakshatraOffset ();
			int val = (int)Math.Floor(offset / (360.0/(27.0*4.0)))+1;
			Trace.Assert (val >= 1 && val <= 4);
			return val;
		}
		public int toAbsoluteNakshatraPada ()
		{
			int n = (int)(this.toNakshatra()).value;
			int p = this.toNakshatraPada();
			return ((n-1)*4) + p;
		}
		public double toNakshatraPadaOffset ()
		{
			int pnum = this.toAbsoluteNakshatraPada(); 
			double cusp = ((double)(pnum-1)) * (360.0 / (27.0*4.0));
			double ret = this.value - cusp;
			Trace.Assert (ret >= 0.0 && ret <= (360.0 / 27.0));
			return ret;
		}
		public double toNakshatraPadaPercentage ()
		{
			double offset = toNakshatraPadaOffset ();
			double perc = offset / (360.0/(27.0*4.0)) * 100;
			Trace.Assert(perc >= 0 && perc <= 100);
			return perc;
		}
		public override string ToString ()
		{
			Longitude lon = this;
			string rasi = lon.toZodiacHouse().value.ToString();
			double offset = lon.toZodiacHouseOffset();
			double minutes = Math.Floor (offset);
			offset = (offset - minutes) * 60.0;
			double seconds = Math.Floor (offset);
			offset = (offset - seconds) * 60.0;
			double subsecs = Math.Floor (offset);
			offset = (offset - subsecs) * 60.0;
			double subsubsecs = Math.Floor (offset);
								
			return
				String.Format("{0:00} {1} {2:00}:{3:00}:{4:00}",
				minutes, rasi, seconds, subsecs, subsubsecs
				);
		}
	}


	internal class DivisionConverter: ExpandableObjectConverter 
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
			return new Division(Basics.DivisionType.Rasi);
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			//Trace.Assert (destType == typeof(string) && value is Division, "DivisionConverter::ConvertTo 1");
			return "Varga";
		}   
	}


	internal class SingleDivisionConverter: ExpandableObjectConverter 
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
			return new Division.SingleDivision(Basics.DivisionType.Rasi);
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			//Trace.Assert (destType == typeof(string) && value is Division, "DivisionConverter::ConvertTo 1");
			Division.SingleDivision dSingle = value as Division.SingleDivision;
			if (dSingle == null) return "Single Varga";
			return dSingle.ToString();
		}   
	}


	[Serializable]
	[TypeConverter(typeof(DivisionConverter))]
	public class Division : ICloneable
	{
		[Serializable]
		[TypeConverter(typeof(SingleDivisionConverter))]
		public class SingleDivision : ICloneable
		{
			private Basics.DivisionType mDtype;
			private int mNumParts;
			public Basics.DivisionType Varga
			{
				get { return mDtype; }
				set { mDtype = value; }
			}
			public int NumParts
			{
				get { return mNumParts; }
				set { mNumParts = value; }
			}
			public SingleDivision (Basics.DivisionType _dtype, int _numParts)
			{
				mDtype = _dtype;
				mNumParts = _numParts;
			}
			public SingleDivision (Basics.DivisionType _dtype)
			{
				mDtype = _dtype;
			}
			public SingleDivision ()
			{
				mDtype = Basics.DivisionType.Rasi;
				mNumParts = 1;
			}
			public override string ToString()
			{
				return this.mDtype.ToString() + " " + mNumParts.ToString();
			}
			public object Clone ()
			{
				return new Division.SingleDivision(this.Varga, this.NumParts);
			}
		}

		private SingleDivision[] mMultipleDivisions = null;

		[PGDisplayName("Composite Division")]
		public SingleDivision[] MultipleDivisions
		{
			get { return this.mMultipleDivisions; }
			set { this.mMultipleDivisions = value; }
		}
		public Division (Basics.DivisionType _dtype)
		{
			this.mMultipleDivisions = new SingleDivision[] { new SingleDivision(_dtype) };
		}
		public Division (Division.SingleDivision single)
		{
			this.mMultipleDivisions = new SingleDivision[] { single };
		}
		public Division ()
		{
			this.mMultipleDivisions = new SingleDivision[] { new SingleDivision(Basics.DivisionType.Rasi) };
		}
		public override string ToString()
		{
			return Basics.numPartsInDivisionString(this);
		}
		public object Clone ()
		{
			Division dRet = new Division();
			ArrayList al = new ArrayList();
			foreach (SingleDivision dSingle in this.MultipleDivisions)
			{
				al.Add (dSingle.Clone());
			}
			dRet.MultipleDivisions = (SingleDivision[])al.ToArray(typeof(Division.SingleDivision));
			return dRet;
		}
		public override bool Equals(object obj)
		{
			if (obj is Division)
				return (this == (Division)obj);
			else
				return base.Equals(obj);
		}

		public static bool operator != (Division d1, Division d2)
		{
			return (!(d1 == d2));
		}
		public static bool operator == (Division d1, Division d2)
		{
			if ((object)d1 == null && (object)d2 == null)
				return true;

			if ((object)d1 == null || (object)d2 == null)
				return false;

			if (d1.MultipleDivisions.Length != d2.MultipleDivisions.Length)
				return false;

			for (int i=0; i<d1.MultipleDivisions.Length; i++)
			{
				if (d1.MultipleDivisions[i].Varga != d2.MultipleDivisions[i].Varga ||
					d1.MultipleDivisions[i].NumParts != d2.MultipleDivisions[i].NumParts)
					return false;
			}
			return true;
		}

		public static void CopyToClipboard (Division div)
		{
			MemoryStream mStr = new MemoryStream();
			BinaryWriter bStr = new BinaryWriter(mStr);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize (mStr, div);
			Clipboard.SetDataObject(mStr, false);
		}
		public static Division CopyFromClipboard ()
		{
			try
			{
				MemoryStream mStr = (MemoryStream)Clipboard.GetDataObject().GetData(typeof(MemoryStream));
				BinaryReader bStr = new BinaryReader(mStr);
				BinaryFormatter formatter = new BinaryFormatter();
				Division div = (Division)formatter.Deserialize(bStr.BaseStream);
				return div;
			}
			catch
			{
				return null;
			}
		}
	}


	/// <summary>
	/// Specifies a DivisionPosition, i.e. a position in a varga chart like Rasi
	/// or Navamsa. It has no notion of "longitude".
	/// </summary>
	public class DivisionPosition
	{
		public Body.Name name;
		public BodyType.Name type;
		public ZodiacHouse zodiac_house;
		public double cusp_lower;
		public double cusp_higher;
		public int part;
		public int ruler_index;
		public string otherString;


		public DivisionPosition (Body.Name _name, BodyType.Name _type, ZodiacHouse _zodiac_house,
			double _cusp_lower, double _cusp_higher, int _part)
		{
			name = _name;
			type = _type;
			zodiac_house = _zodiac_house;
			cusp_lower = _cusp_lower;
			cusp_higher = _cusp_higher;
			part = _part;
			ruler_index = 0;
		}
		public bool isInMoolaTrikona ()
		{
			switch (this.name)
			{
				case Body.Name.Sun: if (zodiac_house.value == ZodiacHouse.Name.Leo) return true; break;
				case Body.Name.Moon: if (zodiac_house.value == ZodiacHouse.Name.Tau) return true; break;
				case Body.Name.Mars: if (zodiac_house.value == ZodiacHouse.Name.Ari) return true; break;
				case Body.Name.Mercury: if (zodiac_house.value == ZodiacHouse.Name.Vir) return true; break;
				case Body.Name.Jupiter: if (zodiac_house.value == ZodiacHouse.Name.Sag) return true; break;
				case Body.Name.Venus: if (zodiac_house.value == ZodiacHouse.Name.Lib) return true; break;
				case Body.Name.Saturn: if (zodiac_house.value == ZodiacHouse.Name.Aqu) return true; break;
				case Body.Name.Rahu: if (zodiac_house.value == ZodiacHouse.Name.Vir) return true; break;
				case Body.Name.Ketu: if (zodiac_house.value == ZodiacHouse.Name.Pis) return true; break;
			}
			return false;
		}
		public bool isInOwnHouse ()
		{
			ZodiacHouse.Name zh = zodiac_house.value;
			switch (this.name)
			{
				case Body.Name.Sun: if (zh == ZodiacHouse.Name.Leo) return true; break;
				case Body.Name.Moon: if (zh == ZodiacHouse.Name.Tau) return true; break;
				case Body.Name.Mars: if (zh == ZodiacHouse.Name.Ari || zh == ZodiacHouse.Name.Sco) return true; break;
				case Body.Name.Mercury: if (zh == ZodiacHouse.Name.Gem || zh == ZodiacHouse.Name.Vir) return true; break;
				case Body.Name.Jupiter: if (zh == ZodiacHouse.Name.Sag || zh == ZodiacHouse.Name.Pis) return true; break;
				case Body.Name.Venus: if (zh == ZodiacHouse.Name.Tau || zh == ZodiacHouse.Name.Lib) return true; break;
				case Body.Name.Saturn: if (zh == ZodiacHouse.Name.Cap || zh == ZodiacHouse.Name.Aqu) return true; break;
				case Body.Name.Rahu: if (zh == ZodiacHouse.Name.Aqu) return true; break;
				case Body.Name.Ketu: if (zh == ZodiacHouse.Name.Sco) return true; break;
			}
			return false;
		}
		public bool isExaltedPhalita () 
		{
			switch (this.name)
			{
				case Body.Name.Sun: if (zodiac_house.value == ZodiacHouse.Name.Ari) return true; break;
				case Body.Name.Moon: if (zodiac_house.value == ZodiacHouse.Name.Tau) return true; break;
				case Body.Name.Mars: if (zodiac_house.value == ZodiacHouse.Name.Cap) return true; break;
				case Body.Name.Mercury: if (zodiac_house.value == ZodiacHouse.Name.Vir) return true; break;
				case Body.Name.Jupiter: if (zodiac_house.value == ZodiacHouse.Name.Can) return true; break;
				case Body.Name.Venus: if (zodiac_house.value == ZodiacHouse.Name.Pis) return true; break;
				case Body.Name.Saturn: if (zodiac_house.value == ZodiacHouse.Name.Lib) return true; break;
				case Body.Name.Rahu: if (zodiac_house.value == ZodiacHouse.Name.Gem) return true; break;
				case Body.Name.Ketu: if (zodiac_house.value == ZodiacHouse.Name.Sag) return true; break;
			}
			return false;
		}
		public bool isDebilitatedPhalita () 
		{
			switch (this.name)
			{
				case Body.Name.Sun: if (zodiac_house.value == ZodiacHouse.Name.Lib) return true; break;
				case Body.Name.Moon: if (zodiac_house.value == ZodiacHouse.Name.Sco) return true; break;
				case Body.Name.Mars: if (zodiac_house.value == ZodiacHouse.Name.Can) return true; break;
				case Body.Name.Mercury: if (zodiac_house.value == ZodiacHouse.Name.Pis) return true; break;
				case Body.Name.Jupiter: if (zodiac_house.value == ZodiacHouse.Name.Cap) return true; break;
				case Body.Name.Venus: if (zodiac_house.value == ZodiacHouse.Name.Vir) return true; break;
				case Body.Name.Saturn: if (zodiac_house.value == ZodiacHouse.Name.Ari) return true; break;
				case Body.Name.Rahu: if (zodiac_house.value == ZodiacHouse.Name.Sag) return true; break;
				case Body.Name.Ketu: if (zodiac_house.value == ZodiacHouse.Name.Gem) return true; break;
			}
			return false;
		}
		public bool GrahaDristi (ZodiacHouse h)
		{
			int num = zodiac_house.numHousesBetween(h);
			if (num == 7) return true;

			if (name == Body.Name.Jupiter && (num == 5 || num == 9)) return true;
			if (name == Body.Name.Rahu && (num == 5 || num == 9 || num == 2)) return true;
			if (name == Body.Name.Mars && (num == 4 || num == 8)) return true;
			if (name == Body.Name.Saturn && (num == 3 || num == 10)) return true;

			return false;
		}
	}

	/// <summary>
	/// Specifies a BodyPosition, i.e. the astronomical characteristics of a body like
	/// longitude, speed etc. It has no notion of its "rasi".
	/// The functions to convert this to a DivisionType (the various vargas) 
	/// are all implemented here
	/// </summary>
	public class BodyPosition: ICloneable
	{
		Longitude m_lon;
		private double m_splon, m_lat, m_splat, m_dist, m_spdist;
		public string otherString;
		public Body.Name name;
		public BodyType.Name type;
		public Horoscope h;
		public Longitude longitude { get { return m_lon; } set { m_lon = value; } }
		public double latitude { get { return m_lat; } set { m_lat = value; } }
		public double distance { get { return m_dist; } set { m_dist = value; } }
		public double speed_longitude { get { return m_splon; } set { m_splon = value; } }
		public double speed_latitude { get { return m_splat; } set { m_splat = value; } }
		public double speed_distance { get { return m_spdist; } set { m_spdist = value; } }
		public BodyPosition (Horoscope _h, Body.Name aname, BodyType.Name atype, Longitude lon, double lat, double dist, double splon, double splat, double spdist) 
		{
			this.longitude = lon;
			m_lat = lat;
			m_dist = dist;
			m_splon = splon;
			m_splat = splat;
			m_spdist = spdist;
			name = aname;
			type = atype;
			h = _h;
			//Console.WriteLine ("{0} {1} {2}", aname.ToString(), lon.value, splon);
		}
		public object Clone ()
		{
			BodyPosition bp = new BodyPosition(h, name, type, m_lon.add(0), m_lat, 
				m_dist, m_splon, m_splat, m_spdist);
			bp.otherString = this.otherString;
			return bp;
		}

		public int partOfZodiacHouse (int n)
		{
			Longitude l = this.longitude;
			double offset = l.toZodiacHouseOffset();
			int part = (int)(Math.Floor (offset / (30.0 / n))) + 1;
			Trace.Assert (part >= 1 && part <= n);
			return part;
		}
		private DivisionPosition populateRegularCusps (int n, DivisionPosition dp)
		{
			int part = partOfZodiacHouse(n);
			double cusp_length = 30.0 / ((double)n);
			dp.cusp_lower = ((double)(part-1))*cusp_length;
			dp.cusp_lower += m_lon.toZodiacHouseBase();
			dp.cusp_higher = dp.cusp_lower + cusp_length;
			dp.part = part;

			//if (dp.type == BodyType.Name.Graha || dp.type == BodyType.Name.Lagna)
			//Console.WriteLine ("D: {0} {1} {2} {3} {4} {5}", 
			//	n, dp.name, cusp_length,
			//	dp.cusp_lower, m_lon.value, dp.cusp_higher);


			return dp;
		}

		/// <summary>
		/// Many of the varga divisions (like navamsa) are regular divisions,
		/// and can be implemented by a single method. We do this when possible.
		/// </summary>
		/// <param name="n">The number of parts a house is divided into</param>
		/// <returns>The DivisionPosition the body falls into</returns>
		/// 
		private DivisionPosition toRegularDivisionPosition (int n)
		{
			int zhouse = (int)(m_lon.toZodiacHouse().value);
			int num_parts = (((int)zhouse) - 1) * n + partOfZodiacHouse (n);
			ZodiacHouse div_house = (new ZodiacHouse (ZodiacHouse.Name.Ari)).add (num_parts);
			DivisionPosition dp = new DivisionPosition (name, type, div_house, 0, 0, 0);
			populateRegularCusps (n, dp);
			return dp;
		}

		private DivisionPosition toRegularDivisionPositionFromCurrentHouseOddEven (int n)
		{
			int zhouse = (int)(m_lon.toZodiacHouse().value);
			int num_parts = partOfZodiacHouse (n);
			ZodiacHouse div_house = m_lon.toZodiacHouse().add(num_parts);
			DivisionPosition dp = new DivisionPosition (name, type, div_house, 0, 0, 0);
			populateRegularCusps (n, dp);
			return dp;
		}


		private DivisionPosition toBhavaDivisionPositionRasi (Longitude[] cusps)
		{
			Debug.Assert (cusps.Length == 13);
			cusps[12] = cusps[0];
			for (int i = 0; i<12; i++)
			{
				if (m_lon.sub(cusps[i]).value <= cusps[i+1].sub(cusps[i]).value)
					return new DivisionPosition (name, type, new ZodiacHouse((ZodiacHouse.Name)i+1),
						cusps[i].value, cusps[i+1].value, 1);
			}
			throw new Exception();
		}
		private DivisionPosition toBhavaDivisionPositionHouse (Longitude[] cusps)
		{
			Debug.Assert (cusps.Length == 13);

			ZodiacHouse zlagna = h.getPosition(Body.Name.Lagna).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house;
			for (int i = 0; i<12; i++)
			{
				if (m_lon.sub(cusps[i]).value < cusps[i+1].sub(cusps[i]).value)
				{
					//Console.WriteLine ("Found {4} - {0} in cusp {3} between {1} and {2}", this.m_lon.value,
					//	cusps[i].value, cusps[i+1].value, i+1, this.name.ToString());

					return new DivisionPosition (name, type, 
						zlagna.add(i+1), cusps[i].value, cusps[i+1].value, 1);
				}
			}
			return new DivisionPosition (name, type, 
				zlagna.add(1), cusps[0].value, cusps[1].value, 1);
		}
		private DivisionPosition toDivisionPositionBhavaEqual()
		{
			double offset = h.getPosition(Body.Name.Lagna).longitude.toZodiacHouseOffset();
			Longitude[] cusps = new Longitude[13];
			for (int i=0; i<12; i++)
				cusps[i] = new Longitude(i*30.0+offset-15.0);
			return this.toBhavaDivisionPositionRasi(cusps);
		}
		private DivisionPosition toDivisionPositionBhavaPada()
		{
			Longitude[] cusps = new Longitude[13];
			double offset = h.getPosition(Body.Name.Lagna).longitude.toZodiacHouseOffset();
			int padasOffset = (int)Math.Floor(offset / (360.0/108.0));
			double startOffset = (double)padasOffset * (360.0/108.0);

			for (int i=0; i<12; i++)
				cusps[i] = new Longitude(i*30.0+startOffset-15.0);
			return this.toBhavaDivisionPositionRasi(cusps);
		}
		private DivisionPosition toDivisionPositionBhavaHelper(int hsys)
		{
			Longitude[] cusps = new Longitude[13];
			double[] dCusps = new double[13];
			double[] ascmc = new double[10];

			if (hsys != h.swephHouseSystem)
			{
				h.swephHouseSystem = hsys;
				h.populateHouseCusps();
			}
			return this.toBhavaDivisionPositionHouse(h.swephHouseCusps);
		}
		private bool HoraSunDayNight ()
		{
			int sign = (int)m_lon.toZodiacHouse().value;
			int part = partOfZodiacHouse(2);
			if (m_lon.toZodiacHouse().isDaySign())
			{
				if (part == 1) return true;
				return false;
			} 
			else 
			{
				if (part == 1) return false;
				return true;
			}
		}
		private bool HoraSunOddEven ()
		{
			int sign = (int)m_lon.toZodiacHouse().value;
			int part = partOfZodiacHouse(2);
			int mod = sign % 2;
			switch (mod)
			{
				case 1:
					if (part == 1) return true;
					return false;
				default:
					if (part == 1) return false;
					return true;
			}
		}
		private DivisionPosition toDivisionPositionHoraKashinath()
		{
			int[] daySigns = new int[13]{0,8,7,6,5,5,6,7,8,12,11,11,12};
			int[] nightSigns = new int[13]{0,1,2,3,4,4,3,2,1,9,10,10,9};

			ZodiacHouse zh;
			int sign = (int)m_lon.toZodiacHouse().value;
			if (this.HoraSunOddEven()) zh = new ZodiacHouse ((ZodiacHouse.Name)daySigns[sign]);
			else zh = new ZodiacHouse ((ZodiacHouse.Name)nightSigns[sign]);
			DivisionPosition dp = new DivisionPosition(name, type, zh, 0, 0, 0);
			this.populateRegularCusps(2, dp);
			return dp;
		}
		private DivisionPosition toDivisionPositionHoraJagannath()
		{
			ZodiacHouse zh = m_lon.toZodiacHouse();

			Console.WriteLine ("{2} in {3}: OddEven is {0}, DayNight is {1}",
				this.HoraSunOddEven(), this.HoraSunDayNight(), this.name, zh.value);

			if (this.HoraSunDayNight() && false == this.HoraSunOddEven())
				zh = zh.add (7);
			else if (false == this.HoraSunDayNight() && true == this.HoraSunOddEven())
				zh = zh.add(7);

			Console.WriteLine ("{0} ends in {1}", this.name, zh.value);
			
			DivisionPosition dp = new DivisionPosition(name, type, zh, 0, 0, 0);
			this.populateRegularCusps(2, dp);
			return dp;
		}
		private DivisionPosition toDivisionPositionHoraParasara()
		{
			ZodiacHouse zh;
			int ruler_index = 0;
			if (this.HoraSunOddEven()) 
			{
				zh = new ZodiacHouse(ZodiacHouse.Name.Leo);
				ruler_index = 1;
			}
			else 
			{
				zh = new ZodiacHouse(ZodiacHouse.Name.Can);
				ruler_index = 2;
			}
			DivisionPosition dp = new DivisionPosition(name, type, zh, 0, 0, 0);
			dp.ruler_index = ruler_index;
			return this.populateRegularCusps(2, dp);
		}
		private DivisionPosition toDivisionPositionDrekanna (int n)
		{
			int[] offset = new int[4] { 9, 1, 5, 9};
			ZodiacHouse zhouse = m_lon.toZodiacHouse();
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = zhouse.add (offset[part%3]);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			populateRegularCusps (n, dp);
			if (n == 3)
			{
				int ruler_index = (int)dp.zodiac_house.value % 3;
				if (ruler_index == 0) ruler_index = 3;
				dp.ruler_index = ruler_index;
			}
			return dp;
		}
		private DivisionPosition toDivisionPositionDrekannaJagannath ()
		{
			ZodiacHouse zh = m_lon.toZodiacHouse();
			ZodiacHouse zhm;
			ZodiacHouse dhouse;
			int mod = ((int)(m_lon.toZodiacHouse().value)) % 3;
			// Find moveable sign in trines
			switch (mod) 
			{
				case 1: zhm = zh.add (1); break;
				case 2: zhm = zh.add (9); break;
				default: zhm = zh.add (5); break;
			}

			// From moveable sign, 3 parts belong to the trines
			int part = partOfZodiacHouse(3);
			switch (part)
			{
				case 1: dhouse = zhm.add (1); break;
				case 2: dhouse = zhm.add (5); break;
				default: dhouse = zhm.add (9); break;
			}
			
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			return this.populateRegularCusps(3, dp);
		}
		private DivisionPosition toDivisionPositionDrekkanaSomnath()
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 2;
			int part = partOfZodiacHouse (3);
			ZodiacHouse zh = m_lon.toZodiacHouse();
			int p = (int)zh.value;

			if (mod == 0) p--;
			p = (p-1)/2;
			int num_done = p*3;

			ZodiacHouse zh1 = new ZodiacHouse(ZodiacHouse.Name.Ari);
			ZodiacHouse zh2;
			if (mod == 1) zh2 = zh1.add (num_done + part);
			else zh2 = zh1.addReverse(num_done + part + 1);
			DivisionPosition dp = new DivisionPosition (name, type, zh2, 0, 0, 0);
			return this.populateRegularCusps(3, dp);
		}
		private DivisionPosition toDivisionPositionChaturthamsa (int n)
		{
			int[] offset = new int[5] { 10, 1, 4, 7, 10};
			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = zhouse.add (offset[part%4]);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse,0, 0, 0);
			if (n == 4)
				dp.ruler_index = part;
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionShashthamsa (int n)
		{
			int mod = ((int)(m_lon.toZodiacHouse().value)) % 2;
			ZodiacHouse.Name dhousen = (mod % 2 == 1) ? ZodiacHouse.Name.Ari : ZodiacHouse.Name.Lib;				
			ZodiacHouse dhouse = (new ZodiacHouse (dhousen)).add (partOfZodiacHouse (n));
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionSaptamsa (int n)
		{
			int part = partOfZodiacHouse(n);
			ZodiacHouse zh = m_lon.toZodiacHouse();
			if (false == zh.isOdd())
				zh = zh.add(7);
			zh = zh.add(part);
			DivisionPosition dp = new DivisionPosition(name, type, zh, 0, 0, 0);
		
			if (n == 7)
			{
				if (this.longitude.toZodiacHouse().isOdd())
					dp.ruler_index = part;
				else
					dp.ruler_index = 8-part;
			}
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionNavamsa ()
		{
			int part = partOfZodiacHouse(9);
			DivisionPosition dp = this.toRegularDivisionPosition(9);
			switch ((int)this.longitude.toZodiacHouse().value % 3)
			{
				case 1: dp.ruler_index = part; break;
				case 2: dp.ruler_index = part+1; break;
				case 0: dp.ruler_index = part+2; break;
			}
			while (dp.ruler_index > 3) dp.ruler_index -= 3;
			return dp;
		}
		private DivisionPosition toDivisionPositionAshtamsaRaman ()
		{
			ZodiacHouse zstart = null;
			switch ((int)m_lon.toZodiacHouse().value % 3)
			{
				case 1: zstart = new ZodiacHouse(ZodiacHouse.Name.Ari); break;
				case 2: zstart = new ZodiacHouse(ZodiacHouse.Name.Leo); break;
				case 0: default:
					zstart = new ZodiacHouse(ZodiacHouse.Name.Sag); break;
			}
			ZodiacHouse dhouse = zstart.add (partOfZodiacHouse (8));
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			return this.populateRegularCusps(8, dp);
		}
		private DivisionPosition toDivisionPositionPanchamsa ()
		{
			ZodiacHouse.Name[] offset_odd = new ZodiacHouse.Name[5] 
						{
							ZodiacHouse.Name.Ari, ZodiacHouse.Name.Aqu, ZodiacHouse.Name.Sag,
							ZodiacHouse.Name.Gem, ZodiacHouse.Name.Lib};
			ZodiacHouse.Name[] offset_even = new ZodiacHouse.Name[5] 
						{
							ZodiacHouse.Name.Tau, ZodiacHouse.Name.Vir, ZodiacHouse.Name.Pis,
							ZodiacHouse.Name.Cap, ZodiacHouse.Name.Sco};
			int part = partOfZodiacHouse(5);
			int mod = ((int)(m_lon.toZodiacHouse().value)) % 2;
			ZodiacHouse.Name dhouse = (mod % 2 == 1) ? offset_odd[part-1] : offset_even[part-1];
			DivisionPosition dp = new DivisionPosition (name, type, new ZodiacHouse (dhouse), 0, 0, 0);	 
			return this.populateRegularCusps(5, dp);
		}
		private DivisionPosition toDivisionPositionRudramsa ()
		{
			ZodiacHouse zari = new ZodiacHouse(ZodiacHouse.Name.Ari);
			ZodiacHouse zhouse = m_lon.toZodiacHouse();
			int diff = zari.numHousesBetween(zhouse);
			ZodiacHouse zstart = zari.addReverse(diff);
			int part = partOfZodiacHouse(11);
			ZodiacHouse zend = zstart.add(part);
			DivisionPosition dp = new DivisionPosition (name, type, zend, 0, 0, 0);
			return this.populateRegularCusps(11, dp);
		}
		private DivisionPosition toDivisionPositionRudramsaRaman ()
		{
			ZodiacHouse zhstart = m_lon.toZodiacHouse().add(12);
			int part = partOfZodiacHouse(11);
			ZodiacHouse zend = zhstart.addReverse(part);
			DivisionPosition dp = new DivisionPosition (name, type, zend, 0, 0, 0);
			return this.populateRegularCusps(11, dp);
		}
		private DivisionPosition toDivisionPositionDasamsa (int n)
		{
			int[] offset = new int[2] { 9, 1 };
			ZodiacHouse zhouse = m_lon.toZodiacHouse();
			ZodiacHouse dhouse = zhouse.add(offset[((int)zhouse.value) % 2]);
			int part = partOfZodiacHouse(n);
			dhouse = dhouse.add(part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			if (n == 10)
			{
				if (this.longitude.toZodiacHouse().isOdd())
					dp.ruler_index = part;
				else
					dp.ruler_index = 11-part;
			}
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionDwadasamsa (int n)
		{
			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			int part = partOfZodiacHouse (n);
			ZodiacHouse dhouse = zhouse.add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			if (n == 12)
				dp.ruler_index = Basics.normalize_inc(1, 4, part);
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionShodasamsa ()
		{
			int part = partOfZodiacHouse (16);
			DivisionPosition dp = this.toRegularDivisionPosition(16);
			int ruler = part;
			if (this.longitude.toZodiacHouse().isOdd())
				ruler = part;
			else 
				ruler = 17-part;
			dp.ruler_index = Basics.normalize_inc(1, 4, ruler);
			return dp;
		}
		private DivisionPosition toDivisionPositionVimsamsa (int n)
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 3;
			ZodiacHouse.Name dhousename;
			switch (mod) 
			{
				case 1: dhousename = ZodiacHouse.Name.Ari; break;
				case 2: dhousename = ZodiacHouse.Name.Sag; break;
				default: dhousename = ZodiacHouse.Name.Leo; break;
			}
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = (new ZodiacHouse (dhousename)).add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			return this.populateRegularCusps (n, dp);
		}
		private DivisionPosition toDivisionPositionVimsamsa ()
		{
			int part = partOfZodiacHouse (20);
			DivisionPosition dp = this.toRegularDivisionPosition(20);
			if (this.longitude.toZodiacHouse().isOdd())
				dp.ruler_index = part;
			else 
				dp.ruler_index = 20+part;
			return dp;
		}
		private DivisionPosition toDivisionPositionChaturvimsamsa (int n)
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 2;
			ZodiacHouse.Name dhousename = (mod % 2 == 1) ? ZodiacHouse.Name.Leo : ZodiacHouse.Name.Can;
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = (new ZodiacHouse (dhousename)).add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			if (n == 24)
			{
				if (this.longitude.toZodiacHouse().isOdd())
					dp.ruler_index = part;
				else
					dp.ruler_index = 25-part;
				dp.ruler_index = Basics.normalize_inc (1, 12, dp.ruler_index);
			}
			return this.populateRegularCusps(n, dp);
		}
		private DivisionPosition toDivisionPositionNakshatramsa (int n)
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 4;
			ZodiacHouse.Name dhousename;
			switch (mod) 
			{
				case 1: dhousename = ZodiacHouse.Name.Ari; break;
				case 2: dhousename = ZodiacHouse.Name.Can; break;
				case 3: dhousename = ZodiacHouse.Name.Lib; break;
				default: dhousename = ZodiacHouse.Name.Cap; break;
			}
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = (new ZodiacHouse (dhousename)).add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			return this.populateRegularCusps (n, dp);
		}
		private DivisionPosition toDivisionPositionNakshatramsa ()
		{
			DivisionPosition dp = this.toRegularDivisionPosition(27);
			dp.ruler_index = this.partOfZodiacHouse(27);
			return dp;
		}
		private DivisionPosition toDivisionPositionTrimsamsaSimple ()
		{
			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			int part = partOfZodiacHouse (30);
			ZodiacHouse dhouse = zhouse.add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			return this.populateRegularCusps(30, dp);
		}
		private DivisionPosition toDivisionPositionTrimsamsa ()
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 2;
			double off = m_lon.toZodiacHouseOffset();
			ZodiacHouse dhouse;
			double cusp_lower = 0;
			double cusp_higher = 0;
			int ruler_index = 0;
			int part = 0;
			if (mod == 1) 
			{
				if (off <= 5) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Ari);
					cusp_lower = 0.0; cusp_higher = 5.0; 
					ruler_index = 1;
					part = 1;
				}
				else if (off <= 10) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Aqu);
					cusp_lower = 5.01; cusp_higher = 10.0;
					ruler_index = 2;
					part = 2;
				}
				else if (off <= 18) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Sag);
					cusp_lower = 10.01; cusp_higher = 18.0; 
					ruler_index = 3;
					part = 3;
				}
				else if (off <= 25) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Gem);
					cusp_lower = 18.01; cusp_higher = 25.0;
					ruler_index = 4;
					part = 4;
				}
				else 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Lib);					  
					cusp_lower = 25.01; cusp_higher = 30.0;
					ruler_index = 5;
					part = 5;
				}
			}
			else
			{
				if (off <= 5) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Tau);
					cusp_lower = 0.0; cusp_higher = 5.0; 
					ruler_index = 5;
					part = 1;
				}
				else if (off <= 12) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Vir);
					cusp_lower = 5.01; cusp_higher = 12.0;
					ruler_index = 4;
					part = 2;
				}
				else if (off <= 20) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Pis);
					cusp_lower = 12.01; cusp_higher = 20.0;
					ruler_index = 3;
					part = 3;
				}
				else if (off <= 25) 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Cap);
					cusp_lower = 20.01; cusp_higher = 25.0;
					ruler_index = 2;
					part = 4;
				}
				else 
				{
					dhouse = new ZodiacHouse(ZodiacHouse.Name.Sco);					  
					cusp_lower = 25.01; cusp_higher = 30.0;
					ruler_index = 1;
					part = 5;
				}
			}
			cusp_lower += m_lon.toZodiacHouseBase();
			cusp_higher += m_lon.toZodiacHouseBase();

			DivisionPosition dp = new DivisionPosition(name, type, dhouse, cusp_lower, cusp_higher, 0);
			dp.ruler_index = ruler_index;
			dp.part = part;
			return dp;
		}
		private DivisionPosition toDivisionPositionKhavedamsa ()
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 2;
			ZodiacHouse.Name dhousename = (mod % 2 == 1) ? ZodiacHouse.Name.Ari : ZodiacHouse.Name.Lib;
			int part = partOfZodiacHouse(40);
			ZodiacHouse dhouse = (new ZodiacHouse (dhousename)).add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			dp.ruler_index = Basics.normalize_inc(1, 12, part);
			return this.populateRegularCusps(40, dp);
		}
		private DivisionPosition toDivisionPositionAkshavedamsa(int n)
		{
			int mod = ((int)(m_lon.toZodiacHouse()).value) % 3;
			ZodiacHouse.Name dhousename;
			switch (mod) 
			{
				case 1: dhousename = ZodiacHouse.Name.Ari; break;
				case 2: dhousename = ZodiacHouse.Name.Leo; break;
				default: dhousename = ZodiacHouse.Name.Sag; break;
			}
			int part = partOfZodiacHouse(n);
			ZodiacHouse dhouse = (new ZodiacHouse (dhousename)).add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);	 
			if (n == 45)
			{
				switch ((int)this.longitude.toZodiacHouse().value % 3)
				{
					case 1: dp.ruler_index = part; break;
					case 2: dp.ruler_index = part+1; break;
					case 0: dp.ruler_index = part+2; break;
				}
				dp.ruler_index = Basics.normalize_inc(1,3,dp.ruler_index);
			}
			return this.populateRegularCusps (n, dp);
		}
		private DivisionPosition toDivisionPositionShashtyamsa ()
		{
			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			int part = partOfZodiacHouse(60);
			ZodiacHouse dhouse = zhouse.add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			if (this.longitude.toZodiacHouse().isOdd())
				dp.ruler_index = part;
			else
				dp.ruler_index = 61-part;
			return this.populateRegularCusps (60, dp);
		}
		private DivisionPosition toDivisionPositionNadiamsa ()
		{
#if DND
			ZodiacHouse zhouse = m_lon.toZodiacHouse();
			int part = partOfZodiacHouse(150);
			ZodiacHouse dhouse = null;
			switch ((int)zhouse.value % 3)
			{
				case 1:	dhouse = zhouse.add(part); break;
				case 2:	dhouse = zhouse.addReverse(part); break;
				default:
				case 0:
					dhouse = zhouse.add(part-75); break;
			}
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
#endif
			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			int part = partOfZodiacHouse(150);
			ZodiacHouse dhouse = zhouse.add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
			switch ((int)this.longitude.toZodiacHouse().value%3)
			{
				case 1: dp.ruler_index = part; break;
				case 2: dp.ruler_index = 151 - part; break;
				case 0: dp.ruler_index = Basics.normalize_inc(1,150,75 + part); break;
			}
			return this.populateRegularCusps(150, dp);	
		}
		static bool mbNadiamsaCKNCalculated = false;
		static double[] mNadiamsaCusps = null;
		private void calculateNadiamsaCusps ()
		{
			if (true == BodyPosition.mbNadiamsaCKNCalculated)
				return;

			int[] bases = new int[] { 1,2,3,4,7,9,10,12,16,20,24,27,30,40,45,60 };
			ArrayList alUnsorted = new ArrayList(150);
			foreach (int iVarga in bases)
			{
				for (int i=0; i<iVarga; i++)
					alUnsorted.Add((double)i/(double)iVarga*(double)30.0);
			}
			alUnsorted.Add ((double)30.0);
			alUnsorted.Sort();
			ArrayList alSorted = new ArrayList(150);

			alSorted.Add((double)0.0);
			for (int i=0; i<alUnsorted.Count; i++)
			{
				if ((double)alUnsorted[i] != (double)alSorted[alSorted.Count-1]) 
					alSorted.Add(alUnsorted[i]);
			}

			Debug.Assert (alSorted.Count == 151, 
				String.Format ("Found {0} Nadis. Expected 151.", alSorted.Count));
			
			mNadiamsaCusps = (double[])alSorted.ToArray(typeof(double));
			BodyPosition.mbNadiamsaCKNCalculated = true;
		}
		private DivisionPosition toDivisionPositionNadiamsaCKN ()
		{
			this.calculateNadiamsaCusps();
			int part = partOfZodiacHouse(150) - 10;
			if (part < 0) part = 0;

			for (; part < 149; part++)
			{
				Longitude lonLow = new Longitude(BodyPosition.mNadiamsaCusps[part]);
				Longitude lonHigh = new Longitude(BodyPosition.mNadiamsaCusps[part+1]);
				Longitude offset = new Longitude(this.longitude.toZodiacHouseOffset());

				if (offset.sub(lonLow).value <= lonHigh.sub(lonLow).value)
					break;
			}
			part++;

#if DND
			ZodiacHouse zhouse = m_lon.toZodiacHouse();
			ZodiacHouse dhouse = null;
			switch ((int)zhouse.value % 3)
			{
				case 1:	dhouse = zhouse.add(part); break;
				case 2:	dhouse = zhouse.addReverse(part); break;
				default:
				case 0:
					dhouse = zhouse.add(part-75); break;
			}
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);
#endif

			ZodiacHouse zhouse = m_lon.toZodiacHouse ();
			ZodiacHouse dhouse = zhouse.add (part);
			DivisionPosition dp = new DivisionPosition (name, type, dhouse, 0, 0, 0);


			switch ((int)this.longitude.toZodiacHouse().value%3)
			{
				case 1: dp.ruler_index = part; break;
				case 2: dp.ruler_index = 151 - part; break;
				case 0: dp.ruler_index = Basics.normalize_inc(1,150,75 + part); break;
			}
			dp.cusp_lower = this.longitude.toZodiacHouseBase() +  BodyPosition.mNadiamsaCusps[part-1];
			dp.cusp_higher = this.longitude.toZodiacHouseBase() +  BodyPosition.mNadiamsaCusps[part];
			dp.part = part;
			return dp;
		}
		private DivisionPosition toDivisionPositionNavamsaDwadasamsa ()
		{
			BodyPosition bp = (BodyPosition)this.Clone();
			bp.longitude = bp.extrapolateLongitude(new Division(Basics.DivisionType.Navamsa));
			DivisionPosition dp = bp.toDivisionPositionDwadasamsa(12);
			this.populateRegularCusps(108, dp);
			return dp;
		}
		private DivisionPosition toDivisionPositionDwadasamsaDwadasamsa ()
		{
			BodyPosition bp = (BodyPosition)this.Clone();
			bp.longitude = bp.extrapolateLongitude(new Division(Basics.DivisionType.Dwadasamsa));
			DivisionPosition dp = bp.toDivisionPositionDwadasamsa(12);
			this.populateRegularCusps(144, dp);
			return dp;
		}
		/// <summary>
		/// Calculated any known Varga positions. Simply calls the appropriate
		/// helper function
		/// </summary>
		/// <param name="dtype">The requested DivisionType</param>
		/// <returns>A division Position</returns>
		

		public DivisionPosition toDivisionPosition (Division d)
		{
			BodyPosition bp = (BodyPosition)this.Clone();
			DivisionPosition dp = null;
			for (int i=0; i<d.MultipleDivisions.Length; i++)
			{
				dp = bp.toDivisionPosition(d.MultipleDivisions[i]);
				bp.longitude = bp.extrapolateLongitude(d.MultipleDivisions[i]);
			}
			return dp;
		}
		public DivisionPosition toDivisionPosition (Division.SingleDivision d) 
		{
			if (d.NumParts < 1)
				d.NumParts = 1;

			switch (d.Varga) 
			{
				case Basics.DivisionType.Rasi:
					return toRegularDivisionPosition (1);
				case Basics.DivisionType.BhavaPada:
					return toDivisionPositionBhavaPada ();
				case Basics.DivisionType.BhavaEqual:
					return toDivisionPositionBhavaEqual ();
				case Basics.DivisionType.BhavaSripati:
					return this.toDivisionPositionBhavaHelper('O');
				case Basics.DivisionType.BhavaKoch:
					return this.toDivisionPositionBhavaHelper('K');
				case Basics.DivisionType.BhavaPlacidus:
					return this.toDivisionPositionBhavaHelper('P');	
				case Basics.DivisionType.BhavaCampanus:
					return this.toDivisionPositionBhavaHelper('C');	
				case Basics.DivisionType.BhavaRegiomontanus:
					return this.toDivisionPositionBhavaHelper('R');	
				case Basics.DivisionType.BhavaAlcabitus:
					return this.toDivisionPositionBhavaHelper('B');	
				case Basics.DivisionType.BhavaAxial:
					return this.toDivisionPositionBhavaHelper('X');	
				case Basics.DivisionType.HoraParivrittiDwaya:
					return toRegularDivisionPosition(2);
				case Basics.DivisionType.HoraKashinath:
					return toDivisionPositionHoraKashinath();
				case Basics.DivisionType.HoraParasara:
					return toDivisionPositionHoraParasara();
				case Basics.DivisionType.HoraJagannath:
					return toDivisionPositionHoraJagannath();
				case Basics.DivisionType.DrekkanaParasara:
					return toDivisionPositionDrekanna(3);
				case Basics.DivisionType.DrekkanaJagannath:
					return toDivisionPositionDrekannaJagannath();
				case Basics.DivisionType.DrekkanaParivrittitraya:
					return toRegularDivisionPosition(3);
				case Basics.DivisionType.DrekkanaSomnath:
					return toDivisionPositionDrekkanaSomnath();
				case Basics.DivisionType.Chaturthamsa:
					return toDivisionPositionChaturthamsa(4);
				case Basics.DivisionType.Panchamsa:
					return toDivisionPositionPanchamsa();
				case Basics.DivisionType.Shashthamsa:
					return toDivisionPositionShashthamsa(6);
				case Basics.DivisionType.Saptamsa:
					return toDivisionPositionSaptamsa(7);
				case Basics.DivisionType.Ashtamsa:
					return toRegularDivisionPosition (8);
				case Basics.DivisionType.AshtamsaRaman:
					return toDivisionPositionAshtamsaRaman();
				case Basics.DivisionType.Navamsa:
					return toDivisionPositionNavamsa();
				case Basics.DivisionType.Dasamsa:
					return toDivisionPositionDasamsa(10);
				case Basics.DivisionType.Rudramsa:
					return toDivisionPositionRudramsa();
				case Basics.DivisionType.RudramsaRaman:
					return toDivisionPositionRudramsaRaman();
				case Basics.DivisionType.Dwadasamsa:
					return toDivisionPositionDwadasamsa(12);
				case Basics.DivisionType.Shodasamsa:
					return toDivisionPositionShodasamsa();
				case Basics.DivisionType.Vimsamsa:
					return toDivisionPositionVimsamsa();
				case Basics.DivisionType.Chaturvimsamsa:
					return toDivisionPositionChaturvimsamsa(24);
				case Basics.DivisionType.Nakshatramsa:
					return toDivisionPositionNakshatramsa();
				case Basics.DivisionType.Trimsamsa:
					return toDivisionPositionTrimsamsa();
				case Basics.DivisionType.TrimsamsaParivritti:
					return toRegularDivisionPosition (30);
				case Basics.DivisionType.TrimsamsaSimple:
					return toDivisionPositionTrimsamsaSimple();
				case Basics.DivisionType.Khavedamsa:
					return toDivisionPositionKhavedamsa();
				case Basics.DivisionType.Akshavedamsa:
					return toDivisionPositionAkshavedamsa(45);
				case Basics.DivisionType.Shashtyamsa:
					return toDivisionPositionShashtyamsa();
				case Basics.DivisionType.Ashtottaramsa:
					return toRegularDivisionPosition (108);
				case Basics.DivisionType.Nadiamsa:
					return toDivisionPositionNadiamsa();
				case Basics.DivisionType.NadiamsaCKN:
					return toDivisionPositionNadiamsaCKN();
				case Basics.DivisionType.NavamsaDwadasamsa:
					return toDivisionPositionNavamsaDwadasamsa();
				case Basics.DivisionType.DwadasamsaDwadasamsa:
					return toDivisionPositionDwadasamsaDwadasamsa();
				case Basics.DivisionType.GenericParivritti:
					return toRegularDivisionPosition (d.NumParts);
				case Basics.DivisionType.GenericShashthamsa:
					return toDivisionPositionShashthamsa (d.NumParts);
				case Basics.DivisionType.GenericSaptamsa:
					return toDivisionPositionSaptamsa (d.NumParts);
				case Basics.DivisionType.GenericDasamsa:
					return toDivisionPositionDasamsa (d.NumParts);
				case Basics.DivisionType.GenericDwadasamsa:
					return toDivisionPositionDwadasamsa (d.NumParts);
				case Basics.DivisionType.GenericChaturvimsamsa:
					return toDivisionPositionChaturvimsamsa (d.NumParts);
				case Basics.DivisionType.GenericChaturthamsa:
					return toDivisionPositionChaturthamsa (d.NumParts);
				case Basics.DivisionType.GenericNakshatramsa:
					return toDivisionPositionNakshatramsa (d.NumParts);
				case Basics.DivisionType.GenericDrekkana:
					return toDivisionPositionDrekanna (d.NumParts);
				case Basics.DivisionType.GenericShodasamsa:
					return toDivisionPositionAkshavedamsa (d.NumParts);
				case Basics.DivisionType.GenericVimsamsa:
					return toDivisionPositionVimsamsa (d.NumParts);
			}
			Trace.Assert (false, "DivisionPosition Error");
			return new DivisionPosition (name, type, new ZodiacHouse (ZodiacHouse.Name.Ari), 0, 0, 0);
		}
		public Longitude extrapolateLongitude (Division d)
		{
			BodyPosition bp = (BodyPosition)this.Clone();
			foreach (Division.SingleDivision dSingle in d.MultipleDivisions)
			{
				bp.longitude = this.extrapolateLongitude(dSingle);
			}
			return bp.longitude;
		}
		public Longitude extrapolateLongitude (Division.SingleDivision d)
		{
			DivisionPosition dp = this.toDivisionPosition(d);
			Longitude lOffset = this.longitude.sub(dp.cusp_lower);
			Longitude lRange = new Longitude(dp.cusp_higher).sub(dp.cusp_lower);
			Trace.Assert(lOffset.value <= lRange.value, "Extrapolation internal error: Slice smaller than range. Weird.");

			double newOffset = (lOffset.value / lRange.value)*30.0;
			double newBase = ((double)((int)dp.zodiac_house.value-1))*30.0;
			return (new Longitude(newOffset + newBase));
		}
	}

	/// <summary>
	/// A package related to a ZodiacHouse
	/// </summary>
	public class ZodiacHouse: ICloneable
	{
		private ZodiacHouse.Name m_zhouse;
		public ZodiacHouse.Name value { get { return m_zhouse; } set { m_zhouse = value; } }
		public ZodiacHouse (ZodiacHouse.Name zhouse) { m_zhouse = zhouse; }
		public enum Name :int
		{
			Ari=1, Tau=2, Gem=3, Can=4, Leo=5, Vir=6, 
			Lib=7, Sco=8, Sag=9, Cap=10, Aqu=11, Pis=12
		}
		public enum RiseType : int
		{
			RisesWithHead, RisesWithFoot, RisesWithBoth
		}
		static public ZodiacHouse.Name[] AllNames = new ZodiacHouse.Name[] 
		{
			Name.Ari, Name.Tau, Name.Gem, Name.Can, Name.Leo, Name.Vir,
			Name.Lib, Name.Sco, Name.Sag, Name.Cap, Name.Aqu, Name.Pis
		};
		public object Clone ()
		{
			return new ZodiacHouse(this.m_zhouse);
		}
		public override string ToString()
		{
			return value.ToString();
		}

		public int normalize () 
		{
			return Basics.normalize_inc (1, 12, (int)m_zhouse);
		}
		public ZodiacHouse add (int i) 
		{
			int znum = Basics.normalize_inc (1, 12, (int)(m_zhouse) + i - 1);
			return new ZodiacHouse ((ZodiacHouse.Name) znum);
		}
		public  ZodiacHouse addReverse (int i)
		{
			int znum = Basics.normalize_inc (1, 12, (int)(m_zhouse) - i + 1);
			return new ZodiacHouse ((ZodiacHouse.Name)znum);
		}
		public int numHousesBetweenReverse (ZodiacHouse zrel)
		{
			return Basics.normalize_inc (1, 12, (14 - this.numHousesBetween(zrel)));
		}
		public  int numHousesBetween ( ZodiacHouse zrel) 
		{	
			int ret = Basics.normalize_inc (1, 12, (int)zrel.value - (int)m_zhouse + 1);
			Trace.Assert (ret >= 1 && ret <= 12, "ZodiacHouse.numHousesBetween failed");
			return ret;
		}
		public bool isDaySign ()
		{
			switch (this.value)
			{
				case ZodiacHouse.Name.Ari:
				case ZodiacHouse.Name.Tau:
				case ZodiacHouse.Name.Gem:
				case ZodiacHouse.Name.Can:
					return false;
					
				case ZodiacHouse.Name.Leo:
				case ZodiacHouse.Name.Vir:
				case ZodiacHouse.Name.Lib:
				case ZodiacHouse.Name.Sco:
					return true;

				case ZodiacHouse.Name.Sag:
				case ZodiacHouse.Name.Cap:
					return false;

				case ZodiacHouse.Name.Aqu:
				case ZodiacHouse.Name.Pis:
					return true;

				default:
					Trace.Assert(false, "isDaySign internal error");
					return true;
			}
		}
		public bool isOdd()
		{
			switch (this.value)
			{
				case ZodiacHouse.Name.Ari:
				case ZodiacHouse.Name.Gem:
				case ZodiacHouse.Name.Leo:
				case ZodiacHouse.Name.Lib:
				case ZodiacHouse.Name.Sag:
				case ZodiacHouse.Name.Aqu:
					return true;

				case ZodiacHouse.Name.Tau:
				case ZodiacHouse.Name.Can:
				case ZodiacHouse.Name.Vir:
				case ZodiacHouse.Name.Sco:
				case ZodiacHouse.Name.Cap:
				case ZodiacHouse.Name.Pis:
					return false;

				default:
					Trace.Assert(false, "isOdd internal error");
					return true;
			}
		}
		public bool isOddFooted ()
		{
			switch (this.value) 
			{
				case ZodiacHouse.Name.Ari: return true;
				case ZodiacHouse.Name.Tau: return true;
				case ZodiacHouse.Name.Gem: return true;
				case ZodiacHouse.Name.Can: return false;
				case ZodiacHouse.Name.Leo: return false;
				case ZodiacHouse.Name.Vir: return false;
				case ZodiacHouse.Name.Lib: return true;
				case ZodiacHouse.Name.Sco: return true;
				case ZodiacHouse.Name.Sag: return true;
				case ZodiacHouse.Name.Cap: return false;
				case ZodiacHouse.Name.Aqu: return false;
				case ZodiacHouse.Name.Pis: return false;
			}
			Trace.Assert (false, "ZOdiacHouse::isOddFooted");
			return false;
		}
		public bool RasiDristi (ZodiacHouse b)
		{
			int ma = (int)value % 3;
			int mb = (int)b.value % 3;

			switch (ma)
			{
				case 1:
					if (mb == 2 && this.add(2).value != b.value) return true;
					return false;
				case 2:
					if (mb == 1 && this.addReverse(2).value != b.value) return true;
					return false;
				case 0:
					if (mb == 0) return true;
					return false;
			}

			Trace.Assert(false, "ZodiacHouse.RasiDristi");
			return false;
		}
		public RiseType RisesWith ()
		{
			switch (this.value)
			{
				case Name.Ari:
				case Name.Tau:
				case Name.Can:
				case Name.Sag:
				case Name.Cap:
					return RiseType.RisesWithFoot;
				case Name.Gem:
				case Name.Leo: 
				case Name.Vir:
				case Name.Lib:
				case Name.Sco:
				case Name.Aqu:
					return RiseType.RisesWithHead;
				default:
					return RiseType.RisesWithBoth;
			}
		}
		public ZodiacHouse LordsOtherSign ()
		{
			ZodiacHouse.Name ret = ZodiacHouse.Name.Ari;
			switch (this.value)
			{
				case ZodiacHouse.Name.Ari: ret = ZodiacHouse.Name.Sco; break;
				case ZodiacHouse.Name.Tau: ret = ZodiacHouse.Name.Lib; break;
				case ZodiacHouse.Name.Gem: ret = ZodiacHouse.Name.Vir; break;
				case ZodiacHouse.Name.Can: ret = ZodiacHouse.Name.Can; break;
				case ZodiacHouse.Name.Leo: ret = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Vir: ret = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Lib: ret = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Sco: ret = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Sag: ret = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Cap: ret = ZodiacHouse.Name.Aqu; break;
				case ZodiacHouse.Name.Aqu: ret = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Pis: ret = ZodiacHouse.Name.Sag; break;
				default: Debug.Assert(false, "ZodiacHouse::KalachakraMirrorSign");  break;
			}
			return new ZodiacHouse(ret);
		}
		public ZodiacHouse AdarsaSign ()
		{
			ZodiacHouse.Name ret = ZodiacHouse.Name.Ari;
			switch (this.value)
			{
				case ZodiacHouse.Name.Ari: ret = ZodiacHouse.Name.Sco; break;
				case ZodiacHouse.Name.Tau: ret = ZodiacHouse.Name.Lib; break;
				case ZodiacHouse.Name.Gem: ret = ZodiacHouse.Name.Vir; break;
				case ZodiacHouse.Name.Can: ret = ZodiacHouse.Name.Aqu; break;
				case ZodiacHouse.Name.Leo: ret = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Vir: ret = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Lib: ret = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Sco: ret = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Sag: ret = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Cap: ret = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Aqu: ret = ZodiacHouse.Name.Can; break;
				case ZodiacHouse.Name.Pis: ret = ZodiacHouse.Name.Sag; break;
				default: Debug.Assert(false, "ZodiacHouse::AdarsaSign");  break;
			}
			return new ZodiacHouse(ret);
		}
		public ZodiacHouse AbhimukhaSign ()
		{
			ZodiacHouse.Name ret = ZodiacHouse.Name.Ari;
			switch (this.value)
			{
				case ZodiacHouse.Name.Ari: ret = ZodiacHouse.Name.Sco; break;
				case ZodiacHouse.Name.Tau: ret = ZodiacHouse.Name.Lib; break;
				case ZodiacHouse.Name.Gem: ret = ZodiacHouse.Name.Sag; break;
				case ZodiacHouse.Name.Can: ret = ZodiacHouse.Name.Aqu; break;
				case ZodiacHouse.Name.Leo: ret = ZodiacHouse.Name.Cap; break;
				case ZodiacHouse.Name.Vir: ret = ZodiacHouse.Name.Pis; break;
				case ZodiacHouse.Name.Lib: ret = ZodiacHouse.Name.Tau; break;
				case ZodiacHouse.Name.Sco: ret = ZodiacHouse.Name.Ari; break;
				case ZodiacHouse.Name.Sag: ret = ZodiacHouse.Name.Gem; break;
				case ZodiacHouse.Name.Cap: ret = ZodiacHouse.Name.Leo; break;
				case ZodiacHouse.Name.Aqu: ret = ZodiacHouse.Name.Can; break;
				case ZodiacHouse.Name.Pis: ret = ZodiacHouse.Name.Vir; break;
				default: Debug.Assert(false, "ZodiacHouse::AbhimukhaSign");  break;
			}
			return new ZodiacHouse(ret);
		}

		public static string ToShortString (ZodiacHouse.Name z)		
		{
			switch (z)
			{
				case ZodiacHouse.Name.Ari: return "Ar";
				case ZodiacHouse.Name.Tau: return "Ta";
				case ZodiacHouse.Name.Gem: return "Ge";
				case ZodiacHouse.Name.Can: return "Cn";
				case ZodiacHouse.Name.Leo: return "Le";
				case ZodiacHouse.Name.Vir: return "Vi";
				case ZodiacHouse.Name.Lib: return "Li";
				case ZodiacHouse.Name.Sco: return "Sc";
				case ZodiacHouse.Name.Sag: return "Sg";
				case ZodiacHouse.Name.Cap: return "Cp";
				case ZodiacHouse.Name.Aqu: return "Aq";
				case ZodiacHouse.Name.Pis: return "Pi";
				default: return "";
			}
		}
	}

	internal class MomentConverter: ExpandableObjectConverter 
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
			Trace.Assert (value is string, "MomentConverter::ConvertFrom 1");
			string s = (string) value;

			int day=1, month=1, year=1, hour=1, min=1, sec=1;

			string[] _arr = s.Split (new Char[2] {' ', ':'});
			ArrayList arr = new ArrayList(_arr);

			if ((String)arr[arr.Count-1] == "") 
				arr[arr.Count -1] = "0";

			if (arr.Count >= 3) 
			{
				while (arr.Count < 6) 
				{
					arr.Add ("0");
				}

				day = int.Parse((String)arr[0]);
				month = Moment.FromStringMonth((String)arr[1]);
				year = int.Parse((String)arr[2]);
				hour = int.Parse ((String)arr[3]);
				min = int.Parse ((String)arr[4]);
				sec = int.Parse ((String)arr[5]);
			}

			//if (day < 1 || day > 31) day = 1;
			if (hour < 0 || hour > 23) hour = 12;
			if (min < 0 || min > 120) min = 30;
			if (sec < 0 || sec > 120) sec = 30;
			Moment m = new Moment (year, month, day, hour, min, sec);
			return m;
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Console.WriteLine ("Foo: destType is {0}", destType);
			// Trace.Assert (destType == typeof(string) && value is Moment, "MomentConverter::ConvertTo 1");
			Moment m = (Moment)value;
			return m.ToString();
		}   
	}

	internal class OrderedGrahasConverter: ExpandableObjectConverter 
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
			Trace.Assert (value is string, "OrderedGrahasConverter::ConvertFrom 1");
			string s = (string) value;

			OrderedGrahas oz = new OrderedGrahas();
			ArrayList al = new ArrayList();
			string[] arr = s.Split (new Char[4] {'.', ' ', ':', ','});
			foreach (string szh_mixed in arr)
			{
				string szh = szh_mixed.ToLower();
				switch (szh)
				{
					case "as": al.Add (Body.Name.Lagna); break;
					case "su": al.Add (Body.Name.Sun); break;
					case "mo": al.Add (Body.Name.Moon); break;
					case "ma": al.Add (Body.Name.Mars); break;
					case "me": al.Add (Body.Name.Mercury); break;
					case "ju": al.Add (Body.Name.Jupiter); break;
					case "ve": al.Add (Body.Name.Venus); break;
					case "sa": al.Add (Body.Name.Saturn); break;
					case "ra": al.Add (Body.Name.Rahu); break;
					case "ke": al.Add (Body.Name.Ketu); break;
				}
			}
			oz.grahas = (ArrayList)al.Clone();
			return oz;
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Trace.Assert (destType == typeof(string) && value is OrderedGrahas, "OrderedGrahas::ConvertTo 1");
			OrderedGrahas oz = (OrderedGrahas)value;
			return oz.ToString();
		}   
	}



	[TypeConverter(typeof(OrderedGrahasConverter))]
	public class OrderedGrahas : ICloneable
	{
		public ArrayList grahas;
		public OrderedGrahas ()
		{
			this.grahas = new ArrayList();
		}
		override public string ToString()
		{
			string s="";
			foreach (Body.Name bn in this.grahas)
				s += Body.toShortString(bn) + " ";
			return s;
		}
		public object Clone()
		{
			OrderedGrahas oz = new OrderedGrahas();
			oz.grahas = (ArrayList)this.grahas.Clone();
			return oz;
		}
	}
	internal class OrderedZodiacHousesConverter: ExpandableObjectConverter 
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
			Trace.Assert (value is string, "OrderedZodiacHousesConverter::ConvertFrom 1");
			string s = (string) value;

			OrderedZodiacHouses oz = new OrderedZodiacHouses();
			ArrayList al = new ArrayList();
			string[] arr = s.Split (new Char[4] {'.', ' ', ':', ','});
			foreach (string szh_mixed in arr)
			{
				string szh = szh_mixed.ToLower();
				switch (szh)
				{
					case "ari": al.Add (ZodiacHouse.Name.Ari); break;
					case "tau": al.Add (ZodiacHouse.Name.Tau); break;
					case "gem": al.Add (ZodiacHouse.Name.Gem); break;
					case "can": al.Add (ZodiacHouse.Name.Can); break;
					case "leo": al.Add (ZodiacHouse.Name.Leo); break;
					case "vir": al.Add (ZodiacHouse.Name.Vir); break;
					case "lib": al.Add (ZodiacHouse.Name.Lib); break;
					case "sco": al.Add (ZodiacHouse.Name.Sco); break;
					case "sag": al.Add (ZodiacHouse.Name.Sag); break;
					case "cap": al.Add (ZodiacHouse.Name.Cap); break;
					case "aqu": al.Add (ZodiacHouse.Name.Aqu); break;
					case "pis": al.Add (ZodiacHouse.Name.Pis); break;
				}
			}
			oz.houses = (ArrayList)al.Clone();
			return oz;
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Trace.Assert (destType == typeof(string) && value is OrderedZodiacHouses, "HMSInfo::ConvertTo 1");
			OrderedZodiacHouses oz = (OrderedZodiacHouses)value;
			return oz.ToString();
		}   
	}

	[TypeConverter(typeof(OrderedZodiacHousesConverter))]
	public class OrderedZodiacHouses : ICloneable
	{
		public ArrayList houses;
		public OrderedZodiacHouses ()
		{
			this.houses = new ArrayList();
		}
		override public string ToString()
		{
			string s="";
			ZodiacHouse.Name[] names = (ZodiacHouse.Name[])houses.ToArray(typeof(ZodiacHouse.Name));
			foreach (ZodiacHouse.Name zn in names)
				s+= zn.ToString() + " ";
			return s;
		}
		public object Clone()
		{
			OrderedZodiacHouses oz = new OrderedZodiacHouses();
			oz.houses = (ArrayList)this.houses.Clone();
			return oz;
		}
	}
	

	internal class MhoraArrayConverter : ArrayConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return base.CanConvertFrom (context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return base.ConvertFrom (context, culture, value);
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			if (destType == typeof(string))
				return "Click Here To Modify";
			else
				return base.ConvertTo(context, culture, value, destType);
		}   

	}

	internal class HMSInfoConverter: ExpandableObjectConverter 
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
			Trace.Assert (value is string, "HMSInfoConverter::ConvertFrom 1");
			string s = (string) value;

			int hour=1, min=1, sec=1;

			HMSInfo.dir_type dir = HMSInfo.dir_type.NS;
			string[] _arr = s.Split (new Char[3] {'.',' ',':'});
			ArrayList arr = new ArrayList (_arr);

			if (arr.Count >= 2) 
			{
				if ((String)arr[arr.Count-1] == "") 
					arr[arr.Count-1] = "0";

				while (arr.Count < 4) 
					arr.Add ("0");

				hour = int.Parse ((String)arr[0]);
				String sdir = (String)arr[1];
				if (sdir == "W" || sdir == "w" || sdir == "S" || sdir == "s")
					hour *= -1;
				if (sdir == "W" || sdir == "w" || sdir == "E" || sdir == "e")
					dir = HMSInfo.dir_type.EW;
				else
					dir = HMSInfo.dir_type.NS;

				min = int.Parse ((String)arr[2]);
				sec = int.Parse ((String)arr[3]);
			} 

			if (hour < -180 || hour > 180) hour = 29;
			if (min < 0 || min > 60) min = 20;
			if (sec < 0 || sec > 60) sec = 30;
			HMSInfo hi = new HMSInfo(hour, min, sec, dir);
			return hi;
		}
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value,    
			Type destType) 
		{
			Trace.Assert (destType == typeof(string) && value is HMSInfo, "HMSInfo::ConvertTo 1");
			HMSInfo hi = (HMSInfo)value;
			return hi.ToString();
		}   
	}



	[Serializable]
	[TypeConverter(typeof(HMSInfoConverter))]
	public class HMSInfo: MhoraSerializableOptions, ICloneable, ISerializable
	{
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected HMSInfo (SerializationInfo info, StreamingContext context):
		this ()
		{
			this.Constructor(this.GetType(), info, context);
		}
		public enum dir_type:int
		{
			NS=1, EW=2
		}
		public dir_type direction;
		private int m_hour, m_minute, m_second;

		public  dir_type dir
		{
			get { return direction; }
			set { direction = value; }
		}

		public int degree 
		{
			get { return m_hour; }
			set { m_hour = value; }
		}

		public int minute
		{
			get { return m_minute; }
			set {m_minute = value; }
		}

		public int second
		{
			get { return m_second; }
			set { m_second = value; }
		}
		public double toDouble () 
		{
			if (m_hour >= 0)
				return (((double)m_hour) + (((double)m_minute) / 60.0) + (((double)m_second) / 3600.0));
			else
				return (((double)m_hour) - (((double)m_minute) / 60.0) - (((double)m_second) / 3600.0));
		}
		public HMSInfo ()
		{
			m_hour = m_minute = m_second = 0;
			direction = dir_type.NS;
		}
		public HMSInfo (int hour, int min, int sec, dir_type dt)
		{
			m_hour = hour; 
			m_minute = min;
			m_second = sec;
			direction = dt;
		}
		public HMSInfo (double hms)
		{
			double hour = Math.Floor (hms);
			hms = (hms - hour) * 60.0;
			double min = Math.Floor (hms);
			hms = (hms - min) * 60.0;
			double sec = Math.Floor (hms);
			m_hour = (int)hour;
			m_minute = (int)min;
			m_second = (int)sec;
			direction = dir_type.NS;
		}
		override public string ToString ()
		{
			string dirs;
			if (direction == dir_type.EW && m_hour < 0) dirs = "W";
			else if (direction == dir_type.EW) dirs = "E";
			else if (direction == dir_type.NS && m_hour < 0) dirs = "S";
			else dirs = "N";
														
														  
			int m_htemp = m_hour >=0 ? m_hour : m_hour * -1;
			return (m_htemp < 10 ? "0":"") + m_htemp.ToString()
				+ " " + dirs 
				+ " " + (m_minute < 10 ? "0" : "") + m_minute.ToString()
				+ ":" + (m_second < 10 ? "0" : "") + m_second .ToString();
		}
		public object Clone () 
		{
			return new HMSInfo(m_hour, m_minute, m_second, direction);
		}
	}
	
	/// <summary>
	/// Specified a Moment. This can be used for charts, dasa times etc
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(MomentConverter))]
	public class Moment: MhoraSerializableOptions, ICloneable, ISerializable
	{
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected Moment (SerializationInfo info, StreamingContext context):
			this ()
		{
			this.Constructor(this.GetType(), info, context);
		}
		private int m_day, m_month, m_year, m_hour, m_minute, m_second;
		public static void doubleToHMS (double h, ref int hour, ref int minute, ref int second) 
		{
			hour = (int)Math.Floor(h);
			h = (h - (double)hour) * 60.0;
			minute = (int)Math.Floor(h);
			h = (h - (double)minute) * 60.0;
			second = (int)Math.Floor(h);
		}
		public Object Clone() 
		{
			return new Moment (m_year, m_month, m_day, m_hour, m_minute, m_second);
		}
		public double toUniversalTime ()
		{
			return sweph.swe_julday(m_year, m_month, m_day, time);
		}
		public double toUniversalTime (Horoscope h)
		{
			double local_ut = sweph.swe_julday(year, month, day, time);
			return local_ut - (h.info.tz.toDouble())/24.0;
		}
		public double time 
		{
			get 
			{
				return (double)m_hour + (double)m_minute/60.0 + (double)m_second/3600.0;
			}
		}
		public int day { get { return m_day; } set { m_day = value; } }
		public int month { get { return m_month; } set { m_month = value; } }
		public int year { get { return m_year; } set { m_year = value; } }
		public int hour { get { return m_hour; } set { m_hour = value; } }
		public int minute { get { return m_minute; } set { m_minute = value; } }
		public int second { get { return m_second; } set { m_second = value; } }
		public Moment ()
		{
			DateTime t = DateTime.Now;
			this.day = t.Day;
			this.month = t.Month;
			this.year = t.Year;
			this.hour = t.Hour;
			this.minute = t.Minute;
			this.second = t.Second;
		}
		public Moment (int year, int month, int day, double time)
		{
			m_day = day;
			m_month = month;
			m_year = year;
			Moment.doubleToHMS(time, ref m_hour, ref m_minute, ref m_second);
		}
		public Moment (int year, int month, int day, int hour, int minute, int second) 
		{
			m_day = day;
			m_month = month;
			m_year = year;
			m_hour = hour;
			m_minute = minute;
			m_second = second;
		}
		public Moment (double tjd_ut, Horoscope h)
		{
			double time = 0;
			tjd_ut += h.info.tz.toDouble() / 24.0;
			sweph.swe_revjul(tjd_ut, ref m_year, ref m_month, ref m_day, ref time);
			Moment.doubleToHMS(time, ref m_hour, ref m_minute, ref m_second);
		}
		public static int FromStringMonth (string s)
		{
			switch (s)
			{
				case "Jan": return 1;
				case "Feb": return 2;
				case "Mar": return 3;
				case "Apr": return 4;
				case "May": return 5;
				case "Jun": return 6;
				case "Jul": return 7;
				case "Aug": return 8;
				case "Sep": return 9;
				case "Oct": return 10;
				case "Nov": return 11;
				case "Dec": return 12;	 
			}
			
			return 1;
		}
		public string ToStringMonth (int i)
		{
			switch (i)
			{
				case 1: return "Jan";
				case 2: return "Feb";
				case 3: return "Mar";
				case 4: return "Apr";
				case 5: return "May";
				case 6: return "Jun";
				case 7: return "Jul";
				case 8: return "Aug";
				case 9: return "Sep";
				case 10: return "Oct";
				case 11: return "Nov";
				case 12: return "Dec";
			}
			Trace.Assert (false, "Moment::ToStringMonth");
			return "";
		}
		override public string ToString()
		{
			return (m_day < 10? "0":"") + m_day.ToString() + 
				" " + ToStringMonth(m_month) + " " + m_year.ToString() 
				+ " " + (m_hour < 10 ? "0":"") + m_hour.ToString() 
				+ ":" + (m_minute < 10 ? "0" : "") + m_minute.ToString()
				+ ":" + (m_second < 10 ? "0" : "") + m_second .ToString();
		}
		public string ToShortDateString ()
		{
			int year = m_year % 100;
			return string.Format ("{0:00}-{1:00}-{2:00}", m_day, m_month, year);
		}
		public string ToDateString ()
		{
			return string.Format ("{0:00} {1} {2}", m_day, ToStringMonth(m_month), m_year);
		}

		public string ToTimeString ()
		{
			return this.ToTimeString(false);
		}
		public string ToTimeString (bool bDisplaySeconds)
		{
			if (bDisplaySeconds)
				return string.Format ("{0:00}:{1:00}:{2:00}", m_hour, m_minute, m_second);
			else
				return string.Format ("{0:00}:{1:00}", m_hour, m_minute);
		}
	}

	public class UIStringTypeEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			string stringInit = "";
			if (value is string)
				stringInit = (string)value;
			LongStringEditor le = new LongStringEditor(stringInit);
			le.TitleText = "Event Description";
			edSvc.ShowDialog(le);
			return le.EditorText;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
	} 

	[Serializable]
	public class UserEvent: MhoraSerializableOptions, ICloneable, ISerializable
	{
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected UserEvent (SerializationInfo info, StreamingContext context): this ()
		{
			this.Constructor(this.GetType(), info, context);
		}

		private string mEventName;
		private string mEventDesc;
		private Moment mEventTime;
		private bool mWorkWithEvent;

		public string EventName
		{
			get { return mEventName; }
			set { mEventName = value; }
		}

		[Editor(typeof(UIStringTypeEditor),typeof(UITypeEditor))]
		public string EventDesc
		{
			get { return mEventDesc; }
			set { mEventDesc = value; }
		}

		public Moment EventTime
		{
			get { return mEventTime; }
			set { mEventTime = value; }
		}

		public bool WorkWithEvent
		{
			get { return mWorkWithEvent; }
			set { mWorkWithEvent = value; }
		}

		public override string ToString()
		{
			string ret = "";

			if (this.WorkWithEvent)
				ret += "* ";
			ret += this.EventName + ": " + this.EventTime.ToString();
			return ret;
		}

		public UserEvent ()
		{
			this.EventName = "Some Event";
			this.EventTime = new Moment();
			this.WorkWithEvent = true;
		}

		public object Clone ()
		{
			UserEvent ue = new UserEvent();
			ue.EventName = this.EventName;
			ue.EventTime = this.EventTime;
			ue.WorkWithEvent = this.WorkWithEvent;
			ue.EventDesc = this.EventDesc;
			return ue;
		}
	}

	/// <summary>
	/// A class containing all required input from the user for a given chart
	/// (e.g.) all the information contained in a .jhd file
	/// </summary>
	///
	[Serializable]
	public class HoraInfo: MhoraSerializableOptions, ICloneable, ISerializable
	{
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected HoraInfo (SerializationInfo info, StreamingContext context):
		this ()
		{
			this.Constructor(this.GetType(), info, context);
		}

		public object Clone () 
		{
			HoraInfo hi = new HoraInfo ((Moment)tob.Clone(), (HMSInfo)lat.Clone(), (HMSInfo)lon.Clone(), (HMSInfo)tz.Clone());
			hi.events = this.events;
			hi.name = this.name;
			hi.defaultYearCompression = this.defaultYearCompression;
			hi.defaultYearLength = this.defaultYearLength;
			hi.defaultYearType = this.defaultYearType;
			return hi;
		}
		public enum Name :int
		{
			Birth, Progression, TithiPravesh, Transit, Dasa
		}
		public enum EFileType 
		{
			JagannathaHora, MudgalaHora
		}
		public Name type;
		public Moment tob;
		//public double lon, lat, alt, tz;
		public double alt;
		public HMSInfo lon, lat, tz;
		public string name;
		public EFileType FileType;
		public double defaultYearLength = 0;
		public double defaultYearCompression = 0;
		public ToDate.DateType defaultYearType = ToDate.DateType.FixedYear;
		
		private UserEvent[] events = null;

		private const string CAT_TOB = "1: Birth Info";
		private const string CAT_EVT = "2: Events";

		[Category(CAT_TOB)]
		[PropertyOrder(1),PGDisplayName("Time of Birth")]
		[Description("Date of Birth. Format is 'dd Mmm yyyy hh:mm:ss'\n Example 23 Mar 1979 23:11:00")]
		public Moment DateOfBirth
		{
			get { return tob; }
			set { tob = value; }
		}

		[Category(CAT_TOB), PropertyOrder(2)]
		[Description("Latitude. Format is 'hh D mm:ss mm:ss'\n Example 23 N 24:00")]
		public HMSInfo Latitude
		{
			get { return lat; }
			set { lat = value; }
		}

		[Category(CAT_TOB), PropertyOrder(3)]
		[Description("Longitude. Format is 'hh D mm:ss mm:ss'\n Example 23 E 24:00")]
		public HMSInfo Longitude
		{
			get { return lon; }
			set { lon = value; }
		}

		[Category(CAT_TOB), PropertyOrder(4)]
		[PGDisplayName("Time zone")]
		[Description("Time Zone. Format is 'hh D mm:ss mm:ss'\n Example 3 E 00:00")]
		public HMSInfo TimeZone
		{
			get { return tz; }
			set { tz = value; }
		}

		[Category(CAT_TOB), PropertyOrder(5)]
		public double Altitude
		{
			get { return alt; }
			set { alt = value; }
		}

		[Category(CAT_EVT), PropertyOrder(1)]
		[Description("Events")]
		public UserEvent[] Events
		{
			get { return this.events; }
			set { this.events = value; }
		}

		public HoraInfo (Moment atob, HMSInfo alat, HMSInfo alon, HMSInfo atz) 
		{
			tob = atob;
			lon = alon;
			lat = alat;
			tz = atz;
			alt = 0.0;
			this.type = Name.Birth;
			this.FileType = EFileType.MudgalaHora;
			this.events = new UserEvent[0];
		}
		public HoraInfo ()
		{
			System.DateTime t = DateTime.Now;
			tob = new Moment(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
			lon = (HMSInfo) MhoraGlobalOptions.Instance.Longitude.Clone();
			lat = (HMSInfo) MhoraGlobalOptions.Instance.Latitude.Clone();
			tz = (HMSInfo) MhoraGlobalOptions.Instance.TimeZone.Clone();
			alt = 0.0;
			this.type = Name.Birth;
			this.FileType = EFileType.MudgalaHora;
			this.events = new UserEvent[0];
		}
	}	

	[Serializable]
	public class HoroscopeOptions: MhoraSerializableOptions, ICloneable, ISerializable
	{
		public enum AyanamsaType :int
		{
			Fagan=0, Lahiri=1, Raman=3, Ushashashi=4, Krishnamurti=5
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum SunrisePositionType :int
		{
			[Description("Sun's center rises")] 
				TrueDiscCenter,
			[Description("Sun's edge rises")]
				TrueDiscEdge, 
			[Description("Sun's center appears to rise")]
				ApparentDiscCenter, 
			[Description("Sun's edge apprears to rise")]
				ApparentDiscEdge,
			[Description("6am Local Mean Time")]
				Lmt
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum EMaandiType
		{
			[Description("Rises at the beginning of Saturn's portion")]			SaturnBegin, 
			[Description("Rises in the middle of Saturn's portion")]			SaturnMid, 
			[Description("Rises at the end of Saturn's portion")]				SaturnEnd, 
			[Description("Rises at the beginning of the lordless portion")]		LordlessBegin, 
			[Description("Rises in the middle of the lordless portion")]		LordlessMid, 
			[Description("Rises at the end of the lordless portion")]			LordlessEnd
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum EUpagrahaType
		{
			[Description("Rises at the beginning of the grahas portion")]		Begin, 
			[Description("Rises in the middle of the grahas portion")]			Mid, 
			[Description("Rises at the end of the grahas portion")]				End
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum EHoraType
		{
			[Description("Day split into equal parts")]							Sunriset, 
			[Description("Daytime and Nighttime split into equal parts")]		SunrisetEqual, 
			[Description("Day (Local Mean Time) split into equal parts")]		Lmt		
		}

		public enum EGrahaPositionType: int
		{
			Apparent, True
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum ENodeType: int
		{
			[Description("Mean Positions of nodes")]			Mean, 
			[Description("True Positions of nodes")]			True
		}

		[TypeConverter(typeof(EnumDescConverter))]
		public enum EBhavaType: int
		{
			[Description("Lagna is at the beginning of the bhava")]			Start, 
			[Description("Lagna is in the middle of the bhava")]			Middle
		}
		public HoroscopeOptions () 
		{
			sunrisePosition = SunrisePositionType.TrueDiscEdge;
			mHoraType = EHoraType.Lmt;
			mKalaType = EHoraType.Sunriset;
			mBhavaType = EBhavaType.Start;
			grahaPositionType = EGrahaPositionType.True;
			nodeType = ENodeType.Mean;
			Ayanamsa = HoroscopeOptions.AyanamsaType.Lahiri;
			AyanamsaOffset = new HMSInfo(0,0,0, HMSInfo.dir_type.EW);
			this.mUserLongitude = new Longitude(0);
			this.MaandiType = EMaandiType.SaturnBegin;
			this.GulikaType = EMaandiType.SaturnMid;
			this.UpagrahaType = EUpagrahaType.Mid;
			mEphemPath = @getExeDir() + "\\eph";
		}
		public object Clone ()
		{
			HoroscopeOptions o = new HoroscopeOptions();
			o.sunrisePosition = this.sunrisePosition;
			o.grahaPositionType = this.grahaPositionType;
			o.nodeType = this.nodeType;
			o.Ayanamsa = this.Ayanamsa;
			o.AyanamsaOffset = this.AyanamsaOffset;
			o.HoraType = this.HoraType;
			o.KalaType = this.KalaType;
			o.BhavaType = this.BhavaType;
			o.mUserLongitude = this.mUserLongitude.add(0);
			o.MaandiType = this.MaandiType;
			o.GulikaType = this.GulikaType;
			o.UpagrahaType = this.UpagrahaType;
			o.EphemerisPath = this.EphemerisPath;
			return o;
		}
		public void Copy (HoroscopeOptions o)
		{
			this.sunrisePosition = o.sunrisePosition;
			this.grahaPositionType = o.grahaPositionType;
			this.nodeType = o.nodeType;
			this.Ayanamsa = o.Ayanamsa;
			this.AyanamsaOffset = o.AyanamsaOffset;
			this.HoraType = o.HoraType;
			this.KalaType = o.KalaType;
			this.BhavaType = o.BhavaType;
			this.mUserLongitude = o.mUserLongitude.add(0);
			this.MaandiType = o.MaandiType;
			this.GulikaType = o.GulikaType;
			this.UpagrahaType = o.UpagrahaType;
			this.EphemerisPath = o.EphemerisPath;
		}
		private SunrisePositionType mSunrisePosition;
		private EHoraType mHoraType;
		private EHoraType mKalaType;
		public EGrahaPositionType grahaPositionType;
		public ENodeType nodeType;
		private AyanamsaType mAyanamsa;
		private HMSInfo mAyanamsaOffset;
		private EBhavaType mBhavaType;
		private Longitude mUserLongitude;
		private EMaandiType mMaandiType;
		private EMaandiType mGulikaType;
		private EUpagrahaType mUpagrahaType;
		private string mEphemPath;

		protected const string CAT_GENERAL = "1: General Settings";
		protected const string CAT_GRAHA = "2: Graha Settings";
		protected const string CAT_SUNRISE = "3: Sunrise Settings";
		protected const string CAT_UPAGRAHA = "4: Upagraha Settings";


		[Category(CAT_GENERAL)]
		[PropertyOrder(1), PGDisplayName("Full Ephemeris Path")]
		public string EphemerisPath
		{
			get { return mEphemPath; }
			set { this.mEphemPath = value; }
		}
		[PropertyOrder(2), Category(CAT_GENERAL)]
		public AyanamsaType Ayanamsa
		{
			get { return mAyanamsa; }
			set { mAyanamsa = value; }
		}
		[PropertyOrder(4), Category(CAT_GENERAL)]
		[PGDisplayName("Custom Longitude")]
		public Longitude CustomBodyLongitude
		{
			get { return mUserLongitude; }
			set { this.mUserLongitude = value; }
		}
		[Category(CAT_GENERAL)]
		[PropertyOrder(3), PGDisplayName("Ayanamsa Offset")]
		public HMSInfo AyanamsaOffset
		{
			get { return mAyanamsaOffset; }
			set { mAyanamsaOffset = value; }
		}

		[Category(CAT_UPAGRAHA)]
		[PropertyOrder(1), PGDisplayName("Upagraha")]
		public EUpagrahaType UpagrahaType
		{
			get { return mUpagrahaType; }
			set { mUpagrahaType = this.mUpagrahaType; }
		}
		[Category(CAT_UPAGRAHA)]
		[PropertyOrder(2), PGDisplayName("Maandi")]
		public EMaandiType MaandiType
		{
			get { return mMaandiType; }
			set { mMaandiType = value; }
		}
		[Category(CAT_UPAGRAHA)]
		[PropertyOrder(3), PGDisplayName("Gulika")]
		public EMaandiType GulikaType
		{
			get { return mGulikaType; }
			set { mGulikaType = value; }
		}

		[Category(CAT_SUNRISE)]
		[PropertyOrder(1), PGDisplayName("Sunrise")]
		public SunrisePositionType sunrisePosition
		{
			get { return mSunrisePosition; }
			set { mSunrisePosition = value; }
		}
		[Category(CAT_SUNRISE)]
		[PropertyOrder(2), PGDisplayName("Hora")]
		public EHoraType HoraType
		{
			get { return mHoraType; }
			set { mHoraType = value; }
		}
		[Category(CAT_SUNRISE)]
		[PropertyOrder(3), PGDisplayName("Kala")]
		public EHoraType KalaType
		{
			get { return mKalaType; }
			set { mKalaType = value; }
		}
		//public EGrahaPositionType GrahaPositionType
		//{
		//	get { return grahaPositionType; }
		//	set { grahaPositionType = value; }
		//}
		[Category(CAT_GRAHA)]
		[PropertyOrder(1), PGDisplayName("Rahu / Ketu")]
		public ENodeType NodeType
		{
			get { return nodeType; }
			set { nodeType = value; }
		}

		[Category(CAT_GRAHA)]
		[PropertyOrder(2), PGDisplayName("Bhava")]
		public EBhavaType BhavaType
		{
			get { return mBhavaType; }
			set { mBhavaType = value; }
		}
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected HoroscopeOptions (SerializationInfo info, StreamingContext context):
		this ()
		{
			this.Constructor(this.GetType(), info, context);
		}
	}

	/// <summary>
	/// Contains all the information for a horoscope. i.e. All ephemeris lookups
	/// have been completed, sunrise/sunset has been calculated etc.
	/// </summary>
	public class Horoscope : ICloneable
	{
		public event EvtChanged Changed;

		public Body.Name LordOfZodiacHouse (ZodiacHouse zh, Division dtype)
		{
			return LordOfZodiacHouse (zh.value, dtype);
		}
		public Body.Name LordOfZodiacHouse (ZodiacHouse.Name zh, Division dtype) 
		{
			FindStronger fs_colord = new FindStronger(this, dtype, FindStronger.RulesStrongerCoLord(this));

			switch (zh) 
			{
				case ZodiacHouse.Name.Aqu:
					return fs_colord.StrongerGraha(Body.Name.Rahu, Body.Name.Saturn, true);
				case ZodiacHouse.Name.Sco:
					return fs_colord.StrongerGraha(Body.Name.Ketu, Body.Name.Mars, true);
				default:
					return Basics.SimpleLordOfZodiacHouse(zh);
			}
		}
		public object Clone ()
		{
			Horoscope h = new Horoscope ((HoraInfo)this.info.Clone(), (HoroscopeOptions)this.options.Clone());
			if (this.strength_options != null)
				h.strength_options = (StrengthOptions)this.strength_options.Clone();
			return h;
		}

		public void OnGlobalCalcPrefsChanged (object o)
		{
			HoroscopeOptions ho = (HoroscopeOptions)o;
			this.options.Copy(ho);
			this.strength_options = null;
			this.OnChanged();
		}

		public void OnlySignalChanged()
		{
			if (Changed != null)
				Changed (this);
		}
		public void OnChanged()
		{
			populateCache();
			if (Changed != null)
				Changed (this);
		}
		public HoroscopeOptions options;

		public DivisionPosition CalculateDivisionPosition (BodyPosition bp, Division d)
		{
			return bp.toDivisionPosition(d);
		}

		public ArrayList CalculateDivisionPositions (Division d)
		{
			ArrayList al = new ArrayList();
			foreach (BodyPosition bp in this.positionList) 
			{
				al.Add (CalculateDivisionPosition (bp, d));
			}
			return al; 
		}
		private DivisionPosition CalculateGrahaArudhaDivisionPosition (Body.Name bn, ZodiacHouse zh, Division dtype)
		{
			DivisionPosition dp = getPosition (bn).toDivisionPosition(dtype);
			DivisionPosition dpl = getPosition (Body.Name.Lagna).toDivisionPosition(dtype);
			int rel = dp.zodiac_house.numHousesBetween(zh);
			int hse = dpl.zodiac_house.numHousesBetween(zh);
			ZodiacHouse zhsum = zh.add(rel);
			int rel2 = dp.zodiac_house.numHousesBetween(zhsum);
			if (rel2 == 1 || rel2 == 7)
				zhsum = zhsum.add(10);
			DivisionPosition dp2 = new DivisionPosition(Body.Name.Other, BodyType.Name.GrahaArudha, zhsum, 0, 0, 0);
			dp2.otherString = String.Format("{0}{1}", Body.toShortString(bn), hse);
			return dp2;
		}
		public ArrayList CalculateGrahaArudhaDivisionPositions (Division dtype)
		{
			
			object[][] parameters = new object[][]
			{
				new object[] { ZodiacHouse.Name.Ari, Body.Name.Mars },
				new object[] { ZodiacHouse.Name.Tau, Body.Name.Venus },
				new object[] { ZodiacHouse.Name.Gem, Body.Name.Mercury },
				new object[] { ZodiacHouse.Name.Can, Body.Name.Moon },
				new object[] { ZodiacHouse.Name.Leo, Body.Name.Sun },
				new object[] { ZodiacHouse.Name.Vir, Body.Name.Mercury },
				new object[] { ZodiacHouse.Name.Lib, Body.Name.Venus },
				new object[] { ZodiacHouse.Name.Sco, Body.Name.Mars },
				new object[] { ZodiacHouse.Name.Sag, Body.Name.Jupiter },
				new object[] { ZodiacHouse.Name.Cap, Body.Name.Saturn },
				new object[] { ZodiacHouse.Name.Aqu, Body.Name.Saturn },
				new object[] { ZodiacHouse.Name.Pis, Body.Name.Jupiter },
				new object[] { ZodiacHouse.Name.Sco, Body.Name.Ketu },
				new object[] { ZodiacHouse.Name.Aqu, Body.Name.Rahu }
			};
			ArrayList al = new ArrayList(14);
			
			for (int i=0; i< parameters.Length; i++)
				al.Add (this.CalculateGrahaArudhaDivisionPosition(
					(Body.Name)parameters[i][1],
					new ZodiacHouse((ZodiacHouse.Name)parameters[i][0]),
					dtype ));
			return al;
		}
		private string[] varnada_strs = new string[]
			{
				"VL", "V2", "V3", "V4", "V5", "V6", "V7", "V8", "V9", "V10", "V11", "V12"
			};
		public ArrayList CalculateVarnadaDivisionPositions (Division dtype)
		{
			ArrayList al = new ArrayList(12);
			ZodiacHouse _zh_l = this.getPosition(Body.Name.Lagna).toDivisionPosition(dtype).zodiac_house;
			ZodiacHouse _zh_hl = this.getPosition(Body.Name.HoraLagna).toDivisionPosition(dtype).zodiac_house;
			
			ZodiacHouse zh_ari = new ZodiacHouse(ZodiacHouse.Name.Ari);
			ZodiacHouse zh_pis = new ZodiacHouse(ZodiacHouse.Name.Pis);
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh_l = _zh_l.add(i);
				ZodiacHouse zh_hl = _zh_hl.add(i);

				int i_l=0, i_hl=0;
				if (zh_l.isOdd()) i_l = zh_ari.numHousesBetween(zh_l);
				else i_l = zh_pis.numHousesBetweenReverse(zh_l);

				if (zh_hl.isOdd()) i_hl = zh_ari.numHousesBetween(zh_hl);
				else i_hl = zh_pis.numHousesBetweenReverse(zh_hl);
			
				int sum=0;
				if (zh_l.isOdd() == zh_hl.isOdd()) sum = i_l + i_hl;
				else sum = Math.Max(i_l, i_hl) - Math.Min(i_l, i_hl);

				ZodiacHouse zh_v = null;
				if (zh_l.isOdd()) zh_v = zh_ari.add(sum);
				else zh_v = zh_pis.addReverse(sum);

				DivisionPosition div_pos = new DivisionPosition(Body.Name.Other, BodyType.Name.Varnada, zh_v, 0, 0, 0);
				div_pos.otherString = varnada_strs[i-1];
				al.Add(div_pos);
			}
			return al;
		}
		private DivisionPosition CalculateArudhaDivisionPosition (ZodiacHouse zh, Body.Name bn, 
			Body.Name aname, Division d, BodyType.Name btype)
		{
			BodyPosition bp = getPosition(bn);
			ZodiacHouse zhb = CalculateDivisionPosition (bp, d).zodiac_house;
			int rel = zh.numHousesBetween(zhb);
			ZodiacHouse zhsum = zhb.add (rel);
			int rel2 = zh.numHousesBetween(zhsum);
			if (rel2 == 1 || rel2 == 7)
				zhsum = zhsum.add(10);
			return new DivisionPosition (aname, btype, zhsum, 0, 0, 0);
		}
		public ArrayList CalculateArudhaDivisionPositions (Division d)
		{
			Body.Name[] bnlist = new Body.Name[]
				{
					Body.Name.Other,
					Body.Name.AL, Body.Name.A2, Body.Name.A3, Body.Name.A4,
					Body.Name.A5, Body.Name.A6, Body.Name.A7, Body.Name.A8,
					Body.Name.A9, Body.Name.A10, Body.Name.A11, Body.Name.UL
				};
								
			FindStronger fs_colord = new FindStronger(this, d, FindStronger.RulesStrongerCoLord(this));
			ArrayList arudha_div_list = new ArrayList (14);		
			DivisionPosition first, second;
			for (int j=1; j<=12; j++)
			{
				ZodiacHouse zlagna = this.CalculateDivisionPosition(this.getPosition(Body.Name.Lagna), d).zodiac_house;
				ZodiacHouse zh = zlagna.add (j);
				Body.Name bn_stronger, bn_weaker=Body.Name.Other; 
				bn_stronger = Basics.SimpleLordOfZodiacHouse(zh.value);
				if (zh.value  == ZodiacHouse.Name.Aqu) 
				{
					bn_stronger = fs_colord.StrongerGraha(Body.Name.Rahu, Body.Name.Saturn, true);
					bn_weaker = fs_colord.WeakerGraha(Body.Name.Rahu, Body.Name.Saturn, true);
				}
				else if (zh.value == ZodiacHouse.Name.Sco) 
				{
					bn_stronger = fs_colord.StrongerGraha(Body.Name.Ketu, Body.Name.Mars, true);
					bn_weaker = fs_colord.WeakerGraha(Body.Name.Ketu, Body.Name.Mars, true);
				}
				first = CalculateArudhaDivisionPosition(zh, bn_stronger, bnlist[j], d, BodyType.Name.BhavaArudha);
				arudha_div_list.Add (first);
				if (zh.value == ZodiacHouse.Name.Aqu || zh.value == ZodiacHouse.Name.Sco) 
				{
					second = CalculateArudhaDivisionPosition(zh, bn_weaker, bnlist[j], d, BodyType.Name.BhavaArudhaSecondary);
					if (first.zodiac_house.value != second.zodiac_house.value)
						arudha_div_list.Add (second);
				}
			}
			return arudha_div_list;
		}

		public object UpdateHoraInfo (Object o)
		{
			HoraInfo i = (HoraInfo)o;
			info.DateOfBirth = i.DateOfBirth;
			info.Altitude = i.Altitude;
			info.Latitude = i.Latitude;
			info.Longitude = i.Longitude;
			info.tz = i.tz;
			info.Events = (UserEvent[])i.Events.Clone();
			OnChanged();
			return info.Clone();
		}
		public HoraInfo info;
		public double sunrise;
		public double sunset;
		public double lmt_sunrise;
		public double lmt_sunset;
		public double next_sunrise;
		public double next_sunset;
		public double ayanamsa;
		public double lmt_offset;
		public double baseUT;
		public Basics.Weekday wday;
		public Basics.Weekday lmt_wday;
		public ArrayList positionList;
		public Longitude[] swephHouseCusps;
		public int swephHouseSystem;
		public StrengthOptions strength_options;
		private void populateLmt ()
		{
			this.lmt_offset = getLmtOffset (info, baseUT);
			this.lmt_sunrise = 6.0 + lmt_offset * 24.0;
			this.lmt_sunset = 18.0 + lmt_offset * 24.0;
		}
		public double getLmtOffsetDays (HoraInfo info, double _baseUT)
		{
			double ut_lmt_noon = this.getLmtOffset(info, _baseUT);
			double ut_noon = this.baseUT - info.tob.time/24.0 + 12.0/24.0;
			return ut_lmt_noon - ut_noon;
		}
		public double getLmtOffset (HoraInfo _info, double _baseUT)
		{
			double[] geopos = new Double[3]{_info.lon.toDouble(), _info.lat.toDouble(), _info.alt};
			double[] tret = new Double[6] {0,0,0,0,0,0};
			double midnight_ut = _baseUT - _info.tob.time/24.0;
			sweph.swe_lmt (midnight_ut , sweph.SE_SUN, sweph.SE_CALC_MTRANSIT, geopos, 0.0, 0.0, tret);
			double lmt_noon_1 = tret[0];
			double lmt_offset_1 = lmt_noon_1 - (midnight_ut + 12.0/24.0);
			sweph.swe_lmt (midnight_ut , sweph.SE_SUN, sweph.SE_CALC_MTRANSIT, geopos, 0.0, 0.0, tret);
			double lmt_noon_2 = tret[0];
			double lmt_offset_2 = lmt_noon_2 - (midnight_ut + 12.0/24.0);

			double ret_lmt_offset = (lmt_offset_1 + lmt_offset_2)/2.0;
			//Console.WriteLine("LMT: {0}, {1}", lmt_offset_1, lmt_offset_2);

			return ret_lmt_offset;
#if DND
			// determine offset from ephemeris time
			lmt_offset = 0;
			double tjd_et = baseUT + sweph.swe_deltat(baseUT);
			System.Text.StringBuilder s = new System.Text.StringBuilder(256);
			int ret = sweph.swe_time_equ(tjd_et, ref lmt_offset, s);
#endif
		}
		private void populateSunrisetCache ()
		{
			double sunrise_ut=0.0;
			this.populateSunrisetCacheHelper(this.baseUT, ref this.next_sunrise, ref this.next_sunset, ref sunrise_ut);
			this.populateSunrisetCacheHelper(sunrise_ut - 1.0 - (1.0/24.0), ref this.sunrise, ref this.sunset, ref sunrise_ut);			
			//Debug.WriteLine("Sunrise[t]: " + this.sunrise.ToString() + " " + this.sunrise.ToString(), "Basics");
		}
		public void populateSunrisetCacheHelper (double ut, ref double sr, ref double ss, ref double sr_ut)
		{
			int srflag = 0;
			switch (options.sunrisePosition) 
			{
				case HoroscopeOptions.SunrisePositionType.Lmt:
					sr = 6.0 + lmt_offset*24.0;
					ss = 18.0 + lmt_offset*24.0;
					break;
				case HoroscopeOptions.SunrisePositionType.TrueDiscEdge:
					srflag = sweph.SE_BIT_NO_REFRACTION; goto default;
				case HoroscopeOptions.SunrisePositionType.TrueDiscCenter:
					srflag = sweph.SE_BIT_NO_REFRACTION | sweph.SE_BIT_DISC_CENTER; goto default;
				case HoroscopeOptions.SunrisePositionType.ApparentDiscCenter:
					srflag = sweph.SE_BIT_DISC_CENTER; goto default;
				case HoroscopeOptions.SunrisePositionType.ApparentDiscEdge:
				default:
					//int sflag = 0;
					//if (options.sunrisePosition == HoroscopeOptions.SunrisePositionType.DiscCenter)
					//	sflag += 256;
					int year=0, month=0, day=0;
					double hour=0.0;

					double[] geopos = new Double[3]{this.info.lon.toDouble(), this.info.lat.toDouble(), this.info.alt};
					double[] tret = new Double[6] {0,0,0,0,0,0};

					sweph.swe_rise (ut, sweph.SE_SUN, srflag, geopos, 0.0, 0.0, tret);
					sr_ut = tret[0];
					sweph.swe_revjul(tret[0], ref year, ref month, ref day, ref hour);
					sr = hour + this.info.tz.toDouble();
					sweph.swe_set (tret[0], sweph.SE_SUN, srflag, geopos, 0.0, 0.0, tret);
					sweph.swe_revjul(tret[0], ref year, ref month, ref day, ref hour);
					ss = hour + this.info.tz.toDouble();
					sr = Basics.normalize_exc(0.0, 24.0, sr);
					ss = Basics.normalize_exc(0.0, 24.0, ss);
					break;
			}
		}


		public double[] getHoraCuspsUt ()
		{
			double[] cusps = null;
			switch (this.options.HoraType)
			{
				case HoroscopeOptions.EHoraType.Sunriset:
					cusps = this.getSunrisetCuspsUt(12);
					break;
				case HoroscopeOptions.EHoraType.SunrisetEqual:
					cusps = this.getSunrisetEqualCuspsUt(12);
					break;
				case HoroscopeOptions.EHoraType.Lmt:
					sweph.obtainLock(this);
					cusps = this.getLmtCuspsUt(12);
					sweph.releaseLock(this);
					break;
			}		
			return cusps;
		}

		public double[] getKalaCuspsUt ()
		{
			double[] cusps = null;
			switch (this.options.KalaType)
			{
				case HoroscopeOptions.EHoraType.Sunriset:
					cusps = this.getSunrisetCuspsUt(8);
					break;
				case HoroscopeOptions.EHoraType.SunrisetEqual:
					cusps = this.getSunrisetEqualCuspsUt(8);
					break;
				case HoroscopeOptions.EHoraType.Lmt:
					sweph.obtainLock(this);
					cusps = this.getLmtCuspsUt(8);
					sweph.releaseLock(this);
					break;
			}		
			return cusps;
		}

		public double[] getSunrisetCuspsUt (int dayParts)
		{
			double[] ret = new double[dayParts*2+1];

			double sr_ut = this.baseUT - this.hoursAfterSunrise()/24.0;
			double ss_ut = sr_ut - this.sunrise/24.0 + this.sunset/24.0;
			double sr_next_ut = sr_ut - this.sunrise/24.0 + this.next_sunrise/24.0 + 1.0;

			double day_span = (ss_ut - sr_ut) / dayParts;
			double night_span = (sr_next_ut - ss_ut) / dayParts;

			for (int i=0; i<dayParts; i++)
				ret[i] = sr_ut + day_span * i;
			for (int i=0; i<=dayParts; i++)
				ret[i+dayParts] = ss_ut + night_span * i;
			return ret;
		}
		public double[] getSunrisetEqualCuspsUt (int dayParts)
		{
			double[] ret = new double[dayParts*2+1];

			double sr_ut = this.baseUT - this.hoursAfterSunrise()/24.0;
			double sr_next_ut = sr_ut - this.sunrise/24.0 + this.next_sunrise/24.0 + 1.0;
			double span = (sr_next_ut - sr_ut) / (dayParts*2);

			for (int i=0; i<=(dayParts*2); i++)
				ret[i] = sr_ut + span * i;
			return ret;
		}

		public double[] getLmtCuspsUt (int dayParts)
		{
			double[] ret = new double[dayParts*2+1];
			double sr_lmt_ut = this.baseUT - this.hoursAfterSunrise()/24.0 - this.sunrise/24.0 + 6.0/24.0;
			double sr_lmt_next_ut = sr_lmt_ut + 1.0;
			//double sr_lmt_ut = this.baseUT - this.info.tob.time / 24.0 + 6.0 / 24.0;
			//double sr_lmt_next_ut = sr_lmt_ut + 1.0;

			double lmt_offset = this.getLmtOffset(this.info, this.baseUT);
			sr_lmt_ut += lmt_offset;
			sr_lmt_next_ut += lmt_offset;

			if (sr_lmt_ut > this.baseUT)
			{
				sr_lmt_ut--;
				sr_lmt_next_ut--;
			}


			double span = (sr_lmt_next_ut - sr_lmt_ut) / (dayParts*2);

			for (int i=0; i<=(dayParts*2); i++)
				ret[i] = sr_lmt_ut + span * i;
			return ret;

		}

		public Body.Name[] kalaOrder = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Mars, Body.Name.Jupiter, Body.Name.Mercury, 
				Body.Name.Venus, Body.Name.Saturn, Body.Name.Moon, Body.Name.Rahu
			};
	
		public Body.Name calculateKala ()
		{
			int iBase = 0;
			return calculateKala (ref iBase);
		}
		public Body.Name calculateKala (ref int iBase)
		{
			int[] offsets_day = new int[] { 0, 6, 1, 3, 2, 4, 5 };
			Body.Name b = Basics.weekdayRuler(this.wday);
			bool bday_birth = this.isDayBirth();

			double[] cusps =  this.getKalaCuspsUt();
			if (this.options.KalaType == HoroscopeOptions.EHoraType.Lmt)
			{
				b = Basics.weekdayRuler(this.lmt_wday);
				bday_birth = 
					this.info.tob.time > this.lmt_sunset || 
					this.info.tob.time < this.lmt_sunrise;
			}
			int i = offsets_day[(int)b];
			iBase = i;
			int j = 0;

			if (bday_birth)
			{
				for (j=0; j<8; j++)
				{
					if (this.baseUT >= cusps[j] && this.baseUT < cusps[j+1])
						break;
				}
				i+=j;
				while (i >= 8) i-=8;
				return kalaOrder[i];
			}
			else 
			{
				//i+=4;
				for (j=8; j<16; j++)
				{
					if (this.baseUT >= cusps[j] && this.baseUT < cusps[j+1])
						break;
				}
				i += j;
				while (i >= 8) i-=8;
				return kalaOrder[i];
			}

		}


		public Body.Name[] horaOrder = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Venus, Body.Name.Mercury, Body.Name.Moon,
				Body.Name.Saturn, Body.Name.Jupiter, Body.Name.Mars
			};
		public Body.Name calculateHora ()
		{
			int iBody = 0;
			return this.calculateHora(this.baseUT, ref iBody);
		}
		public Body.Name calculateHora (double _baseUT, ref int baseBody)
		{
			int[] offsets = new int[] { 0, 3, 6, 2, 5, 1, 4 };
			Body.Name b = Basics.weekdayRuler(this.wday);
			double[] cusps = this.getHoraCuspsUt();
			if (this.options.HoraType == HoroscopeOptions.EHoraType.Lmt)
				b = Basics.weekdayRuler(this.lmt_wday);

			int i = offsets[(int)b];
			baseBody = i;
			int j = 0;
			//for (j=0; j<23; j++)
			//{
			//	Moment m1 = new Moment(cusps[j], this);
			//	Moment m2 = new Moment(cusps[j+1], this);
			//	Console.WriteLine ("Seeing if dob is between {0} and {1}", m1, m2);
			//}
			for (j=0; j<23; j++)
			{
				if (_baseUT >= cusps[j] && _baseUT < cusps[j+1])
					break;
			}
			//Console.WriteLine ("Found hora in the {0}th hora", j);
			i+= j;
			while (i >=7) i-= 7;
			return horaOrder[i];
		}
		private Body.Name calculateUpagrahasStart ()
		{
			if (this.isDayBirth())
				return Basics.weekdayRuler(this.wday);

			switch (this.wday)
			{
				default:
				case Basics.Weekday.Sunday: return Body.Name.Jupiter;
				case Basics.Weekday.Monday: return Body.Name.Venus;
				case Basics.Weekday.Tuesday: return Body.Name.Saturn;
				case Basics.Weekday.Wednesday: return Body.Name.Sun;
				case Basics.Weekday.Thursday: return Body.Name.Moon;
				case Basics.Weekday.Friday: return Body.Name.Mars;
				case Basics.Weekday.Saturday: return Body.Name.Mercury;
			}
		}

		private void calculateUpagrahasSingle(Body.Name b, double tjd)
		{
			Longitude lon = new Longitude(0);
			lon.value = sweph.swe_lagna(tjd);
			BodyPosition bp = new BodyPosition(this, b, BodyType.Name.Upagraha,
				lon, 0, 0, 0, 0, 0);
			positionList.Add(bp);
		}

		private void calculateMaandiHelper (Body.Name b, HoroscopeOptions.EMaandiType mty, double[] jds, double dOffset, int[] bodyOffsets)
		{
			switch (mty)
			{
				case HoroscopeOptions.EMaandiType.SaturnBegin:
					this.calculateUpagrahasSingle(b, jds[bodyOffsets[(int)Body.Name.Saturn]]);
					break;
				case HoroscopeOptions.EMaandiType.SaturnMid:
					this.calculateUpagrahasSingle(b, jds[bodyOffsets[(int)Body.Name.Saturn]]+dOffset);
					break;
				case HoroscopeOptions.EMaandiType.SaturnEnd:
				case HoroscopeOptions.EMaandiType.LordlessBegin:
					int _off1 = bodyOffsets[(int)Body.Name.Saturn]+1;
					this.calculateUpagrahasSingle(b, jds[bodyOffsets[(int)Body.Name.Saturn]]+(dOffset*2.0));
					break;
				case HoroscopeOptions.EMaandiType.LordlessMid:
					this.calculateUpagrahasSingle(b, jds[bodyOffsets[(int)Body.Name.Saturn]]+(dOffset*3.0));
					break;
				case HoroscopeOptions.EMaandiType.LordlessEnd:
					this.calculateUpagrahasSingle(b, jds[bodyOffsets[(int)Body.Name.Saturn]]+(dOffset*4.0));
					break;
			}
		}
		private void calculateUpagrahas()
		{

			double dStart=0, dEnd=0;

			Moment m = this.info.tob;
			dStart = dEnd = sweph.swe_julday(m.year, m.month, m.day, -this.info.tz.toDouble());
			Body.Name bStart = this.calculateUpagrahasStart();

			if (this.isDayBirth())
			{
				dStart += this.sunrise / 24.0;
				dEnd += this.sunset / 24.0;
			} 
			else 
			{
				dStart += this.sunset / 24.0;
				dEnd += 1.0 + this.sunrise / 24.0;
			}
			double dPeriod = (dEnd - dStart)/8.0;
			double dOffset = dPeriod / 2.0;

			double[] jds = new double[8];
			for (int i=0; i<8; i++)
				jds[i] = dStart + ((double)i * dPeriod) + dOffset;

			int[] bodyOffsets = new int[8];
			for (int i=0; i<8; i++)
			{
				int _ib = (int)bStart + i;
				while (_ib >= 8) _ib -= 8;
				bodyOffsets[_ib] = i;
			}

			double dUpagrahaOffset = 0;
			switch (options.UpagrahaType)
			{
				case HoroscopeOptions.EUpagrahaType.Begin:
					dUpagrahaOffset = 0; break;
				case HoroscopeOptions.EUpagrahaType.Mid:
					dUpagrahaOffset = dOffset; break;
				case HoroscopeOptions.EUpagrahaType.End:
					dUpagrahaOffset = dPeriod; break;
			}

			sweph.obtainLock(this);
			this.calculateUpagrahasSingle(Body.Name.Kala, jds[bodyOffsets[(int)Body.Name.Sun]]);
			this.calculateUpagrahasSingle(Body.Name.Mrityu, jds[bodyOffsets[(int)Body.Name.Mars]]);
			this.calculateUpagrahasSingle(Body.Name.ArthaPraharaka, jds[bodyOffsets[(int)Body.Name.Mercury]]);
			this.calculateUpagrahasSingle(Body.Name.YamaGhantaka, jds[bodyOffsets[(int)Body.Name.Jupiter]]);


			this.calculateMaandiHelper(Body.Name.Maandi, options.MaandiType, jds, dOffset, bodyOffsets);
			this.calculateMaandiHelper(Body.Name.Gulika, options.GulikaType, jds, dOffset, bodyOffsets);
			sweph.releaseLock(this);
		}
		private void calculateSunsUpagrahas ()
		{
			Longitude slon = this.getPosition(Body.Name.Sun).longitude;

			BodyPosition bpDhuma = new BodyPosition(this, Body.Name.Dhuma, BodyType.Name.Upagraha,
				slon.add(133.0+20.0/60.0), 0, 0, 0, 0, 0);

			BodyPosition bpVyatipata = new BodyPosition(this, Body.Name.Vyatipata, BodyType.Name.Upagraha,
				new Longitude(360.0).sub(bpDhuma.longitude), 0, 0, 0, 0, 0);

			BodyPosition bpParivesha = new BodyPosition(this, Body.Name.Parivesha, BodyType.Name.Upagraha,
				bpVyatipata.longitude.add(180), 0, 0, 0, 0, 0);

			BodyPosition bpIndrachapa = new BodyPosition(this, Body.Name.Indrachapa, BodyType.Name.Upagraha,
				new Longitude(360.0).sub(bpParivesha.longitude), 0, 0, 0, 0, 0);

			BodyPosition bpUpaketu = new BodyPosition(this, Body.Name.Upaketu, BodyType.Name.Upagraha,
				slon.sub(30), 0, 0, 0, 0, 0);

			positionList.Add(bpDhuma);
			positionList.Add(bpVyatipata);
			positionList.Add(bpParivesha);
			positionList.Add(bpIndrachapa);
			positionList.Add(bpUpaketu);
		}
		private void calculateWeekday ()
		{
			Moment m = this.info.tob;
			double jd = sweph.swe_julday(m.year, m.month, m.day, 12.0);
			if (info.tob.time < sunrise) jd -=1;
			this.wday = (Basics.Weekday)sweph.swe_day_of_week(jd);

			jd = sweph.swe_julday(m.year, m.month, m.day, 12.0);
			if (info.tob.time < lmt_sunrise) jd -=1;
			this.lmt_wday = (Basics.Weekday)sweph.swe_day_of_week(jd);
		}
		
		private void addChandraLagna (string desc, Longitude lon)
		{
			BodyPosition bp = new BodyPosition(
				this, Body.Name.Other, BodyType.Name.ChandraLagna, lon, 0, 0, 0, 0, 0);
			bp.otherString = desc;
			this.positionList.Add (bp);
		}
		private void calculateChandraLagnas ()
		{
			BodyPosition bp_moon = this.getPosition(Body.Name.Moon);
			Longitude lon_base = 
				new Longitude(bp_moon.extrapolateLongitude(
				new Division(Basics.DivisionType.Navamsa)).toZodiacHouseBase());
			lon_base = lon_base.add(bp_moon.longitude.toZodiacHouseOffset());

			//Console.WriteLine ("Starting Chandra Ayur Lagna from {0}", lon_base);

			double ista_ghati = Basics.normalize_exc (0.0, 24.0, info.tob.time - sunrise) * 2.5;
			Longitude gl_lon = lon_base.add (new Longitude(ista_ghati * 30.0));
			Longitude hl_lon = lon_base.add (new Longitude(ista_ghati * 30.0 / 2.5));
			Longitude bl_lon = lon_base.add (new Longitude(ista_ghati * 30.0 / 5.0));

			double vl = ista_ghati * 5.0;
			while (ista_ghati > 12.0) ista_ghati -= 12.0;
			Longitude vl_lon = lon_base.add (new Longitude(vl * 30.0));

			this.addChandraLagna("Chandra Lagna - GL", gl_lon);
			this.addChandraLagna("Chandra Lagna - HL", hl_lon);
			this.addChandraLagna("Chandra Ayur Lagna - BL", bl_lon);
			this.addChandraLagna("Chandra Lagna - ViL", vl_lon);
		}

		private void calculateSL ()
		{
			Longitude mpos = this.getPosition(Body.Name.Moon).longitude;
			Longitude lpos = this.getPosition(Body.Name.Lagna).longitude;
			double sldeg = mpos.toNakshatraOffset() / ((360.0)/27.0) * 360.0;
			Longitude slLon = lpos.add(sldeg);
			BodyPosition bp = new BodyPosition(this, Body.Name.SreeLagna, BodyType.Name.SpecialLagna, 
				slLon, 0, 0, 0, 0, 0);
			this.positionList.Add(bp);
		}
		private void calculatePranapada ()
		{
			Longitude spos = this.getPosition(Body.Name.Sun).longitude;
			double offset = this.info.tob.time - this.sunrise;
			if (offset < 0) offset += 24.0;
			offset *= (60.0*60.0/6.0);
			Longitude ppos = null;
			switch ((int)spos.toZodiacHouse().value % 3)
			{
				case 1: ppos = spos.add(offset); break;
				case 2: ppos = spos.add(offset+8.0*30.0); break;
				default:
				case 0: ppos = spos.add(offset+4.0*30.0); break;
			}
			BodyPosition bp = new BodyPosition(this, Body.Name.Pranapada, BodyType.Name.SpecialLagna,
				ppos, 0, 0, 0, 0, 0);
			this.positionList.Add(bp);
		}
		private void addOtherPoints()
		{
			Longitude lag_pos = this.getPosition(Body.Name.Lagna).longitude;
			Longitude sun_pos = this.getPosition(Body.Name.Sun).longitude;
			Longitude moon_pos = this.getPosition(Body.Name.Moon).longitude;
			Longitude mars_pos = this.getPosition(Body.Name.Mars).longitude;
			Longitude jup_pos = this.getPosition(Body.Name.Jupiter).longitude;
			Longitude ven_pos = this.getPosition(Body.Name.Venus).longitude;
			Longitude sat_pos = this.getPosition(Body.Name.Saturn).longitude;
			Longitude rah_pos = this.getPosition(Body.Name.Rahu).longitude;
			Longitude mandi_pos = this.getPosition(Body.Name.Maandi).longitude;
			Longitude gulika_pos = this.getPosition(Body.Name.Gulika).longitude;
			Longitude muhurta_pos = new Longitude(
				this.hoursAfterSunrise() / (this.next_sunrise+24.0-this.sunrise) * 360.0);

			// add simple midpoints
			this.addOtherPosition("User Specified", this.options.CustomBodyLongitude);
			this.addOtherPosition("Brighu Bindu", rah_pos.add(moon_pos.sub(rah_pos).value/2.0));
			this.addOtherPosition("Muhurta Point", muhurta_pos);
			this.addOtherPosition("Ra-Ke m.p", rah_pos.add(90));
			this.addOtherPosition("Ke-Ra m.p", rah_pos.add(270));

			Longitude l1pos = this.getPosition(this.LordOfZodiacHouse(
				lag_pos.toZodiacHouse(), new Division(Basics.DivisionType.Rasi))).longitude;
			Longitude l6pos = this.getPosition(this.LordOfZodiacHouse(
				lag_pos.toZodiacHouse().add(6), new Division(Basics.DivisionType.Rasi))).longitude;
			Longitude l8pos = this.getPosition(this.LordOfZodiacHouse(
				lag_pos.toZodiacHouse().add(6), new Division(Basics.DivisionType.Rasi))).longitude;
			Longitude l12pos = this.getPosition(this.LordOfZodiacHouse(
				lag_pos.toZodiacHouse().add(6), new Division(Basics.DivisionType.Rasi))).longitude;

			Longitude mrit_sat_pos = new Longitude(mandi_pos.value*8.0+sat_pos.value*8.0);
			Longitude mrit_jup2_pos = new Longitude (
				sat_pos.value*9.0+mandi_pos.value*18.0+jup_pos.value*18.0);
			Longitude mrit_sun2_pos = new Longitude (
				sat_pos.value*9.0+mandi_pos.value*18.0+sun_pos.value*18.0 );
			Longitude mrit_moon2_pos = new Longitude (
				sat_pos.value*9.0+mandi_pos.value*18.0+moon_pos.value*18.0 );

			if (this.isDayBirth())
				this.addOtherPosition("Niryana: Su-Sa sum", sun_pos.add(sat_pos), Body.Name.MrityuPoint);
			else
				this.addOtherPosition("Niryana: Mo-Ra sum", moon_pos.add(rah_pos), Body.Name.MrityuPoint);

			this.addOtherPosition("Mrityu Sun: La-Mn sum", lag_pos.add(mandi_pos), Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Moon: Mo-Mn sum", moon_pos.add(mandi_pos), Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Lagna: La-Mo-Mn sum", lag_pos.add(moon_pos).add(mandi_pos), Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Sat: Mn8-Sa8", mrit_sat_pos, Body.Name.MrityuPoint);
			this.addOtherPosition("6-8-12 sum", l6pos.add(l8pos).add(l12pos), Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Jup: Sa9-Mn18-Ju18", mrit_jup2_pos, Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Sun: Sa9-Mn18-Su18", mrit_sun2_pos, Body.Name.MrityuPoint);
			this.addOtherPosition("Mrityu Moon: Sa9-Mn18-Mo18", mrit_moon2_pos, Body.Name.MrityuPoint);

			this.addOtherPosition("Su-Mo sum", sun_pos.add(moon_pos));
			this.addOtherPosition("Ju-Mo-Ma sum", jup_pos.add(moon_pos).add(mars_pos));
			this.addOtherPosition("Su-Ve-Ju sum", sun_pos.add(ven_pos).add(jup_pos));
			this.addOtherPosition("Sa-Mo-Ma sum", sat_pos.add(moon_pos).add(mars_pos));
			this.addOtherPosition("La-Gu-Sa sum", lag_pos.add(gulika_pos).add(sat_pos));
			this.addOtherPosition("L-MLBase sum", l1pos.add(moon_pos.toZodiacHouseBase()));
		}
		public void populateHouseCusps ()
		{
			this.swephHouseCusps = new Longitude[13];
			double[] dCusps = new double[13];
			double[] ascmc = new double[10];

			sweph.obtainLock(this);
			sweph.swe_houses_ex(this.baseUT, sweph.SEFLG_SIDEREAL,
				info.lat.toDouble(), info.lon.toDouble(), this.swephHouseSystem,
				dCusps, ascmc);
			sweph.releaseLock(this);
			for (int i=0; i<12; i++)
				this.swephHouseCusps[i] = new Longitude(dCusps[i+1]);

			if (this.options.BhavaType == HoroscopeOptions.EBhavaType.Middle)
			{
				Longitude middle = new Longitude((dCusps[1] + dCusps[2])/2.0);
				double offset = middle.sub(swephHouseCusps[0]).value;
				for (int i=0; i<12; i++)
					swephHouseCusps[i] = swephHouseCusps[i].sub(offset);
			}


			this.swephHouseCusps[12] = this.swephHouseCusps[0];
		}
		private void populateCache () 
		{
			// The stuff here is largely order sensitive
			// Try to add new definitions to the end

			baseUT = sweph.swe_julday (info.tob.year, info.tob.month, info.tob.day,
				info.tob.time - info.tz.toDouble());


			sweph.obtainLock(this);
			sweph.swe_set_ephe_path (MhoraGlobalOptions.Instance.HOptions.EphemerisPath);
			// Find LMT offset
			this.populateLmt();
			// Sunrise (depends on lmt)
			populateSunrisetCache();
			// Basic grahas + Special lagnas (depend on sunrise)
			positionList = Basics.CalculateBodyPositions(this, this.sunrise);
			sweph.releaseLock(this);
			// Srilagna etc
			this.calculateSL();
			this.calculatePranapada();
			// Sun based Upagrahas (depends on sun)
			this.calculateSunsUpagrahas();
			// Upagrahas (depends on sunrise)
			this.calculateUpagrahas();
			// Weekday (depends on sunrise)
			this.calculateWeekday();
			// Sahamas
			this.calculateSahamas();
			// Prana sphuta etc. (depends on upagrahas)
			this.getPrashnaMargaPositions();
			this.calculateChandraLagnas();
			this.addOtherPoints();
			// Add extrapolated special lagnas (depends on sunrise)
			this.addSpecialLagnaPositions();
			// Hora (depends on weekday)
			this.calculateHora();
			// Populate house cusps on options refresh
			this.populateHouseCusps();
		}
		public Horoscope (HoraInfo _info, HoroscopeOptions _options) 
		{
			options = _options;
			info = _info;
			this.swephHouseSystem = 'P';
			this.populateCache();
			MhoraGlobalOptions.CalculationPrefsChanged += new EvtChanged(this.OnGlobalCalcPrefsChanged);
		}
		public double lengthOfDay ()
		{
			return (this.next_sunrise + 24.0 - this.sunrise);
			
		}

		public double hoursAfterSunrise ()
		{
			double ret = this.info.tob.time - this.sunrise;
			if (ret < 0) ret += 24.0;
			return ret;
		}
		public double hoursAfterSunRiseSet ()
		{
			double ret = 0;
			if (this.isDayBirth())
				ret = this.info.tob.time - this.sunrise;
			else
				ret = this.info.tob.time - this.sunset;
			if (ret < 0) ret += 24.0;
			return ret;
		}
		public bool isDayBirth ()
		{
			if (info.tob.time >= this.sunrise && 
				info.tob.time < this.sunset) return true;
			return false;
		}

		public void addOtherPosition (string desc, Longitude lon, Body.Name name)
		{
			BodyPosition bp = new BodyPosition(this, name, BodyType.Name.Other, lon, 0, 0, 0, 0, 0);
			bp.otherString = desc;
			this.positionList.Add(bp);
		}
		public void addOtherPosition (string desc, Longitude lon)
		{
			addOtherPosition (desc, lon, Body.Name.Other);
		}

		public void addSpecialLagnaPositions ()
		{
			double diff = this.info.tob.time - this.sunrise;
			if (diff < 0) diff += 24.0;
			sweph.obtainLock(this);
			for (int i=1; i<=12; i++)
			{
				double specialDiff = diff * (double)(i-1);
				double tjd = this.baseUT + specialDiff/24.0;
				double asc = sweph.swe_lagna (tjd);
				string desc = String.Format("Special Lagna ({0:00})", i);
				this.addOtherPosition(desc, new Longitude(asc));
			}
			sweph.releaseLock(this);
		}

		public void getPrashnaMargaPositions ()
		{
			Longitude sunLon = this.getPosition(Body.Name.Sun).longitude;
			Longitude moonLon = this.getPosition(Body.Name.Moon).longitude;
			Longitude lagnaLon = this.getPosition(Body.Name.Lagna).longitude;
			Longitude gulikaLon = this.getPosition(Body.Name.Gulika).longitude;
			Longitude rahuLon = this.getPosition(Body.Name.Rahu).longitude;

			Longitude trisLon = lagnaLon.add(moonLon).add(gulikaLon);
			Longitude chatusLon = trisLon.add(sunLon);
			Longitude panchasLon = chatusLon.add(rahuLon);
			Longitude pranaLon = new Longitude(lagnaLon.value*5.0).add(gulikaLon);
			Longitude dehaLon = new Longitude(moonLon.value*8.0).add(gulikaLon);
			Longitude mrityuLon = new Longitude(gulikaLon.value*7.0).add(sunLon);

			this.addOtherPosition("Trih Sphuta", trisLon);
			this.addOtherPosition("Chatuh Sphuta", chatusLon);
			this.addOtherPosition("Panchah Sphuta", panchasLon);
			this.addOtherPosition("Pranah Sphuta", pranaLon);
			this.addOtherPosition("Deha Sphuta", dehaLon);
			this.addOtherPosition("Mrityu Sphuta", mrityuLon);

		}

		public BodyPosition getPosition (Body.Name b)
		{
			int index = Body.toInt(b);
			System.Type t = positionList[index].GetType();
			String s = t.ToString();
			Trace.Assert (index >= 0 && index < positionList.Count, "Horoscope::getPosition 1");
			Trace.Assert (positionList[index].GetType () == typeof(mhora.BodyPosition), "Horoscope::getPosition 2");
			BodyPosition bp = (BodyPosition)positionList[Body.toInt (b)];
			if (bp.name == b)
				return bp;

			for (int i=(int)Body.Name.Lagna+1; i<positionList.Count; i++)
				if (b == ((BodyPosition)positionList[i]).name)
					return (BodyPosition)positionList[i];

			Trace.Assert(false, "Basics::GetPosition. Unable to find body");
			return (BodyPosition)positionList[0];
		
		}
		private BodyPosition sahamaHelper (string sahama, Body.Name b, Body.Name a, Body.Name c)
		{
			Longitude lonA, lonB, lonC;
			lonA = this.getPosition(a).longitude;
			lonB = this.getPosition(b).longitude;
			lonC = this.getPosition(c).longitude;
			return this.sahamaHelper(sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaHelper (string sahama, Body.Name b, Body.Name a, Longitude lonC)
		{
			Longitude lonA, lonB;
			lonA = this.getPosition(a).longitude;
			lonB = this.getPosition(b).longitude;
			return this.sahamaHelper(sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaHelper (string sahama, Longitude lonB, Body.Name a, Body.Name c)
		{
			Longitude lonA, lonC;
			lonA = this.getPosition(a).longitude;
			lonC = this.getPosition(c).longitude;
			return this.sahamaHelper(sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaHelper (string sahama, Longitude lonB, Longitude lonA, Longitude lonC)
		{
			// b-a+c
			bool bDay = this.isDayBirth();

			Longitude lonR;
			lonR = lonB.sub(lonA).add(lonC);
			if (lonB.sub(lonA).value <= lonC.sub(lonA).value)
				lonR = lonR.add(new Longitude(30.0));

			BodyPosition bp = new BodyPosition(this, Body.Name.Other, BodyType.Name.Sahama, lonR, 
				0.0, 0.0, 0.0, 0.0, 0.0);
			bp.otherString = sahama;
			return bp;
		}

		private BodyPosition sahamaDNHelper (string sahama, Longitude lonB, Longitude lonA, Longitude lonC)
		{
			// b-a+c
			bool bDay = this.isDayBirth();
			Longitude lonR;
			if (bDay) 
				lonR = lonB.sub(lonA).add(lonC);
			else 
				lonR = lonA.sub(lonB).add(lonC);
			
			if (lonB.sub(lonA).value <= lonC.sub(lonA).value)
				lonR = lonR.add(new Longitude(30.0));

			BodyPosition bp = new BodyPosition(this, Body.Name.Other, BodyType.Name.Sahama, lonR, 
				0.0, 0.0, 0.0, 0.0, 0.0);
			bp.otherString = sahama;
			return bp;
		}
		private BodyPosition sahamaDNHelper (string sahama, Body.Name b, Longitude lonA, Body.Name c)
		{
			Longitude lonB, lonC;
			lonB = this.getPosition(b).longitude;
			lonC = this.getPosition(c).longitude;
			return sahamaDNHelper (sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaDNHelper (string sahama, Longitude lonB, Body.Name a, Body.Name c)
		{
			Longitude lonA, lonC;
			lonA = this.getPosition(a).longitude;
			lonC = this.getPosition(c).longitude;
			return sahamaDNHelper (sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaDNHelper (string sahama, Longitude lonB, Longitude lonA, Body.Name c)
		{
			Longitude lonC;
			lonC = this.getPosition(c).longitude;
			return sahamaDNHelper (sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaDNHelper (string sahama, Body.Name b, Body.Name a, Body.Name c)
		{
			Longitude lonA, lonB, lonC;
			lonA = this.getPosition(a).longitude;
			lonB = this.getPosition(b).longitude;
			lonC = this.getPosition(c).longitude;
			return sahamaDNHelper (sahama, lonB, lonA, lonC);
		}
		private BodyPosition sahamaHelperNormalize (BodyPosition b, Body.Name lower, Body.Name higher)
		{
			Longitude lonA = this.getPosition(lower).longitude;
			Longitude lonB = this.getPosition(higher).longitude;
			if (b.longitude.sub(lonA).value < lonB.sub(lonA).value) return b;
			b.longitude = b.longitude.add(new Longitude(30));
			return b;
		}
		public ArrayList calculateSahamas ()
		{
			bool bDay = this.isDayBirth();
			ArrayList al = new ArrayList();
			Longitude lon_lagna = this.getPosition(Body.Name.Lagna).longitude;
			Longitude lon_base = new Longitude(lon_lagna.toZodiacHouseBase());
			ZodiacHouse zh_lagna = lon_lagna.toZodiacHouse();
			ZodiacHouse zh_moon = this.getPosition(Body.Name.Moon).longitude.toZodiacHouse();
			ZodiacHouse zh_sun = this.getPosition(Body.Name.Sun).longitude.toZodiacHouse();


			// Fixed positions. Relied on by other sahams
			al.Add(sahamaDNHelper("Punya", Body.Name.Moon, Body.Name.Sun, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Vidya", Body.Name.Sun, Body.Name.Moon, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Sastra", Body.Name.Jupiter, Body.Name.Saturn, Body.Name.Mercury));

			// Variable positions.
			al.Add(sahamaDNHelper("Yasas", Body.Name.Jupiter, ((BodyPosition)al[0]).longitude, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Mitra", Body.Name.Jupiter, ((BodyPosition)al[0]).longitude, Body.Name.Venus));
			al.Add(sahamaDNHelper("Mahatmya", ((BodyPosition)al[0]).longitude, Body.Name.Mars, Body.Name.Lagna));
				
			Body.Name bLagnaLord = this.LordOfZodiacHouse(zh_lagna, new Division(Basics.DivisionType.Rasi));
			if (bLagnaLord != Body.Name.Mars)
				al.Add(sahamaDNHelper("Samartha", Body.Name.Mars, bLagnaLord, Body.Name.Lagna));
			else
				al.Add(sahamaDNHelper("Samartha", Body.Name.Jupiter, Body.Name.Mars, Body.Name.Lagna));

			al.Add(sahamaHelper("Bhratri", Body.Name.Jupiter, Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Gaurava", Body.Name.Jupiter, Body.Name.Moon, Body.Name.Sun));
			al.Add(sahamaDNHelper("Pitri", Body.Name.Saturn, Body.Name.Sun, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Rajya", Body.Name.Saturn, Body.Name.Sun, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Matri", Body.Name.Moon, Body.Name.Venus, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Putra", Body.Name.Jupiter, Body.Name.Moon, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Jeeva", Body.Name.Saturn, Body.Name.Jupiter, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Karma", Body.Name.Mars, Body.Name.Mercury, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Roga", Body.Name.Lagna, Body.Name.Moon, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Kali", Body.Name.Jupiter, Body.Name.Mars, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Bandhu", Body.Name.Mercury, Body.Name.Moon, Body.Name.Lagna));
			al.Add(sahamaHelper("Mrityu", lon_base.add(8.0*30.0), Body.Name.Moon, Body.Name.Lagna));
			al.Add(sahamaHelper("Paradesa", lon_base.add(9.0*30.0), 
				this.LordOfZodiacHouse(zh_lagna.add(9), new Division(Basics.DivisionType.Rasi)),
				Body.Name.Lagna));
			al.Add(sahamaHelper("Artha", lon_base.add(2.0*30.0), 
				this.LordOfZodiacHouse(zh_lagna.add(2), new Division(Basics.DivisionType.Rasi)),
				Body.Name.Lagna));
			al.Add(sahamaDNHelper("Paradara", Body.Name.Venus, Body.Name.Sun, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Vanik", Body.Name.Moon, Body.Name.Mercury, Body.Name.Lagna));

			if (bDay)
				al.Add(sahamaHelper("Karyasiddhi", Body.Name.Saturn, Body.Name.Sun,
					this.LordOfZodiacHouse(zh_sun, new Division(Basics.DivisionType.Rasi))));
			else
				al.Add(sahamaHelper("Karyasiddhi", Body.Name.Saturn, Body.Name.Moon,
					this.LordOfZodiacHouse(zh_moon, new Division(Basics.DivisionType.Rasi))));
				
			al.Add(sahamaDNHelper("Vivaha", Body.Name.Venus, Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaHelper("Santapa", Body.Name.Saturn, Body.Name.Moon, lon_base.add(6.0*30.0)));
			al.Add(sahamaDNHelper("Sraddha", Body.Name.Venus, Body.Name.Mars, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Preeti", 
				((BodyPosition)al[2]).longitude, ((BodyPosition)al[0]).longitude, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Jadya", Body.Name.Mars, Body.Name.Saturn, Body.Name.Mercury));
			al.Add(sahamaHelper("Vyapara", Body.Name.Mars, Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Satru", Body.Name.Mars, Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Jalapatana", new Longitude(105), Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Bandhana", ((BodyPosition)al[0]).longitude, Body.Name.Saturn, Body.Name.Lagna));
			al.Add(sahamaDNHelper("Apamrityu", lon_base.add(8.0*30.0), Body.Name.Mars, Body.Name.Lagna));
			al.Add(sahamaHelper("Labha", lon_base.add(11.0*30.0), 
				this.LordOfZodiacHouse(zh_lagna.add(11), new Division(Basics.DivisionType.Rasi)), Body.Name.Lagna));
	
			this.positionList.AddRange(al);
			return al;
		}
	}
}
