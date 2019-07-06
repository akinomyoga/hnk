using DDE=System.Windows.Forms.DragDropEffects;
namespace mwg.Windows.TreeViewClasses{
	/// <summary>
	/// TreeView �̍��ڂɑ΂��� DragDrop ������������܂��B
	/// </summary>
	/// <remarks>
	/// ���̂Ƃ���̓m�[�h�̈ړ��݂̂ɑΉ����܂��B
	/// ���Ƃ���̃m�[�h�E���̐�c�� IDropTarget ���������Ă���ꍇ�͂��̏����Q�Ƃ��܂��B
	/// </remarks>
	public abstract class NodeDragDrop{
		/// <summary>
		/// ���̃N���X�Ɋ֘A�t�����Ă��� TreeView ��ێ����܂��B
		/// </summary>
		protected System.Windows.Forms.TreeView view;
		/// <summary>
		/// �h���b�O���̃A�C�e����ێ����܂��B
		/// </summary>
		protected System.Windows.Forms.TreeNode item=null;
		/// <summary>
		/// TreeNodeDragDrop �̃R���X�g���N�^�ł��B
		/// </summary>
		/// <param name="treeView">�m�[�h�̈ʒu��ύX���� TreeView ���w�肵�܂��B</param>
		/// <param name="source">���� TreeView ���m�[�h�� Drag ���ɂȂ鎖�������܂��B</param>
		/// <param name="target">���� TreeView ���m�[�h�� Drop ��ɂȂ鎖�������܂��B</param>
		public NodeDragDrop(System.Windows.Forms.TreeView treeView,bool source,bool target){
			this.view=treeView;
			if(source){
				this.view.ItemDrag+=new System.Windows.Forms.ItemDragEventHandler(view_ItemDrag);
			}
			if(target){
				this.view.AllowDrop=true;
				this.view.DragOver+=new System.Windows.Forms.DragEventHandler(view_DragOver);
				this.view.DragDrop+=new System.Windows.Forms.DragEventHandler(view_DragDrop);
			}
		}
		private void view_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e){
			if(this.item!=null)return;
			this.item=(System.Windows.Forms.TreeNode)e.Item;
			this.view.DoDragDrop(this,DDE.Move);//e.Data.GetData(typeof(System.Windows.Forms.TreeNode));
			this.item=null;
		}
		private void view_DragOver(object sender, System.Windows.Forms.DragEventArgs e){
			const int NONE=0;
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			System.Drawing.Point p=this.view.PointToClient(new System.Drawing.Point(e.X,e.Y));
			System.Windows.Forms.TreeNode target=this.view.GetNodeAt(p);
			if(target==null){
				this.rev_Draw(this.rev_rect,0);
				return;
			}
			this.hov_OnDragOver(target);

			System.Drawing.Rectangle rect=target.Bounds;
			DDE self,chld,sibl;
			int dropAllow=this.GetDropAllowance(target,e,out self,out sibl,out chld);
			switch(this.view_switch(dropAllow,p,rect,target.IsExpanded&&target.Nodes.Count>0)){
				case SELF:
					e.Effect=self;
					this.rev_Draw(rect,2);
					break;
				case SIBLB://sibling before
					e.Effect=sibl;
					this.rev_Draw(new System.Drawing.Rectangle(
						rect.Left,rect.Top,0,0
						),1);
					break;
				case SIBLA://sibling after
					e.Effect=sibl;
					this.rev_Draw(new System.Drawing.Rectangle(
						rect.Left,rect.Bottom,0,0
						),1);
					break;
				case CHILD:
					e.Effect=chld;
					this.rev_Draw(new System.Drawing.Rectangle(
						target.Nodes[0].Bounds.Left,rect.Bottom,0,0
						),1);
					break;
				case NONE://else:
					e.Effect=DDE.None;
					this.rev_Draw(rect,0);
					break;
			}
		}
		private void view_DragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			System.Drawing.Point p=this.view.PointToClient(new System.Drawing.Point(e.X,e.Y));
			System.Windows.Forms.TreeNode target=this.view.GetNodeAt(p);
			this.rev_Draw(target.Bounds,0);

