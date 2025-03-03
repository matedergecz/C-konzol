﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarmuvek
{
    internal class Program
    {
        abstract class Jarmu
        {
            //tulajdonságok(Properties)

            public string Azonosito { get; private set; }
            public string Rendszam { get; set; }
            public int GyartasiEv { get; private set; }
            public double Fogyasztas { get; set; }

            public double FutottKm { get; private set; }
            public int AktualisKoltseg { get; private set; }
            public bool Szabad { get; private set; }

            public static int AktualisEv { get; set; }
            public static int AlapDij { get; set; }
            public static double HaszonKulcs { get; set; }



            // konstruktor (ha már ismerjük a fogyasztást)
            public Jarmu(string azonosito, string rendszam, int gyartasiEv, double fogyasztas)
            {
                this.Azonosito = azonosito;
                this.Rendszam = rendszam;
                this.GyartasiEv = gyartasiEv;
                this.Fogyasztas = fogyasztas;
                this.Szabad = true;
            }

            // konstruktor (ha még nem ismerjük a fogyasztást, pl. vadonatúj a jármű)
            public Jarmu(string azonosito, string rendszam, int gyartasiEv)
            {
                this.Azonosito = azonosito;
                this.Rendszam = rendszam;
                this.GyartasiEv = gyartasiEv;
                this.Szabad = true;
            }

            // azért nem a Foglalt tulajdonságot vezettük be, hogy ezt az értékadást
            // is megmutathassuk.
            // metódusok
            /// <summary>
            /// Kiszámolja a jármű korát.
            /// </summary>
            /// <returns></returns>
            public int Kor()
            {
                return AktualisEv - GyartasiEv;
            }

            /// <summary>
            /// A metódus paraméterében lévő értékkel növeli az eddig megtett kilométerek számát, /// és kiszámolja az aktuális út költségét.
            /// Beállítja, hogy foglalt (nem szabad)
            /// </summary>
            /// <param name="ut"></param>
            // Ha bool típusúként kezeljük, akkor azt is tudjuk figyelni, hogy megvalósult-e a fuvar. // Erre valószínűleg csak akkor "jövünk rá", amikor már a vezérlés részt is
            // végiggondoltuk.
            public bool Fuvaroz(double ut, int benzinAr)
            {
                if (Szabad)
                {
                    FutottKm += ut;

                    AktualisKoltseg = (int)(benzinAr * ut * Fogyasztas / 100);
                    Szabad = false;
                    return true;

                }
                return false;
            }
            /// <summary>
            /// Kiszámolja az alap bérletdíjat.
            /// </summary>
            /// <returns></returns>
            public virtual int BerletDij()
            {
                return (int)(AlapDij + AktualisKoltseg + AktualisKoltseg * HaszonKulcs / 100);
            }



            /// <summary>
            /// Beállítja, hogy szabad
            /// </summary>
            public void Vegzett()
            {
                Szabad = true;
            }


            public override string ToString()
            {
                return $"\nA{this.GetType().Name.ToLower()} azonosítója: {Azonosito}\nkora: {Kor()}\nfogyasztása: {Fogyasztas} 1/100 km\na km-óra állása: {FutottKm} km";
            }

        }

        class Busz : Jarmu
        {

            // tulajdonságok
            public int Ferohely { get; private set; }
            public static double Szorzo { get; set; }



            // konstruktorok
            public Busz(string azonosito, string rendszam, int gyartasiEv, double fogyasztas, int ferohely) :
            base(azonosito, rendszam, gyartasiEv, fogyasztas)
            {
                this.Ferohely = ferohely;
            }

            public Busz(string azonosito, string rendszam, int gyartasiEv, int ferohely) :
                    base(azonosito, rendszam, gyartasiEv)
            {
                this.Ferohely = ferohely;
            }
            //metódusok

            // Kiszámolja a busz bérletdíját.

            public override int BerletDij()
            {
                return (int)(base.BerletDij() + Ferohely * Szorzo);

            }
            public override string ToString()
            {
                return base.ToString() + "\n\tférőhelyek száma: " + Ferohely;
            }
        }

        class TeherAuto : Jarmu
        {
            // tulajdonságok
            public double TeherBiras { get; private set; }
            public static double Szorzo { get; set; }
            // konstruktor
            public TeherAuto(string azonosito, string rendszam, int gyartasiEv,
            double fogyasztas, double teherBiras) :
            base(azonosito, rendszam, gyartasiEv, fogyasztas)
            {
                this.TeherBiras = teherBiras;
            }
            public TeherAuto(string azonosito, string rendszam, int gyartasiEv, double teherBiras) : base(azonosito, rendszam, gyartasiEv)
            {
                this.TeherBiras = teherBiras;
            }

            //metódusok
            // Kiszámolja a teherautó bérletdíját.
            public override int BerletDij()
            {
                return (int)(base.BerletDij() + TeherBiras * Szorzo);
            }

            public override string ToString()
            {
                return base.ToString() + "\n\tteherbírás: " + TeherBiras + "tonna";
            }
        }

        class Vezerles
        {
            private List<Jarmu> jarmuvek = new List<Jarmu>();
            private string BUSZ = "busz";
            private string TEHER_AUTO = "teherautó";
            public void Indit()
            {

                StatikusBeallitas();
                AdatBevitel();
                Kiir("A regisztrált járművek: ");
                Mukodtet();
                Kiir("\nA működés utáni állapot: ");
                AtlagKor();
                Legtobbkilometer();
                Rendez();
            }
            private void StatikusBeallitas()
            {
                // Statikus adatok beállítása, természetesen be is lehetne olvasni.
                Jarmu.AktualisEv = 2015;
                Jarmu.AlapDij = 1000;
                Jarmu.HaszonKulcs = 10;
                Busz.Szorzo = 15;
                TeherAuto.Szorzo = 8.5;
            }


            private void AdatBevitel()
            {
                string tipus, rendszam, azonosito; int gyartEv, ferohely;
                double fogyasztas, teherbiras;
                StreamReader olvasoCsatorna = new StreamReader("C:/txt/jarmuvek.txt");
                int sorszam = 1;
                while (!olvasoCsatorna.EndOfStream)
                {
                    tipus = olvasoCsatorna.ReadLine();
                    Console.WriteLine(tipus);
                    rendszam = olvasoCsatorna.ReadLine();
                    gyartEv = int.Parse(olvasoCsatorna.ReadLine()); fogyasztas = double.Parse(olvasoCsatorna.ReadLine()); azonosito = tipus.Substring(0, 1).ToUpper() + sorszam;
                    if (tipus == BUSZ)
                    {
                        ferohely = int.Parse(olvasoCsatorna.ReadLine());
                        jarmuvek.Add(new Busz(azonosito, rendszam, gyartEv, fogyasztas, ferohely));
                    }
                    else if (tipus == TEHER_AUTO)
                    {

                        teherbiras = double.Parse(olvasoCsatorna.ReadLine()); jarmuvek.Add(new TeherAuto(azonosito, rendszam, gyartEv, fogyasztas, teherbiras));

                    }
                    sorszam++;

                }
                olvasoCsatorna.Close();
            }

            private void Kiir(string cim)
            {
                Console.WriteLine(cim);
                foreach (Jarmu jarmu in jarmuvek)
                {
                    Console.WriteLine(jarmu);
                }

            }

            /// <summary>
            /// Véletlen sokszor ismételjük meg: /// Válasszunk ki véletlenül egy járművet.
            /// Ha ez képes fuvarozni, akkor számoljuk ki
            /// a fuvarok számát,
            /// a cég összes bevételét (véletlen benzinár, véletlen úthossz) /// és az összes költségét.
            /// Egy véletlenszerűen választott jármű végez a fuvarral.
            /// </summary>
            private void Mukodtet()
            {
                // Összetettebb változat
                int osszkoltseg = 0, osszBevetel = 0;




                Random rand = new Random();
                int alsoBenzinAr = 400, felsoBenzinar = 420;
                double utMax = 300;
                int mukodesHatar = 200;
                int jarmuIndex;


                Jarmu jarmu;
                int fuvarSzam = 0;

                for (int i = 0; i < (int)rand.Next(mukodesHatar); i++)
                {
                    jarmuIndex = rand.Next(jarmuvek.Count);
                    jarmu = jarmuvek[jarmuIndex];
                    if (jarmu.Fuvaroz(rand.NextDouble() * utMax, rand.Next(alsoBenzinAr, felsoBenzinar)))
                    {
                        fuvarSzam++;
                        Console.WriteLine("\nAz idnuló jármű rendszáma: " + jarmu.Rendszam + "\nAz aktuális fuvarozási költség: " + jarmu.AktualisKoltseg + "Ft" + "\nAz aktuális bevétel: " + jarmu.BerletDij() + " Ft.");
                        osszBevetel += jarmu.BerletDij();
                        osszkoltseg += jarmu.AktualisKoltseg;
                    }



                    jarmuIndex = rand.Next(jarmuvek.Count);
                    jarmuvek[jarmuIndex].Vegzett();
                }
                Console.WriteLine("\n\nA cég teljes költsége: " + osszkoltseg + "Ft." + "\n\nTeljes bevétele: " + osszBevetel + " Ft." + "\n\nHaszna: " + (osszBevetel - osszkoltseg) + "Ft.");
                Console.WriteLine("\nA fuvarok száma: " + fuvarSzam);
            }
            private void AtlagKor()
            {
                double osszkor = 0;
                int darab = 0;
                foreach (Jarmu jarmu in jarmuvek)
                {
                    osszkor += jarmu.Kor();
                    darab++;
                }

                if (darab > 0)
                {
                    Console.WriteLine("\nA járművek átlag kora: " + osszkor / darab + " év.");
                }
                else
                {
                    Console.WriteLine("Nincsenek járművek.");
                }
            }



            private void Legtobbkilometer()
            {
                double max = jarmuvek[0].FutottKm;
                foreach (Jarmu jarmu in jarmuvek)
                {
                    if (jarmu.FutottKm > max)
                    {
                        max = jarmu.FutottKm;
                    }
                }

                Console.WriteLine("\nA legtöbbet futott jármű(vek) {0: 000.00} km-rel:", max);
                foreach (Jarmu jarmu in jarmuvek)
                {
                    if (jarmu.FutottKm == max)
                    {
                        Console.WriteLine(jarmu.Rendszam);
                    }
                }
            }
            private void Rendez()
            {
                Jarmu temp;
                for (int i = 0; (i < jarmuvek.Count - 1); i++)
                {
                    for (int j = i + 1; (j < jarmuvek.Count); j++)
                    {
                        if (jarmuvek[i].Fogyasztas > jarmuvek[j].Fogyasztas)
                        {
                            temp = jarmuvek[i];
                            jarmuvek[i] = jarmuvek[j];
                            jarmuvek[j] = temp;
                        }
                        Console.WriteLine("\nA járművek fogyasztás szerint rendezve: ");
                        foreach (Jarmu jarmu in jarmuvek)
                        {
                            Console.WriteLine("{0,-10} {1:00.0} liter / 100 km.", jarmu.Rendszam, jarmu.Fogyasztas);
                        }
                    }
                }

            }

        }
        static void Main(string[] args)
        {
            new Vezerles().Indit();

            Console.ReadKey();
        }
    }
}



