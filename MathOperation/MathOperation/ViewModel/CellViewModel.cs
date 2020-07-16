using MathOperation.Common;
using MathOperation.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class CellViewModel : AbstractViewModel
    {
        private CellModel cellModel;


        private Button _Button;
        public Button Button
        {
            get { return _Button; }
            set
            {
                if(_Button != value)
                {
                    _Button = value;
                    OnPropertyChanged("Button");
                }
            }
        }
        public int Number
        {
            get { return cellModel.Number; }
            set
            {
                if (cellModel.Number != value)
                {
                    cellModel.Number = value;
                    OnPropertyChanged("Number");
                }
            }
        }

        public int Row
        {
            get { return cellModel.Row; }
            set
            {
                if(cellModel.Row != value)
                {
                    cellModel.Row = value;
                    OnPropertyChanged("Row");
                }
            }
        }

        public int Column
        {
            get { return cellModel.Column; }
            set
            {
                if(cellModel.Column != value)
                {
                    cellModel.Column = value;
                    OnPropertyChanged("Column");
                }
            }
        }

        private int _SkipRow;
        public int SkipRow
        {
            get { return _SkipRow; }
            set
            {
                if(_SkipRow != value)
                {
                    _SkipRow = value;
                    OnPropertyChanged("SkipRow");
                }
            }
        }

        public int Index
        {
            get { return cellModel.Index; }
            set
            {
                if (cellModel.Index != value)
                {
                    cellModel.Index = value;
                    OnPropertyChanged("Index");
                }
            }
        }

        private bool _IsVisible;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if(_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        private float _Size;
        public float Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    OnPropertyChanged("Size");
                }

            }
        }

        private Color _Color;
        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public CellViewModel()
        {
            cellModel = new CellModel();
        }

        public CellViewModel(CellViewModel oldCell)
        {

        }
    }
}
