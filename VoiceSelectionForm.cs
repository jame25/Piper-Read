using System;
using System.Windows.Forms;

namespace PiperTTS
{
    public class VoiceSelectionForm : Form
    {
        private ListBox listBox1;
        private Button buttonOK;
        private Button buttonCancel;

        public string SelectedModel { get; private set; }

        public VoiceSelectionForm(string[] modelFiles)
        {
            InitializeComponent();
            listBox1.Items.AddRange(modelFiles);
        }

        private void InitializeComponent()
        {
            listBox1 = new ListBox();
            buttonOK = new Button();
            buttonCancel = new Button();

            listBox1.Location = new System.Drawing.Point(12, 12);
            listBox1.Size = new System.Drawing.Size(260, 200);
            listBox1.SelectedIndexChanged += new EventHandler(listBox1_SelectedIndexChanged);

            buttonOK.Location = new System.Drawing.Point(12, 220);
            buttonOK.Size = new System.Drawing.Size(75, 35);
            buttonOK.Text = "OK";
            buttonOK.Click += new EventHandler(buttonOK_Click);

            buttonCancel.Location = new System.Drawing.Point(197, 220);
            buttonCancel.Size = new System.Drawing.Size(75, 35);
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += new EventHandler(buttonCancel_Click);

            Controls.Add(listBox1);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);

            Text = "Select Voice";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new System.Drawing.Size(284, 261);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedModel = listBox1.SelectedItem?.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}