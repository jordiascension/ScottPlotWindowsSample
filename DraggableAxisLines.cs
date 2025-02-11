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
    public partial class DraggableAxisLines : Form
    {
        public string Title => "Draggable Axis Lines";
        public string Description => "Demonstrates how to add mouse interactivity to plotted objects";

        //Line code
        AxisLine? PlottableBeingDragged = null;
        //Span code
        AxisSpanUnderMouse? SpanBeingDragged = null;

        public DraggableAxisLines()
        {
            InitializeComponent();

            // place axis lines on the plot
            formsPlot1.Plot.Add.Signal(Generate.Sin());
            formsPlot1.Plot.Add.Signal(Generate.Cos());

            //Line code
            var verticalLine = formsPlot1.Plot.Add.VerticalLine(23);
            verticalLine.IsDraggable = true;
            verticalLine.Text = "VLine";

            var horizontalLine = formsPlot1.Plot.Add.HorizontalLine(0.42);
            horizontalLine.IsDraggable = true;
            horizontalLine.Text = "HLine";

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
        }

        private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
        {
            //Line code
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
            //Line code
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
