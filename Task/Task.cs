using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

/*
予定
	カレンダーを付ける。カレンダーには、予定などを小さく表示する
		MonthCalender は日付しか表示出来ないので×
		カレンダーコントロールを作成するまで出来ない…
			①日にちと升目に表示する物を記述した Object
			②①の Object を保持(HashTable)し、現在表示している月に該当する日にちがあれば、その升目に表示する
	カレンダーの日にちを選ぶと、その日にやらなければならない事・その日の日記・その日の家計簿を表示／編集する事が出来る様にする
	起動した時には、当日のやらなければならない事、期限が迫っている物を表示する。
		その日にやる予定の物を表示
		やった物にはチェックを入れる
		更に提出などの処置をしたかもチェック→段階のある物は、チェックを複数書く事が出来る様にする
		やる予定の日を過ぎた物も表示(やる予定の日≠期限前日)
*/
namespace Tools{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Task1 : System.Windows.Forms.Form{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Task1(){
			InitializeComponent();
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
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Task1());
		}

	}
}
