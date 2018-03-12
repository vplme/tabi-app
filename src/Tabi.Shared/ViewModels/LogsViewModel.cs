using System;
using System.Text;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class LogsViewModel : BaseViewModel
    {
        public LogsViewModel()
        {
            _htmlWebViewSource = new HtmlWebViewSource();
        }

        private HtmlWebViewSource _htmlWebViewSource;

        public HtmlWebViewSource HtmlSource
        {
            get => _htmlWebViewSource;
            set
            {
                _htmlWebViewSource = value;
                OnPropertyChanged();
            }
        }

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                HtmlWebViewSource source = new HtmlWebViewSource {Html = LogToHtml(_text)};
                HtmlSource = source;
                OnPropertyChanged();
            }
        }

        private string LogToHtml(string text)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<style>body { font-size: 7pt; } </style>");
            builder.Append(text.Replace(Environment.NewLine, "<br/>"));
            return builder.ToString();
        }
    }
}