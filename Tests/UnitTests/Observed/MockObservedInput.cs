using System;
using Models.Core;
using Models.PreSimulationTools;

namespace UnitTests.Observed
{
    [Serializable]
    public class MockObservedInput : Model, IObservedInput
    {
        public string[] ColumnNames => new string[] { "Clock.Today" };

        public string[] SheetNames => new string[] { "Sheet1" };

        public MockObservedInput()
        {

        }
    }
}
