using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class AddCellViewModel : AbstractViewModel
    {
        public string Text => "Add";

        private bool _IsClickable;

        public event EventHandler GenerateNumberAfteClickAddButton;
        private void RaiseEventGenerateButton(object obj, EventArgs args)
        {
            GenerateNumberAfteClickAddButton?.Invoke(obj, args);
        }

        public bool IsClickable
        {
            get { return _IsClickable; }
            set
            {
                if(_IsClickable != value)
                {
                    _IsClickable = value;
                    OnPropertyChanged("IsClickable");
                }
            }
        }

        private Color _ColorClickable;
        public Color ColorClickable
        {
            get { return _ColorClickable; }
            set
            {
                if(_ColorClickable != value)
                {
                    _ColorClickable = value;
                    OnPropertyChanged("ColorClickable");
                }
            }
        }


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

        public void SetClickableButton()
        {
            if(!IsClickable)
            {
                IsClickable = true;
                ColorClickable = StaticResources.GoalBackgroundColor;
            }
        }

        public void SetUnClickableButton()
        {
            if (IsClickable)
            {
                IsClickable = false;
                ColorClickable = StaticResources.AddCellUnClickableColor;
            }
        }

        private RelayCommand _ClickOnAddButton;
        public RelayCommand ClickOnAddButton
        {
            get
            {
                if (_ClickOnAddButton == null)
                    _ClickOnAddButton = new RelayCommand(
                        obj =>
                        {
                            RaiseEventGenerateButton(this, EventArgs.Empty);
                        });

                return _ClickOnAddButton;
            }
        }
    }
}
