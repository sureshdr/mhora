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
	public class RasiStrengthsControl : Form
	{
		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.ComboBox cbStrength;
		private System.ComponentModel.IContainer components = null;
		private UserOptions options = null;
		private System.Windows.Forms.ComboBox cbRasi1;
		private System.Windows.Forms.ComboBox cbRasi2;
		private System.Windows.Forms.ContextMenu cMenu;
		private System.Windows.Forms.MenuItem menuOptions;
		private System.Windows.Forms.Label lVarga;
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

		public RasiStrengthsControl(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.

			InitializeComponent();

			if (false == MhoraGlobalOptions.Instance.RasiStrengthsFormSize.IsEmpty)
				this.Size = MhoraGlobalOptions.Instance.RasiStrengthsFormSize;
			
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
			this.cbRasi1 = new System.Windows.Forms.ComboBox();
			this.cbRasi2 = new System.Windows.Forms.ComboBox();
			this.mList = new System.Windows.Forms.ListView();
			this.cMenu = new System.Windows.Forms.ContextMenu();
			this.menuOptions = new System.Windows.Forms.MenuItem();
			this.lVarga = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cbStrength
			// 
			this.cbStrength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbStrength.Location = new System.Drawing.Point(8, 8);
			this.cbStrength.Name = "cbStrength";
			this.cbStrength.Size = new System.Drawing.Size(128, 21);
			this.cbStrength.TabIndex = 0;
			this.cbStrength.Text = "cbStrength";
			this.cbStrength.SelectedIndexChanged += new System.EventHandler(this.cbStrength_SelectedIndexChanged);
			// 
			// cbRasi1
			// 
			this.cbRasi1.Location = new System.Drawing.Point(8, 40);
			this.cbRasi1.Name = "cbRasi1";
			this.cbRasi1.Size = new System.Drawing.Size(104, 21);
			this.cbRasi1.TabIndex = 1;
			this.cbRasi1.Text = "cbRasi1";
			this.cbRasi1.SelectedIndexChanged += new System.EventHandler(this.cbGraha1_SelectedIndexChanged);
			// 
			// cbRasi2
			// 
			this.cbRasi2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRasi2.Location = new System.Drawing.Point(144, 40);
			this.cbRasi2.Name = "cbRasi2";
			this.cbRasi2.Size = new System.Drawing.Size(112, 21);
			this.cbRasi2.TabIndex = 2;
			this.cbRasi2.Text = "cbRasi2";
			this.cbRasi2.SelectedIndexChanged += new System.EventHandler(this.cbGraha2_SelectedIndexChanged);
			// 
			// mList
			// 
			this.mList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(16, 72);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(240, 176);
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
			this.lVarga.Text = "label1";
			this.lVarga.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// RasiStrengthsControl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 262);
			this.ContextMenu = this.cMenu;
			this.Controls.Add(this.lVarga);
			this.Controls.Add(this.mList);
			this.Controls.Add(this.cbRasi2);
			this.Controls.Add(this.cbRasi1);
			this.Controls.Add(this.cbStrength);
			this.Name = "RasiStrengthsControl";
			this.Text = "Rasi Strengths Reckoner";
			this.Resize += new System.EventHandler(this.RasiStrengthsControl_Resize);
			this.Load += new System.EventHandler(this.GrahaStrengthsControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void mList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private ArrayList GetRules ()
		{
			switch (this.cbStrength.SelectedIndex)
			{
				case 0:
					return FindStronger.RulesNarayanaDasaRasi(h);
				case 1:
					return FindStronger.RulesNaisargikaDasaRasi(h);
				case 2:
					return FindStronger.RulesMoolaDasaRasi(h);
				case 3:
					return FindStronger.RulesKarakaKendradiGrahaDasaRasi(h);
				case 4:
					return FindStronger.RulesNavamsaDasaRasi(h);
				case 5:
					return FindStronger.RulesJaiminiFirstRasi(h);
				case 6:
					return FindStronger.RulesJaiminiSecondRasi(h);
				default:
					return FindStronger.RulesNarayanaDasaRasi(h);
			}
		}
		private void InitializeComboBoxes()
		{
			for (int i = (int)ZodiacHouse.Name.Ari; i <= (int)ZodiacHouse.Name.Pis; i++)
			{
				string s = String.Format("{0}", ((ZodiacHouse.Name)i).ToString());
				this.cbRasi1.Items.Add(s);
				this.cbRasi2.Items.Add(s);
			}
			this.cbRasi1.SelectedIndex=0;
			this.cbRasi2.SelectedIndex=6;

			this.cbStrength.Items.Add ("Narayana Dasa");
			this.cbStrength.Items.Add ("Naisargika Dasa");
			this.cbStrength.Items.Add ("Moola Dasa");
			this.cbStrength.Items.Add ("Karaka Kendradi Graha Dasa");
			this.cbStrength.Items.Add ("Navamsa Dasa");
			this.cbStrength.Items.Add ("Jaimini's 1st Source of Strength");
			this.cbStrength.Items.Add ("Jaimini's 2nd Source of Strength");
			this.cbStrength.SelectedIndex=0;

			this.lVarga.Text = options.Division.ToString();

		}

		private void Compute()
		{
			this.mList.BeginUpdate();
			this.mList.Clear();

			this.mList.BackColor = Color.AliceBlue;
			

			this.mList.Columns.Add ("Body", -1, System.Windows.Forms.HorizontalAlignment.Left);
			this.mList.Columns.Add ("Winner", -1, System.Windows.Forms.HorizontalAlignment.Left);

			int winner=0;
			if (this.cbRasi1.SelectedIndex < 0) this.cbRasi1.SelectedIndex = 0;
			if (this.cbRasi2.SelectedIndex < 0) this.cbRasi2.SelectedIndex = 0;

			ZodiacHouse.Name z1 = (ZodiacHouse.Name)this.cbRasi1.SelectedIndex+1;
			ZodiacHouse.Name z2 = (ZodiacHouse.Name)this.cbRasi2.SelectedIndex+1;

			ArrayList al = this.GetRules();
			for (int i=0; i<al.Count; i++)
			{
				ArrayList rule = new ArrayList();
				rule.Add (al[i]);
				FindStronger fs = new FindStronger(h, options.Division, rule);
				ZodiacHouse.Name zw = fs.StrongerRasi(z1, z2, false, ref winner);
				ListViewItem li = new ListViewItem();
				li.Text = string.Format("{0}", EnumDescConverter.GetEnumDescription((System.Enum)al[i]));
				
				if (winner == 0) 
					li.SubItems.Add (string.Format("{0}", zw));

				this.mList.Items.Add(li);
			}
			
			this.mList.Columns[0].Width = -1;
			this.mList.Columns[1].Width = -2;

			this.mList.EndUpdate();


		}


		private void GrahaStrengthsControl_Load(object sender, System.EventArgs e)
		{


		}

		private void cbStrength_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.Compute();
		}

		private void cbGraha1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
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
			return o2;
		}
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			new MhoraOptions(this.options, new ApplyOptions(this.SetOptions)).ShowDialog();
		}

		private void RasiStrengthsControl_Resize(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.RasiStrengthsFormSize = this.Size;
		}
	}
}

