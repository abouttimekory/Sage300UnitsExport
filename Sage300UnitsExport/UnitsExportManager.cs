using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sage.AOF;
using Sage.Data.CRE;
using Sage.STO.TransactionService;
using Sage.STO.TransactionServiceJCAddin;
using Sage.Messaging;
using Sage.Configuration;
using System.IO;
using System.Data;
using System.Collections;

namespace Sage300UnitsExport
{
    class UnitsExportManager : IMessageTargetWithReturn
    {
        private ATDatabase db;
        private DataRow jctRow;
        private TransactionService ts;
        private TransactionBatch batch;
        private WorkingSet ws;
        private ArrayList jobAL;
        private ArrayList actAL;
        private ArrayList shiftAL;

        public void handleUnitsExport()
        {
            db = new ATDatabase();
            db.connect();
            Console.WriteLine("Log B");
            this.handleSage300Connection();
            Console.WriteLine("Log C");

            this.loadUnits();
            this.exportUnits();
        }

        private void loadUnits()
        {
            actAL = db.selectActivityCode();
            jobAL = db.selectJobCode();
            shiftAL = db.loadUnits();
        }

        private void exportUnits()
        {
           DateTime currentDate = DateTime.Now;
            string jobCode = "";
            string jobExtra = "";
            string costCode = "";
            double units = 0;
            if (shiftAL != null && shiftAL.Count != 0)
            {
                Console.WriteLine("Exporting " + shiftAL.Count + " Record(s)");
                int locCount = 0;
                for (int i = 0, n = shiftAL.Count; i < n; i++)
                {
                    locCount++;
                    TimeShift timeShift = (TimeShift)shiftAL[i];
                    GeneralEntity jobEnt = this.findGeneralEntityByID(timeShift.JobId, jobAL);
                    GeneralEntity actEnt = this.findGeneralEntityByID(timeShift.CostCodeId, actAL);
                    if (jobEnt != null)
                    {
                        jobCode = jobEnt.Code;
                        jobExtra = jobEnt.ExtraCode;
                    }
                    if (actEnt != null)
                    {
                        costCode = actEnt.Code;
                    }
                    //Decimal.TryParse(timeShift.Units, out units);
                    
                    units = timeShift.Units;

                    Console.WriteLine(locCount + " of " + shiftAL.Count + " JOB:" + jobCode + " Extra:" + jobExtra + " Cost Code:" + costCode + " Units:" + units);
                    try
                    {
                        jctRow[JctConstants.JctColumnNames.Job] = jobCode;
                    }
                    catch (Sage.Data.AOF.RowColumnException exception)
                    {
                        Console.WriteLine(exception.Message);
                        Console.WriteLine("FAILED TO FIND-- JOB:" + jobCode);
                    }
                    if (jobExtra != null)
                    {
                        try
                        {
                            jctRow[JctConstants.JctColumnNames.Extra] = jobExtra;
                        }
                        catch (Sage.Data.AOF.RowColumnException exception)
                        {
                            Console.WriteLine(exception.Message);
                            Console.WriteLine("FAILED TO FIND-- Extra:" + jobExtra);
                        }
                    }

                    try
                    {
                        jctRow[JctConstants.JctColumnNames.CostCode] = costCode;
                        
                        //jctRow[JctConstants.JctColumnNames.TranDate] = costCode;
                        //jctRow[JctConstants.JctColumnNames.] = costCode;
                    }
                    catch (Sage.Data.AOF.RowColumnException exception)
                    {
                        Console.WriteLine(exception.Message);
                        Console.WriteLine("FAILED TO FIND-- Cost Code:" + costCode);
                    }
                     
                    try
                     {
                         jctRow[JctConstants.JctColumnNames.TranDate] = timeShift.OutTime;
                         //jctRow[Sage.STO.TransactionServiceJCAddin.JctConstants.JctColumnNames.AccountingDate] = "7/12/2015";
                     }
                     catch (Sage.Data.AOF.RowColumnException exception)
                     {
                         Console.WriteLine(exception.Message);
                         Console.WriteLine("FAILED TO FIND-- Cost Code:" + costCode);
                     }
                    

                    jctRow[Sage.STO.TransactionServiceJCAddin.JctConstants.JctColumnNames.Units] = units;
                    try
                    {
                        batch.Transactions.Tables[JctConstants.DataSetTransactionTableName].Rows.Add(jctRow);
                        batch.ProcessTransactions();
                        jctRow = batch.Transactions.Tables[JctConstants.DataSetTransactionTableName].NewRow();
                        batch.GetHashCode();
                        batch.Post();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FAILED TO POST-- JOB:" + jobCode + " Extra:" + jobExtra + " Cost Code:" + costCode + " Units:" + units);
                    }

                    db.flagExported(timeShift.CompletedShiftId, currentDate);
                }
                Console.WriteLine("Export completed");
            }
            else
            {
                Console.WriteLine("No Units to Export.");
            }
        }

