using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for DatumValue.xaml
	/// </summary>
	public partial class DatumValue : UserControl
	{
		public DatumValue()
		{
			InitializeComponent();
			BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
		}

		private void btnNull_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DatumData datum = (DatumData)DataContext;
			datum.Salt = datum.Index = ushort.MaxValue;
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