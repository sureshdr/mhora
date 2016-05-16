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

namespace mhora
{
	/// <summary>
	/// Summary description for BasicCalculationsControl.
	/// </summary>
	public class BasicCalculationsControl : MhoraControl
	{
		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.ColumnHeader Body;
		private System.Windows.Forms.ColumnHeader Longitude;
		private System.Windows.Forms.ColumnHeader Nakshatra;
		private System.Windows.Forms.ColumnHeader Pada;
		private System.Windows.Forms.ContextMenu calculationsContextMenu;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.MenuItem menuBasicGrahas;
		private System.Windows.Forms.MenuItem menuSpecialTithis;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuSpecialTaras;
		

		private System.Windows.Forms.MenuItem menuChangeVarga;
		private UserOptions options;
		private System.Windows.Forms.MenuItem menuBhavaCusps;
		private System.Windows.Forms.MenuItem menuOtherLongitudes;
		private System.Windows.Forms.MenuItem menuMrityuLongitudes;
		private System.Windows.Forms.MenuItem menuAstroInfo;
		private System.Windows.Forms.MenuItem menuNakshatraAspects;
		private System.Windows.Forms.MenuItem menuCharaKarakas;
		private System.Windows.Forms.MenuItem menuSahamaLongitudes;
		private System.Windows.Forms.MenuItem menuAvasthas;
		private System.Windows.Forms.MenuItem menuCharaKarakas7;
		private System.Windows.Forms.MenuItem menu64Navamsa;
		private System.Windows.Forms.MenuItem menuCopyLon;
		private System.Windows.Forms.MenuItem menuNonLonBodies;

		enum ENakshatraLord
		{
			Vimsottari, Ashtottari, Yogini, Shodashottari, Dwadashottari, Panchottari,
			Shatabdika, ChaturashitiSama, DwisaptatiSama, ShatTrimshaSama
		};
		class UserOptions: ICloneable
		{
			private Division dtype;
			private ENakshatraLord mNakLord;

			[PGNotVisible]
			public Division DivisionType
			{
				get { return dtype; }
				set { dtype = value; }
			}

			[PGDisplayName("Division Type")]
			public Basics.DivisionType UIDivisionType
			{
				get { return dtype.MultipleDivisions[0].Varga; }
				set { dtype = new Division(value); }
			}

			public ENakshatraLord NakshatraLord
			{
				get { return mNakLord; }
				set { mNakLord = value; }
			}

			public Object Clone () 
			{
				UserOptions uo = new UserOptions();
				uo.dtype = this.dtype;
				uo.mNakLord = this.mNakLord;
				return uo;
			}
			public Object Copy (Object o)
			{
				UserOptions uo = (UserOptions) o;
				this.dtype = uo.dtype;
				this.mNakLord = uo.mNakLord;
				return this.Clone();
			}
		}

