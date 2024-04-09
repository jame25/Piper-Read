namespace piper_read;

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

        // Create a MenuStrip control
        MenuStrip menuStrip = new MenuStrip();
        this.Controls.Add(menuStrip);

        // Create a "File" menu item
        ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
        menuStrip.Items.Add(fileMenu);

        // Create an "Open" menu item
        ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open");
        openMenuItem.Click += new EventHandler(OpenMenuItem_Click);
        fileMenu.DropDownItems.Add(openMenuItem);

        // Create an "Exit" menu item
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += new EventHandler(ExitMenuItem_Click);
        fileMenu.DropDownItems.Add(exitMenuItem);

        // Create an "About" menu item
        ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About");
        aboutMenuItem.Click += new EventHandler(AboutMenuItem_Click);
        menuStrip.Items.Add(aboutMenuItem);

        this.txtInput = new System.Windows.Forms.TextBox();
        this.btnConvert = new System.Windows.Forms.Button();
        this.btnClearStop = new System.Windows.Forms.Button();
        this.lblModelName = new System.Windows.Forms.Label();
        this.SuspendLayout();

        // txtInput
        this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
        this.txtInput.Location = new System.Drawing.Point(0, 24);
        this.txtInput.Multiline = true;
        this.txtInput.Name = "txtInput";
        this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtInput.Size = new System.Drawing.Size(800, 397);
        this.txtInput.TabIndex = 0;

        // btnConvert
        this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.btnConvert.Location = new System.Drawing.Point(12, 415);
        this.btnConvert.Name = "btnConvert";
        this.btnConvert.Size = new System.Drawing.Size(75, 23);
        this.btnConvert.TabIndex = 1;
        this.btnConvert.Text = "Convert";
        this.btnConvert.UseVisualStyleBackColor = true;
        this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);

        // btnClearStop
        this.btnClearStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.btnClearStop.Location = new System.Drawing.Point(704, 415);
        this.btnClearStop.Name = "btnClearStop";
        this.btnClearStop.Size = new System.Drawing.Size(75, 23);
        this.btnClearStop.TabIndex = 2;
        this.btnClearStop.Text = "Clear";
        this.btnClearStop.UseVisualStyleBackColor = true;
        this.btnClearStop.Click += new System.EventHandler(this.btnClearStop_Click);

        // lblModelName
        this.lblModelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.lblModelName.AutoSize = true;
        this.lblModelName.Location = new System.Drawing.Point(93, 415);
        this.lblModelName.Name = "lblModelName";
        this.lblModelName.Size = new System.Drawing.Size(38, 15);
        this.lblModelName.TabIndex = 3;
        this.lblModelName.Text = "Model";

        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.lblModelName);
        this.Controls.Add(this.btnClearStop);
        this.Controls.Add(this.btnConvert);
        this.Controls.Add(this.txtInput);
        this.MainMenuStrip = menuStrip;
        this.Name = "Form1";
        this.Text = "Piper Read";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.Button btnConvert;
    private System.Windows.Forms.Button btnClearStop;
    private System.Windows.Forms.Label lblModelName;
}
