using ScottPlot;
using ScottPlot.AxisPanels;

namespace ScottPlotSample
{
    public partial class Form1 : Form
    {
        public string Title => "Data Logger";
        public string Description => "Plots live streaming data as a growing line plot.";

        readonly System.Windows.Forms.Timer AddNewDataTimer = new() { Interval = 1000, Enabled = true };
        readonly System.Windows.Forms.Timer UpdatePlotTimer = new() { Interval = 1000, Enabled = true };

        readonly ScottPlot.Plottables.DataLogger Logger1;
        readonly ScottPlot.Plottables.DataLogger Logger2;

        readonly ScottPlot.DataGenerators.RandomWalker Walker1 = new(2, multiplier: 1000);
        readonly ScottPlot.DataGenerators.RandomWalker Walker2 = new(1, multiplier: 1000);

        public Form1()
        {
            InitializeComponent();
            formsPlot1.UserInputProcessor.Disable();

            // create two loggers and add them to the plot
            // create two loggers and add them to the plot
            Logger1 = formsPlot1.Plot.Add.DataLogger();
            Logger2 = formsPlot1.Plot.Add.DataLogger();
            // use the right axis (already there by default) for the first logger
            RightAxis axis1 = (RightAxis)formsPlot1.Plot.Axes.Right;
            Logger1.Axes.YAxis = axis1;
            axis1.Color(Logger1.Color);

            // create and add a secondary right axis to use for the other logger
            RightAxis axis2 = formsPlot1.Plot.Axes.AddRightAxis();
            Logger2.Axes.YAxis = axis2;
            axis2.Color(Logger2.Color);

            int count = 30000;
            Logger1.Add(Walker1.Next(count));
            Logger2.Add(Walker2.Next(count));
            formsPlot1.Refresh();

            AddNewDataTimer.Tick += (s, e) =>
            {
                int count = 5;
                Logger1.Add(Walker1.Next(count));
                Logger2.Add(Walker2.Next(count));
            };

            UpdatePlotTimer.Tick += (s, e) =>
            {
                if (Logger1.HasNewData || Logger2.HasNewData)
                    formsPlot1.Refresh();
            };


            // wire our buttons to change the view modes of each logger
            //Full automatically adjusts the axis limits to show all data
            btnFull.Click += (s, e) => { Logger1.ViewFull(); Logger2.ViewFull(); };

            //Jump adjusts the axis limits to track the newest data as it comes
            //The axis limits will appear to "jump" when new data runs off the screen
            btnJump.Click += (s, e) => { Logger1.ViewJump(); Logger2.ViewJump(); };

            //Slide automatically adjusts the axis limits to track the newest data as it comes
            //Te axis limits will appear to "slide" continously as new data is added
            btnSlide.Click += (s, e) => { Logger1.ViewSlide(); Logger2.ViewSlide(); };

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnFull_Click(object sender, EventArgs e)
        {

        }
    }
}
