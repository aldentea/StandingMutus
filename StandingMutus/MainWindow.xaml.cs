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

			MyQuestionPlayer.MediaOpened += MyQuestionPlayer_MediaOpened;
			MyQuestionPlayer.QuestionStopped += questionPlayer_QuestionStopped;
			MyQuestionPlayer.QuestionEnded += MyQuestionPlayer_QuestionEnded;

			MyDocument.OrderAdded += MyDocument_OrderAdded;
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




		#region フェイズ関連


		enum Phase
		{
			Ready,
			FirstPlaying,
			FirstThinking,
			SecondPlaying,
			SecondThinking,
			Talking
		}

		#region *CurrentPhaseプロパティ
		Phase CurrentPhase
		{
		get
			{ return _currentPhase; }
			set
			{
				if (_currentPhase != value)
				{
					_currentPhase = value;
					NotifyPropertyChanged("CurrentPhase");
				}
			}
		}
		Phase _currentPhase = Phase.Talking;
		#endregion

		#endregion




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

		#region *MyQuestionPlayerプロパティ
		public Base.StandingQuestionPlayer MyQuestionPlayer
		{
			get
			{
				return _questionPlayer;
			}
		}
		Base.StandingQuestionPlayer _questionPlayer = new Base.StandingQuestionPlayer();
		#endregion

		private void Standby_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// ※とりあえず。
			e.CanExecute = CurrentPhase == Phase.Talking;
		}

		int n = 1;
		private void Standby_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			_questionEnded = false;
			var nextQuestion = MyDocument.GetQuestion(_category, n++);
			if (nextQuestion == null)
			{
				MessageBox.Show("問題がありません。");
			}
			else
			{
				MyDocument.AddOrder(nextQuestion.ID);
				// これ以降の処理は，OrderAddedのイベントハンドラで行う．
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
				MyQuestionPlayer.Open(nextQuestion);
				this.labelQuestionDuration.Content = _questionPlayer.CurrentQuestionDuration;

				this.CurrentPhase = Phase.Ready;
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


		private void MyQuestionPlayer_MediaOpened(object sender, EventArgs e)
		{
			// この時点で，残り(出題)時間を表示するようにしたい．
			//if (MyQuestionPlayer.Duration.HasValue)
			//{
				// 出題用
				//this.sliderSeekSong_Play.Maximum = _songPlayer.Duration.Value.TotalSeconds;
			//}
		}



		#endregion



		#region Start
		void StartQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MyQuestionPlayer.Start();
			CurrentPhase = Phase.FirstPlaying;
		}

		void StartQuestion_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.CurrentPhase == Phase.Ready; // CurrentQuestion must not be null.
		}
		#endregion

		#region Stop

		// SecondPlayの停止は、パラメータにtrueを渡す。
		// (通常はEndで終了するので、手動でこのコマンドを呼び出す必要はないはず。)
		// ↑やっぱやめた。パラメータ不要で、First、Secondいずれにも使えることにする。

		void StopQuestion_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.CurrentPhase == Phase.SecondPlaying || CurrentPhase == Phase.FirstPlaying; // CurrentQuestion must not be null.
		}
		void StopQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MyQuestionPlayer.Stop(CurrentPhase == Phase.SecondPlaying);
		}


		private void questionPlayer_QuestionStopped(object sender, EventArgs e)
		{
			// ※これってEndでも発生するんだっけ？
			CurrentPhase = Phase.FirstThinking;
		}

		#endregion

		#region Restart
		void RestartQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (CurrentPhase == Phase.FirstThinking && !_questionEnded)
			{
				MyQuestionPlayer.Restart();
				CurrentPhase = Phase.SecondPlaying;
			}
		}
		void RestartQuestion_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.CurrentPhase == Phase.FirstThinking && !_questionEnded; // CurrentQuestion must not be null.
		}
		#endregion

		private void MyQuestionPlayer_QuestionEnded(object sender, EventArgs e)
		{
			// ↓これを直ちに実行しない方がいいかな。
			// →SecondPlayingならいいんじゃない？
			if (CurrentPhase == Phase.SecondPlaying)
			{
				CurrentPhase = Phase.SecondThinking;
			}
			else if (CurrentPhase == Phase.FirstPlaying)
			{
				_questionEnded = true;
				CurrentPhase = Phase.FirstThinking;
			}
		}
		bool _questionEnded = false;


		#region EndQuestion

		// これが必要になるケースは？
		// 1. 完全スルーになるケース。
		// 2. 起立が極めて遅く、Restartが不要と判断されるケース。
		// の2つだと思う。
		// いずれにしろ、起立を締め切った時点でEndQuestionを実行することになる。

		void EndQuestion_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			MyQuestionPlayer.End();
			CurrentPhase = Phase.SecondThinking;
		}

		void EndQuestion_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.CurrentPhase == Phase.FirstThinking; // CurrentQuestion must not be null.
		}
		#endregion


		#region SeekSabi

		void SeekSabi_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if ( /*CurrentPhase == PlayingPhase.Judged || */ CurrentPhase == Phase.Talking)
			{
				MyQuestionPlayer.CurrentPosition = CurrentQuestion.SabiPos;
			}
		}

		void SeekSabi_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = /* CurrentPhase == PlayingPhase.Judged || */ CurrentPhase == Phase.Talking;
			//e.Handled = true;
		}

		#endregion


		#region SwitchPlayPauseコマンド

		void SwitchPlayPause_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if ( CurrentPhase == Phase.SecondThinking || /*CurrentPhase == PlayingPhase.Judged || */ CurrentPhase == Phase.Talking)
			{
				CurrentPhase = Phase.Talking;
				MyQuestionPlayer.SwitchPlayPause();
			}
		}

		void SwitchPlayPause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CurrentPhase == Phase.SecondThinking || /* CurrentPhase == Phase.Judged || */ CurrentPhase == Phase.Talking;
			//e.Handled = true;
		}

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
