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
        Program.Log("Starting InitializeComponent");

        try
        {
            Program.Log("Creating initial controls");
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnClearStop = new System.Windows.Forms.Button();
            this.lblModelName = new System.Windows.Forms.Label();
            this.btnReplay = new System.Windows.Forms.Button();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.comboBoxSpeakers = new System.Windows.Forms.ComboBox();
            this.trackBarSpeed = new System.Windows.Forms.TrackBar();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.SuspendLayout();

            Program.Log("Creating TableLayoutPanel");
            TableLayoutPanel bottomPanel = new TableLayoutPanel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 40;
            bottomPanel.ColumnCount = 7;
            bottomPanel.RowCount = 1;

            Program.Log("Setting up column styles");
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            Program.Log("Adding controls to TableLayoutPanel");
            bottomPanel.Controls.Add(btnConvert, 0, 0);
            bottomPanel.Controls.Add(lblModelName, 1, 0);
            bottomPanel.Controls.Add(comboBoxSpeakers, 2, 0);
            bottomPanel.Controls.Add(btnReplay, 3, 0);
            bottomPanel.Controls.Add(btnFastForward, 4, 0);
            bottomPanel.Controls.Add(trackBarSpeed, 5, 0);
            bottomPanel.Controls.Add(btnClearStop, 6, 0);

            // Configure trackBarSpeed
            this.trackBarSpeed.Size = new System.Drawing.Size(200, 45);
            this.trackBarSpeed.Location = new System.Drawing.Point(450, 415);
            this.trackBarSpeed.Minimum = 1;
            this.trackBarSpeed.Maximum = 10;
            this.trackBarSpeed.TickFrequency = 1;
            this.trackBarSpeed.SmallChange = 1;
            this.trackBarSpeed.Value = 10;
            this.trackBarSpeed.ValueChanged += new System.EventHandler(this.trackBarSpeed_ValueChanged);

            // Update the lblSpeed text when the trackBarSpeed value changes
            trackBarSpeed.ValueChanged += (sender, e) =>
            {
                double speed = (11 - trackBarSpeed.Value) * 0.1;
                lblSpeed.Text = $"Speed: {speed:0.0}";
            };

            Program.Log("Setting up MenuStrip");
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            menuStrip.Items.Add(fileMenu);

            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open");
            openMenuItem.Click += new EventHandler(OpenMenuItem_Click);
            fileMenu.DropDownItems.Add(openMenuItem);

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += new EventHandler(ExitMenuItem_Click);
            fileMenu.DropDownItems.Add(exitMenuItem);

            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += new EventHandler(AboutMenuItem_Click);
            menuStrip.Items.Add(aboutMenuItem);

            Program.Log("Configuring control properties");
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(0, 24);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(800, 397);
            this.txtInput.TabIndex = 0;

            this.btnConvert.Location = new System.Drawing.Point(12, 415);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 1;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);

            this.btnClearStop.Location = new System.Drawing.Point(704, 415);
            this.btnClearStop.Name = "btnClearStop";
            this.btnClearStop.Size = new System.Drawing.Size(75, 23);
            this.btnClearStop.TabIndex = 2;
            this.btnClearStop.Text = "Clear";
            this.btnClearStop.UseVisualStyleBackColor = true;
            this.btnClearStop.Click += new System.EventHandler(this.btnClearStop_Click);

            this.lblModelName.AutoSize = true;
            this.lblModelName.Location = new System.Drawing.Point(93, 415);
            this.lblModelName.Name = "lblModelName";
            this.lblModelName.Size = new System.Drawing.Size(38, 15);
            this.lblModelName.TabIndex = 3;
            this.lblModelName.Text = "Model";

            this.btnReplay.Location = new System.Drawing.Point(250, 415);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(75, 23);
            this.btnReplay.TabIndex = 4;
            this.btnReplay.Text = "Cursor";
            this.btnReplay.UseVisualStyleBackColor = true;
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);

            this.btnFastForward.Location = new System.Drawing.Point(330, 415);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(100, 23);
            this.btnFastForward.TabIndex = 4;
            this.btnFastForward.Text = "Skip Forward";
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.btnFastForward_Click);

            Program.Log("Setting up form properties");
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);

            Program.Log("Adding controls to form");
            this.Controls.Add(menuStrip);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(this.txtInput);
            this.MainMenuStrip = menuStrip;
            this.Name = "Form1";
            this.Text = "Piper Read";

            this.ResumeLayout(false);
            this.PerformLayout();
            Program.Log("InitializeComponent completed successfully");
        }
        catch (Exception ex)
        {
            Program.Log($"Error in InitializeComponent: {ex.Message}");
            Program.Log($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    #endregion

    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.Button btnConvert;
    private System.Windows.Forms.Button btnClearStop;
    private System.Windows.Forms.Label lblModelName;
    private System.Windows.Forms.Button btnReplay;
    private System.Windows.Forms.Button btnFastForward;
}
