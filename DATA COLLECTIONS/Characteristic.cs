using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kensa.Correlation.Mongo.DATA_COLLECTIONS
{
    public class Characteristic
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Values")]
        public double[] Measured { get; set; }



        public static Characteristic[] Extract(List<FileInfo> ResultFiles)
        {
            

            IEnumerable<FileInfo> orderedList = ResultFiles.OrderByDescending(x=>x.LastWriteTime).Reverse();


            string[] fileData = File.ReadAllLines(ResultFiles.First().FullName);
            Characteristic[] characteristics = new Characteristic[fileData.Count() - 2];

            int i = 0;
            int a = 0;
            foreach (string dataLine in fileData.Skip(1))
            {
                string[] data = dataLine.Split(';',',');

                string name = data[10];
                if (name == "0") { continue; }
                string valueString = data[3];
                if(valueString == "") { i++; continue; }
                double value = Convert.ToDouble(valueString);

                characteristics[i] = new Characteristic { Name = name, Measured = new double[ResultFiles.Count] };
 
                characteristics[i].Measured[a] = value;
                i++;
            }


            
            foreach (var resultFile in ResultFiles.Skip(1))
            {
                i = 0;
                a++;
                string[] file = File.ReadAllLines(resultFile.FullName);

                foreach (string dataLine in file.Skip(1))
                {
                    string[] data = dataLine.Split(';',',');

                    string name = data[10];
                    if (name == "0") { continue; }
                    double value = -1;
                    try
                    {
                        value = Convert.ToDouble(data[3]);
                    }
                    catch { }
                    try { characteristics[i].Measured[a] = value; } catch { }
                    
                    i++;
                }
            }



            return characteristics;
        }
    }


}
