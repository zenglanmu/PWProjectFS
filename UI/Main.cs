using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PWProjectFS.PWProvider;
using PWProjectFS.DokanyFS;
using DokanNet.Logging;

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
            if(this.provider !=null && this.provider.IsOpen())
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
    }
}
