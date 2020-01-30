using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;



namespace Sage300UnitsExport
{
    class ATDatabase
    {
        private SqlConnection myConnection;

        public void connect()
        {
            try
            {
                Console.WriteLine("Connecting...");
                String server = Properties.Settings.Default.server;
                String database = Properties.Settings.Default.database;
                String port = Properties.Settings.Default.port;
                String user = Properties.Settings.Default.user;
                String pass = Properties.Settings.Default.pass;
                Console.WriteLine("Server=" + server + "," + port + ";Database=" + database + ";Uid=" + user + ";Pwd=" + pass + ";");
                myConnection = new SqlConnection("Server=" + server + "," + port + ";Database=" + database + ";Uid=" + user + ";Pwd=" + pass + ";");
                myConnection.Open();
                Console.WriteLine("About Time Database Connection Successful");

            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.ToString());
            }
            Console.WriteLine("Log A");
        }

        public ArrayList selectJobCode()
        {
            ArrayList jobAL = new ArrayList();
            try
            {
                using (SqlCommand command = new SqlCommand("SELECT jobid, jobcode, userdef1 FROM job", myConnection))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        GeneralEntity generalEntity = new GeneralEntity(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

                        jobAL.Add(generalEntity);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return jobAL;
        }

        public ArrayList selectActivityCode()
        {
            ArrayList actAL = new ArrayList();
            try
            {
                using (SqlCommand command = new SqlCommand("SELECT activityid, activitycode FROM activity", myConnection))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        GeneralEntity generalEntity = new GeneralEntity(reader.GetInt32(0), reader.GetString(1), "");

                        actAL.Add(generalEntity);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return actAL;
        }

        public ArrayList loadUnits()
        {
            ArrayList timeShiftAL = new ArrayList();
            try
            {
                // and exported2 != null
                using (SqlCommand command = new SqlCommand("SELECT completedshiftid, jobid, costcodeid, produnits, outTime FROM completedshift where (prodUnits != 0 or prodUnits != null) and exported2 is null and batchid != -100", myConnection))
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        TimeShift timeShift = new TimeShift();
                        timeShift.CompletedShiftId = reader.GetInt32(0);
                        timeShift.JobId = reader.GetInt32(1);
                        timeShift.CostCodeId = reader.GetInt32(2);
                        timeShift.Units = safeGetDouble(reader, 3);
                        timeShift.OutTime = reader.GetDateTime(4);
                        timeShiftAL.Add(timeShift);

                        Console.WriteLine(timeShift.CompletedShiftId + "::" + timeShift.JobId + "::" + timeShift.CostCodeId + "::" + timeShift.Units + "::" + timeShift.OutTime);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return timeShiftAL;
        }

        private double safeGetDouble(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDouble(colIndex);
            else
                return 0.0;
        }

        public void flagExported(int completedShiftId, DateTime dateVal)
        {
            try
            {
                SqlCommand command = null;
                using (command = new SqlCommand("update completedshift set exported2 = @dt where completedshiftid = @id", myConnection))
                    command.Parameters.AddWithValue("@dt", dateVal);
                command.Parameters.AddWithValue("@id", completedShiftId);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

    }


}
