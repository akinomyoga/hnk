namespace hnk.memopad{
	using DDE=System.Windows.Forms.DragDropEffects;
	/// <summary>
	/// メインウィンドウのクラスです。
	/// </summary>
	[afh.Configuration.RestoreProperties("NormalLeft>Left","NormalTop>Top","NormalWidth>Width","NormalHeight>Height","WindowState")]
	public class Form1 : System.Windows.Forms.Form{
		[afh.Configuration.RestoreProperties("Width")]
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TextBox textBox1;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1(string path){
			InitializeComponentTrue(new MemoDocument(path));
			(this.treeView1 as MemoTreeView).TextBox=this.textBox1;
		}
		public Form1(){
			InitializeComponentTrue(new MemoDocument());
			(this.treeView1 as MemoTreeView).TextBox=this.textBox1;
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
		private void InitializeComponentTrue(MemoDocument doc){
			this.treeView1 = new MemoTreeView(doc);
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageIndex = -1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(200, 389);
			this.treeView1.TabIndex = 0;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 389);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.AcceptsTab = true;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.textBox1.Location = new System.Drawing.Point(203, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(445, 389);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(648, 389);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.treeView1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
		}
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent(){
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageIndex = -1;
			this.treeView1.LabelEdit = true;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(200, 389);
			this.treeView1.TabIndex = 0;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 389);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.AcceptsTab = true;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.textBox1.Location = new System.Drawing.Point(203, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(445, 389);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(648, 389);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.treeView1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[System.STAThread]
		static void Main(string[] args){
			System.Windows.Forms.Application.Run(args.Length>0?new hnk.memopad.Form1(args[0]):new hnk.memopad.Form1());
		}
		//===========================================================
		//
		//===========================================================
		protected override void OnLoad(System.EventArgs e){
			afh.Configuration.RestorePropertiesAttribute.Restore(this);
			base.OnLoad(e);
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e){
			this.treeView1.Dispose();
			afh.Configuration.RestorePropertiesAttribute.Save(this);
			base.OnClosing(e);
		}


		private int cConsole=0;
		private void write(string str){
			System.Console.Write(str);
			if(++cConsole>20){
				cConsole=0;
				System.Console.Write("\r\n");
			}
		}
		//[System.Runtime.InteropServices.DllImport("user32.dll")] 
		//private static extern System.IntPtr SendMessage (System.IntPtr hWnd , int msg , System.IntPtr wp, System.IntPtr lp); 
		//=========================================================
		//		大きさの記憶
		//=========================================================
		private int normal_t=-1;
		private int normal_l=-1;
		private int normal_w=-1;
		private int normal_h=-1;
		public int NormalTop{get{return this.normal_t<0?this.Top:this.normal_t;}}
		public int NormalLeft{get{return this.normal_l<0?this.Left:this.normal_l;}}
		public int NormalWidth{get{return this.normal_w<0?this.DefaultSize.Width:this.normal_w;}}
		public int NormalHeight{get{return this.normal_h<0?this.DefaultSize.Height:this.normal_h;}}
		protected override void OnSizeChanged(System.EventArgs e){
			if(this.WindowState==System.Windows.Forms.FormWindowState.Normal){
				this.normal_w=this.Width;
				this.normal_h=this.Height;
			}
			base.OnSizeChanged (e);
		}
		protected override void OnLocationChanged(System.EventArgs e){
			if(this.WindowState==System.Windows.Forms.FormWindowState.Normal){
				this.normal_l=this.Left;
				this.normal_t=this.Top;
			}
			base.OnLocationChanged (e);
		}
	}
}
