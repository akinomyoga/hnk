namespace hnk.memopad{
	using mwg.Windows.TreeViewClasses;
	/// <summary>
	/// メモを保存する為のクラスです。
	/// </summary>
	public sealed class MemoDocument:System.Xml.XmlDocument,System.IDisposable{
		private string path;
		//===========================================================
		//		.ctor/dtor
		//===========================================================
		public MemoDocument(string path):base(){
			//-- null/空白 のパス
			if(path==null||path==""){
				this.path=System.IO.Path.Combine(afh.Application.Path.ExecutableDirectory,SAVEPATH);
			}else this.path=path;

			//-- ファイル読込
			if(System.IO.File.Exists(this.path)){
				try{this.Load(this.path);}
				catch(System.Exception e){
					System.Console.WriteLine(e.ToString());
					try{
						string dest=afh.Application.Path.GetAvailablePath(System.IO.Path.GetFileNameWithoutExtension(this.path),"bk");
						dest=System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.path),dest);
						System.IO.File.Move(this.path,dest);
					}catch{}
					this.LoadXml(DEFAULTXML);
				}
			}else{
				this.LoadXml(DEFAULTXML);
			}
			this.files=new System.Collections.ArrayList();
			this.RefreshFiles();
		}
		private bool _disposed=false;
		/// <summary>
		/// 後始末をします。
		/// </summary>
		public void Dispose(){
			this.Dispose_main();
			// [調べた結果]
			// System.Xml.XmlDocument:System.Xml.XmlNode:System.Object
			// は有効な Finalize メソッドを持たない
			System.GC.SuppressFinalize(this);
		}
		~MemoDocument(){
			this.Dispose_main();
		}
		private void Dispose_main(){
			if(this._disposed)return;
			this._disposed=true;
			this.Save(this.path);
		}
		//===========================================================
		//		default
		//===========================================================
		/// <summary>
		/// 既定のメモの保存先のパスです。
		/// </summary>
		public const string SAVEPATH="memo.xml";
		/// <summary>
		/// 既定の XML の内容です。
		/// </summary>
		public const string DEFAULTXML="<?xml version=\"1.0\"?>\r\n<R><F name=\"(default)\" title=\"\"/></R>";
		public MemoDocument():this(null){}
		//===========================================================
		//		子ファイル
		//===========================================================
		internal System.Collections.ArrayList files;
		public void RefreshFiles(){
			this.files.Clear();
			foreach(System.Xml.XmlElement e in XmlEnum.ChildrenByTagName(this.DocumentElement,"F")){
				this.files.Add(new File(e));
			}
		}
	}
	/// <summary>
	/// 「ファイル」を表現するクラスです。
	/// </summary>
	public sealed class File :System.Windows.Forms.TreeNode,NodeDragDrop.IOnInsert{
		/// <summary>
		/// このファイルの内容を保持している XML 要素を保持します。
		/// </summary>
		public readonly System.Xml.XmlElement Element;
		internal File(System.Xml.XmlElement elem):base(){
			this.Element=elem;
			this.UpdateNodeText();
			foreach(System.Xml.XmlElement elem0 in XmlEnum.ChildrenByTagName(this.Element,"F"))
				this.Nodes.Add(new File(elem0));
		}
		/// <summary>
		/// ノードに表示される文字列を XmlElement に対応した物に設定します。
		/// </summary>
		public void UpdateNodeText(){
			string text=this.Title;
			this.Text=text!=""?text:"<無題>";
		}
		/// <summary>
		/// この File の中身を取得します。
		/// </summary>
		public string Content{
			get{return this.GetElement("content").InnerText;}
			set{this.GetElement("content").InnerText=value;}
		}
		/// <summary>
		/// この File のタイトルを取得します。
		/// </summary>
		public string Title{
			get{return this.Element.GetAttribute("title");}
			set{this.Element.SetAttribute("title",this.Text=value);}
		}
		/// <summary>
		/// この File の識別名を取得します。
		/// </summary>
		public new string Name{
			get{return this.Element.GetAttribute("name");}
			set{this.Element.SetAttribute("name",value);}
		}
		//===========================================================
		//		子 File
		//===========================================================
		/// <summary>
		/// 親ノードを File として取得します。
		/// </summary>
		public new File Parent{get{return base.Parent as File;}}
		public MemoDocument OwnerDocument{
			get{return (MemoDocument)this.Element.OwnerDocument;}
		}
		public System.Xml.XmlElement ParentElement{
			get{return (System.Xml.XmlElement)this.Element.ParentNode;}
		}
		/// <summary>
		/// このノードの Xml 要素としての Index を取得します。
		/// </summary>
		public int ElementIndex{
			get{
				int i=-1;
				foreach(System.Xml.XmlElement elem in XmlEnum.ChildrenByTagName(this.ParentElement,"F")){
					i++;
					if(this.Element==elem)return i;
				}
				return -1;
			}
		}
#if old
		/// <summary>
		/// 指定した名前の子 File を取得します。
		/// </summary>
		public File this[string name]{
			get{return this.GetFile(name);}
		}
		/// <summary>
		/// ファイルの列挙子を取得します。
		/// </summary>
		/// <returns>ファイルの列挙子を返します。</returns>
		public System.Collections.IEnumerator GetEnumerator(){
			return new FileEnumerator(this.Element.GetElementsByTagName("F").GetEnumerator());
		}
		private class FileEnumerator:System.Collections.IEnumerator{
			private System.Collections.IEnumerator innerEnum;
			private File _current=null;
			public FileEnumerator(System.Collections.IEnumerator elemEnumerator){
				this.innerEnum=enumEnumerator;
			}
			public File Current{get{return this._current;}}
			public bool MoveNext(){
				while(this.innerEnum.MoveNext()){
					if(this.innerEnum.Current is System.Xml.XmlElement){
						this._current=new File(this.innerEnum.Current);
						return true;
					}
				}
				return false;
			}
			public void Reset(){this.innerEnum.Reset();this._current=null;}
		}//*/
#endif
		//===========================================================
		//		File 挿入
		//===========================================================
		/// <summary>
		/// 新しく指定した名前の File を子として作成します。
		/// </summary>
		/// <param name="name">新しい File の名前を指定します。</param>
		/// <returns>新しく作成した File への参照を返します。</returns>
		public File CreateChild(string name,int index){
			System.Xml.XmlElement elem=this.OwnerDocument.CreateElement("F");
			elem.SetAttribute("name",name);
			File file=new File(elem);
			if(index<0){
				this.Element.AppendChild(elem);
				this.Nodes.Add(file);
			}else{
				this.InsertFile(file,index);
			}
			return file;
		}
		public void CreateAfter(string name){
			//--create
			System.Xml.XmlElement elem=this.OwnerDocument.CreateElement("F");
			elem.SetAttribute("name",name);
			File newfile=new File(elem);
			//--search target
			File tgfile=this.Parent;
			System.Xml.XmlElement tgelem=tgfile!=null?tgfile.Element:OwnerDocument.DocumentElement;
			//--insert
			tgelem.InsertAfter(newfile.Element,this.Element);
			if(tgfile==null){
				OwnerDocument.RefreshFiles();
				if(this.TreeView!=null){
					this.TreeView.Nodes.Insert(this.Index+1,newfile);
					//MemoTreeView tv=this.TreeView as MemoTreeView;
					//if(tv!=null)tv.RefreshFiles();
				}
			}else{
                tgfile.Nodes.Insert(this.Index+1,newfile);
			}
		}
		public void CreateBefore(string name){
			//--create
			System.Xml.XmlElement elem=this.OwnerDocument.CreateElement("F");
			elem.SetAttribute("name",name);
			File newfile=new File(elem);
			//--search target
			File tgfile=this.Parent;
			System.Xml.XmlElement tgelem=tgfile!=null?tgfile.Element:OwnerDocument.DocumentElement;
			//--insert
			tgelem.InsertBefore(newfile.Element,this.Element);
			if(tgfile==null){
				OwnerDocument.RefreshFiles();
				if(this.TreeView!=null){
					this.TreeView.Nodes.Insert(this.Index,newfile);
					MemoTreeView tv=this.TreeView as MemoTreeView;
					if(tv!=null)tv.RefreshFiles();
				}
			}else{
                tgfile.Nodes.Insert(this.Index,newfile);
			}
		}
		/// <summary>
		/// 指定したノードを自ノードの子として挿入します。
		/// </summary>
		/// <param name="file">このノードに追加されるノードを指定します。</param>
		/// <param name="index">挿入する位置を指定します。負の値を指定した場合には最後に追加します。</param>
		public void InsertFile(File file,int index){
			if(!this.CanInsert(file))
				throw new System.ApplicationException("子孫関係により指定したノードをこのノードに配置する事は出来ません。");
			if(file.Parent!=null){
				file.Parent.Element.RemoveChild(file.Element);
				file.Parent.Nodes.Remove(file);
			}
			if(index<0){
				this.Element.AppendChild(file.Element);
				this.Nodes.Add(file);
			}else{
				this.Element.InsertBefore(file.Element,((File)this.Nodes[index]).Element);
				this.Nodes.Insert(index,file);
			}
		}
		private bool CanInsert(File file){
			File pp=this;
			do{
				if(pp==file)return false;
				pp=pp.Parent;
			}while(pp!=null);
			return true;
		}
		//===========================================================
		//		IOnInsert
		//===========================================================
		/// <summary>
		/// DragDrop によって挿入される時の処理です。
		/// </summary>
		void NodeDragDrop.IOnInsert.BeforeInsert(System.Windows.Forms.TreeNode target,int index){
			File tgfile=(File)target;
			System.Xml.XmlElement scelem=this.ParentElement;
			System.Xml.XmlElement tgelem=tgfile!=null?tgfile.Element:OwnerDocument.DocumentElement;
			if(index>=0)goto insert;
		add:
			scelem.RemoveChild(this.Element);
			tgelem.AppendChild(this.Element);
			return;
		insert:
			if(scelem==tgelem&&this.ElementIndex==index)return;
			//--挿入位置参照用 File (兄弟 File)
			File sibling;
			if(tgfile!=null){
				if(index>=tgfile.Nodes.Count)goto add;
				sibling=(File)tgfile.Nodes[index];
			}else{
				if(index>=OwnerDocument.files.Count)goto add;
				sibling=(File)OwnerDocument.files[index];
			}

			//--移動元から削除
			scelem.RemoveChild(this.Element);

			//--移動先に挿入
			tgelem.InsertBefore(this.Element,sibling.Element);
			if(tgfile==null)OwnerDocument.RefreshFiles();
			return;
		}
		void NodeDragDrop.IOnInsert.AfterInsert(System.Windows.Forms.TreeNode target,int index){}
		//===========================================================
		//		File 検索
		//===========================================================
		/// <summary>
		/// 指定した名前の子ノードを持つか否かを取得します。
		/// </summary>
		/// <param name="name">検索する子ノードの名前を指定します。</param>
		/// <returns>指定した名前の子ノードを持つ場合に true を返します。</returns>
		public bool ContainsFile(string name){
			foreach(File file in this.Nodes)if(file.Name==name)return true;
			return false;
		}
		/// <summary>
		/// 指定した名前の子ノードを取得します。
		/// </summary>
		/// <param name="name">検索する子ノードの名前を指定します。</param>
		/// <returns>見つかった子ノードを返します。見つからなかった場合には新しく作成して返します。</returns>
		private File GetFile(string name){
			foreach(File file in this.Nodes)if(file.Name==name)return file;
			return this.CreateChild(name,-1);
		}
		//===========================================================
		//		子要素の検索
		//===========================================================
		/// <summary>
		/// 指定した tagName の一つ目の要素を取得します。無い場合は作成します。
		/// </summary>
		/// <param name="tagName">要素名を指定します。</param>
		/// <returns>見つけた、亦は作成した System.Xml.XmlElement のインスタンスを返します。</returns>
		private System.Xml.XmlElement GetElement(string tagName){
			foreach(System.Xml.XmlElement node in XmlEnum.ChildrenByTagName(this.Element,tagName))return node;
			System.Xml.XmlElement elem=this.Element.OwnerDocument.CreateElement(tagName);
			this.Element.AppendChild(elem);
			return elem;
		}

#if old
		/// <summary>
		/// 指定した tagName 指定した name 属性を持つ一つ目の要素を返します。
		/// </summary>
		/// <param name="tagName">検索対象の要素の要素名を指定します。</param>
		/// <param name="attrName">検索対象の要素の name 属性を指定します。</param>
		/// <returns>
		/// 指定した条件を満たす要素を見つけて返します。
		/// 見つからなかった場合には新しく作成します。
		/// </returns>
		private System.Xml.XmlElement GetElement(string tagName,string attrName){
			foreach(System.Xml.XmlElement node in this.Element.GetElementsByTagName(tagName))
				if(node.GetAttribute("name")==attrName)return node;
			System.Xml.XmlElement elem=this.Element.OwnerDocument.CreateElement(tagName);
			elem.SetAttribute("name",attrName);
			this.Element.AppendChild(elem);
			return elem;
		}
		/// <summary>
		/// 指定した要素が既に含まれているかどうかを取得します。
		/// </summary>
		/// <param name="tagName">検索する要素の要素名を指定します。</param>
		/// <param name="attrName">検索する要素の name 属性の内容を指定します。</param>
		/// <returns>含まれている場合に true を返します。</returns>
		private bool ContainsElement(string tagName,string attrName){
			foreach(System.Xml.XmlElement node in this.Element.GetElementsByTagName(tagName))
				if(node.GetAttribute("name")==attrName)return true;
			return false;
		}
#endif
	}
	public class XmlEnum{
		public static System.Collections.IEnumerable ChildrenByTagName(System.Xml.XmlElement parent,string tagName){
			return afh.Collections.Enumerable.From(parent.ChildNodes).Filter(delegate(object obj){
				System.Xml.XmlElement e=obj as System.Xml.XmlElement;
				return e!=null&&e.LocalName==tagName;
			});
			//return new afh.Collections.Enumerable(parent.ChildNodes.GetEnumerator(),new f_ChildrenByTagName(tagName));
		}
		/*
		private sealed class f_ChildrenByTagName:afh.Enumerable.IFilter{
			private string tagName;
			public f_ChildrenByTagName(string tagName){this.tagName=tagName;}
			bool afh.Enumerable.IFilter.Filt(object obj){
				System.Xml.XmlElement e=obj as System.Xml.XmlElement;
				return e!=null&&e.LocalName==tagName;
			}
		}
		public static System.Collections.IEnumerable ChildElements(System.Xml.XmlElement parent){
			return new afh.Enumerable(parent.ChildNodes.GetEnumerator(),new f_ChildElements());
		}
		private sealed class f_ChildElements:afh.Enumerable.IFilter{
			public f_ChildElements(){}
			bool afh.Enumerable.IFilter.Filt(object obj){
				System.Xml.XmlElement e=obj as System.Xml.XmlElement;
				return e!=null;
			}
		}
		//*/
	}
}