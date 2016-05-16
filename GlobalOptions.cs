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
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;

using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
namespace mhora
{

	[Serializable]
	public class MhoraSerializableOptions
	{
		protected void Constructor (System.Type ty, SerializationInfo info, StreamingContext context)
		{
			
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(ty, context);
			for (int i=0; i< mi.Length; i++)
			{
				FieldInfo fi = (FieldInfo)mi[i];
				//Console.WriteLine ("User Preferences: Reading {0}", fi);
				try { 
					fi.SetValue (this, info.GetValue(fi.Name, fi.FieldType));	
				}
				catch 
				{
					//Console.WriteLine ("    Not found");
				}
			}
		}

		protected void GetObjectData(
			System.Type ty, SerializationInfo info, StreamingContext context) 
		{
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(ty, context);
			for (int i=0; i< mi.Length; i++) 
			{
				//Console.WriteLine ("User Preferences: Writing {0}", mi[i].Name);
				info.AddValue(mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));
			}
		}
		static public string getExeDir ()
		{
		
			Process oLocal = Process.GetCurrentProcess();
			ProcessModule oMain = oLocal.MainModule;
			string fileName = Path.GetDirectoryName(oMain.FileName);
			if (fileName[fileName.Length-1] == '\\')
				fileName.Remove(fileName.Length-1, 1);
			//Debug.WriteLine( string.Format("Exe launched from {0}", fileName), "GlobalOptions");
			return fileName;
		}

