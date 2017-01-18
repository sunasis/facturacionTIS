namespace FacturacionElectronica.UI
{
    partial class frmTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnVoided = new System.Windows.Forms.Button();
            this.btnResumen = new System.Windows.Forms.Button();
            this.txtTicket = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtCert = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnCredit = new System.Windows.Forms.Button();
            this.btnDebit = new System.Windows.Forms.Button();
            this.btnXML = new System.Windows.Forms.Button();
            this.cboTipoService = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRetencion = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCeStatus = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCeBaja = new System.Windows.Forms.Button();
            this.txtTicketCre = new System.Windows.Forms.TextBox();
            this.cboServiceRet = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generar y Enviar Invoice";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnVoided
            // 
            this.btnVoided.Location = new System.Drawing.Point(12, 41);
            this.btnVoided.Name = "btnVoided";
            this.btnVoided.Size = new System.Drawing.Size(144, 23);
            this.btnVoided.TabIndex = 3;
            this.btnVoided.Text = "Dar de Baja";
            this.btnVoided.UseVisualStyleBackColor = true;
            this.btnVoided.Click += new System.EventHandler(this.btnVoided_Click);
            // 
            // btnResumen
            // 
            this.btnResumen.Location = new System.Drawing.Point(12, 70);
            this.btnResumen.Name = "btnResumen";
            this.btnResumen.Size = new System.Drawing.Size(144, 23);
            this.btnResumen.TabIndex = 3;
            this.btnResumen.Text = "Resumen Diario";
            this.btnResumen.UseVisualStyleBackColor = true;
            this.btnResumen.Click += new System.EventHandler(this.btnResumen_Click);
            // 
            // txtTicket
            // 
            this.txtTicket.Location = new System.Drawing.Point(12, 122);
            this.txtTicket.Name = "txtTicket";
            this.txtTicket.Size = new System.Drawing.Size(144, 20);
            this.txtTicket.TabIndex = 4;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(58, 148);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(98, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "Enviar Ticket";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtCert
            // 
            this.txtCert.Location = new System.Drawing.Point(12, 177);
            this.txtCert.Name = "txtCert";
            this.txtCert.Size = new System.Drawing.Size(137, 20);
            this.txtCert.TabIndex = 6;
            this.txtCert.Text = "123456";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(87, 203);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 25);
            this.button2.TabIndex = 7;
            this.button2.Text = "file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnCredit
            // 
            this.btnCredit.Location = new System.Drawing.Point(19, 256);
            this.btnCredit.Name = "btnCredit";
            this.btnCredit.Size = new System.Drawing.Size(129, 29);
            this.btnCredit.TabIndex = 8;
            this.btnCredit.Text = "Nota de Credito";
            this.btnCredit.UseVisualStyleBackColor = true;
            this.btnCredit.Click += new System.EventHandler(this.btnCredit_Click);
            // 
            // btnDebit
            // 
            this.btnDebit.Location = new System.Drawing.Point(19, 291);
            this.btnDebit.Name = "btnDebit";
            this.btnDebit.Size = new System.Drawing.Size(129, 29);
            this.btnDebit.TabIndex = 9;
            this.btnDebit.Text = "Nota de Debito";
            this.btnDebit.UseVisualStyleBackColor = true;
            this.btnDebit.Click += new System.EventHandler(this.btnDebit_Click);
            // 
            // btnXML
            // 
            this.btnXML.Location = new System.Drawing.Point(19, 328);
            this.btnXML.Name = "btnXML";
            this.btnXML.Size = new System.Drawing.Size(129, 28);
            this.btnXML.TabIndex = 10;
            this.btnXML.Text = "signed XML";
            this.btnXML.UseVisualStyleBackColor = true;
            this.btnXML.Click += new System.EventHandler(this.btnXML_Click);
            // 
            // cboTipoService
            // 
            this.cboTipoService.FormattingEnabled = true;
            this.cboTipoService.Items.AddRange(new object[] {
            "Beta",
            "Produccion",
            "Homologacion"});
            this.cboTipoService.Location = new System.Drawing.Point(20, 385);
            this.cboTipoService.Name = "cboTipoService";
            this.cboTipoService.Size = new System.Drawing.Size(129, 21);
            this.cboTipoService.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 369);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Tipo de Servicio";
            // 
            // btnRetencion
            // 
            this.btnRetencion.Location = new System.Drawing.Point(23, 29);
            this.btnRetencion.Name = "btnRetencion";
            this.btnRetencion.Size = new System.Drawing.Size(152, 26);
            this.btnRetencion.TabIndex = 13;
            this.btnRetencion.Text = "Enviar";
            this.btnRetencion.UseVisualStyleBackColor = true;
            this.btnRetencion.Click += new System.EventHandler(this.btnRetencion_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCeStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnCeBaja);
            this.groupBox1.Controls.Add(this.btnRetencion);
            this.groupBox1.Controls.Add(this.txtTicketCre);
            this.groupBox1.Controls.Add(this.cboServiceRet);
            this.groupBox1.Location = new System.Drawing.Point(162, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 205);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Retencion y Percepcion";
            // 
            // btnCeStatus
            // 
            this.btnCeStatus.Location = new System.Drawing.Point(23, 119);
            this.btnCeStatus.Name = "btnCeStatus";
            this.btnCeStatus.Size = new System.Drawing.Size(152, 26);
            this.btnCeStatus.TabIndex = 13;
            this.btnCeStatus.Text = "Enviar Ticket";
            this.btnCeStatus.UseVisualStyleBackColor = true;
            this.btnCeStatus.Click += new System.EventHandler(this.btnCeStatus_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Tipo de Servicio";
            // 
            // btnCeBaja
            // 
            this.btnCeBaja.Location = new System.Drawing.Point(23, 61);
            this.btnCeBaja.Name = "btnCeBaja";
            this.btnCeBaja.Size = new System.Drawing.Size(152, 26);
            this.btnCeBaja.TabIndex = 13;
            this.btnCeBaja.Text = "Dar De Baja";
            this.btnCeBaja.UseVisualStyleBackColor = true;
            this.btnCeBaja.Click += new System.EventHandler(this.btnCeBaja_Click);
            // 
            // txtTicketCre
            // 
            this.txtTicketCre.Location = new System.Drawing.Point(23, 93);
            this.txtTicketCre.Name = "txtTicketCre";
            this.txtTicketCre.Size = new System.Drawing.Size(152, 20);
            this.txtTicketCre.TabIndex = 4;
            // 
            // cboServiceRet
            // 
            this.cboServiceRet.FormattingEnabled = true;
            this.cboServiceRet.Items.AddRange(new object[] {
            "Beta",
            "Produccion"});
            this.cboServiceRet.Location = new System.Drawing.Point(23, 170);
            this.cboServiceRet.Name = "cboServiceRet";
            this.cboServiceRet.Size = new System.Drawing.Size(129, 21);
            this.cboServiceRet.TabIndex = 11;
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 418);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnXML);
            this.Controls.Add(this.btnDebit);
            this.Controls.Add(this.cboTipoService);
            this.Controls.Add(this.btnCredit);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtCert);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtTicket);
            this.Controls.Add(this.btnResumen);
            this.Controls.Add(this.btnVoided);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmTest";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnVoided;
        private System.Windows.Forms.Button btnResumen;
        private System.Windows.Forms.TextBox txtTicket;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtCert;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnCredit;
        private System.Windows.Forms.Button btnDebit;
        private System.Windows.Forms.Button btnXML;
        private System.Windows.Forms.ComboBox cboTipoService;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRetencion;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCeStatus;
        private System.Windows.Forms.Button btnCeBaja;
        private System.Windows.Forms.TextBox txtTicketCre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboServiceRet;
    }
}

