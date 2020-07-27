using MathOperation.Common;
using MathOperation.Helpers;
using MathOperation.View;
using MathOperation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MathOperation
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainViewModel MainViewModel { get;  set; }
        public Grid grid { get; private set; }

        public Label label { get; private set; }

        private object locker = new object();
        private DisplayInfo mainDisplayInfo;
        private float cellHeight;
        private float cellWidth;
        private float goalHeight => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.20);
        private float minorLayoutHight => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.06);
        private float whiteSpace => (float)((mainDisplayInfo.Height / mainDisplayInfo.Density) * 0.15);


        public int From;
        public int To;

        public Button timerButton { get; set; }
        public Button addButton { get; set; }

        public event EventHandler RefreshTimer;

        private bool animatiomInProgress = false;
        Stopwatch stopWatch = new Stopwatch();

        private void RaiseRefreshTimer(object sender, EventArgs args)
        {
            RefreshTimer?.Invoke(sender, args);
        }
        public MainPage(string from, string to, MainViewModel  main = null, Button timerButton = null, Grid grid = null)
        {
            if(from == string.Empty && to == string.Empty)
            {
                From = 30;
                To = 40;
            }

            InitializeComponent();
            ParseNumber(from, to);

            if (main == null)
            {
                MainViewModel = new MainViewModel(From);
                MainViewModel.Randomizer.ChangeFromToNumbers(From, To);
                MainViewModel.GenerateListButton += GenerateNewButtons;
                MainViewModel.TableViewModel.RemoveButton += RemoveButton;
            }
            else
            {
                MainViewModel = main;
                MainViewModel.Randomizer.ChangeFromToNumbers(From, To);
                MainViewModel.TimerViewModeal.Resume();
                MainViewModel.RefreshMainModel(From);
                MainViewModel.TableViewModel.ResetEvent();
                MainViewModel.TableViewModel.ResetSelectedList();
                MainViewModel.TableViewModel.RemoveButton += RemoveButton;
            }
            
            MainViewModel.HelpViewModel.ClickOnHelpEvent += GetButtonHelp;

            if (grid != null)
                this.grid = grid;

            if (timerButton != null)
                this.timerButton = timerButton;

            mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            NavigationPage.SetHasNavigationBar(this, false);

            CreateMainView();
        }

        private void ParseNumber(string from, string to)
        {
            if(from != string.Empty && to != string.Empty)
            {
                From = Int32.Parse(from);
                To = Int32.Parse(to);
            }
        }

        private void CreateGrid()
        {
            cellHeight = (float)((mainDisplayInfo.Height / mainDisplayInfo.Density - whiteSpace - goalHeight - minorLayoutHight) / MainViewModel.Row);
            cellWidth = (float)(mainDisplayInfo.Width / mainDisplayInfo.Density / MainViewModel.Column);

            if (grid == null)
            {
                grid = new Grid()
                {
                    RowSpacing = 0,
                    ColumnSpacing = 0
                };

                

                for (int i = 0; i < MainViewModel.Row; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = cellHeight });
                }

                for (int i = 0; i < MainViewModel.Column; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = cellWidth });
                }
            }
            else
            {
                grid.Children.Clear();
            }

            FillGrid();
        }

        private void FillGrid()
        {
            var listOfCells = new List<CellViewModel>();
            for (int i = 0; i < MainViewModel.Row; i++)
            {
                for (int j = 0; j < MainViewModel.Column; j++)
                {
                    CellViewModel currentCell = MainViewModel.TableViewModel.GetNumberByIndexes(i, j);
                    currentCell.SkipRow = i;
                    CreateButton(currentCell, 0, j);

                    listOfCells.Add(currentCell);
                }
            }

            MainViewModel.TableViewModel.RaiseTransleteCells(listOfCells, cellHeight);
        }

        private void CreateMainView()
        {
            mainLayout.Children.Clear();
            CreateHelpAndScoreButtons();
            CreateGoalLabel();
            CreateGrid();
            //mainLayout.Children.Add(label);
            mainLayout.Children.Add(grid);
        }

        private void CreateGoalLabel()
        {
            var goalViewModel = MainViewModel.GoalViewModel;
            var addButton = MainViewModel.AddCellViewModel;

            var goalButton = new Button
            {
                TextColor = StaticResources.ColorGoalText,
                IsVisible = true,
                FontSize = goalViewModel.Size,
                WidthRequest = (float)(mainDisplayInfo.Width / mainDisplayInfo.Density) - 10,
                HeightRequest = goalHeight,
                CornerRadius = StaticResources.RadiusGoal,
                Command = addButton.ClickOnAddButton
            };
            goalButton.Pressed += OnGoalButtonPressed;
            goalButton.Released += OnGoalButtonReleased;


            Binding bindingText = new Binding() { Source = goalViewModel, Path = "Number" };
            goalButton.SetBinding(Button.TextProperty, bindingText);

            Binding bindColor = new Binding() { Source = addButton, Path = "ColorClickable" };
            goalButton.SetBinding(Button.BackgroundColorProperty, bindColor);

            mainLayout.Children.Add(goalButton);
        }

        private void GenerateNewButtons(object twoList, EventArgs args)
        {
            List<CellViewModel> selectedCells = twoList as List<CellViewModel>;
            if(selectedCells != null)
            {
                MainViewModel.TableViewModel.RemoveCellAndAddFallLDown(selectedCells, cellHeight);
            }
            else
            {
                var listInt = twoList as List<int>;
                var newCellViewModel = MainViewModel.TableViewModel.CreaderNewCell(listInt[0], listInt[2]);
                newCellViewModel.SkipRow = listInt[1];
                newCellViewModel.Button = CreateButton(newCellViewModel, 0, listInt[2]);
                MainViewModel.TableViewModel.RaiseTransleteCells(new List<CellViewModel>() { newCellViewModel }, cellHeight);
            }
        }

        private Button CreateButton(CellViewModel currentCell,int i, int j)
        {
            Button btn = new Button
            {
                CommandParameter = currentCell,
                TextColor = Color.White,
                CornerRadius = 30,
                FontSize = currentCell.Size,
                BorderWidth = 6
            };
            currentCell.Button = btn;

            Binding bindText = new Binding() { Source = currentCell, Path = "Number" };
            Binding bindColor = new Binding() { Source = currentCell, Path = "Color" };
            Binding bindVisible = new Binding() { Source = currentCell, Path = "IsVisible", Mode = BindingMode.TwoWay };
            Binding bindMargin = new Binding() { Source = currentCell, Path = "Margin", Mode = BindingMode.TwoWay };
            Binding bindCommand = new Binding() { Source = MainViewModel.TableViewModel, Path = "SelectCell" };

            btn.SetBinding(Button.TextProperty, bindText);
            btn.SetBinding(Button.BackgroundColorProperty, bindColor);
            btn.SetBinding(Button.IsVisibleProperty, bindVisible);
            btn.SetBinding(Button.MarginProperty, bindMargin);
            btn.SetBinding(Button.CommandProperty, bindCommand);

            AddButtonToGridByIndexes(btn, i, j);

            return btn;
        }

        private void RemoveChildFromGrid(List<CellViewModel> cellVM)
        {
            while(cellVM.Count != 0)
            {
                grid.Children.Remove(cellVM[0].Button);
                cellVM.RemoveAt(0);
            }
        }

        private void AddButtonToGridByIndexes(Button btn, int i, int j)
        {
            btn.SetValue(Grid.RowProperty, i);
            btn.SetValue(Grid.ColumnProperty, j);

            grid.Children.Add(btn);
        }

        private void RemoveButton(object button, EventArgs eventArgs)
        {
            grid.Children.Remove(button as Button);

            if (grid.Children.Count == 0)
                RefrashGrid();
        }

        private void CreateHelpAndScoreButtons()
        {
            AddCellViewModel addCell = MainViewModel.AddCellViewModel;
            TimerViewModeal timeVM = MainViewModel.TimerViewModeal;

            StackLayout minorLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = minorLayoutHight,
                Padding = new Thickness(0)
            };

            var helpButton = new Button()
            {
                Text = "Help",
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                CornerRadius = StaticResources.RadiusGoal,
                BackgroundColor = StaticResources.GoalBackgroundColor,
                Command = MainViewModel.HelpViewModel.ClickOnHelp
            };

            if(timerButton == null)
                timerButton = new Button()
                {
                    TextColor = StaticResources.ColorGoalText,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    HeightRequest = minorLayoutHight,
                    Margin = new Thickness(5),
                    CornerRadius = StaticResources.RadiusGoal,
                    BackgroundColor = StaticResources.GoalBackgroundColor
                };

            Button setNumbers = new Button()
            {
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                WidthRequest = StaticResources.Width * 0.3,
                CornerRadius = StaticResources.RadiusGoal,
                BackgroundColor = StaticResources.GoalBackgroundColor,
                Text = "Set Range"
            };
            setNumbers.Clicked += SetNumbersButton_Click;
            


            minorLayout.Children.Add(timerButton);
            minorLayout.Children.Add(setNumbers);
            minorLayout.Children.Add(helpButton);

            mainLayout.Children.Add(minorLayout);
        }

        private void RefrashGrid()
        {
            grid.Children.Clear();
            MainViewModel.ReFillTable();
            FillGrid();
        }

        private async void SetNumbersButton_Click(object sender, EventArgs e)
        {
            MainViewModel.TimerViewModeal.Stop();
            await Navigation.PushModalAsync(new SetNumbersPage(this));
        }

        private void OnGoalButtonPressed(object sender, EventArgs args)
        {
            stopWatch.Start();
            animatiomInProgress = true;
        }

        private void OnGoalButtonReleased(object sender, EventArgs args)
        {
            if(stopWatch.ElapsedMilliseconds > 1000)
            {
                FillFullWhiteSpace();
            }

            animatiomInProgress = false;
            stopWatch.Stop();
        }

        private void FillFullWhiteSpace()
        {
            int count = MainViewModel.TableViewModel.Table.LengthTable(MainViewModel.TableViewModel.Row);
            while (count < 20)
            {
                MainViewModel.GenerateNumber(this, EventArgs.Empty);
                count++;
            }
        }
        
        private void GetButtonHelp(object sender, EventArgs args)
        {
            var helpViewModel = sender as HelpViewModel;
            var buttonsViewModel = new List<CellViewModel>();
            var result = new List<List<int>>();
            var buttons = new List<Button>();

            MainViewModel.Randomizer.GenerateCollection(0, result);
            if(result.Count != 0)
            {
                result[0].ForEach(n => buttonsViewModel.Add(MainViewModel.TableViewModel.FindItem(n)));
                buttonsViewModel.ForEach(b => b.IsSelected = false);

                buttonsViewModel.ForEach(b => buttons.Add(b.Button));
                helpViewModel.Buttons = buttons;
            }
            else
            {

            }
        }
    }
}
