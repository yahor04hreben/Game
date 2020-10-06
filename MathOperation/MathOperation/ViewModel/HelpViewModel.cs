using MathOperation.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MathOperation.ViewModel
{
    public class HelpViewModel : AbstractViewModel
    {
        public int  Count { get; private set; }
        public int DeltaSize { get; private set; }
        public bool NeedHelp { get; private set; }
        public List<List<int>>  ListOfButtonsHelp{ get; set; }
        public List<Button> Buttons { get; set; }
        public event EventHandler ClickOnHelpEvent;

        private readonly object locker = new object();
        public void RaiseTransformButton(object sender, EventArgs args)
        {
            ClickOnHelpEvent?.Invoke(sender, args);
        }

        public void ClearEvent()
        {
            ClickOnHelpEvent = null;
        }

        public HelpViewModel()
        {
            DeltaSize = 5;
            Buttons = new List<Button>();
            ListOfButtonsHelp = new List<List<int>>();

            ResetCount();
        }

        public void  ResetCount()
        {
            Count = -1;
        }

        public void IncreaseCount()
        {
            Count++;
        }
        private async void ScaleToButton(List<Button> buttons)
        {
            var tasks = new List<Task>();
            foreach(var b in buttons)
            {
                tasks.Add(ScaleButton(b));
                b.BorderColor = Color.Goldenrod;
            }

            await Task.WhenAll(tasks);
        }

         private async Task<bool> ScaleButton(Button b)
         {
             await b.ScaleTo(1.1, 200);
             await b.ScaleTo(0.9, 200);
             await b.ScaleTo(1.1, 200);

             lock(locker)       
             {
                  b.BorderColor = Color.Black;
             }

             return true;
         }

        public void ClearLists()
        {
            ListOfButtonsHelp.Clear();
            Buttons.Clear();
        }

        private RelayCommand _ClickOnHelp;
        public RelayCommand ClickOnHelp
        {
            get
            {
                if (_ClickOnHelp == null)
                    _ClickOnHelp = new RelayCommand(obj =>
                  {
                      lock(locker)
                      {
                          RaiseTransformButton(this, EventArgs.Empty);
                          ScaleToButton(Buttons);
                      }
                  });

                return _ClickOnHelp;
            }
        }
    }
}
