using MathOperation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MathOperation.Common
{
    public class Randomizer
    {
        private Random rand;

        private int _DeltaHeader;
        public int DeltaHeader
        {
            get
            {
                if (MinValue == 0)
                    return Goal / 2;
                else
                    return Goal - MinValue;
            }
        }

        public List<int> MassRandNumbers { get; private set; }

        public int Count { get; private set; }

        public int Goal { get; set; }

        private int _MinValue;
        public int MinValue
        {
            get => _MinValue;
            set
            {
                if (_MinValue != value)
                    _MinValue = value;
            }
        }

        private int _MaxValue;
        public int MaxValue
        {
            get => _MaxValue;
            set
            {
                if (_MaxValue != value)
                    _MaxValue = value;
            }
        }

        public Randomizer(int count, int goal)
        {
            rand = new Random();
            Count = count;
            Goal = goal;
            FillListOfRandNumber();
        }

        public void GenerateCollection(int start, List<List<int>> result, params int[] elem)
        {
            if (result.Count > 0)
                return;

            int bufsumm = 0;
            for (int i = 0; i < elem.Length; i++)
            {
                bufsumm += elem[i];

                if (bufsumm > Goal)
                    return;
            }

            List<int> list = new List<int>();
            for (int i = start; i < MassRandNumbers.Count && result.Count == 0; i++)
            {
                if (bufsumm + MassRandNumbers[i] == Goal)
                {
                    for (int k = 0; k < elem.Length; k++)
                    {
                        list.Add(elem[k]);
                    }
                    list.Add(MassRandNumbers[i]);
                    result.Add(list);
                    return;
                }
                else
                {
                    int[] buff = new int[elem.Length + 1];
                    for (int z = 0; z < elem.Length; z++)
                    {
                        buff[z] = elem[z];
                    }
                    buff[buff.Length - 1] = MassRandNumbers[i];

                    GenerateCollection(i + 1, result, buff);
                }
            }
        }

        public void GenerateCollectionForHelp(int start, List<List<int>> result, params int[] elem)
        {
            int bufsumm = 0;
            for (int i = 0; i < elem.Length; i++)
            {
                bufsumm += elem[i];

                if (bufsumm > Goal)
                    return;
            }

            List<int> list = new List<int>();
            for (int i = start; i < MassRandNumbers.Count && result.Count < 5; i++)
            {
                if (bufsumm + MassRandNumbers[i] == Goal)
                {
                    for (int k = 0; k < elem.Length; k++)
                    {
                        list.Add(elem[k]);
                    }
                    list.Add(MassRandNumbers[i]);

                    if(!result.ContainsList(list))
                        result.Add(list);
                    return;
                }
                else
                {
                    int[] buff = new int[elem.Length + 1];
                    for (int z = 0; z < elem.Length; z++)
                    {
                        buff[z] = elem[z];
                    }
                    buff[buff.Length - 1] = MassRandNumbers[i];

                    GenerateCollectionForHelp(i + 1, result, buff);
                }
            }
        }

        public bool Run(List<List<int>> result)
        {
            GenerateCollection(0, result);

            return result.Count != 0;
        }

        public void FillListOfRandNumber()
        {
            MassRandNumbers = GenerateRandomNumbers();
        }

        private List<int> GenerateRandomNumbers()
        {
            return GenerateRandomNumber(Count);
        }

        private List<int> GenerateRandomNumber(int count)
        {
            var mass = new List<int>();
            for (int i = 0; i < count; i++)
            {
                mass.Add(rand.Next(1, Goal - DeltaHeader));
            }

            mass.Shuffle();
            return mass;
        }

        public List<int> GenerateDisapperedNumbers(List<int> disappearedList)
        {
            List<int> resultList = new List<int>();

            int removedCount = disappearedList.Count;

            foreach (var item in disappearedList)
            {
                MassRandNumbers.Remove(item);
            }

            var result = new List<List<int>>();
            Run(result);
            if (result.Count > 0)
            {
                result.Clear();
                resultList = GenerateRandomNumber(removedCount).ToList();
                MassRandNumbers.ToList().AddRange(resultList);
            }
            else
            {
                do
                {
                    result.Clear();
                    GetNewGoalValue();
                    Run(result);
                }
                while (result.Count != 0);
            }

            return resultList;
        }

        public int GenerateNumber()
        {
            int times = 60;
            var result = new List<List<int>>();
            int resultNumber = 0;

            while (!Run(result) && MassRandNumbers.Count != 0)
            {
                result.Clear();
                if (times-- == 0)
                    break;

                MassRandNumbers.Remove(resultNumber);

                resultNumber = GenerateRandNumber(MaxValue, MinValue);
                MassRandNumbers.Add(resultNumber);
            }
            if (resultNumber == 0)
            {
                resultNumber = GenerateRandNumber(MaxValue, MinValue);
                MassRandNumbers.Add(resultNumber);
            }

            return resultNumber;
        }

        private bool IsHasResolves()
        {
            var listOfList = new List<List<int>>();
            Run(listOfList);
            return listOfList.Count != 0;
        }

        public bool IsHasResolves(int start, params int[] elem)
        {
            int bufsumm = 0;
            for (int i = 0; i < elem.Length; i++)
            {
                bufsumm += elem[i];

                if (bufsumm > Goal)
                    return false;
            }

            List<int> list = new List<int>();

            for (int i = start; i < MassRandNumbers.Count; i++)
            {
                if (bufsumm + MassRandNumbers[i] == Goal)
                {
                    for (int k = 0; k < elem.Length; k++)
                    {
                        list.Add(elem[k]);
                    }
                    list.Add(MassRandNumbers[i]);
                    return true;
                }
                else
                {
                    int[] buff = new int[elem.Length + 1];
                    for (int z = 0; z < elem.Length; z++)
                    {
                        buff[z] = elem[z];
                    }
                    buff[buff.Length - 1] = MassRandNumbers[i];
                    IsHasResolves(i + 1, buff);
                }
            }

            return false;
        }

        public int GetNewGoalValue()
        {
            int times = 10;
            do
            {
                Goal = rand.Next(MinValue, MaxValue);
            }
            while (times-- > 0 && Run(new List<List<int>>()));

            return Goal;
        }

        public int GenerateRandNumber(int maxValue, int minValue = 1)
        {
            if (2 * DeltaHeader > MinValue)
                return rand.Next(MinValue - DeltaHeader / 2, MinValue + DeltaHeader / 2);

            return rand.Next(1, MinValue - DeltaHeader * 2);
        }


        public void ChangeFromToNumbers(int from, int to, List<int> list = null)
        {
            MaxValue = to;
            MinValue = from;

            if (list != null)
                MassRandNumbers = list;
        }

        public List<int> GenerateRandCollection(int count)
        {
            var tempList = new List<int>();
            for (int i = 0; i < count; i++)
            {
                tempList.Add(GenerateRandNumber(Goal - DeltaHeader));
            }

            return tempList;
        }

        public List<int> GenerateMissingNumber(List<int> list, int count)
        {
            RemoveNumberFromMass(list);
            var listOfNumber = new List<int>();
            while (!IsHasResolves())
            {
                listOfNumber.Clear();
                listOfNumber = GenerateRandCollection(count);
                MassRandNumbers.AddRange(listOfNumber);
            }

            return listOfNumber;
        }

        public void RemoveNumberFromMass(List<int> listToRemove)
        {
            listToRemove.ForEach(c => MassRandNumbers.Remove(c));
        }
    }
}
