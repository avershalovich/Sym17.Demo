namespace ImageProcess
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cameraPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_screenshot = new System.Windows.Forms.Button();
            this.checkBox_async = new System.Windows.Forms.CheckBox();
            this.button_test_async = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.reduce_list = new System.Windows.Forms.ComboBox();
            this.checkBox_fullscreen = new System.Windows.Forms.CheckBox();
            this.checkBox_truncate_roi = new System.Windows.Forms.CheckBox();
            this.resolutionsList = new System.Windows.Forms.ComboBox();
            this.camerasList = new System.Windows.Forms.ComboBox();
            this.comboBoxDetectorMode = new System.Windows.Forms.ComboBox();
            this.buttonwrite_badge_template = new System.Windows.Forms.Button();
            this.button_read_badge = new System.Windows.Forms.Button();
            this.button_test_badge_stop = new System.Windows.Forms.Button();
            this.button_test_badge = new System.Windows.Forms.Button();
            this.button_test_face_stop = new System.Windows.Forms.Button();
            this.button_test_face = new System.Windows.Forms.Button();
            this.button_set_template = new System.Windows.Forms.Button();
            this.labelBadges = new System.Windows.Forms.Label();
            this.button_stop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button_start = new System.Windows.Forms.Button();
            this.labelFaces = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.recognizedLog = new System.Windows.Forms.RichTextBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.badgePictureBox = new System.Windows.Forms.PictureBox();
            this.facePictureBox = new System.Windows.Forms.PictureBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openBadgeDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.cameraPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.badgePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.facePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraPictureBox
            // 
            this.cameraPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraPictureBox.Location = new System.Drawing.Point(0, 0);
            this.cameraPictureBox.Name = "cameraPictureBox";
            this.cameraPictureBox.Size = new System.Drawing.Size(1378, 814);
            this.cameraPictureBox.TabIndex = 11;
            this.cameraPictureBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_screenshot);
            this.panel1.Controls.Add(this.checkBox_async);
            this.panel1.Controls.Add(this.button_test_async);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.reduce_list);
            this.panel1.Controls.Add(this.checkBox_fullscreen);
            this.panel1.Controls.Add(this.checkBox_truncate_roi);
            this.panel1.Controls.Add(this.resolutionsList);
            this.panel1.Controls.Add(this.camerasList);
            this.panel1.Controls.Add(this.comboBoxDetectorMode);
            this.panel1.Controls.Add(this.buttonwrite_badge_template);
            this.panel1.Controls.Add(this.button_read_badge);
            this.panel1.Controls.Add(this.button_test_badge_stop);
            this.panel1.Controls.Add(this.button_test_badge);
            this.panel1.Controls.Add(this.button_test_face_stop);
            this.panel1.Controls.Add(this.button_test_face);
            this.panel1.Controls.Add(this.button_set_template);
            this.panel1.Controls.Add(this.labelBadges);
            this.panel1.Controls.Add(this.button_stop);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.button_start);
            this.panel1.Controls.Add(this.labelFaces);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.recognizedLog);
            this.panel1.Controls.Add(this.labelTime);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.badgePictureBox);
            this.panel1.Controls.Add(this.facePictureBox);
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Location = new System.Drawing.Point(782, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(596, 771);
            this.panel1.TabIndex = 19;
            // 
            // button_screenshot
            // 
            this.button_screenshot.Location = new System.Drawing.Point(407, 654);
            this.button_screenshot.Name = "button_screenshot";
            this.button_screenshot.Size = new System.Drawing.Size(177, 23);
            this.button_screenshot.TabIndex = 50;
            this.button_screenshot.Text = "sceenshot to clipboard";
            this.button_screenshot.UseVisualStyleBackColor = true;
            this.button_screenshot.Click += new System.EventHandler(this.button_screenshot_Click);
            // 
            // checkBox_async
            // 
            this.checkBox_async.AutoSize = true;
            this.checkBox_async.Checked = true;
            this.checkBox_async.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_async.Location = new System.Drawing.Point(27, 123);
            this.checkBox_async.Name = "checkBox_async";
            this.checkBox_async.Size = new System.Drawing.Size(54, 17);
            this.checkBox_async.TabIndex = 49;
            this.checkBox_async.Text = "async";
            this.checkBox_async.UseVisualStyleBackColor = true;
            this.checkBox_async.CheckedChanged += new System.EventHandler(this.checkBox_async_CheckedChanged);
            // 
            // button_test_async
            // 
            this.button_test_async.Location = new System.Drawing.Point(135, 681);
            this.button_test_async.Name = "button_test_async";
            this.button_test_async.Size = new System.Drawing.Size(115, 23);
            this.button_test_async.TabIndex = 48;
            this.button_test_async.Text = "start test async";
            this.button_test_async.UseVisualStyleBackColor = true;
            this.button_test_async.Click += new System.EventHandler(this.button_test_async_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(534, 630);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 47;
            this.label1.Text = "reduce";
            // 
            // reduce_list
            // 
            this.reduce_list.FormattingEnabled = true;
            this.reduce_list.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.reduce_list.Location = new System.Drawing.Point(407, 627);
            this.reduce_list.Name = "reduce_list";
            this.reduce_list.Size = new System.Drawing.Size(121, 21);
            this.reduce_list.TabIndex = 46;
            this.reduce_list.SelectedIndexChanged += new System.EventHandler(this.reduce_list_SelectedIndexChanged);
            // 
            // checkBox_fullscreen
            // 
            this.checkBox_fullscreen.AutoSize = true;
            this.checkBox_fullscreen.Checked = true;
            this.checkBox_fullscreen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_fullscreen.Location = new System.Drawing.Point(463, 71);
            this.checkBox_fullscreen.Name = "checkBox_fullscreen";
            this.checkBox_fullscreen.Size = new System.Drawing.Size(71, 17);
            this.checkBox_fullscreen.TabIndex = 43;
            this.checkBox_fullscreen.Text = "fullscreen";
            this.checkBox_fullscreen.UseVisualStyleBackColor = true;
            this.checkBox_fullscreen.CheckedChanged += new System.EventHandler(this.checkBox_fullscreen_CheckedChanged);
            // 
            // checkBox_truncate_roi
            // 
            this.checkBox_truncate_roi.AutoSize = true;
            this.checkBox_truncate_roi.Checked = true;
            this.checkBox_truncate_roi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_truncate_roi.Location = new System.Drawing.Point(463, 54);
            this.checkBox_truncate_roi.Name = "checkBox_truncate_roi";
            this.checkBox_truncate_roi.Size = new System.Drawing.Size(87, 17);
            this.checkBox_truncate_roi.TabIndex = 42;
            this.checkBox_truncate_roi.Text = "truncate ROI";
            this.checkBox_truncate_roi.UseVisualStyleBackColor = true;
            this.checkBox_truncate_roi.CheckedChanged += new System.EventHandler(this.checkBox_truncate_roi_CheckedChanged);
            // 
            // resolutionsList
            // 
            this.resolutionsList.FormattingEnabled = true;
            this.resolutionsList.Items.AddRange(new object[] {
            "Face first",
            "Badge first",
            "Both"});
            this.resolutionsList.Location = new System.Drawing.Point(463, 27);
            this.resolutionsList.Name = "resolutionsList";
            this.resolutionsList.Size = new System.Drawing.Size(121, 21);
            this.resolutionsList.TabIndex = 41;
            this.resolutionsList.SelectedIndexChanged += new System.EventHandler(this.resolutionsList_SelectedIndexChanged);
            // 
            // camerasList
            // 
            this.camerasList.FormattingEnabled = true;
            this.camerasList.Location = new System.Drawing.Point(369, 0);
            this.camerasList.Name = "camerasList";
            this.camerasList.Size = new System.Drawing.Size(215, 21);
            this.camerasList.TabIndex = 40;
            this.camerasList.SelectedIndexChanged += new System.EventHandler(this.camerasList_SelectedIndexChanged);
            // 
            // comboBoxDetectorMode
            // 
            this.comboBoxDetectorMode.FormattingEnabled = true;
            this.comboBoxDetectorMode.Items.AddRange(new object[] {
            "Face first",
            "Badge first",
            "Both"});
            this.comboBoxDetectorMode.Location = new System.Drawing.Point(27, 96);
            this.comboBoxDetectorMode.Name = "comboBoxDetectorMode";
            this.comboBoxDetectorMode.Size = new System.Drawing.Size(115, 21);
            this.comboBoxDetectorMode.TabIndex = 39;
            this.comboBoxDetectorMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxDetectorMode_SelectedIndexChanged);
            // 
            // buttonwrite_badge_template
            // 
            this.buttonwrite_badge_template.Location = new System.Drawing.Point(256, 654);
            this.buttonwrite_badge_template.Name = "buttonwrite_badge_template";
            this.buttonwrite_badge_template.Size = new System.Drawing.Size(145, 23);
            this.buttonwrite_badge_template.TabIndex = 38;
            this.buttonwrite_badge_template.Text = "write badge template";
            this.buttonwrite_badge_template.UseVisualStyleBackColor = true;
            this.buttonwrite_badge_template.Click += new System.EventHandler(this.buttonwrite_badge_template_Click);
            // 
            // button_read_badge
            // 
            this.button_read_badge.Location = new System.Drawing.Point(256, 625);
            this.button_read_badge.Name = "button_read_badge";
            this.button_read_badge.Size = new System.Drawing.Size(145, 23);
            this.button_read_badge.TabIndex = 37;
            this.button_read_badge.Text = "show badge template";
            this.button_read_badge.UseVisualStyleBackColor = true;
            this.button_read_badge.Click += new System.EventHandler(this.button_read_badge_Click);
            // 
            // button_test_badge_stop
            // 
            this.button_test_badge_stop.Location = new System.Drawing.Point(135, 652);
            this.button_test_badge_stop.Name = "button_test_badge_stop";
            this.button_test_badge_stop.Size = new System.Drawing.Size(115, 23);
            this.button_test_badge_stop.TabIndex = 36;
            this.button_test_badge_stop.Text = "stop test badge";
            this.button_test_badge_stop.UseVisualStyleBackColor = true;
            this.button_test_badge_stop.Click += new System.EventHandler(this.button_test_badge_stop_Click);
            // 
            // button_test_badge
            // 
            this.button_test_badge.Location = new System.Drawing.Point(135, 623);
            this.button_test_badge.Name = "button_test_badge";
            this.button_test_badge.Size = new System.Drawing.Size(115, 23);
            this.button_test_badge.TabIndex = 35;
            this.button_test_badge.Text = "start test badge";
            this.button_test_badge.UseVisualStyleBackColor = true;
            this.button_test_badge.Click += new System.EventHandler(this.button_test_badge_Click);
            // 
            // button_test_face_stop
            // 
            this.button_test_face_stop.Location = new System.Drawing.Point(14, 652);
            this.button_test_face_stop.Name = "button_test_face_stop";
            this.button_test_face_stop.Size = new System.Drawing.Size(115, 23);
            this.button_test_face_stop.TabIndex = 34;
            this.button_test_face_stop.Text = "stop test face";
            this.button_test_face_stop.UseVisualStyleBackColor = true;
            this.button_test_face_stop.Click += new System.EventHandler(this.button_test_face_stop_Click);
            // 
            // button_test_face
            // 
            this.button_test_face.Location = new System.Drawing.Point(14, 623);
            this.button_test_face.Name = "button_test_face";
            this.button_test_face.Size = new System.Drawing.Size(115, 23);
            this.button_test_face.TabIndex = 33;
            this.button_test_face.Text = "start test face";
            this.button_test_face.UseVisualStyleBackColor = true;
            this.button_test_face.Click += new System.EventHandler(this.button_test_face_Click);
            // 
            // button_set_template
            // 
            this.button_set_template.Location = new System.Drawing.Point(27, 71);
            this.button_set_template.Name = "button_set_template";
            this.button_set_template.Size = new System.Drawing.Size(115, 23);
            this.button_set_template.TabIndex = 31;
            this.button_set_template.Text = "set badge template";
            this.button_set_template.UseVisualStyleBackColor = true;
            this.button_set_template.Click += new System.EventHandler(this.button_set_template_Click);
            // 
            // labelBadges
            // 
            this.labelBadges.AutoSize = true;
            this.labelBadges.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBadges.Location = new System.Drawing.Point(304, 78);
            this.labelBadges.Name = "labelBadges";
            this.labelBadges.Size = new System.Drawing.Size(16, 16);
            this.labelBadges.TabIndex = 29;
            this.labelBadges.Text = "0";
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(27, 44);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(115, 23);
            this.button_stop.TabIndex = 30;
            this.button_stop.Text = "stop capture";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(172, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 28;
            this.label3.Text = "Detecting Badges:";
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(27, 18);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(115, 23);
            this.button_start.TabIndex = 30;
            this.button_start.Text = "start capture";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelFaces
            // 
            this.labelFaces.AutoSize = true;
            this.labelFaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFaces.Location = new System.Drawing.Point(304, 51);
            this.labelFaces.Name = "labelFaces";
            this.labelFaces.Size = new System.Drawing.Size(16, 16);
            this.labelFaces.TabIndex = 27;
            this.labelFaces.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(172, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 16);
            this.label2.TabIndex = 26;
            this.label2.Text = "Detecting Faces:";
            // 
            // recognizedLog
            // 
            this.recognizedLog.Location = new System.Drawing.Point(14, 451);
            this.recognizedLog.Name = "recognizedLog";
            this.recognizedLog.Size = new System.Drawing.Size(570, 155);
            this.recognizedLog.TabIndex = 25;
            this.recognizedLog.Text = "";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTime.Location = new System.Drawing.Point(304, 25);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(16, 16);
            this.labelTime.TabIndex = 23;
            this.labelTime.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(172, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 16);
            this.label4.TabIndex = 22;
            this.label4.Text = "Detecting Time:";
            // 
            // badgePictureBox
            // 
            this.badgePictureBox.Location = new System.Drawing.Point(396, 95);
            this.badgePictureBox.Name = "badgePictureBox";
            this.badgePictureBox.Size = new System.Drawing.Size(188, 278);
            this.badgePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.badgePictureBox.TabIndex = 21;
            this.badgePictureBox.TabStop = false;
            // 
            // facePictureBox
            // 
            this.facePictureBox.Location = new System.Drawing.Point(130, 123);
            this.facePictureBox.Name = "facePictureBox";
            this.facePictureBox.Size = new System.Drawing.Size(250, 250);
            this.facePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.facePictureBox.TabIndex = 20;
            this.facePictureBox.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(14, 390);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(570, 55);
            this.richTextBox1.TabIndex = 19;
            this.richTextBox1.Text = "";
            // 
            // openBadgeDialog
            // 
            this.openBadgeDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif;" +
    " *.png";
            this.openBadgeDialog.Title = "Open Badge Template";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 814);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cameraPictureBox);
            this.Name = "Form1";
            this.Text = "Sym";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cameraPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.badgePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.facePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox cameraPictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelBadges;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelFaces;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox recognizedLog;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox badgePictureBox;
        private System.Windows.Forms.PictureBox facePictureBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_set_template;
        private System.Windows.Forms.OpenFileDialog openBadgeDialog;
        private System.Windows.Forms.Button button_test_face_stop;
        private System.Windows.Forms.Button button_test_face;
        private System.Windows.Forms.Button button_test_badge;
        private System.Windows.Forms.Button button_test_badge_stop;
        private System.Windows.Forms.Button button_read_badge;
        private System.Windows.Forms.Button buttonwrite_badge_template;
        private System.Windows.Forms.ComboBox comboBoxDetectorMode;
        private System.Windows.Forms.ComboBox camerasList;
        private System.Windows.Forms.ComboBox resolutionsList;
        private System.Windows.Forms.CheckBox checkBox_truncate_roi;
        private System.Windows.Forms.CheckBox checkBox_fullscreen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox reduce_list;
        private System.Windows.Forms.Button button_test_async;
        private System.Windows.Forms.CheckBox checkBox_async;
        private System.Windows.Forms.Button button_screenshot;
    }
}

