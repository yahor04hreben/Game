﻿using MathOperation.Common;
using MathOperation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class UndoViewModel : AbstractViewModel
    {
        public int OldAddCell { get;  set; }

        public bool CanClear { get; set; }
        public List<CellViewModel> OldSelectedList { get; private set; }
        public List<CellViewModel> NewGeneratedList { get; set; }
        public CellViewModel[,] OldTable { get; set; }
        public CellViewModel[,] NewTable { get; set; }

        public int OldGoal;
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
            OldSelectedList = new List<CellViewModel>();
            NewGeneratedList = new List<CellViewModel>();
            OldTable = oldTable;
            CanClear = true;

            SetUnEnabled();
        }

        public UndoViewModel( List<CellViewModel> selectedList, CellViewModel[,] oldTable) : this(oldTable)
        {
            OldSelectedList = new List<CellViewModel>();
        }

        public UndoViewModel(AddCellViewModel oldAddCell, List<CellViewModel> selectedList, CellViewModel[,] oldTable) : this(selectedList, oldTable)
        {
            NewGeneratedList = new List<CellViewModel>();
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

        public void AddToOldSelectedList(List<CellViewModel> list)
        {
            OldSelectedList.Clear();
            foreach(var c in list)
            {
                OldSelectedList.Add(new CellViewModel(c));
            }
        }

        public Dictionary<CellViewModel, int> GetTranslateCells(int Row, int Column)
        {
            var resultDictionary = new Dictionary<CellViewModel, int>();
            var newTableList = NewTable.GetTableAsList(Row, Column).Where(c => c != null).ToList();
            var oldTableList = OldTable.GetTableAsList(Row, Column).Where(c => c != null).ToList();

            var skippedCells = newTableList.Where(c => oldTableList.Find(newC => newC.Index == c.Index && newC.SkipRow != c.SkipRow) != null).ToList();
            skippedCells.ForEach(c => resultDictionary.Add(c, oldTableList.First(newC => newC.Index == c.Index).SkipRow - c.SkipRow));
            
            return resultDictionary;
        }

        public List<CellViewModel> GetOldSelectedList(int Row, int Column)
        {

            var newTableList = NewTable.GetTableAsList(Row, Column).Where(c => c != null).ToList();
            var oldTableList = OldTable.GetTableAsList(Row, Column).Where(c => c != null).ToList();

            var oldSelectedList = oldTableList.Where(c => newTableList.FirstOrDefault(newC => newC.Index == c.Index) == null).ToList();
            return oldSelectedList;


            //if(OldSelectedList != null)
            //{
            //    var tempList = OldTable.GetTableAsList(Row, Column).ToList().Where(c => c != null).ToList().Where(c => OldSelectedList.FirstOrDefault(oC => oC.Index == c.Index) != null).ToList();
            //    tempList.ForEach(c =>
            //    {
            //        c.SkipRow = c.Row;
            //        c.IsVisible = true;
            //    });

            //    return tempList;
            //}

            //return new List<CellViewModel>();
        }
    }
}
