
namespace FZ4P
{
    partial class F_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_Main));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.SpecGrid = new System.Windows.Forms.DataGridView();
            this.ModelGroup = new System.Windows.Forms.GroupBox();
            this.btnBarcodeOff = new System.Windows.Forms.Button();
            this.btnBarcodeOn = new System.Windows.Forms.Button();
            this.lblDefaultModel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lbxModelFiles = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MCtypeList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbMcNum = new System.Windows.Forms.TextBox();
            this.TesterNo = new System.Windows.Forms.TextBox();
            this.ApplyTester = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.OpenSpec = new System.Windows.Forms.Button();
            this.EditSpec = new System.Windows.Forms.CheckBox();
            this.SaveSpec = new System.Windows.Forms.Button();
            this.SaveAsSpec = new System.Windows.Forms.Button();
            this.SpecFileName = new System.Windows.Forms.TextBox();
            this.P_AutoLearn = new System.Windows.Forms.Panel();
            this.P_Vision = new System.Windows.Forms.Panel();
            this.P_Manager = new System.Windows.Forms.Panel();
            this.P_Main = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.SettleDataGrid = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.RetryGrid = new System.Windows.Forms.DataGridView();
            this.SetOISBaseCalUpdate = new System.Windows.Forms.Button();
            this.OISBaseCalSetPath = new System.Windows.Forms.RichTextBox();
            this.SetOISFWUpdate = new System.Windows.Forms.Button();
            this.OISFWSetPath = new System.Windows.Forms.RichTextBox();
            this.SetAFPIDUpdate = new System.Windows.Forms.Button();
            this.AFPidSetPath = new System.Windows.Forms.RichTextBox();
            this.ConditinGrid = new System.Windows.Forms.DataGridView();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnToMotion = new System.Windows.Forms.Button();
            this.ToVision = new System.Windows.Forms.Button();
            this.ToOperator = new System.Windows.Forms.Button();
            this.P_Motion = new System.Windows.Forms.Panel();
            this.Move_Down = new System.Windows.Forms.Button();
            this.Move_Up = new System.Windows.Forms.Button();
            this.tbToDoList = new System.Windows.Forms.TextBox();
            this.Actionbox = new System.Windows.Forms.ListBox();
            this.TodoBox = new System.Windows.Forms.ListBox();
            this.tbActionList = new System.Windows.Forms.TextBox();
            this.RemoveItem = new System.Windows.Forms.Button();
            this.AddItem = new System.Windows.Forms.Button();
            this.EditCondition = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.SaveAsCondition = new System.Windows.Forms.Button();
            this.SaveCondition = new System.Windows.Forms.Button();
            this.OpenCondition = new System.Windows.Forms.Button();
            this.RecipeFileName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SpecGrid)).BeginInit();
            this.ModelGroup.SuspendLayout();
            this.panel4.SuspendLayout();
            this.P_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SettleDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RetryGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConditinGrid)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SpecGrid
            // 
            this.SpecGrid.AllowUserToAddRows = false;
            this.SpecGrid.AllowUserToDeleteRows = false;
            this.SpecGrid.AllowUserToResizeColumns = false;
            this.SpecGrid.AllowUserToResizeRows = false;
            this.SpecGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SpecGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SpecGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.SpecGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SpecGrid.Location = new System.Drawing.Point(769, 152);
            this.SpecGrid.Name = "SpecGrid";
            this.SpecGrid.RowHeadersWidth = 72;
            this.SpecGrid.RowTemplate.Height = 23;
            this.SpecGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SpecGrid.Size = new System.Drawing.Size(473, 843);
            this.SpecGrid.TabIndex = 78;
            this.SpecGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.SpecGrid_CellValueChanged);
            this.SpecGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.SpecGrid_CurrentCellDirtyStateChanged);
            // 
            // ModelGroup
            // 
            this.ModelGroup.BackColor = System.Drawing.Color.LightGray;
            this.ModelGroup.Controls.Add(this.btnBarcodeOff);
            this.ModelGroup.Controls.Add(this.btnBarcodeOn);
            this.ModelGroup.Controls.Add(this.lblDefaultModel);
            this.ModelGroup.Controls.Add(this.button1);
            this.ModelGroup.Controls.Add(this.lbxModelFiles);
            this.ModelGroup.Controls.Add(this.label2);
            this.ModelGroup.Controls.Add(this.MCtypeList);
            this.ModelGroup.Controls.Add(this.label1);
            this.ModelGroup.Controls.Add(this.label15);
            this.ModelGroup.Controls.Add(this.tbMcNum);
            this.ModelGroup.Controls.Add(this.TesterNo);
            this.ModelGroup.Controls.Add(this.ApplyTester);
            this.ModelGroup.Location = new System.Drawing.Point(1248, 48);
            this.ModelGroup.Name = "ModelGroup";
            this.ModelGroup.Size = new System.Drawing.Size(647, 524);
            this.ModelGroup.TabIndex = 159;
            this.ModelGroup.TabStop = false;
            // 
            // btnBarcodeOff
            // 
            this.btnBarcodeOff.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnBarcodeOff.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBarcodeOff.BackgroundImage")));
            this.btnBarcodeOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBarcodeOff.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBarcodeOff.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.btnBarcodeOff.ForeColor = System.Drawing.Color.Black;
            this.btnBarcodeOff.Location = new System.Drawing.Point(6, 339);
            this.btnBarcodeOff.Name = "btnBarcodeOff";
            this.btnBarcodeOff.Size = new System.Drawing.Size(178, 35);
            this.btnBarcodeOff.TabIndex = 269;
            this.btnBarcodeOff.Text = "Barcode Off";
            this.btnBarcodeOff.UseVisualStyleBackColor = false;
            this.btnBarcodeOff.Click += new System.EventHandler(this.btnBarcodeOff_Click);
            // 
            // btnBarcodeOn
            // 
            this.btnBarcodeOn.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnBarcodeOn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBarcodeOn.BackgroundImage")));
            this.btnBarcodeOn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBarcodeOn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBarcodeOn.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold);
            this.btnBarcodeOn.ForeColor = System.Drawing.Color.Black;
            this.btnBarcodeOn.Location = new System.Drawing.Point(6, 298);
            this.btnBarcodeOn.Name = "btnBarcodeOn";
            this.btnBarcodeOn.Size = new System.Drawing.Size(178, 35);
            this.btnBarcodeOn.TabIndex = 268;
            this.btnBarcodeOn.Text = "Barcode On ";
            this.btnBarcodeOn.UseVisualStyleBackColor = false;
            this.btnBarcodeOn.Click += new System.EventHandler(this.btnBarcodeOn_Click);
            // 
            // lblDefaultModel
            // 
            this.lblDefaultModel.AutoSize = true;
            this.lblDefaultModel.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblDefaultModel.Location = new System.Drawing.Point(6, 494);
            this.lblDefaultModel.Name = "lblDefaultModel";
            this.lblDefaultModel.Size = new System.Drawing.Size(126, 17);
            this.lblDefaultModel.TabIndex = 239;
            this.lblDefaultModel.Text = "Default Model File";
            this.lblDefaultModel.Visible = false;
            // 
            // button1
            // 
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.button1.Location = new System.Drawing.Point(4, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 28);
            this.button1.TabIndex = 238;
            this.button1.Text = "Load Model FIle List";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbxModelFiles
            // 
            this.lbxModelFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.lbxModelFiles.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxModelFiles.FormattingEnabled = true;
            this.lbxModelFiles.ItemHeight = 15;
            this.lbxModelFiles.Location = new System.Drawing.Point(4, 427);
            this.lbxModelFiles.Name = "lbxModelFiles";
            this.lbxModelFiles.Size = new System.Drawing.Size(643, 49);
            this.lbxModelFiles.TabIndex = 237;
            this.lbxModelFiles.SelectedIndexChanged += new System.EventHandler(this.lbxModelFiles_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.MediumBlue;
            this.label2.Location = new System.Drawing.Point(54, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 23);
            this.label2.TabIndex = 236;
            this.label2.Text = "MC Type";
            // 
            // MCtypeList
            // 
            this.MCtypeList.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MCtypeList.ForeColor = System.Drawing.Color.MediumBlue;
            this.MCtypeList.FormattingEnabled = true;
            this.MCtypeList.ItemHeight = 19;
            this.MCtypeList.Location = new System.Drawing.Point(138, 128);
            this.MCtypeList.Name = "MCtypeList";
            this.MCtypeList.Size = new System.Drawing.Size(130, 80);
            this.MCtypeList.TabIndex = 235;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 23);
            this.label1.TabIndex = 226;
            this.label1.Text = "Machine No.";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(46, 94);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(91, 23);
            this.label15.TabIndex = 226;
            this.label15.Text = "Tester No.";
            // 
            // tbMcNum
            // 
            this.tbMcNum.BackColor = System.Drawing.Color.White;
            this.tbMcNum.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.tbMcNum.Location = new System.Drawing.Point(139, 51);
            this.tbMcNum.Name = "tbMcNum";
            this.tbMcNum.Size = new System.Drawing.Size(130, 30);
            this.tbMcNum.TabIndex = 225;
            this.tbMcNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TesterNo
            // 
            this.TesterNo.BackColor = System.Drawing.Color.White;
            this.TesterNo.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Bold);
            this.TesterNo.Location = new System.Drawing.Point(139, 91);
            this.TesterNo.Name = "TesterNo";
            this.TesterNo.Size = new System.Drawing.Size(130, 30);
            this.TesterNo.TabIndex = 225;
            this.TesterNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TesterNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TesterNo_KeyPress);
            // 
            // ApplyTester
            // 
            this.ApplyTester.BackColor = System.Drawing.Color.MediumBlue;
            this.ApplyTester.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ApplyTester.BackgroundImage")));
            this.ApplyTester.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ApplyTester.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ApplyTester.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.ApplyTester.ForeColor = System.Drawing.Color.White;
            this.ApplyTester.Location = new System.Drawing.Point(9, 9);
            this.ApplyTester.Name = "ApplyTester";
            this.ApplyTester.Size = new System.Drawing.Size(179, 34);
            this.ApplyTester.TabIndex = 132;
            this.ApplyTester.Text = "Apply";
            this.ApplyTester.UseVisualStyleBackColor = false;
            this.ApplyTester.Click += new System.EventHandler(this.ApplyTester_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel4.Controls.Add(this.textBox4);
            this.panel4.Controls.Add(this.OpenSpec);
            this.panel4.Controls.Add(this.EditSpec);
            this.panel4.Controls.Add(this.SaveSpec);
            this.panel4.Controls.Add(this.SaveAsSpec);
            this.panel4.Location = new System.Drawing.Point(769, 46);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(474, 70);
            this.panel4.TabIndex = 91;
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.Color.DimGray;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.ForeColor = System.Drawing.Color.White;
            this.textBox4.Location = new System.Drawing.Point(0, -2);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(474, 26);
            this.textBox4.TabIndex = 90;
            this.textBox4.TabStop = false;
            this.textBox4.Text = "Test Spec";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // OpenSpec
            // 
            this.OpenSpec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OpenSpec.BackgroundImage")));
            this.OpenSpec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.OpenSpec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.OpenSpec.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.OpenSpec.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.OpenSpec.Location = new System.Drawing.Point(15, 31);
            this.OpenSpec.Name = "OpenSpec";
            this.OpenSpec.Size = new System.Drawing.Size(120, 28);
            this.OpenSpec.TabIndex = 88;
            this.OpenSpec.Text = "Open";
            this.OpenSpec.UseVisualStyleBackColor = true;
            this.OpenSpec.Click += new System.EventHandler(this.OpenSpec_Click);
            // 
            // EditSpec
            // 
            this.EditSpec.AutoSize = true;
            this.EditSpec.BackColor = System.Drawing.Color.Transparent;
            this.EditSpec.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditSpec.ForeColor = System.Drawing.Color.White;
            this.EditSpec.Location = new System.Drawing.Point(401, 37);
            this.EditSpec.Name = "EditSpec";
            this.EditSpec.Size = new System.Drawing.Size(47, 19);
            this.EditSpec.TabIndex = 1;
            this.EditSpec.Text = "Edit";
            this.EditSpec.UseVisualStyleBackColor = false;
            this.EditSpec.CheckedChanged += new System.EventHandler(this.EditSpec_CheckedChanged);
            // 
            // SaveSpec
            // 
            this.SaveSpec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SaveSpec.BackgroundImage")));
            this.SaveSpec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SaveSpec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveSpec.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.SaveSpec.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.SaveSpec.Location = new System.Drawing.Point(141, 31);
            this.SaveSpec.Name = "SaveSpec";
            this.SaveSpec.Size = new System.Drawing.Size(120, 28);
            this.SaveSpec.TabIndex = 87;
            this.SaveSpec.Text = "Save";
            this.SaveSpec.UseVisualStyleBackColor = true;
            this.SaveSpec.Click += new System.EventHandler(this.SaveSpec_Click);
            // 
            // SaveAsSpec
            // 
            this.SaveAsSpec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SaveAsSpec.BackgroundImage")));
            this.SaveAsSpec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SaveAsSpec.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveAsSpec.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.SaveAsSpec.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.SaveAsSpec.Location = new System.Drawing.Point(267, 31);
            this.SaveAsSpec.Name = "SaveAsSpec";
            this.SaveAsSpec.Size = new System.Drawing.Size(120, 28);
            this.SaveAsSpec.TabIndex = 89;
            this.SaveAsSpec.Text = "Save As";
            this.SaveAsSpec.UseVisualStyleBackColor = true;
            this.SaveAsSpec.Click += new System.EventHandler(this.SaveAsSpec_Click);
            // 
            // SpecFileName
            // 
            this.SpecFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SpecFileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SpecFileName.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpecFileName.ForeColor = System.Drawing.Color.LightGray;
            this.SpecFileName.Location = new System.Drawing.Point(769, 118);
            this.SpecFileName.Name = "SpecFileName";
            this.SpecFileName.ReadOnly = true;
            this.SpecFileName.Size = new System.Drawing.Size(474, 24);
            this.SpecFileName.TabIndex = 89;
            this.SpecFileName.TabStop = false;
            this.SpecFileName.Text = "Spec File Name";
            this.SpecFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // P_AutoLearn
            // 
            this.P_AutoLearn.Location = new System.Drawing.Point(117, 1023);
            this.P_AutoLearn.Name = "P_AutoLearn";
            this.P_AutoLearn.Size = new System.Drawing.Size(50, 31);
            this.P_AutoLearn.TabIndex = 196;
            // 
            // P_Vision
            // 
            this.P_Vision.Location = new System.Drawing.Point(61, 1023);
            this.P_Vision.Name = "P_Vision";
            this.P_Vision.Size = new System.Drawing.Size(50, 31);
            this.P_Vision.TabIndex = 195;
            // 
            // P_Manager
            // 
            this.P_Manager.Location = new System.Drawing.Point(5, 1023);
            this.P_Manager.Name = "P_Manager";
            this.P_Manager.Size = new System.Drawing.Size(50, 31);
            this.P_Manager.TabIndex = 194;
            // 
            // P_Main
            // 
            this.P_Main.Controls.Add(this.label5);
            this.P_Main.Controls.Add(this.SettleDataGrid);
            this.P_Main.Controls.Add(this.button2);
            this.P_Main.Controls.Add(this.RetryGrid);
            this.P_Main.Controls.Add(this.SetOISBaseCalUpdate);
            this.P_Main.Controls.Add(this.OISBaseCalSetPath);
            this.P_Main.Controls.Add(this.SetOISFWUpdate);
            this.P_Main.Controls.Add(this.OISFWSetPath);
            this.P_Main.Controls.Add(this.SetAFPIDUpdate);
            this.P_Main.Controls.Add(this.AFPidSetPath);
            this.P_Main.Controls.Add(this.ConditinGrid);
            this.P_Main.Controls.Add(this.textBox2);
            this.P_Main.Controls.Add(this.SpecGrid);
            this.P_Main.Controls.Add(this.ModelGroup);
            this.P_Main.Controls.Add(this.panel2);
            this.P_Main.Controls.Add(this.panel4);
            this.P_Main.Controls.Add(this.SpecFileName);
            this.P_Main.Location = new System.Drawing.Point(2, 1);
            this.P_Main.Name = "P_Main";
            this.P_Main.Size = new System.Drawing.Size(1898, 998);
            this.P_Main.TabIndex = 197;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(110)))), ((int)(((byte)(119)))));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(2, 704);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(357, 22);
            this.label5.TabIndex = 247;
            this.label5.Text = "AF Settling Setting";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettleDataGrid
            // 
            this.SettleDataGrid.AllowUserToAddRows = false;
            this.SettleDataGrid.AllowUserToDeleteRows = false;
            this.SettleDataGrid.AllowUserToResizeColumns = false;
            this.SettleDataGrid.AllowUserToResizeRows = false;
            this.SettleDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SettleDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SettleDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.SettleDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SettleDataGrid.Location = new System.Drawing.Point(2, 729);
            this.SettleDataGrid.Name = "SettleDataGrid";
            this.SettleDataGrid.RowHeadersWidth = 72;
            this.SettleDataGrid.RowTemplate.Height = 23;
            this.SettleDataGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SettleDataGrid.Size = new System.Drawing.Size(357, 265);
            this.SettleDataGrid.TabIndex = 246;
            this.SettleDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.SettleDataGrid_CellValueChanged);
            this.SettleDataGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.SettleDataGrid_CurrentCellDirtyStateChanged);
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.button2.Location = new System.Drawing.Point(1414, 577);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 28);
            this.button2.TabIndex = 245;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // RetryGrid
            // 
            this.RetryGrid.AllowUserToAddRows = false;
            this.RetryGrid.AllowUserToDeleteRows = false;
            this.RetryGrid.AllowUserToResizeColumns = false;
            this.RetryGrid.AllowUserToResizeRows = false;
            this.RetryGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RetryGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.RetryGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.RetryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RetryGrid.Location = new System.Drawing.Point(1246, 610);
            this.RetryGrid.Name = "RetryGrid";
            this.RetryGrid.RowHeadersWidth = 72;
            this.RetryGrid.RowTemplate.Height = 23;
            this.RetryGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.RetryGrid.Size = new System.Drawing.Size(299, 385);
            this.RetryGrid.TabIndex = 244;
            // 
            // SetOISBaseCalUpdate
            // 
            this.SetOISBaseCalUpdate.BackColor = System.Drawing.Color.MediumBlue;
            this.SetOISBaseCalUpdate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SetOISBaseCalUpdate.BackgroundImage")));
            this.SetOISBaseCalUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SetOISBaseCalUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SetOISBaseCalUpdate.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.SetOISBaseCalUpdate.ForeColor = System.Drawing.Color.White;
            this.SetOISBaseCalUpdate.Location = new System.Drawing.Point(1557, 860);
            this.SetOISBaseCalUpdate.Name = "SetOISBaseCalUpdate";
            this.SetOISBaseCalUpdate.Size = new System.Drawing.Size(337, 43);
            this.SetOISBaseCalUpdate.TabIndex = 241;
            this.SetOISBaseCalUpdate.Text = "Set OIS BaseCal Update File";
            this.SetOISBaseCalUpdate.UseVisualStyleBackColor = false;
            this.SetOISBaseCalUpdate.Click += new System.EventHandler(this.SetOISBaseCalUpdate_Click);
            // 
            // OISBaseCalSetPath
            // 
            this.OISBaseCalSetPath.BackColor = System.Drawing.Color.White;
            this.OISBaseCalSetPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OISBaseCalSetPath.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OISBaseCalSetPath.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.OISBaseCalSetPath.Location = new System.Drawing.Point(1557, 909);
            this.OISBaseCalSetPath.Name = "OISBaseCalSetPath";
            this.OISBaseCalSetPath.ReadOnly = true;
            this.OISBaseCalSetPath.Size = new System.Drawing.Size(334, 65);
            this.OISBaseCalSetPath.TabIndex = 240;
            this.OISBaseCalSetPath.Text = "";
            // 
            // SetOISFWUpdate
            // 
            this.SetOISFWUpdate.BackColor = System.Drawing.Color.MediumBlue;
            this.SetOISFWUpdate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SetOISFWUpdate.BackgroundImage")));
            this.SetOISFWUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SetOISFWUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SetOISFWUpdate.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.SetOISFWUpdate.ForeColor = System.Drawing.Color.White;
            this.SetOISFWUpdate.Location = new System.Drawing.Point(1557, 746);
            this.SetOISFWUpdate.Name = "SetOISFWUpdate";
            this.SetOISFWUpdate.Size = new System.Drawing.Size(337, 43);
            this.SetOISFWUpdate.TabIndex = 239;
            this.SetOISFWUpdate.Text = "Set OIS FW Update File";
            this.SetOISFWUpdate.UseVisualStyleBackColor = false;
            this.SetOISFWUpdate.Click += new System.EventHandler(this.SetOISFWUpdate_Click);
            // 
            // OISFWSetPath
            // 
            this.OISFWSetPath.BackColor = System.Drawing.Color.White;
            this.OISFWSetPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OISFWSetPath.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.OISFWSetPath.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.OISFWSetPath.Location = new System.Drawing.Point(1557, 795);
            this.OISFWSetPath.Name = "OISFWSetPath";
            this.OISFWSetPath.ReadOnly = true;
            this.OISFWSetPath.Size = new System.Drawing.Size(334, 59);
            this.OISFWSetPath.TabIndex = 238;
            this.OISFWSetPath.Text = "";
            // 
            // SetAFPIDUpdate
            // 
            this.SetAFPIDUpdate.BackColor = System.Drawing.Color.MediumBlue;
            this.SetAFPIDUpdate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SetAFPIDUpdate.BackgroundImage")));
            this.SetAFPIDUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SetAFPIDUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SetAFPIDUpdate.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.SetAFPIDUpdate.ForeColor = System.Drawing.Color.White;
            this.SetAFPIDUpdate.Location = new System.Drawing.Point(1558, 632);
            this.SetAFPIDUpdate.Name = "SetAFPIDUpdate";
            this.SetAFPIDUpdate.Size = new System.Drawing.Size(337, 43);
            this.SetAFPIDUpdate.TabIndex = 237;
            this.SetAFPIDUpdate.Text = "Set AF PID Update File";
            this.SetAFPIDUpdate.UseVisualStyleBackColor = false;
            this.SetAFPIDUpdate.Click += new System.EventHandler(this.SetAFPIDUpdate_Click);
            // 
            // AFPidSetPath
            // 
            this.AFPidSetPath.BackColor = System.Drawing.Color.White;
            this.AFPidSetPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AFPidSetPath.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.AFPidSetPath.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.AFPidSetPath.Location = new System.Drawing.Point(1558, 681);
            this.AFPidSetPath.Name = "AFPidSetPath";
            this.AFPidSetPath.ReadOnly = true;
            this.AFPidSetPath.Size = new System.Drawing.Size(334, 59);
            this.AFPidSetPath.TabIndex = 191;
            this.AFPidSetPath.Text = "";
            // 
            // ConditinGrid
            // 
            this.ConditinGrid.AllowUserToAddRows = false;
            this.ConditinGrid.AllowUserToDeleteRows = false;
            this.ConditinGrid.AllowUserToResizeColumns = false;
            this.ConditinGrid.AllowUserToResizeRows = false;
            this.ConditinGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConditinGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ConditinGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.ConditinGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConditinGrid.Location = new System.Drawing.Point(364, 43);
            this.ConditinGrid.Name = "ConditinGrid";
            this.ConditinGrid.RowHeadersWidth = 72;
            this.ConditinGrid.RowTemplate.Height = 23;
            this.ConditinGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ConditinGrid.Size = new System.Drawing.Size(399, 951);
            this.ConditinGrid.TabIndex = 87;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.LightGray;
            this.textBox2.Location = new System.Drawing.Point(1246, 578);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(162, 26);
            this.textBox2.TabIndex = 89;
            this.textBox2.TabStop = false;
            this.textBox2.Text = "Retry Count";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Silver;
            this.panel2.Controls.Add(this.btnToMotion);
            this.panel2.Controls.Add(this.ToVision);
            this.panel2.Controls.Add(this.ToOperator);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1907, 42);
            this.panel2.TabIndex = 32;
            // 
            // btnToMotion
            // 
            this.btnToMotion.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnToMotion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnToMotion.BackgroundImage")));
            this.btnToMotion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnToMotion.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.btnToMotion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnToMotion.Location = new System.Drawing.Point(765, 0);
            this.btnToMotion.Name = "btnToMotion";
            this.btnToMotion.Size = new System.Drawing.Size(400, 45);
            this.btnToMotion.TabIndex = 121;
            this.btnToMotion.Text = "Motion";
            this.btnToMotion.UseVisualStyleBackColor = false;
            this.btnToMotion.Click += new System.EventHandler(this.btnToMotion_Click);
            // 
            // ToVision
            // 
            this.ToVision.BackColor = System.Drawing.Color.DodgerBlue;
            this.ToVision.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ToVision.BackgroundImage")));
            this.ToVision.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ToVision.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.ToVision.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ToVision.Location = new System.Drawing.Point(363, 0);
            this.ToVision.Name = "ToVision";
            this.ToVision.Size = new System.Drawing.Size(400, 45);
            this.ToVision.TabIndex = 68;
            this.ToVision.Text = "Vision";
            this.ToVision.UseVisualStyleBackColor = false;
            this.ToVision.Click += new System.EventHandler(this.ToVision_Click);
            // 
            // ToOperator
            // 
            this.ToOperator.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.ToOperator.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ToOperator.BackgroundImage")));
            this.ToOperator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ToOperator.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.ToOperator.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ToOperator.Location = new System.Drawing.Point(0, 0);
            this.ToOperator.Name = "ToOperator";
            this.ToOperator.Size = new System.Drawing.Size(357, 45);
            this.ToOperator.TabIndex = 120;
            this.ToOperator.Text = "Operator";
            this.ToOperator.UseVisualStyleBackColor = false;
            this.ToOperator.Click += new System.EventHandler(this.ToOperator_Click);
            // 
            // P_Motion
            // 
            this.P_Motion.Location = new System.Drawing.Point(173, 1023);
            this.P_Motion.Name = "P_Motion";
            this.P_Motion.Size = new System.Drawing.Size(50, 31);
            this.P_Motion.TabIndex = 198;
            // 
            // Move_Down
            // 
            this.Move_Down.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Move_Down.Location = new System.Drawing.Point(207, 672);
            this.Move_Down.Name = "Move_Down";
            this.Move_Down.Size = new System.Drawing.Size(101, 30);
            this.Move_Down.TabIndex = 193;
            this.Move_Down.Text = "Dn";
            this.Move_Down.UseVisualStyleBackColor = true;
            this.Move_Down.Click += new System.EventHandler(this.Move_Down_Click);
            // 
            // Move_Up
            // 
            this.Move_Up.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Move_Up.Location = new System.Drawing.Point(38, 672);
            this.Move_Up.Name = "Move_Up";
            this.Move_Up.Size = new System.Drawing.Size(101, 30);
            this.Move_Up.TabIndex = 192;
            this.Move_Up.Text = "Up";
            this.Move_Up.UseVisualStyleBackColor = true;
            this.Move_Up.Click += new System.EventHandler(this.Move_Up_Click);
            // 
            // tbToDoList
            // 
            this.tbToDoList.BackColor = System.Drawing.Color.White;
            this.tbToDoList.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbToDoList.ForeColor = System.Drawing.Color.Black;
            this.tbToDoList.Location = new System.Drawing.Point(4, 427);
            this.tbToDoList.Name = "tbToDoList";
            this.tbToDoList.ReadOnly = true;
            this.tbToDoList.Size = new System.Drawing.Size(355, 23);
            this.tbToDoList.TabIndex = 191;
            this.tbToDoList.TabStop = false;
            this.tbToDoList.Text = "To Do List";
            this.tbToDoList.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Actionbox
            // 
            this.Actionbox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Actionbox.FormattingEnabled = true;
            this.Actionbox.ItemHeight = 15;
            this.Actionbox.Location = new System.Drawing.Point(3, 170);
            this.Actionbox.Name = "Actionbox";
            this.Actionbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.Actionbox.Size = new System.Drawing.Size(356, 214);
            this.Actionbox.TabIndex = 186;
            this.Actionbox.SelectedIndexChanged += new System.EventHandler(this.Actionbox_SelectedIndexChanged);
            // 
            // TodoBox
            // 
            this.TodoBox.BackColor = System.Drawing.Color.PaleGreen;
            this.TodoBox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TodoBox.FormattingEnabled = true;
            this.TodoBox.ItemHeight = 15;
            this.TodoBox.Location = new System.Drawing.Point(2, 451);
            this.TodoBox.Name = "TodoBox";
            this.TodoBox.Size = new System.Drawing.Size(357, 214);
            this.TodoBox.TabIndex = 190;
            this.TodoBox.SelectedIndexChanged += new System.EventHandler(this.Actionbox_SelectedIndexChanged);
            // 
            // tbActionList
            // 
            this.tbActionList.BackColor = System.Drawing.Color.White;
            this.tbActionList.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbActionList.ForeColor = System.Drawing.Color.Black;
            this.tbActionList.Location = new System.Drawing.Point(3, 146);
            this.tbActionList.Name = "tbActionList";
            this.tbActionList.ReadOnly = true;
            this.tbActionList.Size = new System.Drawing.Size(356, 23);
            this.tbActionList.TabIndex = 187;
            this.tbActionList.TabStop = false;
            this.tbActionList.Text = "Action List";
            this.tbActionList.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RemoveItem
            // 
            this.RemoveItem.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveItem.Location = new System.Drawing.Point(38, 389);
            this.RemoveItem.Name = "RemoveItem";
            this.RemoveItem.Size = new System.Drawing.Size(101, 34);
            this.RemoveItem.TabIndex = 189;
            this.RemoveItem.Text = "↑↑";
            this.RemoveItem.UseVisualStyleBackColor = true;
            this.RemoveItem.Click += new System.EventHandler(this.RemoveItem_Click);
            // 
            // AddItem
            // 
            this.AddItem.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddItem.Location = new System.Drawing.Point(207, 389);
            this.AddItem.Name = "AddItem";
            this.AddItem.Size = new System.Drawing.Size(101, 34);
            this.AddItem.TabIndex = 188;
            this.AddItem.Text = "↓↓";
            this.AddItem.UseVisualStyleBackColor = true;
            this.AddItem.Click += new System.EventHandler(this.AddItem_Click);
            // 
            // EditCondition
            // 
            this.EditCondition.AutoSize = true;
            this.EditCondition.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.EditCondition.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditCondition.ForeColor = System.Drawing.Color.White;
            this.EditCondition.Location = new System.Drawing.Point(302, 39);
            this.EditCondition.Name = "EditCondition";
            this.EditCondition.Size = new System.Drawing.Size(47, 19);
            this.EditCondition.TabIndex = 0;
            this.EditCondition.Text = "Edit";
            this.EditCondition.UseVisualStyleBackColor = false;
            this.EditCondition.CheckedChanged += new System.EventHandler(this.EditCondition_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.DimGray;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(358, 26);
            this.textBox1.TabIndex = 92;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "Recipe";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel3.Controls.Add(this.textBox1);
            this.panel3.Controls.Add(this.SaveAsCondition);
            this.panel3.Controls.Add(this.SaveCondition);
            this.panel3.Controls.Add(this.OpenCondition);
            this.panel3.Controls.Add(this.EditCondition);
            this.panel3.Location = new System.Drawing.Point(2, 47);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(357, 72);
            this.panel3.TabIndex = 185;
            // 
            // SaveAsCondition
            // 
            this.SaveAsCondition.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SaveAsCondition.BackgroundImage")));
            this.SaveAsCondition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SaveAsCondition.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveAsCondition.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.SaveAsCondition.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.SaveAsCondition.Location = new System.Drawing.Point(205, 34);
            this.SaveAsCondition.Name = "SaveAsCondition";
            this.SaveAsCondition.Size = new System.Drawing.Size(89, 28);
            this.SaveAsCondition.TabIndex = 70;
            this.SaveAsCondition.Text = "Save As";
            this.SaveAsCondition.UseVisualStyleBackColor = true;
            this.SaveAsCondition.Click += new System.EventHandler(this.SaveAsCondition_Click);
            // 
            // SaveCondition
            // 
            this.SaveCondition.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SaveCondition.BackgroundImage")));
            this.SaveCondition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SaveCondition.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveCondition.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.SaveCondition.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.SaveCondition.Location = new System.Drawing.Point(104, 34);
            this.SaveCondition.Name = "SaveCondition";
            this.SaveCondition.Size = new System.Drawing.Size(89, 28);
            this.SaveCondition.TabIndex = 69;
            this.SaveCondition.Text = "Save";
            this.SaveCondition.UseVisualStyleBackColor = true;
            this.SaveCondition.Click += new System.EventHandler(this.SaveCondition_Click);
            // 
            // OpenCondition
            // 
            this.OpenCondition.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("OpenCondition.BackgroundImage")));
            this.OpenCondition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.OpenCondition.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.OpenCondition.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.OpenCondition.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.OpenCondition.Location = new System.Drawing.Point(3, 34);
            this.OpenCondition.Name = "OpenCondition";
            this.OpenCondition.Size = new System.Drawing.Size(89, 28);
            this.OpenCondition.TabIndex = 68;
            this.OpenCondition.Text = "Open";
            this.OpenCondition.UseVisualStyleBackColor = true;
            this.OpenCondition.Click += new System.EventHandler(this.OpenCondition_Click);
            // 
            // RecipeFileName
            // 
            this.RecipeFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.RecipeFileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RecipeFileName.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RecipeFileName.ForeColor = System.Drawing.Color.LightGray;
            this.RecipeFileName.Location = new System.Drawing.Point(2, 119);
            this.RecipeFileName.Name = "RecipeFileName";
            this.RecipeFileName.ReadOnly = true;
            this.RecipeFileName.Size = new System.Drawing.Size(357, 24);
            this.RecipeFileName.TabIndex = 184;
            this.RecipeFileName.TabStop = false;
            this.RecipeFileName.Text = "Recipe File Name";
            this.RecipeFileName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // F_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.P_AutoLearn);
            this.Controls.Add(this.P_Vision);
            this.Controls.Add(this.P_Manager);
            this.Controls.Add(this.P_Motion);
            this.Controls.Add(this.Move_Down);
            this.Controls.Add(this.Move_Up);
            this.Controls.Add(this.tbToDoList);
            this.Controls.Add(this.Actionbox);
            this.Controls.Add(this.TodoBox);
            this.Controls.Add(this.tbActionList);
            this.Controls.Add(this.RemoveItem);
            this.Controls.Add(this.AddItem);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.RecipeFileName);
            this.Controls.Add(this.P_Main);
            this.Name = "F_Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "6AxisTester_26062201_SO1C86";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.F_Main_FormClosing);
            this.Load += new System.EventHandler(this.F_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SpecGrid)).EndInit();
            this.ModelGroup.ResumeLayout(false);
            this.ModelGroup.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.P_Main.ResumeLayout(false);
            this.P_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SettleDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RetryGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConditinGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView SpecGrid;
        private System.Windows.Forms.GroupBox ModelGroup;
        private System.Windows.Forms.Button ApplyTester;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button OpenSpec;
        private System.Windows.Forms.CheckBox EditSpec;
        private System.Windows.Forms.Button SaveSpec;
        private System.Windows.Forms.Button SaveAsSpec;
        private System.Windows.Forms.TextBox SpecFileName;
        private System.Windows.Forms.Panel P_AutoLearn;
        private System.Windows.Forms.Panel P_Vision;
        private System.Windows.Forms.Panel P_Manager;
        private System.Windows.Forms.Panel P_Main;
        private System.Windows.Forms.DataGridView ConditinGrid;
        private System.Windows.Forms.Panel P_Motion;
        private System.Windows.Forms.Button Move_Down;
        private System.Windows.Forms.Button Move_Up;
        private System.Windows.Forms.TextBox tbToDoList;
        private System.Windows.Forms.ListBox Actionbox;
        private System.Windows.Forms.ListBox TodoBox;
        private System.Windows.Forms.TextBox tbActionList;
        private System.Windows.Forms.Button RemoveItem;
        private System.Windows.Forms.Button AddItem;
        private System.Windows.Forms.CheckBox EditCondition;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button SaveAsCondition;
        private System.Windows.Forms.Button SaveCondition;
        private System.Windows.Forms.Button OpenCondition;
        private System.Windows.Forms.TextBox RecipeFileName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox TesterNo;
        private System.Windows.Forms.Button SetAFPIDUpdate;
        private System.Windows.Forms.RichTextBox AFPidSetPath;
        private System.Windows.Forms.Button SetOISFWUpdate;
        private System.Windows.Forms.RichTextBox OISFWSetPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lbxModelFiles;
        private System.Windows.Forms.Label lblDefaultModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox MCtypeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMcNum;
        private System.Windows.Forms.DataGridView RetryGrid;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ToVision;
        public System.Windows.Forms.Button ToOperator;
        private System.Windows.Forms.Button SetOISBaseCalUpdate;
        private System.Windows.Forms.RichTextBox OISBaseCalSetPath;
        private System.Windows.Forms.Button btnToMotion;
        private System.Windows.Forms.Button btnBarcodeOff;
        private System.Windows.Forms.Button btnBarcodeOn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView SettleDataGrid;
    }
}

