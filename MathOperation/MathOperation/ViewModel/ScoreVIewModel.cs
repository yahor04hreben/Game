using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class ScoreVIewModel : AbstractViewModel
    {
        private double Cof => 1.3; 
        private string FirstPartoFText => "Score: ";

        private int _Points;
        public int Points
        {
            get { return _Points; }
            set
            {
                if(_Points != value)
                {
                    _Points = value;
                    OnPropertyChanged("Points");
                }
            }
        }

        public void AddPoints(List<int> valurToAdd)
        {
            Points += (int)(valurToAdd.Sum() * valurToAdd.Count * 1.3);
            Text = FirstPartoFText + Points.ToString();
        }

        private string _Text;
        public string Text
        {
            get { return FirstPartoFText + Points.ToString(); }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }
    }
}
