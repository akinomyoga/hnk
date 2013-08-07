using Gdi=System.Drawing;
using Imag=System.Drawing.Imaging;

namespace coord{

  class VectorC{
    public double r;
    public double g;
    public double b;

    VectorC(double r,double g,double b){
      this.r=r;
      this.g=g;
      this.b=b;
    }
    public VectorC(int value){
      r=value>>16&0xFF;
      g=value>> 8&0xFF;
      b=value    &0xFF;
    }
    static int Color2Byte(double x){
      return x<0?0:x>0xFF?0xFF:(int)System.Math.Round(x);
    }
    public int ToInt(){
      return Color2Byte(r)<<16|Color2Byte(g)<<8|Color2Byte(b);
    }

    public static VectorC operator+(VectorC l,VectorC r){
      return new VectorC(l.r+r.r,l.g+r.g,l.b+r.b);
    }
    public static VectorC operator-(VectorC l,VectorC r){
      return new VectorC(l.r-r.r,l.g-r.g,l.b-r.b);
    }
    public static VectorC operator*(VectorC c,double s){
      return new VectorC(c.r*s,c.g*s,c.b*s);
    }
    public static VectorC operator*(double s,VectorC c){
      return new VectorC(c.r*s,c.g*s,c.b*s);
    }
  }

  unsafe class BicubicInterpolatedImage:System.IDisposable{
    byte* p0;
    int stride;
    int width;
    int height;

    Gdi::Bitmap bmp;
    Gdi::Imaging.BitmapData data;

