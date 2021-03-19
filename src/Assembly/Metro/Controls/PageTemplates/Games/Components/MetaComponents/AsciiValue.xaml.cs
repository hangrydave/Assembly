using System.Windows;
using System.Windows.Controls;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for AsciiValue.xaml
	/// </summary>
	public partial class AsciiValue : UserControl
	{
		public AsciiValue()
		{
			InitializeComponent();
		}

        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            base.OnStyleChanged(oldStyle, newStyle);
        }
    }
}