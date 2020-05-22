using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CSCI422_Project.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields and Properties
        public int GridSize { get; set; }

        public int WindowHeight { get; set; }

        public int WindowWidth { get; set; }

        public int Distance { get; set; }

        public int Number { get; set; }

        private List<int> _rowNumber;
        public List<int> RowNumber
        {
            get { return _rowNumber; }
            set { Set(() => RowNumber, ref _rowNumber, value); }
        }

        private List<int> _columnNumber;
        public List<int> ColumnNumber
        {
            get { return _columnNumber; }
            set { Set(() => ColumnNumber, ref _columnNumber, value); }
        }

        public bool IsCellPressed { get; set; }

        private CellViewModel _destination;
        public CellViewModel Destination
        {
            get { return _destination; }
            set { Set(() => Destination, ref _destination, value); }
        }

        private CellViewModel _source;
        public CellViewModel Source
        {
            get { return _source; }
            set { Set(() => Source, ref _source, value); }
        }

        private ObservableCollection<CellViewModel> _cells;
        public ObservableCollection<CellViewModel> Cells
        {
            get { return _cells; }
            set { Set(() => Cells, ref _cells, value); }
        }

        #endregion

        #region Relay Commands

        private RelayCommand<CellViewModel> _pressCellCommand;
        public RelayCommand<CellViewModel> PressCellCommand
        {
            get
            {
                return _pressCellCommand ?? (_pressCellCommand = new RelayCommand<CellViewModel>(cell =>
                {
                    if (!IsCellPressed)
                    {
                        if (cell.IsObstacle)
                        {
                            cell.Colour = new SolidColorBrush(Colors.Gray);
                            return;
                        }
                        cell.Colour = new SolidColorBrush(Colors.Green);
                        Source = cell;
                        IsCellPressed = !IsCellPressed;
                    }
                    else
                    {
                        if(!cell.IsObstacle) cell.Colour = new SolidColorBrush(Colors.Red);
                        Destination = cell;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        Distance = BFS(Source, Destination);
                        DrawPath(Destination, Source);
                        sw.Stop();
                        /*MessageBox.Show($"The shortest path distance is {Distance}. Execution time is {sw.ElapsedMilliseconds} milliseconds.");
                        foreach (var item in Cells)
                        {
                            item.isVisited = false;
                            item.Distance = 0;
                            if (item.IsTemporalPath || item.IsShortestPath) item.Colour = new SolidColorBrush(Colors.Gray);
                        }
                        ResetTempVariables();*/
                    }
                }));
            }
        }

        private RelayCommand<CellViewModel> _createObstacleCommand;
        public RelayCommand<CellViewModel> CreateObstacleCommand
        {
            get
            {
                return _createObstacleCommand ?? (_createObstacleCommand = new RelayCommand<CellViewModel>(cell =>
                {
                    cell.Colour = new SolidColorBrush(Colors.DarkBlue);
                }));
            }
        }

        private RelayCommand _resetMazeCommand;
        public RelayCommand ResetMazeCommand
        {
            get
            {
                return _resetMazeCommand ?? (_resetMazeCommand = new RelayCommand(() =>
                {
                    foreach (var item in Cells)
                    {
                        item.Colour = new SolidColorBrush(Colors.Gray);
                    }
                    ResetTempVariables();
                }));
            }
        }

        private RelayCommand _constructMazeCommand;
        public RelayCommand ConstructMazeCommand
        {
            get
            {
                return _constructMazeCommand ?? (_constructMazeCommand = new RelayCommand(() =>
                {
                    ConstructMaze(GridSize);
                    ResetTempVariables();
                }));
            }
        }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            GridSize = 10;
            WindowHeight = 600;
            WindowWidth = 600;
            IsCellPressed = false;
            Number = 0;

            RowNumber = new List<int>();
            ColumnNumber = new List<int>();

            RowNumber.Add(-1);
            RowNumber.Add(0);
            RowNumber.Add(0);
            RowNumber.Add(1);
            ColumnNumber.Add(0);
            ColumnNumber.Add(-1);
            ColumnNumber.Add(1);
            ColumnNumber.Add(0);

            ConstructMaze(GridSize);
        }

        #endregion

        #region Private Methods

        private void ConstructMaze(int gridSize)
        {
            Cells = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Cells.Add(new CellViewModel(WindowWidth / gridSize, WindowWidth / gridSize, j, i, new SolidColorBrush(Colors.Gray)));
                }
            }
        }

        private bool IsValid(int row, int column)
        {
            return (row >= 0) && (row < GridSize) &&
           (column >= 0) && (column < GridSize);
        }

        private int BFS(CellViewModel source, CellViewModel destination)
        {
            if (destination.IsObstacle) return -1;
            List<CellViewModel> path = new List<CellViewModel>();
            source.isVisited = true;
            path.Add(source);

            while (path.Any())
            {
                CellViewModel currentCell = path.FirstOrDefault();

                if(currentCell == destination)
                {
                    return currentCell.Distance;
                }

                path.Remove(currentCell);

                for (int i = 0; i < 4; i++)
                {
                    int row = currentCell.XIndex + RowNumber[i];
                    int column = currentCell.YIndex + ColumnNumber[i];

                    CellViewModel newCell = Cells.FirstOrDefault(j => j.XIndex == row && j.YIndex == column);

                    if (IsValid(row, column) && !newCell.IsObstacle && !newCell.isVisited)
                    {
                        newCell.isVisited = true;
                        newCell.Distance = currentCell.Distance + 1;
                        if (newCell != Destination)
                        {
                            newCell.Colour = new SolidColorBrush(Colors.LightGreen);
                            Number++;
                        }
                        path.Add(newCell);
                    }
                }
            }

            return -1;
        }

        //private int BruteAlgorithm(CellViewModel source, CellViewModel destination)
        //{
        //    if (destination.IsObstacle) return -1;
        //    Stack<CellViewModel> path = new Stack<CellViewModel>();
        //    source.isVisited = true;
        //    int xDifference = destination.XIndex - source.XIndex;//if > 0 go down, else go up
        //    int yDifference = destination.YIndex - source.YIndex;//if > 0 go right else go left

        //    CellViewModel currentCell = source;
        //    path.Push(currentCell);


        //    while(currentCell != destination)
        //    {
        //        if(xDifference > 0)
        //        {
        //            if(Cells.FirstOrDefault(i => i.))
        //        }
        //    }

        //    return -1;
        //}

        private void DrawPath(CellViewModel destination, CellViewModel source)
        {
            CellViewModel currentCell = destination;

            for (int i = destination.Distance; i > 0; i--)
            {
                for (int k = 0; k < 4; k++)
                {
                    int row = currentCell.XIndex + RowNumber[k];
                    int column = currentCell.YIndex + ColumnNumber[k];

                    CellViewModel newCell = Cells.FirstOrDefault(j => j.XIndex == row && j.YIndex == column);

                    if (newCell == source) return;

                    if(IsValid(row, column) && newCell.Distance != 0)
                    {
                        if (newCell.Distance == (i - 1))
                        {
                            newCell.Colour = new SolidColorBrush(Colors.Orange);
                            currentCell = newCell;
                        }
                    }
                }
            }
        }

        private void ResetTempVariables()
        {
            if (Destination != null && !Destination.IsObstacle) Destination.Colour = new SolidColorBrush(Colors.Gray);
            Destination = null;
            if(Source != null) Source.Colour = new SolidColorBrush(Colors.Gray);
            Source = null;
            IsCellPressed = false;
        }

        #endregion
    }
}