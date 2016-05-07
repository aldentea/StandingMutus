using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aldentea.StandingMutus.Base
{

	public static class Commands
	{
		public static RoutedCommand StandbyCommand = new RoutedCommand();

		public static RoutedCommand RestartCommand = new RoutedCommand();

		public static RoutedCommand EndQuestionCommand = new RoutedCommand();
	}

}
