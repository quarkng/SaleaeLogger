#Use the Saleae Logic (Gen 2) Pod as a data logger
A data logger allows you to log analog signals over long periods of time.  The Saleae Logic software will crash if you capture so many samples that it overruns the physical memory of your computer.  This software provides a compromise.

First you must set up your Logic software to enable the "scripting socket server" and have it listen to port 10429.  The Logic software must be running.  Then, in this SaleaeLogger program, click Connect to establish and test the connection.  When you are ready to log, click Start Logging.

This program will trigger the Logic software to start a new capture as soon as possible when the previous capture ends.  The data will be saved as separete files under C:\_LogicData.  There will be gaps of time where no samples are taken, but it is better than nothing.

One major advantage this have over traditional data loggers is that the sample rate is very high.

<hr>

Note that code in SaleaeAutomationApi  is based on
http://support.saleae.com/hc/en-us/articles/201104764
