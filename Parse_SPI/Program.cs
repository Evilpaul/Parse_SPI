using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Parse_SPI
{
	class Program
	{
		static private string[] currString;
		static private Boolean in_transaction = false;
		static private string curr_transaction;
		static private string curr_start;
		static private List<string> mosi_data = new List<string>();
		static private List<string> miso_data = new List<string>();

		static void Main(string[] args)
		{
			// Check for correct arguments
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " <input file>");
				return;
			}

			try
			{
				if (File.Exists(args[0]))
				{
					using (StreamReader sr = new StreamReader(args[0]))
					{
						Console.WriteLine("/* Data generated " + DateTime.Now.ToString("G") + " from \"" + args[0] + "\" */");

						while (sr.Peek() >= 0)
						{
							currString = sr.ReadLine().Trim().Split(',');
							if(currString.Length == 4)
							{
								if (in_transaction && (curr_transaction == currString[1]))
								{
									// we remain in a transaction, just add data
									mosi_data.Add(currString[2]);
									miso_data.Add(currString[3]);
								}
								else if(in_transaction)
								{
									// start of new transaction, output current data and start again
									// Output the completed data
									Console.WriteLine("Transaction " + curr_transaction + ", Start Time: " + curr_start);
									Console.Write("MOSI: ");
									foreach (string line in mosi_data)
									{
										Console.Write(line + ", ");
									}
									Console.WriteLine();
									Console.Write("MISO: ");
									foreach (string line in miso_data)
									{
										Console.Write(line + ", ");
									}
									Console.WriteLine();
									Console.WriteLine();

									// wipe the lists
									mosi_data = new List<string>();
									miso_data = new List<string>();

									// record new data
									curr_start = currString[0];
									curr_transaction = currString[1];
									mosi_data.Add(currString[2]);
									miso_data.Add(currString[3]);
								}
								else
								{
									in_transaction = true;

									// first transaction
									curr_start = currString[0];
									curr_transaction = currString[1];
									mosi_data.Add(currString[2]);
									miso_data.Add(currString[3]);
								}
							}
						}

						if (in_transaction)
						{
							// start of new transaction, output current data and start again
							// Output the completed data
							Console.WriteLine("Transaction " + curr_transaction + ", Start Time: " + curr_start);
							Console.Write("MOSI: ");
							foreach (string line in mosi_data)
							{
								Console.Write(line + ", ");
							}
							Console.WriteLine();
							Console.Write("MISO: ");
							foreach (string line in miso_data)
							{
								Console.Write(line + ", ");
							}
							Console.WriteLine();
							Console.WriteLine();
						}
					}
				}
				else
				{
					Console.WriteLine("File does not exist");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The process failed: {0}", e.ToString());
			}
		}
	}
}
