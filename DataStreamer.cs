using ScottPlot.Plottables;
using ScottPlot;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScottPlotSample
{
    public partial class DataStreamer : Form
    {
        public string Title => "Data Streamer";
        public string Description => "Plots live streaming data as a fixed-width line plot, " +
            "shifting old data out as new data comes in.";

        AxisLine? PlottableBeingDragged = null;
        //Span code
        AxisSpanUnderMouse? SpanBeingDragged = null;

        readonly System.Windows.Forms.Timer AddNewDataTimer = new() { Interval = 10, Enabled = true };
        readonly System.Windows.Forms.Timer UpdatePlotTimer = new() { Interval = 50, Enabled = true };

        readonly ScottPlot.Plottables.DataStreamer Streamer1;
        readonly ScottPlot.Plottables.DataStreamer Streamer2;

        readonly ScottPlot.DataGenerators.RandomWalker Walker1 = new(0);
        readonly ScottPlot.DataGenerators.RandomWalker Walker2 = new(1);

        readonly ScottPlot.Plottables.VerticalLine VLine;

        public DataStreamer()
        {
            InitializeComponent();

            Streamer1 = formsPlot1.Plot.Add.DataStreamer(2000);
            Streamer2 = formsPlot1.Plot.Add.DataStreamer(2000);
            VLine = formsPlot1.Plot.Add.VerticalLine(0, 2, ScottPlot.Colors.Red);

            var vl = formsPlot1.Plot.Add.VerticalLine(23);
            vl.IsDraggable = true;
            vl.Text = "VLine";

            var hl = formsPlot1.Plot.Add.HorizontalLine(0.42);
            hl.IsDraggable = true;
            hl.Text = "HLine";

            //Span code
            var verticalSpan = formsPlot1.Plot.Add.VerticalSpan(.23, .78);
            verticalSpan.IsDraggable = true;
            verticalSpan.IsResizable = true;

            var horizontalSpan = formsPlot1.Plot.Add.HorizontalSpan(12, 21);
            horizontalSpan.IsDraggable = true;
            horizontalSpan.IsResizable = true;

            formsPlot1.Refresh();

            // use events for custom mouse interactivity
            formsPlot1.MouseDown += FormsPlot1_MouseDown;
            formsPlot1.MouseUp += FormsPlot1_MouseUp;
            formsPlot1.MouseMove += FormsPlot1_MouseMove;

            // disable mouse interaction by default
            //formsPlot1.UserInputProcessor.Disable();

            // only show marker button in scroll mode
            btnMark.Visible = false;

            // setup a timer to add data to the streamer periodically
            AddNewDataTimer.Tick += (s, e) =>
            {
                int count = 5;

                // add new sample data
                Streamer1.AddRange(Walker1.Next(count));
                Streamer2.AddRange(Walker2.Next(count));

                // slide marker to the left
                formsPlot1.Plot.GetPlottables<Marker>()
                    .ToList()
                    .ForEach(m => m.X -= count);

                // remove off-screen marks
                formsPlot1.Plot.GetPlottables<Marker>()
                    .Where(m => m.X < 0)
                    .ToList()
                    .ForEach(m => formsPlot1.Plot.Remove(m));
            };

            // setup a timer to request a render periodically
            UpdatePlotTimer.Tick += (s, e) =>
            {
                if (Streamer1.HasNewData)
                {
                    formsPlot1.Plot.Title($"Processed {Streamer1.Data.CountTotal:N0} points");
                    VLine.IsVisible = Streamer1.Renderer is ScottPlot.DataViews.Wipe;
                    VLine.Position = Streamer1.Data.NextIndex * Streamer1.Data.SamplePeriod + Streamer1.Data.OffsetX;
                    formsPlot1.Refresh();
                }
            };

            // setup configuration actions
            btnWipeRight.Click += (s, e) =>
            {
                Streamer1.ViewWipeRight(0.1);
                Streamer2.ViewWipeRight(0.1);
                btnMark.Visible = false;
                formsPlot1.Plot.Remove<Marker>();
            };

            btnScrollLeft.Click += (s, e) =>
            {
                Streamer1.ViewScrollLeft();
                Streamer2.ViewScrollLeft();
                btnMark.Visible = true;
            };

            btnMark.Click += (s, e) =>
            {
                double x1 = Streamer1.Count;
                double y1 = Streamer1.Data.NewestPoint;
                var marker1 = formsPlot1.Plot.Add.Marker(x1, y1);
                marker1.Size = 20;
                marker1.Shape = MarkerShape.OpenCircle;
                marker1.Color = Streamer1.LineColor;
                marker1.LineWidth = 2;

                double x2 = Streamer2.Count;
                double y2 = Streamer2.Data.NewestPoint;
                var marker2 = formsPlot1.Plot.Add.Marker(x2, y2);
                marker2.Size = 20;
                marker2.Shape = MarkerShape.OpenCircle;
                marker2.Color = Streamer2.LineColor;
                marker2.LineWidth = 2;
            };

            rbManage.CheckedChanged += (s, e) =>
            {
                bool manageAxisLimits = ((RadioButton)s!).Checked;

                if (manageAxisLimits)
                {
                    formsPlot1.Plot.Axes.ContinuouslyAutoscale = false;
                    Streamer1.ManageAxisLimits = true;
                    Streamer2.ManageAxisLimits = true;
                }
                else
                {
                    formsPlot1.Plot.Axes.ContinuouslyAutoscale = true;
                    Streamer1.ManageAxisLimits = false;
                    Streamer2.ManageAxisLimits = false;
                }
            };
        }

        private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
        {
            var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
            if (lineUnderMouse is not null)
            {
                PlottableBeingDragged = lineUnderMouse;
                formsPlot1.UserInputProcessor.Disable(); // disable panning while dragging
            }

            //Span code
            var thingUnderMouse = GetSpanUnderMouse(e.X, e.Y);
            if (thingUnderMouse is not null)
            {
                SpanBeingDragged = thingUnderMouse;
                formsPlot1.UserInputProcessor.Disable(); // disable panning while dragging
            }
        }

        private void FormsPlot1_MouseUp(object? sender, MouseEventArgs e)
        {
            PlottableBeingDragged = null;
            //Span code
            SpanBeingDragged = null;
            formsPlot1.UserInputProcessor.Enable(); // enable panning again
            formsPlot1.Refresh();
        }

        private void FormsPlot1_MouseMove(object? sender, MouseEventArgs e)
        {
            // this rectangle is the area around the mouse in coordinate units
            CoordinateRect rect = formsPlot1.Plot.GetCoordinateRect(e.X, e.Y, radius: 10);

            if (PlottableBeingDragged is null)
            {
                // set cursor based on what's beneath the plottable
                var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
                if (lineUnderMouse is null) Cursor = Cursors.Default;
                else if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine) Cursor = Cursors.SizeWE;
                else if (lineUnderMouse.IsDraggable && lineUnderMouse is HorizontalLine) Cursor = Cursors.SizeNS;
            }
            else
            {
                // update the position of the plottable being dragged
                if (PlottableBeingDragged is HorizontalLine hl)
                {
                    hl.Y = rect.VerticalCenter;
                    hl.Text = $"{hl.Y:0.00}";
                }
                else if (PlottableBeingDragged is VerticalLine vl)
                {
                    vl.X = rect.HorizontalCenter;
                    vl.Text = $"{vl.X:0.00}";
                }
                formsPlot1.Refresh();
            }

            if (SpanBeingDragged is not null)
            {
                // currently dragging something so update it
                Coordinates mouseNow = formsPlot1.Plot.GetCoordinates(e.X, e.Y);
                SpanBeingDragged.DragTo(mouseNow);
                formsPlot1.Refresh();
            }
            else
            {
                // not dragging anything so just set the cursor based on what's under the mouse
                var spanUnderMouse = GetSpanUnderMouse(e.X, e.Y);
                if (spanUnderMouse is null) Cursor = Cursors.Default;
                else if (spanUnderMouse.IsResizingHorizontally) Cursor = Cursors.SizeWE;
                else if (spanUnderMouse.IsResizingVertically) Cursor = Cursors.SizeNS;
                else if (spanUnderMouse.IsMoving) Cursor = Cursors.SizeAll;
            }
        }

        private AxisLine? GetLineUnderMouse(float x, float y)
        {
            CoordinateRect rect = formsPlot1.Plot.GetCoordinateRect(x, y, radius: 10);

            foreach (AxisLine axLine in formsPlot1.Plot.GetPlottables<AxisLine>().Reverse())
            {
                if (axLine.IsUnderMouse(rect))
                    return axLine;
            }

            return null;
        }

        private AxisSpanUnderMouse? GetSpanUnderMouse(float x, float y)
        {
            CoordinateRect rect = formsPlot1.Plot.GetCoordinateRect(x, y, radius: 10);

            foreach (AxisSpan span in formsPlot1.Plot.GetPlottables<AxisSpan>().Reverse())
            {
                AxisSpanUnderMouse? spanUnderMouse = span.UnderMouse(rect);
                if (spanUnderMouse is not null)
                    return spanUnderMouse;
            }

            return null;
        }
    }
}
