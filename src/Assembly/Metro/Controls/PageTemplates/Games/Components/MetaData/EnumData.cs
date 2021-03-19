﻿using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData
{
	public enum EnumType
	{
		Enum8,
		Enum16,
		Enum32
	}

	public class EnumData : ValueField
	{
		private EnumValue _selectedValue;
		private EnumType _type;
		private int _value;

		public EnumData(string name, uint offset, long address, EnumType type, int value, uint pluginLine, string tooltip, Action<uint?, int> fieldSelected)
			: base(name, offset, address, pluginLine, tooltip, fieldSelected)
		{
			_type = type;
			_value = value;
			Values = new ObservableCollection<EnumValue>();
		}

		public EnumType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				NotifyPropertyChanged("Type");
				NotifyPropertyChanged("TypeStr");
			}
		}

		public string TypeStr
		{
			get { return _type.ToString().ToLower(); }
		}

		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				NotifyPropertyChanged("Value");
			}
		}

        public EnumValue SelectedValue
		{
			get { return _selectedValue; }
			set
			{
				_selectedValue = value;
				NotifyPropertyChanged("SelectedValue");
				Value = value.Value;
			}
		}

		public ObservableCollection<EnumValue> Values { get; private set; }

		public override void Accept(IMetaFieldVisitor visitor)
		{
			visitor.VisitEnum(this);
		}

		public override MetaField CloneValue()
		{
			var result = new EnumData(Name, Offset, FieldAddress, _type, _value, PluginLine, ToolTip, _setFieldSelection);
			foreach (EnumValue option in Values)
			{
				var copiedValue = new EnumValue(option.Name, option.Value, option.ToolTip);
				result.Values.Add(copiedValue);
				if (_selectedValue != null && copiedValue.Value == _selectedValue.Value)
					result._selectedValue = copiedValue;
			}
			return result;
		}

		public override int Size()
		{
			switch (_type)
			{
				case EnumType.Enum8:
					return 1;
				case EnumType.Enum16:
					return 2;
				case EnumType.Enum32:
					return 4;
			}
			return 1;
		}
	}

	public class EnumValue : PropertyChangeNotifier
	{
		private string _name;
		private int _value;
		private string _tooltip;

		public EnumValue(string name, int value, string tooltip)
		{
			_name = name;
			_value = value;
			_tooltip = tooltip;
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				NotifyPropertyChanged("Name");
			}
		}

		public string ToolTip
		{
			get
			{
				return _tooltip;
			}
			set
			{
				_tooltip = value;
				NotifyPropertyChanged("ToolTip");
			}
		}

		public bool ToolTipExists
		{
			get { return !string.IsNullOrEmpty(_tooltip); }
		}

		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				NotifyPropertyChanged("Value");
			}
		}
	}
}