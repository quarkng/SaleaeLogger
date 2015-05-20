
namespace SaleaeAutomationApi
{
    //SetTrigger
    public enum Trigger { None, High, Low, Negedge, Posedge };

    public enum PerformanceOption { Full = 100, Half = 50, Third = 33, Quarter = 25, Low = 20 };

    //Export Data
    public enum DataExportSampleRangeType { RangeAll, RangeTimes };
    public enum DataExportType { ExportBinary, ExportCsv, ExportVcd };

    public enum CsvHeadersType { CsvIncludesHeaders, CsvNoHeaders };
    public enum CsvDelimiterType { CsvComma, CsvTab };
    public enum CsvOutputMode { CsvSingleNumber, CsvOneColumnPerBit };
    public enum CsvTimestampType { CsvTime, CsvSample };
    public enum CsvBase { CsvBinary, CsvDecimal, CsvHexadecimal, CsvAscii };
    public enum CsvDensity { CsvTransition, CsvComplete };

    public enum BinaryOutputMode { BinaryEverySample, BinaryEveryChange };
    public enum BinaryBitShifting { BinaryOriginalBitPositions, BinaryShiftRight };
    public enum BinaryOutputWordSize { Binary8Bit, Binary16Bit, Binary32Bit, Binary64Bit };

    public enum AnalogOutputFormat { Voltage, ADC };
}
