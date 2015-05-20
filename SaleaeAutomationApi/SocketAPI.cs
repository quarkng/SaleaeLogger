using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SaleaeAutomationApi
{

    public class SocketAPI
    {
        TcpClient Socket;
        NetworkStream Stream;
        int port;
        String host;

        //Command strings
        const String set_trigger_cmd = "SET_TRIGGER";
        const String set_num_samples_cmd = "SET_NUM_SAMPLES";
        const String set_sample_rate_cmd = "SET_SAMPLE_RATE";
        const String capture_to_file_cmd = "CAPTURE_TO_FILE";
        const String save_to_file_cmd = "SAVE_TO_FILE";
        const String load_from_file_cmd = "LOAD_FROM_FILE";
        const String export_data_cmd = "EXPORT_DATA";

        const String get_all_sample_rates_cmd = "GET_ALL_SAMPLE_RATES";
        const String get_analyzers_cmd = "GET_ANALYZERS";
        const String export_analyzer_cmd = "EXPORT_ANALYZER";
        const String get_inputs_cmd = "GET_INPUTS";
        const String capture_cmd = "CAPTURE";
        const String get_capture_pretrigger_buffer_size_cmd = "GET_CAPTURE_PRETRIGGER_BUFFER_SIZE";
        const String set_capture_pretrigger_buffer_size_cmd = "SET_CAPTURE_PRETRIGGER_BUFFER_SIZE";
        const String get_connected_devices_cmd = "GET_CONNECTED_DEVICES";
        const String select_active_device_cmd = "SELECT_ACTIVE_DEVICE";

        const String get_active_channels_cmd = "GET_ACTIVE_CHANNELS";
        const String set_active_channels_cmd = "SET_ACTIVE_CHANNELS";
        const String reset_active_channels_cmd = "RESET_ACTIVE_CHANNELS";

        const String set_performance_cmd = "SET_PERFORMANCE";
        const String get_performance_cmd = "GET_PERFORMANCE";

        public SocketAPI(String host_str = "127.0.0.1", int port_input = 10429)
        {
            this.port = port_input;
            this.host = host_str;

            Socket = new TcpClient(host, port);
            Stream = Socket.GetStream();
        }

        public EventHandler<SaleaeStringEventArgs> SaleaeStringEvent;

        private void WriteString(String str)
        {
            byte[] data = str.toByteArray().Concat("\0".toByteArray()).ToArray();

            Stream.Write(data, 0, data.Length);

            if (SaleaeStringEvent != null)
            {
                SaleaeStringEvent(this, new SeleaeWriteEventArgs(str));
            }
        }

        private void GetResponse(ref String response)
        {
            while ((String.IsNullOrEmpty(response)))
            {
                response += Stream.ReadString();
            }

            if (SaleaeStringEvent != null)
            {
                SaleaeStringEvent(this, new SeleaeReadEventArgs(response));
            }

            if (!(response.Substring(response.LastIndexOf('A')) == "ACK"))
                throw new SaleaeSocketApiException();
        }

        /// <summary>
        /// Give the Socket API a custom command
        /// </summary>
        /// <param name="export_command">Ex: "set_sample_rate, 10000000"</param>
        /// <returns>Response String</returns>
        public String CustomCommand(String export_command)
        {
            WriteString(export_command);

            String response = "";
            while ((String.IsNullOrEmpty(response)))
            {
                response += Stream.ReadString();
            }

            return response;
        }

        /// <summary>
        /// Set the capture trigger
        /// </summary>
        /// <param name="triggers">List of triggers for active channels. Ex"High, Low, Posedge, Negedge, Low, High, ..."</param>
        public void SetTrigger(Trigger[] triggers)
        {
            String export_command = set_trigger_cmd;
            for (int i = 0; i < triggers.Length; ++i)
            {
                if (triggers[i] == Trigger.None)
                    export_command += ", ";
                else if (triggers[i] == Trigger.High)
                    export_command += ", high";
                else if (triggers[i] == Trigger.Low)
                    export_command += ", low";
                else if (triggers[i] == Trigger.Posedge)
                    export_command += ", posedge";
                else if (triggers[i] == Trigger.Negedge)
                    export_command += ", negedge";
            }

            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set number of samples for capture
        /// </summary>
        /// <param name="num_samples">Number of samples to set</param>
        public void SetNumSamples(int num_samples)
        {
            String export_command = set_num_samples_cmd + ", ";
            export_command += num_samples.ToString();
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set the sample rate for capture
        /// </summary>
        /// <param name="sample_rate">Sample rate to set</param>
        public void SetSampleRate(SampleRate sample_rate)
        {
            String export_command = set_sample_rate_cmd + ", ";
            export_command += sample_rate.DigitalSampleRate.ToString();
            export_command += ", " + sample_rate.AnalogSampleRate.ToString();

            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Start capture and save when capture finishes
        /// </summary>
        /// <param name="file">File to save capture to</param>
        public void CaptureToFile(String file)
        {
            String export_command = capture_to_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Save active tab capture to file
        /// </summary>
        /// <param name="file">File to save capture to</param>
        public void SaveToFile(String file)
        {
            String export_command = save_to_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Load a saved capture from fil
        /// </summary>
        /// <param name="file">File to load</param>
        public void LoadFromFile(String file)
        {
            String export_command = load_from_file_cmd + ", ";
            export_command += file;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        //create input struct
        public void ExportData(ExportDataStruct export_data_struct)
        {
            //channels
            const String all_channels_option = ", ALL_CHANNELS";
            const String digital_channels_option = ", DIGITAL_CHANNELS";
            const String analog_channels_option = ", ANALOG_CHANNELS";

            //time span
            const String all_time_option = ", ALL_TIME";
            const String time_span_option = ", TIME_SPAN";

            const String csv_option = ", CSV";
            const String headers_option = ", HEADERS";
            const String no_headers_option = ", NO_HEADERS";
            const String tab_option = ", TAB";
            const String comma_option = ", COMMA";
            const String sample_number_option = ", SAMPLE_NUMBER";
            const String time_stamp_option = ", TIME_STAMP";
            const String combined_option = ", COMBINED";
            const String separate_option = ", SEPARATE";
            const String row_per_change_option = ", ROW_PER_CHANGE";
            const String row_per_sample_option = ", ROW_PER_SAMPLE";
            const String dec_option = ", DEC";
            const String hex_option = ", HEX";
            const String bin_option = ", BIN";
            const String ascii_option = ", ASCII";

            const String binary_option = ", BINARY";
            const String each_sample_option = ", EACH_SAMPLE";
            const String on_change_option = ", ON_CHANGE";

            const String voltage_option = ", VOLTAGE";
            const String raw_adc_option = ", ADC";
            const String vcd_option = ", VCD";


            String export_command = export_data_cmd;
            export_command += ", " + export_data_struct.FileName;

            if (export_data_struct.ExportAllChannels == true)
                export_command += all_channels_option;
            else
            {
                if (export_data_struct.DigitalChannelsToExport.Length > 0)
                {
                    export_command += digital_channels_option;
                    foreach (int channel in export_data_struct.DigitalChannelsToExport)
                        export_command += ", " + channel.ToString();
                }

                if (export_data_struct.AnalogChannelsToExport.Length > 0)
                {
                    export_command += analog_channels_option;
                    foreach (int channel in export_data_struct.AnalogChannelsToExport)
                        export_command += ", " + channel.ToString();
                }
            }

            // This feature was not included in v1.1.31 and should not be used until 1.1.32 
            if (export_data_struct.AnalogChannelsToExport.Length > 0)
            {
                if (export_data_struct.AnalogFormat == AnalogOutputFormat.Voltage)
                    export_command += voltage_option;
                else if (export_data_struct.AnalogFormat == AnalogOutputFormat.ADC)
                    export_command += raw_adc_option;
            }

            if (export_data_struct.SamplesRangeType == DataExportSampleRangeType.RangeAll)
                export_command += all_time_option;
            else if (export_data_struct.SamplesRangeType == DataExportSampleRangeType.RangeTimes)
            {
                export_command += time_span_option;
                export_command += ", " + export_data_struct.StartingTime;
                export_command += ", " + export_data_struct.EndingTime;
            }

            if (export_data_struct.DataExportType == DataExportType.ExportCsv)
            {
                export_command += csv_option;

                if (export_data_struct.CsvIncludeHeaders == CsvHeadersType.CsvIncludesHeaders)
                    export_command += headers_option;
                else if (export_data_struct.CsvIncludeHeaders == CsvHeadersType.CsvNoHeaders)
                    export_command += no_headers_option;

                if (export_data_struct.CsvDelimiterType == CsvDelimiterType.CsvTab)
                    export_command += tab_option;
                else if (export_data_struct.CsvDelimiterType == CsvDelimiterType.CsvComma)
                    export_command += comma_option;

                if (export_data_struct.CsvTimestampType == CsvTimestampType.CsvSample)
                    export_command += sample_number_option;
                else if (export_data_struct.CsvTimestampType == CsvTimestampType.CsvTime)
                    export_command += time_stamp_option;

                if (export_data_struct.CsvOutputMode == CsvOutputMode.CsvSingleNumber)
                    export_command += combined_option;
                else if (export_data_struct.CsvOutputMode == CsvOutputMode.CsvOneColumnPerBit)
                    export_command += separate_option;

                if (export_data_struct.CsvDensity == CsvDensity.CsvTransition)
                    export_command += row_per_change_option;
                else if (export_data_struct.CsvDensity == CsvDensity.CsvComplete)
                    export_command += row_per_sample_option;

                if (export_data_struct.CsvDisplayBase == CsvBase.CsvDecimal)
                    export_command += dec_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvHexadecimal)
                    export_command += hex_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvBinary)
                    export_command += bin_option;
                else if (export_data_struct.CsvDisplayBase == CsvBase.CsvAscii)
                    export_command += ascii_option;
            }
            else if (export_data_struct.DataExportType == DataExportType.ExportBinary)
            {
                export_command += binary_option;

                if (export_data_struct.BinaryOutputMode == BinaryOutputMode.BinaryEverySample)
                    export_command += each_sample_option;
                else if (export_data_struct.BinaryOutputMode == BinaryOutputMode.BinaryEveryChange)
                    export_command += on_change_option;

                if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary8Bit)
                    export_command += ", 8";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary16Bit)
                    export_command += ", 16";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary32Bit)
                    export_command += ", 32";
                else if (export_data_struct.BinaryOutputWordSize == BinaryOutputWordSize.Binary64Bit)
                    export_command += ", 64";

            }
            else if (export_data_struct.DataExportType == DataExportType.ExportVcd)
            {
                export_command += vcd_option;
            }


            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get the active analyzers on the current tab
        /// </summary>
        /// <returns>A string of the names of the analyzers</returns>
        public Analyzer[] GetAnalyzers()
        {
            string export_command = get_analyzers_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);

            String[] input_strings = response.Split('\n');
            Analyzer[] analyzers = new Analyzer[input_strings.Length - 1];
            for (int i = 1; i < input_strings.Length; ++i)
            {
                String[] analyzer = input_strings[i].Split(',');
                analyzers[i - 1].type = analyzer[0]; //convert from ascii to int val
                analyzers[i - 1].index = i;
            }
            return analyzers;
        }

        /// <summary>
        /// Export a selected analyzer to a file
        /// </summary>
        /// <param name="selected">index of the selected analyzer(GetAnalyzer return string index + 1)</param>
        /// <param name="filename">file to save analyzer to</param>
        /// <param name="mXmitFile">mXmitFile</param>
        public void ExportAnalyzers(int selected, String filename, bool mXmitFile)
        {
            String export_command = export_analyzer_cmd + ", ";
            export_command += selected.ToString() + ", " + filename;
            if (mXmitFile == true)
                export_command += ", mXmitFile";
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
            if (mXmitFile == true)
                Console.WriteLine(response);
        }

        /// <summary>
        /// Start device capture
        /// </summary>
        public void Capture()
        {
            String export_command = capture_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get size of pre-trigger buffer
        /// </summary>
        /// <returns>buffer size in # of samples</returns>
        public int GetCapturePretriggerBufferSize()
        {
            String export_command = get_capture_pretrigger_buffer_size_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
            String[] input_string = response.Split('\n');
            int buffer_size = int.Parse(input_string[0]);
            return buffer_size;
        }

        /// <summary>
        /// set pre-trigger buffer size
        /// </summary>
        /// <param name="buffer_size">buffer size in # of samples</param>
        public void SetCapturePretriggerBufferSize(int buffer_size)
        {
            String export_command = set_capture_pretrigger_buffer_size_cmd + ", ";
            export_command += buffer_size.ToString();
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Return the devices connected to the software
        /// </summary>
        /// <returns>Array of ConnectedDevices structs which contain device information</returns>
        public ConnectedDevices[] GetConnectedDevices()
        {
            String export_command = get_connected_devices_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
            String[] response_strings = response.Split('\n');
            ConnectedDevices[] devices = new ConnectedDevices[response_strings.Length - 1];
            for (int i = 0; i < devices.Length; ++i)
            {
                String[] current_device = response_strings[i].Split(',');
                devices[i].type = current_device[2].Trim();
                devices[i].name = current_device[1].Trim();
                devices[i].index = int.Parse(current_device[0].Trim());
                devices[i].device_id = Convert.ToUInt64(current_device[3].Trim(), 16);
                if (current_device.Length > 4)
                    devices[i].is_active = true;
                else
                    devices[i].is_active = false;
            }
            return devices;

        }

        /// <summary>
        /// Select the active capture device
        /// </summary>
        /// <param name="device_number">Index of device (as returned from ConnectedDevices struct)</param>
        public void SelectActiveDevice(int device_number)
        {
            String export_command = select_active_device_cmd + ", ";
            export_command += device_number.ToString();
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Set the performance option
        /// </summary>
        public void SetPerformanceOption(PerformanceOption performance)
        {
            String export_command = set_performance_cmd + ", ";
            export_command += performance.ToString();
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Get the performance option currently selected.
        /// </summary>
        /// <returns>A PerformanceOption enum</returns>
        public PerformanceOption GetPerformanceOption()
        {
            String export_command = get_performance_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);

            PerformanceOption selected_option = (PerformanceOption)Convert.ToInt32(response.Split(',')[0]);
            return selected_option;
        }


        /// <summary>
        /// Get the currently available sample rates for the selected performance options
        /// </summary>
        /// <returns>Array of sample rate combinations available</returns>
        public List<SampleRate> GetAvailableSampleRates()
        {
            WriteString(get_all_sample_rates_cmd);
            String response = "";
            GetResponse(ref response);

            List<SampleRate> sample_rates = new List<SampleRate>();
            String[] new_line = { "\n" };
            String[] responses = response.Split(new_line, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < responses.Length - 1; i++)
            {
                String[] split_sample_rate = responses[i].Split(',');
                if (split_sample_rate.Length != 2)
                {
                    sample_rates.Clear();
                    return sample_rates;
                }

                SampleRate new_sample_rate;
                new_sample_rate.DigitalSampleRate = Convert.ToInt32(split_sample_rate[0].Trim());
                new_sample_rate.AnalogSampleRate = Convert.ToInt32(split_sample_rate[1].Trim());
                sample_rates.Add(new_sample_rate);
            }

            return sample_rates;
        }

        /// <summary>
        /// Get active channels for devices Logic16, Logic 8(second gen), Logic 8 pro, Logic 16 pro
        /// </summary>
        /// <returns>array of active channel numbers</returns>
        public void GetActiveChannels(List<int> digital_channels, List<int> analog_channels)
        {
            String export_command = get_active_channels_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);

            digital_channels.Clear();
            analog_channels.Clear();

            String[] input_string = response.Split('\n');
            String[] channels_string = input_string[0].Split(',');

            bool add_to_digital_channel_list = true;
            for (int i = 0; i < channels_string.Length; ++i)
            {
                if (channels_string[i] == "digital_channels")
                {
                    add_to_digital_channel_list = true;
                    continue;
                }
                else if (channels_string[i] == "analog_channels")
                {
                    add_to_digital_channel_list = false;
                    continue;
                }

                if (add_to_digital_channel_list)
                    digital_channels.Add(int.Parse(channels_string[i]));
                else
                    analog_channels.Add(int.Parse(channels_string[i]));
            }

        }

        /// <summary>
        /// Set the active channels for devices Logic16, Logic 8(second gen), Logic 8 pro, Logic 16 pro
        /// </summary>
        /// <param name="channels">array of channels to be active: 0-15</param>
        public void SetActiveChannels(int[] digital_channels = null, int[] analog_channels = null)
        {

            String export_command = set_active_channels_cmd;
            if (digital_channels != null)
            {
                export_command += ", " + "digital_channels";
                for (int i = 0; i < digital_channels.Length; ++i)
                    export_command += ", " + digital_channels[i].ToString();
            }
            if (analog_channels != null)
            {
                export_command += ", " + "analog_channels";
                for (int i = 0; i < analog_channels.Length; ++i)
                    export_command += ", " + analog_channels[i].ToString();
            }
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

        /// <summary>
        /// Reset to default active logic 16 channels (0-15)
        /// </summary>
        public void ResetActiveChannels()
        {
            String export_command = reset_active_channels_cmd;
            WriteString(export_command);

            String response = "";
            GetResponse(ref response);
        }

    }
}
