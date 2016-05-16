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
	/// Summary description for DasaControl.
	/// </summary>
	public class DasaControl : MhoraControl //System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ListView dasaItemList;
		private System.Windows.Forms.ColumnHeader Dasa;
		private System.Windows.Forms.ColumnHeader StartDate;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu dasaContextMenu;
		private System.Windows.Forms.MenuItem mOptions;
		private System.Windows.Forms.MenuItem mPreviousCycle;
		private System.Windows.Forms.MenuItem mNextCycle;
		private IDasa id;
		private ToDate td;
		private int min_cycle, max_cycle;
		private System.Windows.Forms.Label dasaInfo;
		private System.Windows.Forms.MenuItem mReset;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mDateOptions;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mEntryChart;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mSolarYears;
		private System.Windows.Forms.MenuItem mTithiYears;
		private System.Windows.Forms.MenuItem mFixedYears365;
		private System.Windows.Forms.MenuItem mFixedYears360;
		private System.Windows.Forms.MenuItem mCustomYears;
		private System.Windows.Forms.Button bPrevCycle;
		private System.Windows.Forms.Button bNextCycle;
		private System.Windows.Forms.Button bDasaOptions;
		private System.Windows.Forms.Button bDateOptions;
		private System.Windows.Forms.Button bRasiStrengths;
		private System.Windows.Forms.Button bGrahaStrengths;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mNormalize;
		private System.Windows.Forms.MenuItem mEditDasas;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mTribhagi80;
		private System.Windows.Forms.MenuItem mTriBhagi40;
		private System.Windows.Forms.MenuItem mResetParamAyus;
		private System.Windows.Forms.MenuItem mCompressSolar;
		private System.Windows.Forms.MenuItem mCompressLunar;
		private System.Windows.Forms.MenuItem mCompressTithiPraveshaTithi;
		private System.Windows.Forms.MenuItem mCompressTithiPraveshaSolar;
		private System.Windows.Forms.MenuItem mCompressedTithiPraveshaFixed;
		private System.Windows.Forms.MenuItem mEntryDate;
		private System.Windows.Forms.MenuItem mLocateEvent;
		private System.Windows.Forms.MenuItem mEntrySunriseChart;
		private System.Windows.Forms.MenuItem mShowEvents;
		private System.Windows.Forms.MenuItem m3Parts;
		private System.Windows.Forms.MenuItem mCompressedYogaPraveshaYoga;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mCompressYoga;
		private System.Windows.Forms.MenuItem mEntryChartCompressed;
		private Dasa.Options mDasaOptions;

		public Dasa.Options DasaOptions
		{
			get { return this.mDasaOptions; }
		}

		public Object DasaSpecificOptions
		{
			get { return id.GetOptions(); }
			set { id.SetOptions(value); }
		}

		private void SetDescriptionLabel ()
		{
			this.dasaInfo.Text = id.Description();
			
			dasaInfo.Text += " (";

			if (this.mDasaOptions.Compression > 0)
				this.dasaInfo.Text += this.mDasaOptions.Compression.ToString();


			dasaInfo.Text = string.Format("{0} {1:0.00} {2}",
				dasaInfo.Text,
				this.mDasaOptions.YearLength,
				this.mDasaOptions.YearType
				);

			dasaInfo.Text += " )";

			return;

		}
		public void ResetDisplayOptions (object o)
		{
			this.dasaItemList.BackColor = MhoraGlobalOptions.Instance.DasaBackgroundColor;
			this.dasaItemList.Font = MhoraGlobalOptions.Instance.GeneralFont;
			foreach (ListViewItem li in this.dasaItemList.Items)
			{
				DasaItem di = (DasaItem)li;
				li.BackColor = MhoraGlobalOptions.Instance.DasaBackgroundColor;
				li.Font = MhoraGlobalOptions.Instance.GeneralFont;
				foreach (ListViewItem.ListViewSubItem si in li.SubItems)
					si.BackColor = MhoraGlobalOptions.Instance.DasaBackgroundColor;
				di.EventDesc = "";
				if (li.SubItems.Count >= 2)
				{
					li.SubItems[0].ForeColor = MhoraGlobalOptions.Instance.DasaPeriodColor;
					li.SubItems[1].ForeColor = MhoraGlobalOptions.Instance.DasaDateColor;
					li.SubItems[1].Font = MhoraGlobalOptions.Instance.FixedWidthFont;
				}
			}
			this.dasaItemList.HoverSelection = MhoraGlobalOptions.Instance.DasaHoverSelect;
			this.LocateChartEvents();
		}
		public void Reset ()
		{
			id.recalculateOptions();
			SetDescriptionLabel();
			dasaItemList.Items.Clear();
			SetDasaYearType();
			min_cycle = max_cycle = 0;
			double compress = mDasaOptions.Compression == 0.0 ? 0.0 : mDasaOptions.Compression / id.paramAyus();

			sweph.obtainLock(this.h);
			ArrayList a = id.Dasa(0);
			foreach (DasaEntry de in a)
			{
				DasaItem di = new DasaItem(de);
				di.populateListViewItemMembers(td, id);
				dasaItemList.Items.Add (di);
			}
			sweph.releaseLock(this.h);
			this.LocateChartEvents();
		}

		public void OnRecalculate (Object o)
		{
			this.Reset();

		}
		public void OnDasaChanged (Object o)
		{
			//Reset();
		}


		public DasaControl(Horoscope _h, IDasa _id): base ()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			h = _h;
			id = _id;
			this.mDasaOptions = new Dasa.Options();

			if (h.info.defaultYearCompression != 0)
			{
				this.mDasaOptions.Compression = h.info.defaultYearCompression;
				this.mDasaOptions.YearLength = h.info.defaultYearLength;
				this.mDasaOptions.YearType = h.info.defaultYearType;
			}


			this.SetDasaYearType();
			//td = new ToDate (h.baseUT, mDasaOptions.YearLength, 0.0, h);
			this.mShowEvents.Checked = MhoraGlobalOptions.Instance.DasaShowEvents;
			ResetDisplayOptions(MhoraGlobalOptions.Instance);

			Dasa d = (Dasa)id;
			d.RecalculateEvent += new Recalculate(recalculateEntries);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(this.ResetDisplayOptions);
			h.Changed += new EvtChanged(OnRecalculate);
			this.SetDescriptionLabel();
			d.Changed += new EvtChanged(OnDasaChanged);
			if (this.dasaItemList.Items.Count >= 1)
				this.dasaItemList.Items[0].Selected = true;

			this.VScroll = true;
			Reset();

			//this.LocateChartEvents();
		}


		public bool LinkToHoroscope 
		{
			set 
			{
				if (value == true)
				{
					h.Changed += new EvtChanged(OnRecalculate);
					((Dasa)id).Changed += new EvtChanged(OnDasaChanged);
				}
				else
				{
					h.Changed -= new EvtChanged(OnRecalculate);
					((Dasa)id).Changed += new EvtChanged(OnDasaChanged);
				}
			}
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
			this.dasaItemList = new System.Windows.Forms.ListView();
			this.Dasa = new System.Windows.Forms.ColumnHeader();
			this.StartDate = new System.Windows.Forms.ColumnHeader();
			this.dasaContextMenu = new System.Windows.Forms.ContextMenu();
			this.mEntryChart = new System.Windows.Forms.MenuItem();
			this.mEntrySunriseChart = new System.Windows.Forms.MenuItem();
			this.mEntryDate = new System.Windows.Forms.MenuItem();
			this.mLocateEvent = new System.Windows.Forms.MenuItem();
			this.mReset = new System.Windows.Forms.MenuItem();
			this.m3Parts = new System.Windows.Forms.MenuItem();
			this.mShowEvents = new System.Windows.Forms.MenuItem();
			this.mOptions = new System.Windows.Forms.MenuItem();
			this.mDateOptions = new System.Windows.Forms.MenuItem();
			this.mPreviousCycle = new System.Windows.Forms.MenuItem();
			this.mNextCycle = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mSolarYears = new System.Windows.Forms.MenuItem();
			this.mTithiYears = new System.Windows.Forms.MenuItem();
			this.mFixedYears360 = new System.Windows.Forms.MenuItem();
			this.mFixedYears365 = new System.Windows.Forms.MenuItem();
			this.mCustomYears = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mTribhagi80 = new System.Windows.Forms.MenuItem();
			this.mTriBhagi40 = new System.Windows.Forms.MenuItem();
			this.mResetParamAyus = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mCompressSolar = new System.Windows.Forms.MenuItem();
			this.mCompressLunar = new System.Windows.Forms.MenuItem();
			this.mCompressYoga = new System.Windows.Forms.MenuItem();
			this.mCompressTithiPraveshaTithi = new System.Windows.Forms.MenuItem();
			this.mCompressTithiPraveshaSolar = new System.Windows.Forms.MenuItem();
			this.mCompressedTithiPraveshaFixed = new System.Windows.Forms.MenuItem();
			this.mCompressedYogaPraveshaYoga = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mEditDasas = new System.Windows.Forms.MenuItem();
			this.mNormalize = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.dasaInfo = new System.Windows.Forms.Label();
			this.bPrevCycle = new System.Windows.Forms.Button();
			this.bNextCycle = new System.Windows.Forms.Button();
			this.bDasaOptions = new System.Windows.Forms.Button();
			this.bDateOptions = new System.Windows.Forms.Button();
			this.bRasiStrengths = new System.Windows.Forms.Button();
			this.bGrahaStrengths = new System.Windows.Forms.Button();
			this.mEntryChartCompressed = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// dasaItemList
			// 
			this.dasaItemList.AllowColumnReorder = true;
			this.dasaItemList.AllowDrop = true;
			this.dasaItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dasaItemList.BackColor = System.Drawing.Color.Lavender;
			this.dasaItemList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.Dasa,
																						   this.StartDate});
			this.dasaItemList.ContextMenu = this.dasaContextMenu;
			this.dasaItemList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dasaItemList.ForeColor = System.Drawing.Color.Black;
			this.dasaItemList.FullRowSelect = true;
			this.dasaItemList.HideSelection = false;
			this.dasaItemList.HoverSelection = true;
			this.dasaItemList.Location = new System.Drawing.Point(8, 40);
			this.dasaItemList.MultiSelect = false;
			this.dasaItemList.Name = "dasaItemList";
			this.dasaItemList.Size = new System.Drawing.Size(424, 264);
			this.dasaItemList.TabIndex = 0;
			this.dasaItemList.View = System.Windows.Forms.View.Details;
			this.dasaItemList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dasaItemList_MouseDown);
			this.dasaItemList.Click += new System.EventHandler(this.dasaItemList_Click);
			this.dasaItemList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dasaItemList_MouseUp);
			this.dasaItemList.DragDrop += new System.Windows.Forms.DragEventHandler(this.dasaItemList_DragDrop);
			this.dasaItemList.MouseEnter += new System.EventHandler(this.dasaItemList_MouseEnter);
			this.dasaItemList.DragEnter += new System.Windows.Forms.DragEventHandler(this.dasaItemList_DragEnter);
			this.dasaItemList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dasaItemList_MouseMove);
			this.dasaItemList.SelectedIndexChanged += new System.EventHandler(this.dasaItemList_SelectedIndexChanged);
			// 
			// Dasa
			// 
			this.Dasa.Text = "Dasa";
			this.Dasa.Width = 150;
			// 
			// StartDate
			// 
			this.StartDate.Text = "Dates";
			this.StartDate.Width = 500;
			// 
			// dasaContextMenu
			// 
			this.dasaContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.mEntryChart,
																							this.mEntryChartCompressed,
																							this.mEntrySunriseChart,
																							this.mEntryDate,
																							this.mLocateEvent,
																							this.mReset,
																							this.m3Parts,
																							this.mShowEvents,
																							this.mOptions,
																							this.mDateOptions,
																							this.mPreviousCycle,
																							this.mNextCycle,
																							this.menuItem3,
																							this.menuItem4,
																							this.menuItem1,
																							this.menuItem2});
			this.dasaContextMenu.Popup += new System.EventHandler(this.dasaContextMenu_Popup);
			// 
			// mEntryChart
			// 
			this.mEntryChart.Index = 0;
			this.mEntryChart.Text = "&Entry Chart";
			this.mEntryChart.Click += new System.EventHandler(this.mEntryChart_Click);
			// 
			// mEntrySunriseChart
			// 
			this.mEntrySunriseChart.Index = 2;
			this.mEntrySunriseChart.Text = "Entry &Sunrise Chart";
			this.mEntrySunriseChart.Click += new System.EventHandler(this.mEntrySunriseChart_Click);
			// 
			// mEntryDate
			// 
			this.mEntryDate.Index = 3;
			this.mEntryDate.Text = "Copy Entry Date";
			this.mEntryDate.Click += new System.EventHandler(this.mEntryDate_Click);
			// 
			// mLocateEvent
			// 
			this.mLocateEvent.Index = 4;
			this.mLocateEvent.Text = "Locate An Event";
			this.mLocateEvent.Click += new System.EventHandler(this.mLocateEvent_Click);
			// 
			// mReset
			// 
			this.mReset.Index = 5;
			this.mReset.Text = "&Reset";
			this.mReset.Click += new System.EventHandler(this.mReset_Click);
			// 
			// m3Parts
			// 
			this.m3Parts.Index = 6;
			this.m3Parts.Text = "3 Parts";
			this.m3Parts.Click += new System.EventHandler(this.m3Parts_Click);
			// 
			// mShowEvents
			// 
			this.mShowEvents.Checked = true;
			this.mShowEvents.Index = 7;
			this.mShowEvents.Text = "Show Events";
			this.mShowEvents.Click += new System.EventHandler(this.mShowEvents_Click);
			// 
			// mOptions
			// 
			this.mOptions.Index = 8;
			this.mOptions.Text = "Dasa &Options";
			this.mOptions.Visible = false;
			this.mOptions.Click += new System.EventHandler(this.mOptions_Click);
			// 
			// mDateOptions
			// 
			this.mDateOptions.Index = 9;
			this.mDateOptions.Text = "&Date Options";
			this.mDateOptions.Visible = false;
			this.mDateOptions.Click += new System.EventHandler(this.mDateOptions_Click);
			// 
			// mPreviousCycle
			// 
			this.mPreviousCycle.Index = 10;
			this.mPreviousCycle.Text = "&Previous Cycle";
			this.mPreviousCycle.Visible = false;
			this.mPreviousCycle.Click += new System.EventHandler(this.mPreviousCycle_Click);
			// 
			// mNextCycle
			// 
			this.mNextCycle.Index = 11;
			this.mNextCycle.Text = "&Next Cycle";
			this.mNextCycle.Visible = false;
			this.mNextCycle.Click += new System.EventHandler(this.mNextCycle_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 12;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mSolarYears,
																					  this.mTithiYears,
																					  this.mFixedYears360,
																					  this.mFixedYears365,
																					  this.mCustomYears,
																					  this.menuItem5,
																					  this.mTribhagi80,
																					  this.mTriBhagi40,
																					  this.mResetParamAyus,
																					  this.menuItem6,
																					  this.mCompressSolar,
																					  this.mCompressLunar,
																					  this.mCompressYoga,
																					  this.mCompressTithiPraveshaTithi,
																					  this.mCompressTithiPraveshaSolar,
																					  this.mCompressedTithiPraveshaFixed,
																					  this.mCompressedYogaPraveshaYoga});
			this.menuItem3.Text = "Year Options";
			// 
			// mSolarYears
			// 
			this.mSolarYears.Index = 0;
			this.mSolarYears.Text = "&Solar Years (360 degrees)";
			this.mSolarYears.Click += new System.EventHandler(this.mSolarYears_Click);
			// 
			// mTithiYears
			// 
			this.mTithiYears.Index = 1;
			this.mTithiYears.Text = "&Tithi Years (360 tithis)";
			this.mTithiYears.Click += new System.EventHandler(this.mTithiYears_Click);
			// 
			// mFixedYears360
			// 
			this.mFixedYears360.Index = 2;
			this.mFixedYears360.Text = "Savana Years (360 days)";
			this.mFixedYears360.Click += new System.EventHandler(this.mFixedYears360_Click);
			// 
			// mFixedYears365
			// 
			this.mFixedYears365.Index = 3;
			this.mFixedYears365.Text = "~ Solar Year (365.2425 days)";
			this.mFixedYears365.Click += new System.EventHandler(this.mFixedYears365_Click);
			// 
			// mCustomYears
			// 
			this.mCustomYears.Index = 4;
			this.mCustomYears.Text = "&Custom Years";
			this.mCustomYears.Click += new System.EventHandler(this.mCustomYears_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 5;
			this.menuItem5.Text = "-";
			// 
			// mTribhagi80
			// 
			this.mTribhagi80.Index = 6;
			this.mTribhagi80.Text = "Tribhagi ParamAyus (80 Years)";
			this.mTribhagi80.Click += new System.EventHandler(this.mTribhagi80_Click);
			// 
			// mTriBhagi40
			// 
			this.mTriBhagi40.Index = 7;
			this.mTriBhagi40.Text = "Tribhagi ParamAyus (40 Years)";
			this.mTriBhagi40.Click += new System.EventHandler(this.mTriBhagi40_Click);
			// 
			// mResetParamAyus
			// 
			this.mResetParamAyus.Index = 8;
			this.mResetParamAyus.Text = "Regular ParamAyus";
			this.mResetParamAyus.Click += new System.EventHandler(this.mResetParamAyus_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 9;
			this.menuItem6.Text = "-";
			// 
			// mCompressSolar
			// 
			this.mCompressSolar.Index = 10;
			this.mCompressSolar.Text = "Compress to Solar Year";
			this.mCompressSolar.Click += new System.EventHandler(this.mCompressSolar_Click);
			// 
			// mCompressLunar
			// 
			this.mCompressLunar.Index = 11;
			this.mCompressLunar.Text = "Compress to Tithi Year";
			this.mCompressLunar.Click += new System.EventHandler(this.mCompressLunar_Click);
			// 
			// mCompressYoga
			// 
			this.mCompressYoga.Index = 12;
			this.mCompressYoga.Text = "Compress to Yoga Year";
			this.mCompressYoga.Click += new System.EventHandler(this.mCompressYoga_Click);
			// 
			// mCompressTithiPraveshaTithi
			// 
			this.mCompressTithiPraveshaTithi.Index = 13;
			this.mCompressTithiPraveshaTithi.Text = "Compress to Tithi Pravesha Year (Tithi)";
			this.mCompressTithiPraveshaTithi.Click += new System.EventHandler(this.mCompressTithiPraveshaTithi_Click);
			// 
			// mCompressTithiPraveshaSolar
			// 
			this.mCompressTithiPraveshaSolar.Index = 14;
			this.mCompressTithiPraveshaSolar.Text = "Compress to Tithi Pravesha Year (Solar)";
			this.mCompressTithiPraveshaSolar.Click += new System.EventHandler(this.mCompressTithiPraveshaSolar_Click);
			// 
			// mCompressedTithiPraveshaFixed
			// 
			this.mCompressedTithiPraveshaFixed.Index = 15;
			this.mCompressedTithiPraveshaFixed.Text = "Compress to Tithi Pravesha Year (Fixed)";
			this.mCompressedTithiPraveshaFixed.Click += new System.EventHandler(this.mCompressedTithiPraveshaFixed_Click);
			// 
			// mCompressedYogaPraveshaYoga
			// 
			this.mCompressedYogaPraveshaYoga.Index = 16;
			this.mCompressedYogaPraveshaYoga.Text = "Compress to Yoga Pravesha Year (Yoga)";
			this.mCompressedYogaPraveshaYoga.Click += new System.EventHandler(this.mCompressedYogaPraveshaYoga_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 13;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mEditDasas,
																					  this.mNormalize});
			this.menuItem4.Text = "Advanced";
			// 
			// mEditDasas
			// 
			this.mEditDasas.Index = 0;
			this.mEditDasas.Text = "Edit Dasas";
			this.mEditDasas.Click += new System.EventHandler(this.mEditDasas_Click);
			// 
			// mNormalize
			// 
			this.mNormalize.Index = 1;
			this.mNormalize.Text = "Normalize Dates";
			this.mNormalize.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 14;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 15;
			this.menuItem2.Text = "-";
			// 
			// dasaInfo
			// 
			this.dasaInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dasaInfo.Location = new System.Drawing.Point(184, 8);
			this.dasaInfo.Name = "dasaInfo";
			this.dasaInfo.Size = new System.Drawing.Size(232, 23);
			this.dasaInfo.TabIndex = 1;
			this.dasaInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.dasaInfo.Click += new System.EventHandler(this.dasaInfo_Click);
			// 
			// bPrevCycle
			// 
			this.bPrevCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bPrevCycle.Location = new System.Drawing.Point(8, 8);
			this.bPrevCycle.Name = "bPrevCycle";
			this.bPrevCycle.Size = new System.Drawing.Size(24, 23);
			this.bPrevCycle.TabIndex = 2;
			this.bPrevCycle.Text = "<";
			this.bPrevCycle.Click += new System.EventHandler(this.bPrevCycle_Click);
			// 
			// bNextCycle
			// 
			this.bNextCycle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bNextCycle.Location = new System.Drawing.Point(32, 8);
			this.bNextCycle.Name = "bNextCycle";
			this.bNextCycle.Size = new System.Drawing.Size(24, 23);
			this.bNextCycle.TabIndex = 3;
			this.bNextCycle.Text = ">";
			this.bNextCycle.Click += new System.EventHandler(this.bNextCycle_Click);
			// 
			// bDasaOptions
			// 
			this.bDasaOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bDasaOptions.Location = new System.Drawing.Point(64, 8);
			this.bDasaOptions.Name = "bDasaOptions";
			this.bDasaOptions.Size = new System.Drawing.Size(40, 23);
			this.bDasaOptions.TabIndex = 4;
			this.bDasaOptions.Text = "Opts";
			this.bDasaOptions.Click += new System.EventHandler(this.bDasaOptions_Click);
			// 
			// bDateOptions
			// 
			this.bDateOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bDateOptions.Location = new System.Drawing.Point(104, 8);
			this.bDateOptions.Name = "bDateOptions";
			this.bDateOptions.Size = new System.Drawing.Size(24, 23);
			this.bDateOptions.TabIndex = 5;
			this.bDateOptions.Text = "Yr";
			this.bDateOptions.Click += new System.EventHandler(this.bDateOptions_Click);
			// 
			// bRasiStrengths
			// 
			this.bRasiStrengths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bRasiStrengths.Location = new System.Drawing.Point(128, 8);
			this.bRasiStrengths.Name = "bRasiStrengths";
			this.bRasiStrengths.Size = new System.Drawing.Size(24, 23);
			this.bRasiStrengths.TabIndex = 6;
			this.bRasiStrengths.Text = "R";
			this.bRasiStrengths.Click += new System.EventHandler(this.bRasiStrengths_Click);
			// 
			// bGrahaStrengths
			// 
			this.bGrahaStrengths.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.bGrahaStrengths.Location = new System.Drawing.Point(152, 8);
			this.bGrahaStrengths.Name = "bGrahaStrengths";
			this.bGrahaStrengths.Size = new System.Drawing.Size(24, 23);
			this.bGrahaStrengths.TabIndex = 7;
			this.bGrahaStrengths.Text = "G";
			this.bGrahaStrengths.Click += new System.EventHandler(this.bGrahaStrengths_Click);
			// 
			// mEntryChartCompressed
			// 
			this.mEntryChartCompressed.Index = 1;
			this.mEntryChartCompressed.Text = "Entry Chart (&Compressed)";
			this.mEntryChartCompressed.Click += new System.EventHandler(this.mEntryChartCompressed_Click);
			// 
			// DasaControl
			// 
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this.Controls.Add(this.bGrahaStrengths);
			this.Controls.Add(this.bRasiStrengths);
			this.Controls.Add(this.bDateOptions);
			this.Controls.Add(this.bDasaOptions);
			this.Controls.Add(this.bNextCycle);
			this.Controls.Add(this.bPrevCycle);
			this.Controls.Add(this.dasaInfo);
			this.Controls.Add(this.dasaItemList);
			this.Name = "DasaControl";
			this.Size = new System.Drawing.Size(440, 312);
			this.Load += new System.EventHandler(this.DasaControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void mEntryChart_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			Horoscope h2 = (Horoscope)h.Clone();
			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];

			sweph.obtainLock(this.h);
			Moment m = this.td.AddYears (di.entry.startUT);
			h2.info.tob = m;		
			sweph.releaseLock(this.h);

			MhoraChild mchild = (MhoraChild)this.ParentForm;
			MhoraContainer mcont= (MhoraContainer)this.ParentForm.ParentForm;

			mcont.AddChild(h2, mchild.Name + ": Dasa Entry - (" + di.entry.shortDesc + ") " + id.Description());
		}


		private void mEntryChartCompressed_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			Horoscope h2 = (Horoscope)h.Clone();
			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];

			sweph.obtainLock(this.h);
			Moment m = this.td.AddYears (di.entry.startUT);
			Moment mEnd = this.td.AddYears(di.entry.startUT + di.entry.dasaLength);

			double ut_diff = mEnd.toUniversalTime() - m.toUniversalTime();
			h2.info.tob = m;		
			sweph.releaseLock(this.h);


			h2.info.defaultYearCompression = 1;
			h2.info.defaultYearLength = ut_diff;
			h2.info.defaultYearType = ToDate.DateType.FixedYear;

			MhoraChild mchild = (MhoraChild)this.ParentForm;
			MhoraContainer mcont= (MhoraContainer)this.ParentForm.ParentForm;

			mcont.AddChild(h2, mchild.Name + ": Dasa Entry - (" + di.entry.shortDesc + ") " + id.Description());

		}



		private void mEntrySunriseChart_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			Horoscope h2 = (Horoscope)h.Clone();
			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];

			sweph.obtainLock(this.h);
			Moment m = this.td.AddYears (di.entry.startUT);
			sweph.releaseLock(this.h);
			h2.info.tob = m;
			
			h2.OnChanged();

			// if done once, get something usually 2+ minutes off. 
			// don't know why this is.
			double offsetSunrise = h2.hoursAfterSunrise() / 24.0;
			m = new Moment(h2.baseUT - offsetSunrise, h2);
			h2.info.tob = m;
			h2.OnChanged();

			// so do it a second time, getting sunrise + 1 second.
			offsetSunrise = h2.hoursAfterSunrise() / 24.0;
			m = new Moment(h2.baseUT - offsetSunrise + (1.0/(24.0*60.0*60.0)), h2);
			h2.info.tob = m;
			h2.OnChanged();

			MhoraChild mchild = (MhoraChild)this.ParentForm;
			MhoraContainer mcont= (MhoraContainer)this.ParentForm.ParentForm;

			mcont.AddChild(h2, mchild.Name + ": Dasa Entry Sunrise - (" + di.entry.shortDesc + ") " + id.Description());

		}



		private void mEntryDate_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];
			sweph.obtainLock(h);
			Moment m = td.AddYears(di.entry.startUT);
			sweph.releaseLock(h);
			Clipboard.SetDataObject(m.ToString(), true);	
		}

		private void SplitDasa ()
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			this.SplitDasa((DasaItem)dasaItemList.SelectedItems[0]);
		}

		private void SplitDasa (DasaItem di)
		{

			//Trace.Assert(dasaItemList.SelectedItems.Count >= 1, "dasaItemList::doubleclick");
			int index = di.Index+1;

			bool action_inserting = true;


			this.dasaItemList.BeginUpdate();
			while (index < dasaItemList.Items.Count)
			{
				DasaItem tdi = (DasaItem)dasaItemList.Items[index];
				if (tdi.entry.level > di.entry.level) 
				{
					action_inserting = false;
					dasaItemList.Items.Remove(tdi);
				}
				else
					break;
			}

			if (action_inserting == false) 
			{
				this.dasaItemList.EndUpdate();
				return;
			}

			ArrayList a  = id.AntarDasa(di.entry);
			double compress = mDasaOptions.Compression == 0.0 ? 0.0 : mDasaOptions.Compression / id.paramAyus();

			sweph.obtainLock(this.h);
			foreach (DasaEntry de in a)
			{
				DasaItem pdi = new DasaItem(de);
				pdi.populateListViewItemMembers(td, id);
				dasaItemList.Items.Insert(index, pdi);
				index = index+1;
			}
			sweph.releaseLock(this.h);
			this.dasaItemList.EndUpdate();
			//this.dasaItemList.Items[index-1].Selected = true;
		}
		protected override void copyToClipboard ()
		{
			int iMaxDescLength = 0;
			for (int i=0; i<dasaItemList.Items.Count; i++)
				iMaxDescLength = Math.Max(dasaItemList.Items[i].Text.Length, iMaxDescLength);
			iMaxDescLength += 2;

			String s = this.dasaInfo.Text + "\r\n\r\n";
			for (int i=0; i< dasaItemList.Items.Count; i++) 
			{
				ListViewItem li = dasaItemList.Items[i];
				DasaItem di = (DasaItem)li;
				int levelSpace = di.entry.level*2;
				s += li.Text.PadRight(iMaxDescLength+levelSpace, ' ');

				for (int j=1; j<li.SubItems.Count; j++) 
				{
					s += "(" + li.SubItems[j].Text + ") ";
				}
				s += "\r\n";
			}
			Clipboard.SetDataObject(s);
		}
		private void recalculateEntries()
		{
			this.SetDescriptionLabel();
			dasaItemList.Items.Clear();
			ArrayList a = new ArrayList();
			for (int i = min_cycle; i<=max_cycle; i++)
			{
				ArrayList b = id.Dasa(i);
				a.AddRange (b);
			}
			sweph.obtainLock(this.h);
			foreach (DasaEntry de in a)
			{
				DasaItem di = new DasaItem(de);
				di.populateListViewItemMembers(td, id);
				dasaItemList.Items.Add(di);
			}
			sweph.releaseLock(this.h);
			this.LocateChartEvents();
		}

		private void mOptions_Click(object sender, System.EventArgs e)
		{
			//object wrapper = new GlobalizedPropertiesWrapper(id.GetOptions());
			MhoraOptions f = new MhoraOptions(id.GetOptions(), new ApplyOptions(id.SetOptions));
			f.pGrid.ExpandAllGridItems();
			f.ShowDialog();	
		}

		private void mPreviousCycle_Click(object sender, System.EventArgs e)
		{
			min_cycle --;
			ArrayList a = id.Dasa (min_cycle);
			int i = 0;
			sweph.obtainLock(this.h);
			foreach (DasaEntry de in a)
			{
				DasaItem di = new DasaItem(de);
				di.populateListViewItemMembers(td, id);
				dasaItemList.Items.Insert (i, di);
				i++;
			}
			sweph.releaseLock(this.h);
		}

		private void mNextCycle_Click(object sender, System.EventArgs e)
		{
			max_cycle++;
			ArrayList a = id.Dasa (max_cycle);
			sweph.obtainLock(this.h);
			foreach (DasaEntry de in a)
			{
				DasaItem di = new DasaItem(de);
				di.populateListViewItemMembers(td, id);
				dasaItemList.Items.Add(di);
			}
			sweph.releaseLock(this.h);
		}

		private void mReset_Click(object sender, System.EventArgs e)
		{
			Reset();
		}

		private void mVimsottari_Click(object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaVimsottari);
		}

		private void mAshtottari_Click(object sender, System.EventArgs e)
		{
			((MhoraControlContainer)this.Parent).h = this.h;
			((MhoraControlContainer)this.Parent).SetView(MhoraControlContainer.BaseUserOptions.ViewType.DasaAshtottari);		
		}


		private void DasaControl_Load(object sender, System.EventArgs e)
		{
			this.AddViewsToContextMenu (this.dasaContextMenu);
		}

		private void SetDasaYearType ()
		{
			double compress = mDasaOptions.Compression == 0.0 ? 0.0 : mDasaOptions.Compression / id.paramAyus();
			if (mDasaOptions.YearType == ToDate.DateType.FixedYear)// ||
				//mDasaOptions.YearType == ToDate.DateType.TithiYear)
				td = new ToDate (h.baseUT, this.mDasaOptions.YearLength, compress, h);
			else
				td = new ToDate (h.baseUT, this.mDasaOptions.YearType, this.mDasaOptions.YearLength, compress, this.h);

			td.SetOffset(this.mDasaOptions.OffsetDays + this.mDasaOptions.OffsetHours /24.0 + this.mDasaOptions.OffsetMinutes /(24.0*60.0));
		}
		private object SetDasaOptions (Object o)
		{
			Dasa.Options opts = (Dasa.Options)o;
			this.mDasaOptions.Copy (opts);
			this.SetDasaYearType();
			this.Reset();
			return mDasaOptions.Clone();
			
		}

		private void mDateOptions_Click(object sender, System.EventArgs e)
		{
			Form f = new MhoraOptions(this.mDasaOptions.Clone(), new ApplyOptions(SetDasaOptions));
			f.ShowDialog();
		}

		private void dasaItemList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DasaItem di = (DasaItem)this.dasaItemList.GetItemAt(e.X, e.Y);
			if (di == null)
				return;
			tooltip_event.SetToolTip(this.dasaItemList, di.EventDesc);
			tooltip_event.InitialDelay = 0;

			if (MhoraGlobalOptions.Instance.DasaMoveSelect)
				di.Selected = true;

			//Console.WriteLine ("MouseMove: {0} {1}", e.Y, li != null ? li.Index : -1);
			//if (li != null)
			//	li.Selected = true;
		}

		private void dasaItemList_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//this.dasaItemList_MouseMove(sender, e);
		}

		private void dasaItemList_MouseEnter(object sender, System.EventArgs e)
		{
			//Console.WriteLine ("Mouse Enter");
			//this.dasaItemList.Focus();
			//this.dasaItemList.Items[0].Selected = true;
		}

		static private ToolTip tooltip_event = new ToolTip();
		private void dasaItemList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//if (this.dasaItemList.SelectedItems.Count <= 0)
			//	return;

			//DasaItem di = (DasaItem)this.dasaItemList.SelectedItems[0];
			
			//tooltip_event.SetToolTip(this.dasaItemList, di.EventDesc);
			//tooltip_event.InitialDelay = 0;
		}


		private void dasaItemList_Click(object sender, System.EventArgs e)
		{
		}

		private void dasaItemList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.dasaItemList_MouseMove (sender, e);
			if (e.Button == MouseButtons.Left)
				this.SplitDasa();		

			ListViewItem li = this.dasaItemList.GetItemAt(e.X, e.Y);
			//Console.WriteLine ("MouseMove Click: {0} {1}", e.Y, li != null ? li.Index : -1);
			if (li != null)
				li.Selected = true;

		}

		private void mFixedYears365_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.YearType == ToDate.DateType.FixedYear &&
				this.mDasaOptions.YearLength == 365.2425)
				return;
			this.mDasaOptions.YearType = ToDate.DateType.FixedYear;
			this.mDasaOptions.YearLength = 365.2425;
			this.SetDasaYearType();
			this.Reset();
		}

		private void mTithiYears_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.YearType == ToDate.DateType.TithiYear &&
				this.mDasaOptions.YearLength == 360.0)
				return;
			this.mDasaOptions.YearType = ToDate.DateType.TithiYear;
			this.mDasaOptions.YearLength = 360.0;
			this.SetDasaYearType();
			this.Reset();
		}

		private void mSolarYears_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.YearType == ToDate.DateType.SolarYear &&
				this.mDasaOptions.YearLength == 360.0)
				return;
			this.mDasaOptions.YearType = ToDate.DateType.SolarYear;
			this.mDasaOptions.YearLength = 360.0;
			this.SetDasaYearType();
			this.Reset();			
		}

		private void mFixedYears360_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.YearType == ToDate.DateType.FixedYear &&
				this.mDasaOptions.YearLength == 360.0)
				return;
			this.mDasaOptions.YearType = ToDate.DateType.FixedYear;
			this.mDasaOptions.YearLength = 360.0;
			this.SetDasaYearType();
			this.Reset();
		}

		private void mTribhagi80_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 80)
				return;
			this.mDasaOptions.Compression = 80;
			this.SetDasaYearType();
			this.Reset();
		}

		private void mTriBhagi40_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 40)
				return;
			this.mDasaOptions.Compression = 40;
			this.SetDasaYearType();
			this.Reset();		
		}

		private void mResetParamAyus_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 0)
				return;
			this.mDasaOptions.Compression = 0;
			this.SetDasaYearType();
			this.Reset();
		}
		private void mCompressSolar_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 1 &&
				this.mDasaOptions.YearType == ToDate.DateType.SolarYear &&
				this.mDasaOptions.YearLength == 360)
				return;
			this.mDasaOptions.Compression = 1;
			this.mDasaOptions.YearLength = 360.0;
			this.mDasaOptions.YearType = ToDate.DateType.SolarYear;
			this.SetDasaYearType();
			this.Reset();		
		}

		private void mCompressLunar_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 1 &&
				this.mDasaOptions.YearType == ToDate.DateType.TithiYear &&
				this.mDasaOptions.YearLength == 360)
				return;
			this.mDasaOptions.Compression = 1;
			this.mDasaOptions.YearLength = 360.0;
			this.mDasaOptions.YearType = ToDate.DateType.TithiYear;
			this.SetDasaYearType();
			this.Reset();			
		}

		private void mCompressYoga_Click(object sender, System.EventArgs e)
		{
			if (this.mDasaOptions.Compression == 1 &&
				this.mDasaOptions.YearType == ToDate.DateType.YogaYear &&
				this.mDasaOptions.YearLength == 324)
				return;
			this.mDasaOptions.Compression = 1;
			this.mDasaOptions.YearLength = 324;
			this.mDasaOptions.YearType = ToDate.DateType.YogaYear;
			this.SetDasaYearType();
			this.Reset();
		}

		private void mCompressTithiPraveshaTithi_Click(object sender, System.EventArgs e)
		{
			this.mDasaOptions.YearType = ToDate.DateType.TithiYear;
			ToDate td_pravesh = new ToDate(h.baseUT, ToDate.DateType.TithiPraveshYear, 360.0, 0, h);
			ToDate td_tithi = new ToDate(h.baseUT, ToDate.DateType.TithiYear, 360.0, 0, h);
			sweph.obtainLock(h);
			if (td_tithi.AddYears(1).toUniversalTime() + 15.0 < td_pravesh.AddYears(1).toUniversalTime())
				this.mDasaOptions.YearLength = 390;
			else
				this.mDasaOptions.YearLength = 360;
			sweph.releaseLock(h);
			this.mDasaOptions.Compression = 1;
			this.SetDasaYearType();
			this.Reset();
		}

		public void compressToYogaPraveshaYearYoga ()
		{
			this.mDasaOptions.YearType = ToDate.DateType.YogaYear;
			ToDate td_pravesh = new ToDate(h.baseUT, ToDate.DateType.YogaPraveshYear, 360.0, 0, h);
			ToDate td_yoga = new ToDate(h.baseUT, ToDate.DateType.YogaYear, 324.0, 0, h);
			sweph.obtainLock(h);
			double date_to_surpass = td_pravesh.AddYears(1).toUniversalTime() - 5;
			double date_current = td_yoga.AddYears(0).toUniversalTime();
			double months = 0;
			while (date_current < date_to_surpass)
			{
				Console.WriteLine ("{0} > {1}", new Moment(date_current, h),
					new Moment(date_to_surpass, h));

				months++;
				date_current = td_yoga.AddYears(months/12.0).toUniversalTime();
			}
			sweph.releaseLock(h);
			this.mDasaOptions.Compression = 1;
			this.mDasaOptions.YearLength = (int)months * 27;
			this.SetDasaYearType();
			this.Reset();

		}
		private void mCompressedYogaPraveshaYoga_Click(object sender, System.EventArgs e)
		{
			this.compressToYogaPraveshaYearYoga();
		}

		private void mCompressTithiPraveshaSolar_Click(object sender, System.EventArgs e)
		{
			ToDate td_pravesh = new ToDate(h.baseUT, ToDate.DateType.TithiPraveshYear, 360.0, 0, h);
			sweph.obtainLock(h);
			double ut_start = td_pravesh.AddYears(0).toUniversalTime();
			double ut_end = td_pravesh.AddYears(1).toUniversalTime();
			BodyPosition sp_start = Basics.CalculateSingleBodyPosition(
				ut_start, sweph.BodyNameToSweph(Body.Name.Sun), Body.Name.Sun, BodyType.Name.Graha, this.h);
			BodyPosition sp_end = Basics.CalculateSingleBodyPosition(
				ut_end, sweph.BodyNameToSweph(Body.Name.Sun), Body.Name.Sun, BodyType.Name.Graha, this.h);
			Longitude lDiff = sp_end.longitude.sub(sp_start.longitude);
			double diff = lDiff.value;
			if (diff < 120.0) diff += 360.0;

			this.DasaOptions.YearType = ToDate.DateType.SolarYear;
			this.DasaOptions.YearLength = diff;
			sweph.releaseLock(h);
			this.DasaOptions.Compression = 1;
			this.Reset();
		}

		private void mCompressedTithiPraveshaFixed_Click(object sender, System.EventArgs e)
		{
			ToDate td_pravesh = new ToDate(h.baseUT, ToDate.DateType.TithiPraveshYear, 360.0, 0, h);
			sweph.obtainLock(h);
			this.DasaOptions.YearType = ToDate.DateType.FixedYear;
			this.DasaOptions.YearLength = td_pravesh.AddYears(1).toUniversalTime() - 
				td_pravesh.AddYears(0).toUniversalTime();
			sweph.releaseLock(h);
			this.Reset();
		}


		private void mCustomYears_Click(object sender, System.EventArgs e)
		{
			this.mDateOptions_Click(sender, e);
		}

		private void bPrevCycle_Click(object sender, System.EventArgs e)
		{
			this.mPreviousCycle_Click (sender, e);
		}

		private void bNextCycle_Click(object sender, System.EventArgs e)
		{
			this.mNextCycle_Click(sender, e);
		}

		private void bDasaOptions_Click(object sender, System.EventArgs e)
		{
			this.mOptions_Click(sender, e);
		}

		private void bDateOptions_Click(object sender, System.EventArgs e)
		{
			this.mDateOptions_Click(sender, e);
		}

		private void bRasiStrengths_Click(object sender, System.EventArgs e)
		{
			new RasiStrengthsControl(h).ShowDialog();
			//this.mRasiStrengths_Click(sender, e);		
		}

		public Object SetDasasOptions (Object a)
		{
			this.dasaItemList.Items.Clear();
			DasaEntry[] al = ((DasaEntriesWrapper)a).Entries;
			sweph.obtainLock(this.h);
			for (int i=0; i<al.Length;i++)
			{
				DasaItem di = new DasaItem(al[i]);
				di.populateListViewItemMembers(td, id);
				this.dasaItemList.Items.Add(di);
			}
			sweph.releaseLock(this.h);
			this.LocateChartEvents();
			return a;
		}

		class DasaEntriesWrapper
		{
			DasaEntry[] al;
			public DasaEntriesWrapper (DasaEntry[] _al)
			{
				al = _al;
			}
			public DasaEntry[] Entries 
			{
				get { return al; }
				set { al = value; }
			}
		}
		private void bEditItems_Click(object sender, System.EventArgs e)
		{

		}

		private void dasaContextMenu_Popup(object sender, System.EventArgs e)
		{
		
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			DasaEntry[] al = new DasaEntry[this.dasaItemList.Items.Count];
			DasaItem[] am = new DasaItem[this.dasaItemList.Items.Count];
			double start = 0.0;
			if (dasaItemList.Items.Count >= 1)
			{
				start = ((DasaItem)dasaItemList.Items[0]).entry.startUT;
			}
			for (int i=0; i<dasaItemList.Items.Count; i++)
			{
				DasaItem di = (DasaItem)dasaItemList.Items[i];
				al[i] = di.entry;
				if (al[i].level == 1) 
				{
					al[i].startUT = start;
					start += al[i].dasaLength;
				}
				am[i] = new DasaItem(al[i]);
			}
			this.dasaItemList.Items.Clear();
			sweph.obtainLock(this.h);
			for (int i=0; i<am.Length; i++)
			{
				am[i].populateListViewItemMembers(td, id);
				this.dasaItemList.Items.Add(am[i]);
			}
			sweph.releaseLock(this.h);
		}

		private void mDasaDown_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			Horoscope h2 = (Horoscope)h.Clone();
			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];

			Moment m = this.td.AddYears (di.entry.startUT);
			h2.info.tob = m;			

			MhoraChild mchild = (MhoraChild)this.ParentForm;
			MhoraContainer mcont= (MhoraContainer)this.ParentForm.ParentForm;

			mcont.AddChild(h2, mchild.Name + ": Dasa Entry Chart - (((" + di.entry.shortDesc + "))) " + id.Description());		
		}

		private void mEditDasas_Click(object sender, System.EventArgs e)
		{
			DasaEntry[] al = new DasaEntry[this.dasaItemList.Items.Count];
			for (int i=0; i<dasaItemList.Items.Count; i++)
			{
				DasaItem di = (DasaItem)dasaItemList.Items[i];
				al[i] = di.entry;
			}
			DasaEntriesWrapper dw = new DasaEntriesWrapper(al);
			MhoraOptions f = new MhoraOptions(dw, new ApplyOptions(this.SetDasasOptions), true);
			f.ShowDialog();		
		}

		private void bGrahaStrengths_Click(object sender, System.EventArgs e)
		{
			GrahaStrengthsControl gc = new GrahaStrengthsControl(h);
			gc.ShowDialog();
		}


		class EventUserOptions : ICloneable
		{
			Moment mEventDate;
			int mLevels;
			public Moment EventDate
			{
				get { return mEventDate; }
				set { this.mEventDate = value; }
			}
			public int Depth
			{
				get { return mLevels; }
				set { this.mLevels = value; }
			}
			public EventUserOptions (Moment _m)
			{
				this.mEventDate = (Moment)_m.Clone();
				this.mLevels = 2;
			}
			public object Clone ()
			{
				EventUserOptions euo = new EventUserOptions(this.mEventDate);
				euo.mLevels = this.mLevels;
				return euo;
			}
		}


		private void ExpandEvent (Moment m, int levels, string eventDesc)
		{
			double ut_m = m.toUniversalTime(h);
			for (int i=0; i<this.dasaItemList.Items.Count; i++)
			{
				DasaItem di = (DasaItem)this.dasaItemList.Items[i];

				sweph.obtainLock(h);
				Moment m_start = td.AddYears(di.entry.startUT);
				Moment m_end = td.AddYears(di.entry.startUT + di.entry.dasaLength);
				sweph.releaseLock(h);


				double ut_start = m_start.toUniversalTime(h);
				double ut_end = m_end.toUniversalTime(h);


				if (ut_m >= ut_start && ut_m < ut_end)
				{
					Console.WriteLine ("Found: Looking for {0} between {1} and {2}", m, m_start, m_end);

					if (levels > di.entry.level)
					{
						if (i==this.dasaItemList.Items.Count-1)
						{
							this.dasaItemList.SelectedItems.Clear();
							this.dasaItemList.Items[i].Selected = true;
							this.SplitDasa((DasaItem)this.dasaItemList.Items[i]);
						}
						if (i<this.dasaItemList.Items.Count-1)
						{
							DasaItem di_next = (DasaItem)this.dasaItemList.Items[i+1];
							if (di_next.entry.level == di.entry.level)
							{
								this.dasaItemList.SelectedItems.Clear();
								this.dasaItemList.Items[i].Selected = true;
								this.SplitDasa((DasaItem)this.dasaItemList.Items[i]);
							}
						}
					}
					else if (levels == di.entry.level)
					{
						foreach (ListViewItem.ListViewSubItem si in di.SubItems)
							si.BackColor = MhoraGlobalOptions.Instance.DasaHighlightColor;

						di.EventDesc += eventDesc;
					}
				}
			}
		}

		public object LocateEvent (object _euo)
		{
			EventUserOptions euo = (EventUserOptions)_euo;
			this.mEventOptionsCache = euo;
			this.ExpandEvent(euo.EventDate, euo.Depth, "Event: " + euo.EventDate.ToString());
			return _euo;
		}

		public void LocateChartEvents ()
		{
			if (this.mShowEvents.Checked == false)
				return;

			foreach (UserEvent ue in h.info.Events)
			{
				if (ue.WorkWithEvent == true)
					this.ExpandEvent(ue.EventTime, MhoraGlobalOptions.Instance.DasaEventsLevel, ue.ToString());
			}
		}

		EventUserOptions mEventOptionsCache = null;
		private void mLocateEvent_Click(object sender, System.EventArgs e)
		{
			if (this.mEventOptionsCache == null) 
			{
				DateTime dtNow = DateTime.Now;
				this.mEventOptionsCache = new EventUserOptions(
					new Moment(dtNow.Year, dtNow.Month, dtNow.Day, 
					(double)dtNow.Hour + (double)dtNow.Minute/60.0 + (double)dtNow.Second/3600.0));
			}

			new MhoraOptions(this.mEventOptionsCache, new ApplyOptions(LocateEvent)).ShowDialog();
		}


		private void dasaItemList_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart))) 
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;	

		}

		private void dasaItemList_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart)))
			{
				Division div = Division.CopyFromClipboard();
				if (null == div) return;
				this.id.DivisionChanged(div);
			}
		}

		private void dasaInfo_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mShowEvents_Click(object sender, System.EventArgs e)
		{
			this.mShowEvents.Checked = !this.mShowEvents.Checked;
		}

		private void m3Parts_Click(object sender, System.EventArgs e)
		{
			if (dasaItemList.SelectedItems.Count == 0)
				return;

			DasaItem di = (DasaItem)dasaItemList.SelectedItems[0];
			DasaEntry de = di.entry;

			Dasa3Parts form = new Dasa3Parts (h, de, td);
			form.Show();
		}

	}



	/// <summary>
	/// Specifies a DasaItem which can be used by any of the Dasa Systems.
	/// Hence it includes _both_ a graha and zodiacHouse in order to be used
	/// by systems which Graha dasas and Rasi bhukti's and vice-versa. The logic
	/// should be checked carefully
	/// </summary>

	public class DasaItem : System.Windows.Forms.ListViewItem
	{
		public DasaEntry entry;
		public string EventDesc;
		public void populateListViewItemMembers (ToDate td, IDasa id)
		{
			this.UseItemStyleForSubItems = false;

			//this.Text = entry.shortDesc;
			this.Font = MhoraGlobalOptions.Instance.GeneralFont;
			this.ForeColor = MhoraGlobalOptions.Instance.DasaPeriodColor;
			Moment m = td.AddYears(entry.startUT);
			Moment m2 = td.AddYears(entry.startUT + entry.dasaLength);
			string sDateRange = m.ToString() + " - " + m2.ToString();
			for (int i=1; i<this.entry.level; i++)
				sDateRange = " " + sDateRange;
			this.SubItems.Add (sDateRange);
			this.Text = entry.shortDesc + id.EntryDescription(entry, m, m2);
			this.SubItems[1].Font = MhoraGlobalOptions.Instance.FixedWidthFont;
			this.SubItems[1].ForeColor = MhoraGlobalOptions.Instance.DasaDateColor;
		}
		private void Construct (DasaEntry _entry)
		{
			entry = _entry;
		}
		public DasaItem (DasaEntry _entry)
		{
			entry = _entry;
		}
		public DasaItem (Body.Name _graha, double _startUT, double _dasaLength, int _level, string _shortDesc)
		{
			Construct (new DasaEntry(_graha, _startUT, _dasaLength, _level, _shortDesc));
		}
		public DasaItem (ZodiacHouse.Name _zodiacHouse, double _startUT, double _dasaLength, int _level, string _shortDesc)
		{
			Construct (new DasaEntry(_zodiacHouse, _startUT, _dasaLength, _level, _shortDesc));
		}
	}


}
