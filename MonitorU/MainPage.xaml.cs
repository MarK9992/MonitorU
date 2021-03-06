﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MonitorU.Resources;
using System.ComponentModel;
using MonitorU.Database;

namespace MonitorU
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<ScreenObscurtionEvent> list = new List<ScreenObscurtionEvent>();
        private bool _isScreenObscured;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();

            // Set the page DataContext property to the DabaseAccess.
            this.DataContext = App.DatabaseAccess;

            // Exemple de code pour la localisation d'ApplicationBar
            //BuildLocalizedApplicationBar();

            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PhoneApplicationFrame rootFrame = App.Current.RootVisual as PhoneApplicationFrame;

            if (rootFrame != null)
            {
                rootFrame.Obscured += OnObscured;
                //rootFrame.Unobscured += Unobscured; inutile ? voir plus bas
            }

            _isScreenObscured = false;
            
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        // Obscured correspond à un appui sur le bouton lock screen, donc il représente également Unobscured quand l'écran était éteint.
        void OnObscured(Object sender, ObscuredEventArgs e)
        {
            ScreenObscurtionEvent ob = new ScreenObscurtionEvent { Type = Database.EventType.Obscurtion, Date = DateTime.Now };
            list.Add(ob);
            System.Diagnostics.Debug.WriteLine("ob");
            ScreenObscurtionEvent screenEvent;
            
            if (_isScreenObscured)
            {
                screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Obscurtion, Date = DateTime.Now };
                _isScreenObscured = false;
                txtUnobs.Text = "Unobscured at " + DateTime.Now.ToString();
                System.Diagnostics.Debug.WriteLine("Unobscured.");
            }
            else
            {
                screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Unobscurtion, Date = DateTime.Now };
                _isScreenObscured = true;
                txtObs.Text = "Obscured at " + DateTime.Now.ToString();
                System.Diagnostics.Debug.WriteLine("Obscured.");
            }

            App.DatabaseAccess.AddScreenObscurtionEvent(screenEvent);
        }

        /* Uboscured c'est quand on retourne sur l'application et non quand on allume l'écran.
        void Unobscured(Object sender, EventArgs e)
        {
<<<<<<< HEAD
            ScreenObscurtionEvent unob = new ScreenObscurtionEvent { Type = Database.Type.Unobscurtion, Date = DateTime.Now };
            //ScreenObscurtionEvent unob = new ScreenObscurtionEvent(1, Database.Type.Obscurtion, DateTime.Now);
            list.Add(unob);
            System.Diagnostics.Debug.WriteLine("unob");
=======
            ScreenObscurtionEvent screenEvent = new ScreenObscurtionEvent { Type = Database.EventType.Unobscurtion, Date = DateTime.Now };

            App.DatabaseAccess.AddScreenObscurtionEvent(screenEvent);
            txtUnobs.Text = "Unobscured at " + DateTime.Now.ToString();
>>>>>>> bd60c72beea55ff4eb96f67cc82dbd8389639e8b
        }
        */

        // Exemple de code pour la conception d'une ApplicationBar localisée
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Définit l'ApplicationBar de la page sur une nouvelle instance d'ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Crée un bouton et définit la valeur du texte sur la chaîne localisée issue d'AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Crée un nouvel élément de menu avec la chaîne localisée d'AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        //Prend les obscured et unobscured de la liste (ou de la base de donnée) et les met sous la forme d'un tableau de int
        //en prévision des calculs des clusters
        private void listToData(object sender, RoutedEventArgs e)
        {
            //Ne sachant pas la taille finale du tableau, on range les infos utiles dans des listes
            List<int> diffs = new List<int>();
            List<int> plages = new List<int>();
            List<ScreenObscurtionEvent> list = App.DatabaseAccess.Events.ToList();
            foreach (ScreenObscurtionEvent screenEvent in list)
            {
                System.Diagnostics.Debug.WriteLine(screenEvent);
            }

            if (list.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Pas de données");
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    // Si c'est un unobscured, on prend le obscured qui sui et on soustrait pour obtenir le temps passé
                    // sur le téléphone
                    if (list[i].Type.Equals(Database.EventType.Unobscurtion) && i < list.Count - 1)
                    {
                        System.TimeSpan diff = list[i+1].Date.Subtract(list[i].Date);
                        diffs.Add((int)diff.TotalMinutes);
                        //A partir de l'horaire du unobscured, on définit une plage horaire
                        plages.Add(dateToPlage(list[i].Date));
                    }
                }

                if (diffs.Count != 0)
                {
                    int[][] datas = new int[diffs.Count][];
                    for (int j = 0; j < diffs.Count; j++)
                    {
                        datas[j] = new int[] { diffs[j], plages[j] };
                    }
                    int[] clustering = Kmeans.Cluster(datas, 4);
                    String clustered=Kmeans.ShowClustered(datas, clustering, 4, 1);
                    kmeansResultTextBox.Text = clustered;
                }
            }
        }


        //Permet à partir d'un DateTime d'obtenir un entier définissant une plage horaire:
        // De 05h00 à 12h00: le matin (1)
        // De 12h00 à 18h00: l'après-midi (2)
        // De 18h00 à 23h00: la soirée (3)
        // De 23h00 à 05h00: la nuit (4)
        int dateToPlage(DateTime date)
        {
            int plage;
            int hour = Int32.Parse(date.ToString("HH"));

            if(5<=hour && hour < 12)
            {
                plage = 1;
            }else if(12<=hour && hour < 18)
            {
                plage = 2;
            }else if(18<=hour && hour < 23)
            {
                plage = 3;
            }
            else
            {
                plage = 4;
            }

            System.Diagnostics.Debug.WriteLine("plage " + plage);
            return plage;
        }

    }

   
}