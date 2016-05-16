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
using System.Windows.Forms;

namespace mhora
{
	public class KutaMatchingControl : mhora.MhoraControl
	{
		private System.Windows.Forms.TextBox tbHorMale;
		private System.Windows.Forms.TextBox tbHorFemale;
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ListView lView;
		private System.Windows.Forms.Button bMaleChange;
		private System.Windows.Forms.Button bFemaleChange;
		private System.Windows.Forms.ContextMenu mContext;
		private System.Windows.Forms.ColumnHeader Male;
		protected System.Windows.Forms.ColumnHeader Female;
		private System.Windows.Forms.ColumnHeader Score;
		private System.Windows.Forms.ColumnHeader Kuta;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		Horoscope h2=null;

		private void populateTextBoxes ()
		{
			this.tbHorMale.Text = "Current Chart";
			this.tbHorFemale.Text = "Current Chart";
			foreach (Form f in ((MhoraContainer)MhoraGlobalOptions.mainControl).MdiChildren)
			if (f is MhoraChild)
			{
				MhoraChild mch = (MhoraChild)f;
				if (mch.getHoroscope() == this.h)
					this.tbHorMale.Text = mch.Text;
				if (mch.getHoroscope() == this.h2)
					this.tbHorFemale.Text = mch.Text;
			}
		}

