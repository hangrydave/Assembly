using System;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData
{
	public enum StringType
	{
		ASCII,
		UTF16
	}

	public class StringData : ValueField
	{
		private int _length;
		private StringType _type;
		private string _value;

		public StringData(string name, uint offset, long address, StringType type, string value, int size, uint pluginLine, string tooltip, Action<long?, int> fieldSelected)
			: base(name, offset, address, pluginLine, tooltip, fieldSelected)
		{
			_value = value;
			_length = size;
			_type = type;
		}

		public string Value
		{
			get { return _value; }
			set
			{
				_value = value;
				NotifyPropertyChanged("Value");
			}
		}

		public new int Length
		{
			get { return _length; }
			set
			{
				_length = value;
				NotifyPropertyChanged("Size");
				NotifyPropertyChanged("MaxLength");
			}
		}

		public int MaxLength
		{
			get
			{
				switch (_type)
				{
					case StringType.ASCII:
						return _length;
					case StringType.UTF16:
						return _length/2;
					default:
						return _length;
				}
			}
		}

		public StringType Type
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

		public override void Accept(IMetaFieldVisitor visitor)
		{
			visitor.VisitString(this);
		}

		public override MetaField CloneValue()
		{
			return new StringData(Name, Offset, FieldAddress, _type, _value, _length, PluginLine, ToolTip, _setFieldSelection);
		}

		public override int Size() => Length;
	}
}