﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Assembly.Metro.Dialogs.ControlDialogs;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
	/// <summary>
	///     Interaction logic for TagValue.xaml
	/// </summary>
	public partial class TagValue : UserControl
	{
		public static RoutedCommand JumpToCommand = new RoutedCommand();

		public TagValue()
		{
			InitializeComponent();
			BorderThickness = new System.Windows.Thickness(2, 2, 2, 2);
			//hax hax
			if (cbTagEntry.IsEnabled)
				if (cbTagEntry.SelectedIndex < 0)
					cbTagEntry.SelectedIndex = 0;

		}

		private void ValueChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbTagEntry.SelectedIndex < 0)
				cbTagEntry.SelectedIndex = 0;

			if (cbTagGroup.SelectedIndex > 0)
			{
				btnSearch.IsEnabled = true;
				cbTagEntry.IsEnabled = true;

				TagEntry currentTag = ((TagEntry)cbTagEntry.SelectedItem);

				if (currentTag != null && currentTag.RawTag != null && !currentTag.IsNull)
					btnJumpToTag.IsEnabled = true;
				else
					btnJumpToTag.IsEnabled = false;
			}
			else
			{
				btnJumpToTag.IsEnabled = false;
				btnSearch.IsEnabled = false;
				cbTagEntry.IsEnabled = false;
			}
			bool tagValid = cbTagEntry.SelectedIndex != 1;
		}

		private void CanExecuteJumpToCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			// hack so that the button is enabled by default
			e.CanExecute = true;
		}

		private void btnNullTag_Click(object sender, RoutedEventArgs e)
		{
			cbTagGroup.SelectedIndex = 0;
		}

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            TagRefData currentTag = ((TagRefData)cbTagGroup.DataContext);

            TagValueSearcher searchDialog = new TagValueSearcher(currentTag.Group);
            searchDialog.ShowDialog();

            if (searchDialog.DialogResult.HasValue && searchDialog.DialogResult.Value)
            {
                cbTagEntry.SelectedValue = searchDialog.SelectedTag;
            }
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

	/// <summary>
	///     Converts a TagGroup to an index in the group list.
	/// </summary>
	[ValueConversion(typeof (TagGroup), typeof (int))]
	internal class TagGroupConverter : DependencyObject, IValueConverter
	{
		public static DependencyProperty TagsSourceProperty = DependencyProperty.Register(
			"TagsSource",
			typeof (TagHierarchy),
			typeof (TagGroupConverter)
			);

		public TagHierarchy TagsSource
		{
			get { return (TagHierarchy) GetValue(TagsSourceProperty); }
			set { SetValue(TagsSourceProperty, value); }
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sourceGroup = (TagGroup) value;
			int index = TagsSource.Groups.IndexOf(sourceGroup);
			return index + 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int adjustedIndex = (int) value - 1;
			if (adjustedIndex >= 0 && adjustedIndex < TagsSource.Groups.Count)
				return TagsSource.Groups[adjustedIndex];
			return null;
		}
	}

	[ValueConversion(typeof (TagGroup), typeof (List<TagEntry>))]
	internal class TagEntryListRetriever : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var sourceGroup = value as TagGroup;
			if (sourceGroup == null)
				return null;

			var tagList = new CompositeCollection();
			tagList.Add(sourceGroup.NullTag);

			var mainTagListContainer = new CollectionContainer();
			mainTagListContainer.Collection = sourceGroup.Children;

			tagList.Add(mainTagListContainer);


			return tagList;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}