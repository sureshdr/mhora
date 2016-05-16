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
	public class GrahaStrengthsControl : Form
	{
		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.ComboBox cbStrength;
		private System.Windows.Forms.ComboBox cbGraha1;
		private System.Windows.Forms.ComboBox cbGraha2;
		private System.ComponentModel.IContainer components = null;
		private UserOptions options = null;
		private System.Windows.Forms.ContextMenu cMenu;
		private System.Windows.Forms.MenuItem menuOptions;
		private System.Windows.Forms.Label lVarga;
		private System.Windows.Forms.Label lColords;
		private Horoscope h = null;

		class UserOptions: ICloneable
		{
			Division m_dtype;

			[PGNotVisible]
			public Division Division
			{
				get { return m_dtype; }
				set { m_dtype = value; }
			}

			[PGDisplayName("Varga")]
			public Basics.DivisionType UIDivision
			{
				get { return m_dtype.MultipleDivisions[0].Varga; }
				set { this.m_dtype = new Division(value); }
			}
			public UserOptions()
			{
				m_dtype = new Division(Basics.DivisionType.Rasi);
			}
			public object Clone ()
			{
				UserOptions uo = new UserOptions();
				uo.Division = this.Division;
				return uo;
			}
			public object SetOptions (object _uo)
			{
				UserOptions uo = (UserOptions)_uo;
				this.Division = uo.Division;
				return this.Clone();
			}
		}

		public GrahaStrengthsControl(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			h.Changed += new EvtChanged(this.OnRecalculate);
			this.options = new UserOptions();
			InitializeComboBoxes();
		}

		public void OnRecalculate (object _h)
		{
			h = (Horoscope)_h;
			this.Compute();
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
			this.cbStrength = new System.Windows.Forms.ComboBox();
			this.cbGraha1 = new System.Windows.Forms.ComboBox();
			this.cbGraha2 = new System.Windows.Forms.ComboBox();
			this.mList = new System.Windows.Forms.ListView();
			this.cMenu = new System.Windows.Forms.ContextMenu();
			this.menuOptions = new System.Windows.Forms.MenuItem();
			this.lVarga = new System.Windows.Forms.Label();
			this.lColords = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbStrength
			// 
			this.cbStrength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbStrength.Location = new System.Drawing.Point(8, 8);
			this.cbStrength.Name = "cbStrength";
			this.cbStrength.Size = new System.Drawing.Size(120, 21);
			this.cbStrength.TabIndex = 0;
			this.cbStrength.Text = "cbStrength";
			this.cbStrength.SelectedIndexChanged += new System.EventHandler(this.cbStrength_SelectedIndexChanged);
			// 
			// cbGraha1
			// 
			this.cbGraha1.Location = new System.Drawing.Point(8, 40);
			this.cbGraha1.Name = "cbGraha1";
			this.cbGraha1.Size = new System.Drawing.Size(104, 21);
			this.cbGraha1.TabIndex = 1;
			this.cbGraha1.Text = "cbGraha1";
			this.cbGraha1.SelectedIndexChanged += new System.EventHandler(this.cbGraha1_SelectedIndexChanged);
			// 
			// cbGraha2
			// 
			this.cbGraha2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGraha2.Location = new System.Drawing.Point(152, 40);
			this.cbGraha2.Name = "cbGraha2";
			this.cbGraha2.Size = new System.Drawing.Size(112, 21);
			this.cbGraha2.TabIndex = 2;
			this.cbGraha2.Text = "cbGraha2";
			this.cbGraha2.SelectedIndexChanged += new System.EventHandler(this.cbGraha2_SelectedIndexChanged);
			// 
			// mList
			// 
			this.mList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(16, 72);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(240, 208);
			this.mList.TabIndex = 3;
			this.mList.View = System.Windows.Forms.View.Details;
			this.mList.SelectedIndexChanged += new System.EventHandler(this.mList_SelectedIndexChanged);
			// 
			// cMenu
			// 
			this.cMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				  this.menuOptions});
			// 
			// menuOptions
			// 
			this.menuOptions.Index = 0;
			this.menuOptions.Text = "Options";
			this.menuOptions.Click += new System.EventHandler(this.menuOptions_Click);
			// 
			// lVarga
			// 
			this.lVarga.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lVarga.Location = new System.Drawing.Point(144, 8);
			this.lVarga.Name = "lVarga";
			this.lVarga.Size = new System.Drawing.Size(104, 23);
			this.lVarga.TabIndex = 4;
			this.lVarga.Text = "lVarga";
			this.lVarga.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lColords
			// 
			this.lColords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lColords.Location = new System.Drawing.Point(16, 288);
			this.lColords.Name = "lColords";
			this.lColords.Size = new System.Drawing.Size(240, 16);
			this.lColords.TabIndex = 5;
			this.lColords.Text = "lColords";
			this.lColords.Click += new System.EventHandler(this.lColords_Click);
			// 
			// GrahaStrengthsControl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 310);
			this.ContextMenu = this.cMenu;
			this.Controls.Add(this.lColords);
			this.Controls.Add(this.lVarga);
			this.Controls.Add(this.mList);
			this.Controls.Add(this.cbGraha2);
			this.Controls.Add(this.cbGraha1);
			this.Controls.Add(this.cbStrength);
			this.Name = "GrahaStrengthsControl";
			this.Text = "Graha Strengths Reckoner";
			this.Resize += new System.EventHandler(this.GrahaStrengthsControl_Resize);
			this.Load += new System.EventHandler(this.GrahaStrengthsControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void mList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		const int RCoLord = 0;
		const int RNaisargikaDasa = 1;
		const int RVimsottariDasa = 2;
		const int RKarakaKendradiGrahaDasa = 3;
		const int RCoLordKarakaKendradiGrahaDasa = 4;

		private ArrayList GetRules (ref bool bSimpleLord)
		{
			bSimpleLord = false;
			switch (this.cbStrength.SelectedIndex)
			{
				case 0:
					bSimpleLord = true;
					return FindStronger.RulesStrongerCoLord(h);
				case 1:
					return FindStronger.RulesNaisargikaDasaGraha(h);
				case RVimsottariDasa:
					return FindStronger.RulesVimsottariGraha(h);
				case RKarakaKendradiGrahaDasa:
					return FindStronger.RulesKarakaKendradiGrahaDasaGraha(h);
				case RCoLordKarakaKendradiGrahaDasa:
					return FindStronger.RulesKarakaKendradiGrahaDasaColord(h);
				default:
					return FindStronger.RulesStrongerCoLord(h);
			}
		}
		private void InitializeComboBoxes()
		{
			for (int i = (int)Body.Name.Sun; i <= (int)Body.Name.Lagna; i++)
			{
				string s = mhora.Body.toString((mhora.Body.Name)i);
				this.cbGraha1.Items.Add(s);
				this.cbGraha2.Items.Add(s);
			}
			this.cbGraha1.SelectedIndex=(int)Body.Name.Mars;
			this.cbGraha2.SelectedIndex=(int)Body.Name.Ketu;

			this.cbStrength.Items.Add ("Co-Lord");
			this.cbStrength.Items.Add ("Naisargika Graha Dasa");
			this.cbStrength.Items.Add ("Vimsottari Dasa");
			this.cbStrength.Items.Add ("Karaka Kendradi Graha Dasa");
			this.cbStrength.Items.Add ("Karaka Kendradi Graha Dasa Co-Lord");
			this.cbStrength.SelectedIndex=0;

			this.lVarga.Text = options.Division.ToString();
			populateColordLabel();

		}

		private void Compute()
		{
			this.mList.BeginUpdate();
			this.mList.Clear();

			this.mList.BackColor = Color.AliceBlue;
			

			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Winner", -1, System.Windows.Forms.HorizontalAlignment.Left);

			int winner=0;
			Body.Name b1 = (Body.Name)this.cbGraha1.SelectedIndex;
			Body.Name b2 = (Body.Name)this.cbGraha2.SelectedIndex;
		
			bool bSimpleLord = false;
			ArrayList al = this.GetRules(ref bSimpleLord);
			for (int i=0; i<al.Count; i++)
			{
				ArrayList rule = new ArrayList();
				rule.Add (al[i]);
				FindStronger fs = new FindStronger(h, options.Division, rule);
				Body.Name bw = fs.StrongerGraha(b1, b2, bSimpleLord, ref winner);

				ListViewItem li = new ListViewItem();
				li.Text = string.Format("{0}", EnumDescConverter.GetEnumDescription((System.Enum)al[i]));
				
				if (winner == 0)
					li.SubItems.Add (Body.toString(bw));

				this.mList.Items.Add(li);
			}
			
			this.mList.Columns[0].Width = -1;
			this.mList.Columns[1].Width = -2;

			this.mList.EndUpdate();


		}


		private void GrahaStrengthsControl_Load(object sender, System.EventArgs e)
		{
			if (false == MhoraGlobalOptions.Instance.GrahaStrengthsFormSize.IsEmpty)
				this.Size = MhoraGlobalOptions.Instance.GrahaStrengthsFormSize;
		}

		private void cbStrength_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.cbStrength.SelectedIndex == RVimsottariDasa)
			{
				options.Division = new Division(Basics.DivisionType.BhavaPada);
				this.cbGraha1.SelectedIndex = (int)Body.Name.Lagna;
				this.cbGraha1.SelectedIndex = (int)Body.Name.Moon;
			}
			this.lVarga.Text = options.Division.ToString();
			this.Compute();
		}

		private void cbGraha1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.cbStrength.SelectedIndex == RCoLord)
			{
				switch (this.cbGraha1.SelectedIndex)
				{
					case (int)Body.Name.Mars:
						this.cbGraha2.SelectedIndex = (int)Body.Name.Ketu;
						break;
					case (int)Body.Name.Ketu:
						this.cbGraha2.SelectedIndex = (int)Body.Name.Mars;
						break;
					case (int)Body.Name.Saturn:
						this.cbGraha2.SelectedIndex = (int)Body.Name.Rahu;
						break;
					case (int)Body.Name.Rahu:
						this.cbGraha2.SelectedIndex = (int)Body.Name.Saturn;
						break;
				}
			}
			this.Compute();
		}

		private void cbGraha2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.Compute();
		}

		public object SetOptions (object o)
		{
			object o2 = options.SetOptions(o);
			this.lVarga.Text = options.Division.ToString();
			populateColordLabel();
			return o2;
		}
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			new MhoraOptions(this.options, new ApplyOptions(SetOptions)).ShowDialog();
		}

		private void populateColordLabel ()
		{
			Body.Name lAqu = h.LordOfZodiacHouse(new ZodiacHouse(ZodiacHouse.Name.Aqu), options.Division);
			Body.Name lSco = h.LordOfZodiacHouse(new ZodiacHouse(ZodiacHouse.Name.Sco), options.Division);
			this.lColords.Text = string.Format("{0} and {1} are the stronger co-lords", lSco, lAqu);
		}
		private void lColords_Click(object sender, System.EventArgs e)
		{
		
		}

		private void GrahaStrengthsControl_Resize(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.GrahaStrengthsFormSize = this.Size;
		}
	}
}