    public BicubicInterpolatedImage(Gdi::Bitmap bmp){
      this.bmp=bmp;
      this.width=bmp.Width;
      this.height=bmp.Height;

      Gdi::Rectangle rect=new Gdi::Rectangle(Gdi::Point.Empty,bmp.Size);
      this.data=bmp.LockBits(rect,Gdi::Imaging.ImageLockMode.ReadOnly,Gdi::Imaging.PixelFormat.Format24bppRgb);

      this.p0=(byte*)this.data.Scan0;
      this.stride=this.data.Stride;
    }
    VectorC this[int x,int y]{
      get{
        if(x<0||width<=x||y<0||height<=y)
          return new VectorC(0xFFFFFF);
        return new VectorC(0xFFFFFF&*(int*)(p0+3*x+stride*y));
      }
    }
    public int Get(double x,double y){
      int ixp=(int)x;double dx=x-(ixp+0.5);
      double xwpp=((+(1.0/6.0)*dx+(1.0/4.0))*dx-(1.0/12.0))*dx-(1.0/12.0);
      double xwmm=((-(1.0/6.0)*dx+(1.0/4.0))*dx+(1.0/12.0))*dx-(1.0/12.0);
      double xwp =((-(1.0/2.0)*dx-(1.0/4.0))*dx+(5.0/4.0 ))*dx+(7.0/12.0);
      double xwm =((+(1.0/2.0)*dx-(1.0/4.0))*dx-(5.0/4.0 ))*dx+(7.0/12.0);
      int iyp=(int)y;double dy=y-(iyp+0.5);

      double ywpp=((+(1.0/6.0)*dy+(1.0/4.0))*dy-(1.0/12.0))*dy-(1.0/12.0);
      double ywmm=((-(1.0/6.0)*dy+(1.0/4.0))*dy+(1.0/12.0))*dy-(1.0/12.0);
      double ywp =((-(1.0/2.0)*dy-(1.0/4.0))*dy+(5.0/4.0 ))*dy+(7.0/12.0);
      double ywm =((+(1.0/2.0)*dy-(1.0/4.0))*dy-(5.0/4.0 ))*dy+(7.0/12.0);

      VectorC color
        =xwpp*(ywpp*this[ixp+2,iyp+2] + ywp*this[ixp+2,iyp+1] + ywm*this[ixp+2,iyp  ] + ywmm*this[ixp+2,iyp-1])
        +xwp *(ywpp*this[ixp+1,iyp+2] + ywp*this[ixp+1,iyp+1] + ywm*this[ixp+1,iyp  ] + ywmm*this[ixp+1,iyp-1])
        +xwm *(ywpp*this[ixp  ,iyp+2] + ywp*this[ixp  ,iyp+1] + ywm*this[ixp  ,iyp  ] + ywmm*this[ixp  ,iyp-1])
        +xwmm*(ywpp*this[ixp-1,iyp+2] + ywp*this[ixp-1,iyp+1] + ywm*this[ixp-1,iyp  ] + ywmm*this[ixp-1,iyp-1]);

      return color.ToInt();
    }
    public void Dispose(){
      this.bmp.UnlockBits(this.data);
    }
  }
  static class FrameDeformQuadratic{
    public unsafe static Gdi::Bitmap Transform(
      Gdi::Bitmap src,int width,int height,
      Vector2 p1,Vector2 p2,Vector2 p3,Vector2 p4,
      Vector2 q1,Vector2 q2,Vector2 q3,Vector2 q4
    ){
      Gdi::Bitmap dst=new Gdi::Bitmap(width,height);
      Gdi::Rectangle dstRect=new Gdi::Rectangle(Gdi::Point.Empty,dst.Size);
      Gdi::Imaging.BitmapData dstData=dst.LockBits(dstRect,Gdi::Imaging.ImageLockMode.WriteOnly,Gdi::Imaging.PixelFormat.Format32bppRgb);
      byte* pdst=(byte*)dstData.Scan0;
      int stride=dstData.Stride;

      double[] x_st_table1=new double[width];
      double[] x_st_table2=new double[width];
      double[] y_st_table1=new double[height];
      double[] y_st_table2=new double[height];
      new BezierArcMeasure(p1,p2,q1).InitializeTable(x_st_table1);
      new BezierArcMeasure(p4,p3,q3).InitializeTable(x_st_table2);
      new BezierArcMeasure(p1,p4,q4).InitializeTable(y_st_table1);
      new BezierArcMeasure(p2,p3,q2).InitializeTable(y_st_table2);

      BezierLine p12=new BezierLine(p1,p2,q1);
      BezierLine p43=new BezierLine(p4,p3,q3);
      BezierLine p14=new BezierLine(p1,p4,q4);
      BezierLine p23=new BezierLine(p2,p3,q2);

      using(BicubicInterpolatedImage srcImage=new BicubicInterpolatedImage(src)){
        for(int ix=0;ix<width;ix++){
          double xt1=x_st_table1[ix];
          double xt2=x_st_table2[ix];
          double dxt=xt2-xt1;
          for(int iy=0;iy<height;iy++){
            double yt1=y_st_table1[iy];
            double yt2=y_st_table2[iy];
            double dyt=yt2-yt1;
            double xt=(xt1+yt1*dxt)/(1-dxt*dyt);
            double yt=(yt1+xt1*dyt)/(1-dxt*dyt);

            // linear interpolation
            Vector2 pl
              =(1-xt1)*(1-yt1)*p1
              +   xt1 *(1-yt2)*p2
              +   xt2 *   yt2 *p3
              +(1-xt2)*   yt1 *p4;

            // bezier interpolation
            Vector2 pBx=p43.GetPoint(xt2)*yt+p12.GetPoint(xt1)*(1-yt);
            Vector2 pBy=p23.GetPoint(yt2)*xt+p14.GetPoint(yt1)*(1-xt);

            Vector2 p=pBx+pBy-pl;
            *(int*)(pdst+4*ix+stride*iy)=srcImage.Get(p.x,p.y);
          }
        }
      }

      dst.UnlockBits(dstData);
      return dst;
    }
    class BezierLine{
      Vector2 p1;
      Vector2 p2;
      Vector2 q;
      public BezierLine(Vector2 p1,Vector2 p2,Vector2 q){
        this.p1=p1;
        this.p2=p2;
        this.q=q;
      }

      public Vector2 GetPoint(double t){
        return t*t*p2+2*t*(1-t)*q+(1-t)*(1-t)*p1;
      }
    }
    class BezierArcMeasure{
      int mode=0;
      double alpha;
      double gamma;
      double cos;
      double sin;

