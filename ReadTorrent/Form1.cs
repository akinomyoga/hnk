using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace ReadTorrent{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Label l_type;
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.l_type = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "ファイル(&F)";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "開く(&O)";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.AcceptsTab = true;
			this.textBox1.Location = new System.Drawing.Point(232, 24);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(272, 296);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// treeView1
			// 
			this.treeView1.ImageIndex = -1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(224, 320);
			this.treeView1.TabIndex = 1;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// l_type
			// 
			this.l_type.Location = new System.Drawing.Point(232, 0);
			this.l_type.Name = "l_type";
			this.l_type.Size = new System.Drawing.Size(272, 16);
			this.l_type.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(512, 329);
			this.Controls.Add(this.l_type);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.textBox1);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Form1";
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

		private void menuItem2_Click(object sender, System.EventArgs e){
			this.openFile();
		}
		private void openFile(){
			System.Windows.Forms.DialogResult dr=this.openFileDialog1.ShowDialog();
			if(dr==System.Windows.Forms.DialogResult.OK){
				bencoding b=new torrent(this.openFileDialog1.FileName);
				this.treeView1.Nodes.Clear();
				this.treeView1.Nodes.Add(new XmlElementTreeNode(b.xdoc.DocumentElement));
			}
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e){
			if(this.treeView1.SelectedNode!=null&&this.treeView1.SelectedNode is XmlElementTreeNode){
				XmlElementTreeNode node=(XmlElementTreeNode)this.treeView1.SelectedNode;
				this.l_type.Text=node.elem.GetAttribute("type");
				this.textBox1.Text=node.elem.GetAttribute("value");
			}	
		}
	}
	public class XmlElementTreeNode:System.Windows.Forms.TreeNode{
		public System.Xml.XmlElement elem;
		public XmlElementTreeNode(System.Xml.XmlElement elem):base("<"+elem.Name+">"){
			this.elem=elem;
			foreach(System.Xml.XmlNode node in elem.ChildNodes){
				if(node is System.Xml.XmlElement){
					this.Nodes.Add(new XmlElementTreeNode((System.Xml.XmlElement)node));
				}
			}
		}
	}
	public class bencoding{
		private System.IO.Stream str;
		public System.Xml.XmlDocument xdoc;
		public bencoding(string filename):this(System.IO.File.OpenRead(filename)){}
		public bencoding(System.IO.Stream str){
			this.str=str;
			this.xdoc=new System.Xml.XmlDocument();
			this.xdoc.InnerXml="<?xml version=\"1.0\"?>\n<bencoding>\n</bencoding>";
			this.init_read(str,this.xdoc.DocumentElement);
			str.Close();
		}

		#region init
		private void init_read(System.IO.Stream str,System.Xml.XmlElement elem){
			System.Xml.XmlElement e;
			char letter;
			while(this.ReadChar(str,out letter)){
				switch(letter){
					case 'e':return;
					case 'l':
						e=this.createElement(elem);
						e.SetAttribute("type","list");
						this.init_read(str,e);
						elem.AppendChild(e);
						break;
					case 'd':
						e=this.createElement(elem);
						e.SetAttribute("type","dic");
						this.init_read(str,e);
						elem.AppendChild(e);
						break;
					case 'i':
						this.init_read_int(str,elem);
						break;
					case '1':case '2':case '3':case '4':case '5':
					case '6':case '7':case '8':case '9':
						this.init_read_str(str,letter,elem);
						break;
				}
			}
		}
		/// <summary>
		/// 整数を読み取って elem に追加します。
		/// </summary>
		/// <param name="str">読み取りもと</param>
		/// <param name="elem">追加先の要素</param>
		private void init_read_int(System.IO.Stream str,System.Xml.XmlElement elem){
			char letter;
			string r="";
			while(this.ReadChar(str,out letter)){
				switch(letter){
					case 'e':goto write;
					case '-':
						if(r!="")goto fail;
						r+="-";
						break;
					case '0':r+="0";break;
					case '1':case '2':case '3':case '4':
					case '5':case '6':case '7':case '8':case '9':
						if(r=="0")goto fail;
						r+=letter.ToString();
						break;
					default:goto fail;
				}
			}
		write:
			if(r=="")goto fail;
			System.Xml.XmlElement e=this.createElement(elem);
			e.SetAttribute("type","int");
			e.SetAttribute("value",r);
			elem.AppendChild(e);
			return;
		fail:
			while(letter!='e')if(!this.ReadChar(str,out letter))break;
		}
		/// <summary>
		/// 文字列を読み取って返します
		/// </summary>
		/// <param name="str"></param>
		/// <param name="firstletter"></param>
		/// <returns></returns>
		private void init_read_str(System.IO.Stream str,char firstletter,System.Xml.XmlElement elem){
			string strLen=firstletter.ToString();
			char letter;
			if(!this.ReadChar(str,out letter))goto fail;
			while('0'<=letter&&letter<='9'){
				strLen+=letter.ToString();
				if(!this.ReadChar(str,out letter))goto fail;
			}
			if(letter!=':')
				goto fail;
			int len=int.Parse(strLen);
			byte[] buf;
			if(len==0){
				buf=new byte[]{};
			}else{
				buf=new byte[len];
				str.Read(buf,0,len);
			}
			System.Xml.XmlElement e=this.createElement(elem);
			string type,value;
			this.init_binary2string(buf,elem,out type,out value);
			e.SetAttribute("type",type);
			e.SetAttribute("value",value);
			elem.AppendChild(e);
			return;
		fail:
			return;
		}
		/// <summary>
		/// 文字を読んで letter に設定します
		/// </summary>
		/// <param name="str">読み取りもと</param>
		/// <param name="letter">出力</param>
		/// <returns>ファイルの終端に来て読み取る事が出来なかった場合は false</returns>
		private bool ReadChar(System.IO.Stream str,out char letter){
			int result=str.ReadByte();
			if(result<0){
				letter=' ';
				return false;
			}else{
				letter=(char)result;
				return true;
			}
		}
		private System.Xml.XmlElement createElement(System.Xml.XmlElement parent){
			string tag=parent.GetAttribute("type")=="dic"?(parent.ChildNodes.Count%2==0?"key":"val"):"var";
			System.Xml.XmlElement e=parent.OwnerDocument.CreateElement(tag);
			return e;
		}
		#endregion

		/// <summary>
		/// 背景に従って指定されたバイナリ配列の
		/// 然るべき型と文字列表記を求め、出力します。
		/// </summary>
		/// <param name="data">バイト配列で表されたデータ</param>
		/// <param name="context">背景となる要素。</param>
		protected virtual void init_binary2string(byte[] data,System.Xml.XmlElement context,out string type,out string value){
			type="string";
			value=System.Text.Encoding.ASCII.GetString(data);
		}
	}
	public class torrent:bencoding{
		public torrent(string filename):base(filename){}
		public torrent(System.IO.Stream str):base(str){}
		protected override void init_binary2string(byte[] data, System.Xml.XmlElement context, out string type, out string value){
			if(context.GetAttribute("type")=="dic"&&context.ChildNodes.Count%2==1){
				string key=((System.Xml.XmlElement)context.LastChild).GetAttribute("value");
				if(key.IndexOf("utf-8")>=0){
					type="string";
					value=System.Text.Encoding.UTF8.GetString(data);
				}else if(key=="pieces"){
					type="binary;base64";
					//System.Text.StringBuilder sb=new System.Text.StringBuilder("",data.Length*2);
					//for(int i=0;i<data.Length;i++)sb.Append(data[i].ToString("X2"));
					//value=sb.ToString();
					value=System.Convert.ToBase64String(data);
				}else base.init_binary2string (data, context, out type, out value);
			}else base.init_binary2string (data, context, out type, out value);
		}

	}
}
