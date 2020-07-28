using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class UndoViewModel : AbstractViewModel
    {
        public AddCellViewModel OldAddCell { get; private set; }
        public List<CellViewModel> OldSelectedList { get; set; }
        public List<CellViewModel> NewGeneratedList { get; set; }
        public CellViewModel[,] OldTable { get; set; }
        public CellViewModel[,] NewTable { get; set; }

        public bool IsEnabled { get; set; }

        private Color UnEnabledColor => StaticResources.AddCellUnClickableColor;
        private Color EnabledColor => StaticResources.GoalBackgroundColor;

        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                if(_Color != value)
                {
                    _Color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public UndoViewModel(CellViewModel[,] oldTable)
        {
            OldAddCell = new AddCellViewModel();
            OldSelectedList = new List<CellViewModel>();
            OldTable = oldTable;
        }

        public UndoViewModel(AddCellViewModel oldAddCell, CellViewModel[,] oldTable)
        {
            OldAddCell = oldAddCell;
            OldTable = oldTable;
        }

        public UndoViewModel(AddCellViewModel oldAddCell, List<CellViewModel> selectedList, CellViewModel[,] oldTable)
        {
            OldAddCell = oldAddCell;
            OldSelectedList = selectedList;
            OldTable = oldTable;
        }

        

        public void SetEnabled()
        {
            IsEnabled = true;
            Color = EnabledColor;
        }

        public void SetUnEnabled()
        {
            IsEnabled = false;
            Color = UnEnabledColor;
        }
    }
}
