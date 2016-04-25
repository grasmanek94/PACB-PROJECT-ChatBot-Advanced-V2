namespace Worker
{
    partial class worker_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(worker_frm));
            this.btn_q_wzj = new System.Windows.Forms.Button();
            this.btn_q_wwj = new System.Windows.Forms.Button();
            this.btn_q_bjm = new System.Windows.Forms.Button();
            this.btn_a_iz = new System.Windows.Forms.Button();
            this.btn_a_ngnfns = new System.Windows.Forms.Button();
            this.btn_d_chat = new System.Windows.Forms.Button();
            this.btn_d_sstr = new System.Windows.Forms.Button();
            this.btn_d_scht = new System.Windows.Forms.Button();
            this.btn_q_lft = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_q_wzj
            // 
            this.btn_q_wzj.Location = new System.Drawing.Point(1, 88);
            this.btn_q_wzj.Name = "btn_q_wzj";
            this.btn_q_wzj.Size = new System.Drawing.Size(82, 23);
            this.btn_q_wzj.TabIndex = 0;
            this.btn_q_wzj.Text = "Wat zoek je?";
            this.btn_q_wzj.UseVisualStyleBackColor = true;
            this.btn_q_wzj.Click += new System.EventHandler(this.btn_q_wzj_Click);
            // 
            // btn_q_wwj
            // 
            this.btn_q_wwj.Location = new System.Drawing.Point(1, 59);
            this.btn_q_wwj.Name = "btn_q_wwj";
            this.btn_q_wwj.Size = new System.Drawing.Size(82, 23);
            this.btn_q_wwj.TabIndex = 1;
            this.btn_q_wwj.Text = "Woonplaats?";
            this.btn_q_wwj.UseVisualStyleBackColor = true;
            this.btn_q_wwj.Click += new System.EventHandler(this.btn_q_wwj_Click);
            // 
            // btn_q_bjm
            // 
            this.btn_q_bjm.Location = new System.Drawing.Point(1, 1);
            this.btn_q_bjm.Name = "btn_q_bjm";
            this.btn_q_bjm.Size = new System.Drawing.Size(82, 23);
            this.btn_q_bjm.TabIndex = 2;
            this.btn_q_bjm.Text = "Meisje?";
            this.btn_q_bjm.UseVisualStyleBackColor = true;
            this.btn_q_bjm.Click += new System.EventHandler(this.btn_q_bjm_Click);
            // 
            // btn_a_iz
            // 
            this.btn_a_iz.Location = new System.Drawing.Point(89, 1);
            this.btn_a_iz.Name = "btn_a_iz";
            this.btn_a_iz.Size = new System.Drawing.Size(69, 52);
            this.btn_a_iz.TabIndex = 3;
            this.btn_a_iz.Text = "Ik zoek...";
            this.btn_a_iz.UseVisualStyleBackColor = true;
            this.btn_a_iz.Click += new System.EventHandler(this.btn_a_iz_Click);
            // 
            // btn_a_ngnfns
            // 
            this.btn_a_ngnfns.Location = new System.Drawing.Point(89, 59);
            this.btn_a_ngnfns.Name = "btn_a_ngnfns";
            this.btn_a_ngnfns.Size = new System.Drawing.Size(69, 52);
            this.btn_a_ngnfns.TabIndex = 4;
            this.btn_a_ngnfns.Text = "NGNFNS...";
            this.btn_a_ngnfns.UseVisualStyleBackColor = true;
            this.btn_a_ngnfns.Click += new System.EventHandler(this.btn_a_ngnfns_Click);
            // 
            // btn_d_chat
            // 
            this.btn_d_chat.Location = new System.Drawing.Point(164, 1);
            this.btn_d_chat.Name = "btn_d_chat";
            this.btn_d_chat.Size = new System.Drawing.Size(82, 52);
            this.btn_d_chat.TabIndex = 5;
            this.btn_d_chat.Text = "Doorchatten";
            this.btn_d_chat.UseVisualStyleBackColor = true;
            this.btn_d_chat.Click += new System.EventHandler(this.btn_d_chat_Click);
            // 
            // btn_d_sstr
            // 
            this.btn_d_sstr.Location = new System.Drawing.Point(164, 59);
            this.btn_d_sstr.Name = "btn_d_sstr";
            this.btn_d_sstr.Size = new System.Drawing.Size(82, 52);
            this.btn_d_sstr.TabIndex = 6;
            this.btn_d_sstr.Text = "Stop verhaal";
            this.btn_d_sstr.UseVisualStyleBackColor = true;
            this.btn_d_sstr.Click += new System.EventHandler(this.btn_d_sstr_Click);
            // 
            // btn_d_scht
            // 
            this.btn_d_scht.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_d_scht.Location = new System.Drawing.Point(1058, 1);
            this.btn_d_scht.Name = "btn_d_scht";
            this.btn_d_scht.Size = new System.Drawing.Size(62, 23);
            this.btn_d_scht.TabIndex = 7;
            this.btn_d_scht.Text = "Stop chat";
            this.btn_d_scht.UseVisualStyleBackColor = true;
            this.btn_d_scht.Click += new System.EventHandler(this.btn_d_scht_Click);
            // 
            // btn_q_lft
            // 
            this.btn_q_lft.Location = new System.Drawing.Point(1, 30);
            this.btn_q_lft.Name = "btn_q_lft";
            this.btn_q_lft.Size = new System.Drawing.Size(82, 23);
            this.btn_q_lft.TabIndex = 8;
            this.btn_q_lft.Text = "Leeftijd?";
            this.btn_q_lft.UseVisualStyleBackColor = true;
            this.btn_q_lft.Click += new System.EventHandler(this.btn_q_lft_Click);
            // 
            // worker_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1161, 655);
            this.Controls.Add(this.btn_q_lft);
            this.Controls.Add(this.btn_d_scht);
            this.Controls.Add(this.btn_d_sstr);
            this.Controls.Add(this.btn_d_chat);
            this.Controls.Add(this.btn_a_ngnfns);
            this.Controls.Add(this.btn_a_iz);
            this.Controls.Add(this.btn_q_bjm);
            this.Controls.Add(this.btn_q_wwj);
            this.Controls.Add(this.btn_q_wzj);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "worker_frm";
            this.Text = "Worker";
            this.Load += new System.EventHandler(this.worker_frm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_q_wzj;
        private System.Windows.Forms.Button btn_q_wwj;
        private System.Windows.Forms.Button btn_q_bjm;
        private System.Windows.Forms.Button btn_a_iz;
        private System.Windows.Forms.Button btn_a_ngnfns;
        private System.Windows.Forms.Button btn_d_chat;
        private System.Windows.Forms.Button btn_d_sstr;
        private System.Windows.Forms.Button btn_d_scht;
        private System.Windows.Forms.Button btn_q_lft;
    }
}

