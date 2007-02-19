namespace TinyFunctions{
	public interface IFunction{
		object Execute(string x,string y,string z);
		string XTitle{get;}
		string YTitle{get;}
		string ZTitle{get;}
	}
	public abstract class FunctionA:IFunction{
		public string XTitle{
			get{
				FunctionAttribute a=this.getAttr();
				return a==null?"--":a.X;
			}
		}
		public string YTitle{
			get{
				FunctionAttribute a=this.getAttr();
				return a==null?"--":a.Y;
			}
		}
		public string ZTitle{
			get{
				FunctionAttribute a=this.getAttr();
				return a==null?"--":a.Z;
			}
		}
		public abstract object Execute(string x,string y,string z);
		public override string ToString(){
			FunctionAttribute a=this.getAttr();
			return a==null?"???":a.Title;
		}
		private FunctionAttribute getAttr(){
			object[] arr=this.GetType().GetCustomAttributes(typeof(FunctionAttribute),true);
			if(arr.Length>0){
				return (FunctionAttribute)arr[0];
			}else return null;
		}
	}
	public class FunctionAttribute:System.Attribute{
		string title;
		string x,y,z;
		public FunctionAttribute(string title){
			this.title=title;
			x=y=z="--";
		}
		public string Title{
			get{return this.title;}
		}
		public string X{
			get{return this.x;}
			set{this.x=value;}
		}
		public string Y{
			get{return this.y;}
			set{this.y=value;}
		}
		public string Z{
			get{return this.z;}
			set{this.z=value;}
		}
	}
}