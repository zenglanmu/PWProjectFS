using PWProjectFS.DokanyFS;
using PWProjectFS.PWProvider;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PWProjectFS.UI
{
    public partial class Main : Form
    {
        private PWDataSourceProvider provider;
        private int PWProjectId;
        private string localMountPath;

        public Main()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.provider = new PWDataSourceProvider();
            this.provider.Initialize();
            this.provider.Login();
        }

        private void btnMount_Click(object sender, EventArgs e)
        {
            if (this.provider != null && this.provider.IsOpen())
            {
                this.PWProjectId = this.provider.ProjectHelper.ShowSelectProjectDlg();
                if (this.PWProjectId >= 0)
                {
                    this.localMountPath = @"N:\";
                    // 放后台不卡主线程
                    this.backgroundMountWorker.RunWorkerAsync();
                }
            }

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            if (this.provider != null)
            {
                this.provider.InvalidateCache();
            }
        }

        private void backgroundMountWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PWFSOperations.Mount(this.PWProjectId, this.localMountPath, this.provider);
        }

        private void btnUnMount_Click(object sender, EventArgs e)
        {

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            var msg = @"北京城建设计发展集团 版权所有
项目主页见https://github.com/zenglanmu/PWProjectFS
如您分发本软件，请保留该信息。";
            MessageBox.Show(msg);
        }
    }
}
