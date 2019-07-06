using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace smtpMail
{
	/// <summary>
	/// Form1 �̊T�v�̐����ł��B
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox Subject;
		private System.Windows.Forms.ComboBox Address;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.TextBox Body;
		private System.Windows.Forms.ComboBox From;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1(){
			InitializeComponent();
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			this.Subject = new System.Windows.Forms.TextBox();
			this.Address = new System.Windows.Forms.ComboBox();
			this.Body = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.From = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// Subject
			// 
			this.Subject.Location = new System.Drawing.Point(0, 48);
			this.Subject.Name = "Subject";
			this.Subject.Size = new System.Drawing.Size(512, 19);
			this.Subject.TabIndex = 0;
			this.Subject.Text = "�薼";
			// 
			// Address
			// 
			this.Address.Items.AddRange(new object[] {
														 "pi314e2718@yahoo.co.jp",
														 "murase99ko@hotmail.co.jp",
														 "kch.murase@docomo.ne.jp"});
			this.Address.Location = new System.Drawing.Point(0, 24);
			this.Address.Name = "Address";
			this.Address.Size = new System.Drawing.Size(512, 20);
			this.Address.TabIndex = 1;
			this.Address.Text = "pi314e2718@yahoo.co.jp";
			// 
			// Body
			// 
			this.Body.Location = new System.Drawing.Point(0, 72);
			this.Body.Multiline = true;
			this.Body.Name = "Body";
			this.Body.Size = new System.Drawing.Size(512, 248);
			this.Body.TabIndex = 2;
			this.Body.Text = "���e";
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(440, 320);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(72, 24);
			this.btnSend.TabIndex = 4;
			this.btnSend.Text = "���M";
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// From
			// 
			this.From.Items.AddRange(new object[] {
													  "pi314e2718@yahoo.co.jp",
													  "murase99ko@hotmail.co.jp",
													  "kch.murase@docomo.ne.jp",
													  "MURAOSLLnZBcUY3I1sH00000002@mura"});
			this.From.Location = new System.Drawing.Point(0, 0);
			this.From.Name = "From";
			this.From.Size = new System.Drawing.Size(512, 20);
			this.From.TabIndex = 5;
			this.From.Text = "pi314e2718@yahoo.co.jp";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(512, 349);
			this.Controls.Add(this.From);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.Body);
			this.Controls.Add(this.Address);
			this.Controls.Add(this.Subject);
			this.Name = "Form1";
			this.Text = "SMTP ���M����";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
		/// </summary>
		[STAThread]
		static void Main(){
			Application.Run(new Form1());
		}

		private void btnSend_Click(object sender, System.EventArgs e){
			//System.Web.Mail.SmtpMail.SmtpServer="localhost";
			System.Web.Mail.MailMessage mm=new System.Web.Mail.MailMessage();
			mm.From=this.From.Text;
			mm.To=this.Address.Text;
			mm.Subject=(this.Subject.Text=="")?"����":this.Subject.Text;
			mm.Body=this.Body.Text;
			mm.BodyEncoding=System.Text.Encoding.GetEncoding("iso-2022-jp");
			System.Web.Mail.SmtpMail.Send(mm);
		}
	}
}
