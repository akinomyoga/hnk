namespace hnk.memopad{
	using mwg.Windows.TreeViewClasses;
	/// <summary>
	/// ������ۑ�����ׂ̃N���X�ł��B
	/// </summary>
	public sealed class MemoDocument:System.Xml.XmlDocument,System.IDisposable{
		private string path;
		//===========================================================
		//		.ctor/dtor
		//===========================================================
		public MemoDocument(string path):base(){
			//-- null/�� �̃p�X
			if(path==null||path==""){
				this.path=System.IO.Path.Combine(afh.Application.Path.ExecutableDirectory,SAVEPATH);
			}else this.path=path;

			//-- �t�@�C���Ǎ�
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
		/// ��n�������܂��B
		/// </summary>
		public void Dispose(){
			this.Dispose_main();
			// [���ׂ�����]
			// System.Xml.XmlDocument:System.Xml.XmlNode:System.Object
			// �͗L���� Finalize ���\�b�h�������Ȃ�
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
		/// ����̃����̕ۑ���̃p�X�ł��B
		/// </summary>
		public const string SAVEPATH="memo.xml";
		/// <summary>
		/// ����� XML �̓��e�ł��B
		/// </summary>
		public const string DEFAULTXML="<?xml version=\"1.0\"?>\r\n<R><F name=\"(default)\" title=\"\"/></R>";
		public MemoDocument():this(null){}
		//===========================================================
		//		�q�t�@�C��
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
	/// �u�t�@�C���v��\������N���X�ł��B
	/// </summary>
	public sealed class File :System.Windows.Forms.TreeNode,NodeDragDrop.IOnInsert{
		/// <summary>
		/// ���̃t�@�C���̓��e��ێ����Ă��� XML �v�f��ێ����܂��B
		/// </summary>
		public readonly System.Xml.XmlElement Element;
		internal File(System.Xml.XmlElement elem):base(){
			this.Element=elem;
			this.UpdateNodeText();
			foreach(System.Xml.XmlElement elem0 in XmlEnum.ChildrenByTagName(this.Element,"F"))
				this.Nodes.Add(new File(elem0));
		}
		/// <summary>
		/// �m�[�h�ɕ\������镶����� XmlElement �ɑΉ��������ɐݒ肵�܂��B
		/// </summary>
		public void UpdateNodeText(){
			string text=this.Title;
			this.Text=text!=""?text:"<����>";
		}
		/// <summary>
		/// ���� File �̒��g���擾���܂��B
		/// </summary>
		public string Content{
			get{return this.GetElement("content").InnerText;}
			set{this.GetElement("content").InnerText=value;}
		}
		/// <summary>
		/// ���� File �̃^�C�g�����擾���܂��B
		/// </summary>
		public string Title{
			get{return this.Element.GetAttribute("title");}
			set{this.Element.SetAttribute("title",this.Text=value);}
		}
		/// <summary>
		/// ���� File �̎��ʖ����擾���܂��B
		/// </summary>
		public new string Name{
			get{return this.Element.GetAttribute("name");}
			set{this.Element.SetAttribute("name",value);}
		}
		//===========================================================
		//		�q File
		//===========================================================
		/// <summary>
		/// �e�m�[�h�� File �Ƃ��Ď擾���܂��B
		/// </summary>
		public new File Parent{get{return base.Parent as File;}}
		public MemoDocument OwnerDocument{
			get{return (MemoDocument)this.Element.OwnerDocument;}
		}
		public System.Xml.XmlElement ParentElement{
			get{return (System.Xml.XmlElement)this.Element.ParentNode;}
		}
		/// <summary>
		/// ���̃m�[�h�� Xml �v�f�Ƃ��Ă� Index ���擾���܂��B
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
		/// �w�肵�����O�̎q File ���擾���܂��B
		/// </summary>
		public File this[string name]{
			get{return this.GetFile(name);}
		}
		/// <summary>
		/// �t�@�C���̗񋓎q���擾���܂��B
		/// </summary>
		/// <returns>�t�@�C���̗񋓎q��Ԃ��܂��B</returns>
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
		//		File �}��
		//===========================================================
		/// <summary>
		/// �V�����w�肵�����O�� File ���q�Ƃ��č쐬���܂��B
		/// </summary>
		/// <param name="name">�V���� File �̖��O���w�肵�܂��B</param>
		/// <returns>�V�����쐬���� File �ւ̎Q�Ƃ�Ԃ��܂��B</returns>
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
		/// �w�肵���m�[�h�����m�[�h�̎q�Ƃ��đ}�����܂��B
		/// </summary>
		/// <param name="file">���̃m�[�h�ɒǉ������m�[�h���w�肵�܂��B</param>
		/// <param name="index">�}������ʒu���w�肵�܂��B���̒l���w�肵���ꍇ�ɂ͍Ō�ɒǉ����܂��B</param>
		public void InsertFile(File file,int index){
			if(!this.CanInsert(file))
				throw new System.ApplicationException("�q���֌W�ɂ��w�肵���m�[�h�����̃m�[�h�ɔz�u���鎖�͏o���܂���B");
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
		/// DragDrop �ɂ���đ}������鎞�̏����ł��B
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
			//--�}���ʒu�Q�Ɨp File (�Z�� File)
			File sibling;
			if(tgfile!=null){
				if(index>=tgfile.Nodes.Count)goto add;
				sibling=(File)tgfile.Nodes[index];
			}else{
				if(index>=OwnerDocument.files.Count)goto add;
				sibling=(File)OwnerDocument.files[index];
			}

			//--�ړ�������폜
			scelem.RemoveChild(this.Element);

			//--�ړ���ɑ}��
			tgelem.InsertBefore(this.Element,sibling.Element);
			if(tgfile==null)OwnerDocument.RefreshFiles();
			return;
		}
		void NodeDragDrop.IOnInsert.AfterInsert(System.Windows.Forms.TreeNode target,int index){}
		//===========================================================
		//		File ����
		//===========================================================
		/// <summary>
		/// �w�肵�����O�̎q�m�[�h�������ۂ����擾���܂��B
		/// </summary>
		/// <param name="name">��������q�m�[�h�̖��O���w�肵�܂��B</param>
		/// <returns>�w�肵�����O�̎q�m�[�h�����ꍇ�� true ��Ԃ��܂��B</returns>
		public bool ContainsFile(string name){
			foreach(File file in this.Nodes)if(file.Name==name)return true;
			return false;
		}
		/// <summary>
		/// �w�肵�����O�̎q�m�[�h���擾���܂��B
		/// </summary>
		/// <param name="name">��������q�m�[�h�̖��O���w�肵�܂��B</param>
		/// <returns>���������q�m�[�h��Ԃ��܂��B������Ȃ������ꍇ�ɂ͐V�����쐬���ĕԂ��܂��B</returns>
		private File GetFile(string name){
			foreach(File file in this.Nodes)if(file.Name==name)return file;
			return this.CreateChild(name,-1);
		}
		//===========================================================
		//		�q�v�f�̌���
		//===========================================================
		/// <summary>
		/// �w�肵�� tagName �̈�ڂ̗v�f���擾���܂��B�����ꍇ�͍쐬���܂��B
		/// </summary>
		/// <param name="tagName">�v�f�����w�肵�܂��B</param>
		/// <returns>�������A���͍쐬���� System.Xml.XmlElement �̃C���X�^���X��Ԃ��܂��B</returns>
		private System.Xml.XmlElement GetElement(string tagName){
			foreach(System.Xml.XmlElement node in XmlEnum.ChildrenByTagName(this.Element,tagName))return node;
			System.Xml.XmlElement elem=this.Element.OwnerDocument.CreateElement(tagName);
			this.Element.AppendChild(elem);
			return elem;
		}

#if old
		/// <summary>
		/// �w�肵�� tagName �w�肵�� name ����������ڂ̗v�f��Ԃ��܂��B
		/// </summary>
		/// <param name="tagName">�����Ώۂ̗v�f�̗v�f�����w�肵�܂��B</param>
		/// <param name="attrName">�����Ώۂ̗v�f�� name �������w�肵�܂��B</param>
		/// <returns>
		/// �w�肵�������𖞂����v�f�������ĕԂ��܂��B
		/// ������Ȃ������ꍇ�ɂ͐V�����쐬���܂��B
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
		/// �w�肵���v�f�����Ɋ܂܂�Ă��邩�ǂ������擾���܂��B
		/// </summary>
		/// <param name="tagName">��������v�f�̗v�f�����w�肵�܂��B</param>
		/// <param name="attrName">��������v�f�� name �����̓��e���w�肵�܂��B</param>
		/// <returns>�܂܂�Ă���ꍇ�� true ��Ԃ��܂��B</returns>
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