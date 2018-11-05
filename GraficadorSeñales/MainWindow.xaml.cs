using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        double amplitudMaxima = 1;
        Señal señal;
        Señal segundaSeñal;
        Señal señalResultado;

        public MainWindow()
        {
            InitializeComponent();



        }

        private void BotonGraficar_Click(object sender, RoutedEventArgs e)
        {
            double tiempoInicial = double.Parse(txtTiempoInicial.Text);
            double tiempoFinal = double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo = double.Parse(txtFrecuenciadeMuestreo.Text);

            

            switch (cbTipoSeñal.SelectedIndex)
            {
                case 0:
                    double amplitud = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion.Children[0]).txtAmplitud.Text);
                    double fase = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion.Children[0]).txtFase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion.Children[0]).txtFrecuencia.Text);

                    señal = new SeñalSenoidal(amplitud, fase, frecuencia);
                    break;

                case 1:
                    señal = new SeñalRampa();
                    break;

                case 2:
                    double alpha = double.Parse(((ConfiguracionSeñalExponencial)panelConfiguracion.Children[0]).txtAlpha.Text);
                    señal = new SeñalExponencial(alpha);
                    break;
                case 3:
                    señal = new SeñalRectangular();
                    break;
                default: señal = null; break;



            }

            switch (cbTipoSeñal_SegundaSeñal.SelectedIndex)
            {
                case 0:
                    double amplitud = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion_SegundaSeñal.Children[0]).txtAmplitud.Text);
                    double fase = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion_SegundaSeñal.Children[0]).txtFase.Text);
                    double frecuencia = double.Parse(((ConfiguracionSeñalSenoidal)panelConfiguracion_SegundaSeñal.Children[0]).txtFrecuencia.Text);

                    segundaSeñal = new SeñalSenoidal(amplitud, fase, frecuencia);
                    break;

                case 1:
                    segundaSeñal = new SeñalRampa();
                    break;

                case 2:
                    double alpha = double.Parse(((ConfiguracionSeñalExponencial)panelConfiguracion_SegundaSeñal.Children[0]).txtAlpha.Text);
                    segundaSeñal = new SeñalExponencial(alpha);
                    break;
                case 3:
                    segundaSeñal = new SeñalRectangular();
                    break;
                default:
                    segundaSeñal = null; break;
            }

            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;

            //segunda señal
            segundaSeñal.TiempoInicial = tiempoInicial;
            segundaSeñal.TiempoFinal = tiempoFinal;
            segundaSeñal.FrecuenciaMuestreo = frecuenciaMuestreo;



            señal.construirSeñalDigital();
            segundaSeñal.construirSeñalDigital();

            //Escalar
            double factorEscala = double.Parse(txtEscalaAmplitud.Text);
            señal.escalar(factorEscala);
            señal.actualizarAmplitudMaxima();

            //Truncar
            if ((bool)cb_Umbral.IsChecked)
            {
                double factorTruncar = double.Parse(txtUmbral.Text);
                señal.truncar(factorTruncar);
            }

            //segunda Señal

            //Escalar
            double factorEscala2 = double.Parse(txtEscalaAmplitud.Text);
            segundaSeñal.escalar(factorEscala2);
            segundaSeñal.actualizarAmplitudMaxima();

            //Truncar
            if ((bool)cb_Umbral_SegundaSeñal.IsChecked)
            {
                double factorTruncar2 = double.Parse(txtUmbral_SegundaSeñal.Text);
                segundaSeñal.truncar(factorTruncar2);
            }

            señal.actualizarAmplitudMaxima();
            segundaSeñal.actualizarAmplitudMaxima();

            amplitudMaxima = señal.AmplitudMaxima;
            if (segundaSeñal.AmplitudMaxima > amplitudMaxima)
            {
                amplitudMaxima = segundaSeñal.AmplitudMaxima;
            }


            plnGrafica.Points.Clear();
            plnGraficaDos.Points.Clear();


            lblmplitudMaximaPositivaY.Text = amplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY.Text = "-" + amplitudMaxima.ToString("F");



            if (señal != null)
            {
                //Recorre una coleccion o arreglo 
                foreach (Muestra muestra in señal.Muestras)
                {
                    plnGrafica.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, (muestra.Y / amplitudMaxima * ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));
                }

            }

            //Segunda señal
            if (segundaSeñal != null)
            {
                //Recorre una coleccion o arreglo 
                foreach (Muestra muestra in segundaSeñal.Muestras)
                {
                    plnGraficaDos.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, (muestra.Y / amplitudMaxima * ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));
                }

            }

            //Recorre una coleccion o arreglo 

            plnEjeX.Points.Clear();
            //Punto del principio 
            plnEjeX.Points.Add(new Point(0, (scrContenedor.Height / 2)));
            //Punto del fin
            plnEjeX.Points.Add(new Point((tiempoFinal - tiempoInicial) * scrContenedor.Width, (scrContenedor.Height / 2)));



            plnEjeY.Points.Clear();
            //Punto del principio 
            plnEjeY.Points.Add(new Point((0 - tiempoInicial) * scrContenedor.Width, (señal.AmplitudMaxima * ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));
            //Punto del fin
            plnEjeY.Points.Add(new Point((0 - tiempoInicial) * scrContenedor.Width, (-señal.AmplitudMaxima * ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));


           
        }

		private void cb_EscalaAmplitud_Checked(object sender, RoutedEventArgs e)
		{
			if (cb_EscalaAmplitud.IsChecked == true)
			{
				txtEscalaAmplitud.IsEnabled = true;
			}
			else
			{
				txtEscalaAmplitud.IsEnabled = false;
			}
		}

        private void cb_EscalaAmplitud_SegundaSeñal_Checked(object sender, RoutedEventArgs e)
        {
            if (cb_EscalaAmplitud_SegundaSeñal.IsChecked == true)
            {
                txtEscalaAmplitud_SegundaSeñal.IsEnabled = true;
            }
            else
            {
                txtEscalaAmplitud_SegundaSeñal.IsEnabled = false;
            }
        }

        private void cb_DesplazamientoY_Checked(object sender, RoutedEventArgs e)
		{
			if (cb_DesplazamientoY.IsChecked == true)
			{
				txtDesplazamientoY.IsEnabled = true;
			}
			else
			{
				txtDesplazamientoY.IsEnabled = false;
			}
		}

        private void cb_DesplazamientoY_SegundaSeñal_Checked(object sender, RoutedEventArgs e)
        {
            if (cb_DesplazamientoY_SegundaSeñal.IsChecked == true)
            {
                txtDesplazamientoY_SegundaSeñal.IsEnabled = true;
            }
            else
            {
                txtDesplazamientoY_SegundaSeñal.IsEnabled = false;
            }
        }

        private void cb_Umbral_Checked(object sender, RoutedEventArgs e)
        {
            if (cb_Umbral.IsChecked == true)
            {
                txtUmbral.IsEnabled = true;
            }
            else
            {
                txtUmbral.IsEnabled = false;
            }
        }

        private void cb_Umbral_SegundaSeñal_Checked(object sender, RoutedEventArgs e)
        {
            if (cb_Umbral_SegundaSeñal.IsChecked == true)
            {
                txtUmbral_SegundaSeñal.IsEnabled = true;
            }
            else
            {
                txtUmbral_SegundaSeñal.IsEnabled = false;
            }
        }

        private void cbTipoSeñal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracion.Children.Clear();
            switch(cbTipoSeñal.SelectedIndex)
            {
                case 0: //Senoidal
                    panelConfiguracion.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;
                case 1://Rampa
                    break;
                case 2://Exponencial
                    panelConfiguracion.Children.Add(new ConfiguracionSeñalExponencial());
                    break;
                case 3://rectangular
                    break;
                default:
                    break;
            }
        }

        private void cbTipoSeñal_SegundaSeñal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelConfiguracion_SegundaSeñal.Children.Clear();
            switch(cbTipoSeñal_SegundaSeñal.SelectedIndex)
            {
                case 0: //Senoidal
                    panelConfiguracion_SegundaSeñal.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;
                case 1://Rampa
                    break;
                case 2://Exponencial
                    panelConfiguracion_SegundaSeñal.Children.Add(new ConfiguracionSeñalExponencial());
                    break;
                case 3: //Rectangular
                    break;
                default:
                    break;
            }
        }

        private void btnRealizarOperacion_Click(object sender, RoutedEventArgs e)
        {
            señalResultado = null;
            switch(cbTipoOperacion.SelectedIndex)
            {
                case 0: //suma
                    señalResultado = Señal.sumar(señal, segundaSeñal);
                    break;
                case 1: //Multiplicacion
                    señalResultado = Señal.multiplicar(señal, segundaSeñal);
                    break;
                case 2: //Combolucion
                    señalResultado = Señal.convolucionar(señal, segundaSeñal);
                    break;
                default:
                    break;
            }

            señalResultado.actualizarAmplitudMaxima();

            plnGraficaResultado.Points.Clear();


            lblmplitudMaximaPositivaY_Resultado.Text = señalResultado.AmplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY_Resultado.Text = "-" + señalResultado.AmplitudMaxima.ToString("F");



            if (señalResultado != null)
            {
                //Recorre una coleccion o arreglo 
                foreach (Muestra muestra in señalResultado.Muestras)
                {
                    plnGraficaResultado.Points.Add(new Point((muestra.X - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, (muestra.Y / señalResultado.AmplitudMaxima * ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) + (scrContenedor_Resultado.Height / 2)));
                }

            }

           
            //Recorre una coleccion o arreglo 

            plnEjeXResultado.Points.Clear();
            //Punto del principio 
            plnEjeXResultado.Points.Add(new Point(0, (scrContenedor_Resultado.Height / 2)));
            //Punto del fin
            plnEjeXResultado.Points.Add(new Point((señalResultado.TiempoFinal - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, (scrContenedor_Resultado.Height / 2)));



            plnEjeYResultado.Points.Clear();
            //Punto del principio 
            plnEjeYResultado.Points.Add(new Point((0 - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, (señalResultado.AmplitudMaxima * ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) + (scrContenedor_Resultado.Height / 2)));
            //Punto del fin
            plnEjeYResultado.Points.Add(new Point((0 - señalResultado.TiempoInicial) * scrContenedor_Resultado.Width, (-señalResultado.AmplitudMaxima * ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) + (scrContenedor_Resultado.Height / 2)));



        }
    }
}
