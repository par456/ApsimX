using System;
using Gtk;

namespace UserInterface.Views
{

    /// <summary>
    /// View for a report component that includes new report variable and report frequency UI sections.
    /// </summary>
    public class ObservedReportView: ViewBase
    {

        private Notebook notebook = null;

        private ViewBase propertyView;

        /// <summary>Provides access to the DataGrid.</summary>
        public ViewBase PropertyView { get { return propertyView; } }

        private ViewBase dataStoreView;

        /// <summary>Provides access to the DataGrid.</summary>
        public ViewBase DataStoreView { get { return dataStoreView; } }

        /// <summary>
        /// Indicates the index of the currently active tab
        /// </summary>
        public int TabIndex
        {
            get { return notebook.CurrentPage; }
            set { notebook.CurrentPage = value; }
        }
        
        /// <summary>Constructor</summary>
        public ObservedReportView(ViewBase owner) : base(owner)
        {
            Builder builder = BuilderFromResource("ApsimNG.Resources.Glade.ObservedReportView.glade");

            notebook = new Notebook();
            mainWidget = notebook;

            propertyView = new PropertyView(this);
            notebook.AppendPage(propertyView.MainWidget, new Label("Properties"));

            dataStoreView = new ViewBase(this, "ApsimNG.Resources.Glade.DataStoreView.glade");
            notebook.AppendPage(dataStoreView.MainWidget, new Label("Data"));

            mainWidget.Destroyed += _mainWidget_Destroyed;
        }

        private void _mainWidget_Destroyed(object sender, System.EventArgs e)
        {
            try
            {
                propertyView.Dispose();
                propertyView = null;
                dataStoreView.Dispose();
                dataStoreView = null;
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



