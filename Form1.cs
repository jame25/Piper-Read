using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace PiperTTS
{
    public partial class Form1 : Form
    {
        private string outputFile = "output.wav";
        private WaveOutEvent waveOut;
        private AudioFileReader audioFileReader;
        private string settingsFile = "settings.conf";
        private bool isConverting = false;
        private Label currentVoiceLabel;
        private Label voiceLabel;
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.Title = "Open Text File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    string fileContents = File.ReadAllText(filePath);
                    textBox1.Text = fileContents;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private const double MinSpeed = 0.1;
        private const double MaxSpeed = 1.0;
        private double selectedSpeed;


        public Form1()
        {
            try
            {
                InitializeComponent();
                this.Icon = new Icon("icon.ico");

                if (!CheckForOnnxFiles())
                {
                    Close();
                    return;
                }


                // Create a TableLayoutPanel to arrange the controls at the bottom
                TableLayoutPanel bottomPanel = new TableLayoutPanel();
                bottomPanel.ColumnCount = 5;
                bottomPanel.RowCount = 1;
                bottomPanel.Dock = DockStyle.Bottom;
                bottomPanel.AutoSize = true;
                bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                bottomPanel.Padding = new Padding(10);
                this.Controls.Add(bottomPanel);

                // Set column widths
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                // Add the convertButton to the bottomPanel
                bottomPanel.Controls.Add(convertButton, 0, 0);

                // Create and configure the voiceLabel
                voiceLabel = new Label();
                voiceLabel.AutoSize = true;
                voiceLabel.Text = "Voice:";
                voiceLabel.Anchor = AnchorStyles.Left | AnchorStyles.None;
                bottomPanel.Controls.Add(voiceLabel, 1, 0);

                // Create and configure the currentVoiceLabel
                currentVoiceLabel = new Label();
                currentVoiceLabel.AutoSize = true;
                currentVoiceLabel.TextAlign = ContentAlignment.MiddleLeft;
                currentVoiceLabel.BorderStyle = BorderStyle.FixedSingle;
                currentVoiceLabel.Anchor = AnchorStyles.Left | AnchorStyles.None;
                currentVoiceLabel.Click += CurrentVoiceLabel_Click;
                bottomPanel.Controls.Add(currentVoiceLabel, 2, 0);

                // Update speedTrackBar properties
                speedTrackBar.Minimum = 1;
                speedTrackBar.Maximum = 10;
                speedTrackBar.Value = 10; // Set the initial value to 10 (corresponding to 1.0 speed)

                // Create and configure the speedLabel
                Label speedLabel = new Label();
                speedLabel.AutoSize = true;
                speedLabel.Text = "    Speed:";
                speedLabel.TextAlign = ContentAlignment.MiddleRight;
                speedLabel.Anchor = AnchorStyles.Left | AnchorStyles.None;
                bottomPanel.Controls.Add(speedLabel, 3, 0);

                // Add the speedTrackBar to the bottomPanel
                speedTrackBar.Anchor = AnchorStyles.Left | AnchorStyles.None;
                speedTrackBar.AutoSize = false;
                speedTrackBar.Size = new Size(300, speedTrackBar.Height); // Set the size of the trackbar
                speedTrackBar.Margin = new Padding(0, 0, 0, 0); // Add top margin to lower the trackbar
                speedTrackBar.Padding = new Padding(0, 0, 50, 0); // Remove any padding to reduce vertical space
                bottomPanel.Controls.Add(speedTrackBar, 4, 0);

                /*// Create a TableLayoutPanel to hold the speedValue labels
                TableLayoutPanel speedValueLabelsPanel = new TableLayoutPanel();
                speedValueLabelsPanel.ColumnCount = 8;
                speedValueLabelsPanel.RowCount = 1;
                speedValueLabelsPanel.Dock = DockStyle.None;
                speedValueLabelsPanel.AutoSize = true;
                speedValueLabelsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                speedValueLabelsPanel.Margin = new Padding(0, -50, 0, 0); // Adjust the top margin to move the labels upwards
                speedValueLabelsPanel.Padding = new Padding(0, -50, 0, 0); // Remove any padding to reduce vertical space
                bottomPanel.Controls.Add(speedValueLabelsPanel, 4, 1); // Add the speedValueLabelsPanel to the second row of bottomPanel

                // Create and add speedValue labels to the speedValueLabelsPanel
                for (int i = 1; i <= 8; i++)
                {
                    Label speedValueLabel = new Label();
                    speedValueLabel.AutoSize = true;
                    speedValueLabel.Text = $"{i * 0.1:0.0}";
                    speedValueLabel.TextAlign = ContentAlignment.MiddleCenter;
                    speedValueLabel.Dock = DockStyle.None;
                    speedValueLabelsPanel.Controls.Add(speedValueLabel, i - 1, 0);

                    // Bring the speedValueLabelsPanel to the front
                    speedValueLabelsPanel.BringToFront();*/


                UpdateCurrentVoiceLabel();

                string speedValue = ReadSpeedFromConfig();
                if (!string.IsNullOrEmpty(speedValue))
                {
                    selectedSpeed = double.Parse(speedValue);
                    speedTrackBar.Value = (int)Math.Round((selectedSpeed - MinSpeed) / (MaxSpeed - MinSpeed) * (speedTrackBar.Maximum - speedTrackBar.Minimum)) + speedTrackBar.Minimum;
                }
                else
                {
                    selectedSpeed = MapSpeedValue(speedTrackBar.Value);
                }

                // Create the "File" menu item if it doesn't exist
                ToolStripMenuItem fileMenu = menuStrip1.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Text == "File");
                if (fileMenu == null)
                {
                    fileMenu = new ToolStripMenuItem("File");
                    menuStrip1.Items.Add(fileMenu);
                }

                // Create the "Open" menu item and add it as the first item in the "File" menu
                ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open");
                openMenuItem.Click += OpenMenuItem_Click;
                fileMenu.DropDownItems.Insert(0, openMenuItem);

                // Add other menu items to the "File" menu if needed
                // ...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during form initialization: {ex.Message}");
                // Log the exception details or write them to a file for further analysis
            }
        }

        private bool CheckForOnnxFiles()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] onnxFiles = Directory.GetFiles(exeDirectory, "*.onnx");

            if (onnxFiles.Length == 0)
            {
                MessageBox.Show("No .onnx files found in the application directory. Please make sure the model files are present.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }




        private string ReadSpeedFromConfig()
        {
            string configFile = "settings.conf";
            if (File.Exists(configFile))
            {
                string[] lines = File.ReadAllLines(configFile);
                foreach (string line in lines)
                {
                    if (line.StartsWith("speed="))
                    {
                        return line.Substring("speed=".Length);
                    }
                }
            }
            return null;
        }



        private void UpdateCurrentVoiceLabel()
        {
            (string speechModel, _) = ReadSpeechModelAndSpeedFromSettings();
            string voiceName = Path.GetFileNameWithoutExtension(speechModel);

            // Remove the "model=" prefix if present
            if (voiceName.StartsWith("model="))
            {
                voiceName = voiceName.Substring("model=".Length);
            }

            currentVoiceLabel.Text = voiceName;
        }



        private async void ConvertButton_Click(object sender, EventArgs e)
        {
            if (isConverting)
            {
                return;
            }

            string text = textBox1?.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Please enter some text to convert.");
                return;
            }

            if (convertButton.Text == "Convert")
            {
                isConverting = true;
                convertButton.Enabled = false;

                try
                {
                    // Delete the previous output file if it exists
                    DeleteOutputFile();

                    // Read the speech model and speed from the settings file
                    (string speechModel, string speed) = ReadSpeechModelAndSpeedFromSettings();

                    // Update the current voice label
                    UpdateCurrentVoiceLabel();

                    // Run Piper TTS to generate speech based on the input text
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "piper.exe";
                    startInfo.Arguments = $"--model {speechModel} --length_scale {speed} --output_file \"{outputFile}\"";
                    startInfo.RedirectStandardInput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;

                    using (Process process = new Process())
                    {
                        process.StartInfo = startInfo;
                        try
                        {
                            process.Start();

                            // Write the input text to the process's standard input
                            using (StreamWriter writer = process.StandardInput)
                            {
                                await writer.WriteAsync(text);
                            }

                            // Start playing the audio in a separate thread
                            Task.Run(() =>
                            {
                                while (process.HasExited == false || File.Exists(outputFile))
                                {
                                    if (File.Exists(outputFile))
                                    {
                                        // Create a new AudioFileReader instance
                                        audioFileReader = new AudioFileReader(outputFile);

                                        // Create a new WaveOutEvent instance
                                        waveOut = new WaveOutEvent();
                                        waveOut.Init(audioFileReader);

                                        // Subscribe to the PlaybackStopped event
                                        waveOut.PlaybackStopped += WaveOut_PlaybackStopped;

                                        // Change the Convert button text to "Pause"
                                        convertButton.Invoke((MethodInvoker)(() => convertButton.Text = "Pause"));

                                        // Change the Clear button text to "Stop"
                                        clearButton.Invoke((MethodInvoker)(() => clearButton.Text = "Stop"));

                                        try
                                        {
                                            // Play the generated speech
                                            waveOut.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show($"An error occurred during playback: {ex.Message}");
                                        }

                                        break;
                                    }

                                    Thread.Sleep(100);
                                }
                            });

                            await Task.Run(() => process.WaitForExit());

                            if (process.ExitCode == 0)
                            {
                                // Wait for the output file to be generated
                                int retryCount = 0;
                                while (!File.Exists(outputFile) && retryCount < 3)
                                {
                                    await Task.Delay(500);
                                    retryCount++;
                                }

                                // Check if the output file exists
                                if (File.Exists(outputFile))
                                {
                                    // Create a new AudioFileReader instance
                                    audioFileReader = new AudioFileReader(outputFile);

                                    // Create a new WaveOutEvent instance
                                    waveOut = new WaveOutEvent();
                                    waveOut.Init(audioFileReader);

                                    // Subscribe to the PlaybackStopped event
                                    waveOut.PlaybackStopped += WaveOut_PlaybackStopped;

                                    // Change the Convert button text to "Pause"
                                    convertButton.Text = "Pause";

                                    // Change the Clear button text to "Stop"
                                    clearButton.Text = "Stop";

                                    try
                                    {
                                        // Play the generated speech
                                        waveOut.Play();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"An error occurred during playback: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Output file not found.");
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Piper TTS process exited with code: {process.ExitCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
                finally
                {
                    isConverting = false;
                    convertButton.Enabled = true;
                }
            }
            else if (convertButton.Text == "Pause")
            {
                // Pause the playback if waveOut is not null
                if (waveOut != null)
                {
                    waveOut.Pause();
                    convertButton.Text = "Resume";
                }
            }
            else if (convertButton.Text == "Resume")
            {
                // Resume the playback if waveOut is not null
                if (waveOut != null)
                {
                    waveOut.Play();
                    convertButton.Text = "Pause";
                }
            }
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Dispose the waveOut and audioFileReader instances
            if (waveOut != null)
            {
                waveOut.Dispose();
                waveOut = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }

            // Change the Convert button text back to "Convert"
            convertButton.Text = "Convert";

            // Change the Clear button text back to "Clear"
            clearButton.Text = "Clear";

            // Delete the output file
            DeleteOutputFile();
        }


        private void DeleteOutputFile()
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }

        private (string speechModel, string speed) ReadSpeechModelAndSpeedFromSettings()
        {
            string settingsPath = "settings.conf";
            string speechModel = "en_us-libritts_r-medium.onnx"; // Default model
            string speed = "1.0"; // Default speed

            if (File.Exists(settingsPath))
            {
                foreach (string line in File.ReadAllLines(settingsPath))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (key == "model")
                        {
                            speechModel = value;
                        }
                        else if (key == "speed")
                        {
                            speed = value;
                        }
                    }
                }
            }

            return (speechModel, speed);
        }

        private void CurrentVoiceLabel_Click(object sender, EventArgs e)
        {
            string modelsDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string[] modelFiles = Directory.GetFiles(modelsDirectory, "*.onnx");

            List<string> modelNames = new List<string>();
            foreach (string modelFile in modelFiles)
            {
                string modelName = Path.GetFileNameWithoutExtension(modelFile);
                modelNames.Add(modelName);
            }

            string[] modelNamesArray = modelNames.ToArray();

            using (VoiceSelectionForm form = new VoiceSelectionForm(modelNamesArray))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedModel = form.SelectedModel;
                    SaveSelectedModelToSettings(selectedModel);
                    UpdateCurrentVoiceLabel();
                }
            }
        }




        private void SaveSelectedModelToSettings(string selectedModel)
        {
            string settingsPath = "settings.conf";
            string[] lines = File.ReadAllLines(settingsPath);

            // Append the ".onnx" extension if it's missing
            if (!selectedModel.EndsWith(".onnx", StringComparison.OrdinalIgnoreCase))
            {
                selectedModel += ".onnx";
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("model="))
                {
                    lines[i] = $"model={selectedModel}";
                    break;
                }
            }

            File.WriteAllLines(settingsPath, lines);
        }


        private double MapSpeedValue(int trackBarValue)
        {
            double speed = (MaxSpeed - MinSpeed) * (10 - trackBarValue) / 9 + MinSpeed;
            return Math.Round(speed, 1);
        }

        private void speedTrackBar_ValueChanged(object sender, EventArgs e)
        {
            selectedSpeed = MapSpeedValue(speedTrackBar.Value);
            // Update any labels or UI elements that display the selected speed

            // Save the updated speed value to the settings file
            SaveSpeedToSettings(selectedSpeed);
        }

        private void SaveSpeedToSettings(double speed)
        {
            string settingsPath = "settings.conf";
            string[] lines = File.ReadAllLines(settingsPath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("speed="))
                {
                    lines[i] = $"speed={speed}";
                    break;
                }
            }

            File.WriteAllLines(settingsPath, lines);
        }



        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (clearButton.Text == "Clear")
            {
                textBox1?.Clear();
            }
            else if (clearButton.Text == "Stop")
            {
                // Stop the playback and dispose the waveOut and audioFileReader if they are not null
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }

                if (audioFileReader != null)
                {
                    audioFileReader.Dispose();
                    audioFileReader = null;
                }

                // Change the Convert button text back to "Convert"
                convertButton.Text = "Convert";

                // Change the Clear button text back to "Clear"
                clearButton.Text = "Clear";

                // Delete the output file
                DeleteOutputFile();
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Add code to handle the Exit menu item click event
            Application.Exit();
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutMessage = "Version: 1.0.0\n" +
                                  "Developed by jame25\n\n" +
                                  "https://github.com/jame25/piper-read";

            // Create a new form for the about dialog
            Form aboutForm = new Form();
            aboutForm.Text = "About Piper Read";
            aboutForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            aboutForm.MaximizeBox = false;
            aboutForm.MinimizeBox = false;
            aboutForm.StartPosition = FormStartPosition.CenterParent;
            aboutForm.ClientSize = new Size(350, 200);

            try
            {
                // Load the icon from the file
                Icon icon = new Icon("icon.ico");
                aboutForm.Icon = icon;

                // Create a PictureBox to display the logo
                PictureBox logoPictureBox = new PictureBox();
                logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                logoPictureBox.Location = new Point(200, -23);
                logoPictureBox.Size = new Size(100, 100); // Adjust the size as needed
                aboutForm.Controls.Add(logoPictureBox);

                // Load the logo image from the icon file
                logoPictureBox.Image = icon.ToBitmap();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading icon: {ex.Message}");
                // Log the exception details
            }

            // Create a LinkLabel to display the about message and URL
            LinkLabel aboutLinkLabel = new LinkLabel();
            aboutLinkLabel.Text = aboutMessage;
            aboutLinkLabel.AutoSize = true;
            aboutLinkLabel.Location = new Point(10, 0);
            aboutForm.Controls.Add(aboutLinkLabel);

            // Set the LinkArea to make the URL clickable
            int startIndex = aboutMessage.IndexOf("https://");
            int length = "https://github.com/jame25/piper-read".Length;
            aboutLinkLabel.LinkArea = new LinkArea(startIndex, length);

            // Handle the LinkClicked event to open the URL in a web browser
            aboutLinkLabel.LinkClicked += (s, args) =>
            {
                string url = "https://github.com/jame25/piper-read";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            };

            // Create a button to close the about dialog
            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.DialogResult = DialogResult.OK;

            // Set the size of the close button
            closeButton.Size = new Size(100, 40); // Adjust the width and height as needed

            // Calculate the position of the close button
            int buttonX = (aboutForm.ClientSize.Width - closeButton.Width) / 2;
            int buttonY = aboutForm.ClientSize.Height - closeButton.Height - 20;
            closeButton.Location = new Point(buttonX, buttonY);

            aboutForm.Controls.Add(closeButton);

            // Display the about dialog
            aboutForm.ShowDialog(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!e.Cancel)
            {
                Application.Exit();
            }

            // Stop the playback and dispose the waveOut and audioFileReader if they are not null
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }

            // Delete the output file
            DeleteOutputFile();
        }
    }
}