		static public string getOptsFilename ()
		{
			string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MhoraOptions.xml";
			//Debug.WriteLine( string.Format("Options stored at {0}", fileName), "GlobalOptions");
			return fileName;
		}
	}
	
	[Serializable]
	public class StrengthOptions : MhoraSerializableOptions, ISerializable, ICloneable
	{
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		FindStronger.EGrahaStrength[] mColord = null;
		FindStronger.EGrahaStrength[] mNaisargikaDasaGraha = null;
		FindStronger.EGrahaStrength[] mKarakaKendradiGrahaDasaGraha = null;
		FindStronger.EGrahaStrength[] mKarakaKendradiGrahaDasaColord = null;
		FindStronger.ERasiStrength[] mNavamsaDasaRasi = null;
		FindStronger.ERasiStrength[] mMoolaDasaRasi = null;
		FindStronger.ERasiStrength[] mNarayanaDasaRasi = null;
		FindStronger.ERasiStrength[] mNaisargikaDasaRasi = null;
		FindStronger.ERasiStrength[] mKarakaKendradiGrahaDasaRasi = null;

		public object Clone ()
		{
			StrengthOptions opts = new StrengthOptions();
			opts.Colord = (FindStronger.EGrahaStrength[])this.Colord.Clone();
			opts.NaisargikaDasaGraha = (FindStronger.EGrahaStrength[])this.NaisargikaDasaGraha.Clone();
			opts.NavamsaDasaRasi = (FindStronger.ERasiStrength[])this.NavamsaDasaRasi.Clone();
			opts.MoolaDasaRasi = (FindStronger.ERasiStrength[])this.MoolaDasaRasi.Clone();
			opts.NarayanaDasaRasi = (FindStronger.ERasiStrength[])this.NarayanaDasaRasi.Clone();
			opts.NaisargikaDasaRasi = (FindStronger.ERasiStrength[])this.NaisargikaDasaRasi.Clone();
			return opts;
		}

		public object Copy (object o)
		{
			StrengthOptions so = (StrengthOptions)o;
			this.Colord = (FindStronger.EGrahaStrength[])so.Colord.Clone();
			this.NaisargikaDasaGraha = (FindStronger.EGrahaStrength[])so.NaisargikaDasaGraha.Clone();
			this.NavamsaDasaRasi = (FindStronger.ERasiStrength[])so.NavamsaDasaRasi.Clone();
			this.MoolaDasaRasi = (FindStronger.ERasiStrength[])so.MoolaDasaRasi.Clone();
			this.NarayanaDasaRasi = (FindStronger.ERasiStrength[])so.NarayanaDasaRasi.Clone();
			this.NaisargikaDasaRasi = (FindStronger.ERasiStrength[])so.NaisargikaDasaRasi.Clone();
			return this.Clone();
		}

		[Category("Co-Lord Strengths")]
		[PGDisplayName("Graha Strength")]
		public FindStronger.EGrahaStrength[] Colord
		{
			get { return mColord; }
			set { mColord = value; }
		}

		[Category("Naisargika Dasa Strengths")]
		[PGDisplayName("Graha Strengths")]
		public FindStronger.EGrahaStrength[] NaisargikaDasaGraha
		{
			get { return mNaisargikaDasaGraha; }
			set { mNaisargikaDasaGraha = value; }
		}

		[Category("Naisargika Dasa Strengths")]
		[PGDisplayName("Rasi Strengths")]
		public FindStronger.ERasiStrength[] NaisargikaDasaRasi
		{
			get { return mNaisargikaDasaRasi; }
			set { mNaisargikaDasaRasi = value; }
		}

		[Category("Navamsa Dasa Strengths")]
		[PGDisplayName("Rasi Strengths")]
		public FindStronger.ERasiStrength[] NavamsaDasaRasi
		{
			get { return mNavamsaDasaRasi; }
			set { mNavamsaDasaRasi = value; }
		}

		[Category("Moola Dasa Strengths")]
		[PGDisplayName("Rasi Strengths")]
		public FindStronger.ERasiStrength[] MoolaDasaRasi
		{
			get { return mMoolaDasaRasi; }
			set { mMoolaDasaRasi = value; }
		}

		[Category("Narayana Dasa Strengths")]
		[PGDisplayName("Rasi Strengths")]
		public FindStronger.ERasiStrength[] NarayanaDasaRasi
		{
			get { return mNarayanaDasaRasi; }
			set { mNarayanaDasaRasi = value; }
		}

		[Category("Karaka Kendradi Graha Dasa")]
		[PGDisplayName("Rasi Strengths")]
		public FindStronger.ERasiStrength[] KarakaKendradiGrahaDasaRasi
		{
			get { return mKarakaKendradiGrahaDasaRasi; }
			set { mKarakaKendradiGrahaDasaRasi = value; }
		}

		[Category("Karaka Kendradi Graha Dasa")]
		[PGDisplayName("Graha Strengths")]
		public FindStronger.EGrahaStrength[] KarakaKendradiGrahaDasaGraha
		{
			get { return mKarakaKendradiGrahaDasaGraha; }
			set { mKarakaKendradiGrahaDasaGraha = value; }
		}

		[PGNotVisible]
		[Category("Karaka Kendradi Graha Dasa")]
		[PGDisplayName("CoLord Strengths")]
		[TypeConverter(typeof(MhoraArrayConverter))]
		public FindStronger.EGrahaStrength[] KarakaKendradiGrahaDasaColord
		{
			get { return mKarakaKendradiGrahaDasaColord; }
			set { mKarakaKendradiGrahaDasaColord = value; }
		}


		public StrengthOptions ()
		{
			Colord = new FindStronger.EGrahaStrength[]
			{
				FindStronger.EGrahaStrength.NotInOwnHouse,
				FindStronger.EGrahaStrength.AspectsRasi,
				FindStronger.EGrahaStrength.Exaltation,
				FindStronger.EGrahaStrength.RasisNature,
				FindStronger.EGrahaStrength.NarayanaDasaLength,
				FindStronger.EGrahaStrength.Longitude
			};

			NaisargikaDasaGraha = new FindStronger.EGrahaStrength[]
			{
				FindStronger.EGrahaStrength.Exaltation,
				FindStronger.EGrahaStrength.LordInOwnHouse,
				FindStronger.EGrahaStrength.MoolaTrikona,
				FindStronger.EGrahaStrength.Longitude
			};

			KarakaKendradiGrahaDasaGraha = new FindStronger.EGrahaStrength[]
			{
				FindStronger.EGrahaStrength.Exaltation,
				FindStronger.EGrahaStrength.MoolaTrikona,
				FindStronger.EGrahaStrength.OwnHouse,
				FindStronger.EGrahaStrength.Longitude
			};

			KarakaKendradiGrahaDasaRasi = new FindStronger.ERasiStrength[]
			{
				FindStronger.ERasiStrength.Conjunction,
				FindStronger.ERasiStrength.AspectsRasi,
				FindStronger.ERasiStrength.Exaltation,
				FindStronger.ERasiStrength.MoolaTrikona,
				FindStronger.ERasiStrength.OwnHouse,
				FindStronger.ERasiStrength.LordsNature,
				FindStronger.ERasiStrength.AtmaKaraka,
				FindStronger.ERasiStrength.Longitude,
				FindStronger.ERasiStrength.LordInDifferentOddity,
				FindStronger.ERasiStrength.KarakaKendradiGrahaDasaLength
			};
			KarakaKendradiGrahaDasaColord = new FindStronger.EGrahaStrength[]
			{
				FindStronger.EGrahaStrength.NotInOwnHouse,
				FindStronger.EGrahaStrength.Conjunction,
				FindStronger.EGrahaStrength.AspectsRasi,
				FindStronger.EGrahaStrength.Exaltation,
				FindStronger.EGrahaStrength.MoolaTrikona,
				FindStronger.EGrahaStrength.OwnHouse,
				FindStronger.EGrahaStrength.LordsNature,
				FindStronger.EGrahaStrength.AtmaKaraka,
				FindStronger.EGrahaStrength.Longitude,
				FindStronger.EGrahaStrength.LordInDifferentOddity,
				FindStronger.EGrahaStrength.KarakaKendradiGrahaDasaLength
			};

			NavamsaDasaRasi = new FindStronger.ERasiStrength[]
			{
				FindStronger.ERasiStrength.AspectsRasi,
				FindStronger.ERasiStrength.Conjunction,
				FindStronger.ERasiStrength.Exaltation,
				FindStronger.ERasiStrength.LordInDifferentOddity,
				FindStronger.ERasiStrength.RasisNature,
				FindStronger.ERasiStrength.LordsLongitude
			};

			MoolaDasaRasi = new FindStronger.ERasiStrength[]
			{
				FindStronger.ERasiStrength.Conjunction,
				FindStronger.ERasiStrength.Exaltation,
				FindStronger.ERasiStrength.MoolaTrikona,
				FindStronger.ERasiStrength.OwnHouse,
				FindStronger.ERasiStrength.RasisNature,
				FindStronger.ERasiStrength.LordsLongitude
			};

			NarayanaDasaRasi = new FindStronger.ERasiStrength[]
			{
				FindStronger.ERasiStrength.Conjunction,
				FindStronger.ERasiStrength.AspectsRasi,
				FindStronger.ERasiStrength.Exaltation,
				FindStronger.ERasiStrength.LordInDifferentOddity,
				FindStronger.ERasiStrength.RasisNature,
				FindStronger.ERasiStrength.LordsLongitude
			};

			NaisargikaDasaRasi = new FindStronger.ERasiStrength[]
			{
				FindStronger.ERasiStrength.Conjunction,
				FindStronger.ERasiStrength.AspectsRasi,
				FindStronger.ERasiStrength.Exaltation,
				FindStronger.ERasiStrength.RasisNature,
				FindStronger.ERasiStrength.LordIsAtmaKaraka,
				FindStronger.ERasiStrength.LordInDifferentOddity,
				FindStronger.ERasiStrength.Longitude
			};
		}

		protected StrengthOptions (SerializationInfo info, StreamingContext context):
		this ()
		{
			this.Constructor(this.GetType(), info, context);
		}
	}

	/// <summary>
	/// Summary description for GlobalOptions.
	/// </summary>
	[XmlRoot("MhoraOptions")]
	[Serializable]
	public class MhoraGlobalOptions: MhoraSerializableOptions, ISerializable
	{
		//[NonSerialized]	public static object Reference = null;
		[NonSerialized] public static object mainControl = null;

		public HoroscopeOptions HOptions = null;
		public StrengthOptions SOptions = null;

		// General
		bool mbShowSplashScreeen;
		bool mbSavePrefsOnExit;
		private string msNotesExtension;

		private HMSInfo mLat;
		private HMSInfo mLon;
		private HMSInfo mTz;

		// Dasa Control
		private bool bDasaHoverSelect;
		private bool bDasaMoveSelect;
		private bool bDasaShowEvents;
		private int miDasaShowEventsLevel;
		private Color mcDasaBackColor;
		private Color mcDasaDateColor;
		private Color mcDasaPeriodColor;
		private Color mcDasaHighlightColor;

		// Body Colors
		private Color mcBodyLagna;
		private Color mcBodySun;
		private Color mcBodyMoon;
		private Color mcBodyMars;
		private Color mcBodyMercury;
		private Color mcBodyJupiter;
		private Color mcBodyVenus;
		private Color mcBodySaturn;
		private Color mcBodyRahu;
		private Color mcBodyKetu;
		private Color mcBodyOther;

		// General Font families
		private Font mfGeneral;
		private Font mfFixedWidth;

		// Varga charts
		private bool bVargaSquare;
		private bool bVargaShowDob;
		private bool bVargaShowSAVRasi;
		private bool bVargaShowSAVVarga;
		private Color mcVargaBackground;
		private Color mcVargaSecondary;
		private Color mcVargaGraha;
		private Color mcVargaLagna;
		private Color mcVargaSAV;
		private Color mcVargaSpecialLagna;
		private DivisionalChart.UserOptions.EChartStyle mChartStyle;
		private Font mfVarga;

		// Tabular Displays
		private Color mcTableBackground;
		private Color mcTableForeground;
		private Color mcTableInterleaveFirst;
		private Color mcTableInterleaveSecond;

		// Chakra Displays
		private Color mcChakraBackground;

		// Form Widths
		public Size GrahaStrengthsFormSize = new Size(0, 0);
		public Size RasiStrengthsFormSize = new Size(0, 0);
		public Size VargaRectificationFormSize = new Size(0, 0);

		protected const string CAT_GENERAL = "1: General Settings";
		protected const string CAT_LOCATION = "2: Default Location";
		protected const string CAT_LF_GEN = "3: Look and Feel";
		protected const string CAT_LF_DASA = "3: Look and Feel: Dasa";
		protected const string CAT_LF_DIV = "4: Look and Feel: Vargas";
		protected const string CAT_LF_TABLE = "5: Look and Feel: Tabular Charts";
		protected const string CAT_LF_CHAKRA = "6: Look and Feel: Chakras";
		protected const string CAT_LF_BINDUS = "7: Look and Feel: Bindus";

		public static MhoraGlobalOptions Instance;
		public static event EvtChanged DisplayPrefsChanged = null;
		public static event EvtChanged CalculationPrefsChanged = null;


		public static void NotifyDisplayChange ()
		{
			MhoraGlobalOptions.DisplayPrefsChanged(MhoraGlobalOptions.Instance);
		}

		public static void NotifyCalculationChange ()
		{
			MhoraGlobalOptions.CalculationPrefsChanged(MhoraGlobalOptions.Instance.HOptions);
		}

		public MhoraGlobalOptions()
		{

			HOptions = new HoroscopeOptions();
			SOptions = new StrengthOptions();
			mLat = new HMSInfo (47, 40, 27, HMSInfo.dir_type.NS);
			mLon = new HMSInfo (-122, 7, 13, HMSInfo.dir_type.EW);
			mTz = new HMSInfo (-7, 0, 0, HMSInfo.dir_type.EW);

			this.mfFixedWidth = new Font ("Courier New", 10);
			this.mfGeneral = new Font ("Microsoft Sans Serif", 10);

			this.bDasaHoverSelect = false;
			this.bDasaMoveSelect = true;
			this.bDasaShowEvents = true;
			this.miDasaShowEventsLevel = 2;
			this.mcDasaBackColor = Color.Lavender;
			this.mcDasaDateColor = Color.DarkRed;
			this.mcDasaPeriodColor = Color.DarkBlue;
			this.mcDasaHighlightColor = Color.White;

			this.mbShowSplashScreeen = true;
			this.mbSavePrefsOnExit = true;
			this.msNotesExtension = "txt";

			this.mcBodyLagna = Color.BlanchedAlmond;
			this.mcBodySun = Color.Orange;
			this.mcBodyMoon = Color.LightSkyBlue;
			this.mcBodyMars = Color.Red;
			this.mcBodyMercury =  Color.Green;
			this.mcBodyJupiter = Color.Yellow;
			this.mcBodyVenus = Color.Violet;
			this.mcBodySaturn =  Color.DarkBlue;
			this.mcBodyRahu =  Color.LightBlue;
			this.mcBodyKetu = Color.LightPink;
			this.mcBodyOther = Color.Black;

			this.mcVargaBackground = Color.AliceBlue;
			this.mcVargaSecondary = Color.CadetBlue;
			this.mcVargaGraha = Color.DarkRed;
			this.mcVargaLagna = Color.DarkViolet;
			this.mcVargaSAV = Color.Gainsboro;
			this.mcVargaSpecialLagna = Color.Gray;
			this.mChartStyle = DivisionalChart.UserOptions.EChartStyle.SouthIndian;
			this.mfVarga = new Font ("Times New Roman", 7);
			this.bVargaSquare = true;
			this.bVargaShowDob = true;
			this.bVargaShowSAVVarga = true;
			this.bVargaShowSAVRasi = false;

			this.mcTableBackground = Color.Lavender;
			this.mcTableForeground = Color.Black;
			this.mcTableInterleaveFirst = Color.AliceBlue;
			this.mcTableInterleaveSecond = Color.Lavender;

			this.mcChakraBackground = Color.AliceBlue;
		}


		[Category(CAT_GENERAL)]
		[PropertyOrder(1), PGDisplayName("Show splash screen")]
		public bool ShowSplashScreen
		{
			get { return this.mbShowSplashScreeen; }
			set { this.mbShowSplashScreeen = value; }
		}

		[Category(CAT_GENERAL)]
		[PropertyOrder(2), PGDisplayName("Save Preferences on Exit")]
		public bool SavePrefsOnExit
		{
			get { return this.mbSavePrefsOnExit; }
			set { this.mbSavePrefsOnExit = value; }
		}

		[Category(CAT_GENERAL)]
		[PropertyOrder(3), PGDisplayName("Notes file type")]
		public string ChartNotesFileExtension
		{
			get { return this.msNotesExtension; }
			set { this.msNotesExtension = value; }
		}
		[Category(CAT_GENERAL)]
		[PropertyOrder(4), PGDisplayName("Yogas file name")]
		public string YogasFileName
		{
			get { return MhoraGlobalOptions.getExeDir() + "\\" + "yogas.mhr"; }
		}

		[PropertyOrder(1), Category(CAT_LOCATION)]
		public HMSInfo Latitude
		{
			get { return mLat; }
			set { mLat = value; }
		}
		[PropertyOrder(2), Category(CAT_LOCATION)]
		public HMSInfo Longitude
		{
			get { return mLon; }
			set { mLon = value; }
		}
		[PropertyOrder(3), Category(CAT_LOCATION)]
		[PGDisplayName("Time zone")]
		public HMSInfo TimeZone
		{
			get { return mTz; }
			set { mTz = value; }
		}


		[Category(CAT_LF_GEN)]
		[PGDisplayName("Font")]
		public Font GeneralFont
		{
			get { return this.mfGeneral; }
			set { this.mfGeneral = value; }
		}
		[Category(CAT_LF_GEN)]
		[PGDisplayName("Fixed width font")]
		public Font FixedWidthFont
		{
			get { return this.mfFixedWidth; }
			set { this.mfFixedWidth = value; }
		}

		[PropertyOrder(1), Category(CAT_LF_DASA)]
		[PGDisplayName("Select by Mouse Hover")]
		public bool DasaHoverSelect
		{
			get { return this.bDasaHoverSelect; }
			set { this.bDasaHoverSelect = value; }
		}

		[PropertyOrder(1), Category(CAT_LF_DASA)]
		[PGDisplayName("Select by Mouse Move")]
		public bool DasaMoveSelect
		{
			get { return this.bDasaMoveSelect; }
			set { this.bDasaMoveSelect = value; }
		}

		[PropertyOrder(2), Category(CAT_LF_DASA)]
		[PGDisplayName("Show Events")]
		public bool DasaShowEvents
		{
			get { return this.bDasaShowEvents; }
			set { this.bDasaShowEvents = value; }
		}
		[PropertyOrder(3), Category(CAT_LF_DASA)]
		[PGDisplayName("Show Events Level")]
		public int DasaEventsLevel
		{
			get { return this.miDasaShowEventsLevel; }
			set { this.miDasaShowEventsLevel = value; }
		}

		[PropertyOrder(4), Category(CAT_LF_DASA)]
		[PGDisplayName("Period foreground color")]
		public Color DasaPeriodColor
		{
			get { return this.mcDasaPeriodColor; }
			set { this.mcDasaPeriodColor = value; }
		}
		[PropertyOrder(5), Category(CAT_LF_DASA)]
		[PGDisplayName("Date foreground color")]
		public Color DasaDateColor
		{
			get { return this.mcDasaDateColor; }
			set { this.mcDasaDateColor = value; }
		}
		[PropertyOrder(6), Category(CAT_LF_DASA)]
		[PGDisplayName("Background colour")]
		public Color DasaBackgroundColor
		{
			get { return mcDasaBackColor; }
			set { mcDasaBackColor = value; }
		}
		[PropertyOrder(7), Category(CAT_LF_DASA)]
		[PGDisplayName("Item highlight color")]
		public Color DasaHighlightColor
		{
			get { return this.mcDasaHighlightColor; }
			set { this.mcDasaHighlightColor = value; }
		}

		[PropertyOrder(1), Category(CAT_LF_DIV)]
		[PGDisplayName("Display style")]
		public DivisionalChart.UserOptions.EChartStyle VargaStyle
		{
			get { return this.mChartStyle; }
			set { this.mChartStyle = value; }
		}
		[PropertyOrder(2), Category(CAT_LF_DIV)]
		[PGDisplayName("Maintain square proportions")]
		public bool VargaChartIsSquare
		{
			get { return this.bVargaSquare; }
			set { this.bVargaSquare = value; }
		}
		[PropertyOrder(3), Category(CAT_LF_DIV)]
		[PGDisplayName("Show time of birth")]
		public bool VargaShowDob
		{
			get { return this.bVargaShowDob; }
			set { this.bVargaShowDob = value; }
		}
		[PropertyOrder(4), Category(CAT_LF_DIV)]
		[PGDisplayName("Show rasi's SAV bindus")]
		public bool VargaShowSAVRasi
		{
			get { return this.bVargaShowSAVRasi; }
			set { this.bVargaShowSAVRasi = value; }
		}
		[PropertyOrder(5), Category(CAT_LF_DIV)]
		[PGDisplayName("Show varga's SAV bindus")]
		public bool VargaShowSAVVarga
		{
			get { return this.bVargaShowSAVVarga; }
			set { this.bVargaShowSAVVarga = value; }
		}
		[PropertyOrder(6), Category(CAT_LF_DIV)]
		[PGDisplayName("Background colour")]
		public Color VargaBackgroundColor
		{
			get { return this.mcVargaBackground; }
			set { this.mcVargaBackground = value; }
		}
		[Category(CAT_LF_DIV)]
		[PropertyOrder(7), PGDisplayName("Graha foreground colour")]
		public Color VargaGrahaColor
		{
			get { return this.mcVargaGraha;; }
			set { this.mcVargaGraha = value; }
		}
		[Category(CAT_LF_DIV)]
		[PropertyOrder(8), PGDisplayName("Secondary foreground colour")]
		public Color VargaSecondaryColor
		{
			get { return this.mcVargaSecondary; }
			set { this.mcVargaSecondary = value; }
		}
		[Category(CAT_LF_DIV)]
		[PropertyOrder(9), PGDisplayName("Lagna foreground colour")]
		public Color VargaLagnaColor
		{
			get { return this.mcVargaLagna; }
			set { this.mcVargaLagna = value; }
		}	
		[Category(CAT_LF_DIV)]
		[PropertyOrder(10), PGDisplayName("Special lagna foreground colour")]
		public Color VargaSpecialLagnaColor
		{
			get { return this.mcVargaSpecialLagna; }
			set { this.mcVargaSpecialLagna = value; }
		}
		[Category(CAT_LF_DIV)]
		[PropertyOrder(11), PGDisplayName("SAV foreground colour")]
		public Color VargaSAVColor
		{
			get { return this.mcVargaSAV; }
			set { this.mcVargaSAV = value; }
		}	
		[Category(CAT_LF_DIV)]
		[PropertyOrder(12), PGDisplayName("Font")]
		public Font VargaFont
		{
			get { return this.mfVarga; }
			set { this.mfVarga = value; }
		}

		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(1), PGDisplayName("Lagna")]
		public Color BindusLagnaColor
		{
			get { return this.mcBodyLagna; }
			set { this.mcBodyLagna = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(2), PGDisplayName("Sun")]
		public Color BindusSunColor
		{
			get { return this.mcBodySun; }
			set { this.mcBodySun = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(3), PGDisplayName("Moon")]
		public Color BindusMoonColor
		{
			get { return this.mcBodyMoon; }
			set { this.mcBodyMoon = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(4), PGDisplayName("Mars")]
		public Color BindusMarsColor
		{
			get { return this.mcBodyMars; }
			set { this.mcBodyMars = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(5), PGDisplayName("Mercury")]
		public Color BindusMercuryColor
		{
			get { return this.mcBodyMercury; }
			set { this.mcBodyMercury = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(6), PGDisplayName("Jupiter")]
		public Color BindusJupiterColor
		{
			get { return this.mcBodyJupiter; }
			set { this.mcBodyJupiter = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(7), PGDisplayName("Venus")]
		public Color BindusVenusColor
		{
			get { return this.mcBodyVenus; }
			set { this.mcBodyVenus = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(8), PGDisplayName("Saturn")]
		public Color BindusSaturnColor
		{
			get { return this.mcBodySaturn; }
			set { this.mcBodySaturn = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(9), PGDisplayName("Rahu")]
		public Color BindusRahuColor
		{
			get { return this.mcBodyRahu; }
			set { this.mcBodyRahu = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(10), PGDisplayName("Ketu")]
		public Color BindusKetuColor
		{
			get { return this.mcBodyKetu; }
			set { this.mcBodyKetu = value; }
		}
		[Category(CAT_LF_BINDUS)]
		[PropertyOrder(11), PGDisplayName("Other")]
		public Color BindusOtherColor
		{
			get { return this.mcBodyOther; }
			set { this.mcBodyOther = value; }
		}

		[Category(CAT_LF_TABLE)]
		[PropertyOrder(1), PGDisplayName("Background colour")]
		public Color TableBackgroundColor
		{
			get { return this.mcTableBackground; }
			set { this.mcTableBackground = value; }
		}
		[Category(CAT_LF_TABLE)]
		[PropertyOrder(2), PGDisplayName("Foreground colour")]
		public Color TableForegroundColor
		{
			get { return this.mcTableForeground; }
			set { this.mcTableForeground = value; }
		}
		[Category(CAT_LF_TABLE)]
		[PropertyOrder(3), PGDisplayName("Interleave colour (odd)")]
		public Color TableOddRowColor
		{
			get { return this.mcTableInterleaveFirst; }
			set { this.mcTableInterleaveFirst = value; }
		}
		[Category(CAT_LF_TABLE)]
		[PropertyOrder(4), PGDisplayName("Interleave colour (even)")]
		public Color TableEvenRowColor
		{
			get { return this.mcTableInterleaveSecond; }
			set { this.mcTableInterleaveSecond = value; }
		}

		[Category(CAT_LF_CHAKRA)]
		[PGDisplayName("Background colour")]
		public Color ChakraBackgroundColor
		{
			get { return this.mcChakraBackground; }
			set { this.mcChakraBackground = value; }
		}

		private Font addToFontSizesHelper (Font f, int i)
		{
			return new Font(f.FontFamily, f.SizeInPoints+i);
		}
		private void addToFontSizes (int i)
		{
			this.mfFixedWidth = this.addToFontSizesHelper(this.mfFixedWidth, i);
			this.mfGeneral = this.addToFontSizesHelper(this.mfGeneral, i);
			this.mfVarga = this.addToFontSizesHelper(this.mfVarga, i);
		}
		public void increaseFontSize()
		{
			this.addToFontSizes(1);
		}
		public void decreaseFontSize()
		{
			this.addToFontSizes(-1);
		}

		public Color getBinduColor (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Lagna: return this.mcBodyLagna;
				case Body.Name.Sun: return this.mcBodySun;
				case Body.Name.Moon: return this.mcBodyMoon;
				case Body.Name.Mars: return this.mcBodyMars;
				case Body.Name.Mercury: return this.mcBodyMercury;
				case Body.Name.Jupiter: return this.mcBodyJupiter;
				case Body.Name.Venus: return this.mcBodyVenus;
				case Body.Name.Saturn: return this.mcBodySaturn;
				case Body.Name.Rahu: return this.mcBodyRahu;
				case Body.Name.Ketu: return this.mcBodyKetu;										
				default: return this.mcBodyOther;
			}
		}


		static public MhoraGlobalOptions readFromFile()
		{
			MhoraGlobalOptions gOpts = new MhoraGlobalOptions();
			try 
			{
				FileStream sOut;
				sOut = new FileStream(MhoraGlobalOptions.getOptsFilename(), FileMode.Open, FileAccess.Read);
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
				gOpts = (MhoraGlobalOptions)formatter.Deserialize(sOut);
				sOut.Close();
			}
			catch {
				Console.WriteLine("MHora: Unable to read user preferences", "GlobalOptions");
			}

			MhoraGlobalOptions.Instance = gOpts;
			return gOpts;
		}

		public void saveToFile ()
		{
			Console.WriteLine ("Saving Preferences to {0}", MhoraGlobalOptions.getOptsFilename());
			FileStream sOut = new FileStream(MhoraGlobalOptions.getOptsFilename(), FileMode.OpenOrCreate, FileAccess.Write);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize (sOut, this);
			sOut.Close();
		}

		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context) 
		{
			this.GetObjectData(this.GetType(), info, context);
		}

		protected MhoraGlobalOptions (SerializationInfo info, StreamingContext context):
			this ()
		{
			this.Constructor(this.GetType(), info, context);
		}

	}
}
