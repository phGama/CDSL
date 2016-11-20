using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC.Helpers
{
    public static class CPFValidator
    {

        public static bool Validate(string CPF)
        {
            if (CPF.Length != 11)
                return false;
            if (CPF.All(x => x == CPF[0]) || CPF == "12345678909")
                return false;


            int[] Digits = new int[11];
            for (int i = 0; i < 11; i++)
                Digits[i] = int.Parse(CPF[i].ToString());

            var FirstDigitSum = 0;
            var SecoundDigitSum = 0;
            int FirstSafetyDigit = 0;
            int SecoundSafetyDigit = 0;

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                    FirstDigitSum += (10 - i) * Digits[i];
                SecoundDigitSum += (11 - i) * Digits[i];
            }


            FirstSafetyDigit = FirstDigitSum % 11;
            SecoundSafetyDigit = SecoundDigitSum % 11;

            if (FirstSafetyDigit == 1 || FirstSafetyDigit == 0)
            {
                if (Digits[9] != 0)
                    return false;
            }
            else if (Digits[9] != 11 - FirstSafetyDigit)
                return false;

            if (SecoundSafetyDigit == 1 || SecoundSafetyDigit == 0)
            {
                if (Digits[10] != 0)
                    return false;
            }
            else
                if (Digits[10] != 11 - SecoundSafetyDigit)
                    return false;

            return true;

        }

    }

}
