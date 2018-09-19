using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int A = int.Parse(inputs[0]);
        int B = int.Parse(inputs[1]);

        //Dzielniki A
        Dictionary<int,int> DividersA  = new Dictionary<int, int>();
        for (int i = 2; i <= A; i++)
        {
            if(A%i == 0)
                DividersA.Add(i, 0);
            
            while(A%i == 0)
            {
                DividersA[i]++;
                A = A / i;
            }
        }

        Dictionary<int,int> DividersB  = new Dictionary<int, int>();
        foreach (var key in DividersA.Keys)
        {
            DividersB.Add(key, 0);
        }
        
        // sprawdaznie dla każdej liczby, ile dzielników
        for (int i = 2; i <= B; i++)
        {
            int j = i;
            foreach (var key in DividersA.Keys)
            {
                while(j%key == 0)
                {
                    DividersB[key] ++;
                    j = j / key;
                }
            }
        }

        int answer = 0;
        bool loop = true;

        while(loop)
        {
            foreach (var key in DividersA.Keys)
            {
                DividersB[key] -= DividersA[key];
                if(DividersB[key] < 0)
                {
                    loop = false;
                    break;
                }
            }
            if(loop)
                answer++;   

        }
        Console.WriteLine(answer);
    }
}