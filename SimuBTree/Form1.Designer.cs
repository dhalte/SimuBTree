namespace SimuBTree
{
  partial class Form1
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.BTreeView = new System.Windows.Forms.TreeView();
      this.btAjouter = new System.Windows.Forms.Button();
      this.btMassAdd = new System.Windows.Forms.Button();
      this.btSupprimer = new System.Windows.Forms.Button();
      this.tbTrace = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tbSeed = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.tbNb = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.tbOrder = new System.Windows.Forms.TextBox();
      this.tbMax = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.btAddSuppr = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // BTreeView
      // 
      this.BTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.BTreeView.Location = new System.Drawing.Point(0, 0);
      this.BTreeView.Name = "BTreeView";
      this.BTreeView.Size = new System.Drawing.Size(645, 356);
      this.BTreeView.TabIndex = 0;
      // 
      // btAjouter
      // 
      this.btAjouter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btAjouter.Location = new System.Drawing.Point(677, 106);
      this.btAjouter.Name = "btAjouter";
      this.btAjouter.Size = new System.Drawing.Size(75, 23);
      this.btAjouter.TabIndex = 9;
      this.btAjouter.Text = "Ajouter";
      this.btAjouter.UseVisualStyleBackColor = true;
      this.btAjouter.Click += new System.EventHandler(this.btAjouter_Click);
      // 
      // btMassAdd
      // 
      this.btMassAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btMassAdd.Location = new System.Drawing.Point(677, 220);
      this.btMassAdd.Name = "btMassAdd";
      this.btMassAdd.Size = new System.Drawing.Size(75, 23);
      this.btMassAdd.TabIndex = 11;
      this.btMassAdd.Text = "Mass Add ";
      this.btMassAdd.UseVisualStyleBackColor = true;
      this.btMassAdd.Click += new System.EventHandler(this.btMassAdd_Click);
      // 
      // btSupprimer
      // 
      this.btSupprimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btSupprimer.Location = new System.Drawing.Point(677, 135);
      this.btSupprimer.Name = "btSupprimer";
      this.btSupprimer.Size = new System.Drawing.Size(75, 23);
      this.btSupprimer.TabIndex = 10;
      this.btSupprimer.Text = "Supprimer";
      this.btSupprimer.UseVisualStyleBackColor = true;
      this.btSupprimer.Click += new System.EventHandler(this.btSupprimer_Click);
      // 
      // tbTrace
      // 
      this.tbTrace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbTrace.Location = new System.Drawing.Point(0, 362);
      this.tbTrace.Multiline = true;
      this.tbTrace.Name = "tbTrace";
      this.tbTrace.ReadOnly = true;
      this.tbTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbTrace.Size = new System.Drawing.Size(645, 88);
      this.tbTrace.TabIndex = 12;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(677, 30);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(31, 15);
      this.label1.TabIndex = 3;
      this.label1.Text = "seed";
      // 
      // tbSeed
      // 
      this.tbSeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbSeed.Location = new System.Drawing.Point(714, 26);
      this.tbSeed.Name = "tbSeed";
      this.tbSeed.Size = new System.Drawing.Size(38, 23);
      this.tbSeed.TabIndex = 4;
      this.tbSeed.Text = "0";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(677, 55);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(23, 15);
      this.label2.TabIndex = 5;
      this.label2.Text = "Nb";
      // 
      // tbNb
      // 
      this.tbNb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbNb.Location = new System.Drawing.Point(714, 51);
      this.tbNb.Name = "tbNb";
      this.tbNb.Size = new System.Drawing.Size(38, 23);
      this.tbNb.TabIndex = 6;
      this.tbNb.Text = "300";
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(677, 5);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(35, 15);
      this.label3.TabIndex = 1;
      this.label3.Text = "order";
      // 
      // tbOrder
      // 
      this.tbOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbOrder.Location = new System.Drawing.Point(714, 1);
      this.tbOrder.Name = "tbOrder";
      this.tbOrder.Size = new System.Drawing.Size(38, 23);
      this.tbOrder.TabIndex = 2;
      this.tbOrder.Text = "5";
      // 
      // tbMax
      // 
      this.tbMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbMax.Location = new System.Drawing.Point(714, 77);
      this.tbMax.Name = "tbMax";
      this.tbMax.Size = new System.Drawing.Size(38, 23);
      this.tbMax.TabIndex = 8;
      this.tbMax.Text = "3000";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(677, 80);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(30, 15);
      this.label4.TabIndex = 7;
      this.label4.Text = "Max";
      // 
      // btAddSuppr
      // 
      this.btAddSuppr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btAddSuppr.Location = new System.Drawing.Point(677, 164);
      this.btAddSuppr.Name = "btAddSuppr";
      this.btAddSuppr.Size = new System.Drawing.Size(75, 23);
      this.btAddSuppr.TabIndex = 10;
      this.btAddSuppr.Text = "Add/Suppr";
      this.btAddSuppr.UseVisualStyleBackColor = true;
      this.btAddSuppr.Click += new System.EventHandler(this.btAddSuppr_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(773, 450);
      this.Controls.Add(this.tbMax);
      this.Controls.Add(this.tbNb);
      this.Controls.Add(this.tbOrder);
      this.Controls.Add(this.tbSeed);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.tbTrace);
      this.Controls.Add(this.btAddSuppr);
      this.Controls.Add(this.btSupprimer);
      this.Controls.Add(this.btMassAdd);
      this.Controls.Add(this.btAjouter);
      this.Controls.Add(this.BTreeView);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TreeView BTreeView;
    private System.Windows.Forms.Button btAjouter;
    private System.Windows.Forms.Button btMassAdd;
    private System.Windows.Forms.Button btSupprimer;
    private System.Windows.Forms.TextBox tbTrace;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox tbSeed;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tbNb;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox tbOrder;
    private System.Windows.Forms.TextBox tbMax;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btAddSuppr;
  }
}

