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
using System.Diagnostics;

namespace mhora
{
	/// <summary>
	/// Summary description for MhoraChild.
	/// </summary>
	public class MhoraChild : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu childMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private Horoscope h;
		public string mJhdFileName = null;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuDobOptions;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemFileSaveAs;
		private System.Windows.Forms.MenuItem menuItemFileClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuLayoutJhora;
		private System.Windows.Forms.MenuItem menuLayout2by2;
		private System.Windows.Forms.MenuItem menuLayoutTabbed;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuCalcOpts;
		private System.Windows.Forms.MenuItem menuLayout3by3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemFilePrint;
		private System.Windows.Forms.MenuItem menuItemPrintPreview;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemChartNotes;
		private System.Windows.Forms.MenuItem menuItemFileSave;
		private System.Windows.Forms.MenuItem menuStrengthOpts;
		private MhoraSplitContainer Contents;
		public MhoraChild(Horoscope _h)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			h = _h;
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
			this.childMenu = new System.Windows.Forms.MainMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemFileSave = new System.Windows.Forms.MenuItem();
			this.menuItemFileSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItemFileClose = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItemPrintPreview = new System.Windows.Forms.MenuItem();
			this.menuItemFilePrint = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItemChartNotes = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuDobOptions = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuLayoutJhora = new System.Windows.Forms.MenuItem();
			this.menuLayoutTabbed = new System.Windows.Forms.MenuItem();
			this.menuLayout2by2 = new System.Windows.Forms.MenuItem();
			this.menuLayout3by3 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuStrengthOpts = new System.Windows.Forms.MenuItem();
			this.menuCalcOpts = new System.Windows.Forms.MenuItem();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			// 
			// childMenu
			// 
			this.childMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemFile,
																					  this.menuItem1});
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemFileSave,
																						 this.menuItemFileSaveAs,
																						 this.menuItemFileClose,
																						 this.menuItem5,
																						 this.menuItemPrintPreview,
																						 this.menuItemFilePrint,
																						 this.menuItem6,
																						 this.menuItemChartNotes});
			this.menuItemFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItemFile.Text = "&File";
			// 
			// menuItemFileSave
			// 
			this.menuItemFileSave.Index = 0;
			this.menuItemFileSave.MergeOrder = 1;
			this.menuItemFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.menuItemFileSave.Text = "&Save";
			this.menuItemFileSave.Click += new System.EventHandler(this.menuItemFileSave_Click);
			// 
			// menuItemFileSaveAs
			// 
			this.menuItemFileSaveAs.Index = 1;
			this.menuItemFileSaveAs.MergeOrder = 1;
			this.menuItemFileSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.menuItemFileSaveAs.Text = "Save &As";
			this.menuItemFileSaveAs.Click += new System.EventHandler(this.menuItemFileSaveAs_Click);
			// 
			// menuItemFileClose
			// 
			this.menuItemFileClose.Index = 2;
			this.menuItemFileClose.MergeOrder = 1;
			this.menuItemFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.menuItemFileClose.Text = "&Close";
			this.menuItemFileClose.Click += new System.EventHandler(this.menuItemFileClose_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.MergeOrder = 1;
			this.menuItem5.Text = "-";
			// 
			// menuItemPrintPreview
			// 
			this.menuItemPrintPreview.Index = 4;
			this.menuItemPrintPreview.MergeOrder = 1;
			this.menuItemPrintPreview.Text = "Print Pre&view";
			this.menuItemPrintPreview.Click += new System.EventHandler(this.menuItemPrintPreview_Click);
			// 
			// menuItemFilePrint
			// 
			this.menuItemFilePrint.Index = 5;
			this.menuItemFilePrint.MergeOrder = 1;
			this.menuItemFilePrint.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuItemFilePrint.Text = "&Print";
			this.menuItemFilePrint.Click += new System.EventHandler(this.menuItemFilePrint_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 6;
			this.menuItem6.MergeOrder = 1;
			this.menuItem6.Text = "-";
			// 
			// menuItemChartNotes
			// 
			this.menuItemChartNotes.Index = 7;
			this.menuItemChartNotes.MergeOrder = 1;
			this.menuItemChartNotes.Text = "Chart Notes";
			this.menuItemChartNotes.Click += new System.EventHandler(this.menuItemChartNotes_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuDobOptions,
																					  this.menuItem2,
																					  this.menuItem3,
																					  this.menuItem4});
			this.menuItem1.MergeOrder = 1;
			this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem1.Text = "&Options";
			// 
			// menuDobOptions
			// 
			this.menuDobOptions.Index = 0;
			this.menuDobOptions.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
			this.menuDobOptions.Text = "&Birth Data && Events";
			this.menuDobOptions.Click += new System.EventHandler(this.menuDobOptions_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuLayoutJhora,
																					  this.menuLayoutTabbed,
																					  this.menuLayout2by2,
																					  this.menuLayout3by3});
			this.menuItem2.Text = "Layout";
			// 
			// menuLayoutJhora
			// 
			this.menuLayoutJhora.Index = 0;
			this.menuLayoutJhora.Text = "2 x &1";
			this.menuLayoutJhora.Click += new System.EventHandler(this.menuLayoutJhora_Click);
			// 
			// menuLayoutTabbed
			// 
			this.menuLayoutTabbed.Index = 1;
			this.menuLayoutTabbed.Text = "2 x 1 (&Tabbed)";
			this.menuLayoutTabbed.Click += new System.EventHandler(this.menuLayoutTabbed_Click);
			// 
			// menuLayout2by2
			// 
			this.menuLayout2by2.Index = 2;
			this.menuLayout2by2.Text = "&2 x 2";
			this.menuLayout2by2.Click += new System.EventHandler(this.menuLayout2by2_Click);
			// 
			// menuLayout3by3
			// 
			this.menuLayout3by3.Index = 3;
			this.menuLayout3by3.Text = "&3 x 3";
			this.menuLayout3by3.Click += new System.EventHandler(this.menuLayout3by3_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuStrengthOpts,
																					  this.menuCalcOpts});
			this.menuItem4.MergeOrder = 2;
			this.menuItem4.Text = "Advanced Options";
			// 
			// menuStrengthOpts
			// 
			this.menuStrengthOpts.Index = 0;
			this.menuStrengthOpts.MergeOrder = 2;
			this.menuStrengthOpts.Text = "Edit Chart &Strength Options";
			this.menuStrengthOpts.Click += new System.EventHandler(this.menuStrengthOpts_Click);
			// 
			// menuCalcOpts
			// 
			this.menuCalcOpts.Index = 1;
			this.menuCalcOpts.MergeOrder = 2;
			this.menuCalcOpts.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.menuCalcOpts.Text = "Edit Chart &Calculation Options";
			this.menuCalcOpts.Click += new System.EventHandler(this.menuCalcOpts_Click);
			// 
			// MhoraChild
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 329);
			this.Menu = this.childMenu;
			this.Name = "MhoraChild";
			this.Text = "MhoraChild";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MhoraChild_Closing);
			this.Load += new System.EventHandler(this.MhoraChild_Load);

		}
		#endregion

		public Horoscope getHoroscope ()
		{
			return this.h;
		}

		private void MhoraChild_Load(object sender, System.EventArgs e)
		{
			this.Contents = null;
			//this.menuLayoutJhora_Click(sender,e);
			//this.menuLayout2by2_Click(sender, e);
			this.menuLayoutTabbed_Click(sender, e);
			//this.menuLayoutJhora_Click (sender, e);
			/*
			DasaControl dc = //new BasicCalculationsControl(h);
				new DasaControl(h, new VimsottariDasa(h));
			MhoraControlContainer c_dc = new MhoraControlContainer(dc);

			DivisionalChart div_rasi = new DivisionalChart(h);
			MhoraControlContainer c_div_rasi = new MhoraControlContainer(div_rasi);

			DivisionalChart div_nav = new DivisionalChart(h);
			div_nav.options.Division = Basics.DivisionType.Navamsa;
			MhoraControlContainer c_div_nav = new MhoraControlContainer(div_nav);


			MhoraSplitContainer sp_ud = new MhoraSplitContainer(c_div_rasi);
			sp_ud.Control2 = c_div_nav;
			sp_ud.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_dc = new MhoraSplitContainer(c_dc);
			
			MhoraSplitContainer sp_lr = new MhoraSplitContainer(sp_ud);
			sp_lr.Control2 = sp_dc;

			int sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - 50;
			sp_lr.Control1.Width = sz;
			sp_ud.Control1.Height = sz;
			//sp_lr.Control1.Height = 300;
			//sp_lr.Control2.Width = 300;
			//sp_lr.Control2.Height = 300;
			




			this.Controls.AddRange(new Control[]{sp_lr});

			DivisionalChart ds = new DivisionalChart(h);
			Splitter sp = new Splitter();
			VimsottariDasa vd1 = new VimsottariDasa(h);
			//VimsottariDasa vd2 = new VimsottariDasa(h);
			//vd2.options.SeedBody = VimsottariDasa.UserOptions.StartBodyType.Moon;
			//vd2.options.start_graha = Body.Name.Moon;
			//vd2.options.start_graha = Body.Name.Moon;
			DasaControl dc1 = new DasaControl(h, vd1,sp);
			//DasaControl dc2 = new DasaControl(h, vd2);

			sp.Dock = DockStyle.Top;
			dc1.Dock = DockStyle.Top;
			ds.Dock = DockStyle.Fill;

			//dc2.Dock = DockStyle.Fill;
			this.Controls.AddRange(new Control[]{ds, sp, dc1});
			sp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
*/
		}

		private void rtOutput_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void DoNothing (object a)
		{
		}

		public void menuShowDobOptions ()
		{
			MhoraOptions f = new MhoraOptions (h.info.Clone(), new ApplyOptions(h.UpdateHoraInfo));
			f.ShowDialog();
		}
		private void menuDobOptions_Click(object sender, System.EventArgs e)
		{
			this.menuShowDobOptions();
			//object wrapper = new GlobalizedPropertiesWrapper((HoraInfo)h.info.Clone());
		}

		public void saveJhdFile ()
		{
			if (this.mJhdFileName == null || this.mJhdFileName.Length == 0)
				this.saveAsJhdFile();
			try
			{
				if (h.info.FileType == HoraInfo.EFileType.JagannathaHora)
					new Jhd(this.mJhdFileName).ToFile(h.info);
				else
					new Mhd(this.mJhdFileName).ToFile(h.info);
			}
			catch (System.ArgumentNullException)
			{
			}
			catch 
			{
				MessageBox.Show(this, "Error Saving File", "Error", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		public void saveAsJhdFile ()
		{
			SaveFileDialog ofd = new SaveFileDialog();
			ofd.AddExtension = true;
			ofd.Filter = "Jagannatha Hora Files (*.jhd)|*.jhd|Mudgala Hora Files (*.mhd)|*.mhd";
			ofd.FilterIndex = 1;

			if (ofd.ShowDialog() != DialogResult.OK)
				return;
			if (ofd.FileName.Length == 0)
				return;

			string[] sparts = ofd.FileName.ToLower().Split('.');
			try 
			{
				if (sparts[sparts.Length-1] == "jhd")
				{
					h.info.FileType = HoraInfo.EFileType.JagannathaHora;
					new Jhd(ofd.FileName).ToFile(h.info);
				}
				else
				{
					h.info.FileType = HoraInfo.EFileType.MudgalaHora;
					new Mhd(ofd.FileName).ToFile(h.info);
				}

				this.mJhdFileName = ofd.FileName;
			}
			catch (System.ArgumentNullException)
			{
			}
			//catch 
			//{
			//	MessageBox.Show(this, "Error Saving File", "Error", 
			//		MessageBoxButtons.OK, MessageBoxIcon.Error);
			//}
		}

		private void menuItemFileSaveAs_Click(object sender, System.EventArgs e)
		{
			saveAsJhdFile();
		
		}

		private void menuItemFileSave_Click(object sender, System.EventArgs e)
		{
			saveJhdFile();		
		}


		private void menuItemFileClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private void menuLayoutJhora_Click(object sender, System.EventArgs e)
		{
			if (Contents != null)
				this.Controls.Remove (Contents);

			DasaControl dc = new DasaControl(h, new VimsottariDasa(h));
			MhoraControlContainer c_dc = new MhoraControlContainer(dc);

			DivisionalChart div_rasi = new DivisionalChart(h);
			MhoraControlContainer c_div_rasi = new MhoraControlContainer(div_rasi);

			DivisionalChart div_nav = new DivisionalChart(h);
			div_nav.options.Varga = new Division(Basics.DivisionType.Navamsa);
			div_nav.SetOptions (div_nav.options);
			MhoraControlContainer c_div_nav = new MhoraControlContainer(div_nav);


			MhoraSplitContainer sp_ud = new MhoraSplitContainer(c_div_rasi);
			sp_ud.Control2 = c_div_nav;
			sp_ud.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_dc = new MhoraSplitContainer(c_dc);
			
			MhoraSplitContainer sp_lr = new MhoraSplitContainer(sp_ud);
			sp_lr.Control2 = sp_dc;

			int sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - 50;
			sp_lr.Control1.Width = sz;
			sp_ud.Control1.Height = sz;
			this.Controls.AddRange(new Control[]{sp_lr});
			Contents = sp_lr;
		
		}

		private void menuLayoutTabbed_Click(object sender, System.EventArgs e)
		{
			if (Contents != null)
				this.Controls.Remove (Contents);

			MhoraControl mc = new JhoraMainTab(h);
			//DasaControl dc = new DasaControl(h, new VimsottariDasa(h));
			MhoraControlContainer c_dc = new MhoraControlContainer(mc);

			DivisionalChart div_rasi = new DivisionalChart(h);
			MhoraControlContainer c_div_rasi = new MhoraControlContainer(div_rasi);

			DivisionalChart div_nav = new DivisionalChart(h);
			div_nav.options.Varga = new Division(Basics.DivisionType.Navamsa);
			div_nav.SetOptions(div_nav.options);
			MhoraControlContainer c_div_nav = new MhoraControlContainer(div_nav);


			MhoraSplitContainer sp_ud = new MhoraSplitContainer(c_div_rasi);
			sp_ud.Control2 = c_div_nav;
			sp_ud.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_dc = new MhoraSplitContainer(c_dc);
			
			MhoraSplitContainer sp_lr = new MhoraSplitContainer(sp_ud);
			sp_lr.Control2 = sp_dc;

			int sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - 50;
			sp_lr.Control1.Width = sz;
			sp_ud.Control1.Height = sz;
			this.Controls.AddRange(new Control[]{sp_lr});
			Contents = sp_lr;
	
		}

		private void menuLayout2by2_Click(object sender, System.EventArgs e)
		{
			if (Contents != null)
				this.Controls.Remove (Contents);

			DasaControl dc1 = new DasaControl(h, new VimsottariDasa(h));
			MhoraControlContainer c_dc1 = new MhoraControlContainer(dc1);

			BasicCalculationsControl dc2 = new BasicCalculationsControl(h);
			MhoraControlContainer c_dc2 = new MhoraControlContainer(dc2);

			DivisionalChart div_rasi = new DivisionalChart(h);
			MhoraControlContainer c_div_rasi = new MhoraControlContainer(div_rasi);

			DivisionalChart div_nav = new DivisionalChart(h);
			div_nav.options.Varga = new Division(Basics.DivisionType.Navamsa);
			div_nav.SetOptions(div_nav.options);
			MhoraControlContainer c_div_nav = new MhoraControlContainer(div_nav);


			MhoraSplitContainer sp_ud = new MhoraSplitContainer(c_div_rasi);
			sp_ud.Control2 = c_div_nav;
			sp_ud.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_ud2 = new MhoraSplitContainer(c_dc1);
			sp_ud2.Control2 = c_dc2;
			sp_ud2.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_dc = new MhoraSplitContainer(sp_ud2);
			
			MhoraSplitContainer sp_lr = new MhoraSplitContainer(sp_ud);
			sp_lr.Control2 = sp_dc;

			int sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - 50;
			sp_lr.Control1.Width = sz;
			sp_ud.Control1.Height = sz;
			sp_ud2.Control1.Height = sz;
			this.Controls.AddRange(new Control[]{sp_lr});
			Contents = sp_lr;
		
		}

		private void MhoraChild_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//this.Close();
			//this.Dispose();
		}

		public object SetCalcOptions (Object o)
		{
			HoroscopeOptions ho = (HoroscopeOptions)o;
			h.options.Copy(ho);
			h.OnChanged();
			return h.options.Clone();
		}
		public object SetStrengthOptions (Object o)
		{
			StrengthOptions so = (StrengthOptions)o;
			h.strength_options.Copy(so);
			h.OnChanged();
			return h.strength_options.Clone();
		}
		private void menuCalcOpts_Click(object sender, System.EventArgs e)
		{
			MhoraOptions f = new MhoraOptions(h.options, new ApplyOptions(this.SetCalcOptions));
			f.ShowDialog();
		}

		
		private void menuStrengthOpts_Click(object sender, System.EventArgs e)
		{
			if (this.h.strength_options == null)
				this.h.strength_options = (StrengthOptions)MhoraGlobalOptions.Instance.SOptions.Clone();

			MhoraOptions f = new MhoraOptions(h.strength_options, new ApplyOptions(this.SetStrengthOptions));
			f.ShowDialog();
		}

		private void menuLayout3by3_Click(object sender, System.EventArgs e)
		{
			if (Contents != null)
				this.Controls.Remove (Contents);

			DivisionalChart d1 = new DivisionalChart(h);
			MhoraControlContainer c_d1 = new MhoraControlContainer(d1);

			DivisionalChart d2 = new DivisionalChart(h);
			d2.options.Varga = new Division(Basics.DivisionType.DrekkanaParasara);
			d2.SetOptions(d2.options);
			MhoraControlContainer c_d2 = new MhoraControlContainer(d2);

			DivisionalChart d3 = new DivisionalChart(h);
			d3.options.Varga = new Division(Basics.DivisionType.Navamsa);
			d3.SetOptions(d3.options);
			MhoraControlContainer c_d3 = new MhoraControlContainer(d3);

			DivisionalChart d4 = new DivisionalChart(h);
			d4.options.Varga = new Division(Basics.DivisionType.Saptamsa);
			d4.SetOptions(d4.options);
			MhoraControlContainer c_d4 = new MhoraControlContainer(d4);

			DivisionalChart d5 = new DivisionalChart(h);
			d5.options.Varga = new Division(Basics.DivisionType.Dasamsa);
			d5.SetOptions(d5.options);
			MhoraControlContainer c_d5 = new MhoraControlContainer(d5);

			DivisionalChart d6 = new DivisionalChart(h);
			d6.options.Varga = new Division(Basics.DivisionType.Vimsamsa);
			d6.SetOptions(d6.options);
			MhoraControlContainer c_d6 = new MhoraControlContainer(d6);


			MhoraSplitContainer sp_ud1 = new MhoraSplitContainer(c_d1);
			sp_ud1.Control2 = c_d2;
			sp_ud1.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_ud2 = new MhoraSplitContainer(c_d3);
			sp_ud2.Control2 = c_d4;
			sp_ud2.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;

			MhoraSplitContainer sp_ud3 = new MhoraSplitContainer(c_d5);
			sp_ud3.Control2 = c_d6;
			sp_ud3.DrawDock = MhoraSplitContainer.DrawStyle.UpDown;



			MhoraSplitContainer lr2 = new MhoraSplitContainer(sp_ud2);
			lr2.Control2 = sp_ud3;


			MhoraSplitContainer lr1 = new MhoraSplitContainer(sp_ud1);
			lr1.Control2 = lr2;


			int h_sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - 30;
			int w_sz = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 3 - 30;
			int sz = Math.Min(h_sz, w_sz);
			lr1.Control1.Width = sz;
			lr2.Control1.Width = sz;
			sp_ud1.Control1.Height = sz;
			sp_ud2.Control1.Height = sz;
			sp_ud3.Control1.Height = sz;

			this.Controls.AddRange(new Control[]{lr1});
			Contents = lr1;
		
		}

		public object OnCalcOptsChanged (object o)
		{
			h.options.Copy((HoroscopeOptions)o);
			h.OnChanged();
			return h.options.Clone();
		}

		private void menuEditCalcOpts_Click(object sender, System.EventArgs e)
		{
			new MhoraOptions(this.h.options, new ApplyOptions(OnCalcOptsChanged)).ShowDialog();
		}

		public void menuPrint ()
		{
			MhoraPrintDocument mdoc = new MhoraPrintDocument(this.h);
			PrintDialog dlgPrint = new PrintDialog();
			dlgPrint.Document = mdoc;

			if (dlgPrint.ShowDialog() == DialogResult.OK)
				mdoc.Print();
		}
		private void menuItemFilePrint_Click(object sender, System.EventArgs e)
		{
			this.menuPrint();
		}

		public void menuPrintPreview ()
		{
			MhoraPrintDocument mdoc = new MhoraPrintDocument(this.h);
			PrintPreviewDialog dlgPreview = new PrintPreviewDialog();
			dlgPreview.Document = mdoc;
			dlgPreview.ShowDialog();
		}
		private void menuItemPrintPreview_Click(object sender, System.EventArgs e)
		{
			this.menuPrintPreview();
		}

		private void menuItemChartNotes_Click(object sender, System.EventArgs e)
		{

			if (null == this.mJhdFileName || this.mJhdFileName.Length == 0)
			{
				MessageBox.Show("Please save the chart before editing notes");
				return;
			}

			System.IO.FileInfo fi = new System.IO.FileInfo(this.mJhdFileName);
			string ext = fi.Extension;

			string sfBase = new string(this.mJhdFileName.ToCharArray(), 0, this.mJhdFileName.Length - ext.Length);
			string sfExt = MhoraGlobalOptions.Instance.ChartNotesFileExtension;
			string sfName = sfBase;

			if (sfExt.Length > 0 && sfExt[0] == '.')
				sfName += sfExt;
			else 
				sfName += "." + sfExt;

			try 
			{
				if (false == System.IO.File.Exists(sfName))
					System.IO.File.Create(sfName).Close();
				Process.Start(sfName);

			}
			catch 
			{
				MessageBox.Show(string.Format("An error occurred. Unable to open file {0}", sfName));
			}
			
		}

		private void menuItemEvalYogas_Click(object sender, System.EventArgs e)
		{
			//this.evaluateYogas();
			//FindYogas.Test(h, new Division(Basics.DivisionType.Rasi));
		}
	}

}
