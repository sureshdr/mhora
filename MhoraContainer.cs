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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace mhora
{
	/// <summary>
	/// Summary description for MhoraContainer.
	/// </summary>
	public class MhoraContainer : System.Windows.Forms.Form
	{
		public MhoraGlobalOptions gOpts;

		private int childCount;
		private System.Windows.Forms.MainMenu MdiMenu;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemFileOpen;
		private System.Windows.Forms.MenuItem menuItemFileExit;
		private System.Windows.Forms.MenuItem menuItemWindow;
		private System.Windows.Forms.MenuItem menuItemWindowTile;
		private System.Windows.Forms.MenuItem menuItemWindowCascade;
		private System.Windows.Forms.MenuItem menuItemHelpAboutMhora;
		private System.Windows.Forms.MenuItem menuItemHelpAboutSjc;
		private System.Windows.Forms.MenuItem menuItemHelp;
		private System.Windows.Forms.StatusBar MdiStatusBar;
		private System.Windows.Forms.ToolBar MdiToolBar;
		private System.Windows.Forms.ImageList MdiImageList;
		private System.Windows.Forms.MenuItem menuItemFileNew;
		private System.Windows.Forms.MenuItem menuItemFileNewPrasna;
		private System.Windows.Forms.MenuItem mViewPreferences;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mSavePreferences;
		private System.Windows.Forms.MenuItem menuItemAdvanced;
		private System.Windows.Forms.MenuItem menuItemFindCharts;
		private System.Windows.Forms.MenuItem mResetPreferences;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemNewView;
		private System.Windows.Forms.MenuItem menuItemSplash;
		private System.Windows.Forms.ToolBarButton toolbarButtonNew;
		private System.Windows.Forms.ToolBarButton toolbarButtonOpen;
		private System.Windows.Forms.ToolBarButton toolbarButtonSave;
		private System.Windows.Forms.ToolBarButton toolbarButtonHelp;
		private System.Windows.Forms.MenuItem mIncreaseFontSize;
		private System.Windows.Forms.MenuItem mDecreaseFontSize;
		private System.Windows.Forms.MenuItem mEditCalcPrefs;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mEditStrengthPrefs;
		private System.Windows.Forms.MenuItem mResetStrengthPreferences;
		private System.Windows.Forms.ToolBarButton toolbarButtonPrint;
		private System.Windows.Forms.ToolBarButton toolbarButtonPreview;
		private System.Windows.Forms.ToolBarButton toolbarButtonDob;
		private System.Windows.Forms.ToolBarButton toolbarButtonDisp;
		private System.ComponentModel.IContainer components;

		public MhoraContainer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			childCount = 0;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			
			Application.Run(new MhoraContainer());
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MhoraContainer));
			this.MdiMenu = new System.Windows.Forms.MainMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemFileNew = new System.Windows.Forms.MenuItem();
			this.menuItemFileNewPrasna = new System.Windows.Forms.MenuItem();
			this.menuItemFileOpen = new System.Windows.Forms.MenuItem();
			this.menuItemFileExit = new System.Windows.Forms.MenuItem();
			this.mViewPreferences = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mEditStrengthPrefs = new System.Windows.Forms.MenuItem();
			this.mEditCalcPrefs = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mResetPreferences = new System.Windows.Forms.MenuItem();
			this.mResetStrengthPreferences = new System.Windows.Forms.MenuItem();
			this.mSavePreferences = new System.Windows.Forms.MenuItem();
			this.mIncreaseFontSize = new System.Windows.Forms.MenuItem();
			this.mDecreaseFontSize = new System.Windows.Forms.MenuItem();
			this.menuItemWindow = new System.Windows.Forms.MenuItem();
			this.menuItemWindowTile = new System.Windows.Forms.MenuItem();
			this.menuItemWindowCascade = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemNewView = new System.Windows.Forms.MenuItem();
			this.menuItemAdvanced = new System.Windows.Forms.MenuItem();
			this.menuItemFindCharts = new System.Windows.Forms.MenuItem();
			this.menuItemHelp = new System.Windows.Forms.MenuItem();
			this.menuItemHelpAboutMhora = new System.Windows.Forms.MenuItem();
			this.menuItemHelpAboutSjc = new System.Windows.Forms.MenuItem();
			this.menuItemSplash = new System.Windows.Forms.MenuItem();
			this.MdiStatusBar = new System.Windows.Forms.StatusBar();
			this.MdiToolBar = new System.Windows.Forms.ToolBar();
			this.toolbarButtonNew = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonOpen = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonSave = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonPrint = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonPreview = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonDob = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonDisp = new System.Windows.Forms.ToolBarButton();
			this.toolbarButtonHelp = new System.Windows.Forms.ToolBarButton();
			this.MdiImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// MdiMenu
			// 
			this.MdiMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItemFile,
																					this.mViewPreferences,
																					this.menuItemWindow,
																					this.menuItemAdvanced,
																					this.menuItemHelp});
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemFileNew,
																						 this.menuItemFileNewPrasna,
																						 this.menuItemFileOpen,
																						 this.menuItemFileExit});
			this.menuItemFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItemFile.Text = "&File";
			// 
			// menuItemFileNew
			// 
			this.menuItemFileNew.Index = 0;
			this.menuItemFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.menuItemFileNew.Text = "&New";
			this.menuItemFileNew.Click += new System.EventHandler(this.menuItemFileNew_Click);
			// 
			// menuItemFileNewPrasna
			// 
			this.menuItemFileNewPrasna.Index = 1;
			this.menuItemFileNewPrasna.Text = "New &Prasna";
			this.menuItemFileNewPrasna.Click += new System.EventHandler(this.menuItemFileNewPrasna_Click);
			// 
			// menuItemFileOpen
			// 
			this.menuItemFileOpen.Index = 2;
			this.menuItemFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.menuItemFileOpen.Text = "&Open";
			this.menuItemFileOpen.Click += new System.EventHandler(this.menuItemFileOpen_Click);
			// 
			// menuItemFileExit
			// 
			this.menuItemFileExit.Index = 3;
			this.menuItemFileExit.MergeOrder = 2;
			this.menuItemFileExit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ;
			this.menuItemFileExit.Text = "E&xit";
			this.menuItemFileExit.Click += new System.EventHandler(this.menuItemFileExit_Click);
			// 
			// mViewPreferences
			// 
			this.mViewPreferences.Index = 1;
			this.mViewPreferences.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menuItem4,
																							 this.mEditStrengthPrefs,
																							 this.mEditCalcPrefs,
																							 this.menuItem2,
																							 this.mResetPreferences,
																							 this.mResetStrengthPreferences,
																							 this.mSavePreferences,
																							 this.mIncreaseFontSize,
																							 this.mDecreaseFontSize});
			this.mViewPreferences.MergeOrder = 1;
			this.mViewPreferences.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.mViewPreferences.Text = "&Options";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.MergeOrder = 1;
			this.menuItem4.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuItem4.Text = "Edit Global Display Options";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// mEditStrengthPrefs
			// 
			this.mEditStrengthPrefs.Index = 1;
			this.mEditStrengthPrefs.MergeOrder = 1;
			this.mEditStrengthPrefs.Text = "Edit Global Strength Options";
			this.mEditStrengthPrefs.Click += new System.EventHandler(this.mEditStrengthPrefs_Click);
			// 
			// mEditCalcPrefs
			// 
			this.mEditCalcPrefs.Index = 2;
			this.mEditCalcPrefs.MergeOrder = 1;
			this.mEditCalcPrefs.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.mEditCalcPrefs.Text = "Edit Global Calculation Options";
			this.mEditCalcPrefs.Click += new System.EventHandler(this.mEditCalcPrefs_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.MergeOrder = 3;
			this.menuItem2.Text = "-";
			// 
			// mResetPreferences
			// 
			this.mResetPreferences.Index = 4;
			this.mResetPreferences.MergeOrder = 3;
			this.mResetPreferences.Text = "Reset All Options";
			this.mResetPreferences.Click += new System.EventHandler(this.mResetPreferences_Click);
			// 
			// mResetStrengthPreferences
			// 
			this.mResetStrengthPreferences.Index = 5;
			this.mResetStrengthPreferences.MergeOrder = 3;
			this.mResetStrengthPreferences.Text = "Reset Strength Options";
			this.mResetStrengthPreferences.Click += new System.EventHandler(this.mResetStrengthPreferences_Click);
			// 
			// mSavePreferences
			// 
			this.mSavePreferences.Index = 6;
			this.mSavePreferences.MergeOrder = 3;
			this.mSavePreferences.Text = "Save Options";
			this.mSavePreferences.Click += new System.EventHandler(this.mSavePreferences_Click);
			// 
			// mIncreaseFontSize
			// 
			this.mIncreaseFontSize.Index = 7;
			this.mIncreaseFontSize.MergeOrder = 3;
			this.mIncreaseFontSize.Text = "+ Font Size";
			this.mIncreaseFontSize.Click += new System.EventHandler(this.mIncreaseFontSize_Click);
			// 
			// mDecreaseFontSize
			// 
			this.mDecreaseFontSize.Index = 8;
			this.mDecreaseFontSize.MergeOrder = 3;
			this.mDecreaseFontSize.Text = "- Size Size";
			this.mDecreaseFontSize.Click += new System.EventHandler(this.mDecreaseFontSize_Click);
			// 
			// menuItemWindow
			// 
			this.menuItemWindow.Index = 2;
			this.menuItemWindow.MdiList = true;
			this.menuItemWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemWindowTile,
																						   this.menuItemWindowCascade,
																						   this.menuItem1,
																						   this.menuItemNewView});
			this.menuItemWindow.MergeOrder = 2;
			this.menuItemWindow.Text = "&Window";
			// 
			// menuItemWindowTile
			// 
			this.menuItemWindowTile.Index = 0;
			this.menuItemWindowTile.Text = "&Tile";
			this.menuItemWindowTile.Click += new System.EventHandler(this.menuItemWindowTile_Click);
			// 
			// menuItemWindowCascade
			// 
			this.menuItemWindowCascade.Index = 1;
			this.menuItemWindowCascade.Text = "&Cascade";
			this.menuItemWindowCascade.Click += new System.EventHandler(this.menuItemWindowCascade_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItemNewView
			// 
			this.menuItemNewView.Index = 3;
			this.menuItemNewView.Text = "&New View";
			this.menuItemNewView.Click += new System.EventHandler(this.menuItemNewView_Click);
			// 
			// menuItemAdvanced
			// 
			this.menuItemAdvanced.Enabled = false;
			this.menuItemAdvanced.Index = 3;
			this.menuItemAdvanced.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menuItemFindCharts});
			this.menuItemAdvanced.MergeOrder = 2;
			this.menuItemAdvanced.Text = "Advanced";
			// 
			// menuItemFindCharts
			// 
			this.menuItemFindCharts.Index = 0;
			this.menuItemFindCharts.Text = "Find Charts";
			this.menuItemFindCharts.Click += new System.EventHandler(this.menuItemFindCharts_Click);
			// 
			// menuItemHelp
			// 
			this.menuItemHelp.Index = 4;
			this.menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemHelpAboutMhora,
																						 this.menuItemHelpAboutSjc,
																						 this.menuItemSplash});
			this.menuItemHelp.MergeOrder = 2;
			this.menuItemHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.menuItemHelp.Text = "&Help";
			// 
			// menuItemHelpAboutMhora
			// 
			this.menuItemHelpAboutMhora.Index = 0;
			this.menuItemHelpAboutMhora.Text = "&About Mudgala Hora";
			this.menuItemHelpAboutMhora.Click += new System.EventHandler(this.menuItemHelpAboutMhora_Click);
			// 
			// menuItemHelpAboutSjc
			// 
			this.menuItemHelpAboutSjc.Index = 1;
			this.menuItemHelpAboutSjc.Text = "About &SJC";
			this.menuItemHelpAboutSjc.Click += new System.EventHandler(this.menuItemHelpAboutSjc_Click);
			// 
			// menuItemSplash
			// 
			this.menuItemSplash.Index = 2;
			this.menuItemSplash.Text = "&View Splash Screen";
			this.menuItemSplash.Click += new System.EventHandler(this.menuItemSplash_Click);
			// 
			// MdiStatusBar
			// 
			this.MdiStatusBar.Location = new System.Drawing.Point(0, 267);
			this.MdiStatusBar.Name = "MdiStatusBar";
			this.MdiStatusBar.Size = new System.Drawing.Size(456, 22);
			this.MdiStatusBar.TabIndex = 1;
			// 
			// MdiToolBar
			// 
			this.MdiToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						  this.toolbarButtonNew,
																						  this.toolbarButtonOpen,
																						  this.toolbarButtonSave,
																						  this.toolbarButtonPrint,
																						  this.toolbarButtonPreview,
																						  this.toolbarButtonDob,
																						  this.toolbarButtonDisp,
																						  this.toolbarButtonHelp});
			this.MdiToolBar.DropDownArrows = true;
			this.MdiToolBar.ImageList = this.MdiImageList;
			this.MdiToolBar.Location = new System.Drawing.Point(0, 0);
			this.MdiToolBar.Name = "MdiToolBar";
			this.MdiToolBar.ShowToolTips = true;
			this.MdiToolBar.Size = new System.Drawing.Size(456, 28);
			this.MdiToolBar.TabIndex = 2;
			this.MdiToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.MdiToolBar_ButtonClick);
			// 
			// toolbarButtonNew
			// 
			this.toolbarButtonNew.ImageIndex = 0;
			this.toolbarButtonNew.Tag = "New";
			this.toolbarButtonNew.ToolTipText = "New Chart";
			// 
			// toolbarButtonOpen
			// 
			this.toolbarButtonOpen.ImageIndex = 1;
			this.toolbarButtonOpen.Tag = "ButtonOpen";
			this.toolbarButtonOpen.ToolTipText = "Open Chart";
			// 
			// toolbarButtonSave
			// 
			this.toolbarButtonSave.ImageIndex = 2;
			this.toolbarButtonSave.Tag = "Save";
			this.toolbarButtonSave.ToolTipText = "Save Chart";
			// 
			// toolbarButtonPrint
			// 
			this.toolbarButtonPrint.ImageIndex = 4;
			this.toolbarButtonPrint.ToolTipText = "Print";
			// 
			// toolbarButtonPreview
			// 
			this.toolbarButtonPreview.ImageIndex = 5;
			this.toolbarButtonPreview.ToolTipText = "Print Preview";
			// 
			// toolbarButtonDob
			// 
			this.toolbarButtonDob.ImageIndex = 7;
			this.toolbarButtonDob.ToolTipText = "Birth Data, Events";
			// 
			// toolbarButtonDisp
			// 
			this.toolbarButtonDisp.ImageIndex = 8;
			this.toolbarButtonDisp.ToolTipText = "Display Preferences";
			// 
			// toolbarButtonHelp
			// 
			this.toolbarButtonHelp.ImageIndex = 6;
			this.toolbarButtonHelp.Tag = "ButtonHelp";
			this.toolbarButtonHelp.ToolTipText = "Help";
			// 
			// MdiImageList
			// 
			this.MdiImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.MdiImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("MdiImageList.ImageStream")));
			this.MdiImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MhoraContainer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 289);
			this.Controls.Add(this.MdiToolBar);
			this.Controls.Add(this.MdiStatusBar);
			this.IsMdiContainer = true;
			this.Menu = this.MdiMenu;
			this.Name = "MhoraContainer";
			this.Text = "Mudgala Hora 0.1";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MhoraContainer_Closing);
			this.Load += new System.EventHandler(this.MhoraContainer_Load);
			this.ResumeLayout(false);

		}
		#endregion


		private void MhoraContainer_Load(object sender, System.EventArgs e)
		{
			gOpts = MhoraGlobalOptions.readFromFile();
			MhoraGlobalOptions.mainControl = this;
			if (MhoraGlobalOptions.Instance.ShowSplashScreen) 
			{
				Genghis.Windows.Forms.SplashScreen ss = new Genghis.Windows.Forms.SplashScreen (typeof(MhoraSplash),Genghis.Windows.Forms.SplashScreenStyles.TopMost );
				System.Threading.Thread.Sleep(0);
				ss.Close( null, 1000 );
			}
			
			this.openNewJhdFile();
		}

		private void menuItemNewView_Click(object sender, System.EventArgs e)
		{
			MhoraChild curr = (MhoraChild)this.ActiveMdiChild;
			if (null == curr) return;
			MhoraChild child2 = new MhoraChild(curr.getHoroscope());
			child2.Text = curr.Text;
			child2.MdiParent = this;
			child2.Name = curr.Name;
			child2.Show();
		}


		private void menuItemWindowTile_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
		}

		private void menuItemWindowCascade_Click(object sender, System.EventArgs e)
		{
			this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
		}

		private void openNewJhdFile ()
		{
			DateTime tNow = DateTime.Now;
			Moment mNow = new Moment (tNow.Year, tNow.Month, tNow.Day, tNow.Hour, tNow.Minute, tNow.Second);
			HoraInfo info = new HoraInfo(mNow,
				MhoraGlobalOptions.Instance.Latitude,
				MhoraGlobalOptions.Instance.Longitude,
				MhoraGlobalOptions.Instance.TimeZone);

			childCount++;
			Horoscope h = new Horoscope (info, (HoroscopeOptions)MhoraGlobalOptions.Instance.HOptions.Clone());
				//new HoroscopeOptions());
			MhoraChild child = new MhoraChild(h);
			child.Text = childCount.ToString() + " - Prasna Chart";
			child.MdiParent = this;
			child.Name = child.Text;
			//info.name = child.Text;
			try 
			{
				child.Show();		
			}
			catch (System.OutOfMemoryException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void openJhdFileNow ()
		{
			DateTime tNow = DateTime.Now;
			Moment mNow = new Moment (tNow.Year, tNow.Month, tNow.Day, tNow.Hour, tNow.Minute, tNow.Second);

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "JHD Files (*.jhd)|*.jhd";

			if (ofd.ShowDialog() != DialogResult.OK)
				return;


			HoraInfo info = (new Jhd(ofd.FileName)).toHoraInfo();
			info.tob = mNow;

			string[] _path_split = ofd.FileName.Split(new Char[]{'/','\\'});
			ArrayList path_split = new ArrayList(_path_split);

			childCount++;
			Horoscope h = new Horoscope (info, (HoroscopeOptions)MhoraGlobalOptions.Instance.HOptions.Clone());

			//Horoscope h = new Horoscope (info, new HoroscopeOptions());
			MhoraChild child = new MhoraChild(h);
			child.Text = childCount.ToString() + " - Prasna Chart";
			child.MdiParent = this;
			child.Name = child.Text;
			child.Show();
		}


		private void openJhdFile ()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Hora Files (*.jhd; *.mhd)|*.jhd;*.mhd";

			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			string[] sparts = ofd.FileName.ToLower().Split(new char[]{'.'});
			HoraInfo info = null;

			if (sparts[sparts.Length-1] == "jhd")
				info = (new Jhd(ofd.FileName)).toHoraInfo();
			else
				info = (new Mhd(ofd.FileName)).toHoraInfo();

			string[] _path_split = ofd.FileName.Split(new Char[]{'/','\\'});
			ArrayList path_split = new ArrayList(_path_split);

			childCount++;
			Horoscope h = new Horoscope (info, new HoroscopeOptions());
			MhoraChild child = new MhoraChild(h);
			child.Text = childCount.ToString() + " - " + path_split[path_split.Count-1];
			child.MdiParent = this;
			child.Name = child.Text;
			child.mJhdFileName = ofd.FileName;

			child.Show();
		}

		public void AddChild (Horoscope h, string name)
		{
			childCount++;
			MhoraChild child = new MhoraChild(h);
			h.OnChanged();
			child.Text = childCount.ToString() + " - " + name;
			child.MdiParent = this;
			child.Name = child.Text;
			child.Show();
		}

		private void menuItemFileOpen_Click(object sender, System.EventArgs e)
		{
			openJhdFile();
		}

		private void menuItemHelpAboutMhora_Click(object sender, System.EventArgs e)
		{
			Form dlg = new AboutMhora();
			dlg.ShowDialog();
		}

		private void menuItemHelpAboutSjc_Click(object sender, System.EventArgs e)
		{
			Form dlg = new AboutSjc();
			dlg.ShowDialog();
		}

		private void MdiToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == toolbarButtonNew)
				this.openNewJhdFile();
			else if (e.Button == toolbarButtonOpen)
				this.openJhdFile();
			else if (e.Button == toolbarButtonSave)
			{
				MhoraChild curr = (MhoraChild)this.ActiveMdiChild;
				if (null == curr) return;
				curr.saveJhdFile();
			}
			else if (e.Button == toolbarButtonHelp)
			{
				Form dlg = new AboutMhora();
				dlg.ShowDialog();
			}
			else if (e.Button == toolbarButtonPrint)
			{
				MhoraChild mc = this.ActiveMdiChild as MhoraChild;
				if (mc != null)
					mc.menuPrint();
			}
			else if (e.Button == toolbarButtonPreview)
			{
				MhoraChild mc = this.ActiveMdiChild as MhoraChild;
				if (mc != null)
					mc.menuPrintPreview();
			} 
			else if (e.Button == toolbarButtonDob)
			{
				MhoraChild mc = this.ActiveMdiChild as MhoraChild;
				if (mc != null)
					mc.menuShowDobOptions();
			}
			else if (e.Button == toolbarButtonDisp)
			{
				this.showMenuGlobalDisplayPrefs();
			}

		}

		private void OnClosing()
		{
			if (MhoraGlobalOptions.Instance.SavePrefsOnExit == true)
				MhoraGlobalOptions.Instance.saveToFile();
		}


		private void menuItemFileExit_Click(object sender, System.EventArgs e)
		{
			this.OnClosing();
			this.Close();
		}



		private void menuItemFileNew_Click(object sender, System.EventArgs e)
		{
			this.openNewJhdFile();

		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuItemFileNewPrasna_Click(object sender, System.EventArgs e)
		{
			this.openJhdFileNow();
		}


		private void mSavePreferences_Click(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.saveToFile();
		}

		private object updateDisplayPreferences (object o)
		{
			MhoraGlobalOptions.NotifyDisplayChange();
			sweph.swe_set_ephe_path(MhoraGlobalOptions.Instance.HOptions.EphemerisPath);
			return o;
		}

		private void showMenuGlobalDisplayPrefs ()
		{
			//object wrapper = new GlobalizedPropertiesWrapper(MhoraGlobalOptions.Instance);
			MhoraOptions f = new MhoraOptions(MhoraGlobalOptions.Instance, new ApplyOptions(this.updateDisplayPreferences), true);
			f.ShowDialog();
		}
		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			this.showMenuGlobalDisplayPrefs();
		}


		private bool checkJhd (string fileName)
		{
			HoraInfo info = (new Jhd(fileName)).toHoraInfo();
			Horoscope h = new Horoscope(info, new HoroscopeOptions());
			if (h.getPosition(Body.Name.Ketu).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value ==
				h.getPosition(Body.Name.Lagna).toDivisionPosition(new Division(Basics.DivisionType.Rasi)).zodiac_house.value)
                return true;

			return false;
		}

		private void findCharts (string pathFrom, string pathTo)
		{
			WshShell shell = new WshShellClass();
			DirectoryInfo di = new DirectoryInfo(pathFrom);

			foreach(FileInfo f in di.GetFiles("*.jhd"))
			{
				bool bMatch = false;
				try 
				{
					bMatch = this.checkJhd(f.FullName);
				} 
				catch {}
				if (bMatch == true) 
				{
					string[] _path_split = f.FullName.Split(new Char[]{'/','\\'});
					ArrayList path_split = new ArrayList(_path_split);
					Link.Update(pathTo, f.FullName, (string)path_split[path_split.Count-1], true);			
					//Console.WriteLine(f.FullName);
				}
			}

			foreach (DirectoryInfo d in di.GetDirectories())
				findCharts (d.FullName, pathTo);
		}

		private void menuItemFindCharts_Click(object sender, System.EventArgs e)
		{

			FolderBrowserDialog fFrom = new FolderBrowserDialog();
			fFrom.Description = "Folder containing charts to search";
			fFrom.ShowDialog();
			string pathFrom = fFrom.SelectedPath;

			FolderBrowserDialog fTo = new FolderBrowserDialog();
			fTo.Description = "Folder where shortcuts should be created";
			fTo.ShowDialog();
			string pathTo = fTo.SelectedPath;

			this.findCharts(pathFrom, pathTo);

		}

		private void mResetPreferences_Click(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions mh = new MhoraGlobalOptions();
			MhoraGlobalOptions.Instance = mh;
			MhoraGlobalOptions.NotifyDisplayChange();
			MhoraGlobalOptions.NotifyCalculationChange();
		}
		private void mResetStrengthPreferences_Click(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.SOptions = new StrengthOptions();
			MhoraGlobalOptions.NotifyCalculationChange();
		}
		private void menuItemSplash_Click(object sender, System.EventArgs e)
		{
			//MhoraSplash f = new MhoraSplash();
			//f.Show();
			Genghis.Windows.Forms.SplashScreen ss = new Genghis.Windows.Forms.SplashScreen (typeof(MhoraSplash),Genghis.Windows.Forms.SplashScreenStyles.None );
			System.Threading.Thread.Sleep(0);
			ss.Close( null, 10000 );
		
		}

		private void mIncreaseFontSize_Click(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.increaseFontSize();
			MhoraGlobalOptions.NotifyDisplayChange();
		}

		private void mDecreaseFontSize_Click(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.decreaseFontSize();
			MhoraGlobalOptions.NotifyDisplayChange();
		}

		public object updateCalcPreferences (object o)
		{
			sweph.swe_set_ephe_path(MhoraGlobalOptions.Instance.HOptions.EphemerisPath);
			MhoraGlobalOptions.NotifyCalculationChange();
			return o;
		}
		private void mEditCalcPrefs_Click(object sender, System.EventArgs e)
		{
			//object wrapper = new GlobalizedPropertiesWrapper(MhoraGlobalOptions.Instance.HOptions);
			MhoraOptions f = new MhoraOptions(MhoraGlobalOptions.Instance.HOptions, new ApplyOptions(this.updateCalcPreferences), true);
			f.ShowDialog();
		}
		private void mEditStrengthPrefs_Click(object sender, System.EventArgs e)
		{
			MhoraOptions f = new MhoraOptions(MhoraGlobalOptions.Instance.SOptions, new ApplyOptions(this.updateCalcPreferences), true);
			f.ShowDialog();
		}
		private void MhoraContainer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.OnClosing();
		}






	}
}
