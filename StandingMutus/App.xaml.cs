using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Aldentea.StandingMutus
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Aldentea.Wpf.Application.Application
	{


		#region  2. お決まりの設定．(コピペでいいかも．)
		// 06/18/2014 by aldentea 
		protected App()
			: base()
		{
			this.Document = new SweetMutus.Data.SweetMutusGameDocument();
			this.Exit += new ExitEventHandler(App_Exit);
		}

		void App_Exit(object sender, ExitEventArgs e)
		{
			MySettings.Save();
		}

		#region *MySettingsプロパティ
		/// <summary>
		/// アプリケーションの設定を取得します．
		/// </summary>
		internal Properties.Settings MySettings
		{
			get
			{
				// 単に"Properties"では通らない．
				return Aldentea.StandingMutus.Properties.Settings.Default;
			}
		}
		#endregion

		public new static App Current
		{
			get
			{
				return System.Windows.Application.Current as App;
			}
		}


		#region ファイル履歴関連

		public override System.Collections.Specialized.StringCollection FileHistory
		{
			get
			{
				return MySettings.FileHistory;
			}
			set
			{
				MySettings.FileHistory = value;
			}
		}

		public override byte FileHistoryCount
		{
			get { return MySettings.FileHistoryCount; }
		}

		public override byte FileHistoryDisplayCount
		{
			get { return MySettings.FileHistoryDisplayCount; }
		}

		#endregion

		// (0.0.5)
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (MySettings.RequireUpgrade)
			{
				MySettings.Upgrade();
				MySettings.RequireUpgrade = false;
			}
		}
		#endregion

	}
}
