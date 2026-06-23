namespace FZ4P.UI
{
    partial class F_Manual
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.rb_DetectPin_Hold = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbb_Channel = new System.Windows.Forms.ComboBox();
            this.cbb_Aixs = new System.Windows.Forms.ComboBox();
            this.cbb_ADC_Select = new System.Windows.Forms.ComboBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.EditCondition = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_PositionMove = new System.Windows.Forms.Button();
            this.txt_PositionCode_AxisZ = new System.Windows.Forms.TextBox();
            this.txt_PositionCode_AxisY = new System.Windows.Forms.TextBox();
            this.txt_PositionCode_AxisX = new System.Windows.Forms.TextBox();
            this.btn_ServoOn = new System.Windows.Forms.Button();
            this.btn_Move_Max = new System.Windows.Forms.Button();
            this.btn_Move_Min = new System.Windows.Forms.Button();
            this.btn_Move_Mid = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.lbl_ReadHall3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_ReadHall2 = new System.Windows.Forms.Label();
            this.lbl_ADC = new System.Windows.Forms.Label();
            this.lbl_ReadHall = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.rb_DetectPin_Hold);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbb_Channel);
            this.groupBox1.Controls.Add(this.cbb_Aixs);
            this.groupBox1.Controls.Add(this.cbb_ADC_Select);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(410, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PeakCurrent";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(124, 79);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(55, 16);
            this.radioButton2.TabIndex = 280;
            this.radioButton2.TabStop = true;
            this.radioButton2.Tag = "2";
            this.radioButton2.Text = "Reset";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Click += new System.EventHandler(this.rb_DetectPin_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(60, 79);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(58, 16);
            this.radioButton1.TabIndex = 279;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "1";
            this.radioButton1.Text = "Detect";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Click += new System.EventHandler(this.rb_DetectPin_Click);
            // 
            // rb_DetectPin_Hold
            // 
            this.rb_DetectPin_Hold.AutoSize = true;
            this.rb_DetectPin_Hold.Location = new System.Drawing.Point(6, 79);
            this.rb_DetectPin_Hold.Name = "rb_DetectPin_Hold";
            this.rb_DetectPin_Hold.Size = new System.Drawing.Size(48, 16);
            this.rb_DetectPin_Hold.TabIndex = 278;
            this.rb_DetectPin_Hold.TabStop = true;
            this.rb_DetectPin_Hold.Tag = "0";
            this.rb_DetectPin_Hold.Text = "Hold";
            this.rb_DetectPin_Hold.UseVisualStyleBackColor = true;
            this.rb_DetectPin_Hold.Click += new System.EventHandler(this.rb_DetectPin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(286, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 12);
            this.label2.TabIndex = 277;
            this.label2.Text = "Axis";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 276;
            this.label1.Text = "Channel";
            // 
            // cbb_Channel
            // 
            this.cbb_Channel.FormattingEnabled = true;
            this.cbb_Channel.Location = new System.Drawing.Point(322, 0);
            this.cbb_Channel.Name = "cbb_Channel";
            this.cbb_Channel.Size = new System.Drawing.Size(64, 20);
            this.cbb_Channel.TabIndex = 275;
            this.cbb_Channel.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // cbb_Aixs
            // 
            this.cbb_Aixs.FormattingEnabled = true;
            this.cbb_Aixs.Location = new System.Drawing.Point(322, 27);
            this.cbb_Aixs.Name = "cbb_Aixs";
            this.cbb_Aixs.Size = new System.Drawing.Size(64, 20);
            this.cbb_Aixs.TabIndex = 274;
            // 
            // cbb_ADC_Select
            // 
            this.cbb_ADC_Select.FormattingEnabled = true;
            this.cbb_ADC_Select.Location = new System.Drawing.Point(160, 100);
            this.cbb_ADC_Select.Name = "cbb_ADC_Select";
            this.cbb_ADC_Select.Size = new System.Drawing.Size(64, 20);
            this.cbb_ADC_Select.TabIndex = 272;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBox2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.ForeColor = System.Drawing.Color.White;
            this.checkBox2.Location = new System.Drawing.Point(6, 101);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(148, 19);
            this.checkBox2.TabIndex = 271;
            this.checkBox2.Text = "Detect Pin Hold/Reset";
            this.checkBox2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 32);
            this.button1.TabIndex = 270;
            this.button1.Text = "Config Register Setting";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // EditCondition
            // 
            this.EditCondition.AutoSize = true;
            this.EditCondition.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.EditCondition.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditCondition.ForeColor = System.Drawing.Color.White;
            this.EditCondition.Location = new System.Drawing.Point(311, 0);
            this.EditCondition.Name = "EditCondition";
            this.EditCondition.Size = new System.Drawing.Size(58, 19);
            this.EditCondition.TabIndex = 269;
            this.EditCondition.Text = "START";
            this.EditCondition.UseVisualStyleBackColor = false;
            this.EditCondition.CheckedChanged += new System.EventHandler(this.EditCondition_CheckedChanged);
            this.EditCondition.CheckStateChanged += new System.EventHandler(this.EditCondition_CheckStateChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBox3);
            this.groupBox5.Controls.Add(this.checkBox1);
            this.groupBox5.Controls.Add(this.btn_PositionMove);
            this.groupBox5.Controls.Add(this.txt_PositionCode_AxisZ);
            this.groupBox5.Controls.Add(this.txt_PositionCode_AxisY);
            this.groupBox5.Controls.Add(this.txt_PositionCode_AxisX);
            this.groupBox5.Controls.Add(this.btn_ServoOn);
            this.groupBox5.Controls.Add(this.btn_Move_Max);
            this.groupBox5.Controls.Add(this.btn_Move_Min);
            this.groupBox5.Controls.Add(this.btn_Move_Mid);
            this.groupBox5.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.groupBox5.Location = new System.Drawing.Point(12, 319);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(392, 167);
            this.groupBox5.TabIndex = 512;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Center Move";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBox1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(265, -2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 19);
            this.checkBox1.TabIndex = 270;
            this.checkBox1.Text = "Power On/Off";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // btn_PositionMove
            // 
            this.btn_PositionMove.BackColor = System.Drawing.Color.Silver;
            this.btn_PositionMove.Location = new System.Drawing.Point(25, 130);
            this.btn_PositionMove.Name = "btn_PositionMove";
            this.btn_PositionMove.Size = new System.Drawing.Size(344, 27);
            this.btn_PositionMove.TabIndex = 8;
            this.btn_PositionMove.Text = "Move";
            this.btn_PositionMove.UseVisualStyleBackColor = false;
            this.btn_PositionMove.Click += new System.EventHandler(this.btn_PositionMove_Click);
            // 
            // txt_PositionCode_AxisZ
            // 
            this.txt_PositionCode_AxisZ.Location = new System.Drawing.Point(261, 56);
            this.txt_PositionCode_AxisZ.Name = "txt_PositionCode_AxisZ";
            this.txt_PositionCode_AxisZ.Size = new System.Drawing.Size(108, 25);
            this.txt_PositionCode_AxisZ.TabIndex = 7;
            // 
            // txt_PositionCode_AxisY
            // 
            this.txt_PositionCode_AxisY.Location = new System.Drawing.Point(147, 56);
            this.txt_PositionCode_AxisY.Name = "txt_PositionCode_AxisY";
            this.txt_PositionCode_AxisY.Size = new System.Drawing.Size(108, 25);
            this.txt_PositionCode_AxisY.TabIndex = 6;
            // 
            // txt_PositionCode_AxisX
            // 
            this.txt_PositionCode_AxisX.Location = new System.Drawing.Point(26, 56);
            this.txt_PositionCode_AxisX.Name = "txt_PositionCode_AxisX";
            this.txt_PositionCode_AxisX.Size = new System.Drawing.Size(108, 25);
            this.txt_PositionCode_AxisX.TabIndex = 5;
            // 
            // btn_ServoOn
            // 
            this.btn_ServoOn.Location = new System.Drawing.Point(26, 18);
            this.btn_ServoOn.Name = "btn_ServoOn";
            this.btn_ServoOn.Size = new System.Drawing.Size(343, 32);
            this.btn_ServoOn.TabIndex = 3;
            this.btn_ServoOn.Text = "Servo Set";
            this.btn_ServoOn.UseVisualStyleBackColor = true;
            this.btn_ServoOn.Click += new System.EventHandler(this.btn_ServoOn_Click);
            // 
            // btn_Move_Max
            // 
            this.btn_Move_Max.Location = new System.Drawing.Point(261, 87);
            this.btn_Move_Max.Name = "btn_Move_Max";
            this.btn_Move_Max.Size = new System.Drawing.Size(108, 37);
            this.btn_Move_Max.TabIndex = 2;
            this.btn_Move_Max.Text = "Max";
            this.btn_Move_Max.UseVisualStyleBackColor = true;
            this.btn_Move_Max.Click += new System.EventHandler(this.btn_Move_Max_Click);
            // 
            // btn_Move_Min
            // 
            this.btn_Move_Min.Location = new System.Drawing.Point(26, 87);
            this.btn_Move_Min.Name = "btn_Move_Min";
            this.btn_Move_Min.Size = new System.Drawing.Size(108, 37);
            this.btn_Move_Min.TabIndex = 1;
            this.btn_Move_Min.Text = "Min";
            this.btn_Move_Min.UseVisualStyleBackColor = true;
            this.btn_Move_Min.Click += new System.EventHandler(this.btn_Move_Min_Click);
            // 
            // btn_Move_Mid
            // 
            this.btn_Move_Mid.Location = new System.Drawing.Point(147, 87);
            this.btn_Move_Mid.Name = "btn_Move_Mid";
            this.btn_Move_Mid.Size = new System.Drawing.Size(108, 37);
            this.btn_Move_Mid.TabIndex = 0;
            this.btn_Move_Mid.Text = "Mid";
            this.btn_Move_Mid.UseVisualStyleBackColor = true;
            this.btn_Move_Mid.Click += new System.EventHandler(this.btn_Move_Mid_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel7);
            this.groupBox2.Controls.Add(this.panel8);
            this.groupBox2.Controls.Add(this.lbl_ReadHall3);
            this.groupBox2.Controls.Add(this.panel5);
            this.groupBox2.Controls.Add(this.panel6);
            this.groupBox2.Controls.Add(this.panel3);
            this.groupBox2.Controls.Add(this.panel4);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.lbl_ReadHall2);
            this.groupBox2.Controls.Add(this.EditCondition);
            this.groupBox2.Controls.Add(this.lbl_ADC);
            this.groupBox2.Controls.Add(this.lbl_ReadHall);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 299);
            this.groupBox2.TabIndex = 513;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Real Data";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel7.Controls.Add(this.label9);
            this.panel7.Location = new System.Drawing.Point(247, 116);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(112, 30);
            this.panel7.TabIndex = 293;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(37, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 12);
            this.label9.TabIndex = 279;
            this.label9.Text = "Code";
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel8.Controls.Add(this.label10);
            this.panel8.Location = new System.Drawing.Point(247, 20);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(112, 30);
            this.panel8.TabIndex = 292;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(27, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 12);
            this.label10.TabIndex = 278;
            this.label10.Text = "Channel3";
            // 
            // lbl_ReadHall3
            // 
            this.lbl_ReadHall3.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_ReadHall3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ReadHall3.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReadHall3.ForeColor = System.Drawing.Color.Yellow;
            this.lbl_ReadHall3.Location = new System.Drawing.Point(247, 51);
            this.lbl_ReadHall3.Name = "lbl_ReadHall3";
            this.lbl_ReadHall3.Size = new System.Drawing.Size(112, 64);
            this.lbl_ReadHall3.TabIndex = 291;
            this.lbl_ReadHall3.Text = "-";
            this.lbl_ReadHall3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel5.Controls.Add(this.label8);
            this.panel5.Location = new System.Drawing.Point(11, 248);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(134, 30);
            this.panel5.TabIndex = 290;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(48, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 12);
            this.label8.TabIndex = 279;
            this.label8.Text = "-";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel6.Controls.Add(this.label5);
            this.panel6.Location = new System.Drawing.Point(11, 152);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(134, 30);
            this.panel6.TabIndex = 289;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(38, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 278;
            this.label5.Text = "Channel";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(129, 116);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(112, 30);
            this.panel3.TabIndex = 288;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(37, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 12);
            this.label7.TabIndex = 279;
            this.label7.Text = "Code";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel4.Controls.Add(this.label4);
            this.panel4.Location = new System.Drawing.Point(129, 20);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(112, 30);
            this.panel4.TabIndex = 287;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(27, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 12);
            this.label4.TabIndex = 278;
            this.label4.Text = "Channel2";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(11, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(112, 30);
            this.panel2.TabIndex = 286;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(36, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 12);
            this.label6.TabIndex = 278;
            this.label6.Text = "Code";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(11, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(112, 30);
            this.panel1.TabIndex = 285;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(30, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 12);
            this.label3.TabIndex = 277;
            this.label3.Text = "Channel1";
            // 
            // lbl_ReadHall2
            // 
            this.lbl_ReadHall2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_ReadHall2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ReadHall2.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReadHall2.ForeColor = System.Drawing.Color.Yellow;
            this.lbl_ReadHall2.Location = new System.Drawing.Point(129, 51);
            this.lbl_ReadHall2.Name = "lbl_ReadHall2";
            this.lbl_ReadHall2.Size = new System.Drawing.Size(112, 64);
            this.lbl_ReadHall2.TabIndex = 284;
            this.lbl_ReadHall2.Text = "-";
            this.lbl_ReadHall2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_ADC
            // 
            this.lbl_ADC.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_ADC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ADC.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ADC.ForeColor = System.Drawing.Color.Yellow;
            this.lbl_ADC.Location = new System.Drawing.Point(10, 183);
            this.lbl_ADC.Name = "lbl_ADC";
            this.lbl_ADC.Size = new System.Drawing.Size(135, 64);
            this.lbl_ADC.TabIndex = 283;
            this.lbl_ADC.Text = "-";
            this.lbl_ADC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_ReadHall
            // 
            this.lbl_ReadHall.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lbl_ReadHall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ReadHall.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ReadHall.ForeColor = System.Drawing.Color.Yellow;
            this.lbl_ReadHall.Location = new System.Drawing.Point(11, 51);
            this.lbl_ReadHall.Name = "lbl_ReadHall";
            this.lbl_ReadHall.Size = new System.Drawing.Size(112, 64);
            this.lbl_ReadHall.TabIndex = 282;
            this.lbl_ReadHall.Text = "-";
            this.lbl_ReadHall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBox3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.ForeColor = System.Drawing.Color.White;
            this.checkBox3.Location = new System.Drawing.Point(168, -2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(91, 19);
            this.checkBox3.TabIndex = 514;
            this.checkBox3.Text = "OIS IC Reset";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckStateChanged += new System.EventHandler(this.checkBox3_CheckStateChanged);
            // 
            // F_Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 522);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Name = "F_Manual";
            this.Text = "F_Manual";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.F_Manual_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox EditCondition;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_PositionMove;
        private System.Windows.Forms.TextBox txt_PositionCode_AxisZ;
        private System.Windows.Forms.TextBox txt_PositionCode_AxisY;
        private System.Windows.Forms.TextBox txt_PositionCode_AxisX;
        private System.Windows.Forms.Button btn_ServoOn;
        private System.Windows.Forms.Button btn_Move_Max;
        private System.Windows.Forms.Button btn_Move_Min;
        private System.Windows.Forms.Button btn_Move_Mid;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ComboBox cbb_ADC_Select;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbb_Channel;
        private System.Windows.Forms.ComboBox cbb_Aixs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rb_DetectPin_Hold;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbl_ReadHall2;
        private System.Windows.Forms.Label lbl_ADC;
        private System.Windows.Forms.Label lbl_ReadHall;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_ReadHall3;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}