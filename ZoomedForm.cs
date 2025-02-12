using ScottPlot;
using ScottPlot.AxisPanels;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace ScottPlotSample
{
    public partial class ZoomedForm : Form
    {
        // Declare the ScottPlot control a
        
        private CoordinateRect RectanguloSeleccionado => new CoordinateRect(coordInicio, coordActual);


        public string Title => "Select Data Points";

        public string Description => "Demonstrates how to use mouse events " +
            "to draw a rectangle around data points to select them";

        private bool mousePresionado = false;
        private Coordinates coordInicio;
        private Coordinates coordActual;

        private readonly ScottPlot.Plottables.Rectangle rectSeleccion;

        public ZoomedForm()
        {
            InitializeComponent();
            formsPlot1.UserInputProcessor.Disable();

            // Agregar datos de ejemplo (señales de seno y coseno)
            formsPlot1.Plot.Add.Signal(Generate.Sin());
            formsPlot1.Plot.Add.Signal(Generate.Cos());

            // Crear el rectángulo de selección, inicialmente invisible
            rectSeleccion = formsPlot1.Plot.Add.Rectangle(0, 0, 0, 0);
            rectSeleccion.FillStyle.Color = Colors.Red.WithAlpha(0.2f);  // Relleno rojo semitransparente
            rectSeleccion.IsVisible = false;

            // Asociar los eventos del mouse para gestionar la selección
            formsPlot1.MouseDown += FormsPlot1_MouseDown;
            formsPlot1.MouseMove += FormsPlot1_MouseMove;
            formsPlot1.MouseUp += FormsPlot1_MouseUp;

            formsPlot1.Refresh();


        }


        private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!checkBox1.Checked)
                return;
            // Deshabilitar la interacción por defecto (pan/zoom) mientras se dibuja el rectángulo
            formsPlot1.UserInputProcessor.Disable();
            mousePresionado = true;
            rectSeleccion.IsVisible = true;
            // Convertir la posición del mouse a coordenadas del plot
            coordInicio = formsPlot1.Plot.GetCoordinates(e.X, e.Y);      
        }

        private void FormsPlot1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!mousePresionado || !checkBox1.Checked)
                return;

            mousePresionado = false;
            // Registrar la posición final del mouse
            coordActual = formsPlot1.Plot.GetCoordinates(e.X, e.Y);
            rectSeleccion.CoordinateRect = RectanguloSeleccionado;

            // Calcular los límites a partir de las dos coordenadas:
            double xMin = Math.Min(coordInicio.X, coordActual.X);
            double xMax = Math.Max(coordInicio.X, coordActual.X);
            double yMin = Math.Min(coordInicio.Y, coordActual.Y);
            double yMax = Math.Max(coordInicio.Y, coordActual.Y);

            // Aplicar el zoom en la zona seleccionada
            formsPlot1.Plot.Axes.SetLimits(xMin, xMax, yMin, yMax);

            // Opcional: ocultar el rectángulo de selección tras el zoom
            rectSeleccion.IsVisible = false;
            // Rehabilitar la interacción por defecto (pan/zoom)
            formsPlot1.UserInputProcessor.Enable();
            formsPlot1.Refresh();
        }

        private void FormsPlot1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!mousePresionado || !checkBox1.Checked)
                return;

            // Actualizar la posición actual en coordenadas del plot
            coordActual = formsPlot1.Plot.GetCoordinates(e.X, e.Y);
            // Actualizar el rectángulo de selección para que siga al mouse
            rectSeleccion.CoordinateRect = RectanguloSeleccionado;
            formsPlot1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnFull_Click(object sender, EventArgs e)
        {
            //formsPlot1.Plot.Axes.Rules.Clear();
            //formsPlot1.Refresh();

            //Other solution to reset the plot
            /*formsPlot1.Plot.Axes.SetLimits(
             double.NaN, double.NaN,
            double.NaN,  double.NaN);
            formsPlot1.Refresh();*/ 
        }

        private void ZoomedForm_Load(object sender, EventArgs e)
        {

        }
    }
}
