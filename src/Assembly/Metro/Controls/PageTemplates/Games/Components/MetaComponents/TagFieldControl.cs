using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components.MetaComponents
{
    public class TagFieldControl : UserControl
    {
        public TagFieldControl() { }

        public override void EndInit()
        {
            base.EndInit();
            BorderThickness = new System.Windows.Thickness(1, 1, 1, 1);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            BorderBrush = Brushes.LightSkyBlue;
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