      double integral_coeff;
      double integral_base;
      public BezierArcMeasure(Vector2 p1,Vector2 p2,Vector2 q){
        Vector2 dP3=2*(p1+p2)-4*q;
        Vector2 dP2=2*(q-p1);
        alpha=dP3.Distance;
        gamma=dP2.Distance;
        cos=Vector2.InnerProduct(dP3,dP2)/(alpha*gamma);
        sin=System.Math.Sqrt(1-cos*cos);

        if(alpha<1e-5){
          // 直線: p = t p2 - (1-t) p1
          //   t^2 の項が打ち消し合って次数が落ち、式の形が変わる。
          mode=1;
          integral_coeff=(p2-p1).Distance;
        }else if(gamma<1e-5){
          // 直線: p = [ t^2 + 2 t (1-t) u ] (p2-p1) + p1
          mode=2;
          integral_coeff=(p2-p1).Distance;
          gamma=0; // u=0
        }else if(cos*cos>=1.0||sin<1e-5){
          // 直線: p = [ t^2 + 2 t (1-t) u ] (p2-p1) + p1
          //       u = (p2-p1,q-p1)/|p2-p1|^2
          integral_coeff=(p2-p1).Distance;
          gamma=Vector2.InnerProduct(q-p1,p2-p1)/(integral_coeff*integral_coeff); // u
          if(0<=gamma&&gamma<=1){
            // ∫|dp| = |p2-p1| |t^2 + 2 t (1-t) u|
            mode=2;
          }else{
            // ∫|dp| = |p2-p1| |1-2u| (t+g)^2 sgn(t+g); g=u/(1-2u)
            mode=3;
            integral_coeff*=System.Math.Abs(1-2*gamma); // |1-2u|
            gamma/=(1-2*gamma); // g= u/(1-2u)
          }
        }else{
          // Bezier 曲線
          mode=0;
          integral_coeff=(gamma*sin)*(gamma*sin)/(4*alpha);
        }

        integral_base=getIndefiniteIntegral(0);
      }

      private static double arcsinh(double x){
        return System.Math.Log(x+System.Math.Sqrt(x*x+1));
      }
      private double getIntegralVariableY(double t){
        return arcsinh((alpha*t+gamma*cos)/(gamma*sin));
      }
      private double getIndefiniteIntegral(double t){
        if(mode==0){ // Bezier
          double y=getIntegralVariableY(t);
          return integral_coeff*(System.Math.Sinh(2*y)+2*y);
        }else if(mode==1){
          return integral_coeff*t;
        }else if(mode==2){
          return integral_coeff*t*System.Math.Abs(t+2*(1-t)*gamma);
        }else{ // mode==3
          t+=gamma;
          return integral_coeff*t*System.Math.Abs(t);
        }
      }
      public double GetArcMeasurementFromBezierParameter(double t){
        return getIndefiniteIntegral(t)-integral_base;
      }
      public double GetBezierParameterFromArcMeasurement(double s){
        double tl=0;
        double tu=1;
        while(tu-tl>1e-10){
          double tm=(tl+tu)*0.5;
          double sm=GetArcMeasurementFromBezierParameter(tm);
          if(sm<s)
            tl=tm;
          else
            tu=tm;
        }
        return (tl+tu)*0.5;
      }

      public void InitializeTable(double[] table){
        int ilen=table.Length;
        double slen=GetArcMeasurementFromBezierParameter(1);
        for(int i=0;i<ilen;i++){
          double s=i*(slen/(ilen-1));
          table[i]=GetBezierParameterFromArcMeasurement(s);
        }
      }
    }

  }

  static class SaveImage{
    public static void SaveBitmap(Gdi::Bitmap bmp,string filename){
      switch(System.IO.Path.GetExtension(filename).ToLower()){
        case ".jpg":case ".jpeg":case ".jfif":
          bmp.Save(filename,JpegCodec,GetJpegEncParams(90));
          break;
        case ".png":
          bmp.Save(filename,PngCodec,GetPngEncParams());
          break;
        default:
          bmp.Save(filename);
          break;
      }
    }
    public static Imag::ImageCodecInfo JpegCodec{
      get{
        foreach(Imag::ImageCodecInfo codec in Imag::ImageCodecInfo.GetImageEncoders()){
          if(codec.FormatID==Imag::ImageFormat.Jpeg.Guid)
          return codec;
        }
        return null;
      }
    }
    public static Imag::EncoderParameters GetJpegEncParams(long quality){
      Imag::EncoderParameter qual=new Imag::EncoderParameter(Imag::Encoder.Quality,quality);
      Imag::EncoderParameters param=new Imag::EncoderParameters(1);
      param.Param[0]=qual;
      return param;
    }

    public static Imag::ImageCodecInfo PngCodec{
      get{
        foreach(Imag::ImageCodecInfo codec in Imag::ImageCodecInfo.GetImageEncoders()){
          if(codec.FormatID==Imag::ImageFormat.Png.Guid)
          return codec;
        }
        return null;
      }
    }
    public static Imag::EncoderParameters GetPngEncParams(){
      Imag::EncoderParameters param=new Imag::EncoderParameters(0);
      return param;
    }

  }
}