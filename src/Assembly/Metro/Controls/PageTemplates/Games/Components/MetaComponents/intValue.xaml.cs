using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for intValue.xaml
	/// </summary>
	public partial class intValue : UserControl
	{
		public intValue()
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

        /*private void viewValueAs_Click(object sender, RoutedEventArgs e)
        {
            MetaData.ValueField value = this.DataContext as MetaData.ValueField;
            MetroViewValueAs.Show(value.MemoryAddress, value.CacheOffset);
        }*/
    }
}