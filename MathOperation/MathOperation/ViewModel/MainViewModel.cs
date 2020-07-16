using MathOperation.Common;
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
        public Randomizer Randomizer { get; private set; }

        public AddCellViewModel AddCellViewModel { get; private set; }

        public CellViewModel GoalViewModel { get; private set; }

        public ScoreVIewModel ScoreVIewModel { get; private set; }

        public event EventHandler GenerateListButton;

        private void RaiseGenerateListButton(object list, EventArgs eventArgs)
        {
            GenerateListButton?.Invoke(list, eventArgs);
        }

        public MainViewModel()
        {
            TableViewModel = new TableViewModel();
            Randomizer = new Randomizer(TableViewModel.GetCellCount, 30);
            TableViewModel.FillTable(Randomizer.MassRandNumbers.ToList());
            GoalViewModel = new CellViewModel { Number = Randomizer.Goal, Size = StaticResources.GoalTextSize};
            AddCellViewModel = new AddCellViewModel() { IsClickable = false, ColorClickable = StaticResources.AddCellUnClickable};
            ScoreVIewModel = new ScoreVIewModel() { Points = 0 };
            AddCellViewModel.GenerateNumberAfteClickAddButton += GenerateNumber;

            TableViewModel.CheckGoalValue += CheckGoalValue;
        }

        public void ReFillTable()
        {
            int goal = Randomizer.GenerateRandNumber(50, 30);
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
                    selectedList.ForEach(c => {
                        c.IsVisible = false;
                    });
                    Randomizer.RemoveNumberFromMass(selectedList.Select(c => c.Number).ToList());

                    RaiseGenerateListButton(selectedList , EventArgs.Empty);
                    AddCellViewModel.SetClickableButton();
                    ScoreVIewModel.AddPoints(selectedList.Select(c => c.Number).ToList());
                    selectedList.Clear();
                }
            }
        }


        private void GenerateNumber(object obj, EventArgs args)
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
    }
}
