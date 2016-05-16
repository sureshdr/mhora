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
using System.Threading;

namespace mhora
{
	public delegate void DelegateComputeFinished();
	public class PanchangaControl : mhora.MhoraControl
	{

		public class UserOptions : ICloneable
		{
			int mNumDays;
			bool bCalculateLagnaCusps=false;
			bool bCalculateTithiCusps=true;
			bool bCalculateKaranaCusps=true;
			bool bCalculateNakshatraCusps=true;
			bool bCalculateHoraCusps=true;
			bool bCalculateSMYogaCusps=true;
			bool bCalculateKalaCusps=true;
			bool bShowSpecialKalas=true;
			bool bShowSunriset=true;
			bool bLargeHours = false;
			bool bShowUpdates = true;
			bool bOneEntryPerLine = false;
			public UserOptions ()
			{
				this.NumDays = 3;
			}
			[Description("Number of days to compute information for")]
			public int NumDays 
			{
				get { return mNumDays; }
				set { mNumDays = value; }
			}
			[Description("Include sunriset / sunset in the output?")]
			public bool ShowSunriset
			{
				get { return this.bShowSunriset; }
				set { this.bShowSunriset = value; }
			}
			[Description("Calculate and include Lagna cusp changes?")]
			public bool CalcLagnaCusps
			{
				get { return bCalculateLagnaCusps; }
				set { this.bCalculateLagnaCusps = value; }
			}
			[Description("Calculate and include Tithi cusp information?")]
			public bool CalcTithiCusps
			{
				get { return this.bCalculateTithiCusps; }
				set { this.bCalculateTithiCusps = value; }
			}
			[Description("Calculate and include Karana cusp information?")]
			public bool CalcKaranaCusps
			{
				get { return this.bCalculateKaranaCusps; }
				set { this.bCalculateKaranaCusps = value; }
			}
			[Description("Calculate and include Sun-Moon yoga cusp information?")]
			public bool CalcSMYogaCusps
			{
				get { return this.bCalculateSMYogaCusps; }
				set { this.bCalculateSMYogaCusps = value; }
			}
			[Description("Calculate and include Nakshatra cusp information?")]
			public bool CalcNakCusps
			{
				get { return this.bCalculateNakshatraCusps; }
				set { this.bCalculateNakshatraCusps = value; }
			}
			[Description("Calculate and include Hora cusp information?")]
			public bool CalcHoraCusps
			{
				get { return this.bCalculateHoraCusps; }
				set { this.bCalculateHoraCusps = value; }
			}
			[Description("Calculate and include special Kalas?")]
			public bool CalcSpecialKalas
			{
				get { return this.bShowSpecialKalas; }
				set { this.bShowSpecialKalas = value; }
			}
			[Description("Calculate and include Kala cusp information?")]
			public bool CalcKalaCusps
			{
				get { return this.bCalculateKalaCusps; }
				set { this.bCalculateKalaCusps = value; }
			}
			[Description("Display 02:00 after midnight as 26:00 or *02:00?")]
			public bool LargeHours
			{
				get { return bLargeHours; }
				set { this.bLargeHours = value; }
			}
			[Description("Display incremental updates?")]
			public bool ShowUpdates
			{
				get { return bShowUpdates; }
				set { this.bShowUpdates = value; }
			}
			[Description("Display only one entry / line?")]
			public bool OneEntryPerLine
			{
				get { return this.bOneEntryPerLine; }
				set { this.bOneEntryPerLine = value; }
			}
			public object Clone ()
			{
				UserOptions uo = new UserOptions();
				uo.NumDays = this.NumDays;
				uo.CalcLagnaCusps = this.CalcLagnaCusps;
				uo.CalcNakCusps = this.CalcNakCusps;
				uo.CalcTithiCusps = this.CalcTithiCusps;
				uo.CalcKaranaCusps = this.CalcKaranaCusps;
				uo.CalcHoraCusps = this.CalcHoraCusps;
				uo.CalcKalaCusps = this.CalcKalaCusps;
				uo.CalcSpecialKalas = this.CalcSpecialKalas;
				uo.LargeHours = this.LargeHours;
				uo.ShowUpdates = this.ShowUpdates;
				uo.ShowSunriset = this.ShowSunriset;
				uo.OneEntryPerLine = this.OneEntryPerLine;
				uo.CalcSMYogaCusps = this.CalcSMYogaCusps;
				return uo;
			}
			public object CopyFrom (object _uo)
			{
				UserOptions uo = (UserOptions)_uo;
				this.NumDays = uo.NumDays;
				this.CalcLagnaCusps = uo.CalcLagnaCusps;
				this.CalcNakCusps = uo.CalcNakCusps;
				this.CalcTithiCusps = uo.CalcTithiCusps;
				this.CalcKaranaCusps = uo.CalcKaranaCusps;
				this.CalcHoraCusps = uo.CalcHoraCusps;
				this.CalcKalaCusps = uo.CalcKalaCusps;
				this.CalcSpecialKalas = uo.CalcSpecialKalas;
				this.LargeHours = uo.LargeHours;
				this.ShowUpdates = uo.ShowUpdates;
				this.ShowSunriset = uo.ShowSunriset;
				this.CalcSMYogaCusps = uo.CalcSMYogaCusps;
				this.OneEntryPerLine = uo.OneEntryPerLine;
				return this.Clone();
			}


		}



		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.Button bOpts;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button bCompute;
		private UserOptions opts = null;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItemPrintPanchanga;
		private System.Windows.Forms.MenuItem menuItemFilePrintPreview;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		public DelegateComputeFinished m_DelegateComputeFinished;


