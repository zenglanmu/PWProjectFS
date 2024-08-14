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

namespace PWProjectFS.UI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var provider = new PWDataSourceProvider();
            provider.Initialize();
            provider.Login();
            var projectno = provider.ProjectHelper.SelectProject();
            provider.ProjectHelper.GetStorageInfo(projectno);
        }
    }
}
