using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

/*
�\��
	�J�����_�[��t����B�J�����_�[�ɂ́A�\��Ȃǂ��������\������
		MonthCalender �͓��t�����\���o���Ȃ��̂Ł~
		�J�����_�[�R���g���[�����쐬����܂ŏo���Ȃ��c
			�@���ɂ��Ə��ڂɕ\�����镨���L�q���� Object
			�A�@�� Object ��ێ�(HashTable)���A���ݕ\�����Ă��錎�ɊY��������ɂ�������΁A���̏��ڂɕ\������
	�J�����_�[�̓��ɂ���I�ԂƁA���̓��ɂ��Ȃ���΂Ȃ�Ȃ����E���̓��̓��L�E���̓��̉ƌv���\���^�ҏW���鎖���o����l�ɂ���
	�N���������ɂ́A�����̂��Ȃ���΂Ȃ�Ȃ����A�����������Ă��镨��\������B
		���̓��ɂ��\��̕���\��
		��������ɂ̓`�F�b�N������
		�X�ɒ�o�Ȃǂ̏��u�����������`�F�b�N���i�K�̂��镨�́A�`�F�b�N�𕡐����������o����l�ɂ���
		���\��̓����߂��������\��(���\��̓��������O��)
*/
namespace Tools{
	/// <summary>
	/// Form1 �̊T�v�̐����ł��B
	/// </summary>
	public class Task1 : System.Windows.Forms.Form{
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Task1(){
			InitializeComponent();
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent(){
			// 
			// Task1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(512, 357);
			this.Name = "Task1";
			this.Text = "Task";

		}
		#endregion

		/// <summary>
		/// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Task1());
		}

	}
}