		public PanchangaControl(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			h.Changed += new EvtChanged (OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged (OnRedisplay);
			opts = new UserOptions();
			this.AddViewsToContextMenu(this.contextMenu);
			this.mutexProgress = new Mutex(false);
			this.OnRedisplay(MhoraGlobalOptions.Instance.TableBackgroundColor);
			this.bCompute_Click(null, null);
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
			this.mList = new System.Windows.Forms.ListView();
			this.bOpts = new System.Windows.Forms.Button();
			this.bCompute = new System.Windows.Forms.Button();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemPrintPanchanga = new System.Windows.Forms.MenuItem();
			this.menuItemFilePrintPreview = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mList
			// 
			this.mList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(8, 40);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(512, 272);
			this.mList.TabIndex = 0;
			this.mList.View = System.Windows.Forms.View.Details;
			this.mList.SelectedIndexChanged += new System.EventHandler(this.mList_SelectedIndexChanged);
			// 
			// bOpts
			// 
			this.bOpts.Location = new System.Drawing.Point(16, 8);
			this.bOpts.Name = "bOpts";
			this.bOpts.TabIndex = 1;
			this.bOpts.Text = "Options";
			this.bOpts.Click += new System.EventHandler(this.bOpts_Click);
			// 
			// bCompute
			// 
			this.bCompute.Location = new System.Drawing.Point(104, 8);
			this.bCompute.Name = "bCompute";
			this.bCompute.TabIndex = 2;
			this.bCompute.Text = "Compute";
			this.bCompute.Click += new System.EventHandler(this.bCompute_Click);
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItemPrintPanchanga,
																						this.menuItemFilePrintPreview,
																						this.menuItem1,
																						this.menuItem2});
			// 
			// menuItemPrintPanchanga
			// 
			this.menuItemPrintPanchanga.Index = 0;
			this.menuItemPrintPanchanga.Text = "Print Panchanga";
			this.menuItemPrintPanchanga.Click += new System.EventHandler(this.menuItemPrintPanchanga_Click);
			// 
			// menuItemFilePrintPreview
			// 
			this.menuItemFilePrintPreview.Index = 1;
			this.menuItemFilePrintPreview.Text = "Print Preview Panchanga";
			this.menuItemFilePrintPreview.Click += new System.EventHandler(this.menuItemFilePrintPreview_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 3;
			this.menuItem2.Text = "-";
			// 
			// PanchangaControl
			// 
			this.ContextMenu = this.contextMenu;
			this.Controls.Add(this.bCompute);
			this.Controls.Add(this.bOpts);
			this.Controls.Add(this.mList);
			this.Name = "PanchangaControl";
			this.Size = new System.Drawing.Size(528, 320);
			this.Load += new System.EventHandler(this.PanchangaControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void PanchangaControl_Load(object sender, System.EventArgs e)
		{
		
		}

		bool bResultsInvalid = true;

		public void OnRedisplay (Object o)
		{
			this.mList.ForeColor = MhoraGlobalOptions.Instance.TableForegroundColor;
			this.mList.BackColor = MhoraGlobalOptions.Instance.TableBackgroundColor;
			this.mList.Font = MhoraGlobalOptions.Instance.GeneralFont;
		}
		public void OnRecalculate (object _h)
		{
			if (bResultsInvalid == true)
				return;

			Horoscope h = (Horoscope)_h;

			ListViewItem li = new ListViewItem();
			li.Text = "Results may be out of date. Click the Compute Button to recalculate the panchanga";
			this.mList.Items.Insert(0, li);
			this.mList.Items.Insert(1, "");
			bResultsInvalid = true;


		}
		public object SetOptions (object o)
		{
			return this.opts.CopyFrom(o);
		}

		private void bOpts_Click(object sender, System.EventArgs e)
		{
			//this.mutexProgress.WaitOne();
			//if (this.fProgress != null)
			//{
			//	MessageBox.Show("Cannot show options when calculation is in progress");
			//	this.mutexProgress.Close();
			//	return;
			//}
			new MhoraOptions(opts, new ApplyOptions(SetOptions)).ShowDialog();
			//this.mutexProgress.Close();
		}

		private void bCompute_Click(object sender, System.EventArgs e)
		{
		
			this.m_DelegateComputeFinished = new DelegateComputeFinished(this.ComputeFinished);
			Thread t = new Thread(new ThreadStart(this.ComputeStart));
			t.Start();
		}

		//ProgressDialog fProgress = null;
		Mutex mutexProgress = null;

		private void ComputeStart()
		{
			//this.mutexProgress.WaitOne();
			//if (fProgress != null)
			//{
			//	this.mutexProgress.Close();
			//	return;
			//}
			//fProgress = new ProgressDialog(opts.NumDays);
			//fProgress.setProgress(opts.NumDays/2);
			Console.WriteLine("Starting threaded computation");
			//fProgress.ShowDialog();
			//this.mutexProgress.Close();
			this.bCompute.Enabled = false;
			this.bOpts.Enabled = false;
			this.ContextMenu = null;
			this.ComputeEntries();
			this.Invoke(this.m_DelegateComputeFinished);

		}
		private void ComputeFinished()
		{
			Console.WriteLine ("Thread finished execution");
			this.bResultsInvalid = false;
			this.bCompute.Enabled = true;
			this.bOpts.Enabled = true;
			this.ContextMenu = this.contextMenu;
			//this.m_DelegateComputeFinished -= new DelegateComputeFinished(this.ComputeFinished);
			//this.mutexProgress.WaitOne();
			//fProgress.Close();
			//fProgress = null;
			//this.mutexProgress.Close();
		}


		private void ComputeEntries ()
		{
			this.mList.Clear();
			this.mList.Columns.Add ("", -2, System.Windows.Forms.HorizontalAlignment.Left);

			if (false == opts.ShowUpdates)
				this.mList.BeginUpdate();

			double ut_start = Math.Floor(h.baseUT);
			double[] geopos = new double[]
			{ h.info.lon.toDouble(), h.info.lat.toDouble(), h.info.alt };

			this.globals = new PanchangaGlobalMoments();
			this.locals = new ArrayList();

			for (int i=0; i< opts.NumDays; i++)
			{
				ComputeEntry (ut_start, geopos);
				ut_start += 1;
				this.mList.Columns[0].Width = -2;
			}
			this.mList.Columns[0].Width = -2;

			if (false == opts.ShowUpdates)
				this.mList.EndUpdate();
		}

		private Moment utToMoment (double found_ut)
		{
			// turn into horoscope
			int year=0, month=0, day=0;
			double hour =0;
			found_ut += (h.info.tz.toDouble() / 24.0);
			sweph.swe_revjul(found_ut, ref year, ref month, ref day, ref hour);
			Moment m = new Moment(year, month, day, hour);
			return m;
		}
		private string utToString (double ut)
		{
			int year=0, month=0, day=0;
			double time=0;
			
			ut += h.info.tz.toDouble()/24.0;
			sweph.swe_revjul(ut, ref year, ref month, ref day, ref time);
			return this.timeToString(time);
		}
		private string utTimeToString (double ut_event, double ut_sr, double sunrise)
		{
			Moment m = this.utToMoment(ut_event);
			HMSInfo hms = new HMSInfo(m.time);

			if (ut_event >= (ut_sr - (sunrise/24.0) + 1.0))
			{
				if (false == opts.LargeHours)
					return string.Format ("*{0:00}:{1:00}", hms.degree, hms.minute);
				else
					return string.Format ("{0:00}:{1:00}", hms.degree+24, hms.minute);
			}
			return string.Format ("{0:00}:{1:00}", hms.degree, hms.minute);
		}
		private string timeToString (double time)
		{
			HMSInfo hms = new HMSInfo(time);
			return string.Format("{0:00}:{1:00}",
				hms.degree, hms.minute, hms.second);
		}

		double sunrise=0;
		double ut_sr=0;

		int[] rahu_kalas = new int[]{7,1,6,4,5,3,2};
		int[] gulika_kalas = new int[]{6,5,4,3,2,1,0};
		int[] yama_kalas = new int[]{4,3,2,1,0,6,5};


		ArrayList locals = new ArrayList();
		PanchangaGlobalMoments globals = new PanchangaGlobalMoments();

		private void ComputeEntry (double ut, double[] geopos)
		{

			int year=0, month=0, day=0;
			double sunset=0, hour=0;
			sweph.obtainLock(h);
			h.populateSunrisetCacheHelper(ut-0.5, ref sunrise, ref sunset, ref ut_sr);
			sweph.releaseLock(h);

			sweph.swe_revjul(ut_sr, ref year, ref month, ref day, ref hour);
			Moment moment_sr = new Moment(year, month, day, hour);
			Moment moment_ut = new Moment(ut, h);
			HoraInfo infoCurr = new HoraInfo(moment_ut, h.info.lat, h.info.lon, h.info.tz);
			Horoscope hCurr = new Horoscope(infoCurr, h.options);

			ListViewItem li = null;

			PanchangaLocalMoments local = new PanchangaLocalMoments();
			local.sunrise = hCurr.sunrise;
			local.sunset = sunset;
			local.sunrise_ut = ut_sr;
			sweph.swe_revjul(ut, ref year, ref month, ref day, ref hour);
			local.wday = (Basics.Weekday) sweph.swe_day_of_week(ut);



			local.kalas_ut = hCurr.getKalaCuspsUt();
			if (this.opts.CalcSpecialKalas)
			{
				Body.Name bStart = Basics.weekdayRuler(hCurr.wday);
				if (hCurr.options.KalaType == HoroscopeOptions.EHoraType.Lmt)
					bStart = Basics.weekdayRuler(hCurr.lmt_wday);

				local.rahu_kala_index = this.rahu_kalas[(int)bStart]; 
				local.gulika_kala_index = this.gulika_kalas[(int)bStart];
				local.yama_kala_index = this.yama_kalas[(int)bStart];
			}

			if (opts.CalcLagnaCusps)
			{
				li = new ListViewItem();
				sweph.obtainLock(h);
				BodyPosition bp_lagna_sr = Basics.CalculateSingleBodyPosition(ut_sr, sweph.BodyNameToSweph(Body.Name.Lagna), Body.Name.Lagna, BodyType.Name.Lagna, h);
				DivisionPosition dp_lagna_sr = bp_lagna_sr.toDivisionPosition(new Division(Basics.DivisionType.Rasi));
				local.lagna_zh = dp_lagna_sr.zodiac_house.value;

				Longitude bp_lagna_base = new Longitude(bp_lagna_sr.longitude.toZodiacHouseBase());
				double ut_transit = ut_sr;
				for (int i=1; i<=12; i++)
				{
					Retrogression r = new Retrogression(h, Body.Name.Lagna);
					ut_transit = r.GetLagnaTransitForward(ut_transit, bp_lagna_base.add(i*30.0));

					PanchangaMomentInfo pmi = new PanchangaMomentInfo(
						ut_transit, (int)bp_lagna_sr.longitude.toZodiacHouse().add(i+1).value);
					local.lagnas_ut.Add(pmi);
				}

				sweph.releaseLock(h);
			}

			if (opts.CalcTithiCusps)
			{
				Transit t = new Transit(h);
				sweph.obtainLock(h);
				Tithi tithi_start = t.LongitudeOfTithi(ut_sr).toTithi();
				Tithi tithi_end = t.LongitudeOfTithi(ut_sr+1.0).toTithi();

				Tithi tithi_curr = tithi_start.add(1);
				local.tithi_index_start = globals.tithis_ut.Count-1;
				local.tithi_index_end = globals.tithis_ut.Count-1;

				while (tithi_start.value != tithi_end.value &&
					tithi_curr.value != tithi_end.value)
				{
					tithi_curr = tithi_curr.add(2);
					double dLonToFind = ((double)(int)tithi_curr.value-1)*(360.0/30.0);
					double ut_found = t.LinearSearchBinary(ut_sr, ut_sr+1.0, new Longitude(dLonToFind),
						new ReturnLon(t.LongitudeOfTithiDir));

					globals.tithis_ut.Add (new PanchangaMomentInfo(ut_found, (int)tithi_curr.value));
					local.tithi_index_end++;
				}
				sweph.releaseLock(h);
			}


			if (opts.CalcKaranaCusps)
			{
				Transit t = new Transit(h);
				sweph.obtainLock(h);
				Karana karana_start = t.LongitudeOfTithi(ut_sr).toKarana();
				Karana karana_end = t.LongitudeOfTithi(ut_sr+1.0).toKarana();

				Karana karana_curr = karana_start.add(1);
				local.karana_index_start = globals.karanas_ut.Count-1;
				local.karana_index_end = globals.karanas_ut.Count-1;

				while (karana_start.value != karana_end.value &&
					karana_curr.value != karana_end.value)
				{
					karana_curr = karana_curr.add(2);
					double dLonToFind = ((double)(int)karana_curr.value-1)*(360.0/60.0);
					double ut_found = t.LinearSearchBinary(ut_sr, ut_sr+1.0, new Longitude(dLonToFind),
						new ReturnLon(t.LongitudeOfTithiDir));

					globals.karanas_ut.Add (new PanchangaMomentInfo(ut_found, (int)karana_curr.value));
					local.karana_index_end++;
				}
				sweph.releaseLock(h);
			}

			if (opts.CalcSMYogaCusps)
			{
				Transit t = new Transit(h);
				sweph.obtainLock(h);
				SunMoonYoga sm_start = t.LongitudeOfSunMoonYoga(ut_sr).toSunMoonYoga();
				SunMoonYoga sm_end = t.LongitudeOfSunMoonYoga(ut_sr+1.0).toSunMoonYoga();

				SunMoonYoga sm_curr = sm_start.add(1);
				local.smyoga_index_start = globals.smyogas_ut.Count-1;
				local.smyoga_index_end = globals.smyogas_ut.Count-1;

				while (sm_start.value != sm_end.value &&
					sm_curr.value != sm_end.value)
				{
					sm_curr = sm_curr.add(2);
					double dLonToFind = ((double)(int)sm_curr.value-1)*(360.0/27);
					double ut_found = t.LinearSearchBinary(ut_sr, ut_sr+1.0, new Longitude(dLonToFind),
						new ReturnLon(t.LongitudeOfSunMoonYogaDir));

					globals.smyogas_ut.Add(new PanchangaMomentInfo(ut_found, (int)sm_curr.value));
					local.smyoga_index_end++;
				}
				
				sweph.releaseLock(h);
			}


			if (opts.CalcNakCusps)
			{
				bool bDiscard = true;
				Transit t = new Transit(h, Body.Name.Moon);
				sweph.obtainLock(h);
				Nakshatra nak_start = t.GenericLongitude(ut_sr, ref bDiscard).toNakshatra();
				Nakshatra nak_end = t.GenericLongitude(ut_sr+1.0, ref bDiscard).toNakshatra();

				local.nakshatra_index_start = globals.nakshatras_ut.Count-1;
				local.nakshatra_index_end = globals.nakshatras_ut.Count-1;

				Nakshatra nak_curr = nak_start.add(1);

				while (nak_start.value != nak_end.value &&
					nak_curr.value != nak_end.value)
				{
					nak_curr = nak_curr.add(2);
					double dLonToFind = ((double)((int)nak_curr.value-1))*(360.0/27.0);
					double ut_found = t.LinearSearchBinary(ut_sr, ut_sr+1.0, new Longitude(dLonToFind),
						new ReturnLon(t.GenericLongitude));

					globals.nakshatras_ut.Add(new PanchangaMomentInfo(ut_found, (int)nak_curr.value));
					Console.WriteLine ("Found nakshatra {0}", nak_curr.value);
					local.nakshatra_index_end++;
				} 
				sweph.releaseLock(h);
			}

			if (opts.CalcHoraCusps)
			{
				local.horas_ut = hCurr.getHoraCuspsUt();
				hCurr.calculateHora(ut_sr+1.0/24.0, ref local.hora_base);
			}

			if (opts.CalcKalaCusps)
			{
				hCurr.calculateKala(ref local.kala_base);
			}


			this.locals.Add(local);
			this.DisplayEntry(local);
		}
		
		private void DisplayEntry (PanchangaLocalMoments local)
		{
			string s;
			int day=0, month=0, year=0;
			double time=0;

			sweph.swe_revjul(local.sunrise_ut, ref year, ref month, ref day, ref time);
			Moment m = new Moment(year, month, day, time);
			this.mList.Items.Add(string.Format("{0}, {1}", local.wday, m.ToDateString()));

			if (this.opts.ShowSunriset)
			{
				s = string.Format("Sunrise at {0}. Sunset at {1}",
					this.timeToString(local.sunrise),
					this.timeToString(local.sunset));
				this.mList.Items.Add(s);
			}

			if (this.opts.CalcSpecialKalas)
			{
					
				string s_rahu = string.Format("Rahu Kala from {0} to {1}",
					new Moment(local.kalas_ut[local.rahu_kala_index], h).ToTimeString(), 
					new Moment(local.kalas_ut[local.rahu_kala_index+1], h).ToTimeString());
				string s_gulika = string.Format("Gulika Kala from {0} to {1}",
					new Moment(local.kalas_ut[local.gulika_kala_index], h).ToTimeString(), 
					new Moment(local.kalas_ut[local.gulika_kala_index+1], h).ToTimeString());
				string s_yama = string.Format("Yama Kala from {0} to {1}",
					new Moment(local.kalas_ut[local.yama_kala_index], h).ToTimeString(), 
					new Moment(local.kalas_ut[local.yama_kala_index+1], h).ToTimeString());

				if (opts.OneEntryPerLine)
				{
					this.mList.Items.Add(s_rahu);
					this.mList.Items.Add(s_gulika);
					this.mList.Items.Add(s_yama);
				}
				else
					this.mList.Items.Add (string.Format("{0}. {1}. {2}.", s_rahu, s_gulika, s_yama));
			}

			if (this.opts.CalcTithiCusps)
			{
				string s_tithi = "";

				if (local.tithi_index_start == local.tithi_index_end &&
					local.tithi_index_start >= 0)
				{
					PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.tithis_ut[local.tithi_index_start];
					Tithi t = new Tithi((Tithi.Name)pmi.info);
					this.mList.Items.Add (string.Format("{0} - full.", t.value));
				}
				else
				{
					for (int i=local.tithi_index_start+1; i<=local.tithi_index_end; i++)
					{
						if (i < 0)
							continue;
						PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.tithis_ut[i];
						Tithi t = new Tithi((Tithi.Name)pmi.info).addReverse(2);
						s_tithi += string.Format("{0} until {1}", 
							t.value,
							this.utTimeToString(pmi.ut, local.sunrise_ut, local.sunrise));

						if (this.opts.OneEntryPerLine)
						{
							this.mList.Items.Add (s_tithi);
							s_tithi = "";
						} 
						else
							s_tithi += ". ";
					}
					if (false == opts.OneEntryPerLine)
						this.mList.Items.Add(s_tithi);
				}
			}


			if (this.opts.CalcKaranaCusps)
			{
				string s_karana = "";

				if (local.karana_index_start == local.karana_index_end &&
					local.karana_index_start >= 0)
				{
					PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.karanas_ut[local.karana_index_start];
					Karana k = new Karana((Karana.Name)pmi.info);
					this.mList.Items.Add (string.Format("{0} karana - full.", k.value));
				}
				else
				{
					for (int i=local.karana_index_start+1; i<=local.karana_index_end; i++)
					{
						if (i < 0)
							continue;
						PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.karanas_ut[i];
						Karana k = new Karana((Karana.Name)pmi.info).addReverse(2);
						s_karana += string.Format("{0} karana until {1}", 
							k.value,
							this.utTimeToString(pmi.ut, local.sunrise_ut, local.sunrise));

						if (this.opts.OneEntryPerLine)
						{
							this.mList.Items.Add (s_karana);
							s_karana = "";
						} 
						else
							s_karana += ". ";
					}
					if (false == opts.OneEntryPerLine)
						this.mList.Items.Add(s_karana);
				}
			}



			if (this.opts.CalcSMYogaCusps)
			{
				string s_smyoga = "";

				if (local.smyoga_index_start == local.smyoga_index_end &&
					local.smyoga_index_start >= 0)
				{
					PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.smyogas_ut[local.smyoga_index_start];
					SunMoonYoga sm = new SunMoonYoga((SunMoonYoga.Name)pmi.info);
					this.mList.Items.Add (string.Format("{0} yoga - full.", sm.value));
				}
				else
				{
					for (int i=local.smyoga_index_start+1; i<=local.smyoga_index_end; i++)
					{
						if (i < 0)
							continue;
						PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.smyogas_ut[i];
						SunMoonYoga sm = new SunMoonYoga((SunMoonYoga.Name)pmi.info).addReverse(2);
						s_smyoga += string.Format("{0} yoga until {1}", 
							sm.value,
							this.utTimeToString(pmi.ut, local.sunrise_ut, local.sunrise));

						if (this.opts.OneEntryPerLine)
						{
							this.mList.Items.Add (s_smyoga);
							s_smyoga = "";
						} 
						else
							s_smyoga += ". ";
					}
					if (false == opts.OneEntryPerLine)
						this.mList.Items.Add(s_smyoga);
				}
			}



			if (this.opts.CalcNakCusps)
			{
				string s_nak = "";

				if (local.nakshatra_index_start == local.nakshatra_index_end &&
					local.nakshatra_index_start >= 0)
				{
					PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.nakshatras_ut[local.nakshatra_index_start];
					Nakshatra n = new Nakshatra((Nakshatra.Name)pmi.info);
					this.mList.Items.Add(string.Format("{0} - full.", n.value));
				}
				else
				{
					for (int i=local.nakshatra_index_start+1; i<=local.nakshatra_index_end; i++)
					{
						if (i < 0)
							continue;
						PanchangaMomentInfo pmi = (PanchangaMomentInfo)globals.nakshatras_ut[i];
						Nakshatra n = new Nakshatra((Nakshatra.Name)pmi.info).addReverse(2);
						s_nak += string.Format("{0} until {1}",
							n.value,
							this.utTimeToString(pmi.ut, local.sunrise_ut, local.sunrise));
						if (this.opts.OneEntryPerLine)
						{
							this.mList.Items.Add (s_nak);
							s_nak = "";
						} 
						else
							s_nak += ". ";
					}
					if (false == opts.OneEntryPerLine)
						this.mList.Items.Add(s_nak);
				}
			}

			if (this.opts.CalcLagnaCusps)
			{
				string sLagna = "    ";
				ZodiacHouse zBase = new ZodiacHouse(local.lagna_zh);
				for (int i=0; i<12; i++)
				{
					PanchangaMomentInfo pmi = (PanchangaMomentInfo)local.lagnas_ut[i];
					ZodiacHouse zCurr = new ZodiacHouse((ZodiacHouse.Name)pmi.info);
					zCurr = zCurr.add(12);
					sLagna = string.Format ("{0}{1} Lagna until {2}. ", sLagna, zCurr.value,
						this.utTimeToString(pmi.ut, local.sunrise_ut, local.sunrise));
					if (opts.OneEntryPerLine || i % 4 == 3)
					{
						this.mList.Items.Add (sLagna);
						sLagna = "";
					}
				}
			}

			if (this.opts.CalcHoraCusps)
			{
				string sHora = "    ";
				for (int i=0; i<24; i++)
				{
					int ib = (int)Basics.normalize_exc_lower(0,7,local.hora_base+i);
					Body.Name bHora = h.horaOrder[ib];
					sHora = string.Format("{0}{1} hora until {2}. ", sHora, bHora, 
						this.utTimeToString(local.horas_ut[i+1], local.sunrise_ut, local.sunrise));
					if (opts.OneEntryPerLine || i % 4 == 3)
					{
						this.mList.Items.Add (sHora);
						sHora = "";
					} 
				}
			}

			if (this.opts.CalcKalaCusps)
			{
				string sKala = "    ";
				for (int i=0; i<16; i++)
				{
					int ib = (int)Basics.normalize_exc_lower(0, 8,local.kala_base+i);
					Body.Name bKala = h.kalaOrder[ib];
					sKala = string.Format("{0}{1} kala until {2}. ", sKala, bKala,
						this.utTimeToString(local.kalas_ut[i+1], local.sunrise_ut, local.sunrise));
					if (opts.OneEntryPerLine || i % 4 == 3)
					{
						this.mList.Items.Add (sKala);
						sKala = "";
					}
				}
			}

			this.mList.Items.Add("");
		}

		private void mList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		protected override void copyToClipboard()
		{
			string s="";
			foreach (ListViewItem li in this.mList.Items)
			{
				s += li.Text + "\r\n";
			}
			Clipboard.SetDataObject(s, false);
		}

		private void menuCopyToClipboard_Click(object sender, System.EventArgs e)
		{
			this.copyToClipboard();
		}

		private void checkPrintReqs ()
		{
			if (opts.CalcKalaCusps == false ||
				opts.CalcNakCusps == false ||
				opts.CalcTithiCusps == false ||
				opts.CalcSpecialKalas == false ||
				opts.CalcSMYogaCusps == false ||
				opts.CalcKaranaCusps == false)
			{
				MessageBox.Show("Not enough information calculated to show panchanga");
				throw new Exception();
			}
		}
		private void menuItemPrintPanchanga_Click(object sender, System.EventArgs e)
		{
			try 
			{
				//this.checkPrintReqs();
				PanchangaPrintDocument mdoc = new PanchangaPrintDocument(opts, h, globals, locals);
				PrintDialog dlgPrint = new PrintDialog();
				dlgPrint.Document = mdoc;

				if (dlgPrint.ShowDialog() == DialogResult.OK)
					mdoc.Print();		
			}
			catch {}
		}

		private void menuItemFilePrintPreview_Click(object sender, System.EventArgs e)
		{
			try
			{
				//this.checkPrintReqs();
				PanchangaPrintDocument mdoc = new PanchangaPrintDocument(opts, h, globals, locals);
				PrintPreviewDialog dlgPreview = new PrintPreviewDialog();
				dlgPreview.Document = mdoc;
				dlgPreview.ShowDialog();
			} 
			catch {}
		}



	}


	public class PanchangaMomentInfo
	{
		public double ut;
		public int info;
		public PanchangaMomentInfo (double _ut, int _info)
		{
			ut = _ut;
			info = _info;
		}
	}

	public class PanchangaGlobalMoments
	{
		public ArrayList nakshatras_ut = new ArrayList();
		public ArrayList tithis_ut = new ArrayList();
		public ArrayList karanas_ut = new ArrayList();
		public ArrayList smyogas_ut = new ArrayList();
		public PanchangaGlobalMoments()
		{
		}
	}

	public class PanchangaLocalMoments 
	{
		public double sunrise;
		public double sunset;
		public Basics.Weekday wday;
		public double sunrise_ut;
		public double[] kalas_ut;
		public double[] horas_ut;
		public ArrayList lagnas_ut = new ArrayList();
		public ZodiacHouse.Name lagna_zh;
		public int rahu_kala_index;
		public int gulika_kala_index;
		public int yama_kala_index;
		public int kala_base;
		public int hora_base;
		public int nakshatra_index_start;
		public int nakshatra_index_end;
		public int tithi_index_start;
		public int tithi_index_end;
		public int karana_index_start;
		public int karana_index_end;
		public int smyoga_index_start;
		public int smyoga_index_end;
	}

}

