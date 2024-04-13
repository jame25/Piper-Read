using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace piper_read
{
    public partial class Form1 : Form
    {
        private List<WaveOutEvent> waveOutEvents = new List<WaveOutEvent>();
        private Dictionary<WaveOutEvent, WaveStream> waveStreams = new Dictionary<WaveOutEvent, WaveStream>();
        private TrackBar trackBarSpeed;
        private Label lblSpeed;
        private bool stopPlayback = false;
        private bool isPaused = false;
        private string settingsPath = "settings.conf";

        public Form1()
        {
            InitializeComponent();

            this.Icon = new System.Drawing.Icon("icon.ico");

            // Create the trackBarSpeed control programmatically
            trackBarSpeed = new TrackBar();
            trackBarSpeed.Size = new Size((this.ClientSize.Width - btnClearStop.Width - 40) / 4, 45); // Adjust the size to quarter the available width
            trackBarSpeed.Location = new Point(btnClearStop.Left - trackBarSpeed.Width - 10, btnClearStop.Top); // Position it closer to the "Clear" button
            trackBarSpeed.Minimum = 1;
            trackBarSpeed.Maximum = 10;
            trackBarSpeed.TickFrequency = 1;
            trackBarSpeed.SmallChange = 1;
            trackBarSpeed.Value = 10;
            trackBarSpeed.ValueChanged += TrackBarSpeed_ValueChanged;
            this.Controls.Add(trackBarSpeed);

            // Create the lblSpeed control programmatically
            lblSpeed = new Label();
            lblSpeed.Text = "Speed: " + (1.0 / trackBarSpeed.Value).ToString("0.0"); // Set the initial text with the calculated speed value
            lblSpeed.AutoSize = true;
            lblSpeed.MinimumSize = new Size(80, 0); // Set a minimum width for the label
            lblSpeed.TextAlign = ContentAlignment.MiddleRight; // Align the text to the right
            lblSpeed.Location = new Point(trackBarSpeed.Left - lblSpeed.Width - 5, trackBarSpeed.Top + (trackBarSpeed.Height - lblSpeed.Height) / 4);
            this.Controls.Add(lblSpeed);

            // Bring the lblSpeed control to the front
            lblSpeed.BringToFront();

            // Update the lblSpeed text when the trackBarSpeed value changes
            trackBarSpeed.ValueChanged += (sender, e) =>
            {
                double speed = (11 - trackBarSpeed.Value) * 0.1;
                lblSpeed.Text = "Speed: " + speed.ToString("0.0");
            };


            // Adjust the positions and sizes of the controls when the form is resized
            this.Resize += (sender, e) =>
            {
                trackBarSpeed.Size = new Size((this.ClientSize.Width - btnClearStop.Width - 40) / 4, 45);
                trackBarSpeed.Location = new Point(btnClearStop.Left - trackBarSpeed.Width - 10, btnClearStop.Top);
                lblSpeed.Location = new Point(trackBarSpeed.Left - lblSpeed.Width - 5, trackBarSpeed.Top + (trackBarSpeed.Height - lblSpeed.Height) / 4);
            };



            // Retrieve the selected model name and length scale from the settings file
            (string selectedModelName, double lengthScale) = GetSelectedModelNameAndLengthScale();

            // Set the model name in the lblModelName control
            if (!string.IsNullOrEmpty(selectedModelName))
            {
                lblModelName.Text = selectedModelName;
            }
            else
            {
                lblModelName.Text = "en_US-libritts_r-medium";
            }

            // Set the length scale value in the settings file if it doesn't exist
            if (!File.ReadAllText(settingsPath).Contains("LengthScale="))
            {
                File.AppendAllText(settingsPath, Environment.NewLine + $"LengthScale={lengthScale}");
            }

            // Set the border style of lblModelName to FixedSingle
            lblModelName.BorderStyle = BorderStyle.FixedSingle;

            // Set the padding of lblModelName to add space around the text
            lblModelName.Padding = new Padding(5);

            // Add a click event handler for lblModelName
            lblModelName.Click += LblModelName_Click;
        }

        private (string, double) GetSelectedModelNameAndLengthScale()
        {
            if (File.Exists(settingsPath))
            {
                string[] lines = File.ReadAllLines(settingsPath);
                string selectedModelName = null;
                double lengthScale = 1.0; // Default value

                foreach (string line in lines)
                {
                    if (line.StartsWith("SelectedModelName="))
                    {
                        selectedModelName = line.Split('=')[1];
                    }
                    else if (line.StartsWith("LengthScale="))
                    {
                        if (double.TryParse(line.Split('=')[1], out double scale))
                        {
                            lengthScale = scale;
                        }
                    }
                }

                // Set the trackbar value based on the saved length scale in the inverted order
                trackBarSpeed.Value = (int)((1.1 - lengthScale) * 10);

                return (selectedModelName, lengthScale);
            }

            return (null, 1.0); // Default value
        }

        private void SaveSelectedModelName(string modelName)
        {
            string content = $"SelectedModelName={modelName}";
            File.WriteAllText(settingsPath, content);
        }

        private void SaveSelectedSpeedValue(double speedValue)
        {
            string content = File.ReadAllText(settingsPath);
            string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string updatedContent = "";
            bool lengthScaleFound = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("LengthScale="))
                {
                    updatedContent += $"LengthScale={speedValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}{Environment.NewLine}";
                    lengthScaleFound = true;
                }
                else
                {
                    updatedContent += line + Environment.NewLine;
                }
            }

            if (!lengthScaleFound)
            {
                updatedContent += $"LengthScale={speedValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}{Environment.NewLine}";
            }

            File.WriteAllText(settingsPath, updatedContent.TrimEnd());
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileContents = File.ReadAllText(filePath);
                txtInput.Text = fileContents;
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            string aboutMessage = "\n\n" +
                                  "Version: 1.0.7\n" +
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


        private void LblModelName_Click(object sender, EventArgs e)
        {
            // Get the directory path of the piper_read.exe
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Get all .onnx files in the directory
            string[] onnxFiles = Directory.GetFiles(exeDirectory, "*.onnx");

            // Create a new form for the popup dialog
            using (Form popupForm = new Form())
            {
                popupForm.Text = "Select Voice";
                popupForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                popupForm.StartPosition = FormStartPosition.CenterParent;
                popupForm.MaximizeBox = false;
                popupForm.MinimizeBox = false;
                popupForm.ClientSize = new Size(300, 200);

                // Create a ListBox control to display the model names
                ListBox listBox = new ListBox();
                listBox.Location = new Point(10, 10);
                listBox.Size = new Size(280, 130);

                // Extract the model names from the file paths and add them to the ListBox
                foreach (string onnxFile in onnxFiles)
                {
                    string modelName = Path.GetFileNameWithoutExtension(onnxFile);
                    listBox.Items.Add(modelName);
                }

                // Create an "OK" button
                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(10, 150);
                okButton.Size = new Size(80, 30);
                okButton.Click += (s, args) =>
                {
                    if (listBox.SelectedItem != null)
                    {
                        // Update the lblModelName text with the selected model name without the .onnx extension
                        lblModelName.Text = listBox.SelectedItem.ToString();
                    }
                };

                // Create a "Cancel" button
                Button cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(210, 150);
                cancelButton.Size = new Size(80, 30);

                popupForm.Controls.Add(listBox);
                popupForm.Controls.Add(okButton);
                popupForm.Controls.Add(cancelButton);
                popupForm.AcceptButton = okButton;
                popupForm.CancelButton = cancelButton;

                // Show the popup dialog and check the result
                if (popupForm.ShowDialog(this) == DialogResult.OK)
                {
                    if (listBox.SelectedItem != null)
                    {
                        // Update the lblModelName text with the selected model name without the .onnx extension
                        string selectedModelName = listBox.SelectedItem.ToString();
                        lblModelName.Text = selectedModelName;

                        // Save the selected model name to the settings file
                        SaveSelectedModelName(selectedModelName);
                    }
                }
            }
        }

        private async void btnReplay_Click(object sender, EventArgs e)
        {
            // Read the contents of "ignore.dict" file
            string[] ignoreKeywords = File.ReadAllLines("ignore.dict");

            // Read the contents of "banned.dict" file
            string[] bannedKeywords = File.ReadAllLines("banned.dict");

            // Read the contents of "replace.dict" file
            string[] replaceKeywords = File.ReadAllLines("replace.dict");

            // Create the replaceDict dictionary
            Dictionary<string, string> replaceDict = new Dictionary<string, string>();
            foreach (string line in replaceKeywords)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    replaceDict[parts[0]] = parts[1];
                }
            }

            // Get the current cursor position in the txtInput control
            int cursorPosition = txtInput.SelectionStart;

            // Find the paragraph index based on the cursor position
            string inputText = txtInput.Text;
            string[] paragraphs = inputText.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int paragraphIndex = 0;
            int currentPosition = 0;

            for (int i = 0; i < paragraphs.Length; i++)
            {
                currentPosition += paragraphs[i].Length + 2; // Add 2 for the double newline separator
                if (currentPosition > cursorPosition)
                {
                    paragraphIndex = i;
                    break;
                }
            }

            // Stop any ongoing playback
            stopPlayback = true;
            await Task.Delay(100); // Wait for the playback to stop

            // Reset the playback state
            stopPlayback = false;
            isPaused = false;

            // Change the "Clear" button text to "Stop"
            btnClearStop.Text = "Stop";

            // Disable the "Replay" button during audio playback
            btnReplay.Enabled = false;

            Task<MemoryStream> previousProcessingTask = null;

            for (int i = paragraphIndex; i < paragraphs.Length; i++)
            {
                if (stopPlayback)
                    break;

                string paragraph = paragraphs[i];

                // Handle full stop before quotation marks
                paragraph = HandleFullStopBeforeQuotationMarks(paragraph);

                // Check if the paragraph contains any banned keywords
                if (bannedKeywords.Any(keyword => paragraph.Contains(keyword)))
                {
                    // Skip the entire line if a banned keyword is found
                    continue;
                }

                // Remove ignore keywords from the paragraph
                foreach (string keyword in ignoreKeywords)
                {
                    paragraph = paragraph.Replace(keyword, string.Empty);
                }

                // Replace words based on the "replace.dict" file
                foreach (var entry in replaceDict)
                {
                    paragraph = paragraph.Replace(entry.Key, entry.Value);
                }

                // Retrieve the length scale value from the settings file
                double lengthScale = (11 - trackBarSpeed.Value) * 0.1;

                // Create a task to process the current paragraph with piper.exe
                Task<MemoryStream> currentProcessingTask = Task.Run(async () =>
                {
                    using (Process piperProcess = new Process())
                    {
                        piperProcess.StartInfo.FileName = "piper.exe";
                        // Append the .onnx extension to the model name
                        string modelName = lblModelName.Text + ".onnx";
                        piperProcess.StartInfo.Arguments = $"--model {modelName} --length_scale {lengthScale} --output-raw";
                        piperProcess.StartInfo.UseShellExecute = false;
                        piperProcess.StartInfo.CreateNoWindow = true;
                        piperProcess.StartInfo.RedirectStandardInput = true;
                        piperProcess.StartInfo.RedirectStandardOutput = true;
                        piperProcess.Start();

                        // Write the paragraph to the standard input of the process
                        using (StreamWriter writer = piperProcess.StandardInput)
                        {
                            await writer.WriteAsync(paragraph);
                        }

                        // Create a buffer to store the audio data
                        byte[] buffer = new byte[16384];
                        MemoryStream audioStream = new MemoryStream();

                        // Read the audio data from piper.exe output in chunks
                        int bytesRead;
                        while ((bytesRead = await piperProcess.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            audioStream.Write(buffer, 0, bytesRead);
                        }

                        return audioStream;
                    }
                });
                if (previousProcessingTask != null)
                {
                    // Wait for the previous paragraph's processing to complete
                    MemoryStream audioStream = await previousProcessingTask;
                    audioStream.Position = 0;

                    using (WaveStream waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22000, 1)))
                    {
                        WaveOutEvent waveOutEvent = new WaveOutEvent();
                        waveOutEvent.Init(waveStream);
                        waveOutEvents.Add(waveOutEvent);
                        waveOutEvent.Play();

                        // Change the button text to "Pause"
                        btnConvert.Text = "Pause";
                        btnConvert.Enabled = true;

                        while (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                        {
                            if (stopPlayback)
                            {
                                waveOutEvent.Stop();
                                break;
                            }

                            if (isPaused && waveOutEvent.PlaybackState == PlaybackState.Playing)
                            {
                                waveOutEvent.Pause();
                            }
                            else if (!isPaused && waveOutEvent.PlaybackState == PlaybackState.Paused)
                            {
                                waveOutEvent.Play();
                            }

                            await Task.Delay(10);
                        }

                        // Remove the completed WaveOutEvent from the list
                        waveOutEvents.Remove(waveOutEvent);
                    }
                }

                previousProcessingTask = currentProcessingTask;
            }

            if (previousProcessingTask != null)
            {
                // Wait for the last paragraph's processing to complete
                MemoryStream audioStream = await previousProcessingTask;
                audioStream.Position = 0;

                using (WaveStream waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22000, 1)))
                {
                    WaveOutEvent waveOutEvent = new WaveOutEvent();
                    waveOutEvent.Init(waveStream);
                    waveOutEvents.Add(waveOutEvent);
                    waveOutEvent.Play();

                    // Change the button text to "Pause"
                    btnConvert.Text = "Pause";
                    btnConvert.Enabled = true;

                    while (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                    {
                        if (stopPlayback)
                        {
                            waveOutEvent.Stop();
                            break;
                        }

                        if (isPaused && waveOutEvent.PlaybackState == PlaybackState.Playing)
                        {
                            waveOutEvent.Pause();
                        }
                        else if (!isPaused && waveOutEvent.PlaybackState == PlaybackState.Paused)
                        {
                            waveOutEvent.Play();
                        }

                        await Task.Delay(10);
                    }

                    // Remove the completed WaveOutEvent from the list
                    waveOutEvents.Remove(waveOutEvent);
                }
            }

            // Change the button text back to "Convert" if playback has ended
            if (!isPaused && waveOutEvents.Count == 0)
            {
                btnConvert.Text = "Convert";
                // Change the "Stop" button text back to "Clear"
                btnClearStop.Text = "Clear";
                // Enable the "Replay" button after audio playback is finished
                btnReplay.Enabled = true;
            }
        }



        private void btnFastForward_Click(object sender, EventArgs e)
        {
            if (waveOutEvents.Count > 0)
            {
                foreach (var waveOutEvent in waveOutEvents)
                {
                    if (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                    {
                        // Get the current position in seconds
                        double currentPositionSeconds = waveOutEvent.GetPosition() / (double)waveOutEvent.OutputWaveFormat.AverageBytesPerSecond;

                        // Calculate the new position by subtracting 5 seconds
                        double newPositionSeconds = Math.Max(0, currentPositionSeconds - 5);

                        // Stop the current playback
                        waveOutEvent.Stop();

                        // Get the original WaveStream from the dictionary
                        if (waveStreams.TryGetValue(waveOutEvent, out var waveStream))
                        {
                            // Set the position of the original WaveStream to the new position
                            waveStream.Position = (long)(newPositionSeconds * waveOutEvent.OutputWaveFormat.AverageBytesPerSecond);

                            // Initialize a new WaveOutEvent with the original WaveStream
                            var newWaveOutEvent = new WaveOutEvent();
                            newWaveOutEvent.Init(waveStream);

                            // Replace the old WaveOutEvent with the new one
                            int index = waveOutEvents.IndexOf(waveOutEvent);
                            waveOutEvents[index] = newWaveOutEvent;

                            // Update the dictionary with the new WaveOutEvent
                            waveStreams.Remove(waveOutEvent);
                            waveStreams.Add(newWaveOutEvent, waveStream);

                            // Start playback from the new position
                            newWaveOutEvent.Play();
                        }
                    }
                }
            }
        }




        private void TrackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            // Calculate the speed value in the inverted order
            double speedValue = (11 - trackBarSpeed.Value) * 0.1;
            lblSpeed.Text = $"Speed: {speedValue:0.0}";

            // Save the selected speed value to the settings file
            SaveSelectedSpeedValue(speedValue);
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            // Read the contents of "ignore.dict" file
            string[] ignoreKeywords = File.ReadAllLines("ignore.dict");

            // Read the contents of "banned.dict" file
            string[] bannedKeywords = File.ReadAllLines("banned.dict");

            // Read the contents of "replace.dict" file
            string[] replaceKeywords = File.ReadAllLines("replace.dict");

            // Create the replaceDict dictionary
            Dictionary<string, string> replaceDict = new Dictionary<string, string>();
            foreach (string line in replaceKeywords)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    replaceDict[parts[0]] = parts[1];
                }
            }


            if (btnConvert.Text == "Convert")
            {
                string inputText = txtInput.Text;

                if (string.IsNullOrWhiteSpace(inputText))
                {
                    MessageBox.Show("Please enter some text to convert.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable the "Convert" and "Replay" buttons during audio playback
                btnConvert.Enabled = false;
                btnReplay.Enabled = false;

                string[] paragraphs = inputText.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                stopPlayback = false;
                isPaused = false;

                // Change the "Clear" button text to "Stop"
                btnClearStop.Text = "Stop";

                Task<MemoryStream> previousProcessingTask = null;

                for (int i = 0; i < paragraphs.Length; i++)
                {
                    if (stopPlayback)
                        break;

                    string paragraph = paragraphs[i];


                    // Handle full stop before quotation marks
                    paragraph = HandleFullStopBeforeQuotationMarks(paragraph);

                    // Check if the paragraph contains any banned keywords
                    if (bannedKeywords.Any(keyword => paragraph.Contains(keyword)))
                    {
                        // Skip the entire line if a banned keyword is found
                        continue;
                    }

                    // Remove ignore keywords from the paragraph
                    foreach (string keyword in ignoreKeywords)
                    {
                        paragraph = paragraph.Replace(keyword, string.Empty);
                    }

                    // Replace words based on the "replace.dict" file
                    foreach (var entry in replaceDict)
                    {
                        paragraph = paragraph.Replace(entry.Key, entry.Value);
                    }

                    // Retrieve the length scale value from the settings file
                    double lengthScale = (11 - trackBarSpeed.Value) * 0.1;

                    // Create a task to process the current paragraph with piper.exe
                    Task<MemoryStream> currentProcessingTask = Task.Run(async () =>
                    {
                        using (Process piperProcess = new Process())
                        {
                            piperProcess.StartInfo.FileName = "piper.exe";
                            // Append the .onnx extension to the model name
                            string modelName = lblModelName.Text + ".onnx";
                            piperProcess.StartInfo.Arguments = $"--model {modelName} --length_scale {lengthScale} --output-raw";
                            piperProcess.StartInfo.UseShellExecute = false;
                            piperProcess.StartInfo.CreateNoWindow = true;
                            piperProcess.StartInfo.RedirectStandardInput = true;
                            piperProcess.StartInfo.RedirectStandardOutput = true;
                            piperProcess.Start();

                            // Write the paragraph to the standard input of the process
                            using (StreamWriter writer = piperProcess.StandardInput)
                            {
                                await writer.WriteAsync(paragraph);
                            }

                            // Create a buffer to store the audio data
                            byte[] buffer = new byte[16384];
                            MemoryStream audioStream = new MemoryStream();

                            // Read the audio data from piper.exe output in chunks
                            int bytesRead;
                            while ((bytesRead = await piperProcess.StandardOutput.BaseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                audioStream.Write(buffer, 0, bytesRead);
                            }

                            return audioStream;
                        }
                    });

                    if (previousProcessingTask != null)
                    {
                        // Wait for the previous paragraph's processing to complete
                        MemoryStream audioStream = await previousProcessingTask;
                        audioStream.Position = 0;

                        using (WaveStream waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22000, 1)))
                        {
                            WaveOutEvent waveOutEvent = new WaveOutEvent();
                            waveOutEvent.Init(waveStream);
                            waveOutEvents.Add(waveOutEvent);
                            waveOutEvent.Play();

                            // Change the button text to "Pause"
                            btnConvert.Text = "Pause";
                            btnConvert.Enabled = true;

                            while (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                            {
                                if (stopPlayback)
                                {
                                    waveOutEvent.Stop();
                                    break;
                                }

                                if (isPaused && waveOutEvent.PlaybackState == PlaybackState.Playing)
                                {
                                    waveOutEvent.Pause();
                                }
                                else if (!isPaused && waveOutEvent.PlaybackState == PlaybackState.Paused)
                                {
                                    waveOutEvent.Play();
                                }

                                await Task.Delay(10);
                            }

                            // Remove the completed WaveOutEvent from the list
                            waveOutEvents.Remove(waveOutEvent);
                        }
                    }

                    previousProcessingTask = currentProcessingTask;
                }

                if (previousProcessingTask != null)
                {
                    // Wait for the last paragraph's processing to complete
                    MemoryStream audioStream = await previousProcessingTask;
                    audioStream.Position = 0;

                    using (WaveStream waveStream = new RawSourceWaveStream(audioStream, new WaveFormat(22000, 1)))
                    {
                        WaveOutEvent waveOutEvent = new WaveOutEvent();
                        waveOutEvent.Init(waveStream);
                        waveOutEvents.Add(waveOutEvent);
                        waveOutEvent.Play();

                        // Change the button text to "Pause"
                        btnConvert.Text = "Pause";
                        btnConvert.Enabled = true;

                        while (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                        {
                            if (stopPlayback)
                            {
                                waveOutEvent.Stop();
                                break;
                            }

                            if (isPaused && waveOutEvent.PlaybackState == PlaybackState.Playing)
                            {
                                waveOutEvent.Pause();
                            }
                            else if (!isPaused && waveOutEvent.PlaybackState == PlaybackState.Paused)
                            {
                                waveOutEvent.Play();
                            }

                            await Task.Delay(10);
                        }

                        // Remove the completed WaveOutEvent from the list
                        waveOutEvents.Remove(waveOutEvent);
                    }
                }

                // Change the button text back to "Convert" if playback has ended
                if (!isPaused && waveOutEvents.Count == 0)
                {
                    btnConvert.Text = "Convert";
                    // Change the "Stop" button text back to "Clear"
                    btnClearStop.Text = "Clear";
                }
            }
            else if (btnConvert.Text == "Pause")
            {
                // Pause the audio playback for all paragraphs
                foreach (var waveOutEvent in waveOutEvents)
                {
                    if (waveOutEvent.PlaybackState == PlaybackState.Playing)
                    {
                        waveOutEvent.Pause();
                    }
                }
                isPaused = true;
                btnConvert.Text = "Resume";
            }
            else if (btnConvert.Text == "Resume")
            {
                // Resume the audio playback for all paragraphs
                foreach (var waveOutEvent in waveOutEvents)
                {
                    if (waveOutEvent.PlaybackState == PlaybackState.Paused)
                    {
                        waveOutEvent.Play();
                    }
                }
                isPaused = false;
                btnConvert.Text = "Pause";
            }
            // Enable the "Replay" button only if playback has ended
            if (btnConvert.Text == "Convert" && waveOutEvents.Count == 0)
            {
                btnReplay.Enabled = true;
            }
        }

        private string HandleFullStopBeforeQuotationMarks(string paragraph)
        {
            string[] quotationMarks = { "\"", "'" };

            foreach (string mark in quotationMarks)
            {
                int index = paragraph.IndexOf(mark);
                while (index != -1)
                {
                    if (index > 0 && paragraph[index - 1] == '.')
                    {
                        paragraph = paragraph.Insert(index, " ");
                        index++;
                    }
                    index = paragraph.IndexOf(mark, index + 1);
                }
            }

            return paragraph;
        }


        private void btnClearStop_Click(object sender, EventArgs e)
        {
            if (btnClearStop.Text == "Clear")
            {
                // Clear the contents of the txtInput control
                txtInput.Clear();
            }
            else if (btnClearStop.Text == "Stop")
            {
                // Set the stopPlayback flag to true
                stopPlayback = true;

                // Stop the audio playback for all paragraphs
                foreach (var waveOutEvent in waveOutEvents)
                {
                    if (waveOutEvent.PlaybackState == PlaybackState.Playing || waveOutEvent.PlaybackState == PlaybackState.Paused)
                    {
                        waveOutEvent.Stop();
                    }
                }
                waveOutEvents.Clear();
                isPaused = false;
                btnConvert.Text = "Convert";
                // Change the "Stop" button text back to "Clear"
                btnClearStop.Text = "Clear";
            }
        }
    }
}
