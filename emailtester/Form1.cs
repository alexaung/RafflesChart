using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace emailtester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetDefault();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Mail To");
                MailAddress to = new MailAddress(txtTo.Text);

                Console.WriteLine("Mail From");
                MailAddress from = new MailAddress(txtFrom.Text);

                MailMessage mail = new MailMessage(from, to);

                Console.WriteLine("Subject");
                mail.Subject = txtSub.Text;

                Console.WriteLine("Your Message");
                mail.Body = txtBody.Text;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = txtHost.Text; //"smtp.gmail.com";
                smtp.Port = int.Parse(txtPort.Text);

                smtp.Credentials = new NetworkCredential(
                    txtDomainUser.Text, txtPass.Text);
                smtp.EnableSsl = chkSsl.Checked;
                Console.WriteLine("Sending email...");
                smtp.Send(mail);
                MessageBox.Show("Successfully Sent");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDefaultValue_Click(object sender, EventArgs e)
        {
            GetDefault();
        }

        private void GetDefault()
        {
           // <smtp from="demoraffle@gmail.com" deliveryMethod="Network">
        //<network enableSsl="true" defaultCredentials="false" host="smtp.gmail.com" port="587" password="Super123!" userName="demoraffle@gmail.com"/>
     // </smtp>

            txtFrom.Text = "demoraffle@gmail.com";
            txtTo.Text = "zarniaung006@gmail.com";
            txtSub.Text = "subj 123";
            txtBody.Text = "congrats! yo been regusterd";
            txtDomainUser.Text = "demoraffle@gmail.com";
            txtPass.Text = "Super123!";
            txtHost.Text = "smtp.gmail.com";
            txtPort.Text = "587";
            chkSsl.Checked = true;
        }
    }
}
