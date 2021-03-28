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
        private FileSegmentGroup _metaArea;
        private SortedDictionary<long, int> _addressToRowDict = new SortedDictionary<long, int>();

        private List<List<string>> _data;
        private int _selectedRow;
        private int _selectedCol;
        private long? _selectedOffset;
        private int _selectedSize;

        public void Init(SortedDictionary<long, int> tagBlockDict, FileSegmentGroup metaArea, IStreamManager streamManager, long baseOffset, int baseSize)
        {
            _metaArea = metaArea;

            int row = 0;
            int col = 0;

            _data = new List<List<string>>();
            _data.Add(new List<string>(16));

            tagBlockDict[baseOffset] = baseSize;

            IReader reader = streamManager.OpenRead();
            foreach (long key in tagBlockDict.Keys)
            {
                reader.SeekTo(key);

                _addressToRowDict[key] = row;
                col = 0;

                int size = tagBlockDict[key];
                byte[] buffer = reader.ReadBlock(size);

                for (int i = 0; i < size; i++)
                {
                    byte b = buffer[i];
                    ProcessByte(ref row, ref col, b);
                }

                row++;
                _data.Add(new List<string>(new string[] { "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--", "--" }));
                row++;
                _data.Add(new List<string>(16));
            }

            InitializeComponent();
            hexGrid.ItemsSource = _data;
        }

        private void ProcessByte(ref int row, ref int col, byte b)
        {
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

        public void SetFieldSelection(long? address, int size)
        {
            if (address == null || _selectedOffset != null)
            {
                ScrollToAndHighlightSelection(null);
                if (address == null)
                    return;
            }

            if (address.HasValue)
            {
                _selectedOffset = _metaArea.PointerToOffset(address.Value);
            }
            else
            {
                _selectedOffset = null;
                _selectedSize = 0;
                return;
            }

            _selectedSize = size;

            long baseOffset = GetBaseOffset(_selectedOffset.Value);
            int baseRow = _addressToRowDict[baseOffset];

            int fieldOffset = (int) (_selectedOffset.Value - baseOffset);
            _selectedRow = baseRow + (fieldOffset / 16);
            _selectedCol = fieldOffset % 16;

            ScrollToAndHighlightSelection(Brushes.LightBlue);
        }

        private long GetBaseOffset(long offset)
        {
            long baseOffset = -1;
            foreach (long key in _addressToRowDict.Keys)
            {
                if (offset < key)
                    break;

                baseOffset = key;
            }
            return baseOffset;
        }

        private void ScrollToAndHighlightSelection(Brush brush)
        {
            int rowCount = (_selectedCol + _selectedSize) / 16;
            int leftovers = (_selectedCol + _selectedSize) % 16;
            if (leftovers > 0)
                rowCount++;

            double? scrollOffset = null;
            for (int row = 0; row < rowCount; row++)
            {
                ContentPresenter rowPresenter = hexGrid.ItemContainerGenerator.ContainerFromIndex(_selectedRow + row) as ContentPresenter;
                if (rowPresenter == null)
                {
                    break;
                }

                ItemsControl rowControl = (ItemsControl)VisualTreeHelper.GetChild(rowPresenter, 0);

                if (scrollOffset == null)
                {
                    scrollOffset = rowPresenter.TranslatePoint(new System.Windows.Point(0, 0), hexGrid).Y;
                }

                int startingCol = 0;
                int endingCol = 0;
                GetStartingAndEndingCol(row, rowCount, leftovers, ref startingCol, ref endingCol);

                for (int col = startingCol; col < endingCol; col++)
                {
                    ContentPresenter valuePresenter = rowControl.ItemContainerGenerator.ContainerFromIndex(col) as ContentPresenter;
                    if (valuePresenter == null)
                        break; // what just happeeeeened

                    TextBlock textBlock = (TextBlock)VisualTreeHelper.GetChild(valuePresenter, 0);
                    textBlock.Background = brush;
                }
            }

            if (!scrollOffset.HasValue)
                return;

            if (scrollViewer.VerticalOffset < scrollOffset && 
                scrollOffset + 30 < scrollViewer.VerticalOffset + scrollViewer.ViewportHeight)
                return;

            scrollViewer.ScrollToVerticalOffset(scrollOffset.Value);
        }

        void GetStartingAndEndingCol(int row, int rowCount, int leftovers, ref int startingCol, ref int endingCol)
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
                    endingCol = leftovers;
            }
        }
    }
}
