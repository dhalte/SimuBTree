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
      this.btAJouter = new System.Windows.Forms.Button();
      this.btMassAdd = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // BTreeView
      // 
      this.BTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.BTreeView.Location = new System.Drawing.Point(0, 0);
      this.BTreeView.Name = "BTreeView";
      this.BTreeView.Size = new System.Drawing.Size(645, 451);
      this.BTreeView.TabIndex = 0;
      // 
      // btAJouter
      // 
      this.btAJouter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btAJouter.Location = new System.Drawing.Point(677, 12);
      this.btAJouter.Name = "btAJouter";
      this.btAJouter.Size = new System.Drawing.Size(75, 23);
      this.btAJouter.TabIndex = 1;
      this.btAJouter.Text = "Ajouter";
      this.btAJouter.UseVisualStyleBackColor = true;
      this.btAJouter.Click += new System.EventHandler(this.btAJouter_Click);
      // 
      // btMassAdd
      // 
      this.btMassAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btMassAdd.Location = new System.Drawing.Point(677, 55);
      this.btMassAdd.Name = "btMassAdd";
      this.btMassAdd.Size = new System.Drawing.Size(75, 23);
      this.btMassAdd.TabIndex = 2;
      this.btMassAdd.Text = "Mass Add ";
      this.btMassAdd.UseVisualStyleBackColor = true;
      this.btMassAdd.Click += new System.EventHandler(this.btMassAdd_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(773, 450);
      this.Controls.Add(this.btMassAdd);
      this.Controls.Add(this.btAJouter);
      this.Controls.Add(this.BTreeView);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView BTreeView;
    private System.Windows.Forms.Button btAJouter;
    private System.Windows.Forms.Button btMassAdd;
  }
}

