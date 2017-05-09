using System;
using System.Linq;
using System.Windows.Forms;
using FacturacionElectronica.Homologacion;
using FacturacionElectronica.Homologacion.Res;

namespace FacturacionElectronica.UI
{
    public partial class MainForm : Form
    {
        SunatManager ws;
        public MainForm()
        {
            InitializeComponent();
            var config = new SolConfig
            {
                Ruc = "20600995805",
                Usuario = "MODDATOS",
                Clave = "moddatos",
                Service = ServiceSunatType.Beta
            };
            ws = new SunatManager(config);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\Users\Giansalex\Dropbox\Facturacion Electronica\ArchivosTest";
            openFileDialog1.Filter = @"XML Files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSendFile.Text = openFileDialog1.FileName;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSendFile.Text))
            {
                var res = ws.SendDocument(txtSendFile.Text);
                if (res.Success)
                {
                    var Response = res.ApplicationResponse;
                    if (Response != null)
                    {
                        MessageBox.Show(string.Format("Code: {0}\n Description: {1}", Response.Codigo, Response.Descripcion), "Exito");
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Errorcode: {0} , \nDescripcion: {1}", res.Error.Code, res.Error.Description), "Error ");
                }
            }
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSendFile.Text))
            {
                var res = ws.SendSummary(txtSendFile.Text);
                if (res.Success)
                {
                    txtTicket.Text = res.Ticket;
                }
                else
                {
                    MessageBox.Show("Error: " + res.Error.Description);
                }
                
            }
        }

        private void btnStatus_Click(object sender, EventArgs e)
        {
            //var resp = ws.getStatusCdr("20600995805", new WebService.Res.ComprobanteEletronico { Tipo = "01", Serie = "F001", Numero = 1 });
            //if (!resp.Success)
            //{
            //    MessageBox.Show(resp.Error.Code + " \n " + resp.Error.Description);
            //}
            if (!string.IsNullOrEmpty(txtTicket.Text))
            {
                var res = ws.GetStatus(txtTicket.Text);
                if (res.Success)
                {
                    MessageBox.Show(res.ApplicationResponse.Descripcion, "exito");
                }
                else
                {
                    MessageBox.Show(res.Error.Description, @"No Available");
                }

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
