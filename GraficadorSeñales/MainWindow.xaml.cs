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


            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;



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

           
            señal.actualizarAmplitudMaxima();
 
            amplitudMaxima = señal.AmplitudMaxima;
           
            plnGrafica.Points.Clear();
      

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
    }
}
