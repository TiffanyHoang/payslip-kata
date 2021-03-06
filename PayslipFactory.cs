using System;
using System.Globalization;
using System.Diagnostics;

namespace payslip_kata
{
    class PayslipFactory
    {
        private static readonly PayslipFactory Instance = new PayslipFactory();
        private TaxBracket[] _taxBrackets;
        private DateTime _startDate;
        private DateTime _endDate;

        private PayslipFactory()
        {
        }

        public static PayslipFactory GetInstance()
        {
            return Instance;
        }

        public void SetTaxBrackets(TaxBracket[] brackets)
        {
            this._taxBrackets = brackets;
        }

        public void SetStartDate(DateTime startDate)
        {
            Debug.Assert(startDate > this._endDate, "Start date must be later than end date.");
            this._startDate = startDate;
        }

        public void SetEndDate(DateTime endDate)
        {
            this._endDate = endDate;
        }

        private double GetTimeDivision()
        {
            var monthSpan = this._endDate.Month - this._startDate.Month + 1;
            return 12.0 / monthSpan;
        }

        private double CalculateTax(int salary)
        {
            double tax = 0;
            var remainingSalary = salary;
            var lastThreshold = 0;
            foreach (TaxBracket bracket in _taxBrackets)
            {
                int deductable;
                if (bracket.threshold <= salary)
                {
                    deductable = bracket.threshold - lastThreshold;
                }
                else
                {
                    deductable = remainingSalary;
                }

                double deduction = deductable * bracket.rate;
                tax += deduction;
                remainingSalary -= deductable;
                // Console.WriteLine($"Threshold: {threshold} @ {rate}: Deducted: {deduction}. New Salary: {remainingSalary}, Last Threshold: {lastThreshold}");
                lastThreshold = bracket.threshold;
                if (remainingSalary <= 0) break;
            }

            return Math.Ceiling(tax);
        }

        private string GetFullName(Employee employee)
        {
            return $"{employee.GetName()} {employee.GetSurname()}";
        }

        private int GetGrossIncome(Employee employee)
            // Calculated.
        {
            return employee.GetAnnualSalary();
        }

        private double GetIncomeTax(Employee employee)
            // Calculated.
        {
            return this.CalculateTax(employee.GetAnnualSalary());
        }

        private int GetNetIncome(Employee employee)
            // Derived.
        {
            return this.GetGrossIncome(employee) - (int) Math.Ceiling(this.GetIncomeTax(employee));
        }

        private int GetSuperAmount(Employee employee)
            // Calculated.
        {
            return (int) (this.GetGrossIncome(employee) * employee.GetSuperRate());
        }

        public Payslip GetPayslip(Employee employee)
        {
            Payslip payslip;
            payslip.name = this.GetFullName(employee);
            payslip.grossIncome = (int) (this.GetGrossIncome(employee) / this.GetTimeDivision());
            payslip.incomeTax = (int) Math.Ceiling(this.GetIncomeTax(employee) / this.GetTimeDivision());
            payslip.netIncome = payslip.grossIncome - payslip.incomeTax;
            payslip.superAmount = (int) (payslip.grossIncome * employee.GetSuperRate());

            return payslip;
        }
    }
}