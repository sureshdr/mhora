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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace mhora
{
	/// <summary>
	/// Summary description for JhoraMainTab.
	/// </summary>
	public class JhoraMainTab : MhoraControl
	{
		private System.Windows.Forms.TabControl mTab;
		private System.Windows.Forms.TabPage tabDasa;
		private System.Windows.Forms.TabPage tabBasics;
		private System.Windows.Forms.TabPage tabVargas;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.TabPage tabTransits;

		bool bTabDasaLoaded = false;
		//bool bTabBasicsLoaded = false;
		bool bTabVargasLoaded = false;
		//bool bTabTajakaLoaded = false;
		bool bTabTithiPraveshLoaded = false;
		bool bTabTransitsLoaded = false;
		bool bTabPanchangaLoaded = false;

		private System.Windows.Forms.TabPage tabTithiPravesh;
		private System.Windows.Forms.TabPage tabPanchanga;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void AddControlToTab (TabPage tab, MhoraControl mcontrol)
		{
			MhoraControlContainer container = new MhoraControlContainer (mcontrol);
			container.Dock = DockStyle.Fill;
			tab.Controls.Add (container);
		}
		public JhoraMainTab(Horoscope _h)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mTab.TabPages[0] = tabBasics;
			this.mTab.TabPages[1] = tabVargas;
			this.mTab.TabPages[2] = tabDasa;
			this.mTab.TabPages[3] = tabTransits;
			this.mTab.TabPages[4] = tabPanchanga;
			this.mTab.TabPages[5] = tabTithiPravesh;

			this.mTab.SelectedTab = tabBasics;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			h = _h;
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnRedisplay);
			this.OnRedisplay(MhoraGlobalOptions.Instance);

			this.AddControlToTab (tabBasics, new JhoraBasicsTab(h));
			//this.bTabBasicsLoaded = true;
		}

		public void OnRedisplay (object o)
		{
			this.Font = MhoraGlobalOptions.Instance.GeneralFont;
			/*
			this.tabBasics.Font = this.Font;
			this.tabDasa.Font = this.Font;
			this.tabPanchanga.Font = this.Font;
			this.tabTithiPravesh.Font = this.Font;
			this.tabTransits.Font = this.Font;
			this.tabVargas.Font = this.Font;
			*/
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mTab = new System.Windows.Forms.TabControl();
			this.tabBasics = new System.Windows.Forms.TabPage();
			this.tabTransits = new System.Windows.Forms.TabPage();
			this.tabDasa = new System.Windows.Forms.TabPage();
			this.tabVargas = new System.Windows.Forms.TabPage();
			this.tabPanchanga = new System.Windows.Forms.TabPage();
			this.tabTithiPravesh = new System.Windows.Forms.TabPage();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.mTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// mTab
			// 
			this.mTab.Controls.Add(this.tabBasics);
			this.mTab.Controls.Add(this.tabDasa);
			this.mTab.Controls.Add(this.tabTransits);
			this.mTab.Controls.Add(this.tabVargas);
			this.mTab.Controls.Add(this.tabPanchanga);
			this.mTab.Controls.Add(this.tabTithiPravesh);
			this.mTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mTab.Location = new System.Drawing.Point(0, 0);
			this.mTab.Name = "mTab";
			this.mTab.Padding = new System.Drawing.Point(20, 4);
			this.mTab.SelectedIndex = 0;
			this.mTab.Size = new System.Drawing.Size(472, 256);
			this.mTab.TabIndex = 0;
			this.mTab.SelectedIndexChanged += new System.EventHandler(this.mTab_SelectedIndexChanged);
			// 
			// tabBasics
			// 
			this.tabBasics.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
			this.tabBasics.Location = new System.Drawing.Point(4, 25);
			this.tabBasics.Name = "tabBasics";
			this.tabBasics.Size = new System.Drawing.Size(464, 227);
			this.tabBasics.TabIndex = 1;
			this.tabBasics.Text = "Basics";
			this.tabBasics.Click += new System.EventHandler(this.tabBasics_Click);
			// 
			// tabTransits
			// 
			this.tabTransits.Location = new System.Drawing.Point(4, 25);
			this.tabTransits.Name = "tabTransits";
			this.tabTransits.Size = new System.Drawing.Size(464, 227);
			this.tabTransits.TabIndex = 4;
			this.tabTransits.Text = "Transits";
			this.tabTransits.Click += new System.EventHandler(this.tabTransits_Click);
			// 
			// tabDasa
			// 
			this.tabDasa.Location = new System.Drawing.Point(4, 25);
			this.tabDasa.Name = "tabDasa";
			this.tabDasa.Size = new System.Drawing.Size(464, 227);
			this.tabDasa.TabIndex = 0;
			this.tabDasa.Text = "Dasas";
			this.tabDasa.Click += new System.EventHandler(this.tabDasa_Click);
			// 
			// tabVargas
			// 
			this.tabVargas.Location = new System.Drawing.Point(4, 25);
			this.tabVargas.Name = "tabVargas";
			this.tabVargas.Size = new System.Drawing.Size(464, 227);
			this.tabVargas.TabIndex = 2;
			this.tabVargas.Text = "Varga";
			this.tabVargas.Click += new System.EventHandler(this.tabVargas_Click);
			// 
			// tabPanchanga
			// 
			this.tabPanchanga.Location = new System.Drawing.Point(4, 25);
			this.tabPanchanga.Name = "tabPanchanga";
			this.tabPanchanga.Size = new System.Drawing.Size(464, 227);
			this.tabPanchanga.TabIndex = 6;
			this.tabPanchanga.Text = "Panchanga";
			this.tabPanchanga.Click += new System.EventHandler(this.tabPanchanga_Click);
			// 
			// tabTithiPravesh
			// 
			this.tabTithiPravesh.Location = new System.Drawing.Point(4, 25);
			this.tabTithiPravesh.Name = "tabTithiPravesh";
			this.tabTithiPravesh.Size = new System.Drawing.Size(464, 227);
			this.tabTithiPravesh.TabIndex = 5;
			this.tabTithiPravesh.Text = "TithiPravesh";
			// 
			// JhoraMainTab
			// 
			this.Controls.Add(this.mTab);
			this.Name = "JhoraMainTab";
			this.Size = new System.Drawing.Size(472, 256);
			this.mTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void tabBasics_Click(object sender, System.EventArgs e)
		{
		}

		private void tabTransits_Click(object sender, System.EventArgs e)
		{
		
		}

		private void tabVargas_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mTab_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.mTab.SelectedTab == tabTransits && this.bTabTransitsLoaded == false)
			{
				this.AddControlToTab (tabTransits, new TransitSearch(h));
				this.bTabTransitsLoaded = true;
				return;
			}

			if (this.mTab.SelectedTab == tabDasa && this.bTabDasaLoaded == false)
			{
				MhoraControl mc = new MhoraControl();
				this.AddControlToTab(tabDasa, mc);

				//MhoraControlContainer mcc = new MhoraControlContainer(mc);
				mc.ControlHoroscope = h;
				switch (h.info.type)
				{
					case HoraInfo.Name.TithiPravesh:
						mc.ViewControl(MhoraControlContainer.BaseUserOptions.ViewType.DasaTithiPraveshAshtottariCompressedTithi);
						break;
					default:
						mc.ViewControl(MhoraControlContainer.BaseUserOptions.ViewType.DasaVimsottari);
						break;
				}
				this.bTabDasaLoaded = true;
				return;
			}

			if (this.mTab.SelectedTab == tabVargas && this.bTabVargasLoaded == false)
			{
				this.AddControlToTab (tabVargas, new DivisionalChart(h));
				this.bTabVargasLoaded = true;
			}

			if (this.mTab.SelectedTab == tabTransits && this.bTabTransitsLoaded == false)
			{
				this.AddControlToTab (tabTransits, new TransitSearch(h));
				this.bTabTransitsLoaded = true;
			}

			if (this.mTab.SelectedTab == tabTithiPravesh && this.bTabTithiPraveshLoaded == false)
			{
				DasaControl dc = new DasaControl(h, new TithiPraveshDasa(h));
				dc.LinkToHoroscope = false;
				dc.DasaOptions.YearType = ToDate.DateType.TithiPraveshYear;
				dc.Reset();
				this.AddControlToTab(tabTithiPravesh, dc);
				this.bTabTithiPraveshLoaded = true;
			}

			//if (this.mTab.SelectedTab == tabTajaka && this.bTabTajakaLoaded == false)
			//{
			//	this.AddControlToTab(tabTajaka, new DasaControl(h, new TajakaDasa(h)));
			//	this.bTabTajakaLoaded = true;		
			//}
			if (this.mTab.SelectedTab == tabPanchanga && this.bTabPanchangaLoaded == false)
			{
				this.AddControlToTab(tabPanchanga, new PanchangaControl(h));
				this.bTabPanchangaLoaded = true;
			}

		}

		private void tabPanchanga_Click(object sender, System.EventArgs e)
		{
		
		}

		private void tabDasa_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
