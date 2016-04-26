using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Shapes;

namespace Aldentea.StandingMutus
{
	using Aldentea.SweetMutus.Data;
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Wpf.Application.BasicWindow, INotifyPropertyChanged
	{

		#region *MyDocumentプロパティ
		protected SweetMutusGameDocument MyDocument
		{
			get { return (SweetMutusGameDocument)App.Current.Document; }
		}
		#endregion

		#region *MySettingsプロパティ
		Properties.Settings MySettings
		{
			get
			{
				return App.Current.MySettings;
			}
		}
		#endregion


		public MainWindow()
		{
			InitializeComponent();

			this.FileHistoryShortcutParent = menuItemHistory;

			//MyDocument.Confirmer = (message) => this.Confirm(message);
			//MyDocument.Initialized += MyDocument_Initialized;
		}

		#region *ウィンドウ初期化時(MainWindow_Initialized)
		private void MainWindow_Initialized(object sender, EventArgs e)
		{
			// 窓の位置やサイズを復元。
			if (MySettings.MainWindowMaximized)
			{
				this.WindowState = System.Windows.WindowState.Maximized;
			}
			if (MySettings.MainWindowRect.Size != new Size(0, 0))
			{
				this.Left = MySettings.MainWindowRect.X;
				this.Top = MySettings.MainWindowRect.Y;
				this.Width = MySettings.MainWindowRect.Width;
				this.Height = MySettings.MainWindowRect.Height;
			}

			//☆this.MySongPlayer.Volume = MySettings.SongPlayerVolume;

		}
		#endregion

		#region *ウィンドウクローズ時(MainWindow_Closed)
		private void MainWindow_Closed(object sender, EventArgs e)
		{
			// 窓の位置やサイズを保存。
			MySettings.MainWindowMaximized = this.WindowState == System.Windows.WindowState.Maximized;
			MySettings.MainWindowRect = new Rect(this.Left, this.Top, this.Width, this.Height);

			//☆MySettings.SongPlayerVolume = this.MySongPlayer.Volume;

		}



		#endregion

		string _category = string.Empty;



		#region *CurrentQuestionプロパティ
		/// <summary>
		/// 出題中の問題を取得します(setterはとりあえずprivateです)．
		/// </summary>
		public SweetQuestion CurrentQuestion
		{
			get
			{
				return _currentQuestion;
			}
			private set
			{
				if (_currentQuestion != value)
				{
					_currentQuestion = value;
					NotifyPropertyChanged("CurrentQuestion");
				}
			}
		}
		SweetQuestion _currentQuestion;
		#endregion

		private void Standby_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// ※とりあえず。
			e.CanExecute = true;
		}

		private void Standby_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var nextQuestion = MyDocument.GetQuestion(_category, 1);
			if (nextQuestion == null)
			{
				MessageBox.Show("問題がありません。");
			}
			else
			{
				MyDocument.AddOrder(nextQuestion.ID);

			}
		}

		#region イベントハンドラ

		#region *Order追加時
		private void MyDocument_OrderAdded(object sender, GrandMutus.Data.OrderEventArgs e)
		{
			var q_id = e.QuestionID;
			if (q_id.HasValue)
			{
				var nextQuestion = MyDocument.Questions.Get(q_id.Value);
				this.CurrentQuestion = nextQuestion;
				//MyQuestionPlayer.Open(nextQuestion);
				//this.CurrentPhase = PlayingPhase.Ready;
			}
		}
		#endregion

		#region *Order削除時
		private void MyDocument_OrderRemoved(object sender, GrandMutus.Data.OrderEventArgs e)
		{
			//this.CurrentPhase = PlayingPhase.Talking;
			//MyQuestionPlayer.Close();
			this.CurrentQuestion = null;
		}
		#endregion

		#endregion


		#region INotifyPropertyChanged実装

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void NotifyPropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

	}
}
