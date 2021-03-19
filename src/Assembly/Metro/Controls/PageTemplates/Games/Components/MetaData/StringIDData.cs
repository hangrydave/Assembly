using Blamite.Util;
using System;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData
{
	public class StringIDData : ValueField
	{
		private Trie _autocompleteTrie;
		private string _value;

		public StringIDData(string name, uint offset, long address, string val, Trie autocompleteTrie, uint pluginLine, string tooltip, Action<uint?, int> fieldSelected)
			: base(name, offset, address, pluginLine, tooltip, fieldSelected)
		{
			_value = val;
			_autocompleteTrie = autocompleteTrie;
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

		public Trie AutocompleteTrie
		{
			get { return _autocompleteTrie; }
			set
			{
				_autocompleteTrie = value;
				NotifyPropertyChanged("AutocompleteTrie");
			}
		}

		public override void Accept(IMetaFieldVisitor visitor)
		{
			visitor.VisitStringID(this);
		}

		public override MetaField CloneValue()
		{
			return new StringIDData(Name, Offset, FieldAddress, _value, _autocompleteTrie, PluginLine, ToolTip, _setFieldSelection);
		}

		public override int Size() => -1;
	}
}