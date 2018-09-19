using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
/**
* Auto-generated code below aims at helping you parse
* the standard input according to the problem statement.
**/
class Solution
{
    static void Main(string[] args)
    {
        int n = int.Parse(Console.ReadLine());
        Console.Error.WriteLine(n);
        CultureInfo ci = new CultureInfo("en-us");
        //pierwsze sprawdzenie
        int k = n;
        while(k % 2 == 0)
            k = k / 2;
        while(k%5 == 0)
            k = k / 5;
        
        if(k == 1)
        {
            string answer = ((double)1/n).ToString("f20", ci);
            for(int i=answer.Length-1; i>2; i--)
            {
                if(answer[i] == '0')
                    answer = answer.Substring(0,i);
                else
                    break;
            }
            Console.WriteLine(answer);    
            return;
        }
        
        int[] x = new int[100];
        
        int dzielnik = 10;
        for (int i = 0; i < x.Length; i++)
        {
            x[i] = dzielnik / n;
            dzielnik = (dzielnik - x[i] * n) * 10;
        }
        
        var s = String.Join("", x);
        string r;
        TryGetRepeatedDigits(s, out r);

        Regex rx = new Regex(r);
        Match m = rx.Match(s);
        int idx = m.Index;

    }
    public static bool TryGetRepeatedDigits(string digitSequence, out string repeatedDigits)
{
    repeatedDigits = null;
    string pattern = @"^\d*(?<repeat>\d+)\k<repeat>+";
    //string pattern = @"^\d*(?<repeat>\d+)\k<repeat>+\d*$";

    if (Regex.IsMatch(digitSequence, pattern))
    {
        Regex r = new Regex(pattern);
        repeatedDigits = r.Match(digitSequence).Result("${repeat}");

        return true;
    }
    else
        return false;
}
}