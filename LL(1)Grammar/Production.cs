namespace LL_1_Grammar
{
    class Production
    {
        public Production()
        {

        }

        public int Number { get; set; }

        public Production(string nonterminal, string goesToRule, int number)
        {
            this.Nonterminal = nonterminal;
            this.GoesToRule = goesToRule;
            this.Number = number;
        }

        public string Nonterminal { get; set; }
        /// <summary>
        /// the right part of a production
        /// </summary>
        public string GoesToRule { get; set; }
    }

}
