using MathOperation.Common;
using MathOperation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class MainViewModel : AbstractViewModel
    {

        public TableViewModel TableViewModel { get; private set; }
        public HelpViewModel HelpViewModel { get; private set; }
        public Randomizer Randomizer { get; private set; }

        public UndoViewModel UndoViewModel { get; private set; }

        public AddCellViewModel AddCellViewModel { get; private set; }

        public CellViewModel GoalViewModel { get; private set; }

        public ScoreVIewModel ScoreVIewModel { get; private set; }

        private TimerViewModeal _TimerViewModeal;
        public TimerViewModeal TimerViewModeal
        {
            get
            {
                if (_TimerViewModeal == null)
                    _TimerViewModeal = new TimerViewModeal();

                return _TimerViewModeal;
            }
            set
            {
                _TimerViewModeal = value;
            }
        }

        public event EventHandler GenerateListButton;

        private void RaiseGenerateListButton(object list, EventArgs eventArgs)
        {
            GenerateListButton?.Invoke(list, eventArgs);
        }

        public MainViewModel(int number)
        {
            TableViewModel = new TableViewModel();
            HelpViewModel = new HelpViewModel();
            Randomizer = new Randomizer(TableViewModel.GetCellCount, number);
            TableViewModel.FillTable(Randomizer.MassRandNumbers.ToList());

            GoalViewModel = new CellViewModel { Number = Randomizer.Goal, Size = StaticResources.GoalTextSize };
            AddCellViewModel = new AddCellViewModel() { IsClickable = false, ColorClickable = StaticResources.AddCellUnClickableColor };
            ScoreVIewModel = new ScoreVIewModel() { Points = 0 };

            AddCellViewModel.GenerateNumberAfteClickAddButton += GenerateNumber;
            TableViewModel.CheckGoalValue += CheckGoalValue;
            TableViewModel.SetNewTableUndo += SetNewTableForUndo;
            TableViewModel.SetOldTableUndo += SetOldTableForUndo;

            UndoViewModel = new UndoViewModel(TableViewModel.Table);
        }

        public void RefreshMainModel(int fromNumber)
        {
            Randomizer.Goal = fromNumber;
            Randomizer.FillListOfRandNumber();
            TableViewModel.FillTable(Randomizer.MassRandNumbers.ToList());
            GoalViewModel.Number = fromNumber;
        }

        public void ReFillTable()
        {
            int goal = Randomizer.GetNewGoalValue();
            Randomizer.Goal = goal;
            GoalViewModel.Number = goal;
            Randomizer.FillListOfRandNumber();
            TableViewModel.FillTable(Randomizer.MassRandNumbers.ToList());
        }

        public int Row => TableViewModel.Row;
        public int Column => TableViewModel.Column;

        private void CheckGoalValue(object list, EventArgs args)
        {
            var selectedList = list as List<CellViewModel>;
            if(selectedList != null)
            {
                if(selectedList.Sum(s => s.Number) == GoalViewModel.Number)
                {
                    UndoViewModel.NewGeneratedList.Clear();
                    UndoViewModel.OldAddCell = GoalViewModel.Number;
                    selectedList.ForEach(c => {
                        c.IsVisible = false;
                    });
                    Randomizer.RemoveNumberFromMass(selectedList.Select(c => c.Number).ToList());

                    RaiseGenerateListButton(selectedList , EventArgs.Empty);
                    AddCellViewModel.SetClickableButton();
                    ScoreVIewModel.AddPoints(selectedList.Select(c => c.Number).ToList());
                    selectedList.Clear();

                    int randNumber = Randomizer.GetNewGoalValue();
                    GoalViewModel.Number = randNumber;
                }
            }
        }


        public void GenerateNumber(object obj, EventArgs args)
        {
            Point? point = TableViewModel.GetLowestCellPoint();
            if (point != null)
            {
                var p = point.Value;
                var newNumber = Randomizer.GenerateNumber();
                RaiseGenerateListButton(new List<int>() { newNumber, (int)p.X, (int)p.Y }, EventArgs.Empty);

                if(TableViewModel.GetLowestCellPoint() == null)
                    AddCellViewModel.SetUnClickableButton();
            }
            else
                AddCellViewModel.SetUnClickableButton();
        }

        private void SetNewTableForUndo(object sender, EventArgs args)
        {
            var newTable = sender as CellViewModel[,];
            UndoViewModel.NewTable = newTable.CopyTable(Row, Column);
        }

        private void SetOldTableForUndo(object sender, EventArgs args)
        {
            var oldTable = sender as CellViewModel[,];
            UndoViewModel.OldTable = oldTable.CopyTable(Row, Column);
        }
    }
}
