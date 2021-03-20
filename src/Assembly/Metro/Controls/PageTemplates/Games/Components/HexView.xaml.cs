using Blamite.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Assembly.Metro.Controls.PageTemplates.Games.Components
{
    /// <summary>
    /// Interaction logic for HexView.xaml
    /// </summary>
    public partial class HexView : UserControl
    {
        private List<List<string>> _data;
        private int _selectedRow;
        private int _selectedCol;
        private uint? _selectedOffset;
        private int _selectedSize;

        public void Init(IStreamManager streamManager, long baseOffset, int baseSize)
        {
            int rowCount = baseSize / 16;
            int extraBytes = baseSize % 16;
            if (extraBytes > 0)
                rowCount++;

            int row = 0;
            int col = 0;

            _data = new List<List<string>>(rowCount);
            _data.Add(new List<string>(16));

            IReader reader = streamManager.OpenRead();
            reader.SeekTo(baseOffset);

            for (int i = 0; i < baseSize; i++)
            {
                byte b = reader.ReadByte();
                string s = b.ToString("X2");
                _data[row].Add(s);

                col++;
                if (col == 16)
                {
                    col = 0;
                    row++;
                    _data.Add(new List<string>(16));
                }
            }

            InitializeComponent();
            hexGrid.ItemsSource = _data;
        }

        public void SetFieldSelection(uint? offset, int size)
        {
            if (offset == null)
            {
                ScrollToAndHighlightSelection(null);
                return;
            }

            if (_selectedOffset != null)
            {
                ScrollToAndHighlightSelection(null);
            }

            _selectedOffset = offset;
            _selectedSize = size;

            if (!offset.HasValue)
                return;

            _selectedRow = ((int) offset.Value) / 16;
            _selectedCol = ((int) offset.Value) % 16;

            ScrollToAndHighlightSelection(Brushes.LightBlue);
        }

        private void ScrollToAndHighlightSelection(Brush brush)
        {
            int rowCount = (_selectedCol + _selectedSize) / 16;
            int leftovers = (_selectedCol + _selectedSize) % 16;
            if (leftovers > 0)
                rowCount++;

            for (int row = 0; row < rowCount; row++)
            {
                ContentPresenter rowPresenter = hexGrid.ItemContainerGenerator.ContainerFromIndex(_selectedRow + row) as ContentPresenter;
                ItemsControl rowControl = (ItemsControl)VisualTreeHelper.GetChild(rowPresenter, 0);

                int startingCol = 0;
                int endingCol = 0;
                GetStartingAndEndingCol(row, ref startingCol, ref endingCol);

                for (int col = startingCol; col < endingCol; col++)
                {
                    ContentPresenter valuePresenter = rowControl.ItemContainerGenerator.ContainerFromIndex(col) as ContentPresenter;
                    if (valuePresenter == null)
                        break; // what just happeeeeened

                    TextBlock textBlock = (TextBlock)VisualTreeHelper.GetChild(valuePresenter, 0);
                    textBlock.Background = brush;
                }
            }

            // 30 is the height of the rows, so stick the selection in roughly the middle of the screen
            double offset = (30 * _selectedRow) - scrollViewer.ViewportHeight / 2;
            scrollViewer.ScrollToVerticalOffset(offset);

            void GetStartingAndEndingCol(int row, ref int startingCol, ref int endingCol)
            {
                if (row == 0)
                {
                    startingCol = _selectedCol;
                    if (rowCount == 1)
                        endingCol = startingCol + _selectedSize;
                    else
                        endingCol = 16;
                }
                else
                {
                    startingCol = 0;
                    if (row < rowCount - 1)
                        endingCol = 16;
                    else
                        endingCol = _selectedSize;
                }
            }
        }
    }
}
