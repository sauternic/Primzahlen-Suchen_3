using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


//Program_UInt64_P_For_Vorselektion.cs
namespace Primzahlen
{
    class CPrimzahlen
    {
        //16 Stellen:   1000000000000000
        //17 Stellen:   10000000000000000
        //20 Stellen:   10000000000000000000
        //21 Stellen :  100000000000000000000
        //Max ulong =   18446744073709551615;
        //Max decimal = 79228162514264337593543950335M;

        //Field
        static Stopwatch s = new Stopwatch();
        static int P_Nr = 0;

        static void Main()
        {
            ulong anfang = 0;
            ulong ende = 0;

            Console.Write("\n   Program_UInt64_P_For_Vorselektion\n\n");
            Console.Write("\n\n   Primzahlenauflisten!\n\n");
            Console.Write("\n   Untere Grenze Eingeben? ");
            anfang = Convert.ToUInt64(Console.ReadLine());

            Console.Write("   Obere  Grenze Eingeben? ");
            ende = Convert.ToUInt64(Console.ReadLine());
            Console.WriteLine();//Leerzeile

            SuchePrimzahlen(anfang, ende);
        }

        static void SuchePrimzahlen(ulong anfang, ulong ende)
        {
            label1:
            s.Start();
            //Aeussere Schleife Ersetzt Hand Eingabe.
            for (; anfang <= ende; anfang++)
            {
                s.Start();

                //Geht bis 19 Stellen!! :O
                long Wurzel_anfang = (long)Math.Pow(anfang, 0.5);

                //Exklusiv zu Inklusiv
                ++Wurzel_anfang;

                //********Abbruch der Parallel.For***********************************
                CancellationTokenSource cts = new CancellationTokenSource();
                // Use ParallelOptions instance to store the CancellationToken

                ParallelOptions po = new ParallelOptions();
                po.CancellationToken = cts.Token;
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
                //*******************************************************************

                try
                {
                    // Neue Primzahlen Engine! :)
                    Parallel.For(2L, Wurzel_anfang, po, (teiler) =>
                    {
                        //2 Einzig Gerade Primzahl darum (... || teiler == 2) gibt true bei 2!!!!
                        //Das ist am Schnellsten nur gerade Zahlen weglassen
                        if (teiler % 2 != 0 || teiler == 2)
                        {
                            //Wenn (anfang mod i == 0) dann ist keine Primzahl
                            //Beachte Typ Castin von long zu (ulong)!!!!!!!!!!!! 
                            if (anfang % (ulong)teiler == 0)
                            {
                                cts.Cancel();
                            }
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    ++anfang;
                    goto label1;
                }
                finally
                {
                    cts.Dispose();
                }

                // 1 und 0 rausputzen! ;)
                if (anfang == 0 || anfang == 1)
                    continue;

                s.Stop();
                TimeSpan timeSpan = s.Elapsed;

                ++P_Nr;
                //Ausgabe
                string ausgString = String.Format("\nPrimzahl {0} :)  {1:#,#}\n", P_Nr, anfang);
                ausgString += String.Format("Time: {0}h {1}m {2}s {3}ms\n", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

                Console.WriteLine(ausgString);
            }

            Console.WriteLine("Fertig! :)");
            Console.WriteLine("\n\tCopyright © Nicolas Sauter");

            //Programm wird beendet nach Tastendruck:
            Console.ReadKey();
        }



    }
}
