using System;
using System.Collections.Generic;
using System.Data;
using Models;
using Models.Core;
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
            Simulations sims = Utilities.GetRunnableSim();
            Utilities.InitialiseModel(sims);
            Simulation sim = sims.FindInScope<Simulation>();
            sim.Name = "Sim1";
            // Remove Report as it removes tables from DataStore for an unknown reason?
            Models.Report report = sim.FindInScope<Models.Report>();
            sim.Children.Remove(report);
            DataStore storage = sim.FindInScope<DataStore>();
            storage.Children.Add(new MockObservedInput());
            sim.Children.Add(new ObservedReport() { EventFrequency = "[Clock].EndOfDay" });
            // Data is created this way to avoid pulling in data from a excel sheet.
            var data1 = new ReportData()
            {
                // CheckpointName is required.
                CheckpointName = "Current",
                SimulationName = "Sim1",
                TableName = "Sheet1",
                ColumnNames = new string[] { "SimulationName", "Clock.EndOfDay" },
                ColumnUnits = new string[] { null, null }
            };
            data1.Rows.Add(new List<object>() { "Sim1", new DateTime(2017, 1, 10) });
            data1.Rows.Add(new List<object>() { "Sim2", new DateTime(2017, 1, 10) });
            data1.Rows.Add(new List<object>() { "Sim3", new DateTime(2017, 1, 10) });

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
            Simulations sims = Utilities.GetRunnableSim();
            Utilities.InitialiseModel(sims);
            Simulation sim = sims.FindInScope<Simulation>();
            sim.Name = "Sim1";
            // Remove Report as it removes tables from DataStore for an unknown reason?
            Models.Report report = sim.FindInScope<Models.Report>();
            sim.Children.Remove(report);
            DataStore storage = sim.FindInScope<DataStore>();
            storage.Children.Add(new MockObservedInput());
            sim.Children.Add(new ObservedReport() { EventFrequency = "[Clock].EndOfDay" });

            // Data is created this way to avoid pulling in data from a excel sheet.
            var data1 = new ReportData()
            {
                // CheckpointName is required.
                CheckpointName = "Current",
                SimulationName = sim.Name,
                TableName = "Sheet1",
                ColumnNames = new string[] { "SimulationName", "Clock.EndOfDay" },
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
