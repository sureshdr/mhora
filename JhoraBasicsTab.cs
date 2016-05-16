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
	/// Summary description for JhoraBasicsTab.
	/// </summary>
	public class JhoraBasicsTab : MhoraControl
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabKeyInfo;
		private System.Windows.Forms.TabPage tabCalculations;
		private System.Windows.Forms.TabPage tabAshtakavarga;
		private System.Windows.Forms.TabPage tabNavamsaChakra;
		private System.Windows.Forms.TabPage tabYogas;
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

		//bool bTabKeyInfoLoaded = false;
		bool bTabCalculationsLoaded = false;
		bool bTabAshtakavargaLoaded = false;
		bool bTabNavamsaChakraLoaded = false;
		bool bTabYogasLoaded = false;
		public JhoraBasicsTab(Horoscope _h)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			h = _h;
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged (OnRedisplay);
			this.OnRedisplay(MhoraGlobalOptions.Instance);
			this.AddControlToTab (tabKeyInfo, new KeyInfoControl(h));
			//this.AddControlToTab (tabTest, new BalasControl(h));
			//this.AddControlToTab (tabTest, new Sarvatobhadra81Control(h));
			//this.AddControlToTab (tabTest, new KutaMatchingControl(h, h));
			//this.AddControlToTab (tabTest, new VaraChakra(h));

			this.tabControl1.TabPages[0] = tabKeyInfo;
			this.tabControl1.TabPages[1] = tabCalculations;
			this.tabControl1.TabPages[2] = tabNavamsaChakra;
			this.tabControl1.TabPages[3] = tabAshtakavarga;
			this.tabControl1.TabPages[4] = tabYogas;

			this.tabControl1.SelectedTab = tabKeyInfo;
			//this.tabControl1.SelectedTab = tabTest;
		}

		public void OnRedisplay(object o)
		{
			this.Font = MhoraGlobalOptions.Instance.GeneralFont;
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabKeyInfo = new System.Windows.Forms.TabPage();
			this.tabNavamsaChakra = new System.Windows.Forms.TabPage();
			this.tabCalculations = new System.Windows.Forms.TabPage();
			this.tabAshtakavarga = new System.Windows.Forms.TabPage();
			this.tabYogas = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tabControl1.Controls.Add(this.tabKeyInfo);
			this.tabControl1.Controls.Add(this.tabCalculations);
			this.tabControl1.Controls.Add(this.tabYogas);
			this.tabControl1.Controls.Add(this.tabNavamsaChakra);
			this.tabControl1.Controls.Add(this.tabAshtakavarga);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.Padding = new System.Drawing.Point(15, 3);
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(292, 266);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabKeyInfo
			// 
			this.tabKeyInfo.Location = new System.Drawing.Point(4, 4);
			this.tabKeyInfo.Name = "tabKeyInfo";
			this.tabKeyInfo.Size = new System.Drawing.Size(284, 238);
			this.tabKeyInfo.TabIndex = 0;
			this.tabKeyInfo.Text = "Key Info";
			this.tabKeyInfo.Click += new System.EventHandler(this.tabKeyInfo_Click);
			// 
			// tabNavamsaChakra
			// 
			this.tabNavamsaChakra.Location = new System.Drawing.Point(4, 4);
			this.tabNavamsaChakra.Name = "tabNavamsaChakra";
			this.tabNavamsaChakra.Size = new System.Drawing.Size(284, 238);
			this.tabNavamsaChakra.TabIndex = 3;
			this.tabNavamsaChakra.Text = "Chakra";
			this.tabNavamsaChakra.Click += new System.EventHandler(this.tabTest_Click);
			// 
			// tabCalculations
			// 
			this.tabCalculations.Location = new System.Drawing.Point(4, 4);
			this.tabCalculations.Name = "tabCalculations";
			this.tabCalculations.Size = new System.Drawing.Size(284, 238);
			this.tabCalculations.TabIndex = 1;
			this.tabCalculations.Text = "Calculations";
			// 
			// tabAshtakavarga
			// 
			this.tabAshtakavarga.Location = new System.Drawing.Point(4, 4);
			this.tabAshtakavarga.Name = "tabAshtakavarga";
			this.tabAshtakavarga.Size = new System.Drawing.Size(284, 238);
			this.tabAshtakavarga.TabIndex = 2;
			this.tabAshtakavarga.Text = "Ashtakavarga";
			// 
			// tabYogas
			// 
			this.tabYogas.Location = new System.Drawing.Point(4, 4);
			this.tabYogas.Name = "tabYogas";
			this.tabYogas.Size = new System.Drawing.Size(284, 238);
			this.tabYogas.TabIndex = 4;
			this.tabYogas.Text = "Yogas";
			// 
			// JhoraBasicsTab
			// 
			this.Controls.Add(this.tabControl1);
			this.Name = "JhoraBasicsTab";
			this.Size = new System.Drawing.Size(292, 266);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void tabKeyInfo_Click(object sender, System.EventArgs e)
		{
		
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TabPage tp = this.tabControl1.SelectedTab;
			if (tp == tabCalculations && this.bTabCalculationsLoaded == false)
			{
				this.AddControlToTab (tabCalculations, new BasicCalculationsControl(h));
				this.bTabCalculationsLoaded = true;
			}

			if (tp == tabAshtakavarga && this.bTabAshtakavargaLoaded == false)
			{
				this.AddControlToTab (tabAshtakavarga, new AshtakavargaControl(h));
				this.bTabAshtakavargaLoaded = true;
			}

			if (tp == tabNavamsaChakra && this.bTabNavamsaChakraLoaded == false)
			{
				this.AddControlToTab (tabNavamsaChakra, new NavamsaControl(h));
				this.bTabNavamsaChakraLoaded = true;
			}
			if (tp == tabYogas && this.bTabYogasLoaded == false)
			{
				//this.AddControlToTab(tabYogas, new YogaControl(h));
				this.bTabYogasLoaded = true;
			}

		}

		private void tabTest_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