		private enum ViewType 
		{
			ViewBasicGrahas, ViewOtherLongitudes, ViewMrityuLongitudes,
			ViewSahamaLongitudes, ViewAvasthas,
			ViewSpecialTithis, ViewSpecialTaras, ViewBhavaCusps,
			ViewAstronomicalInfo, ViewNakshatraAspects, 
			ViewCharaKarakas, ViewCharaKarakas7, View64Navamsa,
			ViewNonLonBodies
		};
		ViewType vt;
		public BasicCalculationsControl(Horoscope _h)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			h = _h;
			this.vt = ViewType.ViewBasicGrahas;
			this.menuBasicGrahas.Checked = true;
			h.Changed += new EvtChanged(OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnRedisplay);
			this.options = new UserOptions();
			this.options.DivisionType = new Division(Basics.DivisionType.Rasi);

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
			this.mList = new System.Windows.Forms.ListView();
			this.Body = new System.Windows.Forms.ColumnHeader();
			this.Longitude = new System.Windows.Forms.ColumnHeader();
			this.Nakshatra = new System.Windows.Forms.ColumnHeader();
			this.Pada = new System.Windows.Forms.ColumnHeader();
			this.calculationsContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuChangeVarga = new System.Windows.Forms.MenuItem();
			this.menuCopyLon = new System.Windows.Forms.MenuItem();
			this.menuBasicGrahas = new System.Windows.Forms.MenuItem();
			this.menuOtherLongitudes = new System.Windows.Forms.MenuItem();
			this.menuMrityuLongitudes = new System.Windows.Forms.MenuItem();
			this.menuSahamaLongitudes = new System.Windows.Forms.MenuItem();
			this.menuNonLonBodies = new System.Windows.Forms.MenuItem();
			this.menuCharaKarakas = new System.Windows.Forms.MenuItem();
			this.menuCharaKarakas7 = new System.Windows.Forms.MenuItem();
			this.menu64Navamsa = new System.Windows.Forms.MenuItem();
			this.menuAstroInfo = new System.Windows.Forms.MenuItem();
			this.menuSpecialTithis = new System.Windows.Forms.MenuItem();
			this.menuSpecialTaras = new System.Windows.Forms.MenuItem();
			this.menuNakshatraAspects = new System.Windows.Forms.MenuItem();
			this.menuBhavaCusps = new System.Windows.Forms.MenuItem();
			this.menuAvasthas = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mList
			// 
			this.mList.AllowDrop = true;
			this.mList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					this.Body,
																					this.Longitude,
																					this.Nakshatra,
																					this.Pada});
			this.mList.ContextMenu = this.calculationsContextMenu;
			this.mList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mList.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(0, 0);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(496, 176);
			this.mList.TabIndex = 0;
			this.mList.View = System.Windows.Forms.View.Details;
			this.mList.MouseHover += new System.EventHandler(this.mList_MouseHover);
			this.mList.DragDrop += new System.Windows.Forms.DragEventHandler(this.mList_DragDrop);
			this.mList.DragEnter += new System.Windows.Forms.DragEventHandler(this.mList_DragEnter);
			this.mList.SelectedIndexChanged += new System.EventHandler(this.mList_SelectedIndexChanged);
			// 
			// Body
			// 
			this.Body.Text = "Body";
			this.Body.Width = 100;
			// 
			// Longitude
			// 
			this.Longitude.Text = "Longitude";
			this.Longitude.Width = 120;
			// 
			// Nakshatra
			// 
			this.Nakshatra.Text = "Nakshatra";
			this.Nakshatra.Width = 120;
			// 
			// Pada
			// 
			this.Pada.Text = "Pada";
			this.Pada.Width = 50;
			// 
			// calculationsContextMenu
			// 
			this.calculationsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									this.menuChangeVarga,
																									this.menuCopyLon,
																									this.menuBasicGrahas,
																									this.menuOtherLongitudes,
																									this.menuMrityuLongitudes,
																									this.menuSahamaLongitudes,
																									this.menuNonLonBodies,
																									this.menuCharaKarakas,
																									this.menuCharaKarakas7,
																									this.menu64Navamsa,
																									this.menuAstroInfo,
																									this.menuSpecialTithis,
																									this.menuSpecialTaras,
																									this.menuNakshatraAspects,
																									this.menuBhavaCusps,
																									this.menuAvasthas,
																									this.menuItem1,
																									this.menuItem2});
			this.calculationsContextMenu.Popup += new System.EventHandler(this.calculationsContextMenu_Popup);
			// 
			// menuChangeVarga
			// 
			this.menuChangeVarga.Index = 0;
			this.menuChangeVarga.Text = "Options";
			this.menuChangeVarga.Click += new System.EventHandler(this.menuChangeVarga_Click);
			// 
			// menuCopyLon
			// 
			this.menuCopyLon.Index = 1;
			this.menuCopyLon.Text = "Copy Longitude";
			this.menuCopyLon.Click += new System.EventHandler(this.menuCopyLon_Click);
			// 
			// menuBasicGrahas
			// 
			this.menuBasicGrahas.Index = 2;
			this.menuBasicGrahas.Text = "Basic Longitudes";
			this.menuBasicGrahas.Click += new System.EventHandler(this.menuBasicGrahas_Click);
			// 
			// menuOtherLongitudes
			// 
			this.menuOtherLongitudes.Index = 3;
			this.menuOtherLongitudes.Text = "Other Longitudes";
			this.menuOtherLongitudes.Click += new System.EventHandler(this.menuOtherLongitudes_Click);
			// 
			// menuMrityuLongitudes
			// 
			this.menuMrityuLongitudes.Index = 4;
			this.menuMrityuLongitudes.Text = "Mrityu Longitudes";
			this.menuMrityuLongitudes.Click += new System.EventHandler(this.menuMrityuLongitudes_Click);
			// 
			// menuSahamaLongitudes
			// 
			this.menuSahamaLongitudes.Index = 5;
			this.menuSahamaLongitudes.Text = "Sahama Longitudes";
			this.menuSahamaLongitudes.Click += new System.EventHandler(this.menuSahamaLongitudes_Click);
			// 
			// menuNonLonBodies
			// 
			this.menuNonLonBodies.Index = 6;
			this.menuNonLonBodies.Text = "Non-Longitude Bodies";
			this.menuNonLonBodies.Click += new System.EventHandler(this.menuNonLonBodies_Click);
			// 
			// menuCharaKarakas
			// 
			this.menuCharaKarakas.Index = 7;
			this.menuCharaKarakas.Text = "Chara Karakas (8)";
			this.menuCharaKarakas.Click += new System.EventHandler(this.menuCharaKarakas_Click);
			// 
			// menuCharaKarakas7
			// 
			this.menuCharaKarakas7.Index = 8;
			this.menuCharaKarakas7.Text = "Chara Karakas (7)";
			this.menuCharaKarakas7.Click += new System.EventHandler(this.menuCharaKarakas7_Click);
			// 
			// menu64Navamsa
			// 
			this.menu64Navamsa.Index = 9;
			this.menu64Navamsa.Text = "64th Navamsa";
			this.menu64Navamsa.Click += new System.EventHandler(this.menu64Navamsa_Click);
			// 
			// menuAstroInfo
			// 
			this.menuAstroInfo.Index = 10;
			this.menuAstroInfo.Text = "Astronomical Info";
			this.menuAstroInfo.Click += new System.EventHandler(this.menuAstroInfo_Click);
			// 
			// menuSpecialTithis
			// 
			this.menuSpecialTithis.Index = 11;
			this.menuSpecialTithis.Text = "Special Tithis";
			this.menuSpecialTithis.Click += new System.EventHandler(this.menuSpecialTithis_Click);
			// 
			// menuSpecialTaras
			// 
			this.menuSpecialTaras.Index = 12;
			this.menuSpecialTaras.Text = "Special Nakshatras";
			this.menuSpecialTaras.Click += new System.EventHandler(this.menuSpecialTaras_Click);
			// 
			// menuNakshatraAspects
			// 
			this.menuNakshatraAspects.Index = 13;
			this.menuNakshatraAspects.Text = "Nakshatra Aspects";
			this.menuNakshatraAspects.Click += new System.EventHandler(this.menuNakshatraAspects_Click);
			// 
			// menuBhavaCusps
			// 
			this.menuBhavaCusps.Index = 14;
			this.menuBhavaCusps.Text = "Bhava Cusps";
			this.menuBhavaCusps.Click += new System.EventHandler(this.menuBhavaCusps_Click);
			// 
			// menuAvasthas
			// 
			this.menuAvasthas.Index = 15;
			this.menuAvasthas.Text = "Avasthas";
			this.menuAvasthas.Click += new System.EventHandler(this.menuAvasthas_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 16;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 17;
			this.menuItem2.Text = "-";
			// 
			// BasicCalculationsControl
			// 
			this.ContextMenu = this.calculationsContextMenu;
			this.Controls.Add(this.mList);
			this.Name = "BasicCalculationsControl";
			this.Size = new System.Drawing.Size(496, 176);
			this.Load += new System.EventHandler(this.BasicCalculationsControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void BasicCalculationsControl_Load(object sender, System.EventArgs e)
		{
			this.AddViewsToContextMenu(this.calculationsContextMenu);
			Repopulate();
		
		}

		private void ResizeColumns ()
		{
			for (int i=0; i<this.mList.Columns.Count; i++)
			{
				this.mList.Columns[i].Width = -1;
			}
			this.mList.Columns[this.mList.Columns.Count-1].Width=-2;
		}
		string getTithiName (double val, ref double tithi, ref double perc)
		{
			double offset = val;
			while (offset >= 12.0) offset -= 12.0;
			int t = (int)Math.Floor(val/12.0) + 1;
			tithi = t;
			perc = 100-(offset/12.0*100);
			string[] tithis = new string[] 
			{
				"Prathama", "Dvitiya", "Tritiya", "Chaturthi", "Panchami", "Shashti",
				"Saptami", "Ashtami", "Navami", "Dashami", "Ekadasi", "Dvadashi",
				"Trayodashi", "Chaturdashi"
			};
			if (t == 15) return "Pournima";
			if (t == 30) return "Amavasya";
			string str;
			if (t > 15) 
			{
				str = "Krishna ";
				t-=15;
			} 
			else
			{
				str = "Shukla ";
			}
			return str + " " + tithis[t-1];

		}

		private void RepopulateSpecialTaras ()
		{
			int[] specialIndices = new int[] 
			{
				1, 10, 18, 16, 
				4, 7, 12, 13, 
				19, 22, 25
			};
			string[] specialNames = new string[]
			{
				"Janma", "Karma", "Saamudaayika", "Sanghaatika",
				"Jaati", "Naidhana", "Desa", "Abhisheka",
				"Aadhaana", "Vainaasika", "Maanasa"
			};

			this.mList.Columns.Clear();
			this.mList.Items.Clear();
			this.mList.Columns.Add ("Name", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Nakshatra (27)", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Nakshatra (28)", -1, System.Windows.Forms.HorizontalAlignment.Left);

			Nakshatra nmoon = h.getPosition(mhora.Body.Name.Moon).longitude.toNakshatra();
			Nakshatra28 nmoon28 = h.getPosition(mhora.Body.Name.Moon).longitude.toNakshatra28();
			for (int i=0; i<specialIndices.Length; i++)
			{
				Nakshatra sn = nmoon.add(specialIndices[i]);
				Nakshatra28 sn28 = nmoon28.add(specialIndices[i]);

				ListViewItem li = new ListViewItem();
				li.Text = String.Format("{0:00}  {1} Tara", specialIndices[i], specialNames[i]);
				li.SubItems.Add(sn.value.ToString());
				li.SubItems.Add(sn28.value.ToString());
				this.mList.Items.Add(li);
			}
			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();

		}

		int[] latta_aspects = new int[] { 12, 22, 3, 7, 6, 5, 8, 9 };
		int[][] tara_aspects = new int[][]
		{
			new int[] { 14, 15 },
			new int[] { 14, 15 },
			new int[] { 1, 3, 7, 8, 15 },
			new int[] { 1, 15 },
			new int[] { 10, 15, 19 },
			new int[] { 1, 15 },
			new int[] { 3, 5, 15, 19 },
			new int[] { }
		};

		string[] karakas = new string[]
		{ "Atma", "Amatya", "Bhratri", "Matri", "Pitri", "Putra", "Jnaati", "Dara" };
		string[] karakas7 = new string[]
		{ "Atma", "Amatya", "Bhratri", "Matri", "Pitri", "Jnaati", "Dara" };

		string[] karakas_s = new string[]
		{ "AK", "AmK", "BK", "MK", "PiK", "PuK", "JK", "DK"	};
		string[] karakas_s7 = new string[]
		{ "AK", "AmK", "BK", "MK", "PiK", "JK", "DK"	};

		private void RepopulateCharaKarakas ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Karaka", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Offset", -2, System.Windows.Forms.HorizontalAlignment.Left);

			ArrayList al = new ArrayList();
			int max = 0;
			if (this.vt == ViewType.ViewCharaKarakas)
				max = (int)mhora.Body.Name.Rahu;
			else
				max = (int)mhora.Body.Name.Saturn;


			for (int i=(int)mhora.Body.Name.Sun; i<=max; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				BodyPosition bp = h.getPosition(b);
				BodyKarakaComparer bkc = new BodyKarakaComparer(bp);
				al.Add (bkc);
			}
			al.Sort();

			for (int i=0; i<al.Count; i++)
			{
				ListViewItem li = new ListViewItem();
				BodyKarakaComparer bk = (BodyKarakaComparer)al[i];
				li.Text = mhora.Body.toString(bk.GetPosition.name);
				if (this.vt == ViewType.ViewCharaKarakas)
					li.SubItems.Add (this.karakas[i]);
				else
					li.SubItems.Add (this.karakas7[i]);
				li.SubItems.Add (string.Format("{0:0.00}", bk.getOffset()));
				this.mList.Items.Add (li);
			}


			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();
		}

		string[] avasthas = new string[] 
		{ 
			"Saisava (child - quarter)", 
			"Kumaara (adolescent - half)", 
			"Yuva (youth - full)", 
			"Vriddha (old - some)", 
			"Mrita (dead - none)" 
		};


		private void Repopulate64NavamsaHelper (Body.Name b, string name, BodyPosition bp, Division div)
		{
			DivisionPosition dp = bp.toDivisionPosition(div);
			ListViewItem li = new ListViewItem();
			li.Text = b.ToString();
			li.SubItems.Add (name);
			li.SubItems.Add (dp.zodiac_house.value.ToString());
			li.SubItems.Add (mhora.Body.toString(h.LordOfZodiacHouse(dp.zodiac_house, div)));
			this.mList.Items.Add (li);
		}
		private void RepopulateNonLonBodies ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Non Longitudinal Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Zodiac House", -2, System.Windows.Forms.HorizontalAlignment.Left);

			ArrayList al = h.CalculateArudhaDivisionPositions(options.DivisionType);
			al.AddRange( h.CalculateVarnadaDivisionPositions(options.DivisionType) );
			
			foreach (DivisionPosition dp in al)
			{
				string desc="";
				if (dp.name == mhora.Body.Name.Other)
					desc = dp.otherString;
				else
					desc = dp.name.ToString();

				ListViewItem li = new ListViewItem(desc);
				li.SubItems.Add(dp.zodiac_house.value.ToString());
				this.mList.Items.Add(li);		

			}
				
			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();

		}
		private void Repopulate64Navamsa()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Reference", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Count", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Rasi", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Lord", -2, System.Windows.Forms.HorizontalAlignment.Left);

			Body.Name[] bodyReferences = new Body.Name[]
			{ mhora.Body.Name.Lagna, mhora.Body.Name.Moon, mhora.Body.Name.Sun };

			foreach (mhora.Body.Name b in bodyReferences)
			{
				BodyPosition bp = (BodyPosition)h.getPosition(b).Clone();
				Longitude bpLon = bp.longitude.add(0);

				bp.longitude = bpLon.add(30.0/9.0*(64-1));
				this.Repopulate64NavamsaHelper (b, "64th Navamsa", bp, new Division(Basics.DivisionType.Navamsa));
				
				bp.longitude = bpLon.add(30.0/3.0*(22-1));
				this.Repopulate64NavamsaHelper (b, "22nd Drekkana", bp, new Division(Basics.DivisionType.DrekkanaParasara));
				this.Repopulate64NavamsaHelper (b, "22nd Drekkana (Parivritti)", bp, new Division(Basics.DivisionType.DrekkanaParivrittitraya));
				this.Repopulate64NavamsaHelper (b, "22nd Drekkana (Somnath)", bp, new Division(Basics.DivisionType.DrekkanaSomnath));
				this.Repopulate64NavamsaHelper (b, "22nd Drekkana (Jagannath)", bp, new Division(Basics.DivisionType.DrekkanaJagannath));
			}

			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();
		}

		private void RepopulateAvasthas()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Age", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Alertness", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Mood", -2, System.Windows.Forms.HorizontalAlignment.Left);

			for (int i=(int)mhora.Body.Name.Sun; i<=(int)mhora.Body.Name.Ketu; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				ListViewItem li = new ListViewItem();
				li.Text = mhora.Body.toString(b);
				DivisionPosition dp = h.getPosition(b).toDivisionPosition(new Division(Basics.DivisionType.Panchamsa));
				int avastha_index=-1;
				switch ((int)dp.zodiac_house.value % 2)
				{
					case 1: avastha_index = dp.part; break;
					case 0: avastha_index = 6-dp.part; break;
				}
				li.SubItems.Add (avasthas[avastha_index-1]);
				this.mList.Items.Add(li);
			}

			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();
		}
		private void RepopulateNakshatraAspects()
		{

			this.mList.Clear();
			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Latta", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Aspected", -2, System.Windows.Forms.HorizontalAlignment.Left);

			for (int i = (int)mhora.Body.Name.Sun; i <= (int)mhora.Body.Name.Rahu; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				bool dirForward = true;
				if (i%2 == 1) dirForward = false;

				BodyPosition bp = h.getPosition(b);
				Nakshatra n = bp.longitude.toNakshatra();
				Nakshatra l = null;
				
				if (dirForward)
					l = n.add(latta_aspects[i]);
				else
					l = n.addReverse(latta_aspects[i]);

				string nak_fmt = "";
				foreach (int j in tara_aspects[i])
				{
					Nakshatra na = n.add (j);
					if (nak_fmt.Length > 0)
						nak_fmt = string.Format ("{0}, {1}-{2}", nak_fmt, j, na.value);
					else
						nak_fmt = string.Format ("{0}-{1}", j, na.value);
				}

				ListViewItem li = new ListViewItem(mhora.Body.toString(b));
				string fmt = string.Format("{0:00}-{1}", latta_aspects[i], l.value);
				li.SubItems.Add (fmt);
				li.SubItems.Add (nak_fmt);
				this.mList.Items.Add (li);
			}

			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();

		}
		private void RepopulateAstronomicalInfo ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Lon (deg)", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("/ day", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Lat (deb)", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("/ day", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Distance (AU)", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("/ day", -1, System.Windows.Forms.HorizontalAlignment.Left);

			for (int i = (int)mhora.Body.Name.Sun; i<=(int)mhora.Body.Name.Saturn; i++)
			{
				Body.Name b = (Body.Name)i;
				BodyPosition bp = h.getPosition(b);
				ListViewItem li = new ListViewItem (mhora.Body.toString(b));
				li.SubItems.Add (bp.longitude.value.ToString());
				li.SubItems.Add (bp.speed_longitude.ToString());
				li.SubItems.Add (bp.latitude.ToString());
				li.SubItems.Add (bp.speed_latitude.ToString());
				li.SubItems.Add (bp.distance.ToString());
				li.SubItems.Add (bp.speed_distance.ToString());
				this.mList.Items.Add (li);
			}
			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();
		}
		private void RepopulateSpecialTithis ()
		{
			string[] specialNames = new string[]
			{
				"", "Janma", "Dhana", "Bhratri", "Matri", "Putra", "Shatru", 
				"Kalatra", "Mrityu", "Bhagya", "Karma", "Laabha", "Vyaya"
			};

			this.mList.Columns.Clear();
			this.mList.Items.Clear();
			this.mList.Columns.Add ("Name", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Tithi", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("% Left", -1, System.Windows.Forms.HorizontalAlignment.Left);

			Longitude spos = h.getPosition(mhora.Body.Name.Sun).longitude;
			Longitude mpos = h.getPosition(mhora.Body.Name.Moon).longitude;
			double baseTithi = mpos.sub(spos).value;
			for (int i=1; i<=12; i++)
			{
				double spTithiVal = new Longitude(baseTithi*i).value;
				double tithi = 0;
				double perc = 0;
				ListViewItem li = new ListViewItem();
				string s1 = String.Format("{0:00}  {1} Tithi", i, specialNames[i]);
				li.Text = s1;
				string s2 = getTithiName (spTithiVal, ref tithi, ref perc);
				li.SubItems.Add (s2);
				string s3 = String.Format("{0:###.##}%", perc);
				li.SubItems.Add(s3);
				this.mList.Items.Add(li);
			}

			this.ColorAndFontRows(this.mList);
			this.ResizeColumns();

		}
		private string getNakLord(Longitude l)
		{
			INakshatraDasa id=null;
			switch (options.NakshatraLord)
			{
				default:
				case ENakshatraLord.Vimsottari:
					id = new VimsottariDasa(h);
					break;
				case ENakshatraLord.Ashtottari:
					id = new AshtottariDasa(h); 
					break;
				case ENakshatraLord.Yogini:
					id = new YoginiDasa(h);
					break;
				case ENakshatraLord.Shodashottari:
					id = new ShodashottariDasa(h);
					break;
				case ENakshatraLord.Dwadashottari:
					id = new DwadashottariDasa(h);
					break;
				case ENakshatraLord.Panchottari:
					id = new PanchottariDasa(h);
					break;
				case ENakshatraLord.Shatabdika:
					id = new ShatabdikaDasa(h);
					break;
				case ENakshatraLord.ChaturashitiSama:
					id = new ChaturashitiSamaDasa(h);
					break;
				case ENakshatraLord.DwisaptatiSama:
					id = new DwisaptatiSamaDasa(h);
					break;
				case ENakshatraLord.ShatTrimshaSama:
					id = new ShatTrimshaSamaDasa(h);
					break;
			}
			Body.Name b = id.lordOfNakshatra(l.toNakshatra());
			return b.ToString();
		}
		private void RepopulateHouseCusps ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("System", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("House", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Start", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Stop", -1, System.Windows.Forms.HorizontalAlignment.Left);

			for (int i=0; i<12; i++)
			{
				ListViewItem li = new ListViewItem ();
				li.Text = string.Format("{0}", (System.Char)h.swephHouseSystem);
				li.SubItems.Add (h.swephHouseCusps[i].value.ToString());
				li.SubItems.Add (h.swephHouseCusps[i+1].value.ToString());
				this.mList.Items.Add (li);
			}
			this.ColorAndFontRows(mList);
			this.ResizeColumns();
		}
		private string longitudeToString (Longitude lon)
		{
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
		private void RepopulateBhavaCusps ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("Cusp Start", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Cusp End", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Rasi", -2, System.Windows.Forms.HorizontalAlignment.Left);

			Longitude lpos = h.getPosition(mhora.Body.Name.Lagna).longitude.add(0);
			BodyPosition bp = new BodyPosition(h, mhora.Body.Name.Lagna, BodyType.Name.Lagna, lpos, 0, 0, 0, 0, 0);
			for (int i=0; i<12; i++)
			{
				DivisionPosition dp = bp.toDivisionPosition(this.options.DivisionType);
				ListViewItem li = new ListViewItem();
				li.Text = this.longitudeToString(new Longitude(dp.cusp_lower));
				li.SubItems.Add (this.longitudeToString(new Longitude(dp.cusp_higher)));
				li.SubItems.Add (dp.zodiac_house.value.ToString());
				bp.longitude = new Longitude(dp.cusp_higher + 1);
				mList.Items.Add(li);
			}
			this.ColorAndFontRows(mList);
			this.ResizeColumns();

		}
		static string[] mRulersHora = new string[]
			{
				"Devaas - Sun", "Pitris - Moon"
			};
		static string[] mRulersDrekkana = new string[]
			{
				"Naarada", "Agastya", "Durvaasa"
			};
		static string[] mRulersChaturthamsa = new string[]
			{
				"Sanaka", "Sanandana", "Sanat Kumaara", "Sanaatana"
			};
		static string[] mRulersSaptamsa = new string[]
			{
				"Kshaara", "Ksheera", "Dadhi", "Ghrita", "Ikshurasa", "Madya", "Shuddha Jala"
			};
		static string[] mRulersNavamsa = new string[]
			{
				"Deva", "Nara", "Rakshasa"
			};
		static string[] mRulersDasamsa = new string[]
			{
				"Indra", "Agni", "Yama", "Rakshasa", "Varuna", "Vayu", "Kubera", "Ishana", "Brahma", "Ananta"
			};
		static string[] mRulersDwadasamsa = new string[]
			{
				"Ganesha", "Ashwini Kumars", "Yama", "Sarpa"
			};
		static string[] mRulersShodasamsa = new string[]
			{
				"Brahma", "Vishnu", "Shiva", "Surya"
			};
		static string[] mRulersVimsamsa = new string[]
			{
				"Kali", "Gauri", "Jaya", "Lakshmi", "Vijaya", "Vimala", "Sati", "Tara",
				"Jwalamukhi", "Shaveta", "Lalita", "Bagla", "Pratyangira", "Shachi", "Raudri",
				"Bhavani", "Varda", "Jaya", "Tripura", "Sumukhi",
				"Daya", "Medha", "China Shirsha", "Pishachini", "Dhoomavati", "Matangi",
				"Bala", "Bhadra", "Aruna", "Anala", "Pingala", "Chuccuka", "Ghora", "Varahi",
				"Vaishnavi", "Sita", "Bhuvaneshi", "Bhairavi", "Mangla", "Aparajita"
			};
		static string[] mRulersChaturvimsamsa = new string[]
			{
				"Skanda", "Parashudhara", "Anala", "Vishvakarma", "Bhaga", "Mitra", "Maya",
				"Antaka", "Vrishdhwaja", "Govinda", "Madana", "Bhima"
			};
		static string[] mRulersNakshatramsa = new string[]
			{
				"Ashwini Kumara", "Yama", "Agni", "Brahma", "Chandra Isa", "Aditi", "Jiva", "Abhi",
				"Pitara", "Bhaga", "Aryama", "Surya", "Tvashta", "Maruta", "Shakragni", "Mitra", "Indra",
				"Rakshasa", "Varuna", "Vishvadeva", "Brahma", "Govinda", "Vasu", "Varuna", "Ajapata", "Ahirbudhnya", "Pusha"
			};
		static string[] mRulersTrimsamsa = new string[]
			{
				"Agni", "Vayu", "Indra", "Kubera", "Varuna"
			};
		static string[] mRulersKhavedamsa = new string[]
			{
				"Vishnu", "Chandra", "Marichi", "Twashta", "Brahma", "Shiva", "Surya", "Yama", "Yakshesha",
				"Ghandharva", "Kala", "Varuna"
			};
		static string[] mRulersAkshavedamsa = new string[]
			{
				"Brahma", "Shiva", "Vishnu"
			};
		static string[] mRulersShashtyamsa = new string[]
			{
				"Ghora", "Rakshasa", "Deva", "Kubera", "Yaksha", "Kinnara",
				"Bharashta", "Kulaghna", "Garala", "Vahni", "Maya", "Purishaka",
				"Apampathi", "Marut", "Kaala", "Sarpa", "Amrita", "Indu",
				"Mridu", "Komala", "Heramba", "Brahma", "Vishnu", "Maheshwara",
				"Deva", "Ardra", "Kalinasa", "Kshitishwara", "Kamalakara", "Gulika",
				"Mrityu", "Kala", "Davagni", "Ghora", "Yama", "Kantaka",
				"Sudha", "Amrita", "Poornachandra", "Vishadagdha", "Kulanasa", "Vamsa Khaya",
				"Utpata", "Kala", "Saumya", "Komala", "Sheetala", "Karala damshtra",
				"Chandramukhi", "Praveena", "Kala Pavaka", "Dandayudha", "Nirmala", "Saumya", 
				"Kroora", "AtiSheetala", "Amrita", "Payodhi", "Bhramana", "Chandrarekha",
			};
		static string[] mRulersNadiamsaRajan = new string[]
		{
			"Vasudha", "Vaishnavi", "Brahmi", "Kalakoota", "Sankari", 
			"Sadaakari", "Samaa", "Saumya", "Suraa", "Maayaa",
			"Manoharaa", "Maadhavi", "Manjuswana", "Ghoraa", "Kumbhini", 
			"Kutilaa", "Prabhaa", "Paraa", "Payaswini", "Maala",
			"Jagathi", "Jarjharaa", "Dhruva", "TO BE CONTINUED"
		};

		static string[] mRulersNadiamsaCKN = new string[]
		{
			"Vasudha", "Vaishnavi", "Brahmi", "Kalakoota", "Sankari",
			"Sudhakarasama", "Saumya", "Suraa", "Maaya", "Manoharaa",
			"Maadhavi", "Manjuswana", "Ghoraa", "Kumbhini", "Kutilaa",
			"Prabhaa", "Paraa", "Payaswini", "Malaa", "Jagathi",
			"Jarjhari", "Dhruvaa", "Musalaa", "Mudgala", "Pasaa",
			"Chambaka", "Daamini", "Mahi", "Kalushaa", "Kamalaa",
			"Kanthaa", "Kaalaa", "Karikaraa", "Kshamaa", "Durdharaa",
			"Durbhagaa", "Viswa", "Visirnaa", "Vihwala", "Anilaa",
			"Bhima", "Sukhaprada", "Snigdha", "Sodaraa", "Surasundari",
			"Amritaprasini", "Karalaa", "KamadrukkaraVeerini", "Gahwaraa", "Kundini",
			"Kanthaa", "Vishakhya", "Vishanaasini", "Nirmada", "Seethala",
			"Nimnaa", "Preeta", "Priyavivardhani", "Manaadha", "Durbhaga",
			"Chitraa", "Vichitra", "Chirajivini", "Boopa", "Gadaaharaa",
			"Naalaa", "Gaalavee", "Nirmalaa", "Nadi", "Sudha", 
			"Mritamsuga", "Kaali", "Kaalika", "Kalushankura", "Krailokyamohanakari",
			"Mahaamaayaa", "Suseethala", "Sukhadaa", "Suprabhaa", "Sobhaa",
			"Sobhana", "Sivadaa", "Siva", "Balaa", "Jwalaa",
			"Gadaa", "Gaadaa", "Sootana", "Sumanoharaa", "Somavalli",
			"Somalatha", "Mangala", "Mudrika", "Sudha", "Melaa", 
			"Apavargaa", "Pasyathaa", "Navaneetha", "Nisachari", "Nivrithi",
			"Nirgathaa", "Saaraa", "Samagaa", "Samadaa", "Samaa",
			"Visvambharaa", "Kumari", "Kokila", "Kunjarakrithi", "Indra",
			"Swaahaa", "Swadha", "Vahni", "Preethaa", "Yakshi", 
			"Achalaprabha", "Saarini", "Madhuraa", "Maitri", "Harini",
			"Haarini", "Maruthaa", "DHananjaya", "Dhanakari", "Dhanada",
			"Kaccapa", "Ambuja", "Isaani", "Soolini", "Raudri",
			"Sivaasivakari", "Kalaa", "Kundaa", "Mukundaa", "Bharata",
			"Kadali", "Smaraa", "Basitha", "Kodala", "Kokilamsa",
			"Kaamini", "Kalasodbhava", "Viraprasoo", "Sangaraa", "Sathayagna",
			"Sataavari", "Sragvi", "Paatalini", "Naagapankaja", "Parameswari"
		};
		private string AmsaRuler (BodyPosition bp, DivisionPosition dp)
		{

			if (dp.ruler_index == 0) return "";
			int ri = dp.ruler_index-1;

			if (this.options.DivisionType.MultipleDivisions.Length == 1)
			switch (this.options.DivisionType.MultipleDivisions[0].Varga)
			{
				case Basics.DivisionType.HoraParasara: return mRulersHora[ri];
				case Basics.DivisionType.DrekkanaParasara: return mRulersDrekkana[ri];
				case Basics.DivisionType.Chaturthamsa: return mRulersChaturthamsa[ri];
				case Basics.DivisionType.Saptamsa: return mRulersSaptamsa[ri];
				case Basics.DivisionType.Navamsa: return mRulersNavamsa[ri];
				case Basics.DivisionType.Dasamsa: return mRulersDasamsa[ri];
				case Basics.DivisionType.Dwadasamsa: return mRulersDwadasamsa[ri];
				case Basics.DivisionType.Shodasamsa: return mRulersShodasamsa[ri];
				case Basics.DivisionType.Vimsamsa: return mRulersVimsamsa[ri];
				case Basics.DivisionType.Chaturvimsamsa: return mRulersChaturvimsamsa[ri];
				case Basics.DivisionType.Nakshatramsa: return mRulersNakshatramsa[ri];
				case Basics.DivisionType.Trimsamsa: return mRulersTrimsamsa[ri];
				case Basics.DivisionType.Khavedamsa: return mRulersKhavedamsa[ri];
				case Basics.DivisionType.Akshavedamsa: return mRulersAkshavedamsa[ri];
				case Basics.DivisionType.Shashtyamsa: return mRulersShashtyamsa[ri];
				case Basics.DivisionType.Nadiamsa: return mRulersNadiamsaCKN[ri];
				case Basics.DivisionType.NadiamsaCKN: return mRulersNadiamsaCKN[ri];
			}
			return "";
		}

		private string GetBodyString (BodyPosition bp)
		{
			string dir = bp.speed_longitude >= 0.0 ? "" : " (R)";

			if (bp.name == mhora.Body.Name.Other ||
				bp.name == mhora.Body.Name.MrityuPoint)
				return bp.otherString + dir;

			return bp.name.ToString();
		}
		private bool CheckBodyForCurrentView (BodyPosition bp)
		{
			switch (this.vt)
			{
				case ViewType.ViewMrityuLongitudes:
					if (bp.name == mhora.Body.Name.MrityuPoint) return true;
					return false;
				case ViewType.ViewOtherLongitudes:
					if (bp.name == mhora.Body.Name.Other &&
						bp.type != BodyType.Name.Sahama) return true;
					return false;
				case ViewType.ViewSahamaLongitudes:
					if (bp.type == BodyType.Name.Sahama) return true;
					return false;
				case ViewType.ViewBasicGrahas:
					if (bp.name == mhora.Body.Name.MrityuPoint ||
						bp.name == mhora.Body.Name.Other)
						return false;
					return true;
			}
			return true;
		}
		private void RepopulateBasicGrahas ()
		{
			this.mList.Columns.Clear();
			this.mList.Items.Clear();

			ArrayList al = new ArrayList();
			for (int i=(int)mhora.Body.Name.Sun; i<=(int)mhora.Body.Name.Rahu; i++)
			{
				mhora.Body.Name b = (mhora.Body.Name)i;
				BodyPosition bp = h.getPosition(b);
				BodyKarakaComparer bkc = new BodyKarakaComparer(bp);
				al.Add (bkc);
			}
			al.Sort();
			int[] karaka_indices = new int[9];
			for (int i=0; i<al.Count; i++)
			{
				BodyKarakaComparer bk = (BodyKarakaComparer)al[i];
				karaka_indices[(int)bk.GetPosition.name] = i;
			}


			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Longitude", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Nakshatra", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Pada", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("NakLord", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add (options.DivisionType.ToString(), 100, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Part", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Ruler", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Cusp Start", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Cusp End", -2, System.Windows.Forms.HorizontalAlignment.Left);


			foreach (BodyPosition bp in h.positionList)
			{
				if (false == this.CheckBodyForCurrentView(bp))
					continue;

				ListViewItem li = new ListViewItem();
				li.Text = GetBodyString(bp);

				if ((int)bp.name >= (int)mhora.Body.Name.Sun && (int)bp.name <= (int)mhora.Body.Name.Rahu)
					li.Text = string.Format ("{0}   {1}", 
						li.Text, this.karakas_s[karaka_indices[(int)bp.name]] );

				li.SubItems.Add (this.longitudeToString(bp.longitude));
				li.SubItems.Add (bp.longitude.toNakshatra().value.ToString());
				li.SubItems.Add (bp.longitude.toNakshatraPada().ToString());
				li.SubItems.Add (this.getNakLord(bp.longitude));

				DivisionPosition dp = bp.toDivisionPosition(options.DivisionType);
				li.SubItems.Add (dp.zodiac_house.value.ToString());
				li.SubItems.Add (dp.part.ToString());
				li.SubItems.Add (this.AmsaRuler (bp, dp));
				li.SubItems.Add (this.longitudeToString(new Longitude(dp.cusp_lower)));
				li.SubItems.Add (this.longitudeToString(new Longitude(dp.cusp_higher)));

				mList.Items.Add (li);

			}
			this.ColorAndFontRows(mList);
			this.ResizeColumns();
		}

		private void Repopulate ()
		{
			this.mList.BeginUpdate();
			switch (this.vt)
			{
				case ViewType.ViewBasicGrahas: this.RepopulateBasicGrahas(); break;
				case ViewType.ViewOtherLongitudes: this.RepopulateBasicGrahas(); break;
				case ViewType.ViewMrityuLongitudes: this.RepopulateBasicGrahas(); break;
				case ViewType.ViewSahamaLongitudes: this.RepopulateBasicGrahas(); break;
				case ViewType.ViewSpecialTithis: this.RepopulateSpecialTithis(); break;
				case ViewType.ViewSpecialTaras: this.RepopulateSpecialTaras(); break;
				case ViewType.ViewBhavaCusps: this.RepopulateBhavaCusps(); break;
				case ViewType.ViewAstronomicalInfo: this.RepopulateAstronomicalInfo(); break;
				case ViewType.ViewNakshatraAspects: this.RepopulateNakshatraAspects(); break;
				case ViewType.ViewCharaKarakas: this.RepopulateCharaKarakas(); break;
				case ViewType.ViewCharaKarakas7: this.RepopulateCharaKarakas(); break;
				case ViewType.ViewAvasthas: this.RepopulateAvasthas(); break;
				case ViewType.View64Navamsa: this.Repopulate64Navamsa(); break;
				case ViewType.ViewNonLonBodies: this.RepopulateNonLonBodies(); break;
			}
			this.mList.EndUpdate();
		}
		void OnRecalculate (Object o)
		{
			Repopulate();
		}

		void OnRedisplay (Object o)
		{
			this.ColorAndFontRows(this.mList);
		}

		protected override void copyToClipboard ()
		{
		}

		private void mList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			//mChangeView_Click (sender, e);
		}

		private void mList_MouseHover(object sender, System.EventArgs e)
		{

		}

		private void ResetMenuItems ()
		{
			foreach (MenuItem mi in this.ContextMenu.MenuItems)
				mi.Checked = false;
			this.menuChangeVarga.Enabled = false;
			this.menuCopyLon.Enabled = false;
		}
		private void menuBasicGrahas_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuCopyLon.Enabled = true;
			this.menuBasicGrahas.Checked = true;
			this.vt = ViewType.ViewBasicGrahas;
			this.Repopulate();
		}

		private void menuSpecialTithis_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuSpecialTithis.Checked = true;
			this.vt = ViewType.ViewSpecialTithis;
			this.Repopulate();
		}

		private void menuSpecialTaras_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuSpecialTaras.Checked = true;
			this.vt = ViewType.ViewSpecialTaras;
			this.Repopulate();
		}


		public object SetOptions (Object o)
		{
			this.options.Copy(o);
			this.Repopulate();
			return this.options.Clone();
		}
		private void menuChangeVarga_Click(object sender, System.EventArgs e)
		{
			MhoraOptions opts = new MhoraOptions(options.Clone(), new ApplyOptions(SetOptions));
			opts.ShowDialog();
		}

		private void menuBhavaCusps_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuBhavaCusps.Checked = true;
			this.menuChangeVarga.Enabled = true;

			this.vt = ViewType.ViewBhavaCusps;
			this.Repopulate();
		}

		private void menuOtherLongitudes_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuOtherLongitudes.Checked = true;
			this.menuCopyLon.Enabled = true;
			this.vt = ViewType.ViewOtherLongitudes;
			this.Repopulate();
		}

		private void menuMrityuLongitudes_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuCopyLon.Enabled = true;
			this.menuMrityuLongitudes.Checked = true;
			this.vt = ViewType.ViewMrityuLongitudes;
			this.Repopulate();		
		}

		private void menuAstroInfo_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuAstroInfo.Checked = true;
			this.vt = ViewType.ViewAstronomicalInfo;
			this.Repopulate();
		}

		private void menuNakshatraAspects_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuNakshatraAspects.Checked = true;
			this.vt = ViewType.ViewNakshatraAspects;
			this.Repopulate();
		}

		private void menuCharaKarakas_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuCharaKarakas.Checked = true;
			this.vt = ViewType.ViewCharaKarakas;
			this.Repopulate();
		}

		private void menuSahamaLongitudes_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuCopyLon.Enabled = true;
			this.menuSahamaLongitudes.Checked = true;
			this.vt = ViewType.ViewSahamaLongitudes;
			this.Repopulate();				
		}

		private void menuAvasthas_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuAvasthas.Checked = true;
			this.vt = ViewType.ViewAvasthas;
			this.Repopulate();
		}

		private void calculationsContextMenu_Popup(object sender, System.EventArgs e)
		{
		
		}

		private void menuCharaKarakas7_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuCharaKarakas7.Checked = true;
			this.vt = ViewType.ViewCharaKarakas7;
			this.Repopulate();
		}

		private void menu64Navamsa_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menu64Navamsa.Checked = true;
			this.vt = ViewType.View64Navamsa;
			this.Repopulate();
		}

		private void menuCopyLon_Click(object sender, System.EventArgs e)
		{
			if (this.mList.SelectedItems.Count <= 0) return;
			ListViewItem li = this.mList.SelectedItems[0];
			Clipboard.SetDataObject(li.SubItems[1].Text, false);
		}

		private void menuNonLonBodies_Click(object sender, System.EventArgs e)
		{
			this.ResetMenuItems();
			this.menuChangeVarga.Enabled = true;
			this.menuNonLonBodies.Checked = true;
			this.vt = ViewType.ViewNonLonBodies;
			this.Repopulate();		
		}

		private void mList_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart))) 
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;	
		}

		private void mList_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart)))
			{
				Division div = Division.CopyFromClipboard();
				if (null == div) return;
				this.options.DivisionType = div;
				this.OnRecalculate(this.options);
			}
		}

	}
}
