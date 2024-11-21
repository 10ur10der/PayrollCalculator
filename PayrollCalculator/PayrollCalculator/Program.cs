
decimal grossSalaryFirstHalf = 20002.5m; // İlk 6 ay için brüt maaş
decimal grossSalarySecondHalf = 20002.5m; // İkinci 6 ay için brüt maaş

var salaryCalculator = new SalaryCalculator();
salaryCalculator.CalculateAnnualSalary(grossSalaryFirstHalf, grossSalarySecondHalf);

public class Salary
{
    public decimal GrossSalary { get; private set; }
    public decimal SskEmployeeDeduction { get; private set; }
    public decimal UnemploymentEmployeeDeduction { get; private set; }
    public decimal TaxableIncome { get; private set; }

    private readonly decimal sskEmployeeRate = 0.14m;
    private readonly decimal unemploymentEmployeeRate = 0.01m;

    public Salary(decimal grossSalary)
    {
        GrossSalary = grossSalary;
        CalculateDeductions();
    }

    private void CalculateDeductions()
    {
        // SSK ve İşsizlik sigortası kesintilerini hesapla
        SskEmployeeDeduction = GrossSalary * sskEmployeeRate;
        UnemploymentEmployeeDeduction = GrossSalary * unemploymentEmployeeRate;
        TaxableIncome = GrossSalary - SskEmployeeDeduction - UnemploymentEmployeeDeduction;
    }
}

public class TaxCalculator
{
    private readonly List<(decimal upperLimit, decimal rate)> taxBrackets = new List<(decimal, decimal)>
        {
            (110000, 0.15m),
            (230000, 0.20m),
            (870000, 0.27m),
            (3000000, 0.35m),
            (decimal.MaxValue, 0.40m)
        };

    public decimal CalculateIncomeTax(decimal cumulativeIncome)
    {
        decimal monthlyIncomeTax = 0;
        decimal remainingIncome = cumulativeIncome;
        decimal previousLimit = 0;

        foreach (var bracket in taxBrackets)
        {
            if (remainingIncome > 0)
            {
                decimal taxableInBracket = Math.Min(remainingIncome, bracket.upperLimit - previousLimit);
                monthlyIncomeTax += taxableInBracket * bracket.rate;
                remainingIncome -= taxableInBracket;
            }
            previousLimit = bracket.upperLimit;

            if (remainingIncome <= 0) break;
        }

        return monthlyIncomeTax;
    }
}

public class SalaryCalculator
{
    private readonly decimal asgariUcret = 17002.12m;
    private readonly decimal asgariVergiIstisnasi;
    private decimal cumulativeTaxPaid = 0;
    private decimal netAnnualIncome = 0;
    private decimal totalTaxableIncome = 0;

    private readonly Salary salary;
    private readonly TaxCalculator taxCalculator;

    public SalaryCalculator()
    {
        asgariVergiIstisnasi = asgariUcret * 0.15m;
        taxCalculator = new TaxCalculator();
    }

    public void CalculateAnnualSalary(decimal grossSalaryFirstHalf, decimal grossSalarySecondHalf)
    {
        for (int month = 0; month < 12; month++)
        {
            // İlk 6 ay için brüt maaş
            decimal grossSalary = month < 6 ? grossSalaryFirstHalf : grossSalarySecondHalf;

           var salary = new Salary(grossSalary); // Salary nesnesi oluşturuluyor

            // Kümülatif gelir hesaplama
            totalTaxableIncome += salary.TaxableIncome;

            // Gelir Vergisini Hesapla
            decimal monthlyIncomeTax = taxCalculator.CalculateIncomeTax(totalTaxableIncome) - cumulativeTaxPaid;

            // Damga vergisi
            decimal stampTax = grossSalary * 0.00759m;

            // Net maaş hesapla
            decimal netSalary = grossSalary - (salary.SskEmployeeDeduction + salary.UnemploymentEmployeeDeduction + monthlyIncomeTax + stampTax);
            netAnnualIncome += netSalary;
            cumulativeTaxPaid += monthlyIncomeTax;

            // Aylık maaş çıktılarını yazdır
            PrintMonthlyResults(month + 1, grossSalary, netSalary, monthlyIncomeTax);
        }

        // Yıllık toplam net maaşı yazdır
        Console.WriteLine($"\nYıllık Toplam Net Maaş: {netAnnualIncome:C}");
    }

    private void PrintMonthlyResults(int month, decimal grossSalary, decimal netSalary, decimal monthlyIncomeTax)
    {
        Console.WriteLine($"Ay {month}: Brüt: {grossSalary:C}, Net: {netSalary:C}, Vergi: {monthlyIncomeTax:C}");
    }
}

