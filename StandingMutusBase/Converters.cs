﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Aldentea.StandingMutus.Base
{
	public class QuestionDurationConverter : IValueConverter
	{
		// TimeSpan? => string
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TimeSpan?)
			{
				var duration = (TimeSpan?)value;
				if (duration.HasValue)
				{
					return duration.Value.TotalSeconds.ToString("f2");
				}
			}
			return "-----";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class QuestionConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return string.Empty;
			}
			else if (value is SweetMutus.Data.SweetQuestion)
			{
				// parameterがtrueならば、アーティスト名を省略します。
				var format = (parameter is bool && (bool)parameter) ? "{0}{1}" : "{0}{1}／{2}";
				var question = (SweetMutus.Data.SweetQuestion)value;
				//return $"{question.No.HasValue ? question.No.Value.ToString("00.") : string.Empty}{question.Title}／{question.Artist}";	// C# 6.0 の記法。← ? : は使えないの？
				return string.Format(format, 
					question.No.HasValue ? question.No.Value.ToString(@"00\.") : string.Empty,
					question.Title, question.Artist);
			}
			else
			{
				throw new ArgumentException("valueにはSweetQuestionを指定して下さい。");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