		public KutaMatchingControl(Horoscope _h, Horoscope _h2)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			h2 = _h2;
			h.Changed += new EvtChanged(OnRecalculate);
			h2.Changed += new EvtChanged(OnRecalculate);
			this.AddViewsToContextMenu(this.mContext);
			this.populateTextBoxes();
			this.OnRecalculate(h);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbHorMale = new System.Windows.Forms.TextBox();
			this.tbHorFemale = new System.Windows.Forms.TextBox();
			this.lView = new System.Windows.Forms.ListView();
			this.Kuta = new System.Windows.Forms.ColumnHeader();
			this.Male = new System.Windows.Forms.ColumnHeader();
			this.Female = new System.Windows.Forms.ColumnHeader();
			this.Score = new System.Windows.Forms.ColumnHeader();
			this.bMaleChange = new System.Windows.Forms.Button();
			this.bFemaleChange = new System.Windows.Forms.Button();
			this.mContext = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbHorMale
			// 
			this.tbHorMale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbHorMale.Location = new System.Drawing.Point(72, 8);
			this.tbHorMale.Name = "tbHorMale";
			this.tbHorMale.ReadOnly = true;
			this.tbHorMale.Size = new System.Drawing.Size(320, 20);
			this.tbHorMale.TabIndex = 0;
			this.tbHorMale.Text = "";
			// 
			// tbHorFemale
			// 
			this.tbHorFemale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbHorFemale.Location = new System.Drawing.Point(72, 40);
			this.tbHorFemale.Name = "tbHorFemale";
			this.tbHorFemale.ReadOnly = true;
			this.tbHorFemale.Size = new System.Drawing.Size(320, 20);
			this.tbHorFemale.TabIndex = 1;
			this.tbHorFemale.Text = "";
			// 
			// lView
			// 
			this.lView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					this.Kuta,
																					this.Male,
																					this.Female,
																					this.Score});
			this.lView.FullRowSelect = true;
			this.lView.Location = new System.Drawing.Point(8, 72);
			this.lView.Name = "lView";
			this.lView.Size = new System.Drawing.Size(544, 264);
			this.lView.TabIndex = 2;
			this.lView.View = System.Windows.Forms.View.Details;
			this.lView.SelectedIndexChanged += new System.EventHandler(this.lView_SelectedIndexChanged);
			// 
			// Kuta
			// 
			this.Kuta.Text = "Kuta";
			this.Kuta.Width = 163;
			// 
			// Male
			// 
			this.Male.Text = "Male";
			this.Male.Width = 126;
			// 
			// Female
			// 
			this.Female.Text = "Female";
			this.Female.Width = 119;
			// 
			// Score
			// 
			this.Score.Text = "Score";
			this.Score.Width = 107;
			// 
			// bMaleChange
			// 
			this.bMaleChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bMaleChange.Location = new System.Drawing.Point(464, 8);
			this.bMaleChange.Name = "bMaleChange";
			this.bMaleChange.Size = new System.Drawing.Size(64, 24);
			this.bMaleChange.TabIndex = 3;
			this.bMaleChange.Text = "Change";
			this.bMaleChange.Click += new System.EventHandler(this.bMaleChange_Click);
			// 
			// bFemaleChange
			// 
			this.bFemaleChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bFemaleChange.Location = new System.Drawing.Point(464, 40);
			this.bFemaleChange.Name = "bFemaleChange";
			this.bFemaleChange.Size = new System.Drawing.Size(64, 23);
			this.bFemaleChange.TabIndex = 4;
			this.bFemaleChange.Text = "Change";
			this.bFemaleChange.Click += new System.EventHandler(this.bFemaleChange_Click);
			// 
			// mContext
			// 
			this.mContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1,
																					 this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 23);
			this.label1.TabIndex = 5;
			this.label1.Text = "Male:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "Female:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// KutaMatchingControl
			// 
			this.ContextMenu = this.mContext;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bFemaleChange);
			this.Controls.Add(this.bMaleChange);
			this.Controls.Add(this.lView);
			this.Controls.Add(this.tbHorFemale);
			this.Controls.Add(this.tbHorMale);
			this.Name = "KutaMatchingControl";
			this.Size = new System.Drawing.Size(560, 344);
			this.Load += new System.EventHandler(this.KutaMatchingControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public string getGhatakaString (bool gh)
		{
			if (gh)
				return "Ghataka";
			else
				return "-";

		}
		public void OnRecalculate (Object o)
		{
			Division dtype = new Division(Basics.DivisionType.Rasi);

			BodyPosition l1 = h.getPosition(Body.Name.Lagna);
			BodyPosition l2 = h2.getPosition(Body.Name.Lagna);
			BodyPosition m1 = h.getPosition(Body.Name.Moon);
			BodyPosition m2 = h2.getPosition(Body.Name.Moon);
			ZodiacHouse z1 = m1.toDivisionPosition(dtype).zodiac_house;
			ZodiacHouse z2 = m2.toDivisionPosition(dtype).zodiac_house;
			Nakshatra n1 = m1.longitude.toNakshatra();
			Nakshatra n2 = m2.longitude.toNakshatra();

			this.lView.Items.Clear();

			{
				ListViewItem li = new ListViewItem("Nakshatra Yoni");
				li.SubItems.Add(KutaNakshatraYoni.getType(n1).ToString()
					+ " (" + KutaNakshatraYoni.getSex(n1).ToString() + ")");
				li.SubItems.Add(KutaNakshatraYoni.getType(n2).ToString()
					+ " (" + KutaNakshatraYoni.getSex(n2).ToString() + ")");
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Rasi Yoni");
				li.SubItems.Add(KutaRasiYoni.getType(z1).ToString());
				li.SubItems.Add(KutaRasiYoni.getType(z2).ToString());
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Varna");
				li.SubItems.Add(KutaVarna.getType(n1).ToString());
				li.SubItems.Add(KutaVarna.getType(n2).ToString());
				li.SubItems.Add(KutaVarna.getScore(n1,n2).ToString() + "/" + KutaVarna.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Gana (Chandra)");
				li.SubItems.Add(KutaGana.getType(n1).ToString());
				li.SubItems.Add(KutaGana.getType(n2).ToString());
				li.SubItems.Add(KutaGana.getScore(n1,n2).ToString() + "/" + KutaGana.getMaxScore().ToString());

				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Gana (Lagna)");
				li.SubItems.Add(KutaGana.getType(l1.longitude.toNakshatra()).ToString());
				li.SubItems.Add(KutaGana.getType(l2.longitude.toNakshatra()).ToString());
				li.SubItems.Add(KutaGana.getScore(l1.longitude.toNakshatra(), l2.longitude.toNakshatra()).ToString()
					+ "/" + KutaGana.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Vedha");
				li.SubItems.Add(KutaVedha.getType(n1).ToString());
				li.SubItems.Add(KutaVedha.getType(n2).ToString());
				li.SubItems.Add(KutaVedha.getScore(n1,n2).ToString() + "/" + KutaVedha.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{	
				ListViewItem li = new ListViewItem("Rajju");
				li.SubItems.Add(KutaRajju.getType(n1).ToString());
				li.SubItems.Add(KutaRajju.getType(n2).ToString());
				li.SubItems.Add(KutaRajju.getScore(n1,n2).ToString() + "/" + KutaRajju.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{	
				ListViewItem li = new ListViewItem("Nadi");
				li.SubItems.Add(KutaNadi.getType(n1).ToString());
				li.SubItems.Add(KutaNadi.getType(n2).ToString());
				li.SubItems.Add(KutaNadi.getScore(n1,n2).ToString() + "/" + KutaNadi.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{	
				ListViewItem li = new ListViewItem("Gotra (TD:Abhi)");
				li.SubItems.Add(KutaGotra.getType(n1).ToString());
				li.SubItems.Add(KutaGotra.getType(n2).ToString());
				li.SubItems.Add(KutaGotra.getScore(n1,n2).ToString() + "/" + KutaGotra.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{	
				ListViewItem li = new ListViewItem("Vihanga");
				li.SubItems.Add(KutaVihanga.getType(n1).ToString());
				li.SubItems.Add(KutaVihanga.getType(n2).ToString());
				li.SubItems.Add(KutaVihanga.getDominator(n1,n2).ToString());
				lView.Items.Add(li);
			}
			{	
				ListViewItem li = new ListViewItem("Bhuta (Nakshatra)");
				li.SubItems.Add(KutaBhutaNakshatra.getType(n1).ToString());
				li.SubItems.Add(KutaBhutaNakshatra.getType(n2).ToString());
				li.SubItems.Add(KutaBhutaNakshatra.getScore(n1,n2).ToString() + "/" + KutaBhutaNakshatra.getMaxScore().ToString());
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka (Moon)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				ZodiacHouse ch = h2.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				bool isGhataka = GhatakaMoon.checkGhataka(ja, ch);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(ch.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka (Tithi)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				Longitude ltithi = h2.getPosition(Body.Name.Moon).longitude.sub(h2.getPosition(Body.Name.Sun).longitude);
				Tithi t = ltithi.toTithi();
                bool isGhataka = GhatakaTithi.checkTithi(ja, t);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(t.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka (Day)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				Basics.Weekday wd = h2.wday;
				bool isGhataka = GhatakaDay.checkDay(ja, wd);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(wd.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka (Star)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				Nakshatra na = h2.getPosition(Body.Name.Moon).longitude.toNakshatra();
				bool isGhataka = GhatakaStar.checkStar(ja, na);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(na.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka Lagna(S)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				ZodiacHouse sa = h2.getPosition(Body.Name.Lagna).toDivisionPosition(dtype).zodiac_house;
				bool isGhataka = GhatakaLagnaSame.checkLagna(ja, sa);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(sa.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			{
				ListViewItem li = new ListViewItem("Ghataka Lagna(O)");
				ZodiacHouse ja = h.getPosition(Body.Name.Moon).toDivisionPosition(dtype).zodiac_house;
				ZodiacHouse op = h2.getPosition(Body.Name.Lagna).toDivisionPosition(dtype).zodiac_house;
				bool isGhataka = GhatakaLagnaOpp.checkLagna(ja, op);
				li.SubItems.Add(ja.ToString());
				li.SubItems.Add(op.ToString());
				li.SubItems.Add(getGhatakaString(isGhataka));
				lView.Items.Add(li);
			}
			this.ColorAndFontRows(this.lView);
		}

		private void KutaMatchingControl_Load(object sender, System.EventArgs e)
		{
		
		}

		private void bMaleChange_Click(object sender, System.EventArgs e)
		{
			ChooseHoroscopeControl f = new ChooseHoroscopeControl();
			f.ShowDialog();
			if (f.GetHorsocope() != null)
			{
				this.h.Changed -= new EvtChanged(OnRecalculate);
				this.h = f.GetHorsocope();
				this.tbHorMale.Text = f.GetHoroscopeName();
				this.h.Changed += new EvtChanged(OnRecalculate);
				this.OnRecalculate(h);
			}
			f.Dispose();
		}

		private void lView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void bFemaleChange_Click(object sender, System.EventArgs e)
		{
			ChooseHoroscopeControl f = new ChooseHoroscopeControl();
			f.ShowDialog();
			if (f.GetHorsocope() != null)
			{
				this.h2.Changed -= new EvtChanged(OnRecalculate);
				this.h2 = f.GetHorsocope();
				this.tbHorFemale.Text = f.GetHoroscopeName();
				this.h2.Changed += new EvtChanged(OnRecalculate);
				this.OnRecalculate(h2);
			}
			f.Dispose();
		}
	}
}

