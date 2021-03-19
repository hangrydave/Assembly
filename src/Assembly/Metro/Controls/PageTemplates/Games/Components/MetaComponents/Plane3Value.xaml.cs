using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for Plane3Value.xaml
	/// </summary>
	public partial class Plane3Value : UserControl
	{
		public Plane3Value()
		{
			InitializeComponent();
            BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            BorderBrush = Brushes.Yellow;
            ((ValueField)DataContext).SetFieldSelection();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            BorderBrush = null;
            ((ValueField)DataContext).SetFieldSelection();
        }
    }
}