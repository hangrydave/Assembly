using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Blamite.Util;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for StringIDValue.xaml
	/// </summary>
	public partial class StringIDValue : UserControl
	{
		public static readonly DependencyProperty SearchTrieProperty = DependencyProperty.Register("SearchTrie", typeof (Trie),
			typeof (StringIDValue));

		public StringIDValue()
		{
			InitializeComponent();
			BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
		}

		public Trie SearchTrie
		{
			get { return (Trie) GetValue(SearchTrieProperty); }
			set { SetValue(SearchTrieProperty, value); }
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