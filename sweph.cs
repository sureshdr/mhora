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
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace mhora
{

	/// <summary>
	/// Errors found withitn the unmanaged Swiss Ephemeris Library
	/// </summary>
	public class SwephException : Exception 
	{
		public string status;
		public SwephException () : base () 
		{
			status = null;
		}
		public SwephException (string message)
		{
			status = message;
		}
	}


	/// <summary>
	/// A Simple wrapper around the swiss ephemeris DLL functions
	/// Many function arguments use sane defaults for Jyotish programs
	/// For documentation go to http://www.astro.ch and follow the
	/// Swiss Ephemeris (for programmers) link.
	/// </summary>
	public class sweph
	{

		public static int SEFLG_SWIEPH = 2;
		public static int SEFLG_TRUEPOS = 16;
		public static int SEFLG_SPEED = 256;
		public static int SEFLG_SIDEREAL = 64 * 1024;
        public static int iflag = SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_SIDEREAL;

		public static int SE_AYANAMSA_LAHIRI = 1;
		public static int SE_AYANAMSA_RAMAN = 3;
		public static int ayanamsa = SE_AYANAMSA_LAHIRI;

		public static int SE_SUN = 0;
		public static int SE_MOON = 1;
		public static int SE_MERCURY = 2;
		public static int SE_VENUS = 3;
		public static int SE_MARS = 4;
		public static int SE_JUPITER = 5;
		public static int SE_SATURN = 6;
		public static int SE_MEAN_NODE = 10;
		public static int SE_TRUE_NODE = 11;

		public static int SE_CALC_RISE = 1;
		public static int SE_CALC_SET =	2;
		public static int SE_CALC_MTRANSIT = 4;
		public static int SE_CALC_ITRANSIT = 8;
		public static int SE_BIT_DISC_CENTER = 256;
		public static int SE_BIT_NO_REFRACTION = 512;

		public static int SE_WK_MONDAY = 0;
		public static int SE_WK_TUESDAY = 1;
		public static int SE_WK_WEDNESDAY = 2;
		public static int SE_WK_THURSDAY = 3;
		public static int SE_WK_FRIDAY = 4;
		public static int SE_WK_SATURDAY = 5;
		public static int SE_WK_SUNDAY = 6;


		private static Horoscope mCurrentLockHolder = null;
		private static Object SwephLockObject = null;

		public static void checkLock ()
		{
			lock (sweph.SwephLockObject)
			{
				if (mCurrentLockHolder == null)
					throw new Exception ("Sweph: Unable to run. Sweph lock not obtained");
			}
		}
		public static void obtainLock (Horoscope h)
		{
			if (sweph.SwephLockObject == null)
				sweph.SwephLockObject = new Object();

			lock (sweph.SwephLockObject)
			{
				if (mCurrentLockHolder != null)
					throw new Exception("Sweph: obtainLock failed. Sweph Lock still held");

				//Debug.WriteLine("Sweph Lock obtained");
				mCurrentLockHolder = h;
				sweph.swe_set_sid_mode ((int)h.options.Ayanamsa, 0.0, 0.0);
			}
		}
		public static void releaseLock (Horoscope h)
		{
			if (mCurrentLockHolder == null)
				throw new Exception("Sweph: releaseLock failed. Lock not held");
			else if (mCurrentLockHolder != h)
				throw new Exception("Sweph: releaseLock failed. Not lock owner");
			//Debug.WriteLine("Sweph Lock released");
			mCurrentLockHolder = null;
		}
		
		public static int BodyNameToSweph (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return sweph.SE_SUN;
				case Body.Name.Moon: return sweph.SE_MOON;
				case Body.Name.Mars: return sweph.SE_MARS;
				case Body.Name.Mercury: return sweph.SE_MERCURY;
				case Body.Name.Jupiter: return sweph.SE_JUPITER;
				case Body.Name.Venus: return sweph.SE_VENUS;
				case Body.Name.Saturn: return sweph.SE_SATURN;
				case Body.Name.Lagna: return sweph.SE_BIT_NO_REFRACTION;
				default: 
					throw new Exception();
			}
		}
		[DllImport("mhora", CharSet=CharSet.Ansi)]
		public extern static void swe_set_ephe_path (string path);

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_set_sid_mode")]
		private extern static void xyz_swe_set_sid_mode (int sid_mode, double t0, double ayan_t0);

		public static void swe_set_sid_mode (int sid_mode, double t0, double ayan_t0)
		{
			sweph.checkLock();
			xyz_swe_set_sid_mode (sid_mode, 0.0, 0.0); 
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_julday")]
		public extern static double xyz_swe_julday (int year, int month, int day, double hour, int gregflag);

		public static double swe_julday (int year, int month, int day, double hour) 
		{
			return xyz_swe_julday (year, month, day, hour, 1);
		}
    
		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_revjul")]
		private extern static double xyz_swe_revjul (double tjd, int gregflag, ref int year, ref int month, ref int day, ref double hour);    

		public static double swe_revjul (double tjd, ref int year, ref int month, ref int day, ref double hour) 
		{
			return xyz_swe_revjul (tjd, 1, ref year, ref month, ref day, ref hour);
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_calc_ut")]
		private extern static int xyz_swe_calc_ut (double tjd_ut, int ipl, int iflag, double[] xx, StringBuilder serr);

		public static void swe_calc_ut (double tjd_ut, int ipl, int addFlags, double[] xx) 
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_calc_ut (tjd_ut, ipl, iflag | addFlags, xx, serr);
			if (ret < 0) 
			{
				Console.WriteLine("Sweph Error: {0}", serr);
				throw new SwephException (serr.ToString());
			}
			xx[0] += sweph.mCurrentLockHolder.options.AyanamsaOffset.toDouble();
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_sol_eclipse_when_glob")]
		private extern static int xyz_swe_sol_eclipse_when_glob (double tjd_ut, int iflag, int ifltype, double[] tret, bool backward, StringBuilder s);

		public static void swe_sol_eclipse_when_glob (double tjd_ut, double[] tret, bool forward)
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_sol_eclipse_when_glob(tjd_ut, iflag, 0, tret, !forward, serr);
			if (ret < 0)
			{
				Console.WriteLine("Sweph Error: {0}", serr);
				throw new SwephException (serr.ToString());
			}
		}
		
		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_sol_eclipse_when_loc")]
		private extern static int xyz_swe_sol_eclipse_when_loc (double tjd_ut, int iflag, double[] geopos, double[] tret, double[] attr, bool backward, StringBuilder s);

		public static void swe_sol_eclipse_when_loc (HoraInfo hi, double tjd_ut, double[] tret, double[] attr, bool forward)
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			double[] geopos = new Double[3]{hi.lon.toDouble(), hi.lat.toDouble(), hi.alt};
			int ret = xyz_swe_sol_eclipse_when_loc(tjd_ut, iflag, geopos, tret, attr, !forward, serr);
			if (ret < 0)
			{
				Console.WriteLine("Sweph Error: {0}", serr);
				throw new SwephException (serr.ToString());
			}
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_lun_eclipse_when")]
		private extern static int xyz_swe_lun_eclipse_when (double tjd_ut, int iflag, int ifltype, double[] tret, bool backward, StringBuilder s);

		public static void swe_lun_eclipse_when (double tjd_ut, double[] tret, bool forward)
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_lun_eclipse_when(tjd_ut, iflag, 0, tret, !forward, serr);
			if (ret < 0)
			{
				Console.WriteLine("Sweph Error: {0}", serr);
				throw new SwephException (serr.ToString());
			}
		}
		
		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_lun_occult_when_loc")]
		private extern static int xyz_swe_lun_occult_when_loc (
			double tjd_ut, int ipl, ref string starname, int iflag, 
			double[] geopos, double[] tret, double[] attr, bool backward, StringBuilder s);


		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_get_ayanamsa_ut")]
		private extern static double xyz_swe_get_ayanamsa_ut (double tjd_ut);

		public static double swe_get_ayanamsa_ut (double tjd_ut)
		{
			sweph.checkLock();
			return sweph.xyz_swe_get_ayanamsa_ut(tjd_ut);
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_rise_trans")]
		private extern static int xyz_swe_rise_trans (double tjd_ut, int ipl, string starname, int epheflag, int rsmi, double[] geopos, double atpress, double attemp, double[] tret, StringBuilder serr);

		public static void swe_rise (double tjd_ut, int ipl, int rsflag, double[] geopos, double atpress, double attemp, double[] tret) 
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_rise_trans (tjd_ut, ipl, "", iflag, SE_CALC_RISE | rsflag
				, geopos, atpress, attemp, tret, serr);
			if (ret < 0) 
			{
				Debug.WriteLine(serr.ToString(), "Sweph");
				throw new SwephException (serr.ToString());
			}
		}
		public static void swe_set (double tjd_ut, int ipl, int rsflag, double[] geopos, double atpress, double attemp, double[] tret) 
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_rise_trans (tjd_ut, ipl, "", iflag, SE_CALC_SET | rsflag, 
				geopos, atpress, attemp, tret, serr);
			if (ret < 0)
			{
				Debug.WriteLine(serr.ToString(), "Sweph");
				throw new SwephException (serr.ToString());
			}
		}

		public static void swe_lmt (double tjd_ut, int ipl, int rsflag, double[] geopos, double atpress, double attemp, double[] tret) 
		{
			sweph.checkLock();
			StringBuilder serr = new StringBuilder(256);
			int ret = xyz_swe_rise_trans (tjd_ut, ipl, "", iflag, rsflag, 
				geopos, atpress, attemp, tret, serr);

			if (ret < 0)
			{
				Debug.WriteLine(serr.ToString(), "Sweph");
				throw new SwephException (serr.ToString());
			}
		}


		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_houses_ex")]
		private extern static int xyz_swe_houses_ex (double tjd_ut, int iflag, double lat, double lon, int hsys, double[] cusps, double[] ascmc);

		public static int swe_houses_ex (double tjd_ut, int iflag, double lat, double lon, int hsys, double[] cusps, double[] ascmc)
		{
			sweph.checkLock();
			int ret = xyz_swe_houses_ex (tjd_ut, iflag, lat, lon, hsys, cusps, ascmc);

			Longitude lOffset = new Longitude(sweph.mCurrentLockHolder.options.AyanamsaOffset.toDouble());

			// House cusps defined from 1 to 12 inclusive as per sweph docs
			// Ascendants defined from 0 to 7 inclusive as per sweph docs
			for (int i=1; i<=12; i++)
				cusps[i] = new Longitude(cusps[i]).add(lOffset).value;
			for (int i=0; i<=7; i++)
				ascmc[i] = new Longitude(ascmc[i]).add(lOffset).value;
			return ret;
		}

		public static double swe_lagna (double tjd_ut) 
		{
			sweph.checkLock();
			HoraInfo hi = sweph.mCurrentLockHolder.info;
			double[] cusps = new double[13];
			double[] ascmc = new double[10];
			int ret = swe_houses_ex (tjd_ut, sweph.SEFLG_SIDEREAL, hi.lat.toDouble(), hi.lon.toDouble(), 'R', cusps, ascmc);
			return ascmc[0];
		}

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_day_of_week")]
		public extern static int swe_day_of_week (double jd);

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_deltat")]
		public extern static double swe_deltat (double tjd_et);

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_set_tid_acc")]
		private extern static void swe_set_tid_acc (double t_acc);

		[DllImport("mhora", CharSet=CharSet.Ansi, EntryPoint="swe_time_equ")]
		public extern static int swe_time_equ (double tjd_et, ref double e, StringBuilder s);




	}
}
