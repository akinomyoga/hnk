using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Gdi=System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace coord {
	public partial class Form1:Form {
		public Form1() {
			InitializeComponent();
		}

		private void button1_Click(object sender,EventArgs e) {
			// クリップボードに格納された画像の取得
			IDataObject data=Clipboard.GetDataObject();
			if(data.GetDataPresent(DataFormats.Bitmap)){
				this.BaseImage=(Gdi::Bitmap)data.GetData(DataFormats.Bitmap);
			}
		}

		private Gdi::Bitmap baseImage=null;
		private Gdi::Bitmap foreImage=null;
		private Gdi::Graphics graphics=null;
		private Gdi::Bitmap BaseImage{
			set{
				if(this.baseImage!=null){
					this.graphics.Dispose();
					this.foreImage.Dispose();
					this.baseImage.Dispose();
				}
				this.baseImage=value;
				this.foreImage=new Gdi::Bitmap(value.Width,value.Height);
				this.graphics=Gdi::Graphics.FromImage(this.foreImage);
				this.graphics.Clear(Gdi::Color.Transparent);
				this.pictureBox1.BackgroundImage=this.baseImage;
				this.pictureBox1.Image=this.foreImage;
			}
			get{
				return this.baseImage;
			}
		}

		private void pictureBox1_MouseClick(object sender,MouseEventArgs e){
			int x=e.X;
			int y=e.Y;
      if(this.graphics!=null){
			  this.graphics.DrawLine(Gdi::Pens.Magenta,x-3,y,x+3,y);
			  this.graphics.DrawLine(Gdi::Pens.Magenta,x,y-3,x,y+3);
      }
			this.textBox1.AppendText(string.Format("{0} {1}\n",x,y));
			this.pictureBox1.Refresh();
		}
	}
}
