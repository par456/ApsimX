using System;
using Models.Core;
using Models.PreSimulationTools;

namespace UnitTests.Observed
{
    [Serializable]
    public class MockObservedInput : Model, IObservedInput
    {
        public string[] ColumnNames
        {
            get
            {
                return new string[] { "Clock.EndOfDay" };
            }
        }

        public string[] SheetNames
        {
            get
            {
                return new string[] { "Sheet1" };
            }
        }

        public MockObservedInput()
        {

        }
    }
}
