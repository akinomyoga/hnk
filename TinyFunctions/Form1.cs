using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace TinyFunctions{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox textX;
		private System.Windows.Forms.TextBox textY;
		private System.Windows.Forms.TextBox textZ;
		private System.Windows.Forms.Label labelX;
		private System.Windows.Forms.Label labelY;
		private System.Windows.Forms.Label labelZ;
		private System.Windows.Forms.TextBox textOut;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1(){
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.textX = new System.Windows.Forms.TextBox();
			this.textY = new System.Windows.Forms.TextBox();
			this.textZ = new System.Windows.Forms.TextBox();
			this.labelX = new System.Windows.Forms.Label();
			this.labelY = new System.Windows.Forms.Label();
			this.labelZ = new System.Windows.Forms.Label();
			this.textOut = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(240, 220);
			this.listBox1.TabIndex = 0;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// textX
			// 
			this.textX.Location = new System.Drawing.Point(248, 24);
			this.textX.Name = "textX";
			this.textX.Size = new System.Drawing.Size(136, 19);
			this.textX.TabIndex = 1;
			this.textX.Text = "";
			// 
			// textY
			// 
			this.textY.Location = new System.Drawing.Point(248, 72);
			this.textY.Name = "textY";
			this.textY.Size = new System.Drawing.Size(136, 19);
			this.textY.TabIndex = 2;
			this.textY.Text = "";
			// 
			// textZ
			// 
			this.textZ.Location = new System.Drawing.Point(248, 120);
			this.textZ.Name = "textZ";
			this.textZ.Size = new System.Drawing.Size(136, 19);
			this.textZ.TabIndex = 3;
			this.textZ.Text = "";
			// 
			// labelX
			// 
			this.labelX.Location = new System.Drawing.Point(248, 8);
			this.labelX.Name = "labelX";
			this.labelX.Size = new System.Drawing.Size(136, 16);
			this.labelX.TabIndex = 4;
			// 
			// labelY
			// 
			this.labelY.Location = new System.Drawing.Point(248, 56);
			this.labelY.Name = "labelY";
			this.labelY.Size = new System.Drawing.Size(136, 16);
			this.labelY.TabIndex = 5;
			// 
			// labelZ
			// 
			this.labelZ.Location = new System.Drawing.Point(248, 104);
			this.labelZ.Name = "labelZ";
			this.labelZ.Size = new System.Drawing.Size(136, 16);
			this.labelZ.TabIndex = 6;
			// 
			// textOut
			// 
			this.textOut.AcceptsReturn = true;
			this.textOut.AcceptsTab = true;
			this.textOut.Location = new System.Drawing.Point(392, 8);
			this.textOut.Multiline = true;
			this.textOut.Name = "textOut";
			this.textOut.Size = new System.Drawing.Size(264, 208);
			this.textOut.TabIndex = 7;
			this.textOut.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(320, 144);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 8;
			this.button1.Text = "Exec";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(666, 223);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textOut);
			this.Controls.Add(this.labelZ);
			this.Controls.Add(this.labelY);
			this.Controls.Add(this.labelX);
			this.Controls.Add(this.textZ);
			this.Controls.Add(this.textY);
			this.Controls.Add(this.textX);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "TinyFunctions";
			this.Load += new System.EventHandler(this.Form1_Load);
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

		private void Form1_Load(object sender, System.EventArgs e){
			this.listBox1.Items.Add(new Base64toUTF8inv());
			this.listBox1.Items.Add(new Base64toUTF8());
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e){
			if(this.listBox1.SelectedItem is IFunction){
				IFunction f=(IFunction)this.listBox1.SelectedItem;
				this.textX.Enabled="--"!=(this.labelX.Text=f.XTitle);
				this.textY.Enabled="--"!=(this.labelY.Text=f.YTitle);
				this.textZ.Enabled="--"!=(this.labelZ.Text=f.ZTitle);
			}
		}

		private void button1_Click(object sender, System.EventArgs e){
			if(this.listBox1.SelectedItems.Count>0
				&&this.listBox1.SelectedItem is IFunction){
				IFunction f=(IFunction)this.listBox1.SelectedItem;
				object result=f.Execute(this.textX.Text,this.textY.Text,this.textZ.Text);
				this.textOut.Text=result.ToString();
			}
		}
	}
	[FunctionAttribute("Base64→binary--(endian 反転)→UTF-8",X="base64 の文字列")]
	public class Base64toUTF8inv:FunctionA{
		public Base64toUTF8inv(){}
		public override object Execute(string x,string y,string z){
			byte[] data=null;
			try{
				data=System.Convert.FromBase64String(x);
			}catch(System.Exception e){
				return "[Error: "+e.Message+"]";
			}
			byte b;
			for(int i=0;i<data.Length;i++){
				b=(byte)(data[i]>>7&1);
				b+=(byte)((data[i]>>6&1)<<1);
				b+=(byte)((data[i]>>5&1)<<2);
				b+=(byte)((data[i]>>4&1)<<3);
				b+=(byte)((data[i]>>3&1)<<4);
				b+=(byte)((data[i]>>2&1)<<5);
				b+=(byte)((data[i]>>1&1)<<6);
				b+=(byte)((data[i]&1)<<7);
				data[i]=b;
			}
			return System.Text.Encoding.UTF8.GetString(data);
		}
	}
	[FunctionAttribute("Base64→UTF-8",X="base64 の文字列")]
	public class Base64toUTF8:FunctionA{
		public Base64toUTF8(){}
		public override object Execute(string x,string y,string z){
			byte[] data=null;
			try{
				data=System.Convert.FromBase64String(x);
			}catch(System.Exception e){
				return "[Error: "+e.Message+"]";
			}
			return System.Text.Encoding.UTF8.GetString(data);
		}
	}
}
