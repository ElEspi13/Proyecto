using ProyectoTicketing.Resources.MisTemas;
using ProyectoTicketing.Resources.Idiomas;
namespace ProyectoTicketing.Vistas
{
    /// <summary>
    /// Clase parcial que representa la página de configuración de la aplicación.
    /// </summary>
    public partial class Configuracion : ContentPage
    {
        private ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        private AppShell shell;
        private double FuenteTitulo=30;
        private double FuenteObjetos=20;


        /// <summary>
        /// Constructor de la clase Configuracion.
        /// </summary>
        /// <param name="shell">Instancia de AppShell.</param>
        public Configuracion(AppShell shell)
        {
            InitializeComponent();
            this.shell = shell;
            mergedDictionaries.Clear();
            LanguagePicker.SelectedIndex = 0;
            ThemePicker.ItemsSource = new string[]
            {
                (string)App.Current.Resources["Original"],
                (string)App.Current.Resources["AltoContraste"]
            };
            ThemePicker.SelectedIndex = 0;
        }

        /// <summary>
        /// Maneja el evento de cambio de selección en el selector de tema y elimina el anterior de tema y crea el elegido.
        /// </summary>
        private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            if (picker.SelectedIndex == 0)
            {

                var diccionarioTemaOscuro = mergedDictionaries.FirstOrDefault(diccionario => diccionario is TemaOscuro);
                if (diccionarioTemaOscuro != null)
                {
                    mergedDictionaries.Remove(diccionarioTemaOscuro);
                    mergedDictionaries.Add(new TemaOriginal());
                }
                else
                {
                    mergedDictionaries.Add(new TemaOriginal());
                }
            }

            else if (picker.SelectedIndex == 1)
            {

                var diccionarioTemaOriginal = mergedDictionaries.FirstOrDefault(diccionario => diccionario is TemaOriginal);
                if (diccionarioTemaOriginal != null)
                {
                    mergedDictionaries.Remove(diccionarioTemaOriginal);
                    mergedDictionaries.Add(new TemaOscuro());
                }
                else
                {
                    mergedDictionaries.Add(new TemaOscuro());
                }
            }
        }

        /// <summary>
        /// Se ejecuta cuando la página es mostrada.
        /// Oculta la barra de navegación y los elementos de la interfaz.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Deshabilitar la navegación cuando aparece la página de configuración
            Shell.SetNavBarIsVisible(this, false); // Oculta la barra de navegación

            // Oculta los ToolbarItems
            Shell.SetNavBarIsVisible(this, false); // Oculta los ToolbarItems
            Shell.SetTabBarIsVisible(this, false); // Oculta los FlyoutItems
        }

        /// <summary>
        /// Se ejecuta cuando la página está a punto de ser ocultada.
        /// Muestra la barra de navegación y los elementos de la interfaz.
        /// </summary>
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            // Habilitar la navegación cuando desaparece la página de configuración
            Shell.SetNavBarIsVisible(this, true); // Muestra la barra de navegación

            // Muestra los ToolbarItems y los FlyoutItems
            Shell.SetNavBarIsVisible(this, true); // Muestra los ToolbarItems
            Shell.SetTabBarIsVisible(this, true); // Muestra los FlyoutItems
            shell.FlyoutBehavior = FlyoutBehavior.Flyout;
            shell.Desconectar.Text = (string)App.Current.Resources["Desconectar"];
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de la interfaz para guardar la configuración en la base de datos.
        /// Muestra una alerta si el usuario no ha iniciado sesión.
        /// </summary>
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (shell.BBDDsesion == true)
            {
                shell.GuardarConfiguracion(ThemePicker.SelectedIndex,LanguagePicker.SelectedIndex,FontSizeSlider.Value);
            }
            else
            {
                DisplayAlert("Error", "Inicie sesion para guardar", "OK");
            }
        }

        /// <summary>
        /// Maneja el evento de cambio de selección en el selector de idioma y elimina del diccionario el lenguaje anterior y agrega el nuevo.
        /// </summary>
        private void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            if (picker.SelectedIndex == 0)
            {

                var diccionarioingles = mergedDictionaries.FirstOrDefault(diccionario => diccionario is lang_en);
                if (diccionarioingles != null)
                {
                    mergedDictionaries.Remove(diccionarioingles);
                    mergedDictionaries.Add(new lang_es());
                }
                else
                {
                    mergedDictionaries.Add(new lang_es());
                }
            }

            else if (picker.SelectedIndex == 1)
            {

                var diccionarioespanol = mergedDictionaries.FirstOrDefault(diccionario => diccionario is lang_es);
                if (diccionarioespanol != null)
                {
                    mergedDictionaries.Remove(diccionarioespanol);
                    mergedDictionaries.Add(new lang_en());
                }
                else
                {
                    mergedDictionaries.Add(new lang_en());
                }
            }
            int seleccionado = ThemePicker.SelectedIndex;
            ThemePicker.ItemsSource = null;
            ThemePicker.ItemsSource = new string[]
            {
                (string)App.Current.Resources["Original"],
                (string)App.Current.Resources["AltoContraste"]
            };
            ThemePicker.SelectedIndex = seleccionado;
        }

        /// <summary>
        /// Maneja el evento de cambio de valor en el control deslizante de tamaño de fuente.
        /// </summary>
        private void FontSizeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Slider fuente = sender as Slider;
            shell.CambiarFuentesPaginas(fuente.Value);
            CambiarFuente();
        }

        /// <summary>
        /// Asigna la configuración de tema, idioma y fuente a los controles correspondientes en la interfaz de usuario.
        /// </summary>
        /// <param name="value">Una tupla que contiene el tema, el idioma y la fuente de la configuración.</param>
        internal void AsignarConfiguracion((int tema, int idioma, double fuente) value)
        {
            ThemePicker.SelectedIndex = value.tema;
            LanguagePicker.SelectedIndex = value.idioma;
            FontSizeSlider.Value = value.fuente;
        }

        /// <summary>
        /// Cambia el tamaño de la fuente de varios elementos de la interfaz de usuario.
        /// </summary>
        public void CambiarFuente()
        {
            ThemePicker.FontSize= FuenteObjetos*FontSizeSlider.Value;
            LanguagePicker.FontSize = FuenteObjetos * FontSizeSlider.Value;
            Titulo.FontSize = FuenteTitulo * FontSizeSlider.Value;
            TemaLabel.FontSize = FuenteObjetos * FontSizeSlider.Value;
            IdiomaLabel.FontSize = FuenteObjetos * FontSizeSlider.Value;
            TamanoLabel.FontSize = FuenteObjetos * FontSizeSlider.Value;
            BotonGuardar.FontSize = FuenteObjetos * FontSizeSlider.Value;
        }
    }
}