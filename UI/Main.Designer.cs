
namespace PWProjectFS.UI
{
    partial class Main
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
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnMount = new System.Windows.Forms.Button();
            this.btnClearCache = new System.Windows.Forms.Button();
            this.backgroundMountWorker = new System.ComponentModel.BackgroundWorker();
            this.btnUnMount = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(140, 39);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(216, 49);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "登陆pw";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnMount
            // 
            this.btnMount.Location = new System.Drawing.Point(140, 125);
            this.btnMount.Name = "btnMount";
            this.btnMount.Size = new System.Drawing.Size(216, 49);
            this.btnMount.TabIndex = 1;
            this.btnMount.Text = "挂载pw目录";
            this.btnMount.UseVisualStyleBackColor = true;
            this.btnMount.Click += new System.EventHandler(this.btnMount_Click);
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(140, 312);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(216, 49);
            this.btnClearCache.TabIndex = 2;
            this.btnClearCache.Text = "清空缓存";
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // backgroundMountWorker
            // 
            this.backgroundMountWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundMountWorker_DoWork);
            // 
            // btnUnMount
            // 
            this.btnUnMount.Location = new System.Drawing.Point(140, 211);
            this.btnUnMount.Name = "btnUnMount";
            this.btnUnMount.Size = new System.Drawing.Size(216, 49);
            this.btnUnMount.TabIndex = 3;
            this.btnUnMount.Text = "卸载目录";
            this.btnUnMount.UseVisualStyleBackColor = true;
            this.btnUnMount.Click += new System.EventHandler(this.btnUnMount_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnUnMount);
            this.Controls.Add(this.btnClearCache);
            this.Controls.Add(this.btnMount);
            this.Controls.Add(this.btnLogin);
            this.Name = "Main";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnMount;
        private System.Windows.Forms.Button btnClearCache;
        private System.ComponentModel.BackgroundWorker backgroundMountWorker;
        private System.Windows.Forms.Button btnUnMount;
    }
}