			System.Drawing.Rectangle rect=target.Bounds;
			DDE self,chld,sibl;
			int dropAllow=this.GetDropAllowance(target,e,out self,out sibl,out chld);
			switch(this.view_switch(dropAllow,p,rect,target.IsExpanded&&target.Nodes.Count>0)){
				case SELF:
					e.Effect=self;
					this.DropOn(target,e);
					break;
				case SIBLB://sibling before
					e.Effect=sibl;
					this.Insert(target.Parent,e,target.Index);
					break;
				case SIBLA://sibling after
					e.Effect=sibl;
					this.Insert(target.Parent,e,target.Index+1);
					break;
				case CHILD:
					e.Effect=chld;
					this.Insert(target,e,0);
					break;
			}
		}
		/// <summary>
		/// �ʒu�֌W���� Drop ���g�p���悤�Ƃ��Ă��錵���ȑΏۂ��擾���܂��B
		/// </summary>
		/// <param name="dropAllow">Drop �̑ΏۂƂ��ċ�����镨�������t���O���w�肵�܂��B</param>
		/// <param name="rect">�J�[�\���̉��ɂ���m�[�h�̋�`���w�肵�܂��B</param>
		/// <param name="expanded">�J�[�\�������̃m�[�h���q��W�J���Ă��邩�ǂ������w�肵�܂��B</param>
		/// <returns>
		/// 0: ���Ƃ����̏o����Ώۂ͂���܂���B
		/// 1: �J�[�\�������̃m�[�h���̂ɗ��Ƃ��܂��B
		/// 2: �����̃m�[�h�̑O�ɑ}�����܂��B
		/// 3: �����̃m�[�h�̌�ɑ}�����܂��B
		/// 4: �����̃m�[�h�̎q�Ƃ��đ}�����܂��B
		/// </returns>
		private int view_switch(int dropAllow,System.Drawing.Point p,System.Drawing.Rectangle rect,bool expanded){
			const int NONE=0;
			const int SELF=1;
			const int SIBLB=2;
			const int SIBLA=3;
			const int CHILD=4;
			//----------------
			switch(dropAllow){
				case 0:return NONE;
				case 1:return SELF;
				case 2:
					if(p.Y>rect.Top+rect.Height/2){
						if(expanded)return NONE;else return SIBLA;
					}else return SIBLB;
				case 3:
					if(p.Y>rect.Top+rect.Height*3/4){
						if(expanded)return NONE;else return SIBLA;
					}else if(p.Y>rect.Top+rect.Height/4){
						return SELF;
					}else return SIBLB;
				case 4:
					if(expanded&&p.Y>rect.Y+rect.Height/2)return CHILD;
					return NONE;
				case 5:
					if(expanded&&p.Y>rect.Y+rect.Height*3/4)return CHILD;
					return SELF;
				case 6:
					if(p.Y>rect.Top+rect.Height/2){
						if(expanded)return CHILD;else return SIBLA;
					}else return SIBLB;
				case 7:
					if(p.Y>rect.Top+rect.Height*3/4){
						if(expanded)return CHILD;else return SIBLA;
					}else if(p.Y>rect.Top+rect.Height/4){
						return SELF;
					}else return SIBLB;
			}
			return NONE;
		}
		/// <summary>
		/// �w�肵���m�[�h�̎w�肵���C���f�b�N�X�̈ʒu�ɃA�C�e����}���������̓�������s���܂��B
		/// </summary>
		/// <param name="target">Drop ��̐e�̃m�[�h���w�肵�܂��B</param>
		/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
		/// <param name="index">Drop �����̎q�m�[�h�Ƃ��Ă̈ʒu���w�肵�܂��B</param>
		protected abstract void Insert(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e,int index);
		/// <summary>
		/// �w�肵���m�[�h�ɃA�C�e���𗎂Ƃ������̓�������s���܂��B
		/// </summary>
		/// <param name="target">Drop ��̃m�[�h���w�肵�܂��B</param>
		/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
		protected abstract void DropOn(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e);
		//===========================================================
		//		�m�[�h�ɃJ�[�\�����Ȃ������̏���
		//===========================================================
		private System.Windows.Forms.TreeNode hov_node;
		private System.DateTime hov_time;
		private void hov_OnDragOver(System.Windows.Forms.TreeNode node){
			if(hov_node==node){
				long milisec=(System.DateTime.Now-this.hov_time).Ticks/System.TimeSpan.TicksPerMillisecond;
				if(!this.hov_node.IsExpanded&&milisec>800){
					this.hov_node.Expand();
					this.hov_node=null;
				}else if(milisec>2000){
					if(this.hov_node.IsExpanded)this.hov_node.Collapse();
					this.hov_node=null;
				}
			}else{
				this.hov_node=node;
				this.hov_time=System.DateTime.Now;
				this.hov_node.EnsureVisible();
			}
		}
		//===========================================================
		//		���]�\���̊Ǘ�
		//===========================================================
		/// <summary>
		/// 0: ���]�\������Ă��镔���͂Ȃ�
		/// 1: ���]�\������Ă��钼��
		/// 2: ���]�\������Ă����`�̈�
		/// </summary>
		private int rev_mode=0;
		private System.Drawing.Rectangle rev_rect;
		/// <summary>
		/// ���]�\�����Ă���ʒu��ύX���܂��B
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="mode">
		/// 0: ���]�\����������
		/// 1: �����𔽓]�\��
		/// 2: ��`�̈�𔽓]�\��
		/// </param>
		private void rev_Draw(System.Drawing.Rectangle rect,int mode){
			if(rev_mode!=mode||rev_rect!=rect){
				if(rev_mode==1){
					this.rev_DrawLine(rev_rect);
				}else if(rev_mode==2){
					this.rev_DrawRect(rev_rect);
				}
				rev_mode=mode;
				rev_rect=rect;
				if(rev_mode==1){
					this.rev_DrawLine(rev_rect);
				}else if(rev_mode==2){
					this.rev_DrawRect(rev_rect);
				}
			}
		}
		private void rev_DrawLine(System.Drawing.Rectangle rect){
			rect.Y--;
			rect.Height=3;
			rect.Width=this.view.ClientRectangle.Width-rect.X;
			rect=this.view.RectangleToScreen(rect);
			System.Windows.Forms.ControlPaint.FillReversibleRectangle(rect,System.Drawing.Color.Black);
		}
		private void rev_DrawRect(System.Drawing.Rectangle rect){
			rect=this.view.RectangleToScreen(rect);
			System.Windows.Forms.ControlPaint.FillReversibleRectangle(rect,System.Drawing.Color.Black);
		}
		//===========================================================
		//		������ Drop �o���邩�ۂ��̔���
		//===========================================================
		/// <summary>
		/// Drop �̎d�����擾���܂��B
		/// </summary>
		/// <param name="target">���݃}�E�X���ڂ��Ă��� TreeNode ���w�肵�܂��B</param>
		/// <param name="dde_on">�w�肵�� Drop �ڕW���̂ɗ��Ƃ����̌��ʂ�Ԃ��܂��B</param>
		/// <param name="dde_prt_bet">�Z��ʒu�ɑ}�����鎞�̌��ʂ�Ԃ��܂��B</param>
		/// <param name="dde_self_bet">�q���ʒu�ɑ}�����鎞�̌��ʂ�Ԃ��܂��B</param>
		/// <returns>
		/// �ȉ��̑g�����̒l���Ԃ���܂��B
		/// 1. �������g�� Drop �\��
		/// 2. �Z��ʒu�ɑ}���\��
		/// 4. �q���ʒu�ɑ}���\��
		/// </returns>
		protected abstract int GetDropAllowance(
			System.Windows.Forms.TreeNode target,
			System.Windows.Forms.DragEventArgs e,
			out DDE dde_on,out DDE dde_prt_bet,out DDE dde_self_bet);

		#region cls:NddByNode
		/// <summary>
		/// �m�[�h���� DragDrop �̐U�镑����ݒ肷��ۂɎg�p���܂��B
		/// </summary>
		public sealed class NddByNode:NodeDragDrop{
			public NddByNode(System.Windows.Forms.TreeView treeView,bool source,bool target):base(treeView,source,target){}
			/// <summary>
			/// �w�肵���m�[�h�̎w�肵���C���f�b�N�X�̈ʒu�ɃA�C�e����}���������̓�������s���܂��B
			/// </summary>
			/// <param name="target">Drop ��̐e�̃m�[�h���w�肵�܂��B</param>
			/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
			/// <param name="index">Drop �����̎q�m�[�h�Ƃ��Ă̈ʒu���w�肵�܂��B</param>
			protected override void Insert(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e, int index){
				((IDropTarget)target).Drop(e,index);
			}
			/// <summary>
			/// �w�肵���m�[�h�ɃA�C�e���𗎂Ƃ������̓�������s���܂��B
			/// </summary>
			/// <param name="target">Drop ��̃m�[�h���w�肵�܂��B</param>
			/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
			protected override void DropOn(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e){
				((IDropTarget)target).Drop(e);
			}
			/// <summary>
			/// Drop �̎d�����擾���܂��B
			/// </summary>
			/// <param name="target">���݃}�E�X���ڂ��Ă��� TreeNode ���w�肵�܂��B</param>
			/// <param name="dde_on">�w�肵�� Drop �ڕW���̂ɗ��Ƃ����̌��ʂ�Ԃ��܂��B</param>
			/// <param name="dde_prt_bet">�Z��ʒu�ɑ}�����鎞�̌��ʂ�Ԃ��܂��B</param>
			/// <param name="dde_self_bet">�q���ʒu�ɑ}�����鎞�̌��ʂ�Ԃ��܂��B</param>
			/// <returns>
			/// �ȉ��̑g�����̒l���Ԃ���܂��B
			/// 1. �������g�� Drop �\��
			/// 2. �Z��ʒu�ɑ}���\��
			/// 4. �q���ʒu�ɑ}���\��
			/// </returns>
			protected override int GetDropAllowance(
				System.Windows.Forms.TreeNode target,
				System.Windows.Forms.DragEventArgs e,
				out DDE dde_on,out DDE dde_prt_bet,out DDE dde_self_bet){
				IDropTarget dt;
				bool on,self_bet,prt_bet;
				//-- (IDropTarget)target
				if((dt=target as IDropTarget)!=null){
					on=dt.DragOver(e);
					dde_on=e.Effect;
					self_bet=dt.DragOverBetweenChild(e);
					dde_self_bet=e.Effect;
				}else{
					on=self_bet=false;
					dde_on=dde_self_bet=DDE.None;
				}
				//-- (IDropTarget)target.Parent
				if((dt=target.Parent as IDropTarget)!=null){
					prt_bet=dt.DragOverBetweenChild(e);
					dde_prt_bet=e.Effect;
				}else{
					prt_bet=false;
					dde_prt_bet=DDE.None;
				}
				//--����
				return (on?1:0)+(prt_bet?2:0)+(self_bet?4:0);
			}
		}
		/// <summary>
		/// NddByNode �ɉ�����h���b�v�^�[�Q�b�g�̓�����L�q���܂��B
		/// </summary>
		public interface IDropTarget{
			/// <summary>
			/// �A�C�e���̏�� Drop ���ꂽ���ɌĂяo���܂��B
			/// </summary>
			/// <param name="e">�h���b�O�̏󋵂��w�肵�܂��B</param>
			void Drop(System.Windows.Forms.DragEventArgs e);
			/// <summary>
			/// �q�m�[�h�Ƃ��đ}������w��� Drop ���ꂽ���ɌĂяo���܂��B
			/// </summary>
			/// <param name="e">�h���b�O�̏󋵂��w�肵�܂��B</param>
			/// <param name="index">�}����� Index �ł��B</param>
			void Drop(System.Windows.Forms.DragEventArgs e,int index);
			/// <summary>
			/// ���݂̏󋵂ŁA�A�C�e���𗎂Ƃ������o���邩�ۂ����擾���܂��B
			/// </summary>
			/// <param name="e">�h���b�O�̏󋵂��w�肵�܂��B</param>
			/// <returns>�h���b�v���鎖���o����ꍇ�� true ��Ԃ��܂��B</returns>
			bool DragOver(System.Windows.Forms.DragEventArgs e);
			/// <summary>
			/// �q�v�f�Ǝq�v�f�̊ԂɃA�C�e���𗎂Ƃ������o���邩���擾���܂��B
			/// </summary>
			/// <param name="e">�h���b�O�̏󋵂��w�肵�܂��B</param>
			/// <returns>
			/// �q�v�f�̊ԂɃA�C�e���𗎂Ƃ������o����ꍇ�� true ��Ԃ��܂��B
			/// �ʏ�́A�v�f�̏����ɈӖ�������A�����ɐV�����v�f��}������ۂȂǂ� true ��Ԃ��܂��B
			/// </returns>
			/// <remarks>
			/// �e�� IDropTarget ����������
			/// </remarks>
			bool DragOverBetweenChild(System.Windows.Forms.DragEventArgs e);
		}
		#endregion

		#region cls:NddMove
		/// <summary>
		/// �m�[�h�̈ړ����s���܂��B
		/// </summary>
		public sealed class NddMove:NodeDragDrop{
			public NddMove(System.Windows.Forms.TreeView treeView):base(treeView,true,true){}
			/// <summary>
			/// �m�[�h�̏�ɗ��Ƃ������擾���͐ݒ肵�܂��B
			/// true �ɐݒ肳��Ă���ƁAtreeView ���̑S��ŗ��Ƃ������o���܂��B
			/// </summary>
			public bool AllowDropOnNode{
				get{return this._on;}
				set{this._on=value;}
			}bool _on=true;
			/// <summary>
			/// �m�[�h�ƃm�[�h�̊Ԃɗ��Ƃ������擾���͐ݒ肵�܂��B
			/// true �ɐݒ肳��Ă���ƁAtreeView ���̑S��ŗ��Ƃ������o���܂��B
			/// </summary>
			public bool AllowDropBetween{
				get{return this._on;}
				set{this._on=value;}
			}bool _bet=true;
			/// <summary>
			/// Drop ���\���ǂ������擾���܂��B
			/// </summary>
			/// <param name="target">Drop �̑Ώۂ� TreeNode ���w�肵�܂��B</param>
			/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
			/// <param name="dde_on">TreeNode �ɗ��Ƃ����̓����Ԃ��܂��B</param>
			/// <param name="dde_prt_bet">�Z��ʒu�ɗ��Ƃ����̓����Ԃ��܂��B</param>
			/// <param name="dde_self_bet">�q�m�[�h�ʒu�ɗ��Ƃ����̓����Ԃ��܂��B</param>
			/// <returns>�\�� Drop �̕��@�𐔒l�ŕԂ��܂��B�p�����̐������Q�Ƃ��ĉ������B</returns>
			protected override int GetDropAllowance(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e, out System.Windows.Forms.DragDropEffects dde_on, out System.Windows.Forms.DragDropEffects dde_prt_bet, out System.Windows.Forms.DragDropEffects dde_self_bet){
				dde_on=dde_prt_bet=dde_self_bet=DDE.Move;
				//System.Console.WriteLine("o");
				return (_on?1:0)+(_bet?6:0);
			}
			/// <summary>
			/// �w�肵���m�[�h�̎w�肵���C���f�b�N�X�̈ʒu�ɃA�C�e����}���������̓�������s���܂��B
			/// </summary>
			/// <param name="target">Drop ��̐e�̃m�[�h���w�肵�܂��B</param>
			/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
			/// <param name="index">Drop �����̎q�m�[�h�Ƃ��Ă̈ʒu���w�肵�܂��B</param>
			protected override void Insert(System.Windows.Forms.TreeNode target,System.Windows.Forms.DragEventArgs e,int index){
				//--�~ ���� TreeView ����̃A�C�e��
				NddMove this0=e.Data.GetData(typeof(NddMove)) as NddMove;
				if(this0==null||this0!=this)return;

				//--�~ �����E�q���Ɉړ�
				System.Windows.Forms.TreeNodeCollection tnc;
				if(target!=null){
					System.Windows.Forms.TreeNode pp=target;
					do{
						if(pp==this.item)return;
						pp=pp.Parent;
					}while(pp!=null);
					tnc=target.Nodes;
				}else{
					tnc=this.view.Nodes;
				}
				
				IOnInsert ioi=this.item as IOnInsert;
				//--�ړ��O�̏���
				if(ioi!=null)ioi.BeforeInsert(target,index);

				//--�ړ�
				if(index<0){
					this.item.Remove();
					tnc.Add(this.item);
				}else{
					if(tnc.Contains(this.item)&&this.item.Index<index)index--;
					this.item.Remove();
					tnc.Insert(index,this.item);
				}

				//--�ړ���̏���
				if(ioi!=null)ioi.AfterInsert(target,index);
			}
			/// <summary>
			/// �w�肵���m�[�h�ɃA�C�e���𗎂Ƃ������̓�������s���܂��B
			/// </summary>
			/// <param name="target">Drop ��̃m�[�h���w�肵�܂��B</param>
			/// <param name="e">Drag �̏󋵂��w�肵�܂��B</param>
			protected override void DropOn(System.Windows.Forms.TreeNode target, System.Windows.Forms.DragEventArgs e){
				this.Insert(target,e,-1);
			}
		}
		/// <summary>
		/// NddMove ��ݒ肵����� TreeView �̎q�m�[�h�Ɏ������܂��B
		/// �m�[�h�̈ʒu��ύX�����ۂɒʒm���󂯎�肽���ꍇ�� TreeNode �Ɏ������ĉ������B
		/// </summary>
		public interface IOnInsert{
			/// <summary>
			/// ���̈ʒu����폜�����O�ɌĂяo����܂��B
			/// </summary>
			/// <param name="target">�ǉ���� TreeNode ���w�肵�܂��Bnull �̏ꍇ�̓g�b�v���x���ɒǉ�����܂��B</param>
			/// <param name="index">�ǉ������̈ʒu���w�肵�܂��B-1 �̏ꍇ�͖����ɒǉ�����܂��B</param>
			void BeforeInsert(System.Windows.Forms.TreeNode target,int index);
			/// <summary>
			/// �V�����ʒu�ɑ}�����ꂽ��ɌĂяo����܂��B
			/// </summary>
			/// <param name="target">�ǉ���� TreeNode ���w�肵�܂��Bnull �̏ꍇ�̓g�b�v���x���ɒǉ�����܂��B</param>
			/// <param name="index">�ǉ������̈ʒu���w�肵�܂��B-1 �̏ꍇ�͖����ɒǉ�����܂��B</param>
			void AfterInsert(System.Windows.Forms.TreeNode target,int index);
		}
		#endregion
	}
	public class ContextMenuByNode{
		private System.Windows.Forms.TreeView treeView;
		public ContextMenuByNode(System.Windows.Forms.TreeView treeView){
			this.treeView=treeView;
			this.treeView.MouseUp+=new System.Windows.Forms.MouseEventHandler(treeView_MouseUp);
		}

		private System.Windows.Forms.TreeNode contextNode=null;
		private void treeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			if(e.Button==System.Windows.Forms.MouseButtons.Right){
				System.Drawing.Point p=new System.Drawing.Point(e.X,e.Y);
				this.OnContextMenu(sender,this.treeView.GetNodeAt(p),p);
			}
		}
		public event ContextMenuEventHandler ContextMenu;
		private void OnContextMenu(object sender,System.Windows.Forms.TreeNode node,System.Drawing.Point p){
			if(this.ContextMenu==null)return;
			ContextMenuEventArgs e=new ContextMenuEventArgs(node,p);
			this.ContextMenu(sender,e);
		}
		public delegate void ContextMenuEventHandler(object sender,ContextMenuEventArgs e);
		/// <summary>
		/// ContextMenu �C�x���g�Ɋւ���������J���܂��B
		/// </summary>
		public class ContextMenuEventArgs{
			private readonly System.Windows.Forms.TreeNode treenode;
			private readonly System.Drawing.Point p;
			public ContextMenuEventArgs(System.Windows.Forms.TreeNode node,System.Drawing.Point p){
				this.treenode=node;
				this.p=p;
			}
			//=======================================================
			//		Properties
			//=======================================================
			/// <summary>
			/// �E�N���b�N���ꂽ�m�[�h���擾���܂��B
			/// </summary>
			public System.Windows.Forms.TreeNode Node{get{return this.treenode;}}
			/// <summary>
			/// �N���C�A���g���W���擾���܂��B
			/// </summary>
			public System.Drawing.Point ClientPoint{get{return this.p;}}
			/// <summary>
			/// �N���C�A���g���W�� X �l���擾���܂��B
			/// </summary>
			public int ClientX{get{return this.p.X;}}
			/// <summary>
			/// �N���C�A���g���W�� Y �l���擾���܂��B
			/// </summary>
			public int ClientY{get{return this.p.Y;}}
			/// <summary>
			/// �N���b�N�����ʒu���m�[�h�̃��x���ォ�ǂ������擾���܂��B
			/// </summary>
			public bool OnNodeLabel{get{return this.treenode.Bounds.Left<=this.p.X&&this.p.X<=this.treenode.Bounds.Right;}}
			/// <summary>
			/// �N���b�N�����ʒu���m�[�h�̍��]�����ǂ������擾���܂��B
			/// </summary>
			public bool OnLeftMargin{get{return this.p.X<this.treenode.Bounds.Left;}}
			/// <summary>
			/// �N���b�N�����ʒu���m�[�h�̉E�]�����ǂ������擾���܂��B
			/// </summary>
			public bool OnRightMargin{get{return this.treenode.Bounds.Right<this.p.X;}}
		}
	}
}