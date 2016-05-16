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
	/// Summary description for MhoraControl.
	/// </summary>


	public class MhoraControl : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		protected Horoscope h;
		protected Splitter sp;

		public Horoscope ControlHoroscope
		{
			get { return h; }
			set { h = value; }
		}
		/*public MhoraControl(Horoscope _h)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			_h = h;

			// TODO: Add any initialization after the InitForm call

		}*/
		public MhoraControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

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
			// 
			// MhoraControl
			// 
			this.AutoScroll = true;
			this.Name = "MhoraControl";
			this.Size = new System.Drawing.Size(360, 216);
			this.Load += new System.EventHandler(this.MhoraControl_Load);

		}
		#endregion

		
		public void ViewControl (MhoraControlContainer.BaseUserOptions.ViewType vt)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(vt);				
		}
		protected void ViewVimsottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaVimsottari);				
		}
		protected void ViewYogaVimsottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaYogaVimsottari);				
		}
		protected void ViewKaranaChaturashitiSamaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaKaranaChaturashitiSama);				
		}
		protected void ViewAshtottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaAshtottari);				
		}
		protected void ViewTithiPraveshAshtottariDasaTithi (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiPraveshAshtottariCompressedTithi);
		}
		protected void ViewTithiPraveshAshtottariDasaSolar (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiPraveshAshtottariCompressedSolar);
		}
		protected void ViewTithiPraveshAshtottariDasaFixed (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiPraveshAshtottariCompressedFixed);
		}
		protected void ViewTithiAshtottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiAshtottari);				
		}
		protected void ViewYogaPraveshVimsottariDasaYoga (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaYogaPraveshVimsottariCompressedYoga);
		}
		protected void ViewShodashottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaShodashottari);				
		}

		protected void ViewDwadashottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaDwadashottari);				
		}

		protected void ViewPanchottariDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaPanchottari);				
		}

		protected void ViewShatabdikaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaShatabdika);				
		}

		protected void ViewChaturashitiSamaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaChaturashitiSama);				
		}

		protected void ViewDwisaptatiSamaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaDwisaptatiSama);				
		}

		protected void ViewShatTrimshaSamaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaShatTrimshaSama);				
		}
		protected void ViewYoginiDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaYogini);				
		}
		protected void ViewKalachakraDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaKalachakra);				
		}
		protected void ViewNaisargikaGrahaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.NaisargikaGrahaDasa);				
		}
		protected void ViewKarakaKendradiGrahaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaKarakaKendradiGraha);				
		}
		protected void ViewMoolaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaMoola);				
		}		
		protected void ViewNaisargikaRasiDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.NaisargikaRasiDasa);				
		}
		protected void ViewNarayanaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaNarayana);				
		}
		protected void ViewNarayanaSamaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaNarayanaSama);				
		}
		protected void ViewShoolaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaShoola);				
		}
		protected void ViewNiryaanaShoolaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaNiryaanaShoola);				
		}
		protected void ViewSuDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaSu);				
		}
		protected void ViewNavamsaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaNavamsa);				
		}
		protected void ViewMandookaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaMandooka);				
		}
		protected void ViewCharaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaChara);				
		}
		protected void ViewTrikonaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTrikona);				
		}
		protected void ViewDrigDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaDrig);				
		}
		protected void ViewSudarshanaChakraDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaSudarshanaChakra);				
		}
		protected void ViewLagnaKendradiRasiDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaLagnaKendradiRasi);				
		}
		protected void ViewSudarshanaChakraDasaCompressed (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaSudarshanaChakraCompressed);				
		}
		protected void ViewMuddaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaMudda);				
		}
		protected void ViewTajakaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTajaka);				
		}
		protected void ViewTattwaDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTattwa);				
		}
		protected void ControlCopyToClipboard (object sender, System.EventArgs e)
		{
			((MhoraControl)this).copyToClipboard();
		}
		protected virtual void copyToClipboard () 
		{
		}
		protected void ViewTithiPraveshDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiPravesh);				
		}
		protected void ViewYogaPraveshDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaYogaPravesh);				
		}
		protected void ViewNakshatraPraveshDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaNakshatraPravesh);				
		}
		protected void ViewKaranaPraveshDasa (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaKaranaPravesh);				
		}
		protected void ViewKeyInfo (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.KeyInfo);				
		}
		protected void ViewBasicCalculations (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.BasicCalculations);				
		}
		protected void ViewDivisionalChart (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DivisionalChart);				
		}
		protected void ViewBalas (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.Balas);				
		}
		protected void ViewAshtakavarga (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.Ashtakavarga);				
		}
		protected void ViewKutaMatching (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.KutaMatching);				
		}
		protected void ViewNavamsaCircle (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.NavamsaCircle);				
		}
		protected void ViewVaraChakra (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.VaraChakra);				
		}
		protected void ViewSarvatobhadraChakra (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.ChakraSarvatobhadra81);				
		}
		protected void ViewTransitsSearch (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.TransitSearch);				
		}
		protected void ViewPanchanga (object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.Panchanga);				
		}
		protected void SplitViewHorizontal (object sender, System.EventArgs e)
		{
			MhoraControlContainer c_this = (MhoraControlContainer)this.Parent;
			MhoraSplitContainer c_grand = (MhoraSplitContainer)c_this.Parent;

			DivisionalChart dc1 = new DivisionalChart (h);
			MhoraControlContainer c_dc1 = new MhoraControlContainer (dc1);

			DivisionalChart dc2 = new DivisionalChart (h);
			MhoraControlContainer c_cd2 = new MhoraControlContainer (dc2);

			MhoraSplitContainer ns = new MhoraSplitContainer (c_dc1);
			ns.Control1 = c_cd2;
			ns.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;
			c_grand.Control2 = ns;
			return;

			//c_grand.Control2 = c_dc;
			//c_dc.Dock = DockStyle.Fill;
			//return;

			/*
			MhoraSplitContainer new_split = new MhoraSplitContainer (c_this);
			new_split.Control2 = c_this;
			new_split.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;
			new_split.Dock = DockStyle.Fill;

			c_grand.Control2 = new_split;
			c_grand.Show();
			*/
		}
		protected void AddViewsToContextMenu (ContextMenu cmenu)
		{

			MenuItem mBasicsMenu = new MenuItem ("Basic Info");
			mBasicsMenu.MenuItems.Add ("Key Info", new EventHandler(ViewKeyInfo));
			mBasicsMenu.MenuItems.Add ("Calculations", new EventHandler(ViewBasicCalculations));
			mBasicsMenu.MenuItems.Add ("Divisional Chart", new EventHandler(ViewDivisionalChart));
			mBasicsMenu.MenuItems.Add ("Balas", new EventHandler(ViewBalas));
			mBasicsMenu.MenuItems.Add ("Ashtakavarga", new EventHandler(ViewAshtakavarga));

			MenuItem mChakrasMenu = new MenuItem ("Chakras");
			mChakrasMenu.MenuItems.Add ("Navamsa Chakra", new EventHandler(ViewNavamsaCircle));
			mChakrasMenu.MenuItems.Add ("Vara Chakra", new EventHandler(ViewVaraChakra));
			mChakrasMenu.MenuItems.Add ("Sarvatobhadra Chakra", new EventHandler(ViewSarvatobhadraChakra));

			MenuItem mNakDasaMenu = new MenuItem("Nakshatra Dasa");
			mNakDasaMenu.MenuItems.Add ("Vimsottari Dasa", new EventHandler(ViewVimsottariDasa));
			mNakDasaMenu.MenuItems.Add ("Ashottari Dasa", new EventHandler(ViewAshtottariDasa));
			mNakDasaMenu.MenuItems.Add ("-");
			mNakDasaMenu.MenuItems.Add ("Panchottari Dasa", new EventHandler(ViewPanchottariDasa));
			mNakDasaMenu.MenuItems.Add ("Dwadashottari Dasa", new EventHandler(ViewDwadashottariDasa));
			mNakDasaMenu.MenuItems.Add ("Shodashottari Dasa", new EventHandler(ViewShodashottariDasa));
			mNakDasaMenu.MenuItems.Add ("Chaturashiti Sama Dasa", new EventHandler(ViewChaturashitiSamaDasa));
			mNakDasaMenu.MenuItems.Add ("Dwisaptati Sama Dasa", new EventHandler(ViewDwisaptatiSamaDasa));
			mNakDasaMenu.MenuItems.Add ("ShatTrimsha Sama Dasa", new EventHandler(ViewShatTrimshaSamaDasa));
			mNakDasaMenu.MenuItems.Add ("Shatabdika Dasa", new EventHandler(ViewShatabdikaDasa));
			mNakDasaMenu.MenuItems.Add ("Kalachakra Dasa", new EventHandler(ViewKalachakraDasa));
			mNakDasaMenu.MenuItems.Add ("Yogini Dasa", new EventHandler(ViewYoginiDasa));
			mNakDasaMenu.MenuItems.Add ("-");
			mNakDasaMenu.MenuItems.Add ("Tithi Ashtottari Dasa", new EventHandler(ViewTithiAshtottariDasa));
			mNakDasaMenu.MenuItems.Add ("Yoga Vimsottari Dasa", new EventHandler(ViewYogaVimsottariDasa));
			mNakDasaMenu.MenuItems.Add ("Karana Chaturashiti Sama Dasa", new EventHandler(ViewKaranaChaturashitiSamaDasa));
			
			MenuItem mGrahaDasaMenu = new MenuItem ("Graha Dasa");
			mGrahaDasaMenu.MenuItems.Add ("Naisargika Dasa", new EventHandler(ViewNaisargikaGrahaDasa));
			mGrahaDasaMenu.MenuItems.Add ("Moola Dasa", new EventHandler(ViewMoolaDasa));
			mGrahaDasaMenu.MenuItems.Add ("Karaka Kendradi Dasa", new EventHandler(ViewKarakaKendradiGrahaDasa));

			MenuItem mRasiDasaMenu = new MenuItem ("Rasi Dasa");
			mRasiDasaMenu.MenuItems.Add ("Naisargika Dasa", new EventHandler(ViewNaisargikaRasiDasa));
			mRasiDasaMenu.MenuItems.Add ("Narayana Dasa", new EventHandler (ViewNarayanaDasa));
			mRasiDasaMenu.MenuItems.Add ("Narayana Sama Dasa", new EventHandler (ViewNarayanaSamaDasa));
			mRasiDasaMenu.MenuItems.Add ("Shoola Dasa", new EventHandler (ViewShoolaDasa));
			mRasiDasaMenu.MenuItems.Add ("Niryaana Shoola Dasa", new EventHandler(ViewNiryaanaShoolaDasa));
			mRasiDasaMenu.MenuItems.Add ("Drig Dasa", new EventHandler (ViewDrigDasa));
			mRasiDasaMenu.MenuItems.Add ("Su Dasa", new EventHandler (ViewSuDasa));
			mRasiDasaMenu.MenuItems.Add ("Sudarshana Chakra Dasa", new EventHandler(ViewSudarshanaChakraDasa));
			mRasiDasaMenu.MenuItems.Add ("Lagna Kendradi Rasi Dasa", new EventHandler(ViewLagnaKendradiRasiDasa));
			mRasiDasaMenu.MenuItems.Add ("Navamsa Ayur Dasa", new EventHandler(ViewNavamsaDasa));
			mRasiDasaMenu.MenuItems.Add ("Mandooka Dasa", new EventHandler(ViewMandookaDasa));
			mRasiDasaMenu.MenuItems.Add ("Chara Dasa", new EventHandler(ViewCharaDasa));
			mRasiDasaMenu.MenuItems.Add ("Trikona Dasa", new EventHandler(ViewTrikonaDasa));

			MenuItem mRelatedChartMenu = new MenuItem ("Yearly Charts");
			mRelatedChartMenu.MenuItems.Add ("Tajaka Chart", new EventHandler (ViewTajakaDasa));
			mRelatedChartMenu.MenuItems.Add ("Sudarshana Chakra Dasa (Solar Year)", new EventHandler(ViewSudarshanaChakraDasaCompressed));
			mRelatedChartMenu.MenuItems.Add ("Mudda Dasa (Solar Year)", new EventHandler(ViewMuddaDasa));
			mRelatedChartMenu.MenuItems.Add ("-");
			mRelatedChartMenu.MenuItems.Add ("Tithi Pravesh Chart", new EventHandler (ViewTithiPraveshDasa));
			mRelatedChartMenu.MenuItems.Add ("Tithi Pravesh Ashtottari Dasa (Tithi Year)", new EventHandler(ViewTithiPraveshAshtottariDasaTithi));
			mRelatedChartMenu.MenuItems.Add ("Tithi Pravesh Ashtottari Dasa (Solar Year)", new EventHandler(ViewTithiPraveshAshtottariDasaSolar));
			mRelatedChartMenu.MenuItems.Add ("Tithi Pravesh Ashtottari Dasa (Fixed Year)", new EventHandler(ViewTithiPraveshAshtottariDasaFixed));
			mRelatedChartMenu.MenuItems.Add ("-");
			mRelatedChartMenu.MenuItems.Add ("Yoga Pravesh Chart", new EventHandler (ViewYogaPraveshDasa));
			mRelatedChartMenu.MenuItems.Add ("Yoga Pravesh Vimsottari Dasa (Yoga Year)", new EventHandler(ViewYogaPraveshVimsottariDasaYoga));
			mRelatedChartMenu.MenuItems.Add ("-");
			mRelatedChartMenu.MenuItems.Add ("Nakshatra Pravesh Chart", new EventHandler(ViewNakshatraPraveshDasa));
			mRelatedChartMenu.MenuItems.Add ("-");
			mRelatedChartMenu.MenuItems.Add ("Karana Pravesh Chart", new EventHandler(ViewKaranaPraveshDasa));
			mRelatedChartMenu.MenuItems.Add ("-");
			mRelatedChartMenu.MenuItems.Add ("Tattwa Dasa", new EventHandler (ViewTattwaDasa));


			MenuItem mOtherMenu = new MenuItem ("Other");
			mOtherMenu.MenuItems.Add ("Kuta Matching", new EventHandler(ViewKutaMatching));
			mOtherMenu.MenuItems.Add ("Transit Search", new EventHandler (ViewTransitsSearch));
			mOtherMenu.MenuItems.Add ("Panchanga", new EventHandler(ViewPanchanga));
			//MenuItem mSplitMenu = new MenuItem ("Split View");
			//mSplitMenu.MenuItems.Add ("Split Horizontal", new EventHandler(SplitViewHorizontal));

			
			cmenu.MenuItems.Add (mBasicsMenu);
			cmenu.MenuItems.Add (mChakrasMenu);
			cmenu.MenuItems.Add (mNakDasaMenu);
			cmenu.MenuItems.Add (mGrahaDasaMenu);
			cmenu.MenuItems.Add (mRasiDasaMenu);
			cmenu.MenuItems.Add (mOtherMenu);
			cmenu.MenuItems.Add (mRelatedChartMenu);

			cmenu.MenuItems.Add ("Copy To clipboard", new EventHandler (ControlCopyToClipboard));
			//cmenu.MenuItems.Add (mSplitMenu);
			
		}
		protected void MhoraControl_Load(object sender, System.EventArgs e)
		{
		}

		protected virtual void FontRows (ListView mList)
		{
			mList.ForeColor = MhoraGlobalOptions.Instance.TableForegroundColor;
			Font f = MhoraGlobalOptions.Instance.GeneralFont;
				//new Font ("Courier New", 10);
			mList.Font = f;
			foreach (ListViewItem li in mList.Items)
			{
				li.Font = f;
			}
		}
		protected virtual void ColorAndFontRows (ListView mList)
		{
			ColorRows (mList);
			FontRows (mList);
		}
		protected virtual void ColorRows (ListView mList)
		{
			Color[] cList = new Color[2];
			cList[1] = MhoraGlobalOptions.Instance.TableOddRowColor;
			cList[0] = MhoraGlobalOptions.Instance.TableEvenRowColor;
			//cList[1] = Color.WhiteSmoke;

			for (int i=0; i<mList.Items.Count; i++)
			{
				if (i%2==1) mList.Items[i].BackColor = cList[0];
				else mList.Items[i].BackColor = cList[1];
			}
			mList.BackColor = MhoraGlobalOptions.Instance.TableBackgroundColor;
		}

		private void DoNothing (Object o)
		{
		}
		/*protected void mChangeView_Click(object sender, System.EventArgs e)
		{
			MhoraControlContainer cont = (MhoraControlContainer)this.Parent;
			MhoraControlContainer.BaseUserOptions options = cont.options;
			cont.h = this.h;
			Form f = new MhoraOptions(options.Clone(), new ApplyOptions(cont.SetBaseOptions));
			f.Show();
		}*/

		private void mViewDasaVimsottari_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mDasa_Click(object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaVimsottari);				
		}

		private void mDivisionalChart_Click(object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DivisionalChart);		
		}
	}
}
