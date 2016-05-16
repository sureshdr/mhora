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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;


namespace mhora
{


	/// <summary>
	/// Summary description for DivisionalChart.
	/// </summary>
	[Serializable()]
	public class DivisionalChart : MhoraControl //System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem mOptions;
		private IDrawChart dc;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mRasi;
		private System.Windows.Forms.MenuItem mNavamsa;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mDrekkanaParasara;
		private System.Windows.Forms.MenuItem mChaturamsa;
		private System.Windows.Forms.MenuItem mPanchamsa;
		private System.Windows.Forms.MenuItem mShashtamsa;
		private System.Windows.Forms.MenuItem mSaptamsa;
		private System.Windows.Forms.MenuItem mDasamsa;
		private System.Windows.Forms.MenuItem mDwadasamsa;
		private System.Windows.Forms.MenuItem mShodasamsa;
		private System.Windows.Forms.MenuItem mVimsamsa;
		private System.Windows.Forms.MenuItem mChaturvimsamsa;
		private System.Windows.Forms.MenuItem mTrimsamsa;
		private System.Windows.Forms.MenuItem mNakshatramsa;
		private System.Windows.Forms.MenuItem mKhavedamsa;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem mDrekkanaJagannath;
		private System.Windows.Forms.MenuItem mDrekkanaSomnath;
		private System.Windows.Forms.MenuItem mDrekkanaParivrittitraya;
		private System.Windows.Forms.MenuItem menuItem2;
	
		public class UserOptions : ICloneable
		{
			[TypeConverter(typeof(EnumDescConverter))]
			public enum EChartStyle { 
				[Description("South Indian Square (Jupiter)")]	SouthIndian, 
				[Description("East Indian Square (Sun)")]		EastIndian 
			};
			
			[TypeConverter(typeof(EnumDescConverter))]
			public enum EViewStyle 
			{ 
				[Description("Regular")]						Normal, 
				[Description("Dual Graha Arudhas")]				DualGrahaArudha, 
				[Description("8 Chara Karakas (regular)")]		CharaKarakas8, 
				[Description("7 Chara Karakas (mundane)")]		CharaKarakas7,
				[Description("Varnada Lagna")]					Varnada, 
				[Description("Panchanga Print View")]			Panchanga
			};

			private Division varga;
			private Division innerVarga;
			private EChartStyle mChartStyle;
			private EViewStyle mViewStyle;
			private bool mbShowInner;

			public UserOptions ()
			{
				this.Varga = new Division(Basics.DivisionType.Rasi);
				mViewStyle = EViewStyle.Normal;
				mChartStyle = MhoraGlobalOptions.Instance.VargaStyle;
				varga = new Division(Basics.DivisionType.Rasi);
				innerVarga = new Division(Basics.DivisionType.Rasi);
				mbShowInner = false;
			}

			[Category("Options")]
			[PGDisplayName("Varga")]
			public Division Varga
			{
				get { return varga; }
				set { varga = value; }
			}


			[PGDisplayName("Dual Chart View")]
			public bool ShowInner
			{
				get { return mbShowInner; }
				set { this.mbShowInner = value; }
			}

			[PGDisplayName("View Type")]
			public EViewStyle ViewStyle
			{
				get { return this.mViewStyle; }
				set { this.mViewStyle = value; }
			}
			[Category("Options")]
			[PGDisplayName("Chart style")]
			public EChartStyle ChartStyle
			{
				get { return this.mChartStyle; }
				set { this.mChartStyle = value; }
			}

			public Object Copy (Object o)
			{
				UserOptions uo = (UserOptions)o;
				this.mChartStyle = uo.mChartStyle;
				this.mViewStyle = uo.mViewStyle;
				this.Varga = uo.Varga;
				this.ShowInner = uo.ShowInner;
				return uo;
			}
			public Object Clone ()
			{
				UserOptions uo = new UserOptions();
				uo.Varga = Varga;
				uo.mChartStyle = this.mChartStyle;
				uo.mViewStyle = this.mViewStyle;
				uo.ShowInner = this.ShowInner;
				return uo;
			}
		}
		public UserOptions options;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mHoraKashinath;
		private System.Windows.Forms.MenuItem mHoraParivritti;
		private System.Windows.Forms.MenuItem mHoraParasara;
		private System.Windows.Forms.MenuItem mHoraJagannath;
		private System.Windows.Forms.MenuItem mTrimsamsaParivritti;
		private System.Windows.Forms.MenuItem mLagnaChange;
		public HoroscopeOptions calculation_options;
		private ArrayList div_pos;
		private System.Windows.Forms.MenuItem mExtrapolate;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mShashtyamsa;
		private System.Windows.Forms.MenuItem mAkshavedamsa;
		private System.Windows.Forms.MenuItem mAshtottaramsa;
		private System.Windows.Forms.MenuItem mRudramsaRath;
		private System.Windows.Forms.MenuItem mRudramsaRaman;
		private System.Windows.Forms.MenuItem mNadiamsa;
		private System.Windows.Forms.MenuItem mAshtamsa;
		private System.Windows.Forms.MenuItem mAshtamsaRaman;
		private System.Windows.Forms.MenuItem mTrimsamsaSimple;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem mBhava;
		private System.Windows.Forms.MenuItem mBhavaEqual;
		private System.Windows.Forms.MenuItem mBhavaSripati;
		private System.Windows.Forms.MenuItem mBhavaKoch;
		private System.Windows.Forms.MenuItem mBhavaPlacidus;
		private System.Windows.Forms.MenuItem menuBhavaAlcabitus;
		private System.Windows.Forms.MenuItem menuBhavaRegiomontanus;
		private System.Windows.Forms.MenuItem menuBhavaCampanus;
		private System.Windows.Forms.MenuItem menuBhavaAxial;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem mNadiamsaCKN;
		private ArrayList arudha_pos;
		private ArrayList varnada_pos;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem mViewNormal;
		private System.Windows.Forms.MenuItem mViewDualGrahaArudha;
		private System.Windows.Forms.MenuItem mNavamsaDwadasamsa;
		private System.Windows.Forms.MenuItem mDwadasamsaDwadasamsa;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem mViewCharaKarakas;
		private System.Windows.Forms.MenuItem mViewCharaKarakas7;
		private System.Windows.Forms.MenuItem mViewVarnada;
		private ArrayList graha_arudha_pos;
		private int[] sav_bindus;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem mRegularParivritti;
		private System.Windows.Forms.MenuItem mRegularFromHouse;
		private System.Windows.Forms.MenuItem mRegularTrikona;
		private System.Windows.Forms.MenuItem mRegularKendraChaturthamsa;
		private System.Windows.Forms.MenuItem mRegularSaptamsaBased;
		private System.Windows.Forms.MenuItem mRegularDasamsaBased;
		private System.Windows.Forms.MenuItem mRegularShashthamsaBased;
		private System.Windows.Forms.MenuItem mRegularShodasamsaBased;
		private System.Windows.Forms.MenuItem mRegularVimsamsaBased;
		private System.Windows.Forms.MenuItem mRegularNakshatramsaBased;
		private System.Windows.Forms.MenuItem menuItem12;

		public bool PrintMode = false;

		DivisionalChart innerControl = null;


		public void AddInnerControl ()
		{
			innerControl = new DivisionalChart(h, true);
			this.Controls.Add(innerControl);
		}

