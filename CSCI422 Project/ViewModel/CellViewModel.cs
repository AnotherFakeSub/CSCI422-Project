using GalaSoft.MvvmLight;
using System.Windows.Media;

namespace CSCI422_Project.ViewModel
{
    public class CellViewModel : ViewModelBase
    {

        #region Fields and Properties

        public double CellWidth { get; set; }

        public double CellHeight { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public int XIndex { get; set; }

        public int YIndex { get; set; }

        private int _distance;
        public int Distance
        {
            get { return _distance; }
            set
            {
                Set(() => Distance, ref _distance, value);
                RaisePropertyChanged("StringDistance");
            }
        }

        public string StringDistance { get { return Distance == 0 ? "" : Distance.ToString(); } }

        public bool isVisited { get; set; }

        private bool _isObstacle;
        public bool IsObstacle
        {
            get { return _isObstacle; }
            set { Set(() => IsObstacle, ref _isObstacle, value); }
        }

        private bool _isTemporalPath;
        public bool IsTemporalPath
        {
            get { return _isTemporalPath; }
            set { Set(() => IsTemporalPath, ref _isTemporalPath, value); }
        }

        private bool _isShortestPath;
        public bool IsShortestPath
        {
            get { return _isShortestPath; }
            set { Set(() => IsShortestPath, ref _isShortestPath, value); }
        }

        private SolidColorBrush _colour;
        public SolidColorBrush Colour
        {
            get { return _colour; }
            set
            {
                Set(() => Colour, ref _colour, value);
                if (value.Color == new SolidColorBrush(Colors.DarkBlue).Color) IsObstacle = true;
                else if (value.Color == new SolidColorBrush(Colors.LightGreen).Color) IsTemporalPath = true;
                else if (value.Color == new SolidColorBrush(Colors.Orange).Color) IsShortestPath = true;
                else
                {
                    IsObstacle = false;
                    IsTemporalPath = false;
                    IsShortestPath = false;
                }
            }
        }

        #endregion

        #region Constructors

        public CellViewModel(double cellWidth, double cellHeight, int row, int column, SolidColorBrush colour)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            X = column * cellWidth;
            Y = row * cellHeight;
            XIndex = row;
            YIndex = column;
            Colour = colour;
            Distance = 0;
            IsObstacle = false;
            IsTemporalPath = false;
            IsShortestPath = false;
        }

        #endregion
    }
}
