using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for metaBlock.xaml
	/// </summary>
	public partial class RawBlock : UserControl
	{
		public static RoutedCommand AllocateCommand = new RoutedCommand();
		public static RoutedCommand IsolateCommand = new RoutedCommand();

		public RawBlock()
		{
			InitializeComponent();
			BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
		}

		private void AllocateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void IsolateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
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
			((ValueField)DataContext).ClearFieldSelection();
		}
	}
}