		public void Constructor (Horoscope _h)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			h = _h;
			options = new UserOptions();
			calculation_options = h.options;
			h.Changed += new EvtChanged(OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnRedisplay);
			this.OnRecalculate(h);
			this.SetChartStyle(this.options.ChartStyle);
			//dc = new SouthIndianChart();
			//dc = new EastIndianChart();
		}

		bool bInnerMode;
		public DivisionalChart(Horoscope _h) : base ()
		{
			this.Constructor(_h);
			this.bInnerMode = false;
		}

		public DivisionalChart(Horoscope _h, bool bInner) : base ()
		{
			this.Constructor(_h);
			this.bInnerMode = true;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.mLagnaChange = new System.Windows.Forms.MenuItem();
			this.mOptions = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.mViewNormal = new System.Windows.Forms.MenuItem();
			this.mViewDualGrahaArudha = new System.Windows.Forms.MenuItem();
			this.mViewVarnada = new System.Windows.Forms.MenuItem();
			this.mViewCharaKarakas = new System.Windows.Forms.MenuItem();
			this.mViewCharaKarakas7 = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mRasi = new System.Windows.Forms.MenuItem();
			this.mNavamsa = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.mBhava = new System.Windows.Forms.MenuItem();
			this.mBhavaEqual = new System.Windows.Forms.MenuItem();
			this.mBhavaSripati = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuBhavaAlcabitus = new System.Windows.Forms.MenuItem();
			this.menuBhavaAxial = new System.Windows.Forms.MenuItem();
			this.menuBhavaCampanus = new System.Windows.Forms.MenuItem();
			this.mBhavaKoch = new System.Windows.Forms.MenuItem();
			this.mBhavaPlacidus = new System.Windows.Forms.MenuItem();
			this.menuBhavaRegiomontanus = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mHoraParasara = new System.Windows.Forms.MenuItem();
			this.mDrekkanaParasara = new System.Windows.Forms.MenuItem();
			this.mChaturamsa = new System.Windows.Forms.MenuItem();
			this.mPanchamsa = new System.Windows.Forms.MenuItem();
			this.mShashtamsa = new System.Windows.Forms.MenuItem();
			this.mSaptamsa = new System.Windows.Forms.MenuItem();
			this.mAshtamsa = new System.Windows.Forms.MenuItem();
			this.mDasamsa = new System.Windows.Forms.MenuItem();
			this.mDwadasamsa = new System.Windows.Forms.MenuItem();
			this.mShodasamsa = new System.Windows.Forms.MenuItem();
			this.mVimsamsa = new System.Windows.Forms.MenuItem();
			this.mChaturvimsamsa = new System.Windows.Forms.MenuItem();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.mHoraParivritti = new System.Windows.Forms.MenuItem();
			this.mHoraJagannath = new System.Windows.Forms.MenuItem();
			this.mHoraKashinath = new System.Windows.Forms.MenuItem();
			this.mDrekkanaParivrittitraya = new System.Windows.Forms.MenuItem();
			this.mDrekkanaJagannath = new System.Windows.Forms.MenuItem();
			this.mDrekkanaSomnath = new System.Windows.Forms.MenuItem();
			this.mAshtamsaRaman = new System.Windows.Forms.MenuItem();
			this.mRudramsaRath = new System.Windows.Forms.MenuItem();
			this.mRudramsaRaman = new System.Windows.Forms.MenuItem();
			this.mTrimsamsaParivritti = new System.Windows.Forms.MenuItem();
			this.mTrimsamsaSimple = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mNakshatramsa = new System.Windows.Forms.MenuItem();
			this.mTrimsamsa = new System.Windows.Forms.MenuItem();
			this.mKhavedamsa = new System.Windows.Forms.MenuItem();
			this.mAkshavedamsa = new System.Windows.Forms.MenuItem();
			this.mShashtyamsa = new System.Windows.Forms.MenuItem();
			this.mAshtottaramsa = new System.Windows.Forms.MenuItem();
			this.mNadiamsa = new System.Windows.Forms.MenuItem();
			this.mNadiamsaCKN = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.mNavamsaDwadasamsa = new System.Windows.Forms.MenuItem();
			this.mDwadasamsaDwadasamsa = new System.Windows.Forms.MenuItem();
			this.mExtrapolate = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.mRegularParivritti = new System.Windows.Forms.MenuItem();
			this.mRegularFromHouse = new System.Windows.Forms.MenuItem();
			this.mRegularSaptamsaBased = new System.Windows.Forms.MenuItem();
			this.mRegularDasamsaBased = new System.Windows.Forms.MenuItem();
			this.mRegularShashthamsaBased = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.mRegularTrikona = new System.Windows.Forms.MenuItem();
			this.mRegularShodasamsaBased = new System.Windows.Forms.MenuItem();
			this.mRegularVimsamsaBased = new System.Windows.Forms.MenuItem();
			this.mRegularKendraChaturthamsa = new System.Windows.Forms.MenuItem();
			this.mRegularNakshatramsaBased = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.mLagnaChange,
																						this.mOptions,
																						this.menuItem8,
																						this.menuItem1,
																						this.mRasi,
																						this.mNavamsa,
																						this.menuItem7,
																						this.menuItem5,
																						this.mHoraParasara,
																						this.mDrekkanaParasara,
																						this.mChaturamsa,
																						this.mPanchamsa,
																						this.mShashtamsa,
																						this.mSaptamsa,
																						this.mAshtamsa,
																						this.mDasamsa,
																						this.mDwadasamsa,
																						this.mShodasamsa,
																						this.mVimsamsa,
																						this.mChaturvimsamsa,
																						this.menuItem18,
																						this.menuItem6,
																						this.menuItem11,
																						this.menuItem3,
																						this.menuItem4});
			// 
			// mLagnaChange
			// 
			this.mLagnaChange.Index = 0;
			this.mLagnaChange.Text = "Lagna Change";
			this.mLagnaChange.Click += new System.EventHandler(this.mLagnaChange_Click);
			// 
			// mOptions
			// 
			this.mOptions.Index = 1;
			this.mOptions.Text = "&Options";
			this.mOptions.Click += new System.EventHandler(this.mOptions_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 2;
			this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mViewNormal,
																					  this.mViewDualGrahaArudha,
																					  this.mViewVarnada,
																					  this.mViewCharaKarakas,
																					  this.mViewCharaKarakas7});
			this.menuItem8.Text = "View";
			// 
			// mViewNormal
			// 
			this.mViewNormal.Index = 0;
			this.mViewNormal.Text = "Normal";
			this.mViewNormal.Click += new System.EventHandler(this.mViewNormal_Click);
			// 
			// mViewDualGrahaArudha
			// 
			this.mViewDualGrahaArudha.Index = 1;
			this.mViewDualGrahaArudha.Text = "Graha Arudha";
			this.mViewDualGrahaArudha.Click += new System.EventHandler(this.mViewDualGrahaArudha_Click);
			// 
			// mViewVarnada
			// 
			this.mViewVarnada.Index = 2;
			this.mViewVarnada.Text = "Varnada";
			this.mViewVarnada.Click += new System.EventHandler(this.mViewVarnada_Click);
			// 
			// mViewCharaKarakas
			// 
			this.mViewCharaKarakas.Index = 3;
			this.mViewCharaKarakas.Text = "Chara Karakas (8)";
			this.mViewCharaKarakas.Click += new System.EventHandler(this.mViewCharaKarakas_Click);
			// 
			// mViewCharaKarakas7
			// 
			this.mViewCharaKarakas7.Index = 4;
			this.mViewCharaKarakas7.Text = "Chara Karakas (7)";
			this.mViewCharaKarakas7.Click += new System.EventHandler(this.mViewCharaKarakas7_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 3;
			this.menuItem1.Text = "-";
			// 
			// mRasi
			// 
			this.mRasi.Index = 4;
			this.mRasi.Text = "Rasi";
			this.mRasi.Click += new System.EventHandler(this.mRasi_Click);
			// 
			// mNavamsa
			// 
			this.mNavamsa.Index = 5;
			this.mNavamsa.Text = "Navamsa";
			this.mNavamsa.Click += new System.EventHandler(this.mNavamsa_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 6;
			this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mBhava,
																					  this.mBhavaEqual,
																					  this.mBhavaSripati,
																					  this.menuItem9,
																					  this.menuBhavaAlcabitus,
																					  this.menuBhavaAxial,
																					  this.menuBhavaCampanus,
																					  this.mBhavaKoch,
																					  this.mBhavaPlacidus,
																					  this.menuBhavaRegiomontanus});
			this.menuItem7.Text = "Bhavas";
			// 
			// mBhava
			// 
			this.mBhava.Index = 0;
			this.mBhava.Text = "Equal houses (9 padas)";
			this.mBhava.Click += new System.EventHandler(this.mBhava_Click);
			// 
			// mBhavaEqual
			// 
			this.mBhavaEqual.Index = 1;
			this.mBhavaEqual.Text = "Equal houses (30 degrees)";
			this.mBhavaEqual.Click += new System.EventHandler(this.mBhavaEqual_Click);
			// 
			// mBhavaSripati
			// 
			this.mBhavaSripati.Index = 2;
			this.mBhavaSripati.Text = "Sripati (Poryphory)";
			this.mBhavaSripati.Click += new System.EventHandler(this.mBhavaSripati_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 3;
			this.menuItem9.Text = "-";
			// 
			// menuBhavaAlcabitus
			// 
			this.menuBhavaAlcabitus.Index = 4;
			this.menuBhavaAlcabitus.Text = "Alcabitus";
			this.menuBhavaAlcabitus.Click += new System.EventHandler(this.menuBhavaAlcabitus_Click);
			// 
			// menuBhavaAxial
			// 
			this.menuBhavaAxial.Index = 5;
			this.menuBhavaAxial.Text = "Axial";
			this.menuBhavaAxial.Click += new System.EventHandler(this.menuBhavaAxial_Click);
			// 
			// menuBhavaCampanus
			// 
			this.menuBhavaCampanus.Index = 6;
			this.menuBhavaCampanus.Text = "Campanus";
			this.menuBhavaCampanus.Click += new System.EventHandler(this.menuBhavaCampanus_Click);
			// 
			// mBhavaKoch
			// 
			this.mBhavaKoch.Index = 7;
			this.mBhavaKoch.Text = "Koch";
			this.mBhavaKoch.Click += new System.EventHandler(this.mBhavaKoch_Click);
			// 
			// mBhavaPlacidus
			// 
			this.mBhavaPlacidus.Index = 8;
			this.mBhavaPlacidus.Text = "Placidus";
			this.mBhavaPlacidus.Click += new System.EventHandler(this.mBhavaPlacidus_Click);
			// 
			// menuBhavaRegiomontanus
			// 
			this.menuBhavaRegiomontanus.Index = 9;
			this.menuBhavaRegiomontanus.Text = "Regiomontanus";
			this.menuBhavaRegiomontanus.Click += new System.EventHandler(this.menuBhavaRegiomontanus_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 7;
			this.menuItem5.Text = "-";
			// 
			// mHoraParasara
			// 
			this.mHoraParasara.Index = 8;
			this.mHoraParasara.Text = "D-2: Hora";
			this.mHoraParasara.Click += new System.EventHandler(this.mHoraParasara_Click);
			// 
			// mDrekkanaParasara
			// 
			this.mDrekkanaParasara.Index = 9;
			this.mDrekkanaParasara.Text = "D-3: Drekkana";
			this.mDrekkanaParasara.Click += new System.EventHandler(this.mDrekkanaParasara_Click);
			// 
			// mChaturamsa
			// 
			this.mChaturamsa.Index = 10;
			this.mChaturamsa.Text = "D-4: Chaturamsa";
			this.mChaturamsa.Click += new System.EventHandler(this.mChaturamsa_Click);
			// 
			// mPanchamsa
			// 
			this.mPanchamsa.Index = 11;
			this.mPanchamsa.Text = "D-5: Panchamsa";
			this.mPanchamsa.Click += new System.EventHandler(this.mPanchamsa_Click);
			// 
			// mShashtamsa
			// 
			this.mShashtamsa.Index = 12;
			this.mShashtamsa.Text = "D-6: Sashtamsa";
			this.mShashtamsa.Click += new System.EventHandler(this.mShashtamsa_Click);
			// 
			// mSaptamsa
			// 
			this.mSaptamsa.Index = 13;
			this.mSaptamsa.Text = "D-7: Saptamsa";
			this.mSaptamsa.Click += new System.EventHandler(this.mSaptamsa_Click);
			// 
			// mAshtamsa
			// 
			this.mAshtamsa.Index = 14;
			this.mAshtamsa.Text = "D-8: Ashtamsa";
			this.mAshtamsa.Click += new System.EventHandler(this.mAshtamsa_Click);
			// 
			// mDasamsa
			// 
			this.mDasamsa.Index = 15;
			this.mDasamsa.Text = "D-10: Dasamsa";
			this.mDasamsa.Click += new System.EventHandler(this.mDasamsa_Click);
			// 
			// mDwadasamsa
			// 
			this.mDwadasamsa.Index = 16;
			this.mDwadasamsa.Text = "D-12: Dwadasamsa";
			this.mDwadasamsa.Click += new System.EventHandler(this.mDwadasamsa_Click);
			// 
			// mShodasamsa
			// 
			this.mShodasamsa.Index = 17;
			this.mShodasamsa.Text = "D-16: Shodasamsa";
			this.mShodasamsa.Click += new System.EventHandler(this.mShodasamsa_Click);
			// 
			// mVimsamsa
			// 
			this.mVimsamsa.Index = 18;
			this.mVimsamsa.Text = "D-20: Vimsamsa";
			this.mVimsamsa.Click += new System.EventHandler(this.mVimsamsa_Click);
			// 
			// mChaturvimsamsa
			// 
			this.mChaturvimsamsa.Index = 19;
			this.mChaturvimsamsa.Text = "D-24: Chaturvimsamsa";
			this.mChaturvimsamsa.Click += new System.EventHandler(this.mChaturvimsamsa_Click);
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 20;
			this.menuItem18.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mHoraParivritti,
																					   this.mHoraJagannath,
																					   this.mHoraKashinath,
																					   this.mDrekkanaParivrittitraya,
																					   this.mDrekkanaJagannath,
																					   this.mDrekkanaSomnath,
																					   this.mAshtamsaRaman,
																					   this.mRudramsaRath,
																					   this.mRudramsaRaman,
																					   this.mTrimsamsaParivritti,
																					   this.mTrimsamsaSimple});
			this.menuItem18.Text = "Other Vargas";
			// 
			// mHoraParivritti
			// 
			this.mHoraParivritti.Index = 0;
			this.mHoraParivritti.Text = "D-2: Parivritti Dvaya Hora";
			this.mHoraParivritti.Click += new System.EventHandler(this.mHoraParivritti_Click);
			// 
			// mHoraJagannath
			// 
			this.mHoraJagannath.Enabled = false;
			this.mHoraJagannath.Index = 1;
			this.mHoraJagannath.Text = "D-2: Jagannath Hora";
			this.mHoraJagannath.Click += new System.EventHandler(this.mHoraJagannath_Click);
			// 
			// mHoraKashinath
			// 
			this.mHoraKashinath.Index = 2;
			this.mHoraKashinath.Text = "D-2: Kashinath Hora";
			this.mHoraKashinath.Click += new System.EventHandler(this.mHoraKashinath_Click);
			// 
			// mDrekkanaParivrittitraya
			// 
			this.mDrekkanaParivrittitraya.Index = 3;
			this.mDrekkanaParivrittitraya.Text = "D-3: Parivritti Traya Drekkana";
			this.mDrekkanaParivrittitraya.Click += new System.EventHandler(this.mDrekkanaParivrittitraya_Click);
			// 
			// mDrekkanaJagannath
			// 
			this.mDrekkanaJagannath.Index = 4;
			this.mDrekkanaJagannath.Text = "D-3: Jagannath Drekkana";
			this.mDrekkanaJagannath.Click += new System.EventHandler(this.mDrekkanaJagannath_Click);
			// 
			// mDrekkanaSomnath
			// 
			this.mDrekkanaSomnath.Index = 5;
			this.mDrekkanaSomnath.Text = "D-3: Somnath Drekkana";
			this.mDrekkanaSomnath.Click += new System.EventHandler(this.mDrekkanaSomnath_Click);
			// 
			// mAshtamsaRaman
			// 
			this.mAshtamsaRaman.Index = 6;
			this.mAshtamsaRaman.Text = "D-8: Raman Ashtamsa";
			this.mAshtamsaRaman.Click += new System.EventHandler(this.mAshtamsaRaman_Click);
			// 
			// mRudramsaRath
			// 
			this.mRudramsaRath.Index = 7;
			this.mRudramsaRath.Text = "D-11: Rath Rudramsa";
			this.mRudramsaRath.Click += new System.EventHandler(this.mRudramsaRath_Click);
			// 
			// mRudramsaRaman
			// 
			this.mRudramsaRaman.Index = 8;
			this.mRudramsaRaman.Text = "D-11: Raman Rudramsa";
			this.mRudramsaRaman.Click += new System.EventHandler(this.mRudramsaRaman_Click);
			// 
			// mTrimsamsaParivritti
			// 
			this.mTrimsamsaParivritti.Index = 9;
			this.mTrimsamsaParivritti.Text = "D-30: Parivritti Trimsa Trimsamasa";
			this.mTrimsamsaParivritti.Click += new System.EventHandler(this.mTrimsamsaParivritti_Click);
			// 
			// mTrimsamsaSimple
			// 
			this.mTrimsamsaSimple.Index = 10;
			this.mTrimsamsaSimple.Text = "D-30: Zodiacal Trimsamsa";
			this.mTrimsamsaSimple.Click += new System.EventHandler(this.mTrimsamsaSimple_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 21;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mNakshatramsa,
																					  this.mTrimsamsa,
																					  this.mKhavedamsa,
																					  this.mAkshavedamsa,
																					  this.mShashtyamsa,
																					  this.mAshtottaramsa,
																					  this.mNadiamsa,
																					  this.mNadiamsaCKN,
																					  this.menuItem10,
																					  this.mNavamsaDwadasamsa,
																					  this.mDwadasamsaDwadasamsa,
																					  this.mExtrapolate});
			this.menuItem6.Text = "Higher Vargas";
			// 
			// mNakshatramsa
			// 
			this.mNakshatramsa.Index = 0;
			this.mNakshatramsa.Text = "D-27: Nakshatramsa";
			this.mNakshatramsa.Click += new System.EventHandler(this.mNakshatramsa_Click);
			// 
			// mTrimsamsa
			// 
			this.mTrimsamsa.Index = 1;
			this.mTrimsamsa.Text = "D-30: Trimsamsa";
			this.mTrimsamsa.Click += new System.EventHandler(this.mTrimsamsa_Click);
			// 
			// mKhavedamsa
			// 
			this.mKhavedamsa.Index = 2;
			this.mKhavedamsa.Text = "D-40: Khavedamsa";
			this.mKhavedamsa.Click += new System.EventHandler(this.mKhavedamsa_Click);
			// 
			// mAkshavedamsa
			// 
			this.mAkshavedamsa.Index = 3;
			this.mAkshavedamsa.Text = "D-45: Akshavedamsa";
			this.mAkshavedamsa.Click += new System.EventHandler(this.mAkshavedamsa_Click_1);
			// 
			// mShashtyamsa
			// 
			this.mShashtyamsa.Index = 4;
			this.mShashtyamsa.Text = "D-60: Shashtyamsa";
			this.mShashtyamsa.Click += new System.EventHandler(this.mShashtyamsa_Click_1);
			// 
			// mAshtottaramsa
			// 
			this.mAshtottaramsa.Index = 5;
			this.mAshtottaramsa.Text = "D-108: Parivritti Ashtottaramsa";
			this.mAshtottaramsa.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// mNadiamsa
			// 
			this.mNadiamsa.Index = 6;
			this.mNadiamsa.Text = "D-150: Nadiamsa";
			this.mNadiamsa.Click += new System.EventHandler(this.mNadiamsa_Click);
			// 
			// mNadiamsaCKN
			// 
			this.mNadiamsaCKN.Index = 7;
			this.mNadiamsaCKN.Text = "D-150: Nadiamsa (Variable)";
			this.mNadiamsaCKN.Click += new System.EventHandler(this.mNadiamsaCKN_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 8;
			this.menuItem10.Text = "-";
			// 
			// mNavamsaDwadasamsa
			// 
			this.mNavamsaDwadasamsa.Index = 9;
			this.mNavamsaDwadasamsa.Text = "D-108: Navamsa Dwadasamsa";
			this.mNavamsaDwadasamsa.Click += new System.EventHandler(this.mNavamsaDwadasamsa_Click);
			// 
			// mDwadasamsaDwadasamsa
			// 
			this.mDwadasamsaDwadasamsa.Index = 10;
			this.mDwadasamsaDwadasamsa.Text = "D-144: Dwadasamsa Dwadasamsa";
			this.mDwadasamsaDwadasamsa.Click += new System.EventHandler(this.mDwadasamsaDwadasamsa_Click);
			// 
			// mExtrapolate
			// 
			this.mExtrapolate.Index = 11;
			this.mExtrapolate.Text = "Extrapolate Horoscope";
			this.mExtrapolate.Click += new System.EventHandler(this.mExtrapolate_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 22;
			this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.mRegularParivritti,
																					   this.mRegularFromHouse,
																					   this.mRegularSaptamsaBased,
																					   this.mRegularDasamsaBased,
																					   this.mRegularShashthamsaBased,
																					   this.menuItem12,
																					   this.mRegularTrikona,
																					   this.mRegularShodasamsaBased,
																					   this.mRegularVimsamsaBased,
																					   this.mRegularKendraChaturthamsa,
																					   this.mRegularNakshatramsaBased});
			this.menuItem11.Text = "Custom Varga Variations";
			// 
			// mRegularParivritti
			// 
			this.mRegularParivritti.Index = 0;
			this.mRegularParivritti.Text = "Regular: Parivritti";
			this.mRegularParivritti.Click += new System.EventHandler(this.mRegularParivritti_Click);
			// 
			// mRegularFromHouse
			// 
			this.mRegularFromHouse.Index = 1;
			this.mRegularFromHouse.Text = "Regular: 1: (d-12,d-60 based)";
			this.mRegularFromHouse.Click += new System.EventHandler(this.mRegularFromHouse_Click);
			// 
			// mRegularSaptamsaBased
			// 
			this.mRegularSaptamsaBased.Index = 2;
			this.mRegularSaptamsaBased.Text = "Regular: 1,7:  (d-7 based)";
			this.mRegularSaptamsaBased.Click += new System.EventHandler(this.mRegularSaptamsaBased_Click);
			// 
			// mRegularDasamsaBased
			// 
			this.mRegularDasamsaBased.Index = 3;
			this.mRegularDasamsaBased.Text = "Regular: 1,9: (d-10 based)";
			this.mRegularDasamsaBased.Click += new System.EventHandler(this.mRegularDasamsaBased_Click);
			// 
			// mRegularShashthamsaBased
			// 
			this.mRegularShashthamsaBased.Index = 4;
			this.mRegularShashthamsaBased.Text = "Regular: Ari,Lib:  (d-6, d-40 based)";
			this.mRegularShashthamsaBased.Click += new System.EventHandler(this.mRegularShashthamsaBased_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 5;
			this.menuItem12.Text = "Regular: Leo,Can: (d-24 based)";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// mRegularTrikona
			// 
			this.mRegularTrikona.Index = 6;
			this.mRegularTrikona.Text = "Trikona: 1,5,9: (d-3 based)";
			this.mRegularTrikona.Click += new System.EventHandler(this.mRegularTrikona_Click);
			// 
			// mRegularShodasamsaBased
			// 
			this.mRegularShodasamsaBased.Index = 7;
			this.mRegularShodasamsaBased.Text = "Trikona: Ari,Leo,Sag: (d-16, d-45 based)";
			this.mRegularShodasamsaBased.Click += new System.EventHandler(this.mRegularShodasamsaBased_Click);
			// 
			// mRegularVimsamsaBased
			// 
			this.mRegularVimsamsaBased.Index = 8;
			this.mRegularVimsamsaBased.Text = "Trikona: Ari,Sag,Leo: (d-8, d-20 based)";
			this.mRegularVimsamsaBased.Click += new System.EventHandler(this.mRegularVimsamsaBased_Click);
			// 
			// mRegularKendraChaturthamsa
			// 
			this.mRegularKendraChaturthamsa.Index = 9;
			this.mRegularKendraChaturthamsa.Text = "Kendra: 1,4,7,10: (d-4 based)";
			this.mRegularKendraChaturthamsa.Click += new System.EventHandler(this.mRegularKendraChaturthamsa_Click);
			// 
			// mRegularNakshatramsaBased
			// 
			this.mRegularNakshatramsaBased.Index = 10;
			this.mRegularNakshatramsaBased.Text = "Kendra: Ari,Can,Lib,Cap: (d-27 based)";
			this.mRegularNakshatramsaBased.Click += new System.EventHandler(this.mRegularNakshatramsaBased_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 23;
			this.menuItem3.Text = "-";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 24;
			this.menuItem4.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = -1;
			this.menuItem2.Text = "Change view 2";
			// 
			// DivisionalChart
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu;
			this.Font = new System.Drawing.Font("Times New Roman", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "DivisionalChart";
			this.Size = new System.Drawing.Size(504, 312);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DivisionalChart_DragEnter);
			this.Resize += new System.EventHandler(this.DivisionalChart_Resize);
			this.Load += new System.EventHandler(this.DivisionalChart_Load);
			this.DragLeave += new System.EventHandler(this.DivisionalChart_DragLeave);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DivisionalChart_Paint);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DivisionalChart_DragDrop);
			this.MouseLeave += new System.EventHandler(this.DivisionalChart_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DivisionalChart_MouseDown);

		}
		#endregion

		private void DivisionalChart_Load(object sender, System.EventArgs e)
		{
			this.AddViewsToContextMenu (this.contextMenu);
		}

		Point GetOffset (ZodiacHouse zh, int item) 
		{
			return dc.GetSmallItemOffset(zh, item);
		}
		private void AddItem (Graphics g, ZodiacHouse zh, int item, DivisionPosition dp, bool large)
		{
			String s = null;

			if (dp.type == BodyType.Name.GrahaArudha ||
				dp.type == BodyType.Name.Varnada)
				s = dp.otherString;
			else
				s = Body.toShortString(dp.name);

			AddItem (g, zh, item, dp, large, s);
		}

		Pen pn_black = new Pen(Color.Black, (float)0.01);
		Brush b = new SolidBrush(Color.Black);
	
		Font fBase = new Font(
			MhoraGlobalOptions.Instance.VargaFont.FontFamily, 
			MhoraGlobalOptions.Instance.VargaFont.SizeInPoints);

		private void AddItem (Graphics g, ZodiacHouse zh, int item, DivisionPosition dp, bool large, string s)
		{	
			Point p;
			Font f;

			if (large) 
			{
				p = dc.GetItemOffset(zh, item);
				f = fBase;
				if (dp.type == BodyType.Name.Graha) 
				{
					BodyPosition bp = h.getPosition(dp.name);
					if (bp.speed_longitude < 0.0 && bp.name != Body.Name.Rahu && bp.name != Body.Name.Ketu)
					f = new Font (fBase.Name, fBase.Size, FontStyle.Underline);
				} 
				else if (dp.name == Body.Name.Lagna)
				{
					f = new Font (fBase.Name, fBase.Size, FontStyle.Bold);
				}

			}
			else
			{
				FontStyle fs = FontStyle.Regular;
				if (dp.type == BodyType.Name.BhavaArudhaSecondary)
					fs = FontStyle.Italic;

				p = dc.GetSmallItemOffset(zh, item);
				f = new Font(fBase.Name, fBase.SizeInPoints-1, fs);
			}

			if (dp.type == BodyType.Name.GrahaArudha)
			{
				f = new Font(fBase.Name, fBase.SizeInPoints-1);
			}

			switch (dp.type) 
			{
				case BodyType.Name.Graha: 
				case BodyType.Name.GrahaArudha:
					b = new SolidBrush(MhoraGlobalOptions.Instance.VargaGrahaColor); 
					break;
				case BodyType.Name.SpecialLagna: 
					b = new SolidBrush(MhoraGlobalOptions.Instance.VargaSpecialLagnaColor); 
					break;
				case BodyType.Name.BhavaArudha:
				case BodyType.Name.Varnada:
				case BodyType.Name.BhavaArudhaSecondary: 
					b = new SolidBrush(MhoraGlobalOptions.Instance.VargaSecondaryColor); 
					break;
				case BodyType.Name.Lagna: 
					b = new SolidBrush(MhoraGlobalOptions.Instance.VargaLagnaColor); 
					break;
			}

			Font f2 = null;
			if (this.bInnerMode == true)
				f2 = new Font(f.FontFamily, f.SizeInPoints+1, f.Style);
			else
				f2 = f;
			//SizeF sf = g.MeasureString (s, f, this.Width);
			//g.FillRectangle(r, p.X, p.Y, sf.Width, sf.Height);
			g.DrawString(s, f2, b, p.X, p.Y);

			if (options.Varga.MultipleDivisions.Length == 1 &&
				options.Varga.MultipleDivisions[0].Varga == Basics.DivisionType.Rasi &&
				this.PrintMode == false &&
				( dp.type == BodyType.Name.Graha ||
				  dp.type == BodyType.Name.Lagna ) )
			{
				Point pLon = dc.GetDegreeOffset(h.getPosition(dp.name).longitude);
				Pen pn = new Pen(MhoraGlobalOptions.Instance.getBinduColor(dp.name), (float)0.01);
				Brush br = new SolidBrush(MhoraGlobalOptions.Instance.getBinduColor(dp.name));
				g.FillEllipse(br, pLon.X-1, pLon.Y-1, 4, 4);
				//g.DrawEllipse(pn, pLon.X-1, pLon.Y-1, 2, 2);
				g.DrawEllipse(new Pen(Color.Gray), pLon.X-1, pLon.Y-1,4,4);
			}

		}

		

		private void PaintObjects(Graphics g)
		{
			switch (this.options.ViewStyle)
			{
				case UserOptions.EViewStyle.Panchanga:
				case UserOptions.EViewStyle.Normal:
				case UserOptions.EViewStyle.Varnada:
					this.PaintNormalView(g);
					break;
				case UserOptions.EViewStyle.DualGrahaArudha:
					this.PaintDualGrahaArudhasView(g);
					break;
				case UserOptions.EViewStyle.CharaKarakas7:
				case UserOptions.EViewStyle.CharaKarakas8:
					this.PaintCharaKarakas(g);
					break;
			}
		}

		string[] karakas_s = new string[]
		{ "AK", "AmK", "BK", "MK", "PiK", "PuK", "JK", "DK"	};
		string[] karakas_s7 = new string[]
		{ "AK", "AmK", "BK", "MK", "PiK", "JK", "DK"	};


		private void PaintCharaKarakas (Graphics g)
		{
			int[] nItems = new int[13]{0,0,0,0,0,0,0,0,0,0,0,0,0};
			ArrayList al = new ArrayList();

			// number of karakas to display
			int max = 0;
			if (options.ViewStyle == UserOptions.EViewStyle.CharaKarakas7)
				max = (int)mhora.Body.Name.Saturn;
			else
				max = (int)mhora.Body.Name.Rahu;

			// determine karakas
			for (int i=(int)mhora.Body.Name.Sun; i<=max; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				BodyPosition bp = h.getPosition(b);
				BodyKarakaComparer bkc = new BodyKarakaComparer(bp);
				al.Add (bkc);
			}
			al.Sort();

			int[] kindex = new int[max+1];
			for (int i=0; i<=max; i++)
			{
				BodyPosition bp = ((BodyKarakaComparer)al[i]).GetPosition;
				kindex[(int)bp.name] = i;
			}


			// display bodies
			for (int i=0; i<=max; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				DivisionPosition dp = (DivisionPosition)this.div_pos[i];
				ZodiacHouse zh = dp.zodiac_house;
				int j = (int)zh.value;
				nItems[j]++;
				if (options.ViewStyle == UserOptions.EViewStyle.CharaKarakas7)
					AddItem (g, zh, nItems[j], dp, true, karakas_s7[kindex[i]]);
				else
					AddItem (g, zh, nItems[j], dp, true, karakas_s[kindex[i]]);
					
			}

			DivisionPosition dp2 = (DivisionPosition)this.div_pos[(int)mhora.Body.Name.Lagna];
			ZodiacHouse zh2 = dp2.zodiac_house;
			int j2 = (int)zh2.value;
			nItems[j2]++;
			AddItem (g, zh2, nItems[j2], dp2, true);
		}
		private void PaintDualGrahaArudhasView (Graphics g)
		{
			int[] nItems = new int[13]{0,0,0,0,0,0,0,0,0,0,0,0,0};

			DivisionPosition dpo;
			int i;
			dpo = h.getPosition(Body.Name.Lagna).toDivisionPosition(options.Varga);
			i = (int)dpo.zodiac_house.value;
			nItems[i]++;
			AddItem(g, dpo.zodiac_house, nItems[i], dpo, true);


			foreach (DivisionPosition dp in this.graha_arudha_pos)
			{
				i = (int)dp.zodiac_house.value;
				nItems[i]++;
				AddItem (g, dp.zodiac_house, nItems[i], dp, true);
			}
		}

		private void PaintSAV (Graphics g)
		{
			if (true == this.PrintMode)
				return;

			if (false == MhoraGlobalOptions.Instance.VargaShowSAVVarga &&
				false == MhoraGlobalOptions.Instance.VargaShowSAVRasi)
				return;

			ZodiacHouse zh = new ZodiacHouse(ZodiacHouse.Name.Ari);
			Brush b = new SolidBrush(MhoraGlobalOptions.Instance.VargaSAVColor);
			Font f = MhoraGlobalOptions.Instance.GeneralFont;
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zhi = zh.add(i);
				Point p = dc.GetSingleItemOffset(zhi);
				g.DrawString(this.sav_bindus[i-1].ToString(), f, b, p.X, p.Y);
			}
		}
		private void PaintNormalView(Graphics g) 
		{
			int[] nItems = new int[13]{0,0,0,0,0,0,0,0,0,0,0,0,0};
			Body.Name[] bItems = new Body.Name[10] 
			 {
				 Body.Name.Lagna, Body.Name.Sun, Body.Name.Moon,
				 Body.Name.Mars, Body.Name.Mercury, Body.Name.Jupiter,
				 Body.Name.Venus, Body.Name.Saturn, Body.Name.Rahu,
				 Body.Name.Ketu
			 };

#if DDD
			foreach (ZodiacHouse.Name _zh in ZodiacHouse.AllNames)
			{
				ZodiacHouse zh = new ZodiacHouse(_zh);
				for (int i=1; i<9; i++)
				{
					DivisionPosition dp = new DivisionPosition(Body.Name.Jupiter,
						BodyType.Name.Graha, zh, 0.0, 0.0);
					AddItem (g, zh, i, dp, true);
				}
				for (int i=1; i<=6; i++)
				{
					DivisionPosition dp = new DivisionPosition(Body.Name.A11, 
											BodyType.Name.BhavaArudha, zh, 0.0, 0.0);
					AddItem (g, zh, i, dp, false);
				}

			}
#endif

			foreach (DivisionPosition dp in div_pos) 
			{
				if (this.options.ViewStyle == UserOptions.EViewStyle.Panchanga &&
					dp.type != BodyType.Name.Graha)
					continue;

				if (dp.type != BodyType.Name.Graha && dp.type != BodyType.Name.Lagna)
					continue;
				ZodiacHouse zh = dp.zodiac_house;
				int i = (int)zh.value;
				nItems[i]++;
				AddItem (g, zh, nItems[i], dp, true);
			}


			if (this.options.ViewStyle == UserOptions.EViewStyle.Panchanga)
				return;
			
			foreach (DivisionPosition dp in div_pos) 
			{
				if (dp.type != BodyType.Name.SpecialLagna)
					continue;
				ZodiacHouse zh = dp.zodiac_house;
				int i = (int)zh.value;
				nItems[i]++;
				AddItem (g, zh, nItems[i], dp, true);
			}
			for (int k=1; k<13; k++)
				nItems[k] = 0;

			ArrayList secondary_pos = null;

			if (this.options.ViewStyle == UserOptions.EViewStyle.Normal)
				secondary_pos = arudha_pos;
			else
				secondary_pos = varnada_pos;

			foreach (DivisionPosition dp in secondary_pos) 
			{
				ZodiacHouse zh = dp.zodiac_house;
				int i = (int)zh.value;
				nItems[i]++;
				AddItem (g, zh, nItems[i], dp, false);
			}	

		}

		private void DivisionalChart_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			DrawChart (g);	
		}

		public void DrawChart (Graphics g)
		{
			DrawChart(g, this.Width, this.Height);
		}
		public void DrawChart (Graphics g, int width, int height)
		{
			DrawChart(g, width, height, false);
		}
		public void DrawChart (Graphics g, int width, int height, bool bDrawInner)
		{
			Font f = MhoraGlobalOptions.Instance.VargaFont;
			//this.BackColor = System.Drawing.Color.White;
			if (width == 0 || height == 0)
				return;


			int xw = dc.GetLength();
			int yw = dc.GetLength();

			int off = 5;
			
			if (this.bInnerMode)
				off = 0;

			int m_wh = Math.Min(height, width) - (2 * off)-off;
			float scale_x = ((float)width - (2 * off)) / (float) xw;
			float scale_y = ((float)height - (2 * off))  / (float) yw;
			float scale = (float)m_wh / (float)xw;

			if (false == PrintMode && false == bDrawInner)
				g.Clear (this.BackColor);

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.TranslateTransform (off, off);
			if (MhoraGlobalOptions.Instance.VargaChartIsSquare)
			{
				scale_x = scale;
				scale_y = scale;
			}

			g.ScaleTransform(scale_x,  scale_y);
			
			if (false == PrintMode)
			{
				Brush bg = new SolidBrush(MhoraGlobalOptions.Instance.VargaBackgroundColor);
				g.FillRectangle (bg, 0, 0, xw, yw);
			}
			dc.DrawOutline(g);
			if (innerControl != null)
			{
				Point ptInner = dc.GetInnerSquareOffset();
				int length = dc.GetLength() - ptInner.X*2;
				innerControl.Left = (int)(ptInner.X* scale_x)+off;
				innerControl.Top = (int)(ptInner.X * scale_y)+off;
				innerControl.Width = (int)(length * scale_x);
				innerControl.Height = (int)(length * scale_y);
				innerControl.Anchor = AnchorStyles.Left;
			}

			PaintSAV (g);
			
			PaintObjects(g);

			string s_dtype = Basics.numPartsInDivisionString(options.Varga);
				//string.Format("D-{0}", Basics.numPartsInDivision(options.Varga));
			SizeF hint = g.MeasureString(s_dtype, f);
			g.DrawString(s_dtype, f, Brushes.Black, xw*2/4-hint.Width/2, yw*2/4-hint.Height/2);

			s_dtype = Basics.variationNameOfDivision(options.Varga);
			hint = g.MeasureString(s_dtype, f);
			g.DrawString(s_dtype, f, Brushes.Black, xw*2/4-hint.Width/2, yw*2/4-f.Height-hint.Height/2);

			if (this.options.ChartStyle == UserOptions.EChartStyle.SouthIndian &&
				true == MhoraGlobalOptions.Instance.VargaShowDob && 
				false == this.PrintMode && false == bDrawInner)
			{
				string tob = h.info.tob.ToString();
				hint = g.MeasureString(tob, f);
				g.DrawString(tob, f, System.Drawing.Brushes.Black, xw*2/4-hint.Width/2, (float)(yw*2/4-hint.Height/2+f.Height*1.5));

				string latlon = h.info.lat.ToString() + " " + h.info.lon.ToString();
				hint = g.MeasureString(latlon, f);
				g.DrawString(latlon, f, System.Drawing.Brushes.Black, xw*2/4-hint.Width/2, (float)(yw*2/4-hint.Height/2+f.Height*2.5));

				hint = g.MeasureString(h.info.name, f);
				g.DrawString(h.info.name, f, System.Drawing.Brushes.Black, xw*2/4-hint.Width/2, (float)(yw*2/4-hint.Height/2-f.Height*1.5));
			}
			

			/*
			ZodiacHouse zh = new ZodiacHouse(ZodiacHouse.Name.Sco);
			for (int i=1; i<9; i++)
				AddItem(g, zh, i, new D, true);

			for (int i=1; i<=12; i++) 
			{
				ZodiacHouse zh = new ZodiacHouse((ZodiacHouse.Name)i);
				AddItem (g, zh, 9, zh.value.ToString());
			}
			*/
			this.Update();			

		}

		private void DivisionalChart_Resize(object sender, System.EventArgs e)
		{
			this.Invalidate();
		}

		private void SetChartStyle (UserOptions.EChartStyle cs)
		{
			switch (cs)
			{
				case UserOptions.EChartStyle.EastIndian:
					dc = new EastIndianChart();
					return;
				case UserOptions.EChartStyle.SouthIndian:
				default:
					dc = new SouthIndianChart();
					return;
			}
		}
		public object SetOptions (Object o)
		{
			
			UserOptions uo = (UserOptions) o;
			if (uo.ChartStyle != this.options.ChartStyle)
				this.SetChartStyle(uo.ChartStyle);
			this.options = uo;
			this.OnRecalculate(h);

			return this.options.Clone();
		}
		private void mOptions_Click(object sender, System.EventArgs e)
		{
			Form f = new MhoraOptions(this.options.Clone(), new ApplyOptions(SetOptions));
			f.ShowDialog();
		}

		private void CalculateBindus()
		{
			if (MhoraGlobalOptions.Instance.VargaShowSAVVarga)
				sav_bindus = new Ashtakavarga(h, options.Varga).getSav();
			else if (MhoraGlobalOptions.Instance.VargaShowSAVRasi)
				sav_bindus = new Ashtakavarga(h, new Division(Basics.DivisionType.Rasi)).getSav();
		}
		private void OnRedisplay (Object o)
		{
			this.SetChartStyle(MhoraGlobalOptions.Instance.VargaStyle);
			this.options.ChartStyle = MhoraGlobalOptions.Instance.VargaStyle;
			fBase = new Font(
				MhoraGlobalOptions.Instance.VargaFont.FontFamily, 
				MhoraGlobalOptions.Instance.VargaFont.SizeInPoints);
			this.CalculateBindus();
			this.Invalidate();
		}

		private void OnRecalculate (Object o)
		{
			div_pos = h.CalculateDivisionPositions(options.Varga);
			arudha_pos = h.CalculateArudhaDivisionPositions(options.Varga);
			varnada_pos = h.CalculateVarnadaDivisionPositions(options.Varga);
			graha_arudha_pos = h.CalculateGrahaArudhaDivisionPositions(options.Varga);
			this.CalculateBindus();
			this.Invalidate();
		}

		private void DivisionalChart_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{

			if (this.options.ShowInner == true &&
				e.Data.GetDataPresent(typeof(mhora.DivisionalChart)))
			{
				Division div = Division.CopyFromClipboard();
				if (null == div) return;
				if (this.innerControl == null)
					this.AddInnerControl();
				innerControl.options.Varga = div;
				innerControl.OnRecalculate(innerControl.h);
				this.Invalidate();
			}


#if DND
			if (MhoraGlobalOptions.Reference is DivisionalChart) 
			{	
				DivisionalChart dc = (DivisionalChart)MhoraGlobalOptions.Reference;
				h.Changed -= new EvtChanged(OnRecalculate);
				dc.ControlHoroscope.Changed += new EvtChanged(OnRecalculate);
				h = dc.ControlHoroscope;
				DivisionalChart.UserOptions uo = (UserOptions)this.options.Clone();
				uo.Division = dc.options.Division;
				this.SetOptions(uo);
				this.OnRecalculate(h);
			} 
#endif
		}

		private void DivisionalChart_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart))) 
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void mRasi_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Rasi);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mNavamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Navamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mBhava_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaPada);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mBhavaEqual_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaEqual);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mBhavaSripati_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaSripati);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mBhavaKoch_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaKoch);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mBhavaPlacidus_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaPlacidus);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void menuBhavaAlcabitus_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaAlcabitus);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void menuBhavaCampanus_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaCampanus);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void menuBhavaRegiomontanus_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaRegiomontanus);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void menuBhavaAxial_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.BhavaAxial);			
			this.OnRecalculate(h);
			this.Invalidate();		
		}
		private void mDrekkanaParasara_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.DrekkanaParasara);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mChaturamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Chaturthamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mPanchamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Panchamsa);
			this.OnRecalculate(h);
			this.Invalidate();		
		}

		private void mShashtamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Shashthamsa);
			this.OnRecalculate(h);
			this.Invalidate();		
		}

		private void mSaptamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Saptamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}
		private void mAshtamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga  = new Division(Basics.DivisionType.Ashtamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mAshtamsaRaman_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.AshtamsaRaman);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mDasamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Dasamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mDwadasamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Dwadasamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mShodasamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Shodasamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mVimsamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Vimsamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mChaturvimsamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Chaturvimsamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mNakshatramsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Nakshatramsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mTrimsamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Trimsamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mKhavedamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Khavedamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mDrekkanaJagannath_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.DrekkanaJagannath);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mDrekkanaSomnath_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.DrekkanaSomnath);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mDrekkanaParivrittitraya_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.DrekkanaParivrittitraya);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mHoraKashinath_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.HoraKashinath);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mHoraParivritti_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.HoraParivrittiDwaya);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mHoraParasara_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.HoraParasara);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mHoraJagannath_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.HoraJagannath);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mTrimsamsaParivritti_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.TrimsamsaParivritti);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mTrimsamsaSimple_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.TrimsamsaSimple);
			this.OnRecalculate(h);
			this.Invalidate();
		}


		private void mLagnaChange_Click(object sender, System.EventArgs e)
		{
			VargaRectificationForm vf = new VargaRectificationForm(h, this, this.options.Varga);
			vf.Show();
		}

		private void mExtrapolate_Click(object sender, System.EventArgs e)
		{

			foreach (BodyPosition bp in h.positionList)
			{
				DivisionPosition dp = bp.toDivisionPosition(this.options.Varga);
				Longitude lLower = new Longitude(dp.cusp_lower);
				Longitude lOffset = bp.longitude.sub(lLower);
				Longitude lRange = new Longitude(dp.cusp_higher).sub(lLower);
				Trace.Assert(lOffset.value <= lRange.value, "Extrapolation internal error: Slice smaller than range. Weird.");

				double newOffset = (lOffset.value / lRange.value)*30.0;
				double newBase = ((double)((int)dp.zodiac_house.value-1))*30.0;
				bp.longitude = new Longitude(newOffset + newBase);
			}

			h.OnlySignalChanged();
			
		}
		private void mAkshavedamsa_Click_1(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Akshavedamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mShashtyamsa_Click_1(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Shashtyamsa);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void mRudramsaRath_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Rudramsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mRudramsaRaman_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.RudramsaRaman);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mNadiamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Nadiamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mNadiamsaCKN_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.NadiamsaCKN);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void mNavamsaDwadasamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.NavamsaDwadasamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mDwadasamsaDwadasamsa_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.DwadasamsaDwadasamsa);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			this.options.Varga = new Division(Basics.DivisionType.Ashtottaramsa);
			this.OnRecalculate(h);
			this.Invalidate();
		
		}
		private void mRegularParivritti_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericParivritti, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
	
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mRegularFromHouse_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericDwadasamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);

			this.OnRecalculate(h);
			this.Invalidate();		
		}


		private void mRegularTrikona_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericDrekkana, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();
		}
		protected override void copyToClipboard ()
		{
			Graphics displayGraphics = this.CreateGraphics();
			int size = Math.Min(this.Width, this.Height);
			Bitmap bmpBuffer = new Bitmap(size, size, displayGraphics);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			DrawChart (imageGraphics);			
			displayGraphics.Dispose();
			Clipboard.SetDataObject(bmpBuffer, true);
			imageGraphics.Dispose();
		}

		private void DivisionalChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.DoDragDrop(this, DragDropEffects.Copy);
				Clipboard.SetDataObject("");
			}
			else
			{
				this.contextMenu.Show(this, new Point(e.X, e.Y));;
			}

		}

		private void mViewNormal_Click(object sender, System.EventArgs e)
		{
			this.options.ViewStyle = UserOptions.EViewStyle.Normal;
			this.Invalidate();
		}

		private void mViewDualGrahaArudha_Click(object sender, System.EventArgs e)
		{
			this.options.ViewStyle = UserOptions.EViewStyle.DualGrahaArudha;
			this.Invalidate();
		}

		private void mViewCharaKarakas_Click(object sender, System.EventArgs e)
		{
			this.options.ViewStyle = UserOptions.EViewStyle.CharaKarakas8;
			this.Invalidate();
		}

		private void mViewCharaKarakas7_Click(object sender, System.EventArgs e)
		{
			this.options.ViewStyle = UserOptions.EViewStyle.CharaKarakas7;
			this.Invalidate();		
		}

		private void mViewVarnada_Click(object sender, System.EventArgs e)
		{
			this.options.ViewStyle = UserOptions.EViewStyle.Varnada;
			this.Invalidate();
		}

		private void mRegularKendraChaturthamsa_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericChaturthamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mRegularSaptamsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericSaptamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mRegularDasamsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericDasamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();
		}

		private void mRegularShashthamsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericShashthamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();		
		}

		private void mRegularShodasamsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericShodasamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();		
		}

		private void mRegularVimsamsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericVimsamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();			
		}

		private void mRegularNakshatramsaBased_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericNakshatramsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();					
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			Division.SingleDivision single = new Division.SingleDivision(Basics.DivisionType.GenericChaturvimsamsa, 
				Basics.numPartsInDivision(options.Varga));
			this.options.Varga = new Division(single);
			this.OnRecalculate(h);
			this.Invalidate();				
		}

		private void DivisionalChart_MouseLeave(object sender, System.EventArgs e)
		{
			//if (e
			//Division.CopyToClipboard(this.options.Varga);
			//this.DoDragDrop(this, DragDropEffects.Copy);

		}

		private void DivisionalChart_DragLeave(object sender, System.EventArgs e)
		{
			Division.CopyToClipboard(this.options.Varga);
		}


	}


	public interface IDrawChart
	{
		Point GetDegreeOffset (Longitude l);
		Point GetSingleItemOffset (ZodiacHouse zh);
		Point GetItemOffset (ZodiacHouse zh, int n);
		Point GetSmallItemOffset (ZodiacHouse zh, int n);
		Point GetInnerSquareOffset ();
		void DrawOutline (Graphics g);
		int GetLength ();
	}


	public class EastIndianChart: IDrawChart
	{
		const int xw = 200;
		const int yw = 200;
		Pen pn_black;

		public EastIndianChart()
		{
			pn_black = new Pen(Color.Black, (float)0.1);
		}
		
		public int GetLength ()
		{
			return xw;
		}
		public void DrawOutline (Graphics g)
		{
			g.DrawLine(pn_black, xw/3, 0, xw/3, yw);
			g.DrawLine(pn_black, xw*2/3, 0, xw*2/3, yw);
			g.DrawLine(pn_black, 0, yw/3, xw, yw/3);
			g.DrawLine(pn_black, 0, yw*2/3, xw, yw*2/3);
			g.DrawLine(pn_black, xw/3, yw/3, 0, 0);
			g.DrawLine(pn_black, xw*2/3, yw/3, xw, 0);
			g.DrawLine(pn_black, xw/3, yw*2/3, 0, yw);
			g.DrawLine(pn_black, xw*2/3, yw*2/3, xw, yw);
		}
		public Point GetInnerSquareOffset ()
		{
			return new Point(xw/3, yw/3);
		}

		public Point GetZhouseOffset(ZodiacHouse zh)
		{
			int iOff = xw/3;
			switch (zh.value)
			{
				case ZodiacHouse.Name.Ari: return new Point (iOff*2, 0);
				case ZodiacHouse.Name.Tau: return new Point (iOff,0);
				case ZodiacHouse.Name.Gem: return new Point (0,0);
				case ZodiacHouse.Name.Can: return new Point (0, iOff);
				case ZodiacHouse.Name.Leo: return new Point (0, iOff*2);
				case ZodiacHouse.Name.Vir: return new Point (0, iOff*3);
				case ZodiacHouse.Name.Lib: return new Point (iOff, iOff*3);
				case ZodiacHouse.Name.Sco: return new Point (iOff*2, iOff*3);
				case ZodiacHouse.Name.Sag: return new Point (iOff*3, iOff*3);
				case ZodiacHouse.Name.Cap: return new Point (iOff*3, iOff*2);
				case ZodiacHouse.Name.Aqu: return new Point (iOff*3, iOff);
				case ZodiacHouse.Name.Pis: return new Point (iOff*3, 0);
			}
			return new Point (0,0);
		}
		public Point GetDegreeOffset (Longitude l)
		{
			ZodiacHouse.Name zh = l.toZodiacHouse().value;
			double dOffset = l.toZodiacHouseOffset();
			int iOff = (int)((dOffset/30.0)*(xw/3));
			Point pBase = this.GetZhouseOffset(l.toZodiacHouse());
			switch (zh)
			{
				case ZodiacHouse.Name.Pis:
				case ZodiacHouse.Name.Ari:	
				case ZodiacHouse.Name.Tau:
					pBase.X -= iOff; break;
				case ZodiacHouse.Name.Gem:
				case ZodiacHouse.Name.Can:
				case ZodiacHouse.Name.Leo:
					pBase.Y += iOff; break;
				case ZodiacHouse.Name.Vir:
				case ZodiacHouse.Name.Lib:
				case ZodiacHouse.Name.Sco:
					pBase.X += iOff; break;
				case ZodiacHouse.Name.Sag:
				case ZodiacHouse.Name.Cap:
				case ZodiacHouse.Name.Aqu:
					pBase.Y -= iOff; break;
			}
			return pBase;
		}

		public Point GetGemOffset (int n)
		{
			int wi = (xw/3)/4;
			int yi = (yw/3)/6;
			switch (n)
			{
				case 4:	return new Point(0, yi*4);
				case 3: return new Point(wi*1, yi*4);
				case 8: return new Point(wi*2, yi*4);
				case 1: return new Point(0, yi*3);
				case 5: return new Point(wi*1,yi*3);
				case 2: return new Point(0, yi*2);
				case 6: return new Point(wi*1-4, yi*2);
				case 7: return new Point(0, yi*1);
			}
			return this.GetGemOffset(1);
		}
		public Point GetSmallGemOffset (int n)
		{
			int wi = (xw/3)/5;
			int yi = (xw/3)/6;
			switch (n)
			{
				case 4: return new Point (0, yi*5);
				case 1: return new Point (wi*1, yi*5);
				case 3: return new Point (wi*2, yi*5);
				case 2: return new Point (wi*3, yi*5);
				case 5: return new Point (wi*4, yi*5);

			}
			return new Point (100,100);
		}
		public Point GetSingleGemOffset ()
		{
			return new Point ((xw/3)/4, (xw/3)*2/3);
		}
		public Point GetSingleItemOffset (ZodiacHouse zh)
		{
			switch (zh.value)
			{
				case ZodiacHouse.Name.Ari: return new Point(90, 0);
				case ZodiacHouse.Name.Can: return new Point(5, 90);
				case ZodiacHouse.Name.Lib: return new Point(90,185);
				case ZodiacHouse.Name.Cap: return new Point(180, 90);
				default:
					Point pret = this.GetSingleGemOffset();
					return FromGemOffset (zh, pret);
			}
		}
		public Point GetItemOffset (ZodiacHouse zh, int n)
		{
			Point pret = this.GetGemOffset(n);
			return FromGemOffset (zh, pret);
		}
		public Point FromGemOffset (ZodiacHouse zh, Point pret)
		{
			int wi = (xw/3)/4;
			int yi = (yw/3)/6;
			switch (zh.value)
			{
				case ZodiacHouse.Name.Gem:
					return pret;
				case ZodiacHouse.Name.Aqu:
					pret.X = xw - pret.X - wi;
					return pret;
				case ZodiacHouse.Name.Leo:
					pret.Y = yw - pret.Y - yi;
					return pret;
				case ZodiacHouse.Name.Sag:
					pret.X = xw - pret.X - wi;
					pret.Y = yw - pret.Y - yi;
					return pret;
				case ZodiacHouse.Name.Pis:
					pret.X += (xw*2/3);
					pret.Y = (yw/3)-pret.Y - yi;
					return pret;
				case ZodiacHouse.Name.Tau:
					pret.X = (xw/3)-pret.X - wi;
					pret.Y = (yw/3)-pret.Y - yi;
					return pret;
				case ZodiacHouse.Name.Vir:
					pret.X = (xw/3)-pret.X - wi;
					pret.Y += (yw*2/3);
					return pret;
				case ZodiacHouse.Name.Sco:
					pret.X += (xw*2/3);
					pret.Y += (yw*2/3);
					return pret;
				case ZodiacHouse.Name.Ari:
					pret.X += xw/3;
					return pret;
				case ZodiacHouse.Name.Can:
					pret.Y += yw/3;
					return pret;
				case ZodiacHouse.Name.Lib:
					pret.X += xw/3;
					pret.Y += yw*2/3;
					return pret;
				case ZodiacHouse.Name.Cap:
					pret.X += xw*2/3;
					pret.Y += yw/3;
					return pret;

			}
			return new Point (100,100);
		}
		public Point GetSmallItemOffset (ZodiacHouse zh, int n)
		{
			int wi = (xw/3)/5;
			//int yi = (xw/3)/6;
			Point pret;
			switch (zh.value)
			{
				case ZodiacHouse.Name.Gem:
					return this.GetSmallGemOffset(n);
				case ZodiacHouse.Name.Tau:
					pret = this.GetSmallGemOffset(n);
					pret.Y = 0;
					pret.X = (xw/3) - pret.X - wi;
					return pret;
				case ZodiacHouse.Name.Pis:
					pret = this.GetSmallGemOffset(n);
					pret.Y = 0;
					pret.X += (xw*2/3);
					return pret;
				case ZodiacHouse.Name.Aqu:
					pret = this.GetSmallGemOffset(n);
					pret.X = ((xw/3)-pret.X) + (xw*2/3) - wi;
					return pret;
				case ZodiacHouse.Name.Vir:
					pret = this.GetSmallGemOffset(n);
					pret.X = ((xw/3)-pret.X) - wi;
					pret.Y += (yw*2/3);
					return pret;
				case ZodiacHouse.Name.Sco:
					pret = this.GetSmallGemOffset(n);
					pret.X += (xw*2/3);
					pret.Y += (yw*2/3);
					return pret;
				case ZodiacHouse.Name.Sag:
					pret = this.GetSmallGemOffset(n);
					pret.Y = yw*2/3;
					pret.X = ((xw/3)-pret.X) + (xw*2/3) - wi;
					return pret;
				case ZodiacHouse.Name.Leo:
					pret = this.GetSmallGemOffset(n);
					pret.Y = yw*2/3;
					return pret;
				case ZodiacHouse.Name.Ari:
					pret = this.GetSmallGemOffset(n);
					pret.X += xw/3;
					return pret;
				case ZodiacHouse.Name.Can:
					pret = this.GetSmallGemOffset(n);
					pret.Y += yw/3;
					return pret;
				case ZodiacHouse.Name.Lib:
					pret = this.GetSmallGemOffset(n);
					pret.X += xw/3;
					pret.Y += yw*2/3;
					return pret;
				case ZodiacHouse.Name.Cap:
					pret = this.GetSmallGemOffset(n);
					pret.X += xw*2/3;
					pret.Y += yw/3;
					return pret;
			}
			return new Point (100, 100);
		}
	}


	public class SouthIndianChart: IDrawChart
	{
		const int xw = 200;
		const int yw = 200;
		const int xo = 0;
		const int yo = 0;

		const int hxs = 200;
		const int hys = 125;
		const int hsys = 75;
		Pen pn_black;

		public SouthIndianChart ()
		{
			pn_black = new Pen(Color.Black, (float)0.1);
		}

		public Point GetInnerSquareOffset()
		{
			return new Point(xw/4,yw/4);
		}
		public int GetLength ()
		{
			return xw;
		}

		public Point GetDegreeOffset (Longitude l)
		{
			ZodiacHouse.Name zh = l.toZodiacHouse().value;
			double dOffset = l.toZodiacHouseOffset();
			int iOff = (int)((dOffset/30.0)*(xw/4));
			Point pBase = this.GetZhouseOffset(l.toZodiacHouse());
			switch (zh)
			{
				case ZodiacHouse.Name.Ari:	
				case ZodiacHouse.Name.Tau:
				case ZodiacHouse.Name.Gem:
					pBase.X += iOff; break;
				case ZodiacHouse.Name.Can:
				case ZodiacHouse.Name.Leo:
				case ZodiacHouse.Name.Vir:
					pBase.X += xw/4;
					pBase.Y += iOff; 
					break;
				case ZodiacHouse.Name.Lib:
				case ZodiacHouse.Name.Sco:
				case ZodiacHouse.Name.Sag:
					pBase.X += xw/4-iOff; 
					pBase.Y += xw/4;
					break;
				case ZodiacHouse.Name.Cap:
				case ZodiacHouse.Name.Aqu:
				case ZodiacHouse.Name.Pis:
					pBase.Y += xw/4-iOff; break;
			}
			return pBase;
		}


		public void DrawOutline (Graphics g)
		{
			g.DrawLine (pn_black, xo, yo+0, xo+0, yo+yw);
			g.DrawLine (pn_black, xo, yo+0, xo+xw, yo+0);
			g.DrawLine (pn_black, xo+xw, yo+yw, xo+0, yo+yw);
			g.DrawLine (pn_black, xo+xw, yo+yw, xo+xw, yo+0);
			g.DrawLine (pn_black, xo, yo+yw/4, xo+xw, yo+yw/4);
			g.DrawLine (pn_black, xo, yo+yw*3/4, xo+xw, yo+yw*3/4);
			g.DrawLine (pn_black, xo+xw/4, yo, xo+xw/4, yo+yw);
			g.DrawLine (pn_black, xo+xw*3/4, yo, xo+xw*3/4, yo+yw);
			g.DrawLine (pn_black, xo+xw/2, yo, xo+xw/2, yo+yw/4);
			g.DrawLine (pn_black, xo+xw/2, yo+yw*3/4, xo+xw/2, yo+yw);
			g.DrawLine (pn_black, xo, yo+yw/2, xo+xw/4, yo+yw/2);
			g.DrawLine (pn_black, xo+xw*3/4, yo+yw/2, xo+xw, yo+yw/2);
		}
		public Point GetSingleItemOffset (ZodiacHouse zh)
		{
			Point p = GetZhouseOffset(zh);
			return new Point (p.X+15, p.Y+15);
		}
		public Point GetItemOffset (ZodiacHouse zh, int n)
		{
			Point p = GetZhouseOffset(zh);
			Point q = GetZhouseItemOffset (n);
			return new Point (p.X + q.X, p.Y + q.Y);
		}
		public Point GetSmallItemOffset (ZodiacHouse zh, int n)
		{
			Point p = GetZhouseOffset(zh);
			Point q = GetSmallZhouseItemOffset (n);
			return new Point (p.X + q.X, p.Y + q.Y);
		}
		private Point GetSmallZhouseItemOffset (int n)
		{
			if (n >= 7)
			{
				Debug.WriteLine("South Indian Chart (s) is too small for data");
				return new Point(0, 0);	
			}
			int[] item_map = new int[7]{0,6,2,3,4,2,1};
			n = item_map[n -1];

			int xiw = hxs / 4;
			int yiw = hsys / 6;

			int row = (int)Math.Floor((double)n / (double)3);
			int col = n - (row * 3);

			return new Point (xiw * row / 3, (hys/4)+yiw * col / 3);

		}
		private Point GetZhouseItemOffset (int n)
		{
			if (n >= 10) 
			{
				Debug.WriteLine("South Indian Chart is too small for data");
				return this.GetSmallZhouseItemOffset(n-10+1);
			}
			int[] item_map = new int[10]{0,5,7,9,3,1,2,4,6,8};
			n = item_map[n] - 1;

			int xiw = hxs / 4;
			int yiw = hys / 4;

			int col = (int)Math.Floor((double)n / (double)3);
			int row = n - (col * 3);

			return new Point (xiw * row / 3, yiw * col / 3);
		}
		private Point GetZhouseOffset (ZodiacHouse zh)
		{
			switch ((int)zh.value)
			{
				case 1: return new Point (xo + xw/4, yo + 0);
				case 2: return new Point (xo + xw*2/4, yo+0);
				case 3: return new Point (xo +xw*3/4, yo+0);
				case 4: return new Point (xo +xw*3/4, yo+yw/4);
				case 5: return new Point (xo +xw*3/4, yo+yw*2/4);
				case 6: return new Point (xo +xw*3/4, yo+yw*3/4);
				case 7: return new Point (xo +xw*2/4, yo+yw*3/4);
				case 8: return new Point (xo +xw/4, yo+yw*3/4);
				case 9: return new Point (xo +0, yo+yw*3/4);
				case 10: return new Point (xo +0, yo+yw*2/4);
				case 11: return new Point (xo +0, yo+yw*1/4);
				case 12: return new Point (xo +0, yo);
			}
			return new Point (0, 0);
		}			

	}

}
