using MathOperation.Common;
using MathOperation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MathOperation.ViewModel
{
    public class TableViewModel : AbstractViewModel
    {
        public CellViewModel[,] Table { get; private set; }

        private object locker = new object();

        private List<CellViewModel> selectedList;
        public event EventHandler CheckGoalValue;
        public event EventHandler RemoveButton;
        public event EventHandler SetNewTableUndo;
        public event EventHandler SetOldTableUndo;

        private void RaiseCheckGaolValue(object list, EventArgs args)
        {
            CheckGoalValue?.Invoke(list, args);
        }

        public void RaiseSetNewTableUndo(object sender, EventArgs args)
        {
            SetNewTableUndo?.Invoke(sender, args);
        }

        public void RaiseSetOldTableUndo(object sender, EventArgs args)
        {
            SetOldTableUndo?.Invoke(sender, args);
        }

        public void ResetEvent()
        {
            RemoveButton = null;
        }

        public void RaiseRemoveButton(object list, EventArgs args)
        {
            RemoveButton?.Invoke(list, args);
        }

        public int Row => 5;
        public int Column => 4;

        public TableViewModel()
        {
            Table = new CellViewModel[Row, Column];
            selectedList = new List<CellViewModel>();
        }

        public void FillTable(List<int> numberOfList)
        {
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                    Table[i, j] = i * Column + j >= numberOfList.Count? null : CreateCellViewModedel(numberOfList[i * Column + j], i, j);
        }

        public int GetCellCount => Row * Column;

        public CellViewModel GetNumberByIndexes(int i, int j)
        {
            return Table[i, j];
        }

        private RelayCommand _SelectCell;
        public RelayCommand SelectCell
        {
            get
            {
                if (_SelectCell == null)
                    _SelectCell = new RelayCommand(cell =>
                    {
                        CellViewModel reallCell = cell as CellViewModel;
                        if (reallCell != null)
                        {
                            selectedList.ForEach(b => b.Button.BorderColor = Color.Black);
                            if (selectedList.Contains(reallCell))
                            {
                                selectedList.Remove(reallCell);
                                reallCell.Color = StaticResources.CellColor;
                            }
                            else
                            {
                                reallCell.Color = StaticResources.CellColorAfterPressed;
                                selectedList.Add(reallCell);
                            }

                            RaiseCheckGaolValue(selectedList, EventArgs.Empty);
                        }
                    });
                return _SelectCell;
            }
        }

        public async void RemoveCellAndAddFallLDown(List<CellViewModel> selectedCells, float cellHeight)
        {
            selectedCells.Sort((c, j) => c.Row.CompareTo(j.Row));
            selectedCells.Reverse();
            var CellsByColumn = from cell in selectedCells
                                group cell by cell.Column;

            var taskList = new List<Task>();
            
            foreach (var cells in CellsByColumn)
            {
                var columns = Table.GetColumn(cells.Key);
                bool IsFirstSkip = columns.Where(c => c.SkipRow != 0).Count() == 0;

                var columnRowBefore = columns.Select(c => c.SkipRow).ToList();
                cells.ForEach(cell => 
                {
                    columns.Where(c => c.Row < cell.Row).ForEach(c => c.SkipRow++);
                });
                columns.Where(c => cells.FirstOrDefault(newC => c.Index == newC.Index) != null).ForEach(c => c.IsVisible = false);

                for(int i = 0; i < columnRowBefore.Count(); i++)
                {
                    //if (!IsFirstSkip)
                        columns[i].Row += columns[i].SkipRow - columnRowBefore.ToList()[i];
                    //else
                    //    columns[i].Row += columns[i].SkipRow;
                }
                TranslateCellsFromTable(columns);
                taskList.AddRange(TranslateCells(columns, cellHeight));
            }

            await Task.WhenAll(taskList);
        }

        public List<Task> TranslateCells(List<CellViewModel> cellList, float cellHeight)
        {
            var taskList = new List<Task>();
            foreach (var cell in cellList)
                taskList.Add(FallDownCell(cell, cellHeight));

            return taskList;
        }

        public async void RaiseTransleteCells(List<CellViewModel> cellList, float cellHeight)
        {

            List<Task> listOfTask = TranslateCells(cellList, cellHeight);
            await Task.WhenAll(listOfTask);
        }

        private async Task<bool> FallDownCell(CellViewModel cell, float cellHeight)
        {
            await cell.Button.TranslateTo(0, cell.SkipRow * cellHeight, 700, Easing.CubicInOut);
            return true;
        }

        private CellViewModel CreateCellViewModedel(int number, int i, int j, int skip = 0)
        {
            int index = -1;
            if (Table.NotEmpty(Row, Column))
                index = Table.GetTableAsList(Row, Column).ToList().Where(c => c != null).Max(c => c.Index);

            return new CellViewModel()
            {
                Number = number,
                Row = i,
                Column = j,
                Index = index + 1,
                Color = StaticResources.CellColor,
                Size = StaticResources.CellTextSize,
                SkipRow = skip,
                IsVisible = true
            };
        }

        public List<CellViewModel> CreateNewCellsFromList(List<int> list)
        {
            var newCellList = new List<CellViewModel>();
            for (int i = 0; i < Column; i++)
            {
                int rowCountByColumn = Table.GetColumn(i).ToList().Count;
                int skipCell = Row - rowCountByColumn - 1;

                newCellList.Add(CreateCellViewModedel(list[i], 0, i, skipCell));
            }

            return newCellList;
        }

        public CellViewModel CreaderNewCell(int number, int column, int row = 0)
        {
            int rowCountByColumnIndex = Table.GetColumn(column).Count;
            int skipCell = row == 0 ? Row - rowCountByColumnIndex - 1 : row;

            var tempCell = CreateCellViewModedel(number, skipCell, column, skipCell);
            Table[skipCell, column] = tempCell;

            return tempCell;
        }

        public void RemoveCellFromTable(List<CellViewModel> cells)
        {
            foreach (var cell in cells)
            {
                Table[cell.Row, cell.Column] = null;
            }
        }

        public void TranslateCellsFromTable(List<CellViewModel> column)
        {
            if(column.Count != 0)
                Table.ClearColumn(column.First().Column, Row);

            column.ForEach(c =>
            {
                if (!c.IsVisible)
                {
                    RaiseRemoveButton(c.Button, EventArgs.Empty);
                }
                else
                {
                    //cell.Row = cell.Row + cell.SkipRow;
                    //cell.SkipRow = 0;
                    Table[c.Row, c.Column] = c;
                }
            });
        }

        public Point? GetLowestCellPoint()
        { 
            for(int i = Row - 1; i >= 0; i--)
                for(int j = 0; j < Column; j++)
                {
                    if (Table[i, j] == null)
                        return new Point(i, j);
                }

            return null;
        }

        public void ReFillTable(List<CellViewModel> listOfCell, bool toRemove = false)
        {
            foreach(var c in listOfCell)
            {
                if (!toRemove)
                    Table[c.Row, c.Column] = new CellViewModel(c);
                else
                    Table[c.Row, c.Column] = null;
            }
        }

        public CellViewModel FindItem(int n)
        {
            for(int i = 0; i < Row; i ++)
                for(int j = 0; j < Column; j++)
                {
                    if (Table[i, j] != null && Table[i, j].Number == n && !Table[i,j].IsSelected)
                    {
                        Table[i, j].IsSelected = true;
                        return Table[i, j];
                    }
                        
                }

            return null;
        }

        public void ResetSelectedList()
        {
            selectedList.Clear();
        }

        public List<CellViewModel> GetTableAsList()
        {
            return Table.GetTableAsList(Row, Column).ToList();
        }
    }
}