        private void handleSage300Connection()
        {
            Console.WriteLine("Connecting Sage 300...");
            String company = Properties.Settings.Default.company;
            String path = Properties.Settings.Default.path;
            this.beginTimberlineConnect();
           
        }

        public void beginTimberlineConnect()
        {
            CRESession session = CRESessionFactory.GetCRESession();
            // session.Company = "Timberline Construction";//Timberline Electrical
            Console.WriteLine("Loading Sage Company:" + Properties.Settings.Default.company);
            //Console.WriteLine("Loading Sage User:" + Properties.Settings.Default.sageuser);
            // Console.WriteLine("Loading Sage Pass:xxxxxxxx");
            session.Company = Properties.Settings.Default.company;
            session.UserName = Properties.Settings.Default.sageuser;
            session.Password = Properties.Settings.Default.sagepassword;
            //Console.WriteLine("TC:A");
            ConnectionProvider.EstablishCompanyConnections(session);
           // Console.WriteLine("TC:B");
            ts = TransactionServiceFactory.CreateTransactionService(session, "4DEB77A7-D9CD-4560-A52D-088AF3020D9A");
           // Console.WriteLine("TC:C");
            ws = new WorkingSet();
            Console.WriteLine("TC:D");
            ws.MessageBoard(WorkingSetScope.AppDomain).TargetCache.AddRunningTarget(JctConstants.ClosedJobWarningMessageTargetId, this);
           // Console.WriteLine("TC:E");
            Hashtable parameters = new Hashtable();
           // Console.WriteLine("TC:F");
            parameters.Add(JctConstants.ParamNewJctFile, "new.jct");
           // Console.WriteLine("TC:G");
            parameters.Add(JctConstants.ParamCurrentJctFile, "current.jct");
           // Console.WriteLine("TC:H");
            ws.AddValue(Sage.STO.TransactionService.TransactionService.EntityParametersKey, parameters);
           // Console.WriteLine("TC:I");
            batch = ts.CreateTransactionBatch(JctConstants.TransactionServiceTypes.MiscTransactions,
                                                               JctConstants.Security.MiscTransactions,
                                                               ws,
                                                               "DP", // origin
                                                               "About Time Tech"); // source description
            //Console.WriteLine("TC:J");
            try
            {
                batch.Transactions.Tables[JctConstants.DataSetTransactionTableName].Columns[JctConstants.JctColumnNames.TransactionType].DefaultValue = JctConstants.TransactionTypes.ProductionUnitsInPlace;
                //Console.WriteLine("TC:K");
            }
            catch (Exception)
            {
                Console.WriteLine("Not Enough Users.");//Not enough users error!
            }
           // Console.WriteLine("TC:L");
            jctRow = batch.Transactions.Tables[JctConstants.DataSetTransactionTableName].NewRow();
            //Console.WriteLine("TC:M");

        }

        public string ReceiveMessage(string xmlTypeDescriptor, object argument)
        {
            return new SuccessResponse(JctConstants.ClosedJobWarningMessageTargetId).Xml;
        }

        public GeneralEntity findGeneralEntityByID(int id, ArrayList generalEntityAL)
        {
            GeneralEntity foundEntity = null;
            for(int i = 0, n = generalEntityAL.Count; i < n; i++)
            {
                GeneralEntity genEntity = (GeneralEntity)generalEntityAL[i];
                if (genEntity.Id == id)
                {
                    foundEntity = genEntity;
                    break;
                }
            }
            return foundEntity;
        }
    }
}
