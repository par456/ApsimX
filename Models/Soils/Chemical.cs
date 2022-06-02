﻿namespace Models.Soils
{
    using APSIM.Shared.APSoil;
    using APSIM.Shared.Utilities;
    using Models.Core;
    using Models.Interfaces;
    using Models.Soils.Nutrients;
    using Models.Soils.Standardiser;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>This class captures chemical soil data</summary>
    [Serializable]
    [ViewName("ApsimNG.Resources.Glade.NewGridView.glade")]
    [PresenterName("UserInterface.Presenters.NewGridPresenter")]
    [ValidParent(ParentType=typeof(Soil))]
    public class Chemical : Model, ITabularData
    {
        /// <summary>An enumeration for specifying PH units.</summary>
        public enum PHUnitsEnum
        {
            /// <summary>PH as water method.</summary>
            [Description("1:5 water")]
            Water,

            /// <summary>PH as Calcium chloride method.</summary>
            [Description("CaCl2")]
            CaCl2
        }

        /// <summary>Depth strings. Wrapper around Thickness.</summary>
        [Description("Depth")]
        [Units("cm")]
        [JsonIgnore]
        public string[] Depth
        {
            get
            {
                return SoilUtilities.ToDepthStringsCM(Thickness);
            }
            set
            {
                Thickness = SoilUtilities.ToThickness(value);
            }
        }

        /// <summary>Thickness of each layer.</summary>
        [Summary]
        [Units("mm")]
        public double[] Thickness { get; set; }

        /// <summary>pH</summary>
        [Summary]
        [Description("PH")]
        [Display(Format = "N1")]
        public double[] PH { get; set; }

        /// <summary>The units of pH.</summary>
        public PHUnitsEnum PHUnits { get; set; }

        /// <summary>Gets or sets the ec.</summary>
        [Summary]
        [Description("EC")]
        [Units("1:5 dS/m")]
        public double[] EC { get; set; }

        /// <summary>Gets or sets the esp.</summary>
        [Summary]
        [Description("ESP")]
        [Units("%")]
        public double[] ESP { get; set; }

        /// <summary>EC metadata</summary>
        public string[] ECMetadata { get; set; }

        /// <summary>CL metadata</summary>
        public string[] CLMetadata { get; set; }

        /// <summary>ESP metadata</summary>
        public string[] ESPMetadata { get; set; }

        /// <summary>PH metadata</summary>
        public string[] PHMetadata { get; set; }

        /// <summary>Tabular data. Called by GUI.</summary>
        public DataTable GetData()
        {
            var solutes = GetStandardisedSolutes();

            var data = new DataTable("Chemical");
            data.Columns.Add("Depth");
            foreach (var solute in solutes)
                data.Columns.Add(solute.Name);
            data.Columns.Add("pH");
            data.Columns.Add("EC");
            data.Columns.Add("ESP");

            // Add units to row 1.
            var unitsRow = data.NewRow();
            unitsRow["Depth"] = "(mm)";
            unitsRow["pH"] = $"({PHUnits})";
            unitsRow["EC"] = "(1:5 dS/m)";
            unitsRow["ESP"] = "(%)";
            foreach (var solute in solutes)
                unitsRow[solute.Name] = $"({solute.InitialValuesUnits})";
            data.Rows.Add(unitsRow);

            var depthStrings = SoilUtilities.ToDepthStrings(Thickness);
            for (int i = 0; i < Thickness.Length; i++)
            {
                var row = data.NewRow();
                row["Depth"] = depthStrings[i];
                if (PH != null && i < PH.Length)
                    row["pH"] = PH[i].ToString("F3");
                if (EC != null && i < EC.Length)
                    row["EC"] = EC[i].ToString("F3");
                if (ESP != null && i < ESP.Length)
                    row["ESP"] = ESP[i].ToString("F3");
                foreach (var solute in solutes)
                    row[solute.Name] = solute.InitialValues[i].ToString("F3");
                data.Rows.Add(row);
            }

            return data;
        }

        /// <summary>Get all solutes with standardised layer structure.</summary>
        /// <returns></returns>
        private IEnumerable<Solute> GetStandardisedSolutes()
        {
            var solutes = new List<Solute>();

            // Add in child solutes.
            foreach (var solute in FindAllChildren<Solute>())
            {
                var standardisedSolute = solute.Clone();
                if (solute.InitialValuesUnits == Solute.UnitsEnum.kgha)
                    standardisedSolute.InitialValues = Layers.MapMass(solute.InitialValues, solute.Thickness, Thickness, false);
                else
                    standardisedSolute.InitialValues = Layers.MapConcentration(solute.InitialValues, solute.Thickness, Thickness, 1.0);

                solutes.Add(standardisedSolute);
            }
            return solutes;
        }
    }
}
