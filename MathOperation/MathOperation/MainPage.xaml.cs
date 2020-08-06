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
        public Grid grid { get; set; }

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


        private bool isSecondClick = false;
        private bool _IsStart;
        public bool IsStart
        {
            get => _IsStart;
            set
            {
                _IsStart = value;
                if(_IsStart && isSecondClick)
                {
                    grid.Children.Clear();
                    MainViewModel.RefreshMainModel(From, true);
                    ResetTableViewModel();
                    CreateMainView();
                }
            }
        }

        private void RaiseRefreshTimer(object sender, EventArgs args)
        {
            RefreshTimer?.Invoke(sender, args);
        }
        public MainPage(string from, string to, MainViewModel  main = null, Button timerButton = null, Grid grid = null, int goal = -1)
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
                isSecondClick = true;
                var randomList = grid == null ? null : main.Randomizer.MassRandNumbers.Select(i => i).ToList();
                MainViewModel = main;
                MainViewModel.Randomizer.ChangeFromToNumbers(From, To, randomList);
                MainViewModel.RefreshMainModel(goal <= 0 ? From : goal, IsStart);
                MainViewModel.TimerViewModeal.Resume();
                ResetTableViewModel();
            }
            
            MainViewModel.HelpViewModel.ClickOnHelpEvent += GetButtonHelp;

            if (grid != null)
                this.grid = grid;

            if (timerButton != null)
                this.timerButton = timerButton;

            mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            NavigationPage.SetHasNavigationBar(this, false);

            CreateMainView();

            IsStart = false;
        }

        private void ResetTableViewModel()
        {
            MainViewModel.TableViewModel.ResetEvent();
            MainViewModel.TableViewModel.ResetSelectedList();
            MainViewModel.TableViewModel.RemoveButton += RemoveButton;
            MainViewModel.TableViewModel.ReFillTable(MainViewModel.TableViewModel.GetTableAsList());
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

            grid.Children.Clear();
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
                    if(currentCell != null)
                    {
                        currentCell.SkipRow = i;
                        CreateButton(currentCell, 0, j);

                        listOfCells.Add(currentCell);
                    }
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
                Command = addButton.ClickOnAddButton,
                BorderWidth = 1
            };
            goalButton.Pressed += OnGoalButtonPressed;
            goalButton.Released += OnGoalButtonReleased;
            addButton.Button = goalButton;


            Binding bindingText = new Binding() { Source = goalViewModel, Path = "Number" };
            goalButton.SetBinding(Button.TextProperty, bindingText);

            Binding bindColor = new Binding() { Source = addButton, Path = "ColorClickable" };
            goalButton.SetBinding(Button.BackgroundColorProperty, bindColor);

            mainLayout.Children.Add(goalButton);
        }

        private void GenerateNewButtons(object twoList, EventArgs args)
        {
            List<CellViewModel> selectedCells = twoList as List<CellViewModel>;
            MainViewModel.SetOldTableForUndo(MainViewModel.TableViewModel.Table, EventArgs.Empty);
            MainViewModel.UndoViewModel.SetEnabled();

            if (selectedCells != null)
            {
                MainViewModel.UndoViewModel.NewGeneratedList.Clear();
                MainViewModel.UndoViewModel.AddToOldSelectedList(selectedCells);
                MainViewModel.TableViewModel.RemoveCellAndAddFallLDown(selectedCells, cellHeight);
            }
            else
            {
                var listInt = twoList as List<int>;
                var newCellViewModel = MainViewModel.TableViewModel.CreaderNewCell(listInt[0], listInt[2]);
                MainViewModel.UndoViewModel.NewGeneratedList.Add(newCellViewModel);
                newCellViewModel.SkipRow = listInt[1];
                newCellViewModel.Button = CreateButton(newCellViewModel, 0, listInt[2]);
                MainViewModel.UndoViewModel.AddToOldSelectedList(new List<CellViewModel> { newCellViewModel });
                MainViewModel.TableViewModel.RaiseTransleteCells(new List<CellViewModel>() { newCellViewModel }, cellHeight);
            }

            MainViewModel.SetNewTableForUndo(MainViewModel.TableViewModel.Table, EventArgs.Empty);
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
            var tempCellsVM = cellVM.ToList();
            while(tempCellsVM.Count != 0)
            {
                grid.Children.Remove(tempCellsVM[0].Button);
                tempCellsVM.RemoveAt(0);
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
            {
                timerButton = new Button()
                {
                    TextColor = StaticResources.ColorGoalText,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    HeightRequest = minorLayoutHight,
                    Margin = new Thickness(5),
                    CornerRadius = StaticResources.RadiusGoal,
                    BackgroundColor = StaticResources.GoalBackgroundColor,
                    Command = ClickOnTimerCommand
                };
               // timerButton.Clicked += ClickOnTimer;
            }
                

            Button setNumbers = new Button()
            {
                TextColor = StaticResources.ColorGoalText,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = minorLayoutHight,
                Margin = new Thickness(5),
                WidthRequest = StaticResources.Width * 0.3,
                CornerRadius = StaticResources.RadiusGoal,
                Text = "Undo",
                Command = ClickUndo
            };
            //setNumbers.Clicked += SetNumbersButton_Click;

            Binding colorBind = new Binding { Source = MainViewModel.UndoViewModel, Path = "Color" };
            setNumbers.SetBinding(Button.BackgroundColorProperty, colorBind);


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
            stopWatch = new Stopwatch();
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
            MainViewModel.UndoViewModel.NewGeneratedList.Clear();
            MainViewModel.UndoViewModel.CanClear = false;

            while (count < 20)
            {
                MainViewModel.GenerateNumber(this, EventArgs.Empty);
                count++;
            }
        }
        
        private async void GetButtonHelp(object sender, EventArgs args)
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
                var addButton = MainViewModel.AddCellViewModel.Button;

                await AnimationAddButton(addButton);
            }
        }

        private async Task<bool> AnimationAddButton(Button button)
        {
            button.BorderColor = Color.Goldenrod;
            button.BorderWidth = 8;

            await button.ScaleTo(1.05, 200);
            await button.ScaleTo(0.9, 200);
            await button.ScaleTo(1, 200);

            lock(locker)
            {
                button.BorderColor = Color.Black;
                button.BorderWidth = 1;
            }

            return true;
        }

        private RelayCommand _ClickUndo;
        public RelayCommand ClickUndo
        {
            get
            {
                if (_ClickUndo == null)
                    _ClickUndo = new RelayCommand(obj =>
                    {
                        if (MainViewModel.UndoViewModel.IsEnabled)
                        {
                            var cells = MainViewModel.UndoViewModel.GetTranslateCells(MainViewModel.Row, MainViewModel.Column);
                            
                            var list = new List<CellViewModel>();

                            foreach(var d in cells)
                            {
                                var cell = d.Key;
                                cell.SkipRow += d.Value;
                                cell.Row += d.Value;
                                list.Add(cell);
                            }
                            MainViewModel.TableViewModel.RaiseTransleteCells(list, cellHeight);
                            MainViewModel.TableViewModel.ReFillTable(cells.Keys.ToList());

                            var selectedList = MainViewModel.UndoViewModel.GetOldSelectedList(MainViewModel.Row, MainViewModel.Column);
                            if(selectedList.Count == 0 && MainViewModel.UndoViewModel.OldSelectedList.Count != 0)
                            {
                                var tempList = MainViewModel.UndoViewModel.OldSelectedList;
                                RemoveChildFromGrid(tempList);
                                MainViewModel.TableViewModel.ReFillTable(tempList, true);
                            }

                            foreach (var c in selectedList)
                            {
                                c.IsVisible = true;
                                c.Color = StaticResources.CellColor;
                                CreateButton(c, 0, c.Column);
                            }

                            var listToRemove = MainViewModel.UndoViewModel.NewGeneratedList;
                            RemoveButton(listToRemove);

                            MainViewModel.TableViewModel.RaiseTransleteCells(selectedList, cellHeight);
                            MainViewModel.TableViewModel.ReFillTable(selectedList);
                            MainViewModel.Randomizer.MassRandNumbers.AddRange(selectedList.Select(c => c.Number));
                            MainViewModel.GoalViewModel.Number = MainViewModel.UndoViewModel.OldGoal;
                            MainViewModel.Randomizer.Goal = MainViewModel.UndoViewModel.OldGoal;

                            MainViewModel.UndoViewModel.SetUnEnabled();
                            if (grid.Children.Count == 20)
                                MainViewModel.AddCellViewModel.SetUnClickableButton();
                        }
                    });

                return _ClickUndo;
            }
        }

        private async void ClickOnTimer(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new MenuPage(timerButton, MainViewModel));
        }

        private RelayCommand _ClickOnTimerCommand;
        public RelayCommand ClickOnTimerCommand
        {
            get
            {
                if (_ClickOnTimerCommand == null)
                    _ClickOnTimerCommand = new RelayCommand(obj =>
                    {
                        Navigation.PushAsync(new MenuPage(timerButton, MainViewModel, grid, MainViewModel.Randomizer.Goal));
                        MainViewModel.TimerViewModeal.Stop();
                        ReFillMinorLatoutChildren();
                    });
                return _ClickOnTimerCommand;
            }
        }
            
        private void ReFillMinorLatoutChildren()
        {
            Button[] tempChild = new Button[2];
            (mainLayout.Children[0] as StackLayout).Children.CopyTo(tempChild, 0);
            (mainLayout.Children[0] as StackLayout).Children.Clear();
            (mainLayout.Children[0] as StackLayout).Children.Add(timerButton);
            (mainLayout.Children[0] as StackLayout).Children.Add(tempChild[0]);
            (mainLayout.Children[0] as StackLayout).Children.Add(tempChild[1]);
        }

        private void RemoveButton(List<CellViewModel> cells)
        {
            MainViewModel.TableViewModel.RemoveCellFromTable(cells);
            RemoveChildFromGrid(cells);
            MainViewModel.Randomizer.RemoveNumberFromMass(cells.Select(c => c.Number).ToList());

            MainViewModel.AddCellViewModel.SetClickableButton();
            MainViewModel.UndoViewModel.NewGeneratedList.Clear();
            MainViewModel.UndoViewModel.CanClear = true;
        }
    }
}
