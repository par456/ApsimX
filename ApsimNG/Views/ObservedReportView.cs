using System;
using System.Collections.Generic;
using System.Drawing;
using GLib;
using Gtk;
using UserInterface.Classes;
using UserInterface.EventArguments;
using UserInterface.Interfaces;
using Utility;

namespace UserInterface.Views
{

    /// <summary>
    /// View for a report component that includes new report variable and report frequency UI sections.
    /// </summary>
    public class ObservedReportView: ViewBase
    {

        private Notebook notebook1 = null;
        private Alignment alignment1 = null;
        private ViewBase dataStoreView1;

        /// <summary>Provides access to the DataGrid.</summary>
        public ViewBase DataStoreView { get { return dataStoreView1; } }

        /// <summary>
        /// Indicates the index of the currently active tab
        /// </summary>
        public int TabIndex
        {
            get { return notebook1.CurrentPage; }
            set { notebook1.CurrentPage = value; }
        }

        /// <summary> Invoked when the selected tab is changed.</summary>
        public event EventHandler TabChanged;
        
        /// <summary>Constructor</summary>
        public ObservedReportView(ViewBase owner) : base(owner)
        {
            Builder builder = BuilderFromResource("ApsimNG.Resources.Glade.ObservedReportView.glade");

            notebook1 = (Notebook)builder.GetObject("notebook1");

            dataStoreView1 = new ViewBase(this, "ApsimNG.Resources.Glade.DataStoreView.glade");
            notebook1.Add(dataStoreView1.MainWidget);
            notebook1.SetTabLabelText(dataStoreView1.MainWidget, "Data");
            notebook1.Add(alignment1);

            mainWidget = notebook1;
            mainWidget.Destroyed += _mainWidget_Destroyed;
            notebook1.SwitchPage += OnSwitchPage;
        }

        /// <summary>
        /// Invoked when the selected tab is changed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="args">Event arguments.</param>
        /// <remarks>
        /// Note that there is no [ConnectBefore] attribute,
        /// so at the time this is called, this.TabIndex
        /// will return the correct (updated) value.
        /// </remarks>
        private void OnSwitchPage(object sender, SwitchPageArgs args)
        {
            try
            {
                TabChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception err)
            {
                ShowError(err);
            }
        }


        private void _mainWidget_Destroyed(object sender, System.EventArgs e)
        {
            try
            {
                notebook1.SwitchPage -= OnSwitchPage;
                dataStoreView1.Dispose();
                dataStoreView1 = null;
                mainWidget.Destroyed -= _mainWidget_Destroyed;
                owner = null;
            }
            catch (Exception err)
            {
                ShowError(err);
            }
        }
    }
}



