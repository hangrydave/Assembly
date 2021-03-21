using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for TagBlock.xaml
	/// </summary>
	public partial class TagBlock : TagFieldControl
	{
		public static RoutedCommand ReallocateCommand = new RoutedCommand();
		public static RoutedCommand IsolateCommand = new RoutedCommand();

		public TagBlock()
		{
			InitializeComponent();

			// Set Information box
			infoToggle.IsChecked = App.AssemblyStorage.AssemblySettings.PluginsShowInformation;
		}

		private void ReallocateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void IsolateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
	}
}