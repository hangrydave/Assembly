using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for intValue.xaml
	/// </summary>
	public partial class intValue : TagFieldControl
	{
		public intValue()
		{
			InitializeComponent();
        }

        /*private void viewValueAs_Click(object sender, RoutedEventArgs e)
        {
            MetaData.ValueField value = this.DataContext as MetaData.ValueField;
            MetroViewValueAs.Show(value.MemoryAddress, value.CacheOffset);
        }*/
    }
}