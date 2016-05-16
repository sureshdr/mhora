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


	public class TransitSearch : mhora.MhoraControl
	{
		private System.Windows.Forms.ColumnHeader Moment;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ColumnHeader Name1;
		private System.Windows.Forms.ContextMenu mContext;
		private System.Windows.Forms.MenuItem mOpenTransit;
		public System.Windows.Forms.PropertyGrid pgOptions;
		private System.Windows.Forms.ColumnHeader Event;
		private System.Windows.Forms.ColumnHeader Date;
		private System.Windows.Forms.ListView mlTransits;
		private System.Windows.Forms.Button bLocalSolarEclipse;
		private System.Windows.Forms.Button bSolarNewYear;
		private System.Windows.Forms.Button bProgressionLon;
		private System.Windows.Forms.Button bClearResults;
		private System.Windows.Forms.Button bNow;
		private System.Windows.Forms.Button bStartSearch;
		private System.Windows.Forms.Button bRetroCusp;
		private System.Windows.Forms.Button bProgression;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button bTransitPrevVarga;
		private System.Windows.Forms.Button bVargaChange;
		private System.Windows.Forms.Button bTransitNextVarga;
		private System.Windows.Forms.Button bGlobSolEcl;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button bGlobalLunarEclipse;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.MenuItem mOpenTransitNext;
		private System.Windows.Forms.MenuItem mOpenTransitChartPrev;

		TransitSearchOptions opts;

		public void Redisplay (object o)
		{
			this.mlTransits.Font = MhoraGlobalOptions.Instance.GeneralFont;
			this.mlTransits.BackColor = MhoraGlobalOptions.Instance.TableBackgroundColor;
			this.mlTransits.ForeColor = MhoraGlobalOptions.Instance.TableForegroundColor;
			this.pgOptions.Font = MhoraGlobalOptions.Instance.GeneralFont;
		}
		public void Reset ()
		{
			this.updateOptions();
			this.mlTransits.Items.Clear();
			this.Redisplay(MhoraGlobalOptions.Instance);
		
			//this.mlTransits.Font = MhoraGlobalOptions.Instance.GeneralFont;
			//this.mlTransits.BackColor = MhoraGlobalOptions.Instance.DasaBackgroundColor;
		}

		public TransitSearch(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			h = _h;
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(Redisplay);
			opts = new TransitSearchOptions();
			this.updateOptions();
			this.AddViewsToContextMenu(this.mContext);
			
			ToolTip tt = null;

			tt = new ToolTip();
			tt.SetToolTip(this.bTransitPrevVarga, "Find when the graha goes to the previous rasi only");
			tt = new ToolTip();
			tt.SetToolTip(this.bTransitNextVarga, "Find when the graha goes to the next rasi only");
			tt = new ToolTip();
			tt.SetToolTip(this.bVargaChange, "Find when the graha changes rasis");
			
			// TODO: Add any initialization after the InitializeComponent call
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
			this.Name1 = new System.Windows.Forms.ColumnHeader();
			this.Moment = new System.Windows.Forms.ColumnHeader();
			this.mContext = new System.Windows.Forms.ContextMenu();
			this.mOpenTransit = new System.Windows.Forms.MenuItem();
			this.pgOptions = new System.Windows.Forms.PropertyGrid();
			this.mlTransits = new System.Windows.Forms.ListView();
			this.Event = new System.Windows.Forms.ColumnHeader();
			this.Date = new System.Windows.Forms.ColumnHeader();
			this.bLocalSolarEclipse = new System.Windows.Forms.Button();
			this.bSolarNewYear = new System.Windows.Forms.Button();
			this.bProgressionLon = new System.Windows.Forms.Button();
			this.bClearResults = new System.Windows.Forms.Button();
			this.bNow = new System.Windows.Forms.Button();
			this.bStartSearch = new System.Windows.Forms.Button();
			this.bRetroCusp = new System.Windows.Forms.Button();
			this.bProgression = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bTransitPrevVarga = new System.Windows.Forms.Button();
			this.bVargaChange = new System.Windows.Forms.Button();
			this.bTransitNextVarga = new System.Windows.Forms.Button();
			this.bGlobSolEcl = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bGlobalLunarEclipse = new System.Windows.Forms.Button();
			this.mOpenTransitNext = new System.Windows.Forms.MenuItem();
			this.mOpenTransitChartPrev = new System.Windows.Forms.MenuItem();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// Name1
			// 
			this.Name1.Text = "Type";
			this.Name1.Width = 269;
			// 
			// Moment
			// 
			this.Moment.Text = "Moment";
			this.Moment.Width = 188;
			// 
			// mContext
			// 
			this.mContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mOpenTransit,
																					 this.mOpenTransitNext,
																					 this.mOpenTransitChartPrev});
			this.mContext.Popup += new System.EventHandler(this.mContext_Popup);
			// 
			// mOpenTransit
			// 
			this.mOpenTransit.Index = 0;
			this.mOpenTransit.Text = "Open Transit Chart";
			this.mOpenTransit.Click += new System.EventHandler(this.mOpenTransit_Click);
			// 
			// pgOptions
			// 
			this.pgOptions.CommandsVisibleIfAvailable = true;
			this.pgOptions.HelpVisible = false;
			this.pgOptions.LargeButtons = false;
			this.pgOptions.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgOptions.Location = new System.Drawing.Point(16, 8);
			this.pgOptions.Name = "pgOptions";
			this.pgOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.pgOptions.Size = new System.Drawing.Size(296, 152);
			this.pgOptions.TabIndex = 5;
			this.pgOptions.Text = "Options";
			this.pgOptions.ToolbarVisible = false;
			this.pgOptions.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pgOptions.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.pgOptions.Click += new System.EventHandler(this.pGrid_Click);
			// 
			// mlTransits
			// 
			this.mlTransits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mlTransits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.Event,
																						 this.Date});
			this.mlTransits.FullRowSelect = true;
			this.mlTransits.Location = new System.Drawing.Point(16, 168);
			this.mlTransits.Name = "mlTransits";
			this.mlTransits.Size = new System.Drawing.Size(648, 208);
			this.mlTransits.TabIndex = 9;
			this.mlTransits.View = System.Windows.Forms.View.Details;
			this.mlTransits.SelectedIndexChanged += new System.EventHandler(this.mlTransits_SelectedIndexChanged_1);
			// 
			// Event
			// 
			this.Event.Text = "Event";
			this.Event.Width = 387;
			// 
			// Date
			// 
			this.Date.Text = "Date";
			this.Date.Width = 173;
			// 
			// bLocalSolarEclipse
			// 
			this.bLocalSolarEclipse.Location = new System.Drawing.Point(8, 40);
			this.bLocalSolarEclipse.Name = "bLocalSolarEclipse";
			this.bLocalSolarEclipse.Size = new System.Drawing.Size(104, 23);
			this.bLocalSolarEclipse.TabIndex = 13;
			this.bLocalSolarEclipse.Text = "(L) Sol. Ecl.";
			this.bLocalSolarEclipse.Click += new System.EventHandler(this.bLocSolEclipse_Click);
			// 
			// bSolarNewYear
			// 
			this.bSolarNewYear.Location = new System.Drawing.Point(152, 168);
			this.bSolarNewYear.Name = "bSolarNewYear";
			this.bSolarNewYear.Size = new System.Drawing.Size(104, 23);
			this.bSolarNewYear.TabIndex = 9;
			this.bSolarNewYear.Text = "Solar Year";
			this.bSolarNewYear.Click += new System.EventHandler(this.bSolarNewYear_Click);
			// 
			// bProgressionLon
			// 
			this.bProgressionLon.Location = new System.Drawing.Point(152, 136);
			this.bProgressionLon.Name = "bProgressionLon";
			this.bProgressionLon.Size = new System.Drawing.Size(104, 23);
			this.bProgressionLon.TabIndex = 8;
			this.bProgressionLon.Text = "Progress Lons";
			this.bProgressionLon.Click += new System.EventHandler(this.bProgressionLon_Click);
			// 
			// bClearResults
			// 
			this.bClearResults.Location = new System.Drawing.Point(16, 16);
			this.bClearResults.Name = "bClearResults";
			this.bClearResults.Size = new System.Drawing.Size(104, 23);
			this.bClearResults.TabIndex = 3;
			this.bClearResults.Text = "Clear Results";
			this.bClearResults.Click += new System.EventHandler(this.bClearResults_Click);
			// 
			// bNow
			// 
			this.bNow.Location = new System.Drawing.Point(16, 40);
			this.bNow.Name = "bNow";
			this.bNow.Size = new System.Drawing.Size(104, 23);
			this.bNow.TabIndex = 7;
			this.bNow.Text = "Now";
			this.bNow.Click += new System.EventHandler(this.bNow_Click);
			// 
			// bStartSearch
			// 
			this.bStartSearch.Location = new System.Drawing.Point(16, 72);
			this.bStartSearch.Name = "bStartSearch";
			this.bStartSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.bStartSearch.Size = new System.Drawing.Size(104, 23);
			this.bStartSearch.TabIndex = 2;
			this.bStartSearch.Text = "Find Transit";
			this.bStartSearch.Click += new System.EventHandler(this.bStartSearch_Click);
			// 
			// bRetroCusp
			// 
			this.bRetroCusp.Location = new System.Drawing.Point(16, 96);
			this.bRetroCusp.Name = "bRetroCusp";
			this.bRetroCusp.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.bRetroCusp.Size = new System.Drawing.Size(104, 23);
			this.bRetroCusp.TabIndex = 4;
			this.bRetroCusp.Text = "Find Retro";
			this.bRetroCusp.Click += new System.EventHandler(this.bRetroCusp_Click);
			// 
			// bProgression
			// 
			this.bProgression.Location = new System.Drawing.Point(152, 112);
			this.bProgression.Name = "bProgression";
			this.bProgression.Size = new System.Drawing.Size(104, 23);
			this.bProgression.TabIndex = 6;
			this.bProgression.Text = "Progress Time";
			this.bProgression.Click += new System.EventHandler(this.bProgression_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bTransitPrevVarga);
			this.groupBox1.Controls.Add(this.bVargaChange);
			this.groupBox1.Controls.Add(this.bTransitNextVarga);
			this.groupBox1.Location = new System.Drawing.Point(144, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(120, 96);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Change Varga";
			// 
			// bTransitPrevVarga
			// 
			this.bTransitPrevVarga.Location = new System.Drawing.Point(8, 16);
			this.bTransitPrevVarga.Name = "bTransitPrevVarga";
			this.bTransitPrevVarga.Size = new System.Drawing.Size(104, 23);
			this.bTransitPrevVarga.TabIndex = 12;
			this.bTransitPrevVarga.Text = "<-- Prev";
			this.bTransitPrevVarga.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bTransitPrevVarga.Click += new System.EventHandler(this.bTransitPrevVarga_Click);
			// 
			// bVargaChange
			// 
			this.bVargaChange.Location = new System.Drawing.Point(8, 40);
			this.bVargaChange.Name = "bVargaChange";
			this.bVargaChange.Size = new System.Drawing.Size(104, 23);
			this.bVargaChange.TabIndex = 13;
			this.bVargaChange.Text = "Change";
			this.bVargaChange.Click += new System.EventHandler(this.bVargaChange_Click);
			// 
			// bTransitNextVarga
			// 
			this.bTransitNextVarga.Location = new System.Drawing.Point(8, 64);
			this.bTransitNextVarga.Name = "bTransitNextVarga";
			this.bTransitNextVarga.Size = new System.Drawing.Size(104, 23);
			this.bTransitNextVarga.TabIndex = 10;
			this.bTransitNextVarga.Text = "Next  -->";
			this.bTransitNextVarga.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.bTransitNextVarga.Click += new System.EventHandler(this.bTransitNextVarga_Click);
			// 
			// bGlobSolEcl
			// 
			this.bGlobSolEcl.Location = new System.Drawing.Point(8, 64);
			this.bGlobSolEcl.Name = "bGlobSolEcl";
			this.bGlobSolEcl.Size = new System.Drawing.Size(104, 23);
			this.bGlobSolEcl.TabIndex = 11;
			this.bGlobSolEcl.Text = "(G) Sol. Ecl.";
			this.bGlobSolEcl.Click += new System.EventHandler(this.bGlobSolEclipse_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.bSolarNewYear);
			this.panel1.Controls.Add(this.bProgressionLon);
			this.panel1.Controls.Add(this.bClearResults);
			this.panel1.Controls.Add(this.bNow);
			this.panel1.Controls.Add(this.bStartSearch);
			this.panel1.Controls.Add(this.bRetroCusp);
			this.panel1.Controls.Add(this.bProgression);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Location = new System.Drawing.Point(320, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(304, 152);
			this.panel1.TabIndex = 8;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.bLocalSolarEclipse);
			this.groupBox2.Controls.Add(this.bGlobSolEcl);
			this.groupBox2.Controls.Add(this.bGlobalLunarEclipse);
			this.groupBox2.Location = new System.Drawing.Point(8, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(120, 96);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Eclipses";
			// 
			// bGlobalLunarEclipse
			// 
			this.bGlobalLunarEclipse.Location = new System.Drawing.Point(8, 16);
			this.bGlobalLunarEclipse.Name = "bGlobalLunarEclipse";
			this.bGlobalLunarEclipse.Size = new System.Drawing.Size(104, 23);
			this.bGlobalLunarEclipse.TabIndex = 14;
			this.bGlobalLunarEclipse.Text = "Lunar Ecl.";
			this.bGlobalLunarEclipse.Click += new System.EventHandler(this.bGlobalLunarEclipse_Click);
			// 
			// mOpenTransitNext
			// 
			this.mOpenTransitNext.Index = 1;
			this.mOpenTransitNext.Text = "Open Transit Chart (Compress &Next)";
			this.mOpenTransitNext.Click += new System.EventHandler(this.mOpenTransitNext_Click);
			// 
			// mOpenTransitChartPrev
			// 
			this.mOpenTransitChartPrev.Index = 2;
			this.mOpenTransitChartPrev.Text = "Open Transit Chart (Compress &Prev)";
			this.mOpenTransitChartPrev.Click += new System.EventHandler(this.mOpenTransitChartPrev_Click);
			// 
			// TransitSearch
			// 
			this.ContextMenu = this.mContext;
			this.Controls.Add(this.pgOptions);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.mlTransits);
			this.Name = "TransitSearch";
			this.Size = new System.Drawing.Size(672, 384);
			this.Load += new System.EventHandler(this.TransitSearch_Load);
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void TransitSearch_Load(object sender, System.EventArgs e)
		{
			this.Reset();

		}

		public object SetOptions (Object o) 
		{
			return this.opts.CopyFrom(o);
		}
		private void bOpts_Click(object sender, System.EventArgs e)
		{
			MhoraOptions f = new MhoraOptions(this.opts, new ApplyOptions(SetOptions));
			f.ShowDialog();
		}


		private Horoscope utToHoroscope (double found_ut, ref Moment m2)
		{
			// turn into horoscope
			int year=0, month=0, day=0;
			double hour =0;
			found_ut += (h.info.tz.toDouble() / 24.0);
			sweph.swe_revjul(found_ut, ref year, ref month, ref day, ref hour);
			Moment m = new Moment(year, month, day, hour);
			HoraInfo inf = new HoraInfo(m, 
				(HMSInfo)h.info.lat.Clone(), 
				(HMSInfo)h.info.lon.Clone(), 
				(HMSInfo)h.info.tz.Clone());
			Horoscope hTransit = new Horoscope(inf, 
				(HoroscopeOptions)h.options.Clone());

			sweph.swe_revjul(found_ut+5.0, ref year, ref month, ref day, ref hour);
			m2 = new Moment(year, month, day, hour);
			return hTransit;
		}

		private void ApplyLocal (Moment m)
		{
			if (opts.Apply)
			{
				h.info.tob = m;
				h.OnChanged();
			}
		}
		private double DirectSpeed (Body.Name b)
		{
			switch (b)
			{
				case Body.Name.Sun: return 365.2425;
				case Body.Name.Moon: return 28.0;
				case Body.Name.Lagna: return 1.0;
			}
			return 0.0;
		}
		private void DirectProgression ()
		{
			if (opts.SearchBody != Body.Name.Sun &&
			opts.SearchBody != Body.Name.Moon) // &&
			//opts.SearchBody != Body.Name.Lagna)
			return;

			double julday_ut = this.opts.StartDate.toUniversalTime() - h.info.tz.toDouble()/24.0;
				//;.tob.time / 24.0;
				
			if (julday_ut <= h.baseUT)
			{
				MessageBox.Show("Error: Unable to progress in the future");
				return;
			}

			double totalProgression = this.GetProgressionDegree();
			double totalProgressionOrig = totalProgression;

			sweph.obtainLock(h);
			Retrogression r = new Retrogression(h, opts.SearchBody);

			Longitude start_lon = r.GetLon(h.baseUT);
			//Console.WriteLine ("Real start lon is {0}", start_lon);
			double curr_julday = h.baseUT;
			Transit t = new Transit(h, opts.SearchBody);
			while (totalProgression >= 360.0)
			{
				curr_julday = t.LinearSearch (
					curr_julday + DirectSpeed(opts.SearchBody),
					start_lon, new ReturnLon(t.GenericLongitude));
				totalProgression -= 360.0;
			}

			curr_julday = t.LinearSearch (
				curr_julday + (totalProgression / 360.0 * DirectSpeed(opts.SearchBody)),
				start_lon.add(totalProgression), new ReturnLon(t.GenericLongitude));


			//bool bDiscard = true;
			//Longitude got_lon = t.GenericLongitude(curr_julday, ref bDiscard);
			//Console.WriteLine ("Found Progressed Sun at {0}+{1}={2}={3}", 
			//	start_lon.value, new Longitude(totalProgressionOrig).value,
			//	got_lon.value, got_lon.sub(start_lon.add(totalProgressionOrig)).value
			//	);

			sweph.releaseLock(h);

			Moment m2 = new Moment(0,0,0,0.0);
			Horoscope hTransit = this.utToHoroscope(curr_julday, ref m2);
			string fmt = hTransit.info.DateOfBirth.ToString();
			ListViewItem li = new TransitItem(hTransit);
			li.Text = string.Format	("{0}'s Prog: {2}+{3:00.00} deg", 
				opts.SearchBody, totalProgressionOrig, 
				(int)Math.Floor(totalProgressionOrig/360.0),
				new Longitude(totalProgressionOrig).value);
			li.SubItems.Add(fmt);
			this.ApplyLocal(hTransit.info.tob);

			this.mlTransits.Items.Add (li);
			this.updateOptions();


		}
		private double GetProgressionDegree ()
		{
			double julday_ut = this.opts.StartDate.toUniversalTime() - h.info.tz.toDouble()/24.0;
			double ut_diff = julday_ut - h.baseUT;

			//Console.WriteLine ("Expected ut_diff is {0}", ut_diff);
			bool bDummy = true;
			sweph.obtainLock(h);
			Transit t = new Transit(h);
		    Longitude lon_start = t.LongitudeOfSun(h.baseUT, ref bDummy);
			Longitude lon_prog = t.LongitudeOfSun(julday_ut, ref bDummy);

			//Console.WriteLine ("Progression lons are {0} and {1}", lon_start, lon_prog);

			double dExpectedLon = ut_diff * 360.0 / 365.2425;	
			Longitude lon_expected = lon_start.add (dExpectedLon);
			sweph.releaseLock(h);


			if (Transit.CircLonLessThan(lon_expected, lon_prog))
				dExpectedLon += lon_prog.sub(lon_expected).value;
			else
				dExpectedLon -= lon_expected.sub(lon_prog).value;

			DivisionPosition dp = h.getPosition(opts.SearchBody).toDivisionPosition(opts.Division);

			//Console.WriteLine ("Sun progress {0} degrees in elapsed time", dExpectedLon);

			double ret = (dExpectedLon / 360.0) * (30.0/(double)(Basics.numPartsInDivision(opts.Division)));
				//(dp.cusp_higher - dp.cusp_lower);
			//Console.WriteLine ("Progressing by {0} degrees", ret);
			return ret;
		}
		private void bProgression_Click(object sender, System.EventArgs e)
		{
			if ((int)opts.SearchBody <= (int)Body.Name.Moon ||
				(int)opts.SearchBody > (int)Body.Name.Saturn)
			{
				DirectProgression();
				return;
			}

			DivisionPosition dp = h.getPosition(opts.SearchBody).toDivisionPosition(opts.Division);
			double yearlyProgression = (dp.cusp_higher - dp.cusp_lower) / 30.0;
			double julday_ut = sweph.swe_julday(
				this.opts.StartDate.year,
				this.opts.StartDate.month,
				this.opts.StartDate.day,
				this.opts.StartDate.hour + (((double)this.opts.StartDate.minute)/60.0)
				+ (((double)this.opts.StartDate.second)/3600.0));			
			
			if (julday_ut <= h.baseUT)
			{
				MessageBox.Show("Error: Unable to progress in the future");
				return;
			}


			double totalProgression = this.GetProgressionDegree();
			double totalProgressionOrig = totalProgression;

			//Console.WriteLine ("Total Progression is {0}", totalProgression);
			bool becomesDirect = false;
			sweph.obtainLock(h);
			Retrogression r = new Retrogression(h, this.opts.SearchBody);
			double curr_ut = h.baseUT;
			double next_ut=0;
			double found_ut = h.baseUT;
			while (true)
			{
				next_ut = r.findNextCuspForward(curr_ut, ref becomesDirect);
				Longitude curr_lon = r.GetLon(curr_ut);
				Longitude next_lon = r.GetLon(next_ut);


				if (false == becomesDirect && next_lon.sub(curr_lon).value >= totalProgression)
				{
					//Console.WriteLine ("1 Found {0} in {1}", totalProgression, next_lon.sub(curr_lon).value);
					found_ut = r.GetTransitForward(curr_ut, curr_lon.add(totalProgression));
					break;
				} 
				else if (true == becomesDirect && curr_lon.sub(next_lon).value >= totalProgression)
				{
					//Console.WriteLine ("2 Found {0} in {1}", totalProgression, curr_lon.sub(next_lon).value);
					found_ut = r.GetTransitForward(curr_ut, curr_lon.sub(totalProgression));
					break;
				}
				if (false == becomesDirect)
				{
					//Console.WriteLine ("Progression: {0} degrees gone in direct motion", next_lon.sub(curr_lon).value);
					totalProgression -= next_lon.sub(curr_lon).value;
				}
				else
				{
					//Console.WriteLine ("Progression: {0} degrees gone in retro motion", curr_lon.sub(next_lon).value);
					totalProgression -= curr_lon.sub(next_lon).value;
				}
				curr_ut = next_ut + 5.0;
			}
			sweph.releaseLock(h);

			Moment m2 = new Moment(0,0,0,0.0);
			Horoscope hTransit = this.utToHoroscope(found_ut, ref m2);
			string fmt = hTransit.info.DateOfBirth.ToString();
			ListViewItem li = new TransitItem(hTransit);
			li.Text = string.Format	("{0}'s Prog: {2}+{3:00.00} deg", 
				opts.SearchBody, totalProgressionOrig, 
				(int)Math.Floor(totalProgressionOrig/360.0),
				new Longitude(totalProgressionOrig).value);
			li.SubItems.Add(fmt);
			this.mlTransits.Items.Add (li);
			this.updateOptions();
			this.ApplyLocal(hTransit.info.tob);
		}

		private void bProgressionLon_Click(object sender, System.EventArgs e)
		{
			if (opts.Apply == false)
			{
				MessageBox.Show("This will modify the current chart. You must set Apply to 'true'");
				return;
			}
			h.OnChanged();
			double degToProgress = this.GetProgressionDegree();
			Longitude lonProgress = new Longitude(degToProgress);

			foreach (BodyPosition bp in h.positionList)
			{
				bp.longitude = bp.longitude.add(lonProgress);
			}
			h.OnlySignalChanged();
		}

		private void bRetroCusp_Click(object sender, System.EventArgs e)
		{
			if ((int)opts.SearchBody <= (int)Body.Name.Moon ||
				(int)opts.SearchBody > (int)Body.Name.Saturn)
				return;

			bool becomesDirect = false;
			sweph.obtainLock(h);
			Retrogression r = new Retrogression(h, this.opts.SearchBody);
			double julday_ut = this.opts.StartDate.toUniversalTime() - h.info.tz.toDouble()/24.0;
			double found_ut = julday_ut;
			if (opts.Forward)
			{
				found_ut = r.findNextCuspForward(julday_ut, ref becomesDirect);
			}
			else
				found_ut  = r.findNextCuspBackward(julday_ut, ref becomesDirect);


			bool bForward = false;
			Longitude found_lon = r.GetLon(found_ut, ref bForward);
			sweph.releaseLock(h);

			// turn into horoscope
			int year=0, month=0, day=0;
			double hour =0;
			found_ut += (h.info.tz.toDouble() / 24.0);
			sweph.swe_revjul(found_ut, ref year, ref month, ref day, ref hour);
			Moment m = new Moment(year, month, day, hour);
			HoraInfo inf = new HoraInfo(m, 
				(HMSInfo)h.info.lat.Clone(), 
				(HMSInfo)h.info.lon.Clone(), 
				(HMSInfo)h.info.tz.Clone());
			Horoscope hTransit = new Horoscope(inf, 
				(HoroscopeOptions)h.options.Clone());

			if (opts.Forward)
				sweph.swe_revjul(found_ut+5.0, ref year, ref month, ref day, ref hour);
			else
				sweph.swe_revjul(found_ut-5.0, ref year, ref month, ref day, ref hour);
			Moment m2 = new Moment(year, month, day, hour);
			this.opts.StartDate = m2;
			// add entry to our list
			string fmt = hTransit.info.DateOfBirth.ToString();
			ListViewItem li = new TransitItem(hTransit);
			li.Text = Body.toString(this.opts.SearchBody);
			if (becomesDirect)
				li.Text += " goes direct at " + found_lon.ToString();
			else
				li.Text += " goes retrograde at " + found_lon.ToString();
			li.SubItems.Add(fmt);
			this.mlTransits.Items.Add (li);
			this.updateOptions();
			this.ApplyLocal(hTransit.info.tob);

		}

		private void bStartSearch_Click(object sender, System.EventArgs e)
		{
			this.StartSearch(true);
		}

		private void UpdateDateForNextSearch (double ut)
		{
			int year=0, month=0, day=0;
			double hour=0;

			double offset = 10.0 / (24.0*60.0*60.0);
			if (opts.Forward == true)
				ut += offset;
			else
				ut -= offset;


			sweph.swe_revjul(ut, ref year, ref month, ref day, ref hour);
			Moment m2 = new Moment(year, month, day, hour);
			this.opts.StartDate = m2;
			this.updateOptions();
		}





		private double StartSearch (bool bUpdateDate)
		{
			CuspTransitSearch cs = new CuspTransitSearch(h);
			Longitude found_lon = opts.TransitPoint.add(0);
			bool bForward = true;
			double found_ut = 
				cs.TransitSearch(opts.SearchBody, opts.StartDate, opts.Forward, opts.TransitPoint,
				found_lon, ref bForward);
			

			Moment m2 = new Moment(0,0,0,0);
			Horoscope hTransit = this.utToHoroscope(found_ut, ref m2);
			this.UpdateDateForNextSearch(found_ut);

			// add entry to our list
			string fmt = hTransit.info.DateOfBirth.ToString();
			ListViewItem li = new TransitItem(hTransit);
			li.Text = Body.toString(this.opts.SearchBody);
			if (bForward == false)
				li.Text += " (R)";
			li.Text += " transits " + found_lon.ToString();

			li.SubItems.Add(fmt);
			this.mlTransits.Items.Add (li);
			this.updateOptions();
			this.ApplyLocal(hTransit.info.tob);

			return found_ut;
		}


		private void mlTransits_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void mContext_Popup(object sender, System.EventArgs e)
		{
		
		}

		private void openTransitHelper (Horoscope hTransit)
		{
			hTransit.info.type = HoraInfo.Name.Transit;
			MhoraChild mcTransit = new MhoraChild(hTransit);
			mcTransit.Name = "Transit Chart";
			mcTransit.Text = "Transit Chart";
			mcTransit.MdiParent = (MhoraContainer)MhoraGlobalOptions.mainControl;
			mcTransit.Show();
		}

		private void mOpenTransit_Click(object sender, System.EventArgs e)
		{
			if (this.mlTransits.SelectedItems.Count == 0)
				return;

			TransitItem ti = (TransitItem)mlTransits.SelectedItems[0];
			Horoscope hTransit = ti.GetHoroscope();
			this.openTransitHelper(hTransit);
		}



		private void mOpenTransitNext_Click(object sender, System.EventArgs e)
		{
			if (this.mlTransits.SelectedItems.Count == 0)
				return;

			TransitItem ti = (TransitItem)mlTransits.SelectedItems[0];
			Horoscope hTransit = ti.GetHoroscope();

			int nextEntry = mlTransits.SelectedItems[0].Index+1;
			if (this.mlTransits.Items.Count >= nextEntry+1)
			{
				TransitItem tiNext = (TransitItem)mlTransits.Items[nextEntry];
				Horoscope hTransitNext = tiNext.GetHoroscope();

				double ut_diff = hTransitNext.baseUT - hTransit.baseUT;
				if (ut_diff > 0) 
				{
					hTransit.info.defaultYearCompression = 1;
					hTransit.info.defaultYearLength = ut_diff;
					hTransit.info.defaultYearType = ToDate.DateType.FixedYear;
				}
				
			}

			this.openTransitHelper(hTransit);
		}

		private void mOpenTransitChartPrev_Click(object sender, System.EventArgs e)
		{
			if (this.mlTransits.SelectedItems.Count == 0)
				return;

			TransitItem ti = (TransitItem)mlTransits.SelectedItems[0];
			Horoscope hTransit = ti.GetHoroscope();
			hTransit.info.type = HoraInfo.Name.Transit;

			int prevEntry = mlTransits.SelectedItems[0].Index-1;
			if (prevEntry >= 0)
			{
				TransitItem tiPrev = (TransitItem)mlTransits.Items[prevEntry];
				Horoscope hTransitPrev = tiPrev.GetHoroscope();

				double ut_diff = hTransit.baseUT - hTransitPrev.baseUT;
				if (ut_diff > 0) 
				{
					hTransit.info.defaultYearCompression = 1;
					hTransit.info.defaultYearLength = ut_diff;
					hTransit.info.defaultYearType = ToDate.DateType.FixedYear;
				}
				
			}

			this.openTransitHelper(hTransit);
		}


		private void cbGrahas_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}


		private void bClearResults_Click(object sender, System.EventArgs e)
		{
			this.mlTransits.Items.Clear();
		}

		private void pGrid_Click(object sender, System.EventArgs e)
		{
		
		}


		private void bNow_Click(object sender, System.EventArgs e)
		{
			System.DateTime now = DateTime.Now;
			Moment m = new Moment(now.Year, now.Month, now.Day, 
				now.Hour + ((double)now.Minute/60.0) + ((double)now.Second/3600.0));
			this.opts.StartDate = m;
			this.updateOptions();
		}

		private void bSolarNewYear_Click(object sender, System.EventArgs e)
		{
			this.opts.SearchBody = Body.Name.Sun;
			this.opts.TransitPoint.value = 0;
			this.updateOptions();
			this.bStartSearch_Click(sender, e);
		}

		private void bTransitPrevVarga_Click(object sender, System.EventArgs e)
		{
			Horoscope h2 = (Horoscope)h.Clone();
			h2.info.tob = this.opts.StartDate;
			h2.OnChanged();
			DivisionPosition dp = h2.getPosition(opts.SearchBody).toDivisionPosition(opts.Division);
			opts.TransitPoint = new Longitude(dp.cusp_lower);

			double found_ut = this.StartSearch(false) + h.info.tz.toDouble() / 24.0;
			this.UpdateDateForNextSearch(found_ut);
			this.updateOptions();
		}

		private void updateOptions ()
		{
			this.pgOptions.SelectedObject = new GlobalizedPropertiesWrapper(this.opts);
		}

		private void bTransitNextVarga_Click(object sender, System.EventArgs e)
		{
			// Update Search Parameters
			Horoscope h2 = (Horoscope)h.Clone();
			h2.info.tob = this.opts.StartDate;
			h2.OnChanged();
			DivisionPosition dp = h2.getPosition(opts.SearchBody).toDivisionPosition(opts.Division);
			opts.TransitPoint = new Longitude(dp.cusp_higher);
			opts.TransitPoint = opts.TransitPoint.add(1.0/(60.0*60.0*60.0));

			double found_ut = this.StartSearch(false) + h.info.tz.toDouble() / 24.0;
			this.UpdateDateForNextSearch(found_ut);
			this.updateOptions();
		}
		private void bVargaChange_Click(object sender, System.EventArgs e)
		{
			if (opts.SearchBody == Body.Name.Sun ||
				opts.SearchBody == Body.Name.Moon ||
				opts.SearchBody == Body.Name.Lagna)
			{
				if (opts.Forward == true)
					this.bTransitNextVarga_Click(sender, e);
				else
					this.bTransitPrevVarga_Click(sender, e);
				return;
			}

			Horoscope h2 = (Horoscope)h.Clone();
			h2.info.tob = this.opts.StartDate;
			h2.OnChanged();
			BodyPosition bp = h2.getPosition(opts.SearchBody);
			DivisionPosition dp = bp.toDivisionPosition(opts.Division);

			bool becomesDirect = false;
			bool bForward = false;
			sweph.obtainLock(h);
			Retrogression r = new Retrogression(h, this.opts.SearchBody);
			double julday_ut = this.opts.StartDate.toUniversalTime() - h.info.tz.toDouble()/24.0;
			double found_ut = julday_ut;
			bool bTransitForwardCusp = true;
			while (true)
			{
				if (opts.Forward)
					found_ut = r.findNextCuspForward(found_ut, ref becomesDirect);
				else
					found_ut = r.findNextCuspBackward(found_ut, ref becomesDirect);

				Longitude found_lon = r.GetLon(found_ut, ref bForward);


				if (new Longitude(dp.cusp_higher).isBetween(bp.longitude, found_lon))
				{
					bTransitForwardCusp = true;
					break;
				}

				if (new Longitude(dp.cusp_lower).isBetween(found_lon, bp.longitude))
				{
					bTransitForwardCusp = false;
					break;
				}
				
				if (opts.Forward)
					found_ut += 5.0;
				else
					found_ut -= 5.0;
			}
			sweph.releaseLock(h);
			if (opts.Forward)
				found_ut += 5.0;
			else
				found_ut -= 5.0;
			this.UpdateDateForNextSearch(found_ut);

			if (bTransitForwardCusp)
			{
				this.opts.TransitPoint.value = dp.cusp_higher;
				this.updateOptions();
				this.bStartSearch_Click(sender, e);
			} 
			else
			{
				this.opts.TransitPoint.value = dp.cusp_lower;
				this.updateOptions();
				this.bStartSearch_Click(sender, e);
			}
		}

		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}


		protected override void copyToClipboard()
		{
			string s="";
			foreach (ListViewItem li in this.mlTransits.Items)
			{
				foreach (ListViewItem.ListViewSubItem si in li.SubItems)
				{
					s += si.Text + ". ";
				}
				s += "\r\n";
				Clipboard.SetDataObject(s, true);
			}
		}

		private void SolarEclipseHelper (double ut, string desc)
		{
			SolarEclipseHelper (ut, ut-1, ut+1, desc);
		}
		private void SolarEclipseHelper (double ut, double start, double end, string desc)
		{
			if (ut < start || ut > end)
				return;

			ListViewItem li = new ListViewItem(desc);
			Moment m = new Moment(0,0,0,0);
		    Horoscope hTransit = this.utToHoroscope(ut, ref m);
			li.SubItems.Add (hTransit.info.tob.ToString());
			this.mlTransits.Items.Add(li);
		}
		private void bGlobSolEclipse_Click(object sender, System.EventArgs e)
		{
			double julday_ut = this.opts.StartDate.toUniversalTime(h);
			double[] tret = new Double[10];
			sweph.obtainLock(h);
			sweph.swe_sol_eclipse_when_glob (julday_ut, tret, opts.Forward);
			sweph.releaseLock(h);
			this.SolarEclipseHelper(tret[2], "Global Solar Eclipse Begins");
			this.SolarEclipseHelper(tret[3], "   Global Solar Eclipse Ends");
			this.SolarEclipseHelper(tret[4], tret[2], tret[3], "   Global Solar Eclipse Totality Begins");
			this.SolarEclipseHelper(tret[5], tret[2], tret[3], "   Global Solar Eclipse Totality Ends");
			this.SolarEclipseHelper(tret[0], "   Global Solar Eclipse Maximum");
			this.SolarEclipseHelper(tret[6], tret[2], tret[3], "   Global Solar Eclipse Centerline Begins");
			this.SolarEclipseHelper(tret[7], tret[2], tret[3], "   Global Solar Eclipse Centerline Begins");
			if (opts.Forward)
				opts.StartDate = new Moment(tret[3] + 1.0, h);
			else 
				opts.StartDate = new Moment(tret[2] - 1.0, h);
			this.updateOptions();
		}
		private void bLocSolEclipse_Click(object sender, System.EventArgs e)
		{
			double julday_ut = this.opts.StartDate.toUniversalTime(h);
			double[] tret = new double[10];
			double[] attr = new double[10];
			sweph.obtainLock(h);
			sweph.swe_sol_eclipse_when_loc(h.info, julday_ut, tret, attr, opts.Forward);
			sweph.releaseLock(h);		
			this.SolarEclipseHelper(tret[0], "Local Solar Eclipse Maximum");
			this.SolarEclipseHelper(tret[1], tret[0]-1, tret[0]+1, "   Local Solar Eclipse 1st Contact");
			this.SolarEclipseHelper(tret[2], tret[0]-1, tret[0]+1, "   Local Solar Eclipse 2nd Contact");
			this.SolarEclipseHelper(tret[3], tret[0]-1, tret[0]+1, "   Local Solar Eclipse 3rd Contact");
			this.SolarEclipseHelper(tret[4], tret[0]-1, tret[0]+1, "   Local Solar Eclipse 4th Contact");
			if (opts.Forward)
				opts.StartDate = new Moment(tret[0] + 1.0, h);
			else
				opts.StartDate = new Moment(tret[0] - 1.0, h);
			this.updateOptions();
		}

		private void bGlobalLunarEclipse_Click(object sender, System.EventArgs e)
		{
			double julday_ut = this.opts.StartDate.toUniversalTime(h);
			double[] tret = new double[10];
			sweph.obtainLock(h);
			sweph.swe_lun_eclipse_when(julday_ut, tret, opts.Forward);
			sweph.releaseLock(h);
			this.SolarEclipseHelper(tret[0], "Lunar Eclipse Maximum");
			this.SolarEclipseHelper(tret[2], tret[0]-1, tret[0]+1, "   Lunar Eclipse Begins");
			this.SolarEclipseHelper(tret[3], tret[0]-1, tret[0]+1, "   Lunar Eclipse Ends");
			this.SolarEclipseHelper(tret[4], tret[0]-1, tret[0]+1, "   Lunar Total Eclipse Begins");
			this.SolarEclipseHelper(tret[5], tret[0]-1, tret[0]+1, "   Lunar Total Eclipse Ends");
			this.SolarEclipseHelper(tret[6], tret[0]-1, tret[0]+1, "   Lunar Penumbral Eclipse Begins");
			this.SolarEclipseHelper(tret[7], tret[0]-1, tret[0]+1, "   Lunar Penumbral Eclipse Ends");
			if (opts.Forward)
				opts.StartDate = new Moment(tret[0] + 1.0, h);
			else
				opts.StartDate = new Moment(tret[0] - 1.0, h);
			this.updateOptions();
		}

		private void mlTransits_SelectedIndexChanged_1(object sender, System.EventArgs e)
		{
		
		}


	}

	

	public class TransitSearchOptions: ICloneable
	{
		private Moment mDateBase;
		private bool mForward;
		private Body.Name mSearchGraha;
		private Longitude mTransitPoint;
		private Division mDivision;
		private bool mApplyNow;

		public TransitSearchOptions ()
		{
			DateTime dt = DateTime.Now;
			mDateBase = new Moment (dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			mSearchGraha = Body.Name.Sun;
			mTransitPoint = new Longitude(0.0);
			mForward = true;
			this.Division = new Division(Basics.DivisionType.Rasi);
		}



		[PGNotVisible]
		public Division Division
		{
			get { return this.mDivision; }
			set { this.mDivision = value; }
		}

		[Category("Transit Search")]
		[PropertyOrder(1),PGDisplayName("In Varga")]
		public Basics.DivisionType UIDivision
		{
			get { return this.mDivision.MultipleDivisions[0].Varga; }
			set { this.mDivision = new Division(value); }
		}

		public enum EForward
		{
			Before, After
		}

		[Category("Transit Search")]
		[PropertyOrder(2),PGDisplayName("Search")]
		public EForward UIForward
		{
			get 
			{ 
				if (Forward) return EForward.After;
				else return EForward.Before;
			}
			set 
			{ 
				if (value == EForward.After) Forward = true;
				else Forward = false;
			}
		}

		[PGNotVisible]
		public bool Forward
		{
			get { return mForward; }
			set { mForward = value; }
		}

		[Category("Transit Search")]
		[PropertyOrder(3),PGDisplayName("Date")]
		public Moment StartDate
		{
			get { return mDateBase; }
			set { mDateBase = value; }
		}
		[Category("Transit Search")]
		[PropertyOrder(4),PGDisplayName("When Body")]
		public Body.Name SearchBody
		{
			get { return mSearchGraha; }
			set { mSearchGraha = value; }
		}
		[Category("Transit Search")]
		[PropertyOrder(5),PGDisplayName("Transits")]
		public Longitude TransitPoint
		{
			get { return mTransitPoint; }
			set { mTransitPoint = value; }
		}
		[Category("Transit Search")]
		[PropertyOrder(6),PGDisplayName("Apply Locally")]
		public bool Apply
		{
			get { return mApplyNow; }
			set { mApplyNow = value; }
		}
		#region ICloneable Members

		public object Clone()
		{
			// TODO:  Add TransitSearchOptions.Clone implementation
			TransitSearchOptions ret = new TransitSearchOptions();
			ret.mDateBase = (Moment)this.mDateBase.Clone();
			ret.mForward = this.mForward;
			ret.mSearchGraha = this.mSearchGraha;
			ret.mTransitPoint = this.mTransitPoint;
			return ret;
		}
		public object CopyFrom (Object o)
		{
			TransitSearchOptions nopt = (TransitSearchOptions)o;
			this.mDateBase = (Moment)nopt.mDateBase.Clone();
			this.mForward = nopt.mForward;
			this.mSearchGraha = nopt.mSearchGraha;
			this.mTransitPoint = nopt.mTransitPoint;
			return this.Clone();
		}

		#endregion
	}

	
	public class TransitItem : System.Windows.Forms.ListViewItem
	{
		private Horoscope h;

		public Horoscope GetHoroscope ()
		{
			return h;
		}
		public TransitItem (Horoscope _h)
		{
			h = _h;
		}
	}

}

