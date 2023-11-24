using System;
using System.Collections.Generic;
using System.Data;
using APSIM.Shared.Utilities;
using Models.Core;
using Models.Core.ApsimFile;
using Models.Core.Run;
using Models.Storage;
using NUnit.Framework;

namespace UnitTests.Observed
{
    [TestFixture]
    public class ObservedReportTests
    {
        /// <summary>
        /// Ensure an ObservedPredicted table exists after file is run.
        /// </summary>
        [Test]
        public void EnsureObservedReportTableCreated()
        {
            // Creating a simulation file from scratch in code was attempted. This proved problematic.
            string json = ReflectionUtilities.GetResourceAsString("UnitTests.Observed.observedTest1.apsimx");
            Simulations sims = FileFormat.ReadFromString<Simulations>(json, e => throw e, false).NewModel as Simulations;

            Simulation sim = sims.FindInScope<Simulation>();
            DataStore storage = sim.FindInScope<DataStore>();
            storage.Children.Add(new MockObservedInput());

            // Data is created this way to avoid pulling in data from a excel sheet.
            var data1 = new ReportData()
            {
                // CheckpointName is required.
                CheckpointName = "Current",
                SimulationName = "Sim1",
                TableName = "Sheet1",
                ColumnNames = new string[] { "SimulationName", "Clock.Today" },
                ColumnUnits = new string[] { null, null }
            };
            data1.Rows.Add(new List<object>() { "Sim1", "Test1" });
            data1.Rows.Add(new List<object>() { "Sim2", "Test2" });
            data1.Rows.Add(new List<object>() { "Sim3", "Test3" });

            AddTableToDataStore(storage, data1);

            Runner runner = new Runner(sims);
            List<Exception> errors = runner.Run();
            Assert.IsTrue(errors.Count <= 0);
            DataTable data = storage.Reader.GetData("ObservedReport");
            if (data.Rows.Count != 0)
            {
                DataRow rowToCheck = data.Rows[0];
                Assert.NotNull(rowToCheck);
            }
            else Assert.Fail("No rows found in ObservedReport Table.");
        }

        [Test]
        public void EnsureUnmatchedSimNamesDoNotShowInObservedReport()
        {
            // Creating a simulation file from scratch in code was attempted. This proved problematic.
            string json = ReflectionUtilities.GetResourceAsString("UnitTests.Observed.observedTest2.apsimx");
            Simulations sims = FileFormat.ReadFromString<Simulations>(json, e => throw e, false).NewModel as Simulations;

            Simulation sim = sims.FindInScope<Simulation>();
            DataStore storage = sim.FindInScope<DataStore>();
            storage.Children.Add(new MockObservedInput());

            // Data is created this way to avoid pulling in data from a excel sheet.
            var data1 = new ReportData()
            {
                // CheckpointName is required.
                CheckpointName = "Current",
                SimulationName = "Sim1",
                TableName = "Sheet1",
                ColumnNames = new string[] { "SimulationName", "Clock.Today" },
                ColumnUnits = new string[] { null, null }
            };
            data1.Rows.Add(new List<object>() { "Sim1", "Test1" });
            data1.Rows.Add(new List<object>() { "Sim2", "Test2" });
            data1.Rows.Add(new List<object>() { "Sim3", "Test3" });

            AddTableToDataStore(storage, data1);

            Runner runner = new Runner(sims);
            List<Exception> errors = runner.Run();
            Assert.IsTrue(errors.Count <= 0);
            DataTable data = storage.Reader.GetData("ObservedReport");
            if (data.Rows.Count != 0)
            {
                bool doesARowContainAnUnmatchedSimName = false;
                foreach (DataRow row in data.Rows)
                {
                    if (row[2].ToString() != sim.Name)
                        doesARowContainAnUnmatchedSimName = true;
                }
                Assert.IsFalse(doesARowContainAnUnmatchedSimName);
            }
            else Assert.Fail("No rows found in ObservedReport Table.");
        }


        [Test]
        public void EnsureExceptionWhenObservedInputMissing()
        {
            string json = ReflectionUtilities.GetResourceAsString("UnitTests.Observed.observedTest3.apsimx");
            Simulations sims = FileFormat.ReadFromString<Simulations>(json, e => throw e, false).NewModel as Simulations;

            Simulation sim = sims.FindInScope<Simulation>();
            DataStore storage = sim.FindInScope<DataStore>();
            Runner runner = new Runner(sim);
            List<Exception> errors = runner.Run();
            Assert.IsTrue(errors.Count > 0);
        }

        /// <summary>
        /// Helper to add tables to datastore. Used avoid using an excel sheet.
        /// </summary>
        /// <param name="datastore"> The DataStore for a particular Simulation.</param>
        private void AddTableToDataStore(DataStore datastore, ReportData reportData)
        {
            datastore.Writer.WriteTable(reportData.ToTable(), false);
            datastore.Writer.Stop();
            datastore.Reader.Refresh();
        }

    }
}
