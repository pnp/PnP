namespace Contoso.Provisioning.Services.SiteManager.FormTester
{
    partial class frmMain
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtWebApplicationUrl = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnCreateSiteCollection = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.txtSiteSecondaryAccount = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtSiteOwnerAccount = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtSiteTemplate = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtSiteLanguageId = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtSiteDescription = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtSiteTitle = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSiteUrl = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnCTCreate = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtContentTypeName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtContentTypeId = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnGetSiteCollections = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.btnGetPolicy = new System.Windows.Forms.Button();
            this.btnPolicySet = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPolicyManifest = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPolicyContentTypeID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPolicySiteCol = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLocaleSiteCollection = new System.Windows.Forms.TextBox();
            this.btnLocaleSet = new System.Windows.Forms.Button();
            this.txtLocaleSetString = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(643, 480);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.txtAccount);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtDomain);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtWebApplicationUrl);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(643, 107);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection info";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(260, 48);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(47, 13);
            this.label30.TabIndex = 10;
            this.label30.Text = "Account";
            // 
            // txtAccount
            // 
            this.txtAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAccount.Location = new System.Drawing.Point(340, 45);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(148, 20);
            this.txtAccount.TabIndex = 9;
            this.txtAccount.Text = "administrator";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(86, 71);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(535, 20);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.Text = "pass@word1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(43, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "Domain";
            // 
            // txtDomain
            // 
            this.txtDomain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDomain.Location = new System.Drawing.Point(86, 45);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(148, 20);
            this.txtDomain.TabIndex = 5;
            this.txtDomain.Text = "contoso";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(20, 13);
            this.label13.TabIndex = 4;
            this.label13.Text = "Url";
            // 
            // txtWebApplicationUrl
            // 
            this.txtWebApplicationUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWebApplicationUrl.Location = new System.Drawing.Point(86, 19);
            this.txtWebApplicationUrl.Name = "txtWebApplicationUrl";
            this.txtWebApplicationUrl.Size = new System.Drawing.Size(535, 20);
            this.txtWebApplicationUrl.TabIndex = 0;
            this.txtWebApplicationUrl.Text = "http://dev.contoso.com";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(643, 369);
            this.splitContainer2.SplitterDistance = 272;
            this.splitContainer2.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(643, 272);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCreateSiteCollection);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.txtSiteSecondaryAccount);
            this.tabPage1.Controls.Add(this.label22);
            this.tabPage1.Controls.Add(this.txtSiteOwnerAccount);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.txtSiteTemplate);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.txtSiteLanguageId);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.txtSiteDescription);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.txtSiteTitle);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.txtSiteUrl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(635, 246);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Create Site Collection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCreateSiteCollection
            // 
            this.btnCreateSiteCollection.Location = new System.Drawing.Point(322, 23);
            this.btnCreateSiteCollection.Name = "btnCreateSiteCollection";
            this.btnCreateSiteCollection.Size = new System.Drawing.Size(173, 23);
            this.btnCreateSiteCollection.TabIndex = 36;
            this.btnCreateSiteCollection.Text = "Create Site Collection";
            this.btnCreateSiteCollection.UseVisualStyleBackColor = true;
            this.btnCreateSiteCollection.Click += new System.EventHandler(this.btnCreateSiteCollection_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(7, 184);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(101, 13);
            this.label24.TabIndex = 25;
            this.label24.Text = "Secondary Account";
            // 
            // txtSiteSecondaryAccount
            // 
            this.txtSiteSecondaryAccount.Location = new System.Drawing.Point(110, 181);
            this.txtSiteSecondaryAccount.Name = "txtSiteSecondaryAccount";
            this.txtSiteSecondaryAccount.Size = new System.Drawing.Size(173, 20);
            this.txtSiteSecondaryAccount.TabIndex = 24;
            this.txtSiteSecondaryAccount.Text = "contoso\\vesaj";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 158);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(81, 13);
            this.label22.TabIndex = 21;
            this.label22.Text = "Onwer Account";
            // 
            // txtSiteOwnerAccount
            // 
            this.txtSiteOwnerAccount.Location = new System.Drawing.Point(110, 155);
            this.txtSiteOwnerAccount.Name = "txtSiteOwnerAccount";
            this.txtSiteOwnerAccount.Size = new System.Drawing.Size(173, 20);
            this.txtSiteOwnerAccount.TabIndex = 20;
            this.txtSiteOwnerAccount.Text = "contoso\\administrator";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 132);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(51, 13);
            this.label21.TabIndex = 19;
            this.label21.Text = "Template";
            // 
            // txtSiteTemplate
            // 
            this.txtSiteTemplate.Location = new System.Drawing.Point(110, 129);
            this.txtSiteTemplate.Name = "txtSiteTemplate";
            this.txtSiteTemplate.Size = new System.Drawing.Size(173, 20);
            this.txtSiteTemplate.TabIndex = 18;
            this.txtSiteTemplate.Text = "sts#0";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 106);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 13);
            this.label20.TabIndex = 17;
            this.label20.Text = "Language ID";
            // 
            // txtSiteLanguageId
            // 
            this.txtSiteLanguageId.Location = new System.Drawing.Point(110, 103);
            this.txtSiteLanguageId.Name = "txtSiteLanguageId";
            this.txtSiteLanguageId.Size = new System.Drawing.Size(173, 20);
            this.txtSiteLanguageId.TabIndex = 16;
            this.txtSiteLanguageId.Text = "1033";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 80);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(60, 13);
            this.label19.TabIndex = 15;
            this.label19.Text = "Description";
            // 
            // txtSiteDescription
            // 
            this.txtSiteDescription.Location = new System.Drawing.Point(110, 77);
            this.txtSiteDescription.Name = "txtSiteDescription";
            this.txtSiteDescription.Size = new System.Drawing.Size(173, 20);
            this.txtSiteDescription.TabIndex = 14;
            this.txtSiteDescription.Text = "Sample Site Collection";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 54);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(27, 13);
            this.label18.TabIndex = 13;
            this.label18.Text = "Title";
            // 
            // txtSiteTitle
            // 
            this.txtSiteTitle.Location = new System.Drawing.Point(110, 51);
            this.txtSiteTitle.Name = "txtSiteTitle";
            this.txtSiteTitle.Size = new System.Drawing.Size(173, 20);
            this.txtSiteTitle.TabIndex = 12;
            this.txtSiteTitle.Text = "Sample";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(74, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Site Collection";
            // 
            // txtSiteUrl
            // 
            this.txtSiteUrl.Location = new System.Drawing.Point(110, 25);
            this.txtSiteUrl.Name = "txtSiteUrl";
            this.txtSiteUrl.Size = new System.Drawing.Size(173, 20);
            this.txtSiteUrl.TabIndex = 8;
            this.txtSiteUrl.Text = "/sites/test";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnCTCreate);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.txtContentTypeName);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.txtContentTypeId);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(635, 246);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Content Type";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnCTCreate
            // 
            this.btnCTCreate.Location = new System.Drawing.Point(114, 74);
            this.btnCTCreate.Name = "btnCTCreate";
            this.btnCTCreate.Size = new System.Drawing.Size(123, 23);
            this.btnCTCreate.TabIndex = 12;
            this.btnCTCreate.Text = "Create CT";
            this.btnCTCreate.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Name";
            // 
            // txtContentTypeName
            // 
            this.txtContentTypeName.Location = new System.Drawing.Point(114, 48);
            this.txtContentTypeName.Name = "txtContentTypeName";
            this.txtContentTypeName.Size = new System.Drawing.Size(173, 20);
            this.txtContentTypeName.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Content Type ID";
            // 
            // txtContentTypeId
            // 
            this.txtContentTypeId.Location = new System.Drawing.Point(114, 22);
            this.txtContentTypeId.Name = "txtContentTypeId";
            this.txtContentTypeId.Size = new System.Drawing.Size(173, 20);
            this.txtContentTypeId.TabIndex = 8;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnGetSiteCollections);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(635, 246);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Enum site collections";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnGetSiteCollections
            // 
            this.btnGetSiteCollections.Location = new System.Drawing.Point(116, 74);
            this.btnGetSiteCollections.Name = "btnGetSiteCollections";
            this.btnGetSiteCollections.Size = new System.Drawing.Size(123, 23);
            this.btnGetSiteCollections.TabIndex = 12;
            this.btnGetSiteCollections.Text = "Get";
            this.btnGetSiteCollections.UseVisualStyleBackColor = true;
            this.btnGetSiteCollections.Click += new System.EventHandler(this.btnGetSiteCollections_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.btnGetPolicy);
            this.tabPage5.Controls.Add(this.btnPolicySet);
            this.tabPage5.Controls.Add(this.label8);
            this.tabPage5.Controls.Add(this.txtPolicyManifest);
            this.tabPage5.Controls.Add(this.label7);
            this.tabPage5.Controls.Add(this.txtPolicyContentTypeID);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Controls.Add(this.txtPolicySiteCol);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(635, 246);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Information Policy";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // btnGetPolicy
            // 
            this.btnGetPolicy.Location = new System.Drawing.Point(237, 209);
            this.btnGetPolicy.Name = "btnGetPolicy";
            this.btnGetPolicy.Size = new System.Drawing.Size(123, 23);
            this.btnGetPolicy.TabIndex = 9;
            this.btnGetPolicy.Text = "Get policy";
            this.btnGetPolicy.UseVisualStyleBackColor = true;
            // 
            // btnPolicySet
            // 
            this.btnPolicySet.Location = new System.Drawing.Point(108, 209);
            this.btnPolicySet.Name = "btnPolicySet";
            this.btnPolicySet.Size = new System.Drawing.Size(123, 23);
            this.btnPolicySet.TabIndex = 8;
            this.btnPolicySet.Text = "Set policy";
            this.btnPolicySet.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Action manifest";
            // 
            // txtPolicyManifest
            // 
            this.txtPolicyManifest.Location = new System.Drawing.Point(108, 83);
            this.txtPolicyManifest.Multiline = true;
            this.txtPolicyManifest.Name = "txtPolicyManifest";
            this.txtPolicyManifest.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPolicyManifest.Size = new System.Drawing.Size(518, 120);
            this.txtPolicyManifest.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Content Type ID";
            // 
            // txtPolicyContentTypeID
            // 
            this.txtPolicyContentTypeID.Location = new System.Drawing.Point(108, 57);
            this.txtPolicyContentTypeID.Name = "txtPolicyContentTypeID";
            this.txtPolicyContentTypeID.Size = new System.Drawing.Size(173, 20);
            this.txtPolicyContentTypeID.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Site Collection";
            // 
            // txtPolicySiteCol
            // 
            this.txtPolicySiteCol.Location = new System.Drawing.Point(108, 31);
            this.txtPolicySiteCol.Name = "txtPolicySiteCol";
            this.txtPolicySiteCol.Size = new System.Drawing.Size(173, 20);
            this.txtPolicySiteCol.TabIndex = 2;
            this.txtPolicySiteCol.Text = "http://intranet.contoso.com";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtStatus);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(643, 93);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(6, 19);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(625, 62);
            this.txtStatus.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.txtLocaleSetString);
            this.tabPage4.Controls.Add(this.btnLocaleSet);
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.txtLocaleSiteCollection);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(635, 246);
            this.tabPage4.TabIndex = 5;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Site Collection";
            // 
            // txtLocaleSiteCollection
            // 
            this.txtLocaleSiteCollection.Location = new System.Drawing.Point(113, 24);
            this.txtLocaleSiteCollection.Name = "txtLocaleSiteCollection";
            this.txtLocaleSiteCollection.Size = new System.Drawing.Size(173, 20);
            this.txtLocaleSiteCollection.TabIndex = 4;
            this.txtLocaleSiteCollection.Text = "http://intranet.contoso.com";
            // 
            // btnLocaleSet
            // 
            this.btnLocaleSet.Location = new System.Drawing.Point(107, 134);
            this.btnLocaleSet.Name = "btnLocaleSet";
            this.btnLocaleSet.Size = new System.Drawing.Size(123, 23);
            this.btnLocaleSet.TabIndex = 13;
            this.btnLocaleSet.Text = "Change";
            this.btnLocaleSet.UseVisualStyleBackColor = true;
            this.btnLocaleSet.Click += new System.EventHandler(this.btnLocaleSet_Click);
            // 
            // txtLocaleSetString
            // 
            this.txtLocaleSetString.Location = new System.Drawing.Point(113, 50);
            this.txtLocaleSetString.Name = "txtLocaleSetString";
            this.txtLocaleSetString.Size = new System.Drawing.Size(173, 20);
            this.txtLocaleSetString.TabIndex = 14;
            this.txtLocaleSetString.Text = "fi-fi";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 480);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmMain";
            this.Text = "WCF Tester App";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtWebApplicationUrl;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCreateSiteCollection;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtSiteSecondaryAccount;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtSiteOwnerAccount;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtSiteTemplate;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtSiteLanguageId;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtSiteDescription;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtSiteTitle;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtSiteUrl;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCTCreate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtContentTypeName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtContentTypeId;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnGetSiteCollections;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnGetPolicy;
        private System.Windows.Forms.Button btnPolicySet;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPolicyManifest;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPolicyContentTypeID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPolicySiteCol;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox txtLocaleSetString;
        private System.Windows.Forms.Button btnLocaleSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLocaleSiteCollection;

    }
}

