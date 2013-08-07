using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Gdi=System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace coord {
  public partial class Form2:Form {
    public Form2() {
      InitializeComponent();
    }

    Gdi::Bitmap targetImage;
    Gdi::Bitmap backImage;
    Gdi::Bitmap foreImage;
    Gdi::Graphics foreGraphics;
    private Gdi::Bitmap TargetImage{
      set{
        if(this.targetImage!=null){
          this.targetImage.Dispose();
        }
        this.targetImage=value;
        if(this.targetImage!=null){
          this.UpdatePictureImage();
          this.pictureBox1.Invalidate();
        }
      }
    }

    void AllocatePictureImage(){
      if(this.backImage!=null){
        if(this.backImage.Width==this.pictureBox1.Width
          &&this.backImage.Height==this.pictureBox1.Height)
          return;
        this.FreePictureImage();
      }

      int w=this.pictureBox1.Width;if(w<=0)w=1;
      int h=this.pictureBox1.Height;if(h<=0)h=1;
      this.backImage=new Gdi::Bitmap(w,h);
      this.foreImage=new Gdi::Bitmap(w,h);
      this.foreGraphics=Gdi::Graphics.FromImage(this.foreImage);
      this.foreGraphics.Clear(Gdi::Color.Transparent);
      this.pictureBox1.BackgroundImage=this.backImage;
      this.pictureBox1.Image=this.foreImage;
      this.UpdatePictureImage();
    }
    void UpdatePictureImage(){
      if(this.targetImage!=null&&this.backImage!=null){
        using(Gdi::Graphics g=Gdi::Graphics.FromImage(this.backImage)){
          double sx=(double)this.targetImage.Width/this.backImage.Width;
          double sy=(double)this.targetImage.Height/this.backImage.Height;
          double s=System.Math.Max(sx,sy)*1.2;
          float x=(float)System.Math.Round(0.5*(this.backImage.Width-this.targetImage.Width/s));
          float y=(float)System.Math.Round(0.5*(this.backImage.Height-this.targetImage.Height/s));
          float w=(float)System.Math.Round(this.targetImage.Width/s);
          float h=(float)System.Math.Round(this.targetImage.Height/s);
          g.Clear(Gdi::Color.Transparent);
          g.DrawImage(this.targetImage,x,y,w,h);
          this.deform.SetRectangle(x,y,w,h);
          this.deform.imageWidth=this.targetImage.Width;
          this.deform.imageHeight=this.targetImage.Height;
          this.deform.Draw(this.foreGraphics);
        }
      }
    }
    void FreePictureImage(){
      if(this.backImage!=null){
        this.pictureBox1.BackgroundImage=null;
        this.pictureBox1.Image=null;
        this.backImage.Dispose();
        this.foreImage.Dispose();
        this.foreGraphics.Dispose();
      }
    }

    FrameDeformQuadraticEdge deform=new FrameDeformQuadraticEdge();


    private void button1_Click(object sender,EventArgs e) {
      if(this.openFileDialog1.ShowDialog(this)==DialogResult.OK){
        this.TargetImage=new Gdi::Bitmap(this.openFileDialog1.FileName);
      }
    }
    private void button2_Click(object sender,EventArgs e) {
			// クリップボードに格納された画像の取得
      try{
			  IDataObject data=Clipboard.GetDataObject();
			  if(data.GetDataPresent(DataFormats.Bitmap)){
				  this.TargetImage=(Gdi::Bitmap)data.GetData(DataFormats.Bitmap);
			  }
      }catch{}
    }
    private void button4_Click(object sender,EventArgs e) {
      string filename=this.openFileDialog1.FileName;
      string ext=System.IO.Path.GetExtension(filename);
      this.saveFileDialog1.FileName
        =filename.Substring(0,filename.Length-ext.Length)+"_deformed"+ext;
      if(this.saveFileDialog1.ShowDialog(this)==DialogResult.OK){
        SaveImage.SaveBitmap(this.targetImage,this.saveFileDialog1.FileName);
      }
    }

    private void pictureBox1_SizeChanged(object sender,EventArgs e) {
      this.AllocatePictureImage();
    }

    private void Form2_Load(object sender,EventArgs e) {
      this.AllocatePictureImage();
    }

    bool mouseDown_active;
    Gdi::PointF mouseDown_start;
    private void pictureBox1_MouseDown(object sender,MouseEventArgs e) {
      if(e.Button==MouseButtons.Left){
        this.pictureBox1.Capture=true;
        this.mouseDown_active=true;
        this.mouseDown_start=new Gdi::PointF(e.X,e.Y);
      }else{
        this.pictureBox1.Capture=false;
        this.mouseDown_active=false;
      }
    }
    private void pictureBox1_MouseLeave(object sender,EventArgs e) {
      if(this.mouseDown_active){
        this.pictureBox1.Capture=false;
        this.mouseDown_active=false;
      }
    }
    private void pictureBox1_MouseUp(object sender,MouseEventArgs e) {
      if(this.mouseDown_active){
        this.pictureBox1.Capture=false;
        this.mouseDown_active=false;
        Gdi::PointF mouseDown_end=new Gdi::PointF(e.X,e.Y);
        if(this.deform.NotifyDrag(mouseDown_start,mouseDown_end)){
          this.deform.Draw(this.foreGraphics);
          this.pictureBox1.Invalidate();
        }
      }
    }

    private void button3_Click(object sender,EventArgs e) {
      Gdi::Bitmap deformed=FrameDeformQuadratic.Transform(
        this.targetImage,this.targetImage.Width,this.targetImage.Height,
        this.deform.P1,this.deform.P2,this.deform.P3,this.deform.P4,
        this.deform.Q1,this.deform.Q2,this.deform.Q3,this.deform.Q4);
      this.TargetImage=deformed;
    }


  }

  sealed class Vector2{
    public double x;
    public double y;
    public Vector2(Gdi::PointF p){
      this.x=p.X;
      this.y=p.Y;
    }
    public Vector2(double x,double y){
      this.x=x;
      this.y=y;
    }
    public static Vector2 operator+(Vector2 a,Vector2 b){
      return new Vector2(a.x+b.x,a.y+b.y);
    }
    public static Vector2 operator-(Vector2 a,Vector2 b){
      return new Vector2(a.x-b.x,a.y-b.y);
    }
    public static Vector2 operator*(double s,Vector2 v){
      return new Vector2(s*v.x,s*v.y);
    }
    public static Vector2 operator*(Vector2 v,double s){
      return new Vector2(s*v.x,s*v.y);
    }
    public static bool operator==(Vector2 l,Vector2 r){
      bool lnull=(object)l==null;
      bool rnull=(object)r==null;
      return lnull==rnull&&(lnull||l.x==r.x&&l.y==r.y);
    }
    public static bool operator!=(Vector2 l,Vector2 r){
      return !(l==r);
    }
    public override bool Equals(object obj){
      return obj is Vector2?this==(Vector2)obj:false;
    }
    public override int GetHashCode() {
      return this.x.GetHashCode()^this.y.GetHashCode();
    }

    public static implicit operator Vector2(Gdi::PointF p){
      return new Vector2(p);
    }
    public Gdi::PointF ToPointF(){
      return new Gdi::PointF((float)this.x,(float)this.y);
    }

    public double Norm{
      get{return this.x*this.x+this.y*this.y;}
    }
    public double Distance{
      get{return System.Math.Sqrt(this.Norm);}
    }
    public double ManhattanNorm{
      get{return System.Math.Max(System.Math.Abs(this.x),System.Math.Abs(this.y));}
    }
    public static double InnerProduct(Vector2 l,Vector2 r) {
      return l.x*r.x+l.y*r.y;
    }
  }

  class FrameDeformQuadraticEdge{
    /*  四辺形の各辺は二次 Bezier 曲線で表現する。
     *  制御点の名称は以下の通りとする。
     *  
     *  p1    q1    p2
     *  
     *  q4          q2
     *  
     *  p4    q3    p3
     */
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;
    public Vector2 p4;
    public Vector2 q1;
    public Vector2 q2;
    public Vector2 q3;
    public Vector2 q4;

    double x;
    double y;
    double w;
    double h;
    public double imageWidth=1000;
    public double imageHeight=1000;
    Vector2 TransformCoord_Pict2Image(Vector2 v){
      return new Vector2((v.x-x)/w*imageWidth,(v.y-y)/h*imageHeight);
    }
    public Vector2 P1{get{return TransformCoord_Pict2Image(this.p1);}}
    public Vector2 P2{get{return TransformCoord_Pict2Image(this.p2);}}
    public Vector2 P3{get{return TransformCoord_Pict2Image(this.p3);}}
    public Vector2 P4{get{return TransformCoord_Pict2Image(this.p4);}}
    public Vector2 Q1{get{return TransformCoord_Pict2Image(this.q1);}}
    public Vector2 Q2{get{return TransformCoord_Pict2Image(this.q2);}}
    public Vector2 Q3{get{return TransformCoord_Pict2Image(this.q3);}}
    public Vector2 Q4{get{return TransformCoord_Pict2Image(this.q4);}}

    double containerWidth=1000;
    double containerHeight=1000;

    public FrameDeformQuadraticEdge(double x,double y,double w,double h){
      this.SetRectangle(x,y,w,h);
    }
    public FrameDeformQuadraticEdge()
      :this(0,0,100,100){}

    public void SetRectangle(double x,double y,double w,double h){
      this.x=x;
      this.y=y;
      this.w=w;
      this.h=h;
      this.p1=new Vector2(x  ,y  );
      this.p2=new Vector2(x+w,y  );
      this.p3=new Vector2(x+w,y+h);
      this.p4=new Vector2(x  ,y+h);
      this.q1=0.5*(this.p1+this.p2);
      this.q2=0.5*(this.p2+this.p3);
      this.q3=0.5*(this.p3+this.p4);
      this.q4=0.5*(this.p4+this.p1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pp1"></param>
    /// <param name="pp2"></param>
    /// <returns>描画内容に変更があった場合に true を返します。</returns>
    public bool NotifyDrag(Vector2 pp1,Vector2 pp2){
      if(pp1==pp2)return false;

      Vector2 p0=null;
      double d0=3;

      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,p1);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,p2);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,p3);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,p4);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,q1);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,q2);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,q3);
      this.NotifyDrag_CheckNearest(ref p0,ref d0,pp1,q4);
      if(p0==null)return false;

      Vector2 delta=pp2-pp1;
      p0.x+=delta.x;
      p0.y+=delta.y;
      if(p0.x<0)p0.x=0;else if(p0.x>containerWidth)p0.x=containerWidth;
      if(p0.y<0)p0.y=0;else if(p0.y>containerHeight)p0.y=containerHeight;
      return true;
    }

    void NotifyDrag_CheckNearest(ref Vector2 p0,ref double d0,Vector2 pp1,Vector2 p){
      double d=(pp1-p).ManhattanNorm;
      if(d<=d0){
        p0=p;
        d0=d;
      }
    }

    public void Draw(Gdi::Graphics g){
      g.Clear(Gdi::Color.Transparent);
      g.DrawLine(Gdi::Pens.Blue,this.p1.ToPointF(),this.q1.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p2.ToPointF(),this.q1.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p2.ToPointF(),this.q2.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p3.ToPointF(),this.q2.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p3.ToPointF(),this.q3.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p4.ToPointF(),this.q3.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p4.ToPointF(),this.q4.ToPointF());
      g.DrawLine(Gdi::Pens.Blue,this.p1.ToPointF(),this.q4.ToPointF());
      DrawBezier(g,this.p1,this.q1,this.p2);
      DrawBezier(g,this.p2,this.q2,this.p3);
      DrawBezier(g,this.p3,this.q3,this.p4);
      DrawBezier(g,this.p4,this.q4,this.p1);
      DrawControlPoint(g,this.p1);
      DrawControlPoint(g,this.p2);
      DrawControlPoint(g,this.p3);
      DrawControlPoint(g,this.p4);
      DrawControlPoint(g,this.q1);
      DrawControlPoint(g,this.q2);
      DrawControlPoint(g,this.q3);
      DrawControlPoint(g,this.q4);
    }

    static void DrawBezier(Gdi::Graphics g,Vector2 p1,Vector2 q,Vector2 p2){
      const int N=100;
      Vector2 pp1=p1;
      for(int i=1;i<=N;i++){
        double t=(1.0/N)*i;
        Vector2 pp2=t*t*p2+2*t*(1-t)*q+(1-t)*(1-t)*p1;
        g.DrawLine(Gdi::Pens.Lime,pp1.ToPointF(),pp2.ToPointF());
        pp1=pp2;
      }
    }
    static void DrawControlPoint(Gdi::Graphics g,Vector2 p1){
      const int W=3;
      Gdi::PointF pp1=p1.ToPointF();pp1.X-=W;pp1.Y-=W;
      Gdi::PointF pp2=p1.ToPointF();pp2.X+=W;pp2.Y-=W;
      Gdi::PointF pp3=p1.ToPointF();pp3.X+=W;pp3.Y+=W;
      Gdi::PointF pp4=p1.ToPointF();pp4.X-=W;pp4.Y+=W;
      g.DrawPolygon(Gdi::Pens.Red,new[]{pp1,pp2,pp3,pp4});
    }
  }

}
