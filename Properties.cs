using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kensa.Correlation.Mongo
{
    internal class Properties
    {
        internal const string MONGO_CONNECTION_STRING = "mongodb://10.2.1.18:27017";                            //NEED TO SET THIS UP WITH A USER AND PASSWORD         

        internal const string MONGO_DATABASE_STRING = "CorrelationDatabase";                                    //RESULT DATA FROM CORRELATION STUDIES

        internal const string MONGO_RESULTS_ASSEMBLY_COLLECTION = "Results_Assembly";                           //
        internal const string MONGO_RESULTS_GAUGEBLOCK_COLLECTION = "Results_GaugeBlock";                       //
        internal const string MONGO_RESULTS_RINGGAUGE_COLLECTION = "Results_RingGauge";                         //   

        internal const string MONGO_CATALOG_ASSEMBLY_COLLECTION = "Catalog_Assembly";                           //
        internal const string MONGO_CATALOG_GAUGEBLOCK_COLLECTION = "Catalog_GaugeBlock";                       //
        internal const string MONGO_CATALOG_RINGGAUGE_COLLECTION = "Catalog_RingGauge";                         //   

        internal const string MONGO_MACHINES_COLLECTION = "Machines";                                           //


        internal const string FILE_TESTINFO_ASSEMBLY = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\DataFiles\_dataBase.csv";
        internal const string FILE_TESTINFO_GAUGEBLOCK = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\DataFiles\TestInfo\GaugeBlock_TestInfo.csv";
        internal const string FILE_TESTINFO_RINGGAUGE = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\DataFiles\TestInfo\RingGauge_TestInfo.csv";


        internal const string DIRECTORY_MACHINEOUTPUTS_ASSEMBLY = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\MachineFiles\MachineOutputs\CorrelationAssembly\";
        internal const string DIRECTORY_MACHINEOUTPUTS_GAUGEBLOCK = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\MachineFiles\MachineOutputs\GaugeBlock\";
        internal const string DIRECTORY_MACHINEOUTPUTS_RINGGAUGE = @"\\dwffs08\ToolNet\ZeroTouch\Correlation\MachineFiles\MachineOutputs\RingGauge\";
    }
}
