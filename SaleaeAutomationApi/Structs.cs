using System;

namespace SaleaeAutomationApi
{
    public struct ConnectedDevices
    {
        public String type;
        public String name;
        public UInt64 device_id;
        public int index;
        public bool is_active;
    }

    public struct ExportDataStruct
    {
        public String FileName;

        //Channels
        public bool ExportAllChannels;
        public int[] DigitalChannelsToExport;
        public int[] AnalogChannelsToExport;

        //Time Range
        public DataExportSampleRangeType SamplesRangeType; //{ RangeAll, RangeTimes }
        public double StartingTime;
        public double EndingTime;

        //Export Type
        public DataExportType DataExportType; //{ ExportBinary, ExportCsv, ExportVcd }

        //Type: CSV (only set if Export type is CSV)
        public CsvHeadersType CsvIncludeHeaders; //{ CsvIncludesHeaders, CsvNoHeaders }
        public CsvDelimiterType CsvDelimiterType;//{ CsvComma, CsvTab }
        public CsvOutputMode CsvOutputMode;//{ CsvSingleNumber, CsvOneColumnPerBit }
        public CsvTimestampType CsvTimestampType;//{ CsvTime, CsvSample }
        public CsvBase CsvDisplayBase;//{ CsvBinary, CsvDecimal, CsvHexadecimal, CsvAscii }
        public CsvDensity CsvDensity;//{ CsvTransition, CsvComplete }

        //Type: Binary
        public BinaryOutputMode BinaryOutputMode;//{ BinaryEverySample, BinaryEveryChange }
        public BinaryBitShifting BinaryBitShifting;//{ BinaryOriginalBitPositions, BinaryShiftRight }
        public BinaryOutputWordSize BinaryOutputWordSize;//{ Binary8Bit, Binary16Bit, Binary32Bit, Binary64Bit }

        //Type: Analog Value
        public AnalogOutputFormat AnalogFormat; //This feature needs v1.1.32+ 
    }

    public struct SampleRate
    {
        public int AnalogSampleRate;
        public int DigitalSampleRate;
    }

    public struct Analyzer
    {
        public String type;
        public int index;
    }

}
