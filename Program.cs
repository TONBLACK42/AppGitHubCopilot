namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();

            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);

        }

        // public struct SalesData. Include the following fields: date sold, department name, product ID, quantity sold, unit price
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }

        /// <summary>
        /// The <c>ProdDepartments</c> struct contains arrays of department names and their corresponding abbreviations.
        /// </summary>
        public struct ProdDepartments
        {
            /// <summary>
            /// An array of department names.
            /// </summary>
            public static string[] departmentNames = new string[]
            {
                "Men's Wear",
                "Women's Wear",
                "Children's Wear",
                "Footwear",
                "Accessories",
                "Sportswear",
                "Underwear",
                "Outerwear"
            };

            /// <summary>
            /// An array of department abbreviations corresponding to the department names.
            /// </summary>
            public static string[] departmentAbbreviations = new string[]
            {
                "MWER",
                "WWER",
                "CWAR",
                "FTWR",
                "ACCS",
                "SPRT",
                "UNDR",
                "OTWR"
            };
        }

        public struct ManufacturingSites
        {
            public static string[] manSites = new string[]
            {
                "US1", "US2", "US3",
                "CA1", "CA2", "CA3",
                "MX1", "MX2", "MX3",
                "MX4"
            };
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < salesData.Length; i++)
            {
                salesData[i].dateSold = new DateOnly(2024,random.Next(1,13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(ProdDepartments.departmentNames.Length)];
                
                int indexOfDept = Array.IndexOf(ProdDepartments.departmentNames, salesData[i].departmentName);
                string deptAbb = ProdDepartments.departmentAbbreviations[indexOfDept];
                string firstDigit = (indexOfDept + 1).ToString();
                string nextTwoDigits = random.Next(1, 100).ToString("D2");
                string sizeCode = "";
                string[] sizes = { "XS", "S", "M", "L", "XL" };
                sizeCode = sizes[random.Next(sizes.Length)];
                string colorCode = "";
                string[] colors = { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" };
                colorCode = colors[random.Next(colors.Length)];
                string manufacturingSite = "";
                manufacturingSite = ManufacturingSites.manSites[random.Next(ManufacturingSites.manSites.Length)];
                
                salesData[i].productID = $"{deptAbb}-{firstDigit}-{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                double discountPercentage = random.Next(5, 21) / 100.0;
                salesData[i].baseCost = salesData[i].unitPrice * (1 - discountPercentage);
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);
            }

            return salesData;
        }

        public void QuarterlySalesReport(SalesData[] salesData)
        {
            // create dictionaries to store the quarterly sales data and profit data
            Dictionary<string, double> quarterlySales = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfit = new Dictionary<string, double>();
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDept = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDept = new Dictionary<string, Dictionary<string, double>>();

            // iterate through the sales data
            foreach (SalesData sale in salesData)
            {
            // calculate the total sales and profit for each quarter
            string quarter = GetQuarter(sale.dateSold.Month);
            double totalSales = sale.quantitySold * sale.unitPrice;
            double totalProfit = (sale.unitPrice - sale.baseCost) * sale.quantitySold;

            // add the total sales to the quarterly sales dictionary
            if (quarterlySales.ContainsKey(quarter))
            {
                quarterlySales[quarter] += totalSales;
                quarterlyProfit[quarter] += totalProfit;
            }
            else
            {
                quarterlySales.Add(quarter, totalSales);
                quarterlyProfit.Add(quarter, totalProfit);
            }

            // add the total sales to the quarterly sales by department dictionary
            if (!quarterlySalesByDept.ContainsKey(quarter))
            {
                quarterlySalesByDept[quarter] = new Dictionary<string, double>();
                quarterlyProfitByDept[quarter] = new Dictionary<string, double>();
            }

            if (quarterlySalesByDept[quarter].ContainsKey(sale.departmentName))
            {
                quarterlySalesByDept[quarter][sale.departmentName] += totalSales;
                quarterlyProfitByDept[quarter][sale.departmentName] += totalProfit;
            }
            else
            {
                quarterlySalesByDept[quarter].Add(sale.departmentName, totalSales);
                quarterlyProfitByDept[quarter].Add(sale.departmentName, totalProfit);
            }
            }

            // print the quarterly sales report
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");

            // sort the quarters in order
            List<string> quarters = new List<string> { "Q1", "Q2", "Q3", "Q4" };
            foreach (string quarter in quarters)
            {
            if (quarterlySales.ContainsKey(quarter))
            {
                double sales = quarterlySales[quarter];
                double profit = quarterlyProfit[quarter];
                double profitPercentage = (profit / sales) * 100;
                Console.WriteLine("{0}: Sales: {1:C}, Profit: {2:C}, Profit Percentage: {3:F2}%", quarter, sales, profit, profitPercentage);

                // print the sales and profit by department
                Console.WriteLine("  By Department:");
                foreach (var dept in quarterlySalesByDept[quarter])
                {
                double deptSales = dept.Value;
                double deptProfit = quarterlyProfitByDept[quarter][dept.Key];
                double deptProfitPercentage = (deptProfit / deptSales) * 100;
                Console.WriteLine("    {0}: Sales: {1:C}, Profit: {2:C}, Profit Percentage: {3:F2}%", dept.Key, deptSales, deptProfit, deptProfitPercentage);
                }
            }
            else
            {
                Console.WriteLine("{0}: Sales: $0.00, Profit: $0.00, Profit Percentage: 0.00%", quarter);
            }
            }
        }

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }

        

    }  
}