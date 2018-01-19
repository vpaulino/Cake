using System;

namespace AppLogic
{
    public class Calculator
    {
        public decimal Add(decimal operand1, decimal operand2)
        {
            return operand1 + operand2;
        }

        public decimal Subtract(decimal operand1, decimal operand2)
        {
            return operand1 - operand2;
        }

        public decimal Divide(decimal operand1, decimal operand2)
        {
            return operand1 / operand2;
        }

        public decimal Multiply(decimal operand1, decimal operand2)
        {
            return operand1 * operand2;
        }

    }
}
