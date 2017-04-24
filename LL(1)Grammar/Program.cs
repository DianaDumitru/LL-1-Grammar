using System;
using System.Collections.Generic;
using System.Linq;

namespace LL_1_Grammar
{
    class Program
    {
        public static Dictionary<int, Production> Productions;
        public static Dictionary<string, Dictionary<string, Production>> Table;
        public static Dictionary<string, string> First;
        public static Dictionary<string, string> Follow;
        public static List<string> Nonterminals;
        public static List<string> Terminals;
        public static string StartSymbol;
        public static string Lambda = "l";
        static void Main(string[] args)
        {
            string path = @"Inputs.txt";
            string line;
            using (System.IO.StreamReader file =
               new System.IO.StreamReader(path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    switch (line)
                    {
                        case "terminal symbols:":
                            line = file.ReadLine();
                            var listOfTerminals = new List<string>(line.Split(' '));
                            Terminals = listOfTerminals;
                            break;
                        case "nonterminal symbols:":
                            line = file.ReadLine();
                            var listOfNonterminals = new List<string>(line.Split(' '));
                            Nonterminals = listOfNonterminals;
                            break;
                        case "productions:":
                            int number = 1;
                            Productions = new Dictionary<int, Production>();
                            while ((line = file.ReadLine()) != null)
                            {
                                List<string> pieces = new List<string>(line.Split(' '));
                                var production = new Production(pieces[0], pieces[1], number);
                                Productions.Add(number, production);
                                number++;
                            }
                            break;
                        case "start symbol:":
                            StartSymbol = file.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Something went wrong while reading the inputs");
                            break;
                    }

                }
            }
            First = new Dictionary<string, string>();
            FindFirstSets();
            Table = new Dictionary<string, Dictionary<string, Production>>();
            foreach (var terminal in Terminals)
            {
                if (!terminal.Equals(Lambda))
                {
                    Table.Add(terminal, new Dictionary<string, Production>());
                    foreach (var nonterminal in Nonterminals)
                    {
                        Table[terminal].Add(nonterminal, null);
                    }
                }
            }

            Follow = new Dictionary<string, string>();

            FindFollowSets();


            CompleteTable();


            //here call to your method




            Console.ReadKey();
        }

        private static void FindFollowSets()
        {
            List<Production> productionsList = new List<Production>();
            foreach (var prod in Productions)
            {
                var production = prod.Value;

                if (production.GoesToRule.Equals(Lambda))
                {
                    var rez = Productions.Where(v => !v.Value.GoesToRule.Equals(Lambda) &&
                    (v.Value.Nonterminal.Equals(production.Nonterminal)
                    || v.Value.GoesToRule.Contains(production.Nonterminal)));
                    foreach (var aux in rez)
                    {
                        char chr = production.Nonterminal.ToCharArray().First();
                        string gtr = String.Concat(aux.Value.GoesToRule.ToArray().Where(c => c != chr));
                        Production p = new Production(aux.Value.Nonterminal, gtr, 0);
                        productionsList.Add(p);
                    }

                }
                else
                {
                    productionsList.Add(production);
                }
                if (!Follow.ContainsKey(prod.Value.Nonterminal))
                {
                    Follow.Add(prod.Value.Nonterminal, String.Empty);
                }
            }
            Follow[StartSymbol] = "$";

            foreach (var production in productionsList)
            {
                for (int i = 0; i < production.GoesToRule.Length; i++)
                {
                    string symbol = production.GoesToRule[i].ToString();
                    if (Nonterminals.Contains(symbol))
                    {
                        string nextSymbol = (i < production.GoesToRule.Length - 1) ?
                                            production.GoesToRule[i + 1].ToString() :
                                            Lambda;

                        if (nextSymbol.Equals(Lambda))
                        {
                            Follow[symbol] += Follow[production.Nonterminal];
                        }
                        else
                        {
                            string M;
                            if (Terminals.Contains(nextSymbol))
                                M = nextSymbol;
                            else
                                M = First[nextSymbol];
                            if (M.Contains(Lambda))
                            {
                                string str = String.Empty;
                                foreach (var c in M)
                                {
                                    if (c != 'l')
                                        str += c;
                                }
                                Follow[symbol] += str + Follow[production.Nonterminal];
                            }
                            else
                            {
                                Follow[symbol] += M;
                            }

                        }
                    }
                }
            }




        }

        private static void CompleteTable()
        {
            foreach (var pair in Productions)
            {
                var currentProduction = pair.Value;
                string symbol = currentProduction.GoesToRule[0].ToString();
                if (Terminals.Contains(symbol) && !symbol.Equals(Lambda))
                {
                    Table[symbol][currentProduction.Nonterminal] = currentProduction;
                }
                else
                {
                    if (Nonterminals.Contains(symbol))
                    {
                        foreach (var ch in First[symbol])
                            Table[ch.ToString()][currentProduction.Nonterminal] = currentProduction;
                    }
                    else
                    {
                        foreach (var ch in Follow[symbol])
                            Table[ch.ToString()][currentProduction.Nonterminal] = currentProduction;
                    }

                }
            }
        }

        private static void FindFirstSets()
        {
            Queue<Production> aux = new Queue<Production>();
            foreach (var prod in Productions)
            {

                aux.Enqueue(prod.Value);
            }

            while (aux.Count != 0)
            {
                var currentProd = aux.Dequeue();


                if (Terminals.Contains(currentProd.GoesToRule[0].ToString()) == true)
                {
                    AddFirstElement(currentProd, currentProd.GoesToRule[0].ToString());
                }
                else
                {
                    if (First.ContainsKey(currentProd.GoesToRule[0].ToString()))
                    {
                        AddFirstElement(currentProd, First[currentProd.GoesToRule[0].ToString()]);
                    }
                    else
                    {
                        aux.Enqueue(currentProd);
                    }
                }

            }
        }

        private static void AddFirstElement(Production currentProd, string v)
        {
            if (First.ContainsKey(currentProd.Nonterminal))
                First[currentProd.Nonterminal] += v;
            else
                First.Add(currentProd.Nonterminal, v);
        }
    }

}
