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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aldentea.StandingMutus
{
	using Aldentea.SweetMutus.Data;
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Wpf.Application.BasicWindow
	{

		#region *MyDocumentプロパティ
		protected SweetMutusGameDocument MyDocument
		{
			get { return (SweetMutusGameDocument)App.Current.Document; }
		}
		#endregion

		public MainWindow()
		{
			InitializeComponent();

			this.FileHistoryShortcutParent = menuItemHistory;

			//MyDocument.Confirmer = (message) => this.Confirm(message);
			//MyDocument.Initialized += MyDocument_Initialized;
		}
	}
}
