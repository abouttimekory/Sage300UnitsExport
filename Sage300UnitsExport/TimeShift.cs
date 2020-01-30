using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage300UnitsExport
{
    class TimeShift
    {
        private int completedShiftId;
        private int jobId;
        private int costCodeId;
        private double units;
        private DateTime inTime;
        private DateTime outTime;

        public int CompletedShiftId
        {
            get { return completedShiftId; }
            set { completedShiftId = value; }
        }

        public int JobId
        {
            get { return jobId; }
            set { jobId = value; }
        }

        public int CostCodeId
        {
            get { return costCodeId; }
            set { costCodeId = value; }
        }

        public double Units
        {
            get { return units; }
            set { units = value; }
        }

        public DateTime InTime
        {
            get { return inTime; }
            set { inTime = value; }
        }

        public DateTime OutTime
        {
            get { return outTime; }
            set { outTime = value; }
        }
    }
}
