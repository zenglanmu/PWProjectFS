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
        public Main()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            provider = new PWDataSourceProvider();
            provider.Initialize();
            provider.Login();            
        }

        private void btnMount_Click(object sender, EventArgs e)
        {
            if(provider!=null && provider.IsOpen())
            {
                var projectno = provider.ProjectHelper.ShowSelectProjectDlg();
                string mountPath = @"N:\";
                PWFSOperations.Mount(projectno, mountPath, provider);
                    
            }
            
        }
    }
}
