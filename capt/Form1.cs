using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Interop=System.Runtime.InteropServices;

namespace capt{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		System.Drawing.Graphics gr;
		private System.Windows.Forms.Button button3;
		private string savepath;

		public Form1(){
			InitializeComponent();

			System.Drawing.Bitmap bmp=new Bitmap(this.pictureBox1.Size.Width,this.pictureBox1.Size.Height);
			this.gr=System.Drawing.Graphics.FromImage(bmp);
			this.pictureBox1.Image=bmp;
			this.savepath=System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			this.savepath=System.IO.Path.Combine(this.savepath,"GAMEN.jpg");
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ){
			if(disposing){
				if(components!=null)components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 0;
			this.button1.Text = "保存";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Location = new System.Drawing.Point(80, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(160, 120);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point(8, 40);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(64, 24);
			this.button3.TabIndex = 3;
			this.button3.Text = "編集";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// Form1
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(250, 135);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "画面保存";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e){
			// ［Alt］キー＋［Print Screen］キーの送信
			System.Windows.Forms.SendKeys.SendWait("^{PRTSC}");
			// クリップボードに格納された画像の取得
			IDataObject data = Clipboard.GetDataObject();
			if(data.GetDataPresent(DataFormats.Bitmap)){
				Bitmap bmp = (Bitmap)data.GetData(DataFormats.Bitmap);
				this.Thumb=bmp;
				this.Save2Desktop(this.Clip(bmp));
				this.button3.Enabled=true;
			}
		}

		private System.Drawing.Image Thumb{
			set{
				System.Drawing.Bitmap bmp=new Bitmap(value,this.pictureBox1.Size);
				this.pictureBox1.BackgroundImage=bmp;
			}
			get{
				return this.pictureBox1.BackgroundImage;
			}
		}
		private void Save2Desktop(System.Drawing.Bitmap bmp){
			bmp.Save(this.savepath,System.Drawing.Imaging.ImageFormat.Jpeg);
		}

		private System.Drawing.Bitmap Clip(System.Drawing.Bitmap bmp){
			if(!this.isRect)return bmp;
			double xrate=bmp.Width/(double)this.pictureBox1.Width;
			double yrate=bmp.Height/(double)this.pictureBox1.Height;
			//System.Windows.Forms.MessageBox.Show(yrate.ToString());
			System.Drawing.Bitmap r=new Bitmap(
				(int)(xrate*this.rect0.Width),
				(int)(yrate*this.rect0.Height)
			);
			System.Drawing.Graphics.FromImage(r).DrawImageUnscaled(bmp,
				-(int)(xrate*this.rect0.Left),
				-(int)(yrate*this.rect0.Top)
			);
			return r;
		}

		#region 矩形の設定
		private System.Drawing.Point pt0;
		private System.Drawing.Rectangle rect0;
		private bool isRect=false;
		private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e){
			if(e.Button!=System.Windows.Forms.MouseButtons.Left)return;
			this.pt0=new Point(e.X,e.Y);
			this.gr.Clear(System.Drawing.Color.Transparent);
			this.gr.DrawLine(System.Drawing.Pens.Magenta,e.X-3,e.Y,e.X+3,e.Y);
			this.gr.DrawLine(System.Drawing.Pens.Magenta,e.X,e.Y-3,e.X,e.Y+3);
			this.pictureBox1.Refresh();
		}
		private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			if(e.Button!=System.Windows.Forms.MouseButtons.Left)return;
			//矩形
			int x=pt0.X;int y=pt0.Y;
			int w=e.X;int h=e.Y;
			//--大小
			if(x==w||y==h){this.ClearRect();return;}
			if(w<x){x=e.X;w=pt0.X;}
			if(h<y){y=e.Y;h=pt0.Y;}
			//--over-flow
			if(x<0)x=0;
			if(y<0)y=0;
			if(w>this.pictureBox1.Width)w=this.pictureBox1.Width;
			if(h>this.pictureBox1.Height)h=this.pictureBox1.Height;
			//--create
			this.rect0=new Rectangle(x,y,w-x+1,h-y+1);
			//表示
			this.gr.Clear(System.Drawing.Color.Transparent);
			this.gr.DrawRectangle(System.Drawing.Pens.Magenta,this.rect0);
			this.pictureBox1.Refresh();
			//
			this.isRect=true;
		}
		private void ClearRect(){
			this.isRect=false;
			this.gr.Clear(System.Drawing.Color.Transparent);
			this.pictureBox1.Refresh();
		}
		private void button2_Click(object sender, System.EventArgs e){
			this.ClearRect();
		}
		#endregion

		private void Edit(){
			if(!System.IO.File.Exists(this.savepath)){
				this.button3.Enabled=false;
				return;
			}
			System.Diagnostics.Process p=System.Diagnostics.Process.Start("mspaint","\""+this.savepath+"\"");
		}

		private void button3_Click(object sender, System.EventArgs e){
			this.Edit();
		}
	}
}
