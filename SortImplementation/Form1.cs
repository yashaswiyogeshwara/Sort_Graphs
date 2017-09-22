using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SortImplementation
{
    public partial class Form1 : Form
    {
        private SynchronizationContext _uiSyncContext;
        public Form1()
        {
            _uiSyncContext = SynchronizationContext.Current;
            InitializeComponent();
        }
        private System.Timers.Timer aTimer;

        private int recordIndex = 0;

        private int[] GetUnsortedNumbers()
        {
            string[] inputArr = this.textBox1.Text.Split(',');
            int[] unsortedRecords = new int[inputArr.Length];
            for (int i = 0; i < inputArr.Length; i++)
            {
                int.TryParse(inputArr[i], out unsortedRecords[i]);
            }
            return unsortedRecords;
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            MergeSort mergeSort = new MergeSort(this.chart1);

            recordIndex = 0;
            int[] unsortedArr = GetUnsortedNumbers().Length > 0 ? GetUnsortedNumbers() : new int[] { 7, 8, 5, 4, 12, 11, 14, 35, 2, 1, 5, 6, 8, 2, 3, 19, 10, 89, 87, 76, 56, 67, 57, 97, 67, 68, 79, 97, 89, 20, 10, 44, 55, 67, 87 };
            this.chart1.Series["sorting1"].Points.DataBindY(unsortedArr);
            int[] sortedArr = mergeSort.sort(unsortedArr);
            List<List<int>> Iterations = mergeSort.GetIterations();

            aTimer = new System.Timers.Timer(10000);

            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += (send, el) => OnTimedEvent(send, el, Iterations);

            // Set the Interval to 2 seconds (2000 milliseconds).
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
        }

        // to be done
        private void OnTimedEvent(object source, ElapsedEventArgs e, List<List<int>> recordList)
        {
            if (recordIndex < recordList.Count)
            {
                // this step is to make sure that the recordIndex value is created every time a new thread is created.
                int value = recordIndex;
                _uiSyncContext.Post(_ => this.chart1.Series["sorting1"].Points.DataBindY(recordList[value]), null);
                recordIndex++;
            }
            else
            {
                aTimer.Stop();
                return;
            }

        }

        private List<int> SortNumbers()
        {
            return null;
        }
        internal class MergeSort
        {

            private int _elemCount = 0;
            private int[] _sortedArr;
            private bool _doSort = true;
            public Chart _chart;
            private List<List<int>> _iterations = new List<List<int>>();
            public MergeSort(Chart chart)
            {
                _chart = chart;
                _chart.Refresh();
            }

            public List<List<int>> GetIterations()
            {
                return _iterations;
            }

            public int[] sort(int[] numbers)
            {
                _elemCount = numbers.Length;
                List<int[]> unsortedArr = new List<int[]>();

                foreach (int i in numbers)
                {
                    unsortedArr.Add(new int[] { i });
                }
                GetSortedArrList(unsortedArr);
                return _sortedArr;
            }

            private async void GetSortedArrList(List<int[]> arrList)
            {
                if (arrList[0].Length == _elemCount)
                {
                    _doSort = false;
                    _sortedArr = arrList[0];
                }
                if (_doSort)
                {
                    List<int[]> sortedArr = new List<int[]>();
                    int j = 0;
                    int i = 0;
                    for (; i < arrList.Count - 1; i = i + 2)
                    {
                        int temp = i;
                        int[] tempArr= await Task.Run(() => merge(arrList[temp], arrList[temp + 1]));
                        sortedArr.Add(tempArr);
                    }
                    if (arrList.Count % 2 > 0)
                    {
                        sortedArr.Add(arrList[i]);
                        i = 0;
                    }
                    LoadChartData(sortedArr);
                    GetSortedArrList(sortedArr);
                }

            }
            // merge not working

            public void LoadChartData(List<int[]> arr)
            {
                List<int> yNum = new List<int>();
                foreach (int[] intArr in arr)
                {
                    foreach (int i in intArr)
                    {
                        yNum.Add(i);
                    }
                }
                _iterations.Add(yNum);
            }

            private int[] merge(int[] leftArr, int[] rightArr)
            {
               
                    int[] arr = new int[leftArr.Length + rightArr.Length];
                    SortArr(leftArr, rightArr, ref arr, 0);
                    return arr;
                
            }

            private void SortArr(int[] leftArr, int[] rightArr, ref int[] arr, int k)
            {

                if (leftArr.Length == 0)
                {
                    foreach (int i in rightArr)
                    {
                        arr[k] = i;
                        k++;
                    }
                    return;
                }

                if (rightArr.Length == 0)
                {
                    foreach (int i in leftArr)
                    {
                        arr[k] = i;
                        k++;
                    }
                    return;
                }

                if (leftArr[0] < rightArr[0])
                {
                    arr[k] = leftArr[0];
                    leftArr = leftArr.Skip(1).ToArray();

                }
                else
                {
                    arr[k] = rightArr[0];
                    rightArr = rightArr.Skip(1).ToArray();
                }
                k++;
                SortArr(leftArr, rightArr, ref arr, k);
            }
        }

        private void chart1_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void chart1_Customize(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseHover(object sender, EventArgs e)
        {
            this.toolTip1.ToolTipTitle = "Info";
            this.toolTip1.Show("Enter numbers to be sortetd in comma seperated format", this.textBox1);
        }
    }
